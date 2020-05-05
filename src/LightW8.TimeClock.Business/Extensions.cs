using System;
using System.Collections.Generic;

namespace LightW8.TimeClock.Business
{
    public static class IEnumberableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action) { foreach (var item in items) { action(item); } }
        public static IEnumerable<TReturn> ForEach<T, TReturn>(this IEnumerable<T> items, Func<T, TReturn> func) { foreach (var item in items) { yield return func(item); } }
    }
}
