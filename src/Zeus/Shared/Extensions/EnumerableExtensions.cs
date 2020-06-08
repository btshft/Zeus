using System.Collections.Generic;
using System.Linq;

namespace Zeus.Shared.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<Indexed<T>> Index<T>(this IEnumerable<T> enumerable) =>
            enumerable.Select((arg1, i) => new Indexed<T>(i, arg1));

        public struct Indexed<T>
        {
            public readonly int Index;
            public readonly T Item;

            public Indexed(int index, T item)
            {
                Index = index;
                Item = item;
            }

            public void Deconstruct(out int index, out T item)
            {
                index = Index;
                item = Item;
            }
        }
    }
}