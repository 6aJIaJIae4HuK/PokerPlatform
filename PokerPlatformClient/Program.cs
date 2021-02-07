using RabbitMqCommon;
using System;
using System.Text;

namespace PokerPlatformClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RabbitMqClient("127.0.0.1", "queue");
            while (true)
            {
                string str = Console.ReadLine();
                var reqBytes = Encoding.UTF8.GetBytes(str);
                var respBytes = client.Call(reqBytes);
                Console.WriteLine(Encoding.UTF8.GetString(respBytes));
            }
        }
    }
}
