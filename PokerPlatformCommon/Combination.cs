using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PokerPlatformCommon
{
    public class Combination : IComparable<Combination>
    {
        private static readonly int NumOfCards = 5;

        public Combination(IEnumerable<Card> cards)
        {
            if (cards.Count() != NumOfCards)
                throw new ArgumentException("There are must be exactly 5 cards!");
            this.cards = new List<Card>(cards);
            CombinationType = GetCombinationType();
        }

        public int CompareTo(Combination other)
        {
            if (CombinationType != other.CombinationType)
                return (CombinationType.CompareTo(CombinationType));
            for (int i = NumOfCards - 1; i >= 0; i--)
            {
                if (Cards[i].Rank != other.Cards[i].Rank)
                    return Cards[i].Rank.CompareTo(other.Cards[i].Rank);
            }
            return 0;
        }

        private CombinationType GetCombinationType()
        {
            cards.Sort((x, y) => x.Rank.CompareTo(y.Rank));
            bool isFlash = true;
            bool isStraight = true;
            for (int i = 0; i < NumOfCards; i++)
            {
                if (cards[i].Suit != cards[0].Suit)
                {
                    isFlash = false;
                }

                if (i + 2 < NumOfCards && cards[i].Rank + 1 != cards[i + 1].Rank)
                {
                    isStraight = false;
                }
            }

            if (isStraight)
            {
                if (cards[NumOfCards - 2].Rank + 1 != cards[NumOfCards - 1].Rank)
                {
                    if (cards[0].Rank == Rank.TWO && cards[NumOfCards - 1].Rank == Rank.ACE)
                    {
                        Card card = cards[NumOfCards - 1];
                        for (int i = NumOfCards - 1; i > 0; i--)
                            cards[i] = cards[i - 1];
                        cards[0] = card;
                    }
                    else
                    {
                        isStraight = false;
                    }
                }
            }

            if (isFlash || isStraight)
            {
                if (isFlash && isStraight)
                {
                    return CombinationType.STRAIGHTFLUSH;
                }
                else
                {
                    return isFlash ? CombinationType.FLUSH : CombinationType.STRAIGHT;
                }
            }

            if (cards[0].Rank == cards[3].Rank || cards[1].Rank == cards[4].Rank)
            {
                if (cards[0].Rank == cards[3].Rank)
                {
                    cards.Swap(0, 3);
                }
                return CombinationType.FOUROFKIND;
            }

            if (cards[0].Rank == cards[2].Rank && cards[3].Rank == cards[4].Rank ||
                cards[0].Rank == cards[1].Rank && cards[2].Rank == cards[4].Rank)
            {
                if (cards[0].Rank == cards[2].Rank && cards[3].Rank == cards[4].Rank)
                {
                    cards.Swap(0, 3);
                    cards.Swap(1, 4);
                }
                return CombinationType.FULLHOUSE;
            }

            if (cards[0].Rank == cards[2].Rank ||
                cards[1].Rank == cards[3].Rank ||
                cards[2].Rank == cards[4].Rank)
            {
                if (cards[0].Rank == cards[2].Rank)
                {
                    cards.Swap(0, 3);
                    cards.Swap(1, 4);
                }
                else if (cards[1].Rank == cards[3].Rank)
                {
                    cards.Swap(1, 4);
                }

                return CombinationType.THREEOFKIND;
            }

            if (cards[0].Rank == cards[1].Rank && cards[2].Rank == cards[3].Rank ||
                cards[0].Rank == cards[1].Rank && cards[3].Rank == cards[4].Rank ||
                cards[1].Rank == cards[2].Rank && cards[3].Rank == cards[4].Rank)
            {
                if (cards[0].Rank == cards[1].Rank && cards[2].Rank == cards[3].Rank)
                {
                    cards.Swap(2, 4);
                    cards.Swap(0, 2);
                }
                else if (cards[0].Rank == cards[1].Rank && cards[3].Rank == cards[4].Rank)
                {
                    cards.Swap(0, 2);
                }
                return CombinationType.TWOPAIRS;
            }

            if (cards[0].Rank == cards[1].Rank ||
                cards[1].Rank == cards[2].Rank ||
                cards[2].Rank == cards[3].Rank ||
                cards[3].Rank == cards[4].Rank)
            {
                if (cards[0].Rank == cards[1].Rank)
                {
                    cards.Swap(0, 2);
                    cards.Swap(1, 3);
                    cards.Swap(2, 4);
                }
                else if (cards[1].Rank == cards[2].Rank)
                {
                    cards.Swap(1, 3);
                    cards.Swap(2, 4);
                }
                else if (cards[2].Rank == cards[3].Rank)
                {
                    cards.Swap(2, 4);
                }
                return CombinationType.ONEPAIR;
            }

            return CombinationType.HIGHCARD;
        }

        private readonly List<Card> cards;

        public CombinationType CombinationType { get; }
        public ReadOnlyCollection<Card> Cards { get { return new ReadOnlyCollection<Card>(cards); } }
    }
}
