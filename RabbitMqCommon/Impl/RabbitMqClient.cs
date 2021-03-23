using RabbitMQ.Client;
using System;

namespace RabbitMqCommon.Impl
{
    internal class RabbitMqClient : IClient, IDisposable
    {
        public RabbitMqClient(string host, string sendQueueName, ICodec codec)
        {
            var factory = new ConnectionFactory();
            Connection = factory.CreateConnection(host);
            Channel = Connection.CreateModel();
            Requester = new RabbitMqRequester(Channel, sendQueueName, codec);
            Subscriber = new RabbitMqSubscriber(Channel, codec);
        }

        public void Dispose()
        {
            Channel.Dispose();
            Connection.Dispose();
        }

        public readonly IConnection Connection;
        public readonly IModel Channel;
        public IRequester Requester { get; }
        public ISubscriber Subscriber { get; }
    }
}
