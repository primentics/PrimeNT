using AzyWorks.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;

namespace AzyWorks.Randomization.Weighted
{
    public static class WeightPicker
    {
        public static T Pick<T>(this IEnumerable<T> collection) where T : IWeightable
        {
            if (!TryPick<T>(collection, out var result))
                result = default;

            return result;
        }

        public static bool TryPick<T>(this IEnumerable<T> collection, out T result) where T : IWeightable
        {
            if (!collection.Any())
            {
                result = default;
                return false;
            }

            var totalWeight = collection.Sum(x => x.Weight);
            var choice = StaticRandom.RandomInt(0, totalWeight);
            var sum = 0;

            foreach (var item in collection)
            {
                for (int i = sum; i < item.Weight + sum; i++)
                {
                    if (i >= choice)
                    {
                        result = item;
                        return true;
                    }
                }

                sum += item.Weight;
            }

            result = collection.First();
            return true;
        }

        public static T Pick<T>(this IEnumerable<T> collection, Func<T, int> weightPicker)
        {
            if (!TryPick<T>(collection, weightPicker, out var result))
                result = default;

            return result;
        }

        public static bool TryPick<T>(this IEnumerable<T> collection, Func<T, int> weightPicker, out T result)
        {
            if (!collection.Any())
            {
                result = default;
                return false;
            }

            var totalWeight = collection.Sum(x => weightPicker(x));
            var choice = StaticRandom.RandomInt(0, totalWeight);
            var sum = 0;

            foreach (var item in collection)
            {
                var itemWeight = weightPicker(item);

                for (int i = sum; i < itemWeight + sum; i++)
                {
                    if (i >= choice)
                    {
                        result = item;
                        return true;
                    }
                }

                sum += itemWeight;
            }

            result = collection.First();
            return true;
        }
    }
}
