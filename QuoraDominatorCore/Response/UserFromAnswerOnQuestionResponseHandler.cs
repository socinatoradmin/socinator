using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.Response
{
    public class UserFromAnswerOnQuestionResponseHandler : QuoraResponseHandler
    {
        public List<string> UserNames = new List<string>();
        public int PaginationCount { get; set; }
        public bool HasMoreResult { get; set; }
        public UserFromAnswerOnQuestionResponseHandler(IResponseParameter response,bool IsBrowser=true) : base(response)
        {
            try
            {
                if (IsBrowser)
                {
                    if(RespJ is null)
                    {
                        var followers = HtmlDocument.DocumentNode.SelectNodes("//a[@class='q-box Link___StyledBox-t2xg9c-0 dFkjrQ puppeteer_test_link qu-color--gray_dark qu-cursor--pointer qu-hover--textDecoration--underline']").ToList();
                        if (followers != null && followers.Count > 0)
                            followers.ForEach(follower =>
                            {
                                var username = follower.OuterHtml;
                                var url = Utilities.GetBetween(follower.OuterHtml.Replace("\"", "'"), "href='", "'");
                                url = string.IsNullOrEmpty(url) ? Utilities.GetBetween(username, "<span><span>", "</span></span></div></a>") : url;
                                if(!string.IsNullOrEmpty(url))
                                    UserNames.Add(url.Replace($"{QdConstants.HomePageUrl}/profile/", ""));
                            });
                    }
                    else
                    {
                        var objHtmlDocument = new HtmlDocument();
                        objHtmlDocument.LoadHtml(RespJ["value"].ToString());
                        var users = objHtmlDocument.DocumentNode.SelectNodes("//a[@class='user']").ToArray();
                        if (users != null && users.Length > 0)
                            users.ForEach(follower => {
                                var username = follower.Attributes["href"].Value.Replace("/profile/", "");
                                if(!string.IsNullOrEmpty(username))
                                    UserNames.Add(username);
                            });
                    }
                    if(UserNames.Count>=10)
                        UserNames=UserNames.Take(10).ToList();
                    HasMoreResult=true;
                    PaginationCount = 4;
                }
                else
                {
                    if (!string.IsNullOrEmpty(response.Response) && response.Response.Contains("--qgqlmpb"))
                    {
                        var Answers = Regex.Split(response.Response, "Content-Type: application/json;");
                        if (Answers != null && Answers.Length > 0)
                        {
                            foreach (var answer in Answers)
                            {
                                if (Answers.FirstOrDefault() == answer || Answers.LastOrDefault() == answer)
                                    continue;
                                var jsonObject = jsonHandler.ParseJsonToJObject(answer?.Replace("\r\n", "")?.Replace("--qgqlmpb", ""));
                                var answers = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "data", "node", "answers", "edges"));
                                if (answers != null && answers.HasValues)
                                    answers.ForEach(ans =>
                                    {
                                        var PublicIdentifier = jsonHandler.GetJTokenValue(ans, "node", "answer", "author", "profileUrl")?.Replace("/profile/", "");
                                        if (!string.IsNullOrEmpty(PublicIdentifier))
                                            UserNames.Add(PublicIdentifier);
                                    });
                                else
                                {
                                    jsonObject = jsonHandler.ParseJsonToJObject(answer?.Replace("\r\n", "")?.Replace("--qgqlmpb", ""));
                                    var PublicIdentifier = jsonHandler.GetJTokenValue(jsonObject, "data", "node", "answer", "author", "profileUrl")?.Replace("/profile/", "");
                                    if (!string.IsNullOrEmpty(PublicIdentifier))
                                        UserNames.Add(PublicIdentifier);
                                }
                            }
                            var PageInfo = jsonHandler.GetJTokenOfJToken(jsonHandler.ParseJsonToJObject(Answers.LastOrDefault()?.Replace("\r\n", "")?.Replace("--qgqlmpb", "")?.Replace("--", "")), "data", "pageInfo");
                            int.TryParse(jsonHandler.GetJTokenValue(PageInfo, "endCursor"), out int endCursor);
                            PaginationCount = endCursor;
                            bool.TryParse(jsonHandler.GetJTokenValue(PageInfo, "hasNextPage"), out bool hasNextPage);
                            HasMoreResult = hasNextPage;
                        }
                    }
                }
            }catch(Exception ex) { ex.DebugLog(); }
        }
    }
}