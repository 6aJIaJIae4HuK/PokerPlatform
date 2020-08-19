using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerPlatform
{
    public class Pot
    {
        public Pot(uint size, IReadOnlyCollection<int> builtBy)
        {
            Size = size;
            BuiltBy = builtBy;
        }

        public uint Size { get; }

        public IReadOnlyCollection<int> BuiltBy { get; }
    }

    public struct Bet
    {
        public Bet(uint size, bool isAllIn)
        {
            Size = size;
            IsAllIn = isAllIn;
        }

        public uint Size { get; }
        public bool IsAllIn { get; }

        static public Bet operator+(Bet b1, Bet b2)
        {
            return new Bet(
                b1.Size + b2.Size,
                b1.IsAllIn || b2.IsAllIn
            );
        }
    }

    public class PotBuilder
    {
        private struct SizeAndPosition : IComparable<SizeAndPosition>
        {
            public SizeAndPosition(uint size, int position)
            {
                Size = size;
                Position = position;
            }

            public readonly uint Size;
            public readonly int Position;

            public int CompareTo(SizeAndPosition other)
            {
                if (Size != other.Size)
                    return Size.CompareTo(other.Size);
                return Position.CompareTo(other.Position);
            }
        }


        private readonly Dictionary<int, Bet> Bets = new Dictionary<int, Bet>();
        private readonly List<SizeAndPosition> SortedBySizeAllIn = new List<SizeAndPosition>();
        private readonly List<SizeAndPosition> SortedBySizeNotAllInNotFolded = new List<SizeAndPosition>();
        private uint FoldedSize = 0;

        public bool CanBuild =>
            (!SortedBySizeNotAllInNotFolded.Any() || SortedBySizeNotAllInNotFolded.First().Size == SortedBySizeNotAllInNotFolded.Last().Size) &&
            (!SortedBySizeNotAllInNotFolded.Any() || !SortedBySizeAllIn.Any() || SortedBySizeAllIn.Last().Size <= SortedBySizeNotAllInNotFolded.First().Size);

        public uint BetLevel => SortedBySizeNotAllInNotFolded.Any() ? SortedBySizeNotAllInNotFolded.Last().Size : 0;

        public Bet this[int index]
        {
            get
            {
                if (Bets.TryGetValue(index, out Bet res))
                    return res;
                return new Bet();
            }
        }

        public void AddBet(int pos, Bet bet)
        {
            Bets.TryGetValue(pos, out Bet oldBet);
            if (oldBet.IsAllIn)
            {
                throw new InvalidCastException($"Player #{pos} already gone all-in");
            }
            Bets[pos] = oldBet + bet;
            SizeAndPosition toRemove = new SizeAndPosition(oldBet.Size, pos);
            SizeAndPosition toInsert = new SizeAndPosition(oldBet.Size + bet.Size, pos);
            SortedBySizeNotAllInNotFolded.Remove(toRemove);
            var listToInsert = bet.IsAllIn ? SortedBySizeAllIn : SortedBySizeNotAllInNotFolded;
            int index = listToInsert.BinarySearch(toInsert);
            index = (index < 0 ? ~index : index);
            listToInsert.Insert(index, toInsert);
        }

        public void RemovePlayer(int pos)
        {
            if (!SortedBySizeNotAllInNotFolded.Remove(new SizeAndPosition(Bets[pos].Size, pos)))
            {
                throw new ArgumentNullException("This player already folded or already gone all-in");
            }
            FoldedSize += Bets[pos].Size;
        }

        public List<Pot> BuildPots()
        {
            if (!CanBuild)
            {
                throw new InvalidOperationException("Cannot build pots at the moment");
            }

            List<Pot> res = new List<Pot>();

            uint prev = 0;
            var list = SortedBySizeAllIn.Concat(SortedBySizeNotAllInNotFolded).ToList();
            uint allCnt = (uint)list.Count;
            int prevOffset = 0;
            int curOffset = 0;
            uint toSubtract = 0;
            bool isFirst = true;
            foreach (var item in list)
            {
                if (item.Size > prev || isFirst)
                {
                    if (!isFirst)
                    {
                        res.Add(new Pot(
                            (prev - toSubtract) * (allCnt - (uint)prevOffset),
                            list.Skip(prevOffset).Select(i => i.Position).ToList()
                        ));
                        prevOffset = curOffset;
                        toSubtract = prev;
                    }
                    prev = item.Size;
                }
                ++curOffset;
                isFirst = false;
            }

            res.Add(new Pot(
                (prev - toSubtract) * (allCnt - (uint)prevOffset) + FoldedSize, // All bets from folded should be transfered to main pot
                list.Skip(prevOffset).Select(i => i.Position).ToList()
            ));

            Bets.Clear();
            SortedBySizeAllIn.Clear();
            SortedBySizeNotAllInNotFolded.Clear();
            FoldedSize = 0;
            return res;
        }
    }
}
