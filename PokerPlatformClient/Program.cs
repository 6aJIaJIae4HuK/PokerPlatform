using PokerPlatformCommon;
using RabbitMqCommon;
using System;
using System.Linq;
using System.Threading;

namespace PokerPlatformClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var codec = new CodecBuilder().AddPokerMessages().Build();
            var client = new RabbitMqClientBuilder("127.0.0.1", "rpc", codec).Build();
            client.Subscriber.Subscribe<PokerPlatformCommon.Proto.TimestampEvent>(
                (ev) => 
                { 
                    Console.WriteLine($"Time: {DateTimeOffset.FromUnixTimeSeconds(ev.Value)}");
                }
            );
            Random rand = new Random();
            while (true)
            {
                int left = rand.Next(100);
                int right = rand.Next(100);
                var resp = client.Requester.Call<PokerPlatformCommon.Proto.Query, PokerPlatformCommon.Proto.Answer>(new PokerPlatformCommon.Proto.Query
                {
                    Left = left,
                    Right = right
                });
                Console.WriteLine($"Response: {resp.Result}");
                int wait = rand.Next(10000);
                Thread.Sleep(wait);
            }
        }
    }
}
