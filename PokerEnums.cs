﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerPlatform
{
    public enum Rank
    {
        ACE,
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
}
