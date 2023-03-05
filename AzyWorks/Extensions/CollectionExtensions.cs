using AzyWorks.Pooling.Pools;
using AzyWorks.System;

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

        public static void Shuffle<T>(this ICollection<T> source)
        {
            var copy = ListPool<T>.Instance.Get();
            var size = copy.Count;

            while (size > 1)
            {
                size--;

                var index = RandomGenerator.Int32(0, size + 1);
                var value = copy.ElementAt(index);

                copy[index] = copy[size];
                copy[size] = value;
            }

            source.Clear();

            foreach (var value in copy)
                source.Add(value);

            ListPool<T>.Instance.Push(copy);
        }

        public static void AddRange<T>(this ICollection<T> destination, IEnumerable<T> source)
        {
            foreach (var item in source)
            {
                destination.Add(item);
            }
        }

        public static void AddRange<T>(this ICollection<T> destination, IEnumerable<T> source, Func<T, bool> condition)
        {
            foreach (var item in source)
            {
                if (!condition(item))
                    continue;

                destination.Add(item);
            }
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