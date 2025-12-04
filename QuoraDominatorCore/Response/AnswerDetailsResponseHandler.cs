using System;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.Response
{
    public class AnswerDetailsResponseHandler : QuoraResponseHandler
    {
        public int AnsweredLast; //588
        public string AnsweredUserUrl; //https://www.quora.com/Why-is-the-sun-considered-the-biggest-star-in-the-solar-system/answer/Yechiel-Kay
        public int AnswerView; //81500
        public int UpvoteCount; //6100
        public string Username;
        public string ProfileUrl;
        public AnswerDetailsResponseHandler(IResponseParameter response,bool IsBrowser=true) : base(response)
        {
            if (!Success)
                return;
            try
            {
                if (IsBrowser)
                {
                    var html = new HtmlDocument();
                    html.LoadHtml(response.Response);
                    AnsweredUserUrl =Utilities.GetBetween(response.Response, "\\\"permaUrlOnOriginalQuestion\\\":\\\"", "\\\",").Replace("\\\\u00e", "%C3%A").Replace("\\\\u2019", "%E2%80%99");
                    AnsweredUserUrl = AnsweredUserUrl.Contains(".quora.com") ? AnsweredUserUrl : $"{QdConstants.HomePageUrl}{AnsweredUserUrl}";
                    if (!(AnsweredUserUrl.StartsWith($"{QdConstants.HomePageUrl}") || AnsweredUserUrl.StartsWith("https://fr.quora.com/")))
                    {
                        var profileData = Regex.Split(response.Response, "profileUrl");
                        ProfileUrl = Utilities.GetBetween(profileData.Length > 4 ? profileData[4] : string.Empty, "\\\":\\\"", "\\\"");
                        var lastAnswerdDateLists = Regex.Split(response.Response, "q-box qu-cursor--pointer qu-hover--textDecoration--underline answer_timestamp b2c1r2a puppeteer_test_link");
                        var lastAnsweredDateString = Utilities.GetBetween(lastAnswerdDateLists.Length > 1? lastAnswerdDateLists[1]:string.Empty, "inherit;\">", "</a>").Replace("Answered", "").Replace("Updated", "").Trim();
                        var lastAnsweredDate = QdTimeStampUtility.ConvertTimestamp(lastAnsweredDateString);
                        AnsweredLast = QdUtilities.GetDateDifferenceFromTimeStamp(lastAnsweredDate);
                    }
                    else
                    {
                        var view = Utilities.GetBetween(response.Response, "\\\"numViews\\\":", ",");
                        int ansview = 0;
                        if (view.Contains("k"))
                        {
                            int.TryParse(view.Replace("k", ""), out ansview);
                            ansview *= 1000;
                        }
                        else
                            int.TryParse(view.Trim(), out ansview);
                        AnswerView = ansview;
                        var Nodes = HtmlParseUtility.GetListNodeFromPartialTagNamecontains(response.Response, "a", "class", "q-box qu-cursor--pointer qu-hover--textDecoration--underline answer_timestamp b2c1r2a puppeteer_test_link");
                        var lastAnsweredDateString = Nodes?.FirstOrDefault()?.InnerHtml?.Replace("Answered", "")?.Replace("Updated", "")?.Trim();
                        var upvote = Utilities.GetBetween(response.Response, "\\\"numUpvotes\\\":", ",");
                        int.TryParse(upvote, out int upvoteCount);
                        UpvoteCount = upvoteCount;
                        var lastAnsweredDate = QdTimeStampUtility.ConvertTimestamp(lastAnsweredDateString);
                        AnsweredLast = QdUtilities.GetDateDifferenceFromTimeStamp(lastAnsweredDate);
                    }
                }
                else
                {
                    var jsonString = QdConstants.GetJsonForAllTypePosts(response.Response.Replace("\\\"", "\"").Replace("\\\"", "\""), "answer");
                    if(!jsonString.Contains("author"))
                        jsonString = QdConstants.GetJsonForAllTypePosts(response.Response.Replace("\\\"", "\"").Replace("\\\"", "\""), "tribeItem");
                    var jsonObject = jsonHandler.ParseJsonToJObject(jsonString);
                    var AnswerDetails = jsonHandler.GetJTokenOfJToken(jsonObject, "data", "answer" );
                    if(!AnswerDetails.HasValues)
                        AnswerDetails = jsonHandler.GetJTokenOfJToken(jsonObject, "data", "tribeItem", "answer");
                    var profileUrl = jsonHandler.GetJTokenValue(AnswerDetails, "author", "profileUrl").Replace("\\u00f8", "ø");
                    if(!string.IsNullOrEmpty(profileUrl))
                    {
                        ProfileUrl = profileUrl.Contains(".quora.com/") ? profileUrl : $"{QdConstants.HomePageUrl}{profileUrl}";
                        Username = profileUrl.Split('/').LastOrDefault(y => y != string.Empty);
                    }
                    int.TryParse(jsonHandler.GetJTokenValue(AnswerDetails, "numViews"), out AnswerView);
                    var url = jsonHandler.GetJTokenValue(AnswerDetails, "question","url");
                    if(!string.IsNullOrEmpty(url))
                    {
                        url = url.Contains("https://") ? url : $"{QdConstants.HomePageUrl}{url}";
                        AnsweredUserUrl = url.EndsWith("/") ? url.Contains("www.quora.com") ? url + $"answer/{Username}" : url : url.Contains("www.quora.com") ? url + $"/answer/{Username}" : url;
                    }
                    long.TryParse(jsonHandler.GetJTokenValue(AnswerDetails, "creationTime"), out long lastAnswerTimeStamp);
                    AnsweredLast = QdUtilities.GetDateDifferenceFromTimeStamp(lastAnswerTimeStamp / 1000000);

                    //var jsonObject = jsonHandler.ParseJsonToJObject(jsonString);
                    //var UserDetails = jsonHandler.GetJTokenOfJToken(jsonObject, "data", "answer", "author");
                    //var AnswerDetails = jsonHandler.GetJTokenOfJToken(jsonObject, "data", "answer", "question");
                    //var profileUrl = jsonHandler.GetJTokenValue(UserDetails, "profileUrl");
                    //if (!string.IsNullOrEmpty(profileUrl))
                    //{
                    //    ProfileUrl = profileUrl.Contains(".quora.com/") ? profileUrl: $"{QdConstants.HomePageUrl}{profileUrl}";
                    //    Username = profileUrl.Split('/').LastOrDefault(y => y != string.Empty);
                    //}
                    //int.TryParse(jsonHandler.GetJTokenValue(UserDetails, "allTimePublicAnswerViews"), out int answerViewCount);
                    //AnswerView = answerViewCount;
                    //var url = jsonHandler.GetJTokenValue(AnswerDetails, "url");
                    //if (!string.IsNullOrEmpty(url))
                    //{
                    //    url = url.Contains("https://") ? url : $"{QdConstants.HomePageUrl}{url}";
                    //    AnsweredUserUrl =url.EndsWith("/")?url.Contains("www.quora.com")?url+ $"answer/{Username}":url:url.Contains("www.quora.com")? url + $"/answer/{Username}":url;
                    //}
                    //long.TryParse(jsonHandler.GetJTokenValue(UserDetails, "creationTime"),out long lastAnswerTimeStamp);
                    //var time = new DateTime(lastAnswerTimeStamp/1000000);
                    //AnsweredLast = QdUtilities.GetDateDifferenceFromTimeStamp(lastAnswerTimeStamp/1000000);
                }
            }catch(Exception ex) { ex.DebugLog(); }
        }
    }
}