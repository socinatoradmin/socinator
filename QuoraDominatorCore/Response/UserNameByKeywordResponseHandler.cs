using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.Response
{
    public class UserNameByKeywordResponseHandler : QuoraResponseHandler
    {
        public List<string> UserList = new List<string>();
        public int PaginationCount { get; set; }
        public bool HasMoreResult { get; set; }
        public UserNameByKeywordResponseHandler(IResponseParameter response,bool IsBrowser=true) : base(response)
        {
            try
            {
                if (IsBrowser)
                {
                    var decodedResponse = QdUtilities.GetDecodedResponse(response.Response);
                    var document = new HtmlDocument();
                    document.LoadHtml(decodedResponse);
                    var followersColleciton = document.DocumentNode
                        .SelectNodes("//a[@class='q-box qu-color--blue_dark qu-cursor--pointer qu-hover--textDecoration--underline b2c1r2a puppeteer_test_link']");
                    if (followersColleciton is null)
                        return;
                    var followers = followersColleciton.ToArray();
                    followers.ForEach(follower =>
                    {
                        var url = Utilities.GetBetween(follower.OuterHtml.Replace("\"", "'"), "href='", "'");
                        UserList.Add(url.Replace($"{QdConstants.HomePageUrl}/profile/", ""));
                    });
                    if(UserList.Count >=50)
                        UserList=UserList.Take(50).ToList();
                    //It is given as 49 because in browser first search and scroll
                    //It scrape 50 result so we need to perform pagination after 50 results,i.e 0-49.
                    PaginationCount = 49;
                    HasMoreResult = true;
                }
                else
                {
                    var jsonObject = jsonHandler.ParseJsonToJObject(response.Response);
                    var Users = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jsonObject, "data", "searchConnection", "edges"));
                    if (Users != null && Users.Count > 0)
                        Users.ForEach(User =>
                        {
                            var ProfileUrl = jsonHandler.GetJTokenValue(User, "node", "user", "profileUrl");
                            if (!string.IsNullOrEmpty(ProfileUrl))
                                UserList.Add(ProfileUrl.Replace("/profile/", ""));
                        });
                    int.TryParse(jsonHandler.GetJTokenValue(jsonObject, "data", "searchConnection", "pageInfo", "endCursor"), out int endCursor);
                    PaginationCount = endCursor;
                    bool.TryParse(jsonHandler.GetJTokenValue(jsonObject, "data", "searchConnection", "pageInfo", "hasNextPage"), out bool hasNextPage);
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