using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventType = System.Object;

namespace PokerPlatform
{
    public class Game
    {
        private class PlayerContext
        {
            public PlayerContext(Player player, int tablePosition, IReadOnlyCollection<Card> handCards)
            {
                Player = player;
                TablePosition = tablePosition;
                HandCards = handCards;
            }

            public bool IsFolded { get; set; } = false;
            public uint StackSize => Player.StackSize;

            public readonly Player Player;
            public readonly int TablePosition;
            public readonly IReadOnlyCollection<Card> HandCards; // TODO: Is there really no way to create fixed size length array???
        }

        public Game(PokerTableSettings settings, IReadOnlyList<Player> players, int buttonPosition, Deck deck)
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
                Console.WriteLine($"Player #{Players[pos].TablePosition} got {String.Join(" and ", Players[pos].HandCards)}");
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

        private void HandlePlayerAction(int pos, PlayerAction action, bool isShowdown)
        {
            if (Players[pos].IsFolded)
            {
                throw new InvalidOperationException($"Player #{Players[pos].TablePosition} already folded");
            }

            bool fallbackToFold = false;
            if (isShowdown)
            {
                switch (action.Type)
                {
                    case PlayerActionType.FOLD:
                        Players[pos].IsFolded = true;
                        // TODO: notify about fold 
                        break;
                    case PlayerActionType.SHOW:
                        // TODO: notify about show cards
                        break;
                    default:
                        fallbackToFold = true;
                        break;
                }
            }
            else
            {
                switch (action.Type)
                {
                    case PlayerActionType.FOLD:
                        Players[pos].IsFolded = true;
                        // TODO: notify about fold
                        break;
                    case PlayerActionType.BET:
                        uint currentBet = PotBuilder.BetLevel;
                        Bet prevBet = PotBuilder[pos];
                        if (prevBet.IsAllIn)
                        {
                            fallbackToFold = true;
                            break;
                        }
                        if (action.Size < Players[pos].StackSize)
                        {
                            fallbackToFold = true;
                            break;
                        }
                        uint needToCall = currentBet - prevBet.Size;
                        if (action.Size < needToCall)
                        {
                            if (action.Size == Players[pos].StackSize)
                            {
                                Players[pos].Player.Withdraw(action.Size);
                                PotBuilder.AddBet(pos, new Bet(action.Size, isAllIn: true));
                            }
                            else
                            {
                                fallbackToFold = true;
                            }
                        }
                        else
                        {
                            Players[pos].Player.Withdraw(action.Size);
                            PotBuilder.AddBet(pos, new Bet(action.Size, isAllIn: false));
                        }
                        // TODO: notify about bet
                        break;
                    default:
                        fallbackToFold = true;
                        break;
                }
            }

            if (fallbackToFold)
            {
                Players[pos].IsFolded = true;
                // TODO: Notify incorrect action
            }
        }

        private void StartTradingRoundFrom(int startIndex)
        {
            int cur = startIndex;
            int leftPlayers = LeftPlayers;
            for (int i = 0; i < leftPlayers || !PotBuilder.CanBuild; ++i)
            {
                var action = Players[cur].Player.RequestMoveAsync().Result;
                HandlePlayerAction(cur, action, isShowdown: false);
                NotifyAllPlayers(new EventType()); // Event type action
                cur = NextNotFoldAfter(cur);
            }
            var potsToAdd = PotBuilder.BuildPots();
            while (potsToAdd.Any() && potsToAdd.Last().BuiltBy.Count == 1)
            {
                uint returnSize = potsToAdd.Last().Size;
                int returnTo = potsToAdd.Last().BuiltBy.First();
                // TODO: return pot
                potsToAdd.RemoveAt(potsToAdd.Count - 1);
            }

            if (Pots.Any() && potsToAdd.Any() && Pots.Last().BuiltBy.Count == potsToAdd.First().BuiltBy.Count)
            {
                var lastSize = Pots.Last().Size;
                var lastBuiltBy = Pots.Last().BuiltBy;
                Pots.RemoveAt(Pots.Count - 1);
                Pots.Add(new Pot(lastSize + potsToAdd.First().Size, lastBuiltBy));
                Pots.AddRange(potsToAdd.Skip(1));
            }
            else
            {
                Pots.AddRange(potsToAdd);
            }
            Pots.AddRange(PotBuilder.BuildPots());
        }

        private void NotifyOnePlayer(int index, EventType ev)
        {
            Players[index].Player.HandleEvent(ev);
        }

        private void NotifyAllPlayers(EventType ev)
        {
            foreach (var player in Players)
            {
                player.Player.HandleEvent(ev);
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
                player.Player.Withdraw(Settings.Ante);
            }

            Players[SMALL_BLIND_POS % Players.Count].Player.Withdraw(Settings.SmallBlind);
            Players[BIG_BLIND_POS % Players.Count].Player.Withdraw(Settings.BigBlind);
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
        private readonly PotBuilder PotBuilder = new PotBuilder();
        private readonly List<Pot> Pots = new List<Pot>();

        private const int DEALER_POS = 0;
        private const int SMALL_BLIND_POS = 1;
        private const int BIG_BLIND_POS = 2;
    }
}
