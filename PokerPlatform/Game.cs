using System;
using System.Collections.Generic;

namespace PokerPlatform
{
    public class Game
    {
        private class PlayerContext
        {
            public PlayerContext(IPlayer player, int tablePosition, IReadOnlyCollection<Card> handCards)
            {
                Player = player;
                TablePosition = tablePosition;
                HandCards = handCards;
            }

            public readonly IPlayer Player;
            public readonly int TablePosition;
            public readonly IReadOnlyCollection<Card> HandCards; // TODO: Is there really no way to create fixed size length array???
        }

        public Game(PokerTableSettings settings, IReadOnlyList<IPlayer> players, int buttonPosition, Deck deck)
        {
            if (players == null)
            {
                throw new ArgumentNullException("players"); // TODO: get param name at runtime???
            }

            if (deck == null)
            {
                throw new ArgumentNullException("deck");
            }

            if (players.Count < 2)
            {
                throw new ArgumentException("There must be at least two players", "players");
            }

            if (!(0 <= buttonPosition && buttonPosition < players.Count))
            {
                throw new ArgumentException("buttonPosition");
            }

            Settings = settings;
            Deck = deck;

            Players = new List<PlayerContext>();
            for (int pos = buttonPosition; pos < players.Count; ++pos)
            {
                if (players[pos] != null)
                {
                    Players.Add(new PlayerContext(players[pos], pos, TakeHandCards(Deck)));
                }
            }

            Stages = new Action[]
            {
                RunPreflop,
                RunFlop,
                RunTurn,
                RunRiver,
                RunShowdown
            };
        }

        public void Run()
        {
            foreach (Action stage in Stages)
            {
                stage();
                if (Players.Count == 1)
                    break;
            }
            GivePayoff();
        }

        private static IReadOnlyCollection<Card> TakeHandCards(Deck deck)
        {
            return new Card[]
            {
                deck.PeekTop().Value,
                deck.PeekTop().Value
            };
        }

        private void RunPreflop()
        {

        }

        private void RunFlop()
        {

        }

        private void RunTurn()
        {

        }

        private void RunRiver()
        {

        }

        private void RunShowdown()
        {

        }

        private void GivePayoff() // TODO: I don't know English :(
        {

        }

        private readonly IReadOnlyCollection<Action> Stages;

        private readonly PokerTableSettings Settings;
        private readonly List<PlayerContext> Players;
        private readonly Deck Deck;
    }
}
