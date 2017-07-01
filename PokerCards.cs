using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerPlatform
{
    public class Card
    {
        public Card(Suit _suit, Rank _rank)
        {
            suit = _suit;
            rank = _rank;
        }

        public override string ToString()
        {
            return String.Format("{0} of {1}", rank.ToString().ToLower(), suit.ToString().ToLower());
        }

        private readonly Suit suit;
        private readonly Rank rank;
        public Suit Suit { get { return suit; } }
        public Rank Rank { get { return rank; } }
    }

    public class Deck
    {
        public Deck()
        {
            rand = new Random();
            deck = new List<Card>(52);
            used = new List<Card>(52);
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    deck.Add(new Card(suit, rank));
                }
            }
            ShuffleCards();
        }
        
        public void ShuffleCards()
        {
            deck.AddRange(used);
            used.Clear();
            int sz = deck.Count;
            for (int i = sz - 1; i >= 0; i--)
            {
                int j = rand.Next(i + 1);
                Card tmp = deck[i];
                deck[i] = deck[j];
                deck[j] = tmp;
            }
        }

        public Card PeekTop()
        {
            if (deck.Count == 0)
                return null;
            Card res = deck.Last();
            used.Add(deck.Last());
            deck.RemoveAt(deck.Count - 1);
            return res;
        }

        private List<Card> deck;
        private List<Card> used;
        private Random rand;
    }
}
