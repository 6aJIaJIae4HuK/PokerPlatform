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
                cards.Add(deck.PeekTop().Value);
            Combination combination = new Combination(cards);
        }

        [TestMethod]
        public void CombinationCheck1()
        {
            List<Card> cards = new List<Card>();
            cards.Add(Card.AceOfDiamonds);
            cards.Add(Card.FourOfDiamonds);
            cards.Add(Card.AceOfSpides);
            cards.Add(Card.SixOfHearts);
            cards.Add(Card.KingOfDiamonds);
            Combination combination = new Combination(cards);
            Assert.AreEqual(combination.CombinationType, CombinationType.ONEPAIR);
        }

        [TestMethod]
        public void CombinationCheck2()
        {
            List<Card> cards = new List<Card>();
            cards.Add(Card.AceOfDiamonds);
            cards.Add(Card.AceOfDiamonds);
            cards.Add(Card.AceOfSpides);
            cards.Add(Card.SixOfHearts);
            cards.Add(Card.KingOfDiamonds);
            Combination combination = new Combination(cards);
            Assert.AreEqual(combination.CombinationType, CombinationType.THREEOFKIND);
        }

        [TestMethod]
        public void CombinationCheck3()
        {
            List<Card> cards = new List<Card>();
            cards.Add(Card.AceOfDiamonds);
            cards.Add(Card.FourOfDiamonds);
            cards.Add(Card.QueenOfDiamonds);
            cards.Add(Card.SixOfDiamonds);
            cards.Add(Card.KingOfDiamonds);
            Combination combination = new Combination(cards);
            Assert.AreEqual(combination.CombinationType, CombinationType.FLUSH);
        }

        [TestMethod]
        public void CombinationCheck4()
        {
            List<Card> cards = new List<Card>();
            cards.Add(Card.AceOfDiamonds);
            cards.Add(Card.KingOfDiamonds);
            cards.Add(Card.JackOfSpides);
            cards.Add(Card.TenOfHearts);
            cards.Add(Card.QueenOfDiamonds);
            Combination combination = new Combination(cards);
            Assert.AreEqual(combination.CombinationType, CombinationType.STRAIGHT);
        }
    }
}
