using System;
using System.Text.RegularExpressions;
using System.Web;
using DominatorHouseCore;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Helper
{
    public class TweetHelper
    {
        public static string GetEmoji(string TweetContainer)
        {
            var TwtText = string.Empty;
            try
            {
                #region Get tweet with their emoji

                TwtText = HtmlAgilityHelper.getStringInnerHtmlFromClassName(TweetContainer, "js-tweet-text-container");
                var matches = Regex.Matches(TwtText, "<img class=.Emoji Emoji--forText(.*?)alt=\"(.*?)\"(.*?)>");
                foreach (Match match in matches)
                    TwtText = TwtText.Replace(match.Groups[0].ToString(), match.Groups[2].ToString());

                TwtText = Regex.Replace(TwtText, "(.)http", spaceReplacer);
                TwtText = Regex.Replace(TwtText, "<.*?>", "").Replace("\n", " ").Replace(@"\s+", " ");
                TwtText = Regex.Replace(TwtText, @"\s+", " ");
                TwtText = HttpUtility.HtmlDecode(TwtText).Trim();

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return TwtText;
        }

        public static string spaceReplacer(Match match)
        {
            var replacedString = match.Groups[0].ToString();
            var matchedChar = char.Parse(match.Groups[1].ToString());
            try
            {
                if (!char.IsWhiteSpace(matchedChar))
                    replacedString = replacedString.Replace(matchedChar.ToString(), matchedChar + " ");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return replacedString;
        }

        public static bool IsLikedPost(string tweetResponse)
        {
            var IsLiked = false;
            try
            {
                var pattern = @"js-original-tweet(\s*?)favorited";
                var match = Regex.Match(tweetResponse, pattern);
                if (match.Success)
                {
                    IsLiked = true;
                }
                else
                {
                    // checking again if own tweet liked
                    pattern = @"favorited has-cards";
                    match = Regex.Match(tweetResponse, pattern);
                    if (match.Success || tweetResponse.Contains("my-tweet favorited with-social-proof"))
                        IsLiked = true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return IsLiked;
        }
    }
}