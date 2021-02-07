using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace PokerPlatformClient
{
    public class RabbitMqServer
    {
        public RabbitMqServer(string host, string queueName, Func<byte[], byte[]> handler)
        {
            var factory = new ConnectionFactory() { HostName = host };
            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();
            QueueName = queueName;
            Channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            Channel.BasicQos(0, 1, false);
            RequestHandler = handler;
        }

        public void Start()
        {
            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = Channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                byte[] response = null;
                try
                {
                    response = RequestHandler(body);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    Channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, response);
                    Channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };
            Channel.BasicConsume(QueueName, false, consumer);
        }

        public void Dispose()
        {
            Channel.Dispose();
            Connection.Dispose();
        }

        private readonly IConnection Connection;
        private readonly IModel Channel;
        private readonly string QueueName;
        private readonly Func<byte[], byte[]> RequestHandler;
    }
}
