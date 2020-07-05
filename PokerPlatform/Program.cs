using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerPlatform
{
    class Program
    {
        static void Main()
        {
            Deck deck = new Deck();
            int itCount = 1000000;
            var combinationValues = Enum.GetValues(typeof(CombinationType)).Cast<CombinationType>().ToArray();
            int[] cnt = new int[combinationValues.Length];
            foreach (int it in Enumerable.Range(0, itCount))
            {
                List<Card> cards = new List<Card>(5);
                for (int i = 0; i < 5; i++)
                    cards.Add(deck.PeekTop().Value);
                Combination combination = new Combination(cards);
                ++cnt[Array.IndexOf(combinationValues, combination.CombinationType)];
                deck.ShuffleCards();
            }

            foreach (var type in combinationValues)
            {
                Console.WriteLine($"Number of {type} is {cnt[Array.IndexOf(combinationValues, type)]}");
            }
        }
    }
}
