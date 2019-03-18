package main

import (
	"context"
	"io"
	"strings"

	"google.golang.org/grpc/codes"
	"google.golang.org/grpc/status"

	"github.com/pkg/errors"
	"github.com/uw-labs/proximo/proto"
	"github.com/uw-labs/substrate"
	"github.com/uw-labs/sync/gogroup"
)

type producerConfig struct {
	topic string
}

type AsyncSinkFactory interface {
	NewAsyncSink(ctx context.Context, conf producerConfig) (substrate.AsyncMessageSink, error)
}

type SinkServer struct {
	sinkFactory AsyncSinkFactory
}

func (s *SinkServer) Publish(stream proto.MessageSink_PublishServer) error {
	sCtx := stream.Context()

	g, ctx := gogroup.New(sCtx)

	messages := make(chan substrate.Message)
	acks := make(chan substrate.Message)
	startRequest := make(chan *proto.StartPublishRequest)

	g.Go(func() error {
		return s.receiveMessages(ctx, stream, startRequest, messages)
	})
	g.Go(func() error {
		return s.sendConfirmations(ctx, stream, acks)
	})
	g.Go(func() error {
		var conf producerConfig
		select {
		case sr := <-startRequest:
			conf.topic = sr.GetTopic()
		case <-ctx.Done():
			return nil
		}

		sink, err := s.sinkFactory.NewAsyncSink(ctx, conf)
		if err != nil {
			return err
		}
		defer sink.Close()

		return sink.PublishMessages(ctx, acks, messages)
	})

	if err := g.Wait(); err != nil {
		return err
	}

	if err := sCtx.Err(); err == context.Canceled {
		return status.Error(codes.Canceled, err.Error())
	}
	return sCtx.Err()
}

// receiveSinkStream is a subset of proto.MessageSink_PublishServer that only exposes the receive method
type receiveSinkStream interface {
	Recv() (*proto.PublisherRequest, error)
}

// receiveMessages receives messages from the client
func (s *SinkServer) receiveMessages(ctx context.Context, stream receiveSinkStream, startRequest chan<- *proto.StartPublishRequest, messages chan<- substrate.Message) error {
	started := false
	for {
		msg, err := stream.Recv()
		if err != nil {
			if err == io.EOF {
				return nil
			}
			if strings.HasSuffix(err.Error(), "context canceled") {
				return nil
			}
			return err
		}
		switch {
		case msg.GetStartRequest() != nil:
			if started {
				return errStartedTwice
			}
			select {
			case startRequest <- msg.GetStartRequest():
			case <-ctx.Done():
				return nil
			}
			started = true
		case msg.GetMsg() != nil:
			if !started {
				return errNotConnected
			}
			select {
			case messages <- proximoMsg{msg: msg.GetMsg()}:
			case <-ctx.Done():
				return nil
			}
		default:
			return errInvalidRequest
		}
	}
}

// sendSinkStream is a subset of proto.MessageSink_PublishServer that only exposes the send method
type sendSinkStream interface {
	Send(*proto.Confirmation) error
}

// sendConfirmations sends confirmations back to the client
func (s *SinkServer) sendConfirmations(ctx context.Context, stream sendSinkStream, forClient <-chan substrate.Message) error {
	for {
		select {
		case msg := <-forClient:
			pMsg, ok := msg.(proximoMsg)
			if !ok {
				return errors.Errorf("unexpected message: %v", pMsg)
			}
			if err := stream.Send(&proto.Confirmation{MsgID: pMsg.msg.Id}); err != nil {
				return err
			}
		case <-ctx.Done():
			return nil
		}
	}
}

type proximoMsg struct {
	msg *proto.Message
}

func (m proximoMsg) Data() []byte {
	return m.msg.Data
}
