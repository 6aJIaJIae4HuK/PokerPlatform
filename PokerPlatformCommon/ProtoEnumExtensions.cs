using System;
using System.Collections.Generic;
using System.Text;

namespace PokerPlatformCommon
{
    public static class ProtoEnumExtensions
    {
        public static Rank ToRank(this Proto.Rank rank)
        {
            return rank switch
            {
                Proto.Rank.Two => Rank.TWO,
                Proto.Rank.Three => Rank.THREE,
                Proto.Rank.Four => Rank.FOUR,
                Proto.Rank.Five => Rank.FIVE,
                Proto.Rank.Six => Rank.SIX,
                Proto.Rank.Seven => Rank.SEVEN,
                Proto.Rank.Eight => Rank.EIGHT,
                Proto.Rank.Nine => Rank.NINE,
                Proto.Rank.Ten => Rank.TEN,
                Proto.Rank.Jack => Rank.JACK,
                Proto.Rank.Queen => Rank.QUEEN,
                Proto.Rank.King => Rank.KING,
                Proto.Rank.Ace => Rank.ACE,
                _ => throw new ArgumentException()
            };
        }

        public static Suit ToSuit(this Proto.Suit suit)
        {
            return suit switch
            {
                Proto.Suit.Hearts => Suit.HEARTS,
                Proto.Suit.Clubs => Suit.CLUBS,
                Proto.Suit.Spides => Suit.SPIDES,
                Proto.Suit.Diamonds => Suit.DIAMONDS,
                _ => throw new ArgumentException()
            };
        }

        public static Street ToStreet(this Proto.Street street)
        {
            return street switch
            {
                Proto.Street.Preflop => Street.PREFLOP,
                Proto.Street.Flop => Street.FLOP,
                Proto.Street.Turn => Street.TURN,
                Proto.Street.River => Street.RIVER,
                Proto.Street.Showdown => Street.SHOWDOWN,
                Proto.Street.Payoff => Street.PAYOFF,
                _ => throw new ArgumentException()
            };
        }
    }
}
