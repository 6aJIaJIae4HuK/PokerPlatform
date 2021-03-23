using System;
using System.Collections.Generic;

namespace RabbitMqCommon.Impl
{
    internal class RequestDispatcher
    {
        public RequestDispatcher(ICodec codec)
        {
            Codec = codec;
        }

        public void RegisterHandler<TRequest, TReply>(TypedRequestHandler<TRequest, TReply> handler)
            where TRequest : new()
            where TReply : new()
        {
            int requestTypeId = Codec.CheckedGetTypeId<TRequest>();
            _ = Codec.CheckedGetTypeId<TReply>();
            if (!Handlers.TryAdd(requestTypeId, handler))
            {
                throw new Exception($"Already register handler for type #{requestTypeId}");
            }
        }

        public byte[] HandleRequest(int typeId, ReadOnlyMemory<byte> bytes)
        {
            try
            {
                if (!Handlers.TryGetValue(typeId, out var handler))
                {
                    throw new Exception($"There is no registered handler for type #{typeId}");
                }
                return handler.HandleRequest(bytes);
            }
            catch (Exception e)
            {
                return Codec.SerializeError(new Error { Str = e.ToString() });
            }
        }

        private readonly ICodec Codec;
        private readonly Dictionary<int, IRequestHandler> Handlers = new Dictionary<int, IRequestHandler>();
    }
}
