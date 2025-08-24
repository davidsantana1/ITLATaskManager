using System.Collections.Concurrent;

namespace ITLATaskManager.Utils
{
    public static class MemoizationUtility
    {
        public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> func)
        {
            var cache = new ConcurrentDictionary<T, TResult>();
            return (a) => cache.GetOrAdd(a, func);
        }
    }
}