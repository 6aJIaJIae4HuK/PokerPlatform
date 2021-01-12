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
            table.AddPlayer(0, new BotPlayer(1));
            table.AddPlayer(2, new BotPlayer(1));
            table.AddPlayer(3, new BotPlayer(1));
            table.AddPlayer(5, new BotPlayer(1));
            table.StartGames();
            Task.Delay(60000).Wait();
            table.StopGames();
        }
    }
}
