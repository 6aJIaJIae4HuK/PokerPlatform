using PokerPlatform;
using System.Linq;
using System.Collections.Generic;
using Xunit;

namespace PokerPlatformUnitTests
{
    public class PotUnitTests
    {
        [Fact]
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
            Assert.Equal(3, pots.Count);
            Assert.Equal(6, pots[0].BuiltBy.Count);
            Assert.Equal(3, pots[1].BuiltBy.Count);
            Assert.Equal(1, pots[2].BuiltBy.Count);
            Assert.Equal(20u * 6, pots[0].Size);
            Assert.Equal(20u * 3, pots[1].Size);
            Assert.Equal(10u * 1, pots[2].Size);
            Assert.Equal(new List<int> { 0, 1, 2, 3, 4, 5 }, pots[0].BuiltBy.ToList());
            Assert.Equal(new List<int> { 3, 4, 5 }, pots[1].BuiltBy.ToList());
            Assert.Equal(new List<int> { 5 }, pots[2].BuiltBy.ToList());
        }
    }
}
