using System;
using System.Threading.Tasks;

namespace PokerPlatform
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var table = PokerTable.CreateForSixPlayers(new PokerTableSettings(1, 2, 0));
            table.SetPlayer(0, new BotPlayer(20));
            table.SetPlayer(2, new BotPlayer(20));
            table.SetPlayer(3, new BotPlayer(20));
            table.SetPlayer(5, new BotPlayer(20));
            table.StartGames();
            Task.Delay(60000).Wait();
            table.StopGames();
        }
    }
}
