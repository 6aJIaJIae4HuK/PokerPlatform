using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokerPlatform
{
    class Program
    {
        static void Main()
        {
            var table = PokerTable.CreateForSixPlayers(new PokerTableSettings(1, 2, 0));
            table.SetPlayer(0, new Player());
            table.SetPlayer(2, new Player());
            table.SetPlayer(3, new Player());
            table.SetPlayer(5, new Player());
            table.StartGames();
            Task.Delay(60000).Wait();
            table.StopGames();
        }
    }
}
