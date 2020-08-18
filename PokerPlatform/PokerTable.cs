using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PokerPlatform
{
    public struct PokerTableSettings
    {
        public PokerTableSettings(uint smallBlind, uint bigBlind, uint ante)
        {
            if (smallBlind >= bigBlind || smallBlind == 0 || bigBlind == 0)
            {
                throw new ArgumentException("There must be 0 < smallBlind < bigBlind and ante >= 0");
            }
            SmallBlind = smallBlind;
            BigBlind = bigBlind;
            Ante = ante;
        }
        public readonly uint SmallBlind;
        public readonly uint BigBlind;
        public readonly uint Ante;
        
        //TODO: Currency. Where?
    }
    public class PokerTable
    {
        public PokerTable(int capacity, PokerTableSettings settings)
        {
            Capacity = capacity;
            ButtonPosition = -1;
            Settings = settings;
            Players = Enumerable.Repeat<Player>(null, capacity).ToList();
        }

        private void RelaxButtonPosition()
        {
            if (ButtonPosition == -1 || Players[ButtonPosition] == null)
            {
                int init = ButtonPosition == -1 ? 0 : ButtonPosition;
                int cur = init;
                while (Players[cur] == null)
                {
                    cur = (cur + 1) % Capacity;
                    if (cur == init)
                    {
                        ButtonPosition = -1;
                        return;
                    }
                }
                ButtonPosition = cur;
            }
        }

        public void SetPlayer(int pos, Player player)
        {
            if (Players[pos] != null || player == null)
            {
                throw new ArgumentException();
            }
            Players[pos] = player;
            RelaxButtonPosition();
        }

        private CancellationTokenSource gameTokenSource = new CancellationTokenSource();
        private CancellationToken gameToken;
        private Task gameCycleTask;

        private readonly List<Action> commandsToExecuteBetweenRounds = new List<Action>();
        private readonly object lockCommands = new object();

        public void SetSettings(PokerTableSettings settings)
        {
            lock (lockCommands)
            {
                commandsToExecuteBetweenRounds.Add(
                    () =>
                    {
                        Settings = settings;
                    }
                );
            }
        }

        private void RunGamesCycle(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Deck.ShuffleCards();
                {
                    Game game = new Game(Settings, Players, ButtonPosition, Deck);
                    game.Run();
                }
                lock (lockCommands)
                {
                    foreach (var command in commandsToExecuteBetweenRounds)
                    {
                        command();
                    }
                    commandsToExecuteBetweenRounds.Clear();
                }

                do
                {
                    ButtonPosition = (ButtonPosition + 1) % Capacity;
                } while (Players[ButtonPosition] == null);
            }
        }

        public void StartGames()
        {
            if (gameCycleTask != null && !gameCycleTask.IsCompleted)
            {
                throw new Exception("Games already started");
            }
            gameToken = gameTokenSource.Token;
            gameCycleTask = Task.Run(() => RunGamesCycle(gameToken), gameToken);
        }

        public void StopGames()
        {
            if (gameCycleTask == null || gameCycleTask.IsCompleted)
            {
                throw new Exception("Games not running");
            }
            SendStopGames();
            gameCycleTask.Wait();
        }

        public void SendStopGames()
        {
            gameTokenSource.Cancel();
        }

        public int Capacity { get; }
        public PokerTableSettings Settings { get; private set; }
        public int ButtonPosition { get; private set; }
        

        private readonly List<Player> Players;
        private readonly Deck Deck = new Deck();

        static public PokerTable CreateForSixPlayers(PokerTableSettings settings)
        {
            return new PokerTable(6, settings);
        }

        static public PokerTable CreateForNinePlayers(PokerTableSettings settings)
        {
            return new PokerTable(9, settings);
        }
    }
}
