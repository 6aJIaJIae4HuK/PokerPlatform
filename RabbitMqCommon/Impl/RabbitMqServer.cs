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
            InternalPublisher = new RabbitMqPublisher(Connection, codec);
            RequestHandler = new RabbitMqRequestHandler(Connection, codec, receiveQueueName, dispatcherBuilder.Build(codec, Publisher));
        }

        public void Dispose()
        {
            RequestHandler.Dispose();
            InternalPublisher.Dispose();
            Connection.Dispose();
        }

        public IPublisher Publisher => InternalPublisher;

        private readonly IConnection Connection;
        private readonly RabbitMqPublisher InternalPublisher;
        private readonly RabbitMqRequestHandler RequestHandler;
    }
}
