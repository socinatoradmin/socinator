using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel.ReportModel;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;


// ReSharper disable once CheckNamespace
namespace LinkedDominatorCore.LDUtility
{
    public class Utils
    {
        public static string DesktopPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "CSV_DominatorHouse_SocialNetwork_Linkedin");

        private static readonly IDelayService DelayService = InstanceProvider.GetInstance<IDelayService>();

        public static Regex IdCheck = new Regex("^[0-9]*$");


        public static string GetMediaType(string extension)
        {
            try
            {
                return extension == ".jpg" ? "image/jpeg" : "image/" + extension.Replace(".", "");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return "";
            }
        }
        public static string GetBetween(string strSource, string strStart, string strEnd)
        {
            try
            {
                if (string.IsNullOrEmpty(strSource)?true:!strSource.Contains(strStart) || !strSource.Contains(strEnd))
                    return "";

                var start = strSource.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length;
                var end = strSource.IndexOf(strEnd, start, StringComparison.Ordinal);
                return strSource.Substring(start, end - start);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            return "";
        }

        public static string GetBetweenLast(string strSource, string strStart, string strEnd)
        {
            try
            {
                if (!strSource.Contains(strStart) || !strSource.Contains(strEnd))
                    return "";

                var start = strSource.LastIndexOf(strStart) + strStart.Length;
                var end = strSource.IndexOf(strEnd, start, StringComparison.Ordinal);
                return strSource.Substring(start, end - start);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            return "";
        }
        public static string GenerateNc()
        {
            var nc = string.Empty;
            try
            {
                var objRandom = new Random();

                try
                {
                    var unixTimestamp = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                    nc = unixTimestamp.ToString();

                    var randomThreeDigit = objRandom.Next(100, 999);
                    nc += randomThreeDigit.ToString();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return nc;
        }
        public static long GenerateNcInMilliSecond()
        {
            return (long) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
        public static string GenerateTrackingId()
        {
            var trackingId = string.Empty;

            var alphaNumericString = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ+/";
            var objRandom = new Random();

            //bCookie = objRandom.Next(10, 99).ToString();
            try
            {
                for (var i = 1; i <= 22; i++)
                {
                    var randomNum = objRandom.Next(0, alphaNumericString.Length);
                    trackingId += alphaNumericString[randomNum];
                }

                trackingId += "==";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return trackingId;
        }


        public static string InsertSpecialCharactersInCsv(string item)
        {
            try
            {
                if (item != null && (item.Contains(",") || item.Contains("\n") || item.Contains("\r") || item.Contains("\t")))
                    item = item.Replace("\"", "\"\"");
                return item;
            }
            catch (Exception)
            {
                //ignored
                return "";
            }
        }

        public static string AssignNa(string input)
        {
            try
            {
                return string.IsNullOrWhiteSpace(input) ? "N/A" : input?.Trim();
            }
            catch (Exception)
            {
                //ignored
                return "N/A";
            }
        }

        public static string RemoveHtmlTags(string pageResponse)
        {
            var removedHtmlTags = string.Empty;
            try
            {
                removedHtmlTags = Regex.Replace(pageResponse, "<(.*?)>", " ")?.Trim();
                removedHtmlTags = Regex.Replace(removedHtmlTags, "<(.*)?", " ")?.Trim();
                removedHtmlTags = Regex.Replace(removedHtmlTags, "(.*)?>", " ")?.Trim();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return removedHtmlTags;
        }

        public static string RemoveSpecialCharacters(string Response)=>string.IsNullOrEmpty(Response)?Response:Regex.Replace(Response, @"[^0-9A-Za-z:./\-\(\)’•]", " ")?.Replace("\r","")?.Replace("\n","")?.Replace("\r\n"," ")?.Replace("\t","")?.Replace("\\r","")?.Replace("\\n", "")?.Replace("\\t", "")?.Replace("\"","\"\"");
        public static void RandomDelay(int startDelay = 5000, int endDelay = 10000)
        {
            DelayService.ThreadSleep(new Random().Next(startDelay, endDelay));
        }


        public static bool IsMultiContains<T>(T checkInput, params T[] checkingInput)
        {
            return checkingInput != null && checkingInput.Contains(checkInput);
        }

        public static bool IsContains(string pageSource, params string[] containsList)
        {
            foreach (var contain in containsList)
                if (pageSource.Contains(contain))
                    return true;

            return false;
        }

        public static bool IsStringContainsAnyFromParams(string checkInput, params string[] checkingInput)
        {
            return !string.IsNullOrEmpty(checkInput) && checkingInput.Any(checkInput.Contains);
        }

        public static string GetMonth(string monthNum)
        {
            var monthDict = new Dictionary<string, string>
            {
                {"1", "January"}, {"2", "February"}, {"3", "March"}, {"4", "April"},
                {"5", "May"}, {"6", "June"}, {"7", "July"}, {"8", "August"},
                {"9", "September"}, {"10", "October"}, {"11", "November"}, {"12", "December"}
            };
            return monthDict.ContainsKey(monthNum) && !string.IsNullOrEmpty(monthNum)
                ? monthDict[monthNum]
                : "January";
        }

        public static int ConvertDoubleAndInt(string input)
        {
            return Convert.ToInt32(Convert.ToDouble(input));
        }

        public static string GetBetweenAndAddStart(string strSource, string strStart, string strEnd)
        {
            var value = GetBetween(strSource, strStart, strEnd);
            if (!string.IsNullOrEmpty(value))
                value = strStart + value;
            return value;
        }
        public static string GetMessageFilePath(string campaignName, string sender)
        {
            var folderPath = "";
            try
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (string.IsNullOrEmpty(sender))
                    folderPath =
                        $@"{documentsPath}\LinkedinMessage\{campaignName.Replace("-", "_").Replace("/", "_").Replace(":", "_")}";
                else
                    folderPath =
                        $@"{documentsPath}\LinkedinMessage\{campaignName.Replace("-", "_").Replace("/", "_").Replace(":", "_")}\{sender.Replace(".", "")}";

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return folderPath;
        }

        public static string GetLivechatAttachmentFilePath()
        {
            var folderPath = "";
            try
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                folderPath =
                    $@"{documentsPath}\{ConstantVariable.ApplicationName}\LinkedinLiveChatAttacment\{DateTime.Today}";

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return folderPath;
        }

        public static string GetDownloadedMediaPath()
        {
            var folderPath = "";
            try
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                folderPath = $@"{documentsPath}\{ConstantVariable.ApplicationName}\LinkedInMediaPath";

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return folderPath;
        }
        public static string GetMediaResolutionFromRemoteUrl(string RemoteUrl, CookieContainer cookieContainer = null)
        {
            var resolution = string.Empty;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(RemoteUrl);
                request.Method = "GET";
                request.CookieContainer = cookieContainer;
                var response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    var contentType = response?.ContentType.Split('/');
                    if (contentType.FirstOrDefault().Equals("image") || contentType.FirstOrDefault().Equals("Image"))
                    {
                        Image image = Image.FromStream(stream);
                        resolution = image.Width + "<:>" + image.Height;
                    }
                }
            }
            catch (Exception)
            {
                resolution = "<:>";
            }
            return resolution;
        }
        public static Tuple<JArray, bool, bool> GetArrayToken(string response,bool IsPeople=true, JsonJArrayHandler handler = null)
        {
            try
            {
                var jsonJArrayHandler = handler == null ? JsonJArrayHandler.GetInstance : handler;
                var RespJ = jsonJArrayHandler.ParseJsonToJObject(response);
                var Success = true;
                var HasMoreResults = false;
                var arrayElement = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "elements", 0, "socialDetail"));
                if (IsNullOrZeroLength(arrayElement))
                    arrayElement = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "included"));
                if (IsNullOrZeroLength(arrayElement) && IsPeople)
                    arrayElement = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "data", "searchDashClustersByAll", "elements"))?.FirstOrDefault(x => jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(x, "items")).Count > 2), "items"));
                if (IsNullOrZeroLength(arrayElement) && IsPeople)
                    arrayElement = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "elements"))?.FirstOrDefault(x => jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(x, "items")).Count > 2), "items"));
                if (IsNullOrZeroLength(arrayElement) && !IsPeople)
                    arrayElement = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "data", "searchDashClustersByAll", "elements"))?.FirstOrDefault(x => jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(x, "items")).Count > 2), "items"));
                if (IsNullOrZeroLength(arrayElement))
                    arrayElement = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "data", "searchDashClustersByAll", "elements", 0, "items"));
                if (IsNullOrZeroLength(arrayElement))
                    arrayElement = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "elements", 1, "items"));
                if (IsNullOrZeroLength(arrayElement))
                    arrayElement = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "elements", 0, "items"));
                if (IsNullOrZeroLength(arrayElement))
                    arrayElement = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "elements", 0, "results"));
                if (IsNullOrZeroLength(arrayElement) && IsNullOrZeroLength(arrayElement = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "elements", 0, "elements")))
                    && IsNullOrZeroLength(arrayElement = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "elements", 1, "elements")))
                    && IsNullOrZeroLength(arrayElement = jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "included"))) && IsNullOrZeroLength(arrayElement =jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "elements", 1, "results"))) && IsNullOrZeroLength(arrayElement =jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "elements", 2, "results"))) && IsNullOrZeroLength(arrayElement =jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "elements"))) && IsNullOrZeroLength(arrayElement =jsonJArrayHandler.GetJArrayElement(jsonJArrayHandler.GetJTokenValue(RespJ, "searchResults"))))
                {
                    Success = false;
                    HasMoreResults = true;
                }else
                    HasMoreResults = arrayElement != null && arrayElement?.Count > 0;
                return new Tuple<JArray, bool, bool>(arrayElement, Success, HasMoreResults);
            }
            catch(Exception ex)
            {
                ex.DebugLog(ex.GetBaseException().Message);
                return new Tuple<JArray, bool, bool>(new JArray(), false, false);
            }
        }
        public static bool IsNullOrZeroLength(JArray arrayElement)
        {
            return arrayElement == null || arrayElement.Count == 0;
        }
        public static string GetCompanyUniversalName(string pageUrl)
        {
            try
            {
                string[] endPoints = new string[] { "https:", "www.linkedin.com", "company", "people","jobs", "posts", "?feedView=all","about" };
                if (string.IsNullOrEmpty(pageUrl) || !pageUrl.Contains("/"))
                    return pageUrl;
                var data = pageUrl.Split('/').ToList();
                data.RemoveAll(x => x == string.Empty);
                data.RemoveAll(x => endPoints.Any(y => y == x));
                return WebUtility.UrlEncode(data.FirstOrDefault(x=>x!=string.Empty));
            }
            catch(Exception ex) { ex.DebugLog(ex.GetBaseException().Message);return pageUrl; }
        }
        public static string GetDecodedResponse(string Response, bool HtmlDecode = true, bool UrlDecode = true) =>
            string.IsNullOrEmpty(Response) ? Response : HtmlDecode && UrlDecode ? WebUtility.UrlDecode(WebUtility.HtmlDecode(Response)) : HtmlDecode ? WebUtility.HtmlDecode(Response) : WebUtility.UrlDecode(Response);
        public static bool IsJobOrWorkNotification(string NotificationType) => string.IsNullOrEmpty(NotificationType) ? false :
           (NotificationType.Contains("New Job Greeting") || NotificationType.Contains("Work Anniversary Greeting"));
    }

    public class LdUtils
    {
        public string PostsReportHeader(bool addAccount = true, bool addMyComment = false)
        {
            var listResource = new List<string>();
            if (addAccount)
                listResource.Add("LangKeyAccount");
            listResource.Add("LangKeyQueryType");
            listResource.Add("LangKeyQueryValue");
            listResource.Add("LangKeyActivityType");
            listResource.Add("LangKeyPostOwnerName");
            listResource.Add("LangKeyPostOwnerURl");
            listResource.Add("LangKeyPostUrl");
            listResource.Add("LangKeyPostDescription");
            if (addMyComment)
                listResource.Add("LangKeyMyComment");
            listResource.Add("LangKeyLikeCount");
            listResource.Add("LangKeyCommentCount");
            listResource.Add("LangKeyInteractionDateTime");

            return listResource.ReportHeaderFromResourceDict();
        }

        public string PostsReportCSVData(InteractedPostsReportModel model, bool addAccount = true,
            bool addMyComment = false)
        {
            return (addAccount ? $"{model.AccountEmail}," : "") +
                   $"{model.QueryType},{model.QueryValue},{model.ActivityType},{model.PostOwnerFullName},{model.PostOwnerProfileUrl}," +
                   $"{model.PostLink},{model.PostDescription.AsCsvData()},"
                   + (addMyComment ? $"{model.MyComment.AsCsvData()}," : "") +
                   $"{model.LikeCount},{model.CommentCount},{model.InteractionDateTime}";
        }
    }
}