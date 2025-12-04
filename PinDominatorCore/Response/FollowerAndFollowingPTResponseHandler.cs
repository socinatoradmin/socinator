using System;
using System.Collections.Generic;
using System.ComponentModel;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;

namespace PinDominatorCore.Response
{
    [Localizable(false)]
    public class FollowerAndFollowingPtResponseHandler : PdResponseHandler
    {
        public List<PinterestUser> UsersList { get; } = new List<PinterestUser>();
        public bool HasMoreResults { get; set; }
        public string BookMark { get; set; }

        public FollowerAndFollowingPtResponseHandler(IResponseParameter response, string profileId, string user = "") : base(response)
        {
            try
            {
                if(!string.IsNullOrEmpty(response.Response) && response.Response.Contains("\"data\":{\"v3GetUserHandlerQuery\""))
                {
                    var jObject = handler.ParseJsonToJObject(response.Response);
                    BookMark = handler.GetJTokenValue(jObject, "data", "v3GetUserHandlerQuery", "data", "followers", "connection", "pageInfo", "endCursor");
                    HasMoreResults = !string.IsNullOrEmpty(BookMark) && !BookMark.Contains("-end-");
                    var Followers = handler.GetJArrayElement(handler.GetJTokenValue(jObject, "data", "v3GetUserHandlerQuery", "data", "followers", "connection","edges"));
                    if(Followers!=null && Followers.HasValues)
                    {
                        Followers.ForEach(data =>
                        {
                            try
                            {
                                var objPinterestUser = new PinterestUser
                                {
                                    Username = handler.GetJTokenValue(data, "node", "username"),
                                    FullName = handler.GetJTokenValue(data, "node", "fullName"),
                                    ProfilePicUrl = handler.GetJTokenValue(data, "node", "imageMediumUrl"),
                                    UserId = handler.GetJTokenValue(data, "node", "entityId")
                                };
                                objPinterestUser.HasProfilePic = !string.IsNullOrEmpty(objPinterestUser.ProfilePicUrl) && objPinterestUser.ProfilePicUrl != "N/A" && !objPinterestUser.ProfilePicUrl.Contains("user/default_280");
                                if (objPinterestUser.Username != profileId)
                                    UsersList.Add(objPinterestUser);
                            }
                            catch { }
                        });
                    }
                    return;
                }
                else
                {

                
                var jsonHand = new JsonHandler(response.Response);
                var jsonToken = jsonHand.GetJToken("resource_response", "data");
                BookMark = Utilities.GetBetween(response.Response, PdConstants.BookMark, "\"");
                HasMoreResults = !BookMark.Contains("end");

                //For browser automation
                if (!jsonToken.HasValues)
                {
                    jsonToken = jsonHand.GetJToken("resources", "data", "UserFollowersResource", $"hide_find_friends_rep=true,username=\"{user}\"", "data");
                    if (!jsonToken.HasValues)
                        jsonToken = jsonHand.GetJToken("resources", "data", "UserFollowingResource", $"hide_find_friends_rep=true,username=\"{user}\"", "data");
                }

                foreach (var token in jsonToken)
                {
                    int.TryParse(jsonHand.GetJTokenValue(token, "follower_count"), out int followersCount);
                    int.TryParse(jsonHand.GetJTokenValue(token, "board_count"), out int boardsCount);
                    int.TryParse(jsonHand.GetJTokenValue(token, "pin_count"), out int pinsCount);

                    var objPinterestUser = new PinterestUser
                    {
                        Username = jsonHand.GetJTokenValue(token, "username"),
                        UserId = jsonHand.GetJTokenValue(token, "id"),
                        FollowersCount = followersCount,
                        FullName = jsonHand.GetJTokenValue(token, "full_name"),
                        BoardsCount = boardsCount,
                        PinsCount = pinsCount,
                        HasProfilePic = jsonHand.GetJTokenValue(token, "is_default_image") == " False",
                        ProfilePicUrl = jsonHand.GetJTokenValue(token, "image_xlarge_url"),
                        IsFollowedByMe = jsonHand.GetJTokenValue(token, "explicitly_followed_by_me") == "True" ? true : false
                    };
                    objPinterestUser.HasProfilePic = !string.IsNullOrEmpty(objPinterestUser.ProfilePicUrl) && objPinterestUser.ProfilePicUrl != "N/A" && !objPinterestUser.ProfilePicUrl.Contains("user/default_280");
                    if (objPinterestUser.Username != profileId)
                        UsersList.Add(objPinterestUser);
                }

                BookMark = Utilities.GetBetween(response.Response, PdConstants.BookMark, "\"");
                HasMoreResults = !BookMark.Contains("end");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            if (UsersList.Count > 0)
                Success = true;
        }
    }
}