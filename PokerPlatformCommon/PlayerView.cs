namespace PokerPlatformCommon
{
    public class PlayerView
    {
        public PlayerView(Proto.Player proto)
        {
            Id = proto.Id;
            StackSize = proto.StackSize;
        }

        public ulong Id { get; private set; }

        public ulong StackSize { get; private set; }
    }
}
