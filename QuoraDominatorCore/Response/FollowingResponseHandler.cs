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
    public class FollowingsResponseHandler : QuoraResponseHandler
    {
        public List<string> FollowingList = new List<string>();
        public int PaginationCount { get; set; }
        public bool HasMoreResult { get; set; }
        public FollowingsResponseHandler(IResponseParameter response,bool IsBrowser=true) : base(response)
        {
            try
            {
                if(IsBrowser)
                {
                    var html = new HtmlDocument();
                    html.LoadHtml(response.Response);
                    var followings = html.DocumentNode.SelectNodes("//div[@class=\"q-box qu-color--gray_dark qu-borderBottom qu-tapHighlight--none qu-display--flex qu-alignItems--center\"]").ToArray();
                    if (followings != null)
                        followings.ForEach(following =>
                        {
                            var url = Utilities.GetBetween(following.OuterHtml.Replace("\"", "'"), "href='", "'");
                            url = Regex.Unescape(Regex.Replace(url, "\\\\([^u])", "\\\\$1")).Replace("\\", "");
                            FollowingList.Add(url.Replace($"{QdConstants.HomePageUrl}/profile/", ""));
                        });
                    if(FollowingList.Count >=10)
                        FollowingList=FollowingList.Take(10).ToList();
                    HasMoreResult = true;
                    PaginationCount = 9;
                }
                else
                {
                    var jsonObject = jsonHandler.ParseJsonToJObject(response.Response);
                    var Followings = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "data", "user", "followingUsersConnection", "edges"));
                    if (Followings != null && Followings.Count > 0)
                        Followings.ForEach(Following =>
                        {
                            var ProfileId = jsonHandler.GetJTokenValue(Following, "node", "profileUrl");
                            if (!string.IsNullOrEmpty(ProfileId))
                                FollowingList.Add(ProfileId?.Replace("/profile/", "")?.Replace("/", ""));
                        });
                    int.TryParse(jsonHandler.GetJTokenValue(jsonObject, "data", "user", "followingUsersConnection", "pageInfo", "endCursor"), out int endCursor);
                    PaginationCount = endCursor;
                    bool.TryParse(jsonHandler.GetJTokenValue(jsonObject, "data", "user", "followingUsersConnection", "pageInfo", "hasNextPage"), out bool hasNextPage);
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