using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;

namespace RabbitMqCommon.Impl
{
    internal class RabbitMqServer : IServer, IDisposable
    {
        public RabbitMqServer(IServiceProvider provider, string host, string receiveQueueName)
        {
            var factory = new ConnectionFactory();
            Connection = factory.CreateConnection(host);
            var codec = provider.GetRequiredService<ICodec>();
            var dispatcher = provider.GetRequiredService<RequestDispatcher>();
            InternalPublisher = new RabbitMqPublisher(Connection, codec);
            RequestHandler = new RabbitMqRequestHandler(Connection, codec, receiveQueueName, dispatcher);
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
