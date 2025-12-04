using DominatorHouseCore.Utility;
using System.Collections.Concurrent;

namespace DominatorHouseCore.Extensions
{
    public static class FixedSizedQueue
    {
        public static void FixedEnqueue<T>(this ConcurrentQueue<T> obj,T item)
        {
            obj.Enqueue(item);

            while (obj.Count > Constants.BrowserResourceCountLimit)
            {
                T outObj;
                obj.TryDequeue(out outObj);
            }
        }
        public static void Clear<T>(this ConcurrentQueue<T> queue)
        {
            T item;
            while (queue.TryDequeue(out item))
            {
                // do nothing
            }
        }
    }
}
