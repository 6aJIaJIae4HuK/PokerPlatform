using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerPlatform
{
    public class Combination : IComparable<Combination>
    {
        private const int NumOfCards = 5;

        public Combination(IEnumerable<Card> cards)
        {
            if (cards.Count() != NumOfCards)
                throw new ArgumentException("There are must be exactly 5 cards!");
            this.cards = new List<Card>(cards);
            combinationType = getCombinationType();
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

        private CombinationType getCombinationType()
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

                if (i > 1 && cards[i - 1].Rank + 1 != cards[i].Rank)
                {
                    isStraight = false;
                }
            }

            if (isStraight)
            {
                if (cards[0].Rank + 1 != cards[1].Rank)
                {
                    if (cards[0].Rank == Rank.ACE && cards[4].Rank == Rank.FIVE)
                    {
                        Card card = cards[0];
                        for (int i = 0; i < 4; i++)
                            cards[i] = cards[i + 1];
                        cards[4] = card;
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
                    swap(cards[0], cards[3]);
                }
                return CombinationType.FOUROFKIND;
            }

            if (cards[0].Rank == cards[2].Rank && cards[3].Rank == cards[4].Rank ||
                cards[0].Rank == cards[1].Rank && cards[2].Rank == cards[4].Rank)
            {
                if (cards[0].Rank == cards[2].Rank && cards[3].Rank == cards[4].Rank)
                {
                    swap(cards[0], cards[3]);
                    swap(cards[1], cards[4]);
                }
                return CombinationType.FULLHOUSE;
            }

            if (cards[0].Rank == cards[2].Rank ||
                cards[1].Rank == cards[3].Rank ||
                cards[2].Rank == cards[4].Rank)
            {
                if (cards[0].Rank == cards[2].Rank)
                {
                    swap(cards[0], cards[3]);
                    swap(cards[1], cards[4]);
                }
                else
                if (cards[1].Rank == cards[3].Rank)
                {
                    swap(cards[1], cards[4]);
                }

                return CombinationType.THREEOFKIND;
            }

            if (cards[0].Rank == cards[1].Rank && cards[2].Rank == cards[3].Rank ||
                cards[0].Rank == cards[1].Rank && cards[3].Rank == cards[4].Rank ||
                cards[1].Rank == cards[2].Rank && cards[3].Rank == cards[4].Rank)
            {
                if (cards[0].Rank == cards[1].Rank && cards[2].Rank == cards[3].Rank)
                {
                    swap(cards[2], cards[4]);
                    swap(cards[0], cards[2]);
                }
                else
                if (cards[0].Rank == cards[1].Rank && cards[3].Rank == cards[4].Rank)
                {
                    swap(cards[0], cards[2]);
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
                    swap(cards[0], cards[2]);
                    swap(cards[1], cards[3]);
                    swap(cards[2], cards[4]);
                }
                else
                if (cards[1].Rank == cards[2].Rank)
                {
                    swap(cards[1], cards[3]);
                    swap(cards[2], cards[4]);
                }
                else
                if (cards[2].Rank == cards[3].Rank)
                {
                    swap(cards[2], cards[4]);
                }
                return CombinationType.ONEPAIR;
            }

            return CombinationType.HIGHCARD;
        }

        private void swap(Card card1, Card card2)
        {
            Card card = card1;
            card1 = card2;
            card2 = card;
        }

        private readonly List<Card> cards;
        private readonly CombinationType combinationType;
        public CombinationType CombinationType { get { return combinationType; } }
        public ReadOnlyCollection<Card> Cards { get { return new ReadOnlyCollection<Card>(cards); } }
    }
}
