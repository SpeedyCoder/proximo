package natsstreaming

import (
	"context"
	"time"

	"github.com/uw-labs/proximo/proto"
	"github.com/uw-labs/substrate"
	"github.com/uw-labs/substrate/natsstreaming"
)

type AsyncSourceFactory struct {
	URL                    string
	ClientID               string
	ClusterID              string
	AckWait                time.Duration
	MaxInflight            int
	ConnectionNumPings     int
	ConnectionPingInterval int
}

func (f AsyncSourceFactory) NewAsyncSource(ctx context.Context, req *proto.StartConsumeRequest) (substrate.AsyncMessageSource, error) {
	var offset int64
	switch req.GetInitialOffset() {
	case proto.Offset_OFFSET_OLDEST, proto.Offset_OFFSET_DEFAULT:
		offset = natsstreaming.OffsetOldest
	case proto.Offset_OFFSET_NEWEST:
		offset = natsstreaming.OffsetNewest
	}
	return natsstreaming.NewAsyncMessageSource(natsstreaming.AsyncMessageSourceConfig{
		URL:                    f.URL,
		ClientID:               f.ClientID,
		ClusterID:              f.ClusterID,
		Subject:                req.GetTopic(),
		QueueGroup:             req.GetConsumer(),
		Offset:                 offset,
		AckWait:                f.AckWait,
		MaxInFlight:            f.MaxInflight,
		ConnectionNumPings:     f.ConnectionNumPings,
		ConnectionPingInterval: f.ConnectionPingInterval,
	})
}

type AsyncSinkFactory struct {
	URL       string
	ClientID  string
	ClusterID string
}

func (f AsyncSinkFactory) NewAsyncSink(ctx context.Context, req *proto.StartPublishRequest) (substrate.AsyncMessageSink, error) {
	return natsstreaming.NewAsyncMessageSink(natsstreaming.AsyncMessageSinkConfig{
		URL:       f.URL,
		ClientID:  f.ClientID,
		ClusterID: f.ClusterID,
		Subject:   req.GetTopic(),
	})
}
