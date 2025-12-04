#region

using System.Collections.Generic;

#endregion

namespace DominatorHouseCore.Utility
{
    public static class ListHelper
    {
        /// <summary>
        ///     Shuffles items of a given list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            for (var index = 0; index < list.Count; ++index)
                list.Swap(index, RandomUtilties.GetRandomNumber(list.Count - 1, index));
        }

        /// <summary>
        ///     Swaps two given integet values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            var obj = list[i];
            list[i] = list[j];
            list[j] = obj;
        }
    }
}