using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PokerPlatformCommon;
using RabbitMqCommon;

namespace PokerPlatformServer
{
    public class SampleHandler : TypedRequestHandler<PokerPlatformCommon.Proto.Query, PokerPlatformCommon.Proto.Answer>
    {
        public SampleHandler(ICodec codec, IPublisher publisher)
            : base(codec, publisher)
        { }

        public override PokerPlatformCommon.Proto.Answer DoHandle(PokerPlatformCommon.Proto.Query query)
        {
            return new PokerPlatformCommon.Proto.Answer
            {
                Result = query.Left + query.Right
            };
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var codec = new CodecBuilder().AddPokerMessages().Build();
            var server = new RabbitMqServerBuilder("127.0.0.1", "rpc", codec)
                .AddRequestHandler((codec, publisher) => new SampleHandler(codec, publisher))
                .Build();

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
