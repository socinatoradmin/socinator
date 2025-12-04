using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response
{
    public class MediaRetweetsResponseHandler : MediaInteractionResponseHandler
    {
        public MediaRetweetsResponseHandler(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;

            try
            {
                if (response.Response.Contains("<!DOCTYPE html>"))
                    BrowserHtmlResponse(response);
                else if (response.Response.Contains("{\"globalObjects\":{\"tweets\":{") || response.Response.Contains("{\"data\":{\"retweeters_timeline\":{"))
                    BrowserAutomationresponsehandler(response);
                else
                    JsonResponse(response);
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }

        private void BrowserAutomationresponsehandler(IResponseParameter response)
        {
            UserList = new List<TwitterUser>();
            var jsonReponse = new Jsonhandler(response.Response);
            var userlist = jsonReponse.GetJToken("globalObjects", "users");
            if(userlist != null && userlist.HasValues)
            {
                foreach (var users in userlist)
                {
                    var UserTokens = users.First;
                    UserList.Add(new TwitterUser
                    {
                        Username = jsonReponse.GetJTokenValue(UserTokens, "screen_name"),
                        UserId = jsonReponse.GetJTokenValue(UserTokens, "id_str"),
                        IsPrivate = jsonReponse.GetJTokenValue(UserTokens, "protected") == "True",
                        HasProfilePic = jsonReponse.GetJTokenValue(UserTokens, "default_profile") == "True",
                        FollowStatus = jsonReponse.GetJTokenValue(UserTokens, "following") == "True",
                        FollowBackStatus = jsonReponse.GetJTokenValue(UserTokens, "followed_by") == "True",
                        IsMuted = jsonReponse.GetJTokenValue(UserTokens, "muting") == "True",
                        IsVerified = jsonReponse.GetJTokenValue(UserTokens, "verified") == "True",
                        JoiningDate = TdUtility.GetDateTime(jsonReponse.GetJTokenValue(UserTokens, "created_at"))
                    });
                }
            }
            else
            {
                userlist = jsonReponse.GetJToken("data", "retweeters_timeline", "timeline", "instructions",0, "entries");
                if(userlist != null && userlist.HasValues)
                {
                    MinPosition = jsonReponse.GetJTokenValue(userlist.Last, "content", "value");
                    foreach(var userNode in userlist)
                    {
                        var userToken = jsonReponse.GetJTokenOfJToken(userNode, "content", "itemContent", "user_results", "result");
                        var legacyToken = jsonReponse.GetJTokenOfJToken(userToken, "legacy");
                        UserList.Add(new TwitterUser
                        {
                            Username = jsonReponse.GetJTokenValue(legacyToken, "screen_name"),
                            UserId = jsonReponse.GetJTokenValue(userToken, "rest_id"),
                            IsPrivate = jsonReponse.GetJTokenValue(legacyToken, "protected") == "True",
                            HasProfilePic = jsonReponse.GetJTokenValue(legacyToken, "default_profile") == "True",
                            FollowStatus = jsonReponse.GetJTokenValue(legacyToken, "following") == "True",
                            FollowBackStatus = jsonReponse.GetJTokenValue(legacyToken, "followed_by") == "True",
                            IsMuted = jsonReponse.GetJTokenValue(legacyToken, "muting") == "True",
                            IsVerified = jsonReponse.GetJTokenValue(legacyToken, "verified") == "True",
                            JoiningDate = TdUtility.GetDateTime(jsonReponse.GetJTokenValue(legacyToken, "created_at"))
                        });
                    }
                }
            }
            
        }

        private void BrowserHtmlResponse(IResponseParameter response)
        {
            var htmlDocument = new HtmlDocument();
            UserList = new List<TwitterUser>();

            //js-stream-item stream-item stream-item
            //user-actions btn-group not-following not-muting

            var pageResponse =
                HtmlAgilityHelper.getStringTextFromClassName(response.Response, "activity-popup-dialog-users clearfix");
            htmlDocument.LoadHtml(pageResponse);
            var nodeList =
                HtmlAgilityHelper.GetNodesFromClassName("", "js-stream-item stream-item stream-item", htmlDocument);
            foreach (var node in nodeList)
            {
                var x = node.InnerHtml;
                UserList.Add(new TwitterUser
                {
                    //username u-dir u-textTruncate data-screen-name="
                    Username = Utilities.GetBetween(x, "data-screen-name=\"", "\""),
                    UserId = Utilities.GetBetween(x, "data-user-id=\"", "\""),
                    IsPrivate = x.Contains("data-protected=\"true\""),
                    HasProfilePic = !x.Contains("default_profile_images"),
                    FollowStatus = !x.Contains("not-following") && x.Contains("following"),
                    FollowBackStatus = x.Contains("FollowStatus"),
                    IsMuted = !x.Contains("not-muting"),
                    IsVerified = x.Contains("Icon--verified")
                });
            }
        }

        private void JsonResponse(IResponseParameter response)
        {
            try
            {
                var UserInfos = JObject.Parse(response.Response)["htmlUsers"].ToString();

                var listUserInfo = HtmlAgilityHelper.getListInnerHtmlFromClassName(UserInfos,
                    "js-actionable-user js-profile-popup-actionable");
                UserList = new List<TwitterUser>();
                listUserInfo.ForEach(x =>
                {
                    UserList.Add(new TwitterUser
                    {
                        Username = Utilities.GetBetween(x, "data-screen-name=\"", "\""),
                        UserId = Utilities.GetBetween(x, "data-user-id=\"", "\""),
                        IsPrivate = x.Contains("data-protected=\"true\""),
                        HasProfilePic = !x.Contains("default_profile_images"),
                        FollowStatus = !x.Contains("not-following") && x.Contains("following"),
                        FollowBackStatus = x.Contains("FollowStatus"),
                        IsMuted = !x.Contains("not-muting"),
                        IsVerified = x.Contains("Icon--verified")
                    });
                });
            }
            catch (Exception)
            {
            }
        }
    }
}