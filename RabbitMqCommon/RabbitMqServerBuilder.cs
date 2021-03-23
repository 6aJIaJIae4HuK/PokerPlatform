using System;

namespace RabbitMqCommon
{
    public class RabbitMqServerBuilder
    {
        public RabbitMqServerBuilder(string host, string receiveQueueName)
        {
            Host = host;
            ReceiveQueueName = receiveQueueName;
            Codec = new Impl.ProtobufCodec();
        }

        public RabbitMqServerBuilder AddType<T>(int typeId)
        {
            Codec.RegisterType<T>(typeId);
            return this;
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
        private readonly Impl.RequestDispatcherBuilder DispatcherBuilder;
    }
}
