using PokerPlatformClient;
using System;
using System.Text;

namespace PokerPlatformServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Func<byte[], byte[]> handler = request =>
            {
                string str = Encoding.UTF8.GetString(request);
                char[] chars = str.ToCharArray();
                Array.Reverse(chars);
                return Encoding.UTF8.GetBytes(new string(chars));
            };
            var server = new RabbitMqServer("127.0.0.1", "queue", handler);
            server.Start();
            Console.ReadLine();
        }
    }
}
