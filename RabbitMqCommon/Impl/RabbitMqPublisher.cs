using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace RabbitMqCommon.Impl
{
    internal class RabbitMqPublisher : IPublisher, IDisposable
    {
        public RabbitMqPublisher(IConnection connection, ICodec codec)
        {
            Channel = connection.CreateModel();
            Codec = codec;
        }

        public void Publish<T>(KeyValuePair<string, string>? arg, T ev)
        {
            string exchange = (arg == null ? "common" : $"{arg.Value.Key}={arg.Value.Value}");
            if (Exchanges.Add(exchange))
            {
                Channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Fanout);
            }

            var eventBytes = Codec.SerializeEnvelope(ev);
            Channel.BasicPublish(exchange: exchange, routingKey: "", basicProperties: null, body: eventBytes);
        }

        public void Dispose()
        {
            foreach (var exchangeName in Exchanges)
            {
                Channel.ExchangeDelete(exchangeName);
            }
            Channel.Dispose();
        }

        private readonly IModel Channel;
        private readonly ICodec Codec;
        private readonly HashSet<string> Exchanges = new HashSet<string>();
    }
}
