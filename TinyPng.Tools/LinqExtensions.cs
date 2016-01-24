using System;
using System.Collections.Generic;

namespace TinyPng.Tools
{
    internal static class LinqExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, Int32> action)
        {
            var index = 0;
            foreach (var item in enumerable)
            {
                action(item, index);
                index++;
            }
        }
    }
}

