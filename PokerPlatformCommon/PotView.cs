using System;
using System.Collections.Generic;
using System.Text;

namespace PokerPlatformCommon
{
    public class PotView
    {
        public PotView(Proto.Pot proto)
        {
            PlayersInner = new List<int>(proto.Players);
            Size = proto.Size;
        }

        public IReadOnlyList<int> Players { get { return PlayersInner; } }

        public ulong Size { get; }

        private readonly List<int> PlayersInner;
    }
}
