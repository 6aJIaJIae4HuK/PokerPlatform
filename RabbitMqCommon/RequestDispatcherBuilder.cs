using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace RabbitMqCommon
{
    internal delegate byte[] RequestHandler(ReadOnlyMemory<byte> reqBytes);
    internal delegate void HandlerRegistrator(IServiceProvider provider, Dictionary<int, RequestHandler> handlers);

    public class RequestDispatcherBuilder
    {

        public RequestDispatcherBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public RequestDispatcherBuilder AddHandler<TRequest, TReply, THandler>()
            where TRequest : new()
            where THandler : TypedRequestHandler<TRequest, TReply>
        {
            Services.AddSingleton<THandler>();
            Registrators.Add(
                (provider, handlers) =>
                {
                    var codec = provider.GetRequiredService<ICodec>();
                    int requestTypeId = codec.CheckedGetTypeId<TRequest>();
                    _ = codec.CheckedGetTypeId<TReply>();
                    if (!handlers.TryAdd(requestTypeId, reqBytes =>
                    {
                        var handler = provider.GetRequiredService<THandler>();
                        var request = codec.Deserialize<TRequest>(reqBytes);
                        var reply = handler.DoHandle(request);
                        return codec.SerializeEnvelope(reply);
                    }))
                    {
                        throw new Exception($"Already register handler for type #{requestTypeId}");
                    }
                }
            );
            return this;
        }

        internal Impl.RequestDispatcher Build(IServiceProvider provider)
        {
            return new Impl.RequestDispatcher(provider, Registrators);
        }

        private readonly IServiceCollection Services;
        private readonly List<HandlerRegistrator> Registrators = new List<HandlerRegistrator>();
    }
}
