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
                if (players[pos] != null && players[pos].StackSize > 0)
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

        private void HandleBet(int pos, Bet bet)
        {
            // TODO: make some difference between manual bets and ante/blinds
            Players[pos].Player.Withdraw(bet.Size);
            PotBuilder.AddBet(pos, bet);
            Console.WriteLine($"Player #{Players[pos].TablePosition} bet ({bet.Size}{(bet.IsAllIn ? ", all-in" : "")})");
            NotifyAllPlayers(new EventType()); // TODO: notify about bet
        }

        private void HandleFold(int pos, PlayerAction incorrectAction = null)
        {
            Players[pos].IsFolded = true;
            PotBuilder.RemovePlayer(pos);
            --LeftPlayers;
            if (incorrectAction != null)
            {
                NotifyAllPlayers(new EventType()); // TODO: notify about fold due to incorrect action
            }
            else
            {
                Console.WriteLine($"Player #{Players[pos].TablePosition} has been folded");
                NotifyAllPlayers(new EventType()); // TODO: notify about fold
            }
        }

        private void HandleShow(int pos)
        {
            NotifyAllPlayers(new EventType()); // TODO: notify about show cards
        }

        public void Run()
        {
            Console.WriteLine("================================");
            Console.WriteLine($"{String.Join("; ", Players.Select(pl => $"Player ${pl.TablePosition} has (${pl.StackSize})"))}");
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
                        HandleFold(pos); 
                        break;
                    case PlayerActionType.SHOW:
                        HandleShow(pos);
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
                        HandleFold(pos);
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
                        Bet betToRegister;
                        uint needToCall = currentBet - prevBet.Size;
                        if (action.Size < needToCall)
                        {
                            if (action.Size == Players[pos].StackSize)
                            {
                                betToRegister = new Bet(action.Size, isAllIn: true);
                            }
                            else
                            {
                                fallbackToFold = true;
                                break;
                            }
                        }
                        else
                        {
                            betToRegister = new Bet(action.Size, isAllIn: false);
                        }
                        HandleBet(pos, betToRegister);
                        break;
                    default:
                        fallbackToFold = true;
                        break;
                }
            }

            if (fallbackToFold)
            {
                HandleFold(pos, action);
            }
        }

        private void RunTradingRoundFrom(int startIndex)
        {
            int minCntToMove = Players.Count(pl => pl != null && !pl.IsFolded && pl.StackSize > 0);
            for (
                int i = 0, cur = startIndex;
                LeftPlayers > 1 && (i < minCntToMove || !PotBuilder.CanBuild);
                ++i, cur = NextCanMoveAfter(cur)
            ) {
                var action = Players[cur].Player.RequestMoveAsync().Result;
                HandlePlayerAction(cur, action, isShowdown: false);
            }
            var potsToAdd = PotBuilder.BuildPots();
            while (potsToAdd.Any() && potsToAdd.Last().BuiltBy.Count == 1)
            {
                uint returnSize = potsToAdd.Last().Size;
                int returnTo = potsToAdd.Last().BuiltBy.First();
                Players[returnTo].Player.Deposit(returnSize);
                potsToAdd.RemoveAt(potsToAdd.Count - 1);
                Console.WriteLine($"Return to #{Players[returnTo].TablePosition} pot({returnSize})");
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

        private int NextCanMoveAfter(int pos)
        {
            int cur = (pos + 1) % Players.Count;
            while (cur != pos && (Players[cur].IsFolded || Players[cur].StackSize == 0))
            {
                cur = (cur + 1) % Players.Count;
            }
            return cur;
        }

        private void RunPreflop()
        {
            Console.WriteLine("Preflop...");
            for (int i = 0; i < Players.Count; ++i)
            {
                var player = Players[i];
                HandleBet(
                    i,
                    new Bet(
                        Math.Min(player.Player.StackSize, Settings.Ante),
                        isAllIn: player.Player.StackSize <= Settings.Ante
                    )
                );
            }

            var smallBlindPos = SMALL_BLIND_POS % Players.Count;
            var bigBlindPos = BIG_BLIND_POS % Players.Count;
            var smallBlindPlayer = Players[smallBlindPos];
            var bigBlindPlayer = Players[bigBlindPos];
            HandleBet(
                smallBlindPos,
                new Bet(
                    Math.Min(smallBlindPlayer.Player.StackSize, Settings.SmallBlind),
                    isAllIn: smallBlindPlayer.Player.StackSize <= Settings.SmallBlind
                )
            );
            HandleBet(
                bigBlindPos,
                new Bet(
                    Math.Min(bigBlindPlayer.Player.StackSize, Settings.BigBlind),
                    isAllIn: bigBlindPlayer.Player.StackSize <= Settings.BigBlind
                )
            );
            RunTradingRoundFrom(NextCanMoveAfter(bigBlindPos));
        }

        private void RunFlop()
        {
            Console.WriteLine("Flop...");
            AddCommonCard();
            AddCommonCard();
            AddCommonCard();
            RunTradingRoundFrom(NextCanMoveAfter(DEALER_POS));
        }

        private void RunTurn()
        {
            Console.WriteLine("Turn...");
            AddCommonCard();
            RunTradingRoundFrom(NextCanMoveAfter(DEALER_POS));
        }

        private void RunRiver()
        {
            Console.WriteLine("River...");
            AddCommonCard();
            RunTradingRoundFrom(NextCanMoveAfter(DEALER_POS));
        }

        private void RunShowdown()
        {
            // TODO: request players in order to show/fold cards
            Console.WriteLine("Showdown...");
        }

        private void GivePayoff() // TODO: I don't know English :(
        {
            if (LeftPlayers == 1)
            {
                Console.WriteLine($"All players but #{Players.First(p => !p.IsFolded).TablePosition} folded");
                return;
            }
            var combinations = new List<(Combination Combination, int Position)>();
            foreach (var (player, pos) in Players.Select((pl, ind) => (pl, ind)))
            {
                if (player.IsFolded)
                    continue;
                var combination = new Combination(CommonCards);
                // TODO: enumerate C(7, 5) combinations instead of 7!
                foreach (var permuration in player.HandCards.Concat(CommonCards).ListPermutations())
                {
                    var newCombination = new Combination(permuration.Take(5));
                    if (combination.CompareTo(newCombination) < 0)
                    {
                        combination = newCombination;
                    }
                }
                combinations.Add((combination, pos));
            }
            combinations.Sort((x, y) => 
                {
                    int cmp = x.Combination.CompareTo(y.Combination);
                    if (cmp != 0)
                        return cmp;
                    return x.Position.CompareTo(y.Position);
                }
            );

            var winners = Enumerable
                .Repeat(new List<(Combination Combination, int Position)>(), Pots.Count)
                .ToList();

            // TODO: Is there need to use knowledge that if player x wins pot y then x wins all pots after y in which he participating?
            for (int i = 0; i < combinations.Count; ++i)
            {
                var (combination, pos) = combinations[i];
                for (int j = 0; j < Pots.Count; ++j)
                {
                    var pot = Pots[j];
                    if (pot.BuiltBy.Contains(pos))
                    {
                        if (!winners[j].Any() || winners[j].Last().Combination.CompareTo(combination) == 0)
                        {
                            winners[j].Add((combination, pos));
                        }
                    }
                }
            }

            for (int i = 0; i < winners.Count; ++i)
            {
                var pot = Pots[i];
                uint potSize = pot.Size;
                uint cnt = (uint)(winners[i].Count);
                foreach (int pos in winners[i].OrderBy(x => x.Position).Select(x => x.Position))
                {
                    uint toDeposit = potSize / cnt + (potSize % cnt == 0 ? 0u : 1u);
                    Players[pos].Player.Deposit(toDeposit);
                    Console.WriteLine($"Player ${Players[pos].TablePosition} won {toDeposit} from pot #{i} ({potSize})");
                }
            }
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
