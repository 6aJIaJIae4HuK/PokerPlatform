using System.Collections.Generic;

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
    }
}
