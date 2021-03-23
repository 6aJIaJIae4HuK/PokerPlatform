using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMqCommon
{
    public class RabbitMqClientBuilder
    {
        public RabbitMqClientBuilder(string host, string sendQueueName)
        {
            Host = host;
            SendQueueName = sendQueueName;
            Codec = new Impl.ProtobufCodec();
        }

        public RabbitMqClientBuilder AddType<T>(int typeId)
        {
            Codec.RegisterType<T>(typeId);
            return this;
        }

        public IClient Build()
        {
            return new Impl.RabbitMqClient(Host, SendQueueName, Codec);
        }

        private readonly string Host;
        private readonly string SendQueueName;
        private readonly ICodec Codec;
    }
}
