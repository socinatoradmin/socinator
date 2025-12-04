using DominatorHouseCore;
using DominatorHouseCore.Enums.TumblrQuery;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using TumblrDominatorCore.Models;

namespace TumblrDominatorCore.TmblrUtility
{
    public class TumblrUtility
    {
        public static string GenerateJsonPayload(object obj)
        {
            var urlFormData = string.Empty;
            var serializedPostData = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            return serializedPostData;
        }

        public static string GetDecodedResponseOfJson(string response)
        {
            string decodedResponse = "";
            if (!response.IsValidJson() && response.Contains("}}csrf :") && !response.EndsWith("}}"))
                decodedResponse = Regex.Replace(response, "}}csrf :(.*)?", "") + "}}";
            if (!response.IsValidJson() && response.Contains("window['___INITIAL_STATE___'] =") && response.Contains("</script>"))
                decodedResponse = Utilities.GetBetween(WebUtility.HtmlDecode(response), "window['___INITIAL_STATE___'] =", "</script>")?.Trim()?.TrimEnd(';');
            if (!response.IsValidJson() && response.Contains("id=\"___INITIAL_STATE___\">") && response.Contains("</script>"))
                decodedResponse = Utilities.GetBetween(WebUtility.HtmlDecode(response), "id=\"___INITIAL_STATE___\">", "</script>")?.Trim()?.TrimEnd(';');
            if (!decodedResponse.IsValidJson())
            {
                try
                {
                    decodedResponse = Regex.Replace(response, "\\\\([^u])", "\\\\$1").Replace("\\", "");
                    decodedResponse = WebUtility.HtmlDecode(decodedResponse).Replace("u003C", "<").Replace("u00252C", ",");

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    decodedResponse = response;
                    decodedResponse = WebUtility.HtmlDecode(decodedResponse).Replace("u003C", "<").Replace("u00252C", ",");
                }
            }
            if (!decodedResponse.IsValidJson() && decodedResponse.Contains("<!--"))
            {
                decodedResponse = decodedResponse.Replace("<!--", string.Empty).Replace("--!>", string.Empty);
            }
            return decodedResponse;

        }

