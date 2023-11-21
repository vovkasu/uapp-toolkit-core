using System;
using System.Collections.Generic;
using System.Linq;

namespace UAppToolKit.Core.ExtendedMethods
{
    public static class ListExtendedMethods
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T GetRandomItem<T>(this IList<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static T GetRandomItemByWeight<T>(this IList<T> list, Func<T, int> heightGetter)
        {
            int allWeights = 0;
            var weighed = new Dictionary<int, T>();
            
            foreach (var item in list)
            {
                allWeights += heightGetter(item);
                weighed.Add(allWeights, item);
            }
            var range = UnityEngine.Random.Range(0, allWeights + 1);
            foreach (var weighedKey in weighed.Keys)
            {
                if (range <= weighedKey)
                {
                    return weighed[weighedKey];
                }
            }

            return weighed.Last().Value;
        }
    }
}