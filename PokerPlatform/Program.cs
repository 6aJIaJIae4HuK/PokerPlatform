using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerPlatform
{
    class Program
    {
        static void Main(string[] args)
        {
            Deck deck = new Deck();
            List<Card> cards = new List<Card>(5);
            for (int i = 0; i < 5; i++)
                cards.Add(deck.PeekTop());

            Combination combination = new Combination(cards);

            foreach (Card card in combination.Cards)
                Console.WriteLine(card);
        }
    }
}
