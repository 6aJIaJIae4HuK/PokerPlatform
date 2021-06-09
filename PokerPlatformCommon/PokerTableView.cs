using System.Collections.Generic;

namespace PokerPlatformCommon
{
    public class PokerTableView
    {
        public IReadOnlyList<PokerPlayerView> Players { get; }

        public PokerGameView CurrentGame { get; }

        public int SmallBlind { get; }

        public int BigBlind { get; }

        public int Ante { get; }

        public int ButtonPosition { get; }
    }
}
