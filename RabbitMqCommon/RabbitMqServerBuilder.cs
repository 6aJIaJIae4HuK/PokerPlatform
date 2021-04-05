using System;

namespace RabbitMqCommon
{
    public class RabbitMqServerBuilder
    {
        public RabbitMqServerBuilder(string host, string receiveQueueName, ICodec codec)
        {
            Host = host;
            ReceiveQueueName = receiveQueueName;
            Codec = codec;
        }

        public RabbitMqServerBuilder AddRequestHandler<TRequest, TReply>(Func<ICodec, IPublisher, TypedRequestHandler<TRequest, TReply>> factory)
            where TRequest : new()
            where TReply : new()
        {
            DispatcherBuilder.AddRegistrator(factory);
            return this;
        }

        public IServer Build()
        {
            return new Impl.RabbitMqServer(Host, ReceiveQueueName, Codec, DispatcherBuilder);
        }

        private readonly string Host;
        private readonly string ReceiveQueueName;
        private readonly ICodec Codec;
        private readonly Impl.RequestDispatcherBuilder DispatcherBuilder = new Impl.RequestDispatcherBuilder();
    }
}
