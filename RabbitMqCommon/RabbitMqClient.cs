using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;

namespace RabbitMqCommon
{
    public class RabbitMqClient : IDisposable
    {
        public RabbitMqClient(string host, string queueName)
        {
            var factory = new ConnectionFactory() { HostName = host };

            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();
            RequestQueueName = queueName;
            ReplyQueueName = Channel.QueueDeclare().QueueName;
            Consumer = new EventingBasicConsumer(Channel);

            Props = Channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            Props.CorrelationId = correlationId;
            Props.ReplyTo = ReplyQueueName;

            Consumer.Received += (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    ResponseQueue.Add(ea.Body.ToArray());
                }
            };
        }

        public byte[] Call(byte[] requestBytes)
        {
            Channel.BasicPublish(exchange: "", routingKey: RequestQueueName, basicProperties: Props, body: requestBytes);
            Channel.BasicConsume(consumer: Consumer, queue: ReplyQueueName, autoAck: true);
            return ResponseQueue.Take();
        }

        public void Dispose()
        {
            Channel.Dispose();
            Connection.Dispose();
        }

        private readonly IConnection Connection;
        private readonly IModel Channel;
        private readonly string RequestQueueName;
        private readonly string ReplyQueueName;
        private readonly EventingBasicConsumer Consumer;
        private readonly IBasicProperties Props;
        private readonly BlockingCollection<byte[]> ResponseQueue = new BlockingCollection<byte[]>();
    }
}
