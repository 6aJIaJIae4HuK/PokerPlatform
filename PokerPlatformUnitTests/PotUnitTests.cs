using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PokerPlatform;
using System.Linq;
using System.Collections.Generic;

namespace PokerPlatformUnitTests
{
    [TestClass]
    public class PotUnitTests
    {
        [TestMethod]
        public void PotCheck()
        {
            var potBuilder = new PotBuilder();
            potBuilder.AddBet(0, new Bet(20, true));
            potBuilder.AddBet(1, new Bet(20, true));
            potBuilder.AddBet(2, new Bet(20, true));
            potBuilder.AddBet(3, new Bet(40, true));
            potBuilder.AddBet(4, new Bet(40, true));
            potBuilder.AddBet(5, new Bet(50, false));
            var pots = potBuilder.BuildPots();
            Assert.AreEqual(3, pots.Count);
            Assert.AreEqual(6, pots[0].BuiltBy.Count);
            Assert.AreEqual(3, pots[1].BuiltBy.Count);
            Assert.AreEqual(1, pots[2].BuiltBy.Count);
            Assert.AreEqual(20 * 6, pots[0].Size);
            Assert.AreEqual(20 * 3, pots[1].Size);
            Assert.AreEqual(10 * 1, pots[2].Size);
            CollectionAssert.AreEquivalent(new List<int> { 0, 1, 2, 3, 4, 5 }, pots[0].BuiltBy.ToList());
            CollectionAssert.AreEquivalent(new List<int> { 3, 4, 5 }, pots[1].BuiltBy.ToList());
            CollectionAssert.AreEquivalent(new List<int> { 5 }, pots[2].BuiltBy.ToList());
        }
    }
}
