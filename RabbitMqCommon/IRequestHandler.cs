using System;

namespace RabbitMqCommon
{
    public interface IRequestHandler
    {
        byte[] HandleRequest(ReadOnlyMemory<byte> reqBytes);
    }
}
