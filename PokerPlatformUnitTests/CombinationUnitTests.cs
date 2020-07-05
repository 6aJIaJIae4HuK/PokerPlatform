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
            _ = new Combination(cards);
        }

        [TestMethod]
        public void CombinationCheck1()
        {
            List<Card> cards = new List<Card>
            {
                Card.AceOfDiamonds,
                Card.FourOfDiamonds,
                Card.AceOfSpides,
                Card.SixOfHearts,
                Card.KingOfDiamonds
            };
            Combination combination = new Combination(cards);
            Assert.AreEqual(combination.CombinationType, CombinationType.ONEPAIR);
        }

        [TestMethod]
        public void CombinationCheck2()
        {
            List<Card> cards = new List<Card>
            {
                Card.AceOfDiamonds,
                Card.AceOfDiamonds,
                Card.AceOfSpides,
                Card.SixOfHearts,
                Card.KingOfDiamonds
            };
            Combination combination = new Combination(cards);
            Assert.AreEqual(combination.CombinationType, CombinationType.THREEOFKIND);
        }

        [TestMethod]
        public void CombinationCheck3()
        {
            List<Card> cards = new List<Card>
            {
                Card.AceOfDiamonds,
                Card.FourOfDiamonds,
                Card.QueenOfDiamonds,
                Card.SixOfDiamonds,
                Card.KingOfDiamonds
            };
            Combination combination = new Combination(cards);
            Assert.AreEqual(combination.CombinationType, CombinationType.FLUSH);
        }

        [TestMethod]
        public void CombinationCheck4()
        {
            List<Card> cards = new List<Card>
            {
                Card.AceOfDiamonds,
                Card.KingOfDiamonds,
                Card.JackOfSpides,
                Card.TenOfHearts,
                Card.QueenOfDiamonds
            };
            Combination combination = new Combination(cards);
            Assert.AreEqual(combination.CombinationType, CombinationType.STRAIGHT);
        }
    }
}
