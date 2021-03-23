using RabbitMQ.Client;
using System;

namespace RabbitMqCommon.Impl
{
    internal class RabbitMqServer : IServer, IDisposable
    {
        public RabbitMqServer(string host, string receiveQueueName, ICodec codec, RequestDispatcherBuilder dispatcherBuilder)
        {
            var factory = new ConnectionFactory();
            Connection = factory.CreateConnection(host);
            Channel = Connection.CreateModel();
            Publisher = new RabbitMqPublisher();
            RequestHandler = new RabbitMqRequestHandler(Channel, codec, receiveQueueName, dispatcherBuilder.Build(codec, Publisher));
        }

        public void Dispose()
        {
            RequestHandler.Dispose();
            Channel.Dispose();
            Connection.Dispose();
        }

        public IPublisher Publisher { get; }

        private readonly IConnection Connection;
        private readonly IModel Channel;
        private readonly RabbitMqRequestHandler RequestHandler;
    }
}
