namespace PokerPlatform
{
    public enum PlayerActionType
    {
        BET = 1,
        FOLD,
        SHOW
    }

    public class PlayerAction
    {
        private PlayerAction() { }

        private PlayerAction(PlayerActionType type, uint size)
        {
            Type = type;
            Size = size;
        }

        public static PlayerAction Check()
        {
            return new PlayerAction(PlayerActionType.BET, 0);
        }

        public static PlayerAction Bet(uint size)
        {
            return new PlayerAction(PlayerActionType.BET, size);
        }

        public static PlayerAction Fold()
        {
            return new PlayerAction(PlayerActionType.FOLD, 0);
        }

        public static PlayerAction Show()
        {
            return new PlayerAction(PlayerActionType.SHOW, 0);
        }

        public PlayerActionType Type { get; }
        public uint Size { get; }
    }
}
