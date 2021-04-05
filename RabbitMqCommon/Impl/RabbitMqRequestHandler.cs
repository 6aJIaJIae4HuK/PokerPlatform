using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace RabbitMqCommon.Impl
{
    internal class RabbitMqRequestHandler : IDisposable
    {
        public RabbitMqRequestHandler(IConnection connection, ICodec codec, string receiveQueueName, RequestDispatcher dispatcher)
        {
            Channel = connection.CreateModel();
            Codec = codec;
            Dispatcher = dispatcher;

            Channel.QueueDeclare(
                queue: receiveQueueName,
                durable: true,
                exclusive: false
            );

            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += OnRequest;
            ConsumeTag = Channel.BasicConsume(
                queue: receiveQueueName,
                autoAck: false,
                consumer: consumer
            );
        }

        private void OnRequest(object sender, BasicDeliverEventArgs ea)
        {
            Console.WriteLine($"Get request for {ea.BasicProperties.CorrelationId}");
            var envelope = Codec.DeserializeEnvelope(ea.Body);
            var responseBytes = Dispatcher.HandleRequest(envelope.TypeId, envelope.Bytes);
            var props = ea.BasicProperties;
            var replyProps = Channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            Channel.BasicPublish(
                exchange: "",
                routingKey: props.ReplyTo,
                basicProperties: replyProps,
                body: responseBytes
            );
            Channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }

        public void Dispose()
        {
            Channel.BasicCancel(ConsumeTag);
            Channel.Dispose();
        }

        private readonly IModel Channel;
        private readonly ICodec Codec;
        private readonly RequestDispatcher Dispatcher;
        private readonly string ConsumeTag;
    }
}
