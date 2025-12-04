using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.BrowserResponseHandler.UserResponseHandler
{

    public class SendGreetingsReponseHandler : FdResponseHandler, IResponseHandler
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

        public SendGreetingsReponseHandler(IResponseParameter responseParameter, List<string> listUserData,
            bool _hasmoreResults = false, bool isClassicUi = true)
            : base(responseParameter)
        {
            ObjFdScraperResponseParameters = new FdScraperResponseParameters();

            ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();

            GetPostCommentor(responseParameter.Response, listUserData);

            HasMoreResults = _hasmoreResults;
        }

        private void GetPostCommentor(string pageSource, List<string> fanpageResponseList)
        {
            string scrapedUrl = string.Empty;

            ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();

            foreach (string response in fanpageResponseList)
            {

                FacebookUser objFacebookUser = new FacebookUser();

                var decodedResponse = FdFunctions.GetDecodedResponse(response);

                string username = string.Empty;
                string userProfile = string.Empty;
                string date = string.Empty;


                try
                {
                    List<string> multipleUsers = new List<string>();
                    using (FdHtmlParseUtility htmlParseUtility = new FdHtmlParseUtility())
                    {
                        multipleUsers = htmlParseUtility.GetListOuterHtmlFromPartialTagNameContains(decodedResponse, "a", "class", "xggy1nq x1a2a7pz xt0b8zv xzsf02u x1s688f");
                    }

                    if (multipleUsers.Count > 1)
                    {


                        //    string todaysData1 = string.Empty;
                        foreach (string user in multipleUsers)
                        {
                            objFacebookUser = new FacebookUser();



                            username = string.Empty;
                            userProfile = string.Empty;
                            date = string.Empty;
                            try
                            {
                                string userId = FdFunctions.GetIntegerOnlyString(FdRegexUtility.FirstMatchExtractor(user, "user.php\\?id=(.*?)&"));

                                scrapedUrl = FdRegexUtility.FirstMatchExtractor(user, FdConstants.ScrapedUrlRegx);

                                if (string.IsNullOrEmpty(userId) || userId == "0")
                                {
                                    if (scrapedUrl.Contains("profile.php"))
                                        userId = FdFunctions.GetIntegerOnlyString(scrapedUrl);
                                    else if (scrapedUrl.Contains("?"))
                                    {
                                        scrapedUrl = scrapedUrl.Split('?').FirstOrDefault();
                                        userProfile = scrapedUrl.Split('/').LastOrDefault(x => !string.IsNullOrEmpty(x));
                                    }
                                    else
                                    {
                                        userProfile = scrapedUrl.Split('/').LastOrDefault(x => !string.IsNullOrEmpty(x));
                                    }
                                }

                                username = FdRegexUtility.FirstMatchExtractor(user, FdConstants.AriaLabelRegex);

                                if (string.IsNullOrEmpty(username))
                                    username = FdRegexUtility.FirstMatchExtractor(user, "alt=\"(.*?)\"");

                                if (string.IsNullOrEmpty(username))
                                    using (FdHtmlParseUtility htmlParseUtility = new FdHtmlParseUtility())
                                    {
                                        username = htmlParseUtility.GetInnerTextFromPartialTagName(user, "a", "role", "link");
                                    }
                                using (FdHtmlParseUtility htmlParseUtility = new FdHtmlParseUtility())
                                {
                                    date = htmlParseUtility.GetInnerTextFromPartialTagNameContains(decodedResponse, "span", "class", "x1lkfr7t x1lbecb7 x1s688f xzsf02u x1yc453h");
                                }


                                if ((!FdFunctions.IsIntegerOnly(userId) || userId == "0") & string.IsNullOrEmpty(userProfile))
                                    continue;
                                if (!string.IsNullOrEmpty(userId) && userId != "0" && FdFunctions.IsIntegerOnly(userId))
                                    objFacebookUser = FdConstants.getFaceBookUserFromUrlOrIdOrUserName(userId);
                                else if (!string.IsNullOrEmpty(scrapedUrl))
                                    objFacebookUser = FdConstants.getFaceBookUserFromUrlOrIdOrUserName(scrapedUrl);
                                objFacebookUser.UserId = userId;

                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                                objFacebookUser.IsAlreadyFriend = "true";
                            }

                            objFacebookUser.ClassName = FdConstants.SentFriend2Element;

                            objFacebookUser.Familyname = username;

                            objFacebookUser.ScrapedProfileUrl = scrapedUrl;
                            objFacebookUser.ProfileId = userProfile;
                            objFacebookUser.InteractionDate = date;

                            objFacebookUser.UserId = string.IsNullOrEmpty(objFacebookUser.UserId) || objFacebookUser.UserId == "0" ? objFacebookUser.ProfileId : objFacebookUser.UserId;

                            string[] splitValues = Regex.Split(pageSource, "discj3wi ihqw7lf3");
                            string todaysData = splitValues.FirstOrDefault(x => x.Contains("Today's birthdays"));
                            if (!string.IsNullOrEmpty(todaysData))
                                objFacebookUser.InteractionDate = todaysData.Contains(objFacebookUser.ScrapedProfileUrl) ? DateTime.Now.AddYears(-20).ToString("dd MMM yyyy",
                                            CultureInfo.InvariantCulture) : objFacebookUser.InteractionDate;

                            if (ObjFdScraperResponseParameters.ListUser.FirstOrDefault(x => x.ScrapedProfileUrl == objFacebookUser.ScrapedProfileUrl) == null)
                                ObjFdScraperResponseParameters.ListUser.Add(objFacebookUser);
                        }
                    }
                    else
                    {
                        try
                        {
                            string userId = FdFunctions.GetIntegerOnlyString(FdRegexUtility.FirstMatchExtractor(decodedResponse, "user.php\\?id=(.*?)&"));

                            scrapedUrl = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.ScrapedUrlRegx);

                            if (string.IsNullOrEmpty(userId) || userId == "0")
                            {
                                if (scrapedUrl.Contains("profile.php"))
                                    userId = FdFunctions.GetIntegerOnlyString(scrapedUrl);
                                else if (scrapedUrl.Contains("?"))
                                {
                                    scrapedUrl = scrapedUrl.Split('?').FirstOrDefault();
                                    userProfile = scrapedUrl.Split('/').LastOrDefault(x => !string.IsNullOrEmpty(x));
                                }
                                else
                                {
                                    userProfile = scrapedUrl.Split('/').LastOrDefault(x => !string.IsNullOrEmpty(x));
                                }
                            }

                            username = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.AriaLabelRegex);

                            if (string.IsNullOrEmpty(username))
                                username = FdRegexUtility.FirstMatchExtractor(decodedResponse, "alt=\"(.*?)\"");

                            using (FdHtmlParseUtility htmlParseUtility = new FdHtmlParseUtility())
                            {
                                date = htmlParseUtility.GetInnerTextFromPartialTagNameContains(decodedResponse, "span", "class", "x1lkfr7t x1lbecb7 x1s688f xzsf02u x1yc453h");
                            }


                            if ((!FdFunctions.IsIntegerOnly(userId) || userId == "0") & string.IsNullOrEmpty(userProfile))
                                continue;
                            if (!string.IsNullOrEmpty(userId) && userId != "0" && FdFunctions.IsIntegerOnly(userId))
                                objFacebookUser = FdConstants.getFaceBookUserFromUrlOrIdOrUserName(userId);
                            else if (!string.IsNullOrEmpty(scrapedUrl))
                                objFacebookUser = FdConstants.getFaceBookUserFromUrlOrIdOrUserName(scrapedUrl);
                            objFacebookUser.UserId = userId;

                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                            objFacebookUser.IsAlreadyFriend = "true";
                        }

                        objFacebookUser.ClassName = FdConstants.SentFriend2Element;

                        objFacebookUser.Familyname = username;

                        objFacebookUser.ScrapedProfileUrl = scrapedUrl;
                        objFacebookUser.ProfileId = userProfile;
                        objFacebookUser.InteractionDate = date;

                        objFacebookUser.UserId = string.IsNullOrEmpty(objFacebookUser.UserId) || objFacebookUser.UserId == "0" ? objFacebookUser.ProfileId : objFacebookUser.UserId;

                        string[] splitValues = Regex.Split(pageSource, "discj3wi ihqw7lf3");
                        string todaysData = splitValues.FirstOrDefault(x => x.Contains("Today's birthdays"));
                        if (!string.IsNullOrEmpty(todaysData))
                            objFacebookUser.InteractionDate = todaysData.Contains(objFacebookUser.ScrapedProfileUrl) ? DateTime.Now.AddYears(-20).ToString("dd MMM yyyy",
                                        CultureInfo.InvariantCulture) : objFacebookUser.InteractionDate;

                        if (ObjFdScraperResponseParameters.ListUser.FirstOrDefault(x => x.ScrapedProfileUrl == objFacebookUser.ScrapedProfileUrl) == null)
                            ObjFdScraperResponseParameters.ListUser.Add(objFacebookUser);
                    }


                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            Status = ObjFdScraperResponseParameters.ListUser.Count > 0;
        }

    }
}
