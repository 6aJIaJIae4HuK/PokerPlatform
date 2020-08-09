using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventType = System.Object;
using MoveType = System.Object;

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

            public int Withdraw(int amount)
            {
                //TODO: Withdraw from player
                return 0;
            }

            public void Deposit(int amount)
            {
                //TODO: deposit to player
            }

            public async Task<MoveType> RequestMoveAsync()
            {
                //TODO: call from player request to move
                return await Task.Run(() =>
                {
                    Console.Write($"Player #{TablePosition} to move... ");
                    Task.Delay(500).Wait();
                    Console.WriteLine("Done");
                    return new object();
                });
            }

            public void Notify(EventType ev)
            {
                //TODO: call event handler of player
            }

            public bool IsFolded { get; private set; } = false;

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
            for (int offset = 0; offset < players.Count; ++offset)
            {
                int pos = (buttonPosition + offset) % players.Count;
                if (players[pos] != null)
                {
                    Players.Add(new PlayerContext(players[pos], pos, TakeHandCards(Deck)));
                }
            }
            LeftPlayers = Players.Count;

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
            Console.WriteLine("================================");
            Console.WriteLine($"Button is player #{Players[DEALER_POS].TablePosition}");
            for (int pos = 0; pos < Players.Count; ++pos)
            {
                NotifyOnePlayer(pos, new EventType()); // Got two cards
                Console.WriteLine($"Player #{pos} got {String.Join(" and ", Players[pos].HandCards)}");
            }
            foreach (Action stage in Stages)
            {
                stage();
                if (LeftPlayers == 1)
                    break;
            }
            GivePayoff();
        }

        private static IReadOnlyCollection<Card> TakeHandCards(Deck deck)
        {
            return new Card[]
            {
                deck.PeekTop(),
                deck.PeekTop()
            };
        }

        private void StartTradingRoundFrom(int startIndex)
        {
            int cur = startIndex;
            int leftPlayers = LeftPlayers;
            int movedPlayers = 0;
            while (movedPlayers < leftPlayers) // TODO: and all bets are equal
            {
                _ = Players[cur].RequestMoveAsync().Result;
                NotifyAllPlayers(new EventType()); // Event type action
                ++movedPlayers;
                cur = NextNotFoldAfter(cur);
            }
        }

        private void NotifyOnePlayer(int index, EventType ev)
        {
            Players[index].Notify(ev);
        }

        private void NotifyAllPlayers(EventType ev)
        {
            foreach (var player in Players)
            {
                player.Notify(ev);
            }
        }

        private void AddCommonCard()
        {
            var card = Deck.PeekTop();
            CommonCards.Add(card);
            Console.WriteLine($"Added {card}");
            NotifyAllPlayers(new EventType()); // Event added common card
        }

        private int NextNotFoldAfter(int pos)
        {
            int cur = (pos + 1) % Players.Count;
            while (Players[cur].IsFolded)
            {
                cur = (cur + 1) % Players.Count;
            }
            return cur;
        }

        private void RunPreflop()
        {
            Console.WriteLine("Preflop...");
            foreach (var player in Players)
            {
                player.Withdraw(Settings.Ante);
            }

            Players[SMALL_BLIND_POS % Players.Count].Withdraw(Settings.SmallBlind);
            Players[BIG_BLIND_POS % Players.Count].Withdraw(Settings.BigBlind);
            StartTradingRoundFrom(NextNotFoldAfter(BIG_BLIND_POS % Players.Count));
        }

        private void RunFlop()
        {
            Console.WriteLine("Flop...");
            AddCommonCard();
            AddCommonCard();
            AddCommonCard();
            StartTradingRoundFrom(NextNotFoldAfter(DEALER_POS));
        }

        private void RunTurn()
        {
            Console.WriteLine("Turn...");
            AddCommonCard();
            StartTradingRoundFrom(NextNotFoldAfter(DEALER_POS));
        }

        private void RunRiver()
        {
            Console.WriteLine("River...");
            AddCommonCard();
            StartTradingRoundFrom(NextNotFoldAfter(DEALER_POS));
        }

        private void RunShowdown()
        {
            // TODO: request players in order to show/fold cards
            Console.WriteLine("Showdown...");
        }

        private void GivePayoff() // TODO: I don't know English :(
        {
            // TODO: there assume that all players not folded (only for showing)
            var combinations = new List<(Combination Combination, int TablePosition)>();
            foreach (var player in Players)
            {
                var combination = new Combination(CommonCards);
                foreach (var permuration in player.HandCards.Concat(CommonCards).ListPermutations())
                {
                    var newCombination = new Combination(permuration.Take(5));
                    if (combination.CompareTo(newCombination) < 0)
                    {
                        combination = newCombination;
                    }
                }
                combinations.Add((combination, player.TablePosition));
            }
            combinations.Sort((x, y) => 
                {
                    int cmp = x.Combination.CompareTo(y.Combination);
                    if (cmp != 0)
                        return cmp;
                    return x.TablePosition.CompareTo(y.TablePosition);
                }
            );
            var (winnerCombination, winnerPosition) = combinations.Last();
            Console.WriteLine($"Won player #{winnerPosition} with combination {String.Join(",", winnerCombination.Cards)} ({winnerCombination.CombinationType})");
        }

        private readonly IReadOnlyCollection<Action> Stages;

        private readonly PokerTableSettings Settings;
        private readonly List<PlayerContext> Players;
        private int LeftPlayers;
        private readonly Deck Deck;
        private readonly List<Card> CommonCards = new List<Card>();
        private readonly List<Pot> Pots = new List<Pot>();

        private const int DEALER_POS = 0;
        private const int SMALL_BLIND_POS = 1;
        private const int BIG_BLIND_POS = 2;
    }
}
