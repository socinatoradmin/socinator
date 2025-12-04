#region

using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore.Diagnostics;

#endregion

namespace DominatorHouseCore.Utility
{
    public static class MacrosHelper
    {
        /// <summary>
        ///     Macros patterns
        /// </summary>
        private static readonly Regex MacrosParser =
            new Regex(@"{[^}]*}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        ///     Substitute the macros actual texts with macros
        /// </summary>
        /// <param name="text">given text which inclues macros</param>
        /// <returns></returns>
        public static string SubstituteMacroValues(string text)
        {
            // Intialize all macros
            SocinatorInitialize.InitializeMacros();
            // Call the replace macros       
            return MacrosParser.Replace(text, MacrosMatchEvaluator);
        }

        /// <summary>
        ///     Macros match evaluators
        /// </summary>
        /// <param name="match">matched items</param>
        /// <returns></returns>
        private static string MacrosMatchEvaluator(Match match)
        {
            // fetch the value of macros with respective with macros key
            var matchReplaceValue = SocinatorInitialize.Macros.FirstOrDefault(x => x.Key == match.Value)?.Value;
            // Check an empty value, and replace with actual value
            return string.IsNullOrEmpty(matchReplaceValue) ? match.Value : matchReplaceValue;
        }
    }
}