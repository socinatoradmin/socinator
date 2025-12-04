#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace DominatorHouseCore.Utility
{
    public static class RandomUtilties
    {
        public static Random ObjRandom { get; } = new Random(Guid.NewGuid().GetHashCode());

        /// <summary>
        ///     This Medthod is used to get the random item from the list
        /// </summary>
        /// <typeparam name="T">The Type of the element of the inputList</typeparam>
        /// <param name="inputList">List of value for get the random number from it</param>
        /// <returns>Returns the random item from input list</returns>
        public static T GetRandomItem<T>(this IList<T> inputList)
        {
            // Get the random index from list max count and min count 
            var index = GetRandomNumber(inputList.Count - 1);

            // return the random item from the input list based on index value
            return inputList[index];
        }


        /// <summary>
        ///     GetRandomNumber method is used to get the random value which lies from maxValue and MinValue
        /// </summary>
        /// <param name="maxValue">Maxvalue is define the upper bound</param>
        /// <param name="minValue">Maxvalue is define the lower bound</param>
        /// <returns>Return the random value lies from upper and lower</returns>
        public static int GetRandomNumber(int maxValue, int minValue = 0)
        {
            // increase the maxValue for getting a chance for select a last item as random number
            ++maxValue;
            // Collect the random value from min and max value
            return ObjRandom.Next(minValue, maxValue);
        }


        /// <summary>
        ///     GetRandomString method is used to get the random string from alphanumeric character with required length
        /// </summary>
        /// <param name="outputStringLength">The required string count from alphanumeric character </param>
        /// <returns>returns random string from alphanumeric character</returns>
        public static string GetRandomString(int outputStringLength)
        {
            // returns random string from alphanumeric character
            return new string
            (Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", outputStringLength)
                .Select(s => s[ObjRandom.Next(s.Length)]).ToArray());
        }
    }
}