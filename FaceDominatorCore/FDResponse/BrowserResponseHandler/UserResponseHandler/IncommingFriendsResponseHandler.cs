using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDResponse.BrowserResponseHandler.UserResponseHandler
{

    public class IncommingFriendsResponseHandler : FdResponseHandler, IResponseHandler
    {
        public string TotalCount { get; set; } = "0";

        public string ShownIds { get; set; } = string.Empty;

        public List<FacebookUser> ListFacebookUser { get; set; }

        public bool HasMoreResults { get; set; }

        public string PostId { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }

        public IncommingFriendsResponseHandler(IResponseParameter responseParameter, List<string> listUserData,
            bool _hasmoreResults = false, bool isClassicUi = false)
            : base(responseParameter)
        {
            ObjFdScraperResponseParameters = new FdScraperResponseParameters();
            ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();

            GetProfileList(listUserData);

            HasMoreResults = _hasmoreResults;
        }

        private void GetProfileList(List<string> listUserData)
        {
            string scrapedUrl = string.Empty;

            ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();
            listUserData.Reverse();
            foreach (string response in listUserData)
            {
                FacebookUser objFacebookUser = new FacebookUser();

                var decodedResponse = FdFunctions.GetDecodedResponse(response);

                var username = string.Empty;

                try
                {
                    try
                    {
                        var userId = FdFunctions.GetIntegerOnlyString(FdRegexUtility.FirstMatchExtractor(decodedResponse, "user.php\\?id=(.*?)&"));
                        if (string.IsNullOrEmpty(userId) || userId.Equals("0"))
                            userId = FdFunctions.GetIntegerOnlyString(FdRegexUtility.FirstMatchExtractor(decodedResponse, "profile.php\\?id=(.*?)\""));

                        if (string.IsNullOrEmpty(userId) || userId.Equals("0"))
                        {
                            var userDetails = FdRegexUtility.FirstMatchExtractor(decodedResponse, "profile.php\\?id=(.*?)\"");
                            if (userDetails.Contains("profile.php?id=") || userDetails.Contains("href="))
                                userId = FdFunctions.GetIntegerOnlyString(FdRegexUtility.FirstMatchExtractor(userDetails, "(.*?)\""));
                            else
                                userId = FdFunctions.GetIntegerOnlyString(FdRegexUtility.FirstMatchExtractor(decodedResponse, "profile.php\\?id=(.*?)&"));
                        }

                        scrapedUrl = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.ScrapedUrlRegx);

                        username = FdRegexUtility.FirstMatchExtractor(decodedResponse, "title=\"(.*?)\"");

                        if (string.IsNullOrEmpty(username))
                            username = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.AriaLabelRegex);

                        if (username.Contains("Friends"))
                            username = string.Empty;

                        if (!FdFunctions.IsIntegerOnly(userId))
                            continue;
                        if (!string.IsNullOrEmpty(userId) && userId != "0" && FdFunctions.IsIntegerOnly(userId))
                            objFacebookUser = FdConstants.getFaceBookUserFromUrlOrIdOrUserName(userId);
                        else if (!string.IsNullOrEmpty(scrapedUrl))
                            objFacebookUser = FdConstants.getFaceBookUserFromUrlOrIdOrUserName(scrapedUrl);
                        objFacebookUser.UserId = userId == "0" ? string.Empty : userId;

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                        objFacebookUser.IsAlreadyFriend = "true";
                    }

                    objFacebookUser.ClassName = FdConstants.IncommingFriend2Element;

                    objFacebookUser.Familyname = username;

                    objFacebookUser.ScrapedProfileUrl = scrapedUrl;

                    objFacebookUser.ProfilePicUrl = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.ImageSrc2Regex);

                    if (objFacebookUser.ScrapedProfileUrl.Contains("groups"))
                        continue;

                    using (FdHtmlParseUtility parser = new FdHtmlParseUtility())
                    {
                        var mutualUrl = objFacebookUser.ScrapedProfileUrl + "&sk=friends_mutual";
                        var mutualFriendDetails = parser.GetInnerTextFromPartialTagName(decodedResponse, "a", "href", mutualUrl);
                        var mutualFriendCount = FdFunctions.GetIntegerOnlyString(mutualFriendDetails);
                        if (mutualFriendDetails != null)
                            objFacebookUser.HasMutualFriends = mutualFriendDetails.Contains("mutual friends") && mutualFriendCount != "0" ? true : false;
                    }

                    var boolValue = string.IsNullOrEmpty(objFacebookUser.UserId) ? !string.IsNullOrEmpty(objFacebookUser.ScrapedProfileUrl) : !string.IsNullOrEmpty(objFacebookUser.UserId);

                    if (boolValue && (ObjFdScraperResponseParameters.ListUser.FirstOrDefault
                        (x => x.UserId == objFacebookUser.UserId) == null || ObjFdScraperResponseParameters.ListUser.FirstOrDefault(x => x.ScrapedProfileUrl == objFacebookUser.ScrapedProfileUrl) == null))
                        ObjFdScraperResponseParameters.ListUser.Add(objFacebookUser);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            Status = ObjFdScraperResponseParameters.ListUser.Count > 0;

            ObjFdScraperResponseParameters.ListUser.Reverse();
        }

    }
}
