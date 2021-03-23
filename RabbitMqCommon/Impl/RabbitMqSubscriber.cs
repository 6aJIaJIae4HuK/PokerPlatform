using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace RabbitMqCommon.Impl
{
    internal class RabbitMqSubscriber : ISubscriber
    {
        public RabbitMqSubscriber(IModel channel, ICodec codec)
        {
            Channel = channel;
            SubscribeQueue = Channel.QueueDeclare().QueueName;
            Codec = codec;
        }

        public ISubscription Subscribe<T>(Action<T> action, KeyValuePair<string, string>? arg)
            where T : new()
        {
            var group = GetOrAddGroup(GetExchange(arg));
            return group.Subscribe<T>(action);
        }

        private RabbitMqSubscribtionGroup GetOrAddGroup(string exchange)
        {
            if (!SubscriptionGroups.TryGetValue(exchange, out var group))
            {
                group = new RabbitMqSubscribtionGroup(Channel, Codec, SubscribeQueue, exchange);
                SubscriptionGroups.Add(exchange, group);
            }
            return group;
        }

        private string GetExchange(KeyValuePair<string, string>? arg)
        {
            if (arg == null)
            {
                return "common";
            }
            else
            {
                return $"{arg.Value.Key}={arg.Value.Value}";
            }
        }

        private readonly IModel Channel;
        private readonly string SubscribeQueue;
        private readonly ICodec Codec;

        private readonly Dictionary<string, RabbitMqSubscribtionGroup> SubscriptionGroups = new Dictionary<string, RabbitMqSubscribtionGroup>();
    }
}
