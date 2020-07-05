using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerPlatform
{
    public partial struct Card
    {
        public Card(Suit _suit, Rank _rank)
        {
            Suit = _suit;
            Rank = _rank;
        }

        public override string ToString()
        {
            return $"{Rank} of {Suit}".ToLower();
        }

        public Suit Suit { get; }
        public Rank Rank { get; }
    }

    public class Deck
    {
        public Deck()
        {
            rand = new Random();
            var suits = Enum.GetValues(typeof(Suit));
            var ranks = Enum.GetValues(typeof(Rank));
            int totalCards = suits.Length * ranks.Length;
            deck = new List<Card>(totalCards);
            used = new List<Card>(totalCards);
            foreach (Suit suit in suits)
            {
                foreach (Rank rank in ranks)
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

        public Card? PeekTop()
        {
            if (deck.Count == 0)
                return null;
            Card res = deck.Last();
            used.Add(deck.Last());
            deck.RemoveAt(deck.Count - 1);
            return res;
        }

        private readonly List<Card> deck;
        private readonly List<Card> used;
        private readonly Random rand;
    }
}
