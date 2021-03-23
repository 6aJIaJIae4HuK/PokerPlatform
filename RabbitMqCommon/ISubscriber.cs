using System;
using System.Collections.Generic;

namespace RabbitMqCommon
{
    public interface ISubscriber
    {
        ISubscription Subscribe<T>(Action<T> action, KeyValuePair<string, string>? args = null)
            where T : new();
    }
}
