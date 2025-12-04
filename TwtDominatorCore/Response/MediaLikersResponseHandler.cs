using System;
using System.Collections.Generic;
using System.Globalization;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response
{
    public class MediaLikersResponseHandler : MediaInteractionResponseHandler
    {
        public MediaLikersResponseHandler(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;
            
            try
            {
                if (response.Response.Contains("<!DOCTYPE html>"))
                    BrowserHtmlResponse(response);
                else if (response.Response.Contains("{\"globalObjects\":{\"tweets\":{") || response.Response.Contains("{\"data\":{\"favoriters_timeline\":{"))
                    BrowserResponseHandler(response);
                else
                    JsonResponse(response);
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }

        private void BrowserResponseHandler(IResponseParameter response)
        {
            try
            {
                HasMoreResults = true;
                UserList = new List<TwitterUser>();
                var jsonReponse = new Jsonhandler(response.Response);
                var minPositionEntries = jsonReponse.GetJToken("timeline", "instructions", 0,"addEntries","entries").Last;
                if(minPositionEntries is null)
                {
                    var userlist = jsonReponse.GetJToken("data", "favoriters_timeline", "timeline", "instructions",0, "entries");
                    MinPosition = jsonReponse.GetJTokenValue(userlist.Last, "content","value");
                    foreach (var users in userlist)
                    {
                        var UserToken = jsonReponse.GetJTokenOfJToken(users, "content", "itemContent", "user_results", "result");
                        if(UserToken != null && UserToken.HasValues)
                        {
                            var legacyToken = jsonReponse.GetJTokenOfJToken(UserToken, "legacy");
                            var joinedDate = TdTimeStampUtility.ConvertTimestamp(jsonReponse, legacyToken);
                            UserList.Add(new TwitterUser
                            {
                                Username = jsonReponse.GetJTokenValue(legacyToken, "screen_name"),
                                UserId = jsonReponse.GetJTokenValue(UserToken, "rest_id"),
                                IsPrivate = jsonReponse.GetJTokenValue(legacyToken, "protected") == "True",
                                HasProfilePic = jsonReponse.GetJTokenValue(legacyToken, "default_profile") == "True",
                                FollowStatus = jsonReponse.GetJTokenValue(legacyToken, "following") == "True",
                                FollowBackStatus = jsonReponse.GetJTokenValue(legacyToken, "followed_by") == "True",
                                IsMuted = jsonReponse.GetJTokenValue(legacyToken, "muting") == "True",
                                IsVerified = jsonReponse.GetJTokenValue(legacyToken, "verified") == "True",
                                JoiningDate = joinedDate
                            });
                        }
                    }
                }
                else
                {
                    MinPosition = jsonReponse.GetJTokenValue(minPositionEntries, "content", "operation", "cursor", "value");
                    var userlist = jsonReponse.GetJToken("globalObjects", "users");
                    foreach (var users in userlist)
                    {
                        var UserToken = users.First;
                        var joinedDate = TdTimeStampUtility.ConvertTimestamp(jsonReponse, UserToken);
                        UserList.Add(new TwitterUser
                        {
                            Username = jsonReponse.GetJTokenValue(UserToken, "screen_name"),
                            UserId = jsonReponse.GetJTokenValue(UserToken, "id_str"),
                            IsPrivate = jsonReponse.GetJTokenValue(UserToken, "protected") == "True",
                            HasProfilePic = jsonReponse.GetJTokenValue(UserToken, "default_profile") == "True",
                            FollowStatus = jsonReponse.GetJTokenValue(UserToken, "following") == "True",
                            FollowBackStatus = jsonReponse.GetJTokenValue(UserToken, "followed_by") == "True",
                            IsMuted = jsonReponse.GetJTokenValue(UserToken, "muting") == "True",
                            IsVerified = jsonReponse.GetJTokenValue(UserToken, "verified") == "True",
                            JoiningDate = joinedDate
                        });
                    }
                }
                if (UserList.Count == 0)
                    HasMoreResults = false;
            }
            catch (Exception e)
            {

                e.DebugLog();
            }
        }

        private void BrowserHtmlResponse(IResponseParameter response)
        {
            var htmlDocument = new HtmlDocument();
            UserList = new List<TwitterUser>();

            //js-stream-item stream-item stream-item
            //user-actions btn-group not-following not-muting

            var pageResponse =
                HtmlAgilityHelper.getStringTextFromClassName(response.Response,
                    "activity-popup-users dropdown-threshold");
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
    }
}