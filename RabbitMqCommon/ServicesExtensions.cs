using Microsoft.Extensions.DependencyInjection;
using System;

namespace RabbitMqCommon
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddCodec(this IServiceCollection services, Func<CodecBuilder, CodecBuilder> configurator)
        {
            CodecBuilder builder = new CodecBuilder();
            configurator(builder);
            services.AddSingleton(_ => builder.Build());
            return services;
        }

        public static IServiceCollection AddRequestHandlers(this IServiceCollection services, Func<RequestDispatcherBuilder, RequestDispatcherBuilder> configurator)
        {
            RequestDispatcherBuilder builder = new RequestDispatcherBuilder(services);
            configurator(builder);
            services.AddSingleton(provider => builder.Build(provider));
            return services;
        }

        public static IServiceCollection AddRabbitMqServer(this IServiceCollection services, string host, string receiveQueue)
        {
            services.AddSingleton<IServer>(provider => new Impl.RabbitMqServer(provider, host, receiveQueue));
            return services;
        }
    }
}
