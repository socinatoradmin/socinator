using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response
{
    public class FollowerFollowingResponseHandler : TdBaseHtmlResponseHandler
    {
        public List<TwitterUser> ListOfTwitterUser = new List<TwitterUser>();
        public List<string> ListOfUserData = new List<string>();

        public FollowerFollowingResponseHandler(IResponseParameter responseParameter) : base(responseParameter)
        {
            if (!Success)
                return;
            var response = responseParameter.Response;
            MinPosition = Utilities.GetBetween(response, "data-min-position=\"", "\"");
            if (response.Contains("<!DOCTYPE html>"))
            {
                NormalResponseHandler(response);
            }
            else if (!response.Contains("<!DOCTYPE html>") &&
                     (response.Contains("items_html") || response.Contains("min_position")))
            {
                var jsonObject = JObject.Parse(response);
                response = jsonObject["items_html"].ToString();
                MinPosition = jsonObject["min_position"].ToString();
                NormalResponseHandler(response);
            }
            else
            {
                var jObject = JObject.Parse(response);
                if (response.Contains("{\"data\":{\"user\":{\"followers_timeline\":") ||
                    response.Contains("{\"data\":{\"user\":{\"following_timeline\":"))
                {
                    var count = 0;
                    var jobj = new JsonHandler(response);
                    var users = jobj.GetJTokenOfJToken(jObject, "data", "user", "followers_timeline", "timeline",
                        "instructions", 2, "entries");
                    if (users.Children().Count() == 0)
                        users = jobj.GetJTokenOfJToken(jObject, "data", "user", "following_timeline", "timeline",
                            "instructions", 2, "entries");
                    if (users.Children().Count() == 0)
                        users = jobj.GetJTokenOfJToken(jObject, "data", "user", "followers_timeline", "timeline",
                            "instructions", 0, "entries");
                    if (users.Children().Count() == 0)
                        users = jobj.GetJTokenOfJToken(jObject, "data", "user", "following_timeline", "timeline",
                            "instructions", 0, "entries");
                    if (users.Children().Count() == 0)
                        users = jobj.GetJTokenOfJToken(jObject, "data", "user", "following_timeline", "timeline",
                            "instructions", 3, "entries");
                    if (users.Children().Count() == 0)
                        users = jobj.GetJTokenOfJToken(jObject, "data", "user", "followers_timeline", "timeline",
                            "instructions", 3, "entries");

                    var pagination = jobj.GetJTokenValue(users, 20, "content", "value");
                    MinPosition = pagination;
                    foreach (var userDetails in users)
                    {
                        count++;
                        var getUsers = jobj.GetJTokenOfJToken(userDetails, "content", "itemContent", "user");
                        var userId = jobj.GetJTokenValue(getUsers, "rest_id");
                        var userFullDetails = jobj.GetJTokenOfJToken(getUsers, "legacy");
                        var userName = jobj.GetJTokenValue(userFullDetails, "screen_name");
                        var fullName = jobj.GetJTokenValue(userFullDetails, "name");
                        var isPrivate = jobj.GetJTokenValue(userFullDetails, "protected");
                        var isProfilePic = jobj.GetJTokenValue(userFullDetails, "default_profile");
                        var statusFollow = jobj.GetJTokenValue(userFullDetails, "following");
                        var statusFollowBack = jobj.GetJTokenValue(userFullDetails, "followed_by");
                        var isverified = jobj.GetJTokenValue(userFullDetails, "verified");
                        var ismute = jobj.GetJTokenValue(userFullDetails, "muting");
                        var objTwitterUser = new TwitterUser
                        {
                            Username = userName,
                            UserId = userId,
                            FullName = fullName,
                            IsPrivate = isPrivate == "True",
                            IsMuted = ismute == "True",
                            IsVerified = isverified == "True",
                            HasProfilePic = isProfilePic == "True",
                            FollowStatus = statusFollow == "True",
                            FollowBackStatus = statusFollowBack == "True"
                        };
                        if (count <= 20)
                            ListOfTwitterUser.Add(objTwitterUser);
                    }

                    Paginationresult();
                }else if(!string.IsNullOrEmpty(response) && response.Contains("{\"data\":{\"user\":{\"result\":{\"__typename\":\"User\",\"timeline\":"))
                {
                    jObject = handler.ParseJsonToJObject(response);
                    var userArray = handler.GetJArrayElement(handler.GetJTokenValue(jObject, "data","user","result", "timeline", "timeline", "instructions"));
                    if(userArray != null && userArray.HasValues)
                    {
                        foreach (var user in userArray)
                        {
                            var userDetails = handler.GetJArrayElement(handler.GetJTokenValue(user, "entries"));
                            if(userDetails != null && userDetails.HasValues)
                            {
                                foreach(var item in userDetails)
                                {
                                    var userProfileData = handler.GetJTokenOfJToken(item, "content", "itemContent", "user_results", "result");
                                    if(userProfileData != null && userProfileData.HasValues)
                                    {
                                        var userId = handler.GetJTokenValue(userProfileData, "rest_id");
                                        var userFullDetails = handler.GetJTokenOfJToken(userProfileData, "legacy");
                                        var userName = handler.GetJTokenValue(userFullDetails, "screen_name");
                                        userName = string.IsNullOrEmpty(userName) ? handler.GetJTokenValue(userProfileData, "core", "screen_name") : userName;
                                        var fullName = handler.GetJTokenValue(userFullDetails, "name");
                                        fullName = string.IsNullOrEmpty(fullName) ? handler.GetJTokenValue(userProfileData, "core", "name") : fullName;
                                        var isPrivate = handler.GetJTokenValue(userFullDetails, "protected");
                                        isPrivate = string.IsNullOrEmpty(isPrivate) ? handler.GetJTokenValue(userProfileData, "privacy", "protected") : isPrivate;
                                        var isProfilePic = handler.GetJTokenValue(userFullDetails, "default_profile");
                                        isProfilePic = string.IsNullOrEmpty(isProfilePic) ? handler.GetJTokenValue(userProfileData, "avatar", "image_url") : isProfilePic;
                                        var statusFollow = handler.GetJTokenValue(userFullDetails, "following");
                                        statusFollow = string.IsNullOrEmpty(statusFollow) ? handler.GetJTokenValue(userProfileData, "relationship_perspectives", "following") : statusFollow;
                                        var statusFollowBack = handler.GetJTokenValue(userFullDetails, "followed_by");
                                        var isverified = handler.GetJTokenValue(userFullDetails, "verified");
                                        isverified = string.IsNullOrEmpty(isverified) ? handler.GetJTokenValue(userProfileData, "verification", "verified") : isverified;
                                        var ismute = handler.GetJTokenValue(userFullDetails, "muting");
                                        ismute = string.IsNullOrEmpty(ismute) ? handler.GetJTokenValue(userProfileData, "relationship_perspectives", "muting") : ismute;
                                        var objTwitterUser = new TwitterUser
                                        {
                                            Username = userName,
                                            UserId = userId,
                                            FullName = fullName,
                                            IsPrivate = isPrivate == "True",
                                            IsMuted = ismute == "True",
                                            IsVerified = isverified == "True" || handler.GetJTokenValue(userProfileData, "is_blue_verified")=="True",
                                            HasProfilePic = isProfilePic == "True",
                                            FollowStatus = statusFollow == "True",
                                            FollowBackStatus = statusFollowBack == "True"
                                        };
                                        ListOfTwitterUser.Add(objTwitterUser);
                                    }
                                    else
                                    {
                                        var cursorToken = handler.GetJTokenOfJToken(item, "content");
                                        if(string.IsNullOrEmpty(MinPosition) && cursorToken!=null && cursorToken.HasValues)
                                        {
                                            var type = handler.GetJTokenValue(cursorToken, "entryType");
                                            var cursorType = handler.GetJTokenValue(cursorToken, "cursorType");
                                            if(!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(cursorType) && type == "TimelineTimelineCursor" && cursorType == "Bottom")
                                            {
                                                var cursorValue = handler.GetJTokenValue(cursorToken, "value");
                                                if(!string.IsNullOrEmpty(cursorValue) && !cursorValue.StartsWith("0"))
                                                    MinPosition = WebUtility.UrlEncode(cursorValue);
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                        Paginationresult();
                    }
                }
                else
                {
                    MinPosition = jObject["next_cursor_str"].ToString();
                    BrowserNewUIResponse(response);
                }
            }
        }

        public string MinPosition { get; set; }

        public bool HasMoreResults { get; set; }

        public void NormalResponseHandler(string responses)
        {
            #region NewUiHtmlresponse

            //ListOfUserData = Regex.Split(response, "css-1dbjc4n r-my5ep6 r-qklmqi r-1adg3ll").ToList();
            //foreach (var userData in ListOfUserData)
            //    try
            //    {
            //        if (userData.Contains("<!DOCTYPE html>"))
            //            continue;
            //        var fullName = Regex.Matches(userData, "<span class=\"css-901oao css-16my406 r-1qd0xha r-ad9z0x r-bcqeeo r-qvutc0\"><span class=\"css-901oao css-16my406 r-1qd0xha r-ad9z0x r-bcqeeo r-qvutc0\">(.*?)</span>", RegexOptions.Singleline)[0].Groups[1].ToString();
            //        var userName = Regex.Matches(userData, "<span class=\"css-901oao css-16my406 r-1qd0xha r-ad9z0x r-bcqeeo r-qvutc0\">(.*?)</span>", RegexOptions.Singleline)[1].Groups[1].ToString();
            //        var userId = Regex.Matches(userData, "data-testid=\"(.*?)\">", RegexOptions.Singleline)[1].Groups[1].ToString();
            //        var ProfilePic = Utilities.GetBetween(userData, "src=\"", "\"");
            //        bool UserProfile = userData.Contains("src=\"");

            //        if (userId.Contains("unfollow") || userId.Contains("follow"))
            //        {

            //            var UserId = Regex.Split(userId, "-").ToList();
            //            userId = UserId[0];

            //        }
            //        var objTwitterUser = new TwitterUser
            //        {
            //            Username = userName,
            //            UserId = userId,
            //            FullName = fullName,
            //            IsPrivate = userData.Contains("data-protected=\"true\""),
            //            HasProfilePic = UserProfile,
            //            FollowStatus = userData.Contains("Follows you"),
            //            FollowBackStatus = userData.Contains("Following"),
            //            //  IsMuted = !userData.Contains("not-muting"),
            //            //IsVerified = userData.Contains("Icon--verified")
            //        };
            //        ListOfTwitterUser.Add(objTwitterUser);
            //    }
            //    catch (Exception)
            //    {
            //    }
            //if (ListOfTwitterUser.Count != 0)
            //{
            //    HasMoreResults = true;
            //}

            #endregion

            ListOfUserData = Regex.Split(responses, "ProfileCard js-actionable-user").ToList();

            foreach (var userData in ListOfUserData)
                try
                {
                    if (userData.Contains("<!DOCTYPE html>") || !userData.Contains("data-user-id"))
                        continue;
                    var objTwitterUser = new TwitterUser
                    {
                        Username = Utilities.GetBetween(userData, "data-screen-name=\"", "\""),
                        UserId = Utilities.GetBetween(userData, "data-user-id=\"", "\""),
                        FullName = Utilities.GetBetween(userData, "data-name=\"", "\""),
                        IsPrivate = userData.Contains("data-protected=\"true\""),
                        HasProfilePic = !userData.Contains("default_profile_images"),
                        FollowStatus = !userData.Contains("not-following") && userData.Contains("following"),
                        FollowBackStatus = userData.Contains("FollowStatus"),
                        IsMuted = !userData.Contains("not-muting"),
                        IsVerified = userData.Contains("Icon--verified")
                    };
                    ListOfTwitterUser.Add(objTwitterUser);
                }

                catch (Exception)
                {
                }

            Paginationresult();
        }

        public void BrowserNewUIResponse(string responses)
        {
            ListOfTwitterUser = new List<TwitterUser>();
            var jObject = handler.ParseJsonToJObject(responses);
            var users = handler.GetJArrayElement(handler.GetJTokenValue(jObject, "users"));
            var profilepic = true;

            foreach (var items in users)
            {
                //  var ss = items["default_profile"];
                var profilePicUrl = handler.GetJTokenValue(items, "profile_image_url_https");
                if (string.IsNullOrEmpty(profilePicUrl))
                    profilepic = false;
                var objTwitterUser = new TwitterUser
                {
                    Username = handler.GetJTokenValue(items, "screen_name"),
                    UserId = handler.GetJTokenValue(items, "id_str"),
                    FullName = handler.GetJTokenValue(items, "name"),
                    IsPrivate = handler.GetJTokenValue(items, "protected") == "True",
                    HasProfilePic = handler.GetJTokenValue(items, "default_profile") == "True" ? true : profilepic,
                    FollowStatus = handler.GetJTokenValue(items, "following") == "True",
                    FollowBackStatus = handler.GetJTokenValue(items, "followed_by") == "True",
                    IsMuted = handler.GetJTokenValue(items, "muting") == "True",
                    JoiningDate = TdUtility.GetDateTime(handler.GetJTokenValue(items, "created_at")),
                    IsVerified = handler.GetJTokenValue(items, "verified") == "True"
                };
                ListOfTwitterUser.Add(objTwitterUser);
            }

            Paginationresult();
        }

        private void Paginationresult()
        {
            if (!MinPosition.Equals("0") && !string.IsNullOrWhiteSpace(MinPosition)) HasMoreResults = true;
        }
    }
}