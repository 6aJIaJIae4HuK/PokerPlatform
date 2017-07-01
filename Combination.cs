using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerPlatform
{
    public class Combination : IComparable<Combination>
    {
        public Combination(IEnumerable<Card> cards)
        {
            if (cards.Count() != 5)
                throw new Exception("There are must be exactly 5 cards!");
            this.cards = new List<Card>(cards);
            combinationType = getCombinationType();
        }

        public int CompareTo(Combination other)
        {
            if (this.CombinationType != other.CombinationType)
                return ((int)this.CombinationType).CompareTo((int)other.CombinationType);
            throw new NotImplementedException();
        }

        private CombinationType getCombinationType()
        {
            return CombinationType.FLUSH;
        }

        private readonly List<Card> cards;
        private readonly CombinationType combinationType;
        public CombinationType CombinationType { get { return combinationType; } }
        public IList<Card> Cards { get { return new ReadOnlyCollection<Card>(cards); } }
    }
}
