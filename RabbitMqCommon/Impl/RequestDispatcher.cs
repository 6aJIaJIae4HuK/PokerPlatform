using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace RabbitMqCommon.Impl
{
    internal class RequestDispatcher
    {
        public RequestDispatcher(IServiceProvider provider, List<HandlerRegistrator> registrators)
        {
            foreach (var registrator in registrators)
            {
                registrator(provider, Handlers);
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
                return handler(bytes);
            }
            catch (Exception e)
            {
                return Codec.SerializeError(new Error { Str = e.ToString() });
            }
        }

        private readonly ICodec Codec;
        private readonly Dictionary<int, RequestHandler> Handlers = new Dictionary<int, RequestHandler>();
    }
}
