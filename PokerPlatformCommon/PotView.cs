using System;
using System.Collections.Generic;
using System.Text;

namespace PokerPlatformCommon
{
    public class PotView
    {
        public IReadOnlyList<uint> Players { get; }

        public ulong Size { get; }
    }
}
