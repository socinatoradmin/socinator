using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.Response
{
    public class FollowerResponseHandler : QuoraResponseHandler
    {
        public List<string> FollowerList { get; set; } = new List<string>();
        public int PaginationCount { get; set; }
        public bool HasMoreResult { get; set; }
        public FollowerResponseHandler(IResponseParameter response,bool IsBrowser=false) : base(response)
        {
            try
            {
                if (IsBrowser)
                {
                    var followers = HtmlDocument.DocumentNode.SelectNodes("//div[@class=\"q-box qu-color--gray_dark qu-borderBottom qu-tapHighlight--none qu-display--flex qu-alignItems--center\"]").ToArray();
                    if (followers != null)
                        followers.ForEach(follower =>
                        {
                            var url = Utilities.GetBetween(follower.OuterHtml.Replace("\"", "'"), "href='", "'");
                            url = Regex.Unescape(Regex.Replace(url, "\\\\([^u])", "\\\\$1")).Replace("\\", "");
                            FollowerList.Add(WebUtility.UrlDecode(url.Replace($"{QdConstants.HomePageUrl}/profile/", "")));
                        });
                    if(FollowerList.Count >=10)
                        FollowerList=FollowerList.Take(10).ToList();
                    PaginationCount = 9;
                    HasMoreResult = true;
                }
                else
                {
                    var jsonObject = jsonHandler.ParseJsonToJObject(response.Response);
                    var Followers = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "data", "user", "followerUsersConnection", "edges"));
                    if (Followers != null && Followers.Count > 0)
                        Followers.ForEach(Follower =>
                        {
                            var ProfileId = jsonHandler.GetJTokenValue(Follower, "node", "profileUrl");
                            if (!string.IsNullOrEmpty(ProfileId))
                                FollowerList.Add(ProfileId?.Replace("/profile/", "")?.Replace("/", ""));
                        });
                    int.TryParse(jsonHandler.GetJTokenValue(jsonObject, "data", "user", "followerUsersConnection", "pageInfo", "endCursor"), out int endCursor);
                    PaginationCount = endCursor;
                    bool.TryParse(jsonHandler.GetJTokenValue(jsonObject, "data", "user", "followerUsersConnection", "pageInfo", "hasNextPage"), out bool hasNextPage);
                    HasMoreResult = hasNextPage;
                }
                
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}