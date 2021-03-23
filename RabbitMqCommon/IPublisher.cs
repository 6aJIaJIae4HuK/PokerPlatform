using System.Collections.Generic;

namespace RabbitMqCommon
{
    public interface IPublisher
    {
        void Publish<T>(KeyValuePair<string, string>? arg, T ev);
    }
}
