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
        [ExpectedException(typeof(ArgumentException))]
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
            CombinationCheck(new List<Card>
            {
                Card.AceOfDiamonds,
                Card.FourOfDiamonds,
                Card.AceOfSpides,
                Card.SixOfHearts,
                Card.KingOfDiamonds
            }, CombinationType.ONEPAIR);
        }

        [TestMethod]
        public void CombinationCheck2()
        {
            CombinationCheck(new List<Card>
            {
                Card.AceOfDiamonds,
                Card.AceOfDiamonds,
                Card.AceOfSpides,
                Card.SixOfHearts,
                Card.KingOfDiamonds
            }, CombinationType.THREEOFKIND);
        }

        [TestMethod]
        public void CombinationCheck3()
        {
            CombinationCheck(new List<Card>
            {
                Card.AceOfDiamonds,
                Card.FourOfDiamonds,
                Card.QueenOfDiamonds,
                Card.SixOfDiamonds,
                Card.KingOfDiamonds
            }, CombinationType.FLUSH);
        }

        [TestMethod]
        public void CombinationCheck4()
        {
            CombinationCheck(new List<Card>
            {
                Card.AceOfDiamonds,
                Card.KingOfDiamonds,
                Card.JackOfSpides,
                Card.TenOfHearts,
                Card.QueenOfDiamonds
            }, CombinationType.STRAIGHT);
        }

        [TestMethod]
        public void CombinationCheck5()
        {
            CombinationCheck(new List<Card>
            {
                Card.AceOfDiamonds,
                Card.TwoOfDiamonds,
                Card.ThreeOfSpides,
                Card.FourOfHearts,
                Card.FiveOfDiamonds
            }, CombinationType.STRAIGHT);
        }

        private void CombinationCheck(List<Card> cards, CombinationType expectedType)
        {
            Combination combination = new Combination(cards);
            Assert.AreEqual(expectedType, combination.CombinationType);
            CheckAllPermutationsSame(cards);
        }

        private void CheckAllPermutationsSame(List<Card> cards)
        {
            Combination combination = new Combination(cards);
            foreach (var p in cards.ListPermutations())
            {
                Combination testCombination = new Combination(p);
                Assert.IsTrue(combination.CompareTo(testCombination) == 0);
            }
        }
    }
}
