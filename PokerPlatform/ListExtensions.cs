using System.Collections.Generic;
using System.Linq;

namespace PokerPlatform
{
    public static class ListExtensions
    {
        // Use only for class or small structs
        public static void Swap<T>(this IList<T> list, int i1, int i2)
        {
            T tmp = list[i1];
            list[i1] = list[i2];
            list[i2] = tmp;
        }

        public static IEnumerable<IEnumerable<T>> ListPermutations<T>(this IEnumerable<T> s)
        {
            if (s == null)
            {
                yield break;
            }

            List<T> list = s.ToList();

            if (!list.Any())
            {
                yield return Enumerable.Empty<T>();
            }
            else
            {
                int ind = 0;
                foreach (var item in list)
                {
                    var otherItems = list.Take(ind).Concat(list.Skip(ind + 1)).ToList();
                    foreach (var p in otherItems.ListPermutations())
                    {
                        IEnumerable<T> concat(T first, IEnumerable<T> other)
                        {
                            yield return first;
                            foreach (var i in other)
                            {
                                yield return i;
                            }
                        };
                        yield return concat(item, p);
                    }
                    ++ind;
                }
            }
        }
    }
}
