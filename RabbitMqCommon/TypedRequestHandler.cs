using System;

namespace RabbitMqCommon
{
    public abstract class TypedRequestHandler<TRequest, TReply>
    {
        public abstract TReply DoHandle(TRequest request);
    }
}
