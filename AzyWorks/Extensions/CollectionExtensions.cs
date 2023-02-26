using System;
using System.Collections.Generic;
using System.Linq;

namespace AzyWorks.Extensions
{
    public static class CollectionExtensions
    {
        public static bool Match<T>(this IEnumerable<T> source, IEnumerable<T> target)
        {
            if (source.Count() != target.Count()) 
                return false;

            for (int i = 0; i < source.Count(); i++)
            {
                var item = source.ElementAt(i);
                var targetItem = target.ElementAt(i);

                if (item.Equals(targetItem))
                    continue;
                else
                    return false;
            }

            return true;
        }    

        public static bool TryDequeue<T>(this Queue<T> queue, out T value)
        {
            if (queue.Count <= 0)
            {
                value = default;
                return false;
            }

            value = queue.Dequeue();
            return true;
        }

        public static int FindIndex<T>(this T[] array, Func<T, bool> predicate)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (predicate(array[i]))
                    return i;
            }

            return -1;
        }

        public static bool TryGetKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value, out TKey key)
        {
            foreach (var pair in dictionary)
            {
                if (pair.Value.Equals(value))
                {
                    key = pair.Key;
                    return true;
                }
            }

            key = default;
            return false;
        }
    }
}