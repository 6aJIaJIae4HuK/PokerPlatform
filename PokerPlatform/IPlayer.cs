using System;
using System.Threading.Tasks;

namespace PokerPlatform
{
    // Can be either bot with collecting all events or real player with pass every event to client
    public abstract class Player
    {
        protected Player(uint initStackSize)
        {
            StackSize = initStackSize;
        }

        public uint StackSize { get; private set; }

        public void Withdraw(uint amount)
        {
            if (amount > StackSize)
            {
                throw new ArgumentException("Cannot withdraw amount above existing", "amount");
            }
            StackSize -= amount;
        }

        public void Deposit(uint amount)
        {
            StackSize += amount;
        }

        public abstract void HandleEvent(object ev);

        public abstract Task<PlayerAction> RequestMoveAsync();
    }

    public class BotPlayer : Player
    {
        public BotPlayer(uint initStackSize)
            : base(initStackSize)
        { }

        public override void HandleEvent(object ev)
        {
        }

        public override async Task<PlayerAction> RequestMoveAsync()
        {
            return await Task.Run(() =>
            {
                Task.Delay(500).Wait();
                return PlayerAction.Bet(StackSize);
            });
        }
    }
}
