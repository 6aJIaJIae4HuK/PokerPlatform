using System;
using System.Linq;
using System.Threading;

namespace PokerPlatform
{
    public class EventNotifier
    {
        public readonly PlayerContainer Container;

        public EventNotifier(PlayerContainer container)
        {
            Container = container;
        }

        public void NotifyOnePlayer(int pos, IPokerEvent ev)
        {
            Container.NotifyOnePlayer(pos, ev);
        }

        public void NotifyAllPlayers(IPokerEvent ev)
        {
            Container.NotifyAllPlayers(ev);
        }
    }

    public class PlayerContainer
    {
        public PlayerContainer(int count)
        {
            Players = new Player[count];
        }

        public void Add(int pos, Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException();
            }
            lock (LockObject)
            {
                if (Players[pos] != null)
                {
                    throw new InvalidOperationException($"Position #{pos} is already taken");
                }
                Players[pos] = player;
            }
        }

        public void Remove(int pos)
        {
            lock (LockObject)
            {
                if (Players[pos] == null)
                {
                    throw new InvalidOperationException($"Position #{pos} is already free");
                }
                Players[pos] = null;
            }
        }

        public int GetNextPlayerPos(int afterPos)
        {
            int res = afterPos;
            lock (LockObject)
            {
                do
                {
                    res = (res + 1) % Players.Length;
                } while (Players[res] == null);
                return res;
            }
        }

        public int Count
        {
            get
            {
                lock (LockObject)
                {
                    return Players.Count(pl => pl != null);
                }
            }
        }

        public void NotifyAllPlayers(IPokerEvent ev)
        {
            Player[] playersToNotify;
            lock (LockObject)
            {
                playersToNotify = Players.Where(pl => pl != null).ToArray();
            }

            foreach (var pl in playersToNotify)
            {
                pl.HandleEvent(ev);
            }
        }

        public void NotifyOnePlayer(int pos, IPokerEvent ev)
        {
            Player playerToNotify;
            lock (LockObject)
            {
                playerToNotify = Players[pos];
            }

            playerToNotify.HandleEvent(ev);
        }

        public Player[] GetCurrentPlayers()
        {
            Player[] res;
            lock (LockObject)
            {
                res = Players;
            }
            return res;
        }

        public EventNotifier GetNotifier()
        {
            return new EventNotifier(this);
        }

        private readonly Player[] Players;
        private readonly object LockObject = new object();
    }
}
