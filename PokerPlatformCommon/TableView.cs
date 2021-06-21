using System.Collections.Generic;
using System.Linq;

namespace PokerPlatformCommon
{
    public class TableView
    {
        public TableView(Proto.TableView proto)
        {
            PlayersInner = proto.Players.Select(p => new PlayerView(p)).ToList();
            CurrentGame = proto.CurrentGame == null ? null : new GameView(proto.CurrentGame);
            SmallBlind = proto.SmallBlind;
            BigBlind = proto.BigBlind;
            Ante = proto.Ante;
            ButtonPosition = proto.ButtonPosition;
        }

        public IReadOnlyList<PlayerView> Players => PlayersInner;

        public GameView CurrentGame { get; private set; }

        public ulong SmallBlind { get; private set; }

        public ulong BigBlind { get; private set; }

        public ulong Ante { get; private set; }

        public int ButtonPosition { get; private set; }

        private readonly List<PlayerView> PlayersInner;
    }
}
