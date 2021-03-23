using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace RabbitMqCommon.Impl
{
    internal class RabbitMqSubscribtionGroup
    {
        private interface IEventHandler : ISubscription
        {
            void HandleEvent(ReadOnlyMemory<byte> eventBytes);
        }

        private class EventHandler<T> : IEventHandler
            where T : new()
        {
            public EventHandler(Action<T> action, ICodec codec, Action remover)
            {
                Action = action;
                Codec = codec;
                Remover = remover;
            }

            private readonly Action<T> Action;
            private readonly Action Remover;

            public void HandleEvent(ReadOnlyMemory<byte> eventBytes)
            {
                var ev = Codec.Deserialize<T>(eventBytes);
                Action(ev);
            }

            public void Stop()
            {
                Remover();
            }

            private readonly ICodec Codec;
        }

        public RabbitMqSubscribtionGroup(IModel channel, ICodec codec, string queueName, string exchangeName)
        {
            Channel = channel;
            Codec = codec;
            QueueName = queueName;
            ExchangeName = exchangeName;

            Consumer = new EventingBasicConsumer(Channel);
            Consumer.Received += OnEvent;
            Channel.QueueBind(
                queue: QueueName,
                exchange: ExchangeName,
                routingKey: ""
            );
        }

        public ISubscription Subscribe<T>(Action<T> action)
            where T : new()
        {
            int typeId = Codec.CheckedGetTypeId<T>();
            var res = new EventHandler<T>(action, Codec, () => RemoveSubscription(typeId));
            AddSubscription(typeId, res);
            return res;
        }

        private void AddSubscription(int typeId, IEventHandler subscription)
        {
            EventHandlers[typeId] = subscription;
            if (EventHandlers.Count == 1)
            {
                ConsumeTag = Channel.BasicConsume(
                    queue: QueueName,
                    autoAck: true,
                    consumer: Consumer
                );
            }
        }

        private void RemoveSubscription(int typeId)
        {
            EventHandlers.Remove(typeId);
            if (EventHandlers.Count == 0)
            {
                Channel.BasicCancel(ConsumeTag);
            }
        }

        private void OnEvent(object sender, BasicDeliverEventArgs ea)
        {
            var envelope = Codec.DeserializeEnvelope(ea.Body);
            if (EventHandlers.TryGetValue(envelope.TypeId, out var handler))
            {
                handler.HandleEvent(envelope.Bytes);
            }
        }

        private readonly IModel Channel;
        private readonly ICodec Codec;
        private readonly string QueueName;
        private readonly string ExchangeName;
        private readonly EventingBasicConsumer Consumer;
        private string ConsumeTag;
        private readonly Dictionary<int, IEventHandler> EventHandlers = new Dictionary<int, IEventHandler>();
        
    }
}
