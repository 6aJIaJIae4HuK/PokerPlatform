using System.Collections.Generic;
using System.Linq;

namespace PokerPlatformCommon
{
    public class Hand
    {
        public Hand(Proto.Hand proto)
        {
            Card1 = new Card(proto.Card1);
            Card2 = new Card(proto.Card2);
        }

        public Card Card1 { get; }

        public Card Card2 { get; }
    }

    public class GameView
    {
        public GameView(Proto.Game proto)
        {
            PotsInner = proto.Pots.Select(p => new PotView(p)).ToList();
            CurrentStreet = proto.CurrentStreet.ToStreet();
            HandsInner = proto.Hands.Select(p => p.Hand == null ? null : new Hand(p.Hand)).ToList();
            TableCardsInner = proto.TableCards.Select(p => new Card(p)).ToList();
        }

        public IReadOnlyList<PotView> Pots => PotsInner;

        public Street CurrentStreet { get; }

        public IReadOnlyList<Hand> Hands => HandsInner;

        public IReadOnlyList<Card> TableCards => TableCardsInner;

        private readonly List<PotView> PotsInner;
        private readonly List<Hand> HandsInner;
        private readonly List<Card> TableCardsInner;
    }
}
