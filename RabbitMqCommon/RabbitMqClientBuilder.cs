using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMqCommon
{
    public class RabbitMqClientBuilder
    {
        public RabbitMqClientBuilder(string host, string sendQueueName, ICodec codec)
        {
            Host = host;
            SendQueueName = sendQueueName;
            Codec = codec;
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
