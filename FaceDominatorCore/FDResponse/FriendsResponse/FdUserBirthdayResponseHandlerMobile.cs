using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDResponse.FriendsResponse
{
    public class FdUserBirthdayResponseHandlerMobile : FdResponseHandler, IResponseHandler
    {

        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; } = new FdScraperResponseParameters();


        public FdUserBirthdayResponseHandlerMobile(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();
            if (responseParameter.HasError)
                return;


            try
            {
                var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);
                if (!string.IsNullOrEmpty(decodedResponse))
                    GetAllUserDetail(decodedResponse);

                PageletData = FdRegexUtility.FirstMatchExtractor(decodedResponse, "cursor=(.*?)\"");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        public void GetAllUserDetail(string decodedResponse)
        {

            try
            {

                using (FdHtmlParseUtility objHtmlParseUtility = new FdHtmlParseUtility())
                {
                    try
                    {
                        var recentBirthDays = objHtmlParseUtility.GetInnerHtmlFromTagName(decodedResponse, "article", "title",
                                            "Recent Birthdays");

                        if (!string.IsNullOrEmpty(recentBirthDays))
                            GetListUsers(recentBirthDays, true);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    try
                    {
                        var upcommingBirthDays = objHtmlParseUtility.GetInnerHtmlFromPartialTagName(decodedResponse, "article", "title",
                            "Today");

                        if (!string.IsNullOrEmpty(upcommingBirthDays))
                            GetListUsers(upcommingBirthDays, true);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    try
                    {
                        var upcommingBirthDays = objHtmlParseUtility.GetInnerHtmlFromTagName(decodedResponse, "article", "title",
                            "Upcoming Birthdays");

                        if (!string.IsNullOrEmpty(upcommingBirthDays))
                            GetListUsers(upcommingBirthDays, false);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    try
                    {
                        var laterThisMonthBirthDays = objHtmlParseUtility.GetInnerHtmlFromPartialTagName(decodedResponse, "article", "title",
                            "Later in");

                        if (!string.IsNullOrEmpty(laterThisMonthBirthDays))
                            GetListUsers(laterThisMonthBirthDays, false);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        public void GetListUsers(string decodedResponse, bool isRecentBirthday)
        {

            using (FdHtmlParseUtility objHtmlParseUtility = new FdHtmlParseUtility())
            {
                List<string> lstUserList;

                if (isRecentBirthday)
                {
                    lstUserList = objHtmlParseUtility.GetListInnerHtmlFromTagName
                        (decodedResponse, "div", "class", "_55ws _2vyq");
                }
                else
                {
                    lstUserList = objHtmlParseUtility.GetListInnerHtmlFromTagName
                        (decodedResponse, "li", "class", "_55ws _5as-");
                }

                foreach (var userDetails in lstUserList)
                {
                    try
                    {
                        var innerLinkDetails = objHtmlParseUtility.GetInnerHtmlFromTagName
                                        (userDetails, "a", "", "");

                        var link = FdRegexUtility.FirstMatchExtractor(objHtmlParseUtility.GetOuterHtmlFromTagName
                            (userDetails, "a", "", ""), FdConstants.ScrapedUrlRegx);

                        var userName = objHtmlParseUtility.GetInnerHtmlFromTagName
                            (innerLinkDetails, "p", "class", "_52jh _5at0 _592p");

                        var userBirthDay = objHtmlParseUtility.GetInnerHtmlFromTagName
                            (innerLinkDetails, "p", "class", "_52jc _52ja _5iev _592p");

                        var userAgeList = objHtmlParseUtility.GetListInnerHtmlFromTagName
                            (innerLinkDetails, "p", "class", "_52jc _52ja _5iev _592p");

                        FacebookUser objFacebookUser = new FacebookUser()
                        {
                            ScrapedProfileUrl = link.Contains("m.facebook") ? link : $"https://m.facebook.com{link}",
                            Familyname = userName,
                            DateOfBirth = userBirthDay,
                            Age = userAgeList.Count > 1 ? FdFunctions.GetIntegerOnlyString(userAgeList[1]) : string.Empty
                        };

                        if (ObjFdScraperResponseParameters.ListUser?.FirstOrDefault(
                                x => x.ScrapedProfileUrl == objFacebookUser.ScrapedProfileUrl) == null)
                        {
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
}


