#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

#endregion

namespace DominatorHouseCore.Utility
{
    public static class IEnumerableExtensions
    {
        ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }

            return -1;
        }

        ///<summary>Finds the index of the first occurence of an item in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="item">The item to find.</param>
        ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
        public static int IndexOf<T>(this IEnumerable<T> items, T item)
        {
            return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));
        }


        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if(items!=null)
            foreach (var item in items)
                try
                {
                    action(item);
                }
                catch (IOException io)
                {
                    io.DebugLog();
                }
                catch (Exception e)
                {
                    e.DebugLog();
                }
        }


        /// <summary>
        ///     To replace the string in between two index with source
        /// </summary>
        /// <param name="source"> source string</param>
        /// <param name="index"> start location to replace at (0-based)</param>
        /// <param name="length"> number of characters to be removed before inserting</param>
        /// <param name="replace">the string that is replacing characters</param>
        /// <returns></returns>
        public static string ReplaceAt(this string source, int index, int length, string replace)
        {
            return source.Remove(index, Math.Min(length, source.Length - index))
                .Insert(index, replace);
        }


        public static bool IsGetMacros(this string source)
        {
            return Regex.Replace(source, @"{[^}]*}", string.Empty).Contains("{");
        }


        public static string ApplyMacros(this string source, int caretIndex, string selectedMacro)
        {
            var getFirstSubString = source.Substring(0, caretIndex);
            var startIndexOfCurrentWord = getFirstSubString.LastIndexOf("{", StringComparison.Ordinal);
            if (startIndexOfCurrentWord == -1)
                return source;

            var length = caretIndex - startIndexOfCurrentWord;

            return source.ReplaceAt(startIndexOfCurrentWord, length, selectedMacro);
        }
    }
}