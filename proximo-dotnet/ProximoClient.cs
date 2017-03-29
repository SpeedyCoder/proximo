﻿using Google.Protobuf;
using Grpc.Core;
using Proximo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace proximo_dotnet
{
    /// <summary>
    /// Client code that consumes messages from the server.
    /// </summary>
    public class ConsumerClient : IConsumerClient
    {
        readonly MessageSource.MessageSourceClient _client;
        readonly string _clientId;
        readonly string _topic;

        public ConsumerClient(MessageSource.MessageSourceClient messageSourceClient, string clientId, string topic)
        {
            _client = messageSourceClient;
            _clientId = clientId;
            _topic = topic;
        }

        /// <summary>
        /// Consume messages from proximo server and adds them to an in-memory queue
        /// </summary>
        /// <param name="messagesQueue">The in-memory queue. (id, message, time spent)</param>
        /// /// <param name="cancellationToken">The cancellation token.</param>
        public async Task ConsumeMessages(List<(string, string, double)> messagesQueue, CancellationToken cancellationToken)
        {
            var confirmations = new Queue<Message>();
            try
            {
                using (var call = _client.Consume())
                {
                    Stopwatch sw = null;
                    Action<Message> consumerRequestAction = (async (Message confMsg) =>
                    {
                        //Confirmm message was received.
                        var cr = new ConsumerRequest
                        {
                            Confirmation = new Confirmation { MsgID = confMsg.Id }
                        };

                        try
                        {
                            await call.RequestStream.WriteAsync(cr);
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    });

                    var responseReaderTask = Task.Run(() =>
                    {
                        //Reading messages
                        try
                        {
                            while (call.ResponseStream.MoveNext(cancellationToken).Result)
                            {
                                sw = Stopwatch.StartNew();

                                var confirm = call.ResponseStream.Current;
                                confirmations.Enqueue(confirm);
                                var data = confirm.Data.ToString(Encoding.UTF8);

                                var consumerRequestTask = new Task(r => consumerRequestAction(confirm), cancellationToken);
                                consumerRequestTask.RunSynchronously();

                                //add to local queue
                                messagesQueue.Add((confirm.Id, data, sw.Elapsed.TotalSeconds));
                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    });


                    var scr = new ConsumerRequest
                    {
                        StartRequest = new StartConsumeRequest
                        {
                            Consumer = _clientId,
                            Topic = _topic
                        }
                    };

                    await call.RequestStream.WriteAsync(scr);
                    await responseReaderTask;
                }
            }
            catch (RpcException e)
            {
                throw e;
            }
        }
    }


    /// <summary>
    /// Client publisher that makes gRPC calls to the server.
    /// </summary>
    public class PublisherClient : IPublisherClient
    {
        readonly MessageSink.MessageSinkClient _client;
        readonly string _clientId;
        readonly string _topic;

        public PublisherClient(MessageSink.MessageSinkClient messageSinkClient, string clientId, string topic)
        {
            _client = messageSinkClient;
            _clientId = clientId;
            _topic = topic;
        }

        /// <summary>
        /// Publish messages to proximo server and adds the confirmation ids to an in-memory queue
        /// </summary>
        /// <param name="messagesList">A list of string messages</param>
        /// <param name="receiveQueue">The in-memory queue.</param>
        public async Task PublishMessages((string, string) message, Queue<string> receiveQueue)
        {
            (string, byte[]) converted = (message.Item1, Encoding.UTF8.GetBytes(message.Item2));

            await PublishMessages(converted, receiveQueue);
        }

        /// <summary>
        /// Publish messages to proximo server and adds the confirmation ids to an in-memory queue
        /// </summary>
        /// <param name="messagesList">A list of byte[] messages</param>
        /// <param name="receiveQueue">The in-memory queue.</param>
        public async Task PublishMessages((string, byte[]) message, Queue<string> receiveQueue)
        {
            try
            {
                var request = new Proximo.Message
                {
                    Id = message.Item1,
                    Data = ByteString.CopyFrom(message.Item2)
                };

                using (var call = _client.Publish(new CallOptions { }))
                {
                    var responseReaderTask = Task.Run(() =>
                    {
                        try
                        {
                            while (call.ResponseStream.MoveNext().Result)
                            {
                                var confirm = call.ResponseStream.Current;
                                receiveQueue.Enqueue(confirm.MsgID);
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }

                    });

                    var spr = new PublisherRequest
                    {
                        StartRequest = new StartPublishRequest { Topic = _topic }
                    };
                    await call.RequestStream.WriteAsync(spr);


                    var pr = new PublisherRequest
                    {
                        Msg = request
                    };

                    try
                    {
                        await call.RequestStream.WriteAsync(pr);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    await call.RequestStream.CompleteAsync();
                    await responseReaderTask;
                }
            }
            catch (RpcException e)
            {
                throw e;
            }
        }
    }
}
