using System;
using System.Collections.Generic;
using UnityEngine;


namespace AudioSystem
{
    public static class FillSelectedDictionary
    {
        public static void Fill<TKey, TValue>(
       Dictionary<TKey, TValue> dict,
       IEnumerable<TValue> items,
       Func<TValue, TKey> keySelector)
        {
            dict.Clear();

            foreach (var item in items)
            {
                if (item == null) continue;

                TKey key = keySelector(item);
                dict[key] = item;
            }
        }
    }
}