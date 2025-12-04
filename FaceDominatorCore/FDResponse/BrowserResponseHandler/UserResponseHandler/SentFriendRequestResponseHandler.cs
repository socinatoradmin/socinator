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

    public class SentFriendRequestResponseHandler : FdResponseHandler, IResponseHandler
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

        public SentFriendRequestResponseHandler(IResponseParameter responseParameter, List<string> listUserData,
            bool _hasmoreResults = false, bool isClassicUi = false, bool isAddedFriend = false)
            : base(responseParameter)
        {
            ObjFdScraperResponseParameters = new FdScraperResponseParameters();

            ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();

            GetPostCommentor(listUserData, isAddedFriend);

            HasMoreResults = _hasmoreResults;
        }

        private void GetPostCommentor(List<string> fanpageResponseList, bool isaddedFriend)
        {
            string scrapedUrl = string.Empty;

            ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();

            foreach (string response in fanpageResponseList)
            {

                FacebookUser objFacebookUser = new FacebookUser();

                var decodedResponse = FdFunctions.GetDecodedResponse(response);

                var username = string.Empty;

                var date = string.Empty;

                try
                {
                    try
                    {
                        var userId = FdRegexUtility.GetMatchCount(decodedResponse, "user.php\\?id=(.*?)&") >= 2
                            ? FdFunctions.GetIntegerOnlyString(FdRegexUtility.GetNthMatch(decodedResponse, "user.php\\?id=(.*?)&", 1)) : "0";

                        userId = userId == "0" ? FdFunctions.GetIntegerOnlyString(FdRegexUtility.FirstMatchExtractor(decodedResponse, "profile.php\\?id=(.*?)\"")) : userId;

                        scrapedUrl = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.ScrapedUrlRegx);

                        var profileId = System.Text.RegularExpressions.Regex.Split(scrapedUrl, "\\/")?.ToList();
                        objFacebookUser.ProfileId = profileId?.LastOrDefault();
                        if (objFacebookUser.ProfileId.Contains("profile.php"))
                            objFacebookUser.ProfileId = FdFunctions.GetIntegerOnlyString(objFacebookUser.ProfileId);

                        userId = userId == "0" ? objFacebookUser.ProfileId : userId;
                        username = FdRegexUtility.GetMatchCount(decodedResponse, "title=\"(.*?)\"") >= 2 ? FdRegexUtility.GetNthMatch(decodedResponse, "title=\"(.*?)\"", 1) : string.Empty;
                        if (string.IsNullOrEmpty(username))
                        {
                            using (FdHtmlParseUtility htmlParseUtility = new FdHtmlParseUtility())
                            {
                                username = htmlParseUtility.GetInnerTextFromPartialTagNameContains(decodedResponse, "span", "class", "jq4qci2q a3bd9o3v lrazzd5p oo9gr5id");
                                username = string.IsNullOrEmpty(username) || username == "Friends" ? htmlParseUtility.GetInnerTextFromPartialTagNameContains(decodedResponse, "span", "class", "a5q79mjw g1cxx5fr lrazzd5p oo9gr5id") : username;
                                username = string.IsNullOrEmpty(username) ? htmlParseUtility.GetInnerTextFromPartialTagNameContains(decodedResponse, "span", "class", "jagab5yi g1cxx5fr lrazzd5p oo9gr5id") : username;
                                username = string.IsNullOrEmpty(username) ? htmlParseUtility.GetInnerTextFromPartialTagNameContains(decodedResponse, "span", "class", "exr7barw k1z55t6l oog5qr5w innypi6y pbevjfx6") : username;
                                username = string.IsNullOrEmpty(username) ? htmlParseUtility.GetInnerTextFromPartialTagNameContains(decodedResponse, "span", "class", "szxhu1pg hpj0pwwo sggt6rq5 innypi6y pbevjfx6") : username;
                                username = string.IsNullOrEmpty(username) ? htmlParseUtility.GetInnerTextFromPartialTagNameContains(decodedResponse, "span", "class", "x676frb x1lkfr7t x1lbecb7 x1s688f xzsf02u") : username;
                            }
                        }

                        if ((!FdFunctions.IsIntegerOnly(userId) || userId == "0") && string.IsNullOrEmpty(objFacebookUser.ProfileId))
                            continue;


                        objFacebookUser.UserId = userId;

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                        objFacebookUser.IsAlreadyFriend = "true";
                    }


                    objFacebookUser.ClassName = isaddedFriend ? FdConstants.AddedFriend3 : FdConstants.SentFriend2Element;

                    objFacebookUser.Familyname = username;

                    objFacebookUser.ScrapedProfileUrl = scrapedUrl;

                    objFacebookUser.InteractionDate = date;

                    objFacebookUser.ProfilePicUrl = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.ImageSrcRegex);

                    if (ObjFdScraperResponseParameters.ListUser.FirstOrDefault(x => x.UserId == objFacebookUser.UserId && x.ProfileId == objFacebookUser.UserId) == null)
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
