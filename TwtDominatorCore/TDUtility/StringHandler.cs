using System;
using DominatorHouseCore;

namespace TwtDominatorCore.TDUtility
{
    public static class StringHandler
    {
        public static readonly Func<string, string, bool> IsEqualsIgnoreCase = (firstString, secondString) =>
        {
            try
            {
                return firstString.ToLower().Equals(secondString.ToLower());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        };

        public static readonly Func<string, string, bool> IsContainsIgnoreCase = (firstString, secondString) =>
        {
            try
            {
                return firstString.ToLower().Contains(secondString.ToLower());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        };
    }
}