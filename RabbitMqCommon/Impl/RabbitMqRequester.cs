using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading;

namespace RabbitMqCommon.Impl
{
    internal class RabbitMqRequester : IRequester
    {
        public RabbitMqRequester(IModel channel, string sendQueueName, ICodec codec)
        {
            Channel = channel;
            SendQueueName = sendQueueName;
            ReceiveQueueName = "amq.rabbitmq.reply-to";
            Codec = codec;

            Consumer = new EventingBasicConsumer(Channel);
            Consumer.Received += OnResponse;
            Channel.BasicConsume(
                queue: ReceiveQueueName,
                autoAck: true,
                consumer: Consumer
            );
        }

        public TReply Call<TRequest, TReply>(TRequest request)
            where TReply : new()
        {
            var reqBytes = Codec.SerializeEnvelope(request);
            var props = Channel.CreateBasicProperties();
            props.CorrelationId = Guid.NewGuid().ToString();
            props.ReplyTo = ReceiveQueueName;

            Channel.BasicPublish(
                exchange: "",
                routingKey: SendQueueName,
                basicProperties: props,
                body: reqBytes
            );

            Handle.WaitOne();
            CurrentCorrelationId = null;
            var envelope = Codec.DeserializeEnvelope(CurrentResponse);
            if (Codec.IsErrorType(envelope.TypeId))
            {
                var error = Codec.DeserializeError(envelope.Bytes);
                throw new Exception(error.Str);
            }
            else if (Codec.CheckedGetTypeId<TReply>() == envelope.TypeId)
            {
                var reply = Codec.Deserialize<TReply>(envelope.Bytes);
                CurrentResponse = null;
                Handle.Reset();
                return reply;
            }
            else
            {
                throw new Exception($"Unexpected type #{envelope.TypeId}");
            }
        }

        private void OnResponse(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            if (ea.BasicProperties.CorrelationId == CurrentCorrelationId)
            {
                CurrentResponse = body;
                Handle.Set();
            }
        }

        private readonly IModel Channel;
        private readonly string SendQueueName;
        private readonly string ReceiveQueueName;
        private readonly ICodec Codec;

        private readonly EventingBasicConsumer Consumer;
        private string CurrentCorrelationId = null;
        private ReadOnlyMemory<byte> CurrentResponse = null;
        private readonly EventWaitHandle Handle = new AutoResetEvent(false);
    }
}
