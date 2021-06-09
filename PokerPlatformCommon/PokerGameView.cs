using System.Collections.Generic;

namespace PokerPlatformCommon
{
    public class Hand
    {
        public Card Card1 { get; }

        public Card Card2 { get; }
    }

    public class PokerGameView
    {
        public IReadOnlyList<PotView> Pots { get; }

        public StreetType CurrentStreet { get; }

        public IReadOnlyList<Hand> Hands { get; }

        public IReadOnlyList<Card> TableCards { get; }
    }
}
