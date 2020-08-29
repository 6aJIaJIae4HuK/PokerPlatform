using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerPlatform
{
    public interface IPokerEvent {}

    public abstract class PokerEventWithTablePos : IPokerEvent
    {
        protected PokerEventWithTablePos(int tablePos)
        {
            TablePos = tablePos;
        }

        public int TablePos { get; }
    }

    public class ChangeStageEvent : IPokerEvent
    {
        public ChangeStageEvent(GameStage stage)
        {
            Stage = stage;
        }

        public GameStage Stage { get; }
    }

    public class BetEvent : PokerEventWithTablePos
    {
        public BetEvent(int tablePos, Bet bet)
            : base(tablePos)
        {
            Bet = bet;
        }
        public Bet Bet { get; }
    }

    public class FoldEvent : PokerEventWithTablePos
    {
        public FoldEvent(int tablePos)
            : base(tablePos)
        { }
    }

    public class FoldDueIncorrectEvent : PokerEventWithTablePos
    {
        public FoldDueIncorrectEvent(int tablePos, PlayerAction action)
            : base(tablePos)
        {
            Action = action;
        }

        public PlayerAction Action { get; }
    }

    public class AddHandCardsEvent : PokerEventWithTablePos
    {
        public AddHandCardsEvent(int tablePos, IReadOnlyCollection<Card> cards)
            : base(tablePos)
        {
            Cards = cards;
        }

        public IReadOnlyCollection<Card> Cards { get; }
    }

    public class AddCommonCardEvent : IPokerEvent
    {
        public AddCommonCardEvent(Card card)
        {
            Card = card;
        }

        public Card Card { get; }
    }

    public class ShowCardsEvent : PokerEventWithTablePos
    {
        public ShowCardsEvent(int tablePos, IReadOnlyCollection<Card> cards)
            : base(tablePos)
        {
            Cards = cards;
        }

        public IReadOnlyCollection<Card> Cards { get; }
    }
}
