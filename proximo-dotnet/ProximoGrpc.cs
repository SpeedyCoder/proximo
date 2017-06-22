// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: proximo.proto
#region Designer generated code

using System;
using System.Threading;
using System.Threading.Tasks;
using grpc = global::Grpc.Core;

namespace Proximo {
  /// <summary>
  /// Consumer types
  /// </summary>
  public static partial class MessageSource
  {
    static readonly string __ServiceName = "proximo.MessageSource";

    static readonly grpc::Marshaller<global::Proximo.ConsumerRequest> __Marshaller_ConsumerRequest = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Proximo.ConsumerRequest.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::Proximo.Message> __Marshaller_Message = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Proximo.Message.Parser.ParseFrom);

    static readonly grpc::Method<global::Proximo.ConsumerRequest, global::Proximo.Message> __Method_Consume = new grpc::Method<global::Proximo.ConsumerRequest, global::Proximo.Message>(
        grpc::MethodType.DuplexStreaming,
        __ServiceName,
        "Consume",
        __Marshaller_ConsumerRequest,
        __Marshaller_Message);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Proximo.ProximoReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of MessageSource</summary>
    public abstract partial class MessageSourceBase
    {
      public virtual global::System.Threading.Tasks.Task Consume(grpc::IAsyncStreamReader<global::Proximo.ConsumerRequest> requestStream, grpc::IServerStreamWriter<global::Proximo.Message> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for MessageSource</summary>
    public partial class MessageSourceClient : grpc::ClientBase<MessageSourceClient>
    {
      /// <summary>Creates a new client for MessageSource</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public MessageSourceClient(grpc::Channel channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for MessageSource that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public MessageSourceClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected MessageSourceClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected MessageSourceClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      public virtual grpc::AsyncDuplexStreamingCall<ConsumerRequest, Message> Consume(grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return Consume(new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncDuplexStreamingCall<ConsumerRequest, Message> Consume(grpc::CallOptions options)
      {
        return CallInvoker.AsyncDuplexStreamingCall(__Method_Consume, null, options);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override MessageSourceClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new MessageSourceClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(MessageSourceBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_Consume, serviceImpl.Consume).Build();
    }

  }
  /// <summary>
  /// Producer types
  /// </summary>
  public static partial class MessageSink
  {
    static readonly string __ServiceName = "proximo.MessageSink";

    static readonly grpc::Marshaller<global::Proximo.PublisherRequest> __Marshaller_PublisherRequest = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Proximo.PublisherRequest.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::Proximo.Confirmation> __Marshaller_Confirmation = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Proximo.Confirmation.Parser.ParseFrom);

    static readonly grpc::Method<global::Proximo.PublisherRequest, global::Proximo.Confirmation> __Method_Publish = new grpc::Method<global::Proximo.PublisherRequest, global::Proximo.Confirmation>(
        grpc::MethodType.DuplexStreaming,
        __ServiceName,
        "Publish",
        __Marshaller_PublisherRequest,
        __Marshaller_Confirmation);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Proximo.ProximoReflection.Descriptor.Services[1]; }
    }

    /// <summary>Base class for server-side implementations of MessageSink</summary>
    public abstract partial class MessageSinkBase
    {
      public virtual global::System.Threading.Tasks.Task Publish(grpc::IAsyncStreamReader<global::Proximo.PublisherRequest> requestStream, grpc::IServerStreamWriter<global::Proximo.Confirmation> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for MessageSink</summary>
    public partial class MessageSinkClient : grpc::ClientBase<MessageSinkClient>
    {
      /// <summary>Creates a new client for MessageSink</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public MessageSinkClient(grpc::Channel channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for MessageSink that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public MessageSinkClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected MessageSinkClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected MessageSinkClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      public virtual grpc::AsyncDuplexStreamingCall<PublisherRequest, Confirmation> Publish(grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return Publish(new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncDuplexStreamingCall<PublisherRequest, Confirmation> Publish(grpc::CallOptions options)
      {
        return CallInvoker.AsyncDuplexStreamingCall(__Method_Publish, null, options);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override MessageSinkClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new MessageSinkClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(MessageSinkBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_Publish, serviceImpl.Publish).Build();
    }

  }
}
#endregion