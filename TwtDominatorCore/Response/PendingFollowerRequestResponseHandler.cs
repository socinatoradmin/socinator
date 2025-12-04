using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.Response
{
    public class PendingFollowerRequestResponseHandler : TdResponseHandler
    {
        public List<TwitterUser> ListOfTwitterUser = new List<TwitterUser>();

        public PendingFollowerRequestResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                if (!Success)
                    return;
                var Response = response.Response;
                var ListOfUserData = new List<string>();
                MinPosition = Utilities.GetBetween(Response, "data-min-position=\"", "\"");
                if (!Response.Contains("<!DOCTYPE html>"))
                {
                    var JsonObject = JObject.Parse(response.Response);
                    Response = JsonObject["items_html"].ToString();
                    MinPosition = JsonObject["min_position"].ToString();
                }

                ListOfUserData = Regex.Split(Response, "ProfileCard js-actionable-user").ToList();

                foreach (var UserData in ListOfUserData)
                    try
                    {
                        if (UserData.Contains("<!DOCTYPE html>") || !UserData.Contains("data-user-id"))
                            continue;
                        var objTwitterUser = new TwitterUser
                        {
                            Username = Utilities.GetBetween(UserData, "data-screen-name=\"", "\""),
                            UserId = Utilities.GetBetween(UserData, "data-user-id=\"", "\""),
                            FullName = Utilities.GetBetween(UserData, "data-name=\"", "\""),
                            IsPrivate = UserData.Contains("data-protected=\"true\""),
                            HasProfilePic = !UserData.Contains("default_profile_images"),
                            FollowStatus = !UserData.Contains("not-following") && UserData.Contains("following"),
                            FollowBackStatus = UserData.Contains("FollowStatus")
                        };

                        ListOfTwitterUser.Add(objTwitterUser);
                    }
                    catch (Exception)
                    {
                    }

                if (!MinPosition.Equals("0"))
                    HasMoreResults = true;
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }

        public string MinPosition { get; }

        public bool HasMoreResults { get; }
    }
}