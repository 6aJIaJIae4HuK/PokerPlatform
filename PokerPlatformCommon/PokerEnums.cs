﻿namespace PokerPlatformCommon
{
    public enum Rank
    {
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIX,
        SEVEN,
        EIGHT,
        NINE,
        TEN,
        JACK,
        QUEEN,
        KING,
        ACE
    }

    public enum Suit
    {
        HEARTS,
        CLUBS,
        SPIDES,
        DIAMONDS
    }

    public enum CombinationType
    {
        HIGHCARD,
        ONEPAIR,
        TWOPAIRS,
        THREEOFKIND,
        STRAIGHT,
        FLUSH,
        FULLHOUSE,
        FOUROFKIND,
        STRAIGHTFLUSH
    }

    public enum Street
    {
        PREFLOP,
        FLOP,
        TURN,
        RIVER,
        SHOWDOWN,
        PAYOFF
    }
}
