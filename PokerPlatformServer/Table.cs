using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerPlatformServer
{
    public class Table
    {
        public Table(int capacity)
        {
            Players = Enumerable.Repeat<Player>(null, capacity).ToList();
            FreeSpots = Enumerable.Range(0, capacity).ToHashSet();
        }

        int AddPlayer(Player player)
        {
            int pos = FreeSpots.First();
            FreeSpots.Remove(pos);
            Players[pos] = player;
            return pos;
        }

        void RemovePlayer(int pos)
        {
            Players[pos] = null;
            FreeSpots.Add(pos);
        }

        private readonly List<Player> Players;
        private readonly HashSet<int> FreeSpots;
    }
}
