using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PokerPlatformCommon;
using RabbitMqCommon;

namespace PokerPlatformServer
{
    public class ConnectToTableHandler : TypedRequestHandler<PokerPlatformCommon.Proto.ConnectToTableRequest, PokerPlatformCommon.Proto.ConnectToTableReply>
    {
        public override PokerPlatformCommon.Proto.ConnectToTableReply DoHandle(PokerPlatformCommon.Proto.ConnectToTableRequest query)
        {
            return new PokerPlatformCommon.Proto.ConnectToTableReply
            {
                TableView = null
            };
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection collection = new ServiceCollection()
                .AddCodec(builder => builder.AddPokerMessages())
                .AddRequestHandlers(builder =>
                    builder.AddHandler<PokerPlatformCommon.Proto.ConnectToTableRequest, PokerPlatformCommon.Proto.ConnectToTableReply, ConnectToTableHandler>()
                )
                .AddRabbitMqServer("127.0.0.1", "rpc");

            var provider = collection.BuildServiceProvider();
            var server = provider.GetRequiredService<IServer>();
            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                while (true)
                {
                    server.Publisher.Publish(null, new PokerPlatformCommon.Proto.TimestampEvent
                    {
                        Value = DateTimeOffset.Now.ToUnixTimeSeconds()
                    });
                    Thread.Sleep(1000);
                }
            }, cts.Token);

            Thread.Sleep(60000);
            cts.Cancel();
        }
    }
}
