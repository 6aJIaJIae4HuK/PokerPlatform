using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PokerPlatform;
using System.Collections.Generic;

namespace PokerPlatformUnitTests
{
    [TestClass]
    public class CombinationUnitTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "There are must be exactly 5 cards!")]
        public void CombinationException()
        {
            Deck deck = new Deck();
            List<Card> cards = new List<Card>();
            for (int i = 0; i < 6; i++)
                cards.Add(deck.PeekTop());
            Combination combination = new Combination(cards);
        }

        [TestMethod]
        public void CombinationCheck1()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Suit.DIAMONDS, Rank.ACE));
            cards.Add(new Card(Suit.DIAMONDS, Rank.FOUR));
            cards.Add(new Card(Suit.SPIDES, Rank.ACE));
            cards.Add(new Card(Suit.HEARTS, Rank.SIX));
            cards.Add(new Card(Suit.DIAMONDS, Rank.KING));
            Combination combination = new Combination(cards);
            Assert.AreEqual(combination.CombinationType, CombinationType.ONEPAIR);
        }

        [TestMethod]
        public void CombinationCheck2()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Suit.DIAMONDS, Rank.ACE));
            cards.Add(new Card(Suit.DIAMONDS, Rank.ACE));
            cards.Add(new Card(Suit.SPIDES, Rank.ACE));
            cards.Add(new Card(Suit.HEARTS, Rank.SIX));
            cards.Add(new Card(Suit.DIAMONDS, Rank.KING));
            Combination combination = new Combination(cards);
            Assert.AreEqual(combination.CombinationType, CombinationType.THREEOFKIND);
        }

        [TestMethod]
        public void CombinationCheck3()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Suit.DIAMONDS, Rank.ACE));
            cards.Add(new Card(Suit.DIAMONDS, Rank.FOUR));
            cards.Add(new Card(Suit.DIAMONDS, Rank.QUEEN));
            cards.Add(new Card(Suit.DIAMONDS, Rank.SIX));
            cards.Add(new Card(Suit.DIAMONDS, Rank.KING));
            Combination combination = new Combination(cards);
            Assert.AreEqual(combination.CombinationType, CombinationType.FLUSH);
        }

        [TestMethod]
        public void CombinationCheck4()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card(Suit.DIAMONDS, Rank.ACE));
            cards.Add(new Card(Suit.DIAMONDS, Rank.KING));
            cards.Add(new Card(Suit.SPIDES, Rank.JACK));
            cards.Add(new Card(Suit.HEARTS, Rank.TEN));
            cards.Add(new Card(Suit.DIAMONDS, Rank.QUEEN));
            Combination combination = new Combination(cards);
            Assert.AreEqual(combination.CombinationType, CombinationType.STRAIGHT);
        }
    }
}
