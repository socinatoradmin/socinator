using System;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.Utility;

namespace PinDominatorCore.Response
{
    public class UserNameInfoPtResponseHandler : PdResponseHandler
    {
        public UserNameInfoPtResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                if (string.IsNullOrEmpty(response.Response))
                {
                    Success = false;
                    return;
                }
                var jsonData = PdRequestHeaderDetails.GetJsonString(response.Response);
                var jObject = handler.ParseJsonToJObject(jsonData);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    string currentUrl = handler.GetJTokenValue(jObject, "context", "current_url");
                    currentUrl = string.IsNullOrEmpty(currentUrl) ? Utilities.GetBetween(response.Response, "\"current_url\": \"", ",") : currentUrl;
                    currentUrl = string.IsNullOrEmpty(currentUrl) ? Utilities.GetBetween(response.Response, "\"current_url\":\"", "\",") : currentUrl;
                    if (currentUrl.Contains("show_error=true"))
                    {
                        Issue = new PinterestIssue() { Error = PDEnums.PinterestError.UsernameNotExist };
                        Success = false;
                        return;
                    }
                }
                JToken profileObject = null;
                try
                {
                    foreach (var jtoken in handler.GetJTokenOfJToken(jObject,"users"))
                    {
                        profileObject = jtoken.First();
                        if (profileObject.Count() > 18)
                            break;
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        profileObject = handler.GetJTokenOfJToken(jObject,"tree", "data");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                if(profileObject == null)
                {
                    foreach (var jtoken in handler.GetJTokenOfJToken(jObject,"resource_response"))
                    {
                        profileObject = jtoken.First();
                        if (profileObject.Count() >= 9)
                            break;
                    }
                }
                if (profileObject == null)
                {
                    var username = $"username=\"{Utilities.GetBetween(response.Response, "www.pinterest.com/user/", "\" data-app")}\"";
                    profileObject = handler.GetJTokenOfJToken(jObject,"props", "initialReduxState", "resources", "UserResource", username, "data");
                }
                if (!profileObject.HasValues)
                {
                    try
                    {
                        foreach (var jtoken in handler.GetJTokenOfJToken(jObject,"props", "initialReduxState", "users"))
                        {
                            profileObject = jtoken.First();
                            if (profileObject.Count() >= 9)
                                break;
                        }
                    }
                    catch(Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                if(profileObject == null || !profileObject.HasValues)
                    profileObject = PdRequestHeaderDetails.GetRequestHeader(response.Response, TokenDetailsType.Users).jToken;
                if (profileObject != null)
                {
                    Success = true;
                    FirstName = PdUtility.AssignNA(handler.GetJTokenValue(profileObject, "first_name"));
                    LastName =PdUtility.AssignNA(handler.GetJTokenValue(profileObject, "last_name"));
                    Username =PdUtility.AssignNA(handler.GetJTokenValue(profileObject, "username"));
                    UserId =PdUtility.AssignNA(handler.GetJTokenValue(profileObject, "id"));
                    ImageSmallUrl = PdUtility.AssignNA(handler.GetJTokenValue(profileObject, "image_small_url"));
                    ImageMediumUrl = PdUtility.AssignNA(handler.GetJTokenValue(profileObject, "image_medium_url"));
                    ImageLargeUrl = PdUtility.AssignNA(handler.GetJTokenValue(profileObject, "image_large_url"));
                    ImageXlargeUrl = PdUtility.AssignNA(handler.GetJTokenValue(profileObject, "image_xlarge_url"));
                    var followercount = PdUtility.AssignNA(handler.GetJTokenValue(profileObject, "follower_count"));
                    if (string.IsNullOrEmpty(followercount) || followercount=="N/A")
                        followercount=Utilities.GetBetween(response.Response, "\"pinterestapp:followers\" content=\"", "\"");
                    int n = 0, m = 0;
                    followercount =PdUtility.ConvertDecimalToNumber(string.IsNullOrEmpty(followercount) ? Utilities.GetBetween(Utilities.GetBetween(response.Response, "\"profile-followers-count\"", "followers</span>") + "**", "<span class=\"tBJ dyH iFc sAJ O2T zDA IZT H2s\">", "**") : followercount);
                    int.TryParse(followercount, out n);
                    FollowerCount = n;
                    var followingcount =PdUtility.AssignNA(handler.GetJTokenValue(profileObject, "following_count"));
                    if (string.IsNullOrEmpty(followingcount) || followingcount=="N/A")
                        followingcount = Utilities.GetBetween(response.Response, "\"pinterestapp:following\" content=\"", "\"");
                    followingcount = PdUtility.ConvertDecimalToNumber(string.IsNullOrEmpty(followingcount)? Utilities.GetBetween(Utilities.GetBetween(response.Response, "\"profile-following-count\"", "following</span>") + "**", "<span class=\"tBJ dyH iFc sAJ O2T zDA IZT H2s\">", "**"):followingcount);
                    int.TryParse(followingcount, out n);
                    FollowingCount = n;
                    var boardcount = handler.GetJTokenValue(profileObject, "board_count");
                    int.TryParse(boardcount, out n);
                    var secretboardcount = handler.GetJTokenValue(profileObject, "secret_board_count");
                    int.TryParse(secretboardcount, out m);
                    BoardCount = m + n;
                    var pincount = handler.GetJTokenValue(profileObject, "pin_count");
                    if (string.IsNullOrEmpty(pincount))
                        pincount = Utilities.GetBetween(response.Response, "\"pinterestapp:pins\" content=\"", "\"");
                    int.TryParse(pincount, out n);
                    PinsCount = n;
                    Biography = PdUtility.AssignNA(handler.GetJTokenValue(profileObject, "about"));
                    WebsiteUrl = PdUtility.AssignNA(handler.GetJTokenValue(profileObject, "website_url"));
                    FullName = PdUtility.AssignNA(handler.GetJTokenValue(profileObject, "full_name"));
                    var identity = handler.GetJTokenOfJToken(profileObject, "verified_identity");
                    if (identity.Any()) IsVerified = handler.GetJTokenValue(profileObject, "verified") == "True";
                    HasProfilePicture = handler.GetJTokenValue(profileObject, "show_creator_profile") == "True";
                    ProfilePicUrl = PdUtility.AssignNA(handler.GetJTokenValue(profileObject, "image_xlarge_url"));
                    if (string.IsNullOrEmpty(ProfilePicUrl))
                        ProfilePicUrl = PdUtility.AssignNA(Utilities.GetBetween(response.Response, "name=\"og:image\" content=\"", "\""));
                    HasProfilePicture =!string.IsNullOrEmpty(ProfilePicUrl)&& ProfilePicUrl!="N/A" && !ProfilePicUrl.Contains("default");
                    IsFollowed = handler.GetJTokenValue(profileObject, "explicitly_followed_by_me") == "True" ? true : false;
                    var Details = handler.GetJTokenOfJToken(profileObject,"resource_response", "data");
                    var IsPrivateProfile = handler.GetJTokenValue(Details, "is_private_profile");
                    if (string.IsNullOrEmpty(IsPrivateProfile) ? false : IsPrivateProfile == "True")
                    {
                        UserId = string.IsNullOrEmpty(UserId) || UserId == "N/A" ?handler.GetJTokenValue(Details,"id"): UserId;
                        Username = string.IsNullOrEmpty(Username) || Username == "N/A" ? handler.GetJTokenValue(Details, "username") : Username;
                        FullName = string.IsNullOrEmpty(FullName) || FullName == "N/A" ? handler.GetJTokenValue(Details, "full_name") : FullName;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        

        public string Biography { get; set; } = string.Empty;

        public bool IsFollowed { get; set; }

        public int FollowerCount { get; set; }

        public int FollowingCount { get; set; }

        public int BoardCount { get; set; }

        public int PinsCount { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public bool HasProfilePicture { get; set; }

        public string ProfilePicUrl { get; set; } = string.Empty;

        public string ImageSmallUrl { get; set; } = string.Empty;

        public string ImageMediumUrl { get; set; } = string.Empty;

        public string ImageLargeUrl { get; set; } = string.Empty;

        public string ImageXlargeUrl { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public string WebsiteUrl { get; set; } = string.Empty;

        public bool IsVerified { get; set; }

        public static implicit operator PinterestUser(UserNameInfoPtResponseHandler user)
        {
            var pinterestUser = new PinterestUser(user.FullName, user.Username)
            {
                UserBio = user.Biography,
                FollowersCount = user.FollowerCount,
                FollowingsCount = user.FollowingCount,
                BoardsCount = user.BoardCount,
                PinsCount = user.PinsCount,
                HasProfilePic = user.HasProfilePicture,
                FullName = user.FullName,
                ProfilePicUrl = user.ProfilePicUrl,
                WebsiteUrl = user.WebsiteUrl,
                Username = user.Username,
                UserId = user.UserId,
                IsVerified = user.IsVerified,
                ImageLargeUrl = user.ImageLargeUrl,
                ImageMediumUrl = user.ImageMediumUrl,
                ImageSmallUrl = user.ImageSmallUrl,
                ImageXlargeUrl = user.ImageXlargeUrl,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsFollowedByMe = user.IsFollowed,
            };
            return pinterestUser;
        }
    }
}