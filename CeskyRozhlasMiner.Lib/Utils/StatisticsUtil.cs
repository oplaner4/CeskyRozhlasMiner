using System;
using System.Collections.Generic;

namespace CeskyRozhlasMiner.Lib.Utils
{
    internal class StatisticsUtil
    {
        internal static Dictionary<Key, int> GetPropAndCount<Key, T>(
            IEnumerable<T> collection, Func<T, Key> keyCreator)
        {
            Dictionary<Key, int> result = new();

            foreach (T item in collection)
            {
                Key key = keyCreator(item);

                if (result.ContainsKey(key))
                {
                    result[key]++;
                }
                else
                {
                    result[key] = 1;
                }
            }

            return result;
        }
    }
}