        public static TumblrPost getTumblrPostFromPostUrl(string postUrl)
        {
            var post = new TumblrPost() { PostUrl = postUrl };
            var PostUrlSplittedList = new List<string>();
            if (!postUrl.Contains("/post/") && postUrl.Contains(".tumblr.com"))
            {
                var SplitedQuery = Regex.Split(postUrl, "\\?source");
                PostUrlSplittedList = SplitedQuery[0].Split('/').ToList<string>();
                post.OwnerUsername = Utilities.GetBetween(postUrl, "//.tumblr.com/", "/");
                if (string.IsNullOrEmpty(post.OwnerUsername))
                    post.OwnerUsername = PostUrlSplittedList[3];
                post.BlogName = post.OwnerUsername;
            }
            else if (postUrl.Contains("/post/") && postUrl.Contains(".tumblr.com"))
            {
                PostUrlSplittedList = postUrl.Split('/').ToList<string>();
                post.OwnerUsername = Utilities.GetBetween(postUrl, "https://", ".tumblr.com/");
                if (string.IsNullOrEmpty(post.OwnerUsername))
                    post.OwnerUsername = PostUrlSplittedList.SingleOrDefault(x => x.Contains(".tumblr.com")).TrimEnd(".tumblr.com".ToArray());
                post.BlogName = post.OwnerUsername;
            }
            post.Id = PostUrlSplittedList.SingleOrDefault(x => !string.IsNullOrEmpty(x) && IsIntegerOnly(x));
            if (string.IsNullOrEmpty(post.Id))
                post.Id = PostUrlSplittedList[4];
            if (PostUrlSplittedList.Count > 4)
                post.PostName = PostUrlSplittedList.LastOrDefault(x => !string.IsNullOrEmpty(x) && !x.Equals(post.Id));

            return post;
        }
        public static TumblrUser getTumblrUserFromPostUrlorBlogUrlOrUsername(string value)
        {
            var user = new TumblrUser();
            var splittedQueryList = value.Split('/').ToList<string>();
            if (value.StartsWith(ConstantHelpDetails.TumblrUrl))
            {
                user.Username = Utilities.GetBetween(value + "/", "//www.tumblr.com/", "/");
                if (string.IsNullOrEmpty(user.Username))
                    user.Username = splittedQueryList[3];
            }
            else if (value.EndsWith(".tumblr.com/") || value.EndsWith(".tumblr.com") || value.Contains(".tumblr.com/post/"))
            {
                user.Username = Utilities.GetBetween(value, "https://", ".tumblr.com");
                if (string.IsNullOrEmpty(user.Username))
                    user.Username = splittedQueryList.SingleOrDefault(x => x.Contains(".tumblr.com"))?.TrimEnd(".tumblr.com".ToCharArray());
            }
            else if (!value.Contains(".tumblr.com"))
                user.Username = value;
            user.UserId = user.Username;
            user.PageUrl = ConstantHelpDetails.BlogViewUrl(user.Username);
            return user;
        }
        public static bool IsVedio(string fileExtension)
        {
            return new List<string>
                {
                    ".3g2", ".3gp", ".3gpp", ".asf", ".avi", ".dat", ".divx", ".dv", ".f4v", ".flv", ".m2ts", ".m4v", ".mkv",
                    ".mod", ".mov", ".mp4", ".mpe", ".mpeg", ".mpeg4", ".mpg", ".mts", ".nsv", ".ogm", ".ogv", ".qt", ".tod", ".ts",
                    ".vob", ".wmv"
                }.Contains(fileExtension);
        }
        public static bool isImage(string fileExtension)
        {
            var tumblrSupportedImageFormat = new List<string>
                {
                    ".jpeg",".jpg",".png",".gif",".tif", ".tiff",".bmp"
                };
            if (tumblrSupportedImageFormat.Contains(fileExtension)) return true;
            return false;
        }
        public static bool isAudio(string fileExtension)
        {
            var tumblrSupportedAudiofomat = new List<string>
                {
                    ".mp3",".aa",".aac",".mp4a",".wav"
                };
            if (tumblrSupportedAudiofomat.Contains(fileExtension)) return true;
            return false;
        }
        public static KeyValuePair<int, int> GetScreenResolution()
        {
            int height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            int width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            return new KeyValuePair<int, int>(width, height);
        }
        public static bool IsIntegerOnly(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;

            if (!input.Any(char.IsNumber))
                return false;

            var match = Regex.Match(input, "^[0-9]+$");
            return match.Success;
        }
        public static byte[] ConvertMediaFileToByte(string mediaPath)
        {
            try
            {
                using (FileStream fileStream = new FileStream(mediaPath, FileMode.Open, FileAccess.Read))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fileStream.CopyTo(memoryStream);
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception)
            {
                return new byte[0];
            }

        }
        public static string GetJsonFromPageResponse(string pageResponse)
        {
            var json = string.Empty;
            try
            {
                json = Utilities.GetBetween(WebUtility.HtmlDecode(pageResponse), "window['___INITIAL_STATE___'] = ", "</script>")?.Replace("undefined", "\"\"")?.Replace("\r\n", "")?.Replace("\t", "")?.Replace(" ", "");
                json = string.IsNullOrEmpty(json) ? Utilities.GetBetween(WebUtility.HtmlDecode(pageResponse), "id=\"___INITIAL_STATE___\">", "</script>")?.Replace("undefined", "\"\"")?.Replace("\r\n", "")?.Replace("\t", "")?.Replace(" ", "") : json;
                if (!json.IsValidJson())
                    json = json.Remove(json.Length - 2, 1);
                if (json.IsValidJson())
                    return json;
                else if (!string.IsNullOrEmpty(json) && json.Contains("\"PeeprRoute\":{\"initialTimeline\""))
                {
                    json = Utilities.GetBetween(json, "\"PeeprRoute\":", ",\"queries\":");
                    json = Utilities.ValidateJsonString(json);
                    if (json.IsValidJson()) return json;
                    return string.Empty;
                }
                else
                    json = string.Empty;
            }
            catch (Exception) { }
            return json;
        }
        public static (bool, bool, bool) GetActivityByQuery(string QueryType)
        {
            if (QueryType == EnumUtility.GetQueryFromEnum(TumblrQuery.UserCommentedOnPost))
                return (true, false, false);
            if (QueryType == EnumUtility.GetQueryFromEnum(TumblrQuery.UserReblogedThePost))
                return (false, true, false);
            if (QueryType == EnumUtility.GetQueryFromEnum(TumblrQuery.UserLikedThePost))
                return (false, false, true);
            else
                return (true, false, false);
        }
    }
}
