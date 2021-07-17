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
            var resp = client.Requester.Call<PokerPlatformCommon.Proto.ConnectToTableRequest, PokerPlatformCommon.Proto.ConnectToTableReply>(new PokerPlatformCommon.Proto.ConnectToTableRequest
            {
                PlayerId = 0
            });
            Console.WriteLine(resp.TableView?.ToString() ?? "Empty");
        }
    }
}
