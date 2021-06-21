using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerPlatformCommon
{
    // TODO: Maybe use ref struct instead of struct?
    public partial struct Card
    {
        public Card(Proto.Card proto)
            : this(proto.Suit.ToSuit(), proto.Rank.ToRank())
        {}

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public override string ToString()
        {
            return $"{GetRankChar()}{GetSuitChar()}";
        }

        public Suit Suit { get; }
        public Rank Rank { get; }

        private char GetRankChar()
        {
            switch (Rank)
            {
                case Rank.TWO:
                case Rank.THREE:
                case Rank.FOUR:
                case Rank.FIVE:
                case Rank.SIX:
                case Rank.SEVEN:
                case Rank.EIGHT:
                case Rank.NINE:
                    return (char)(Convert.ToUInt16('0') + Rank + 2);
                case Rank.TEN:
                    return 'T';
                case Rank.JACK:
                    return 'J';
                case Rank.QUEEN:
                    return 'Q';
                case Rank.KING:
                    return 'K';
                case Rank.ACE:
                    return 'A';
                default:
                    throw new ArgumentException();
            }
        }

        private char GetSuitChar()
        {
            switch (Suit)
            {
                case Suit.HEARTS:
                    return (char)(0x2665);
                case Suit.CLUBS:
                    return (char)(0x2663);
                case Suit.SPIDES:
                    return (char)(0x2660);
                case Suit.DIAMONDS:
                    return (char)(0x2666);
                default:
                    throw new ArgumentException();
            }
        }
    }

    public class Deck
    {
        public Deck()
        {
            Rand = new Random();
            var suits = Enum.GetValues(typeof(Suit));
            var ranks = Enum.GetValues(typeof(Rank));
            int totalCards = suits.Length * ranks.Length;
            Cards = new List<Card>(totalCards);
            UsedCards = new List<Card>();
            foreach (Suit suit in suits)
            {
                foreach (Rank rank in ranks)
                {
                    Cards.Add(new Card(suit, rank));
                }
            }
            ShuffleCards();
        }
        
        public void ShuffleCards()
        {
            Cards.AddRange(UsedCards);
            UsedCards.Clear();
            int sz = Cards.Count;
            for (int i = sz - 1; i >= 0; i--)
            {
                int j = Rand.Next(i + 1);
                Cards.Swap(i, j);
            }
        }

        public Card PeekTop()
        {
            Card res = Cards.Last();
            Cards.RemoveAt(Cards.Count - 1);
            UsedCards.Add(res);
            return res;
        }

        private readonly List<Card> Cards;
        private readonly List<Card> UsedCards;
        private readonly Random Rand;
    }
}
