using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.AccountsResponse
{

    public class FdFriendsInfoNewResponseHandler : FdResponseHandler, IResponseHandler
    {

        public bool HasMoreResults { get; set; } = true;

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; } = new FdScraperResponseParameters();


        public FdFriendsInfoNewResponseHandler(IResponseParameter responseParameter, FriendsPager objPager,
            string userId, bool isExtraPager) : base(responseParameter)
        {

            if (responseParameter.HasError)
                return;

            HtmlDocument objHtmlDocument = new HtmlDocument();

            List<string> friendSuggetionList = new List<string>();

            if (objPager != null)
            {
                ObjFdScraperResponseParameters.FriendsPager = objPager;
                ObjFdScraperResponseParameters.FriendsPager.IsExtraPager = isExtraPager;
            }
            else
            {
                ObjFdScraperResponseParameters.FriendsPager = new FriendsPager();
            }

            ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();

            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);


            try
            {

                objHtmlDocument.LoadHtml(decodedResponse);
                HtmlNodeCollection objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//table[starts-with(@class,'uiGrid _51mz _5f0n')])");

                if (objHtmlNodeCollection != null)
                {
                    foreach (var objNode in objHtmlNodeCollection)
                    {
                        if (objNode.InnerText.Contains("removed"))
                            continue;

                        friendSuggetionList.Add(objNode.OuterHtml);
                    }

                    GetFriends(friendSuggetionList, responseParameter.Response);

                }


                GetPaginationData(responseParameter.Response, userId);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void GetPaginationData(string decodedResponse, string userId)
        {
            try
            {
                string previousShownTime = string.Empty;
                string scrubberMonth = string.Empty;
                string scrubberYear = string.Empty;
                string previousCursor = string.Empty;
                string maxKey = string.Empty;
                string currentKey = string.Empty;


                if (!ObjFdScraperResponseParameters.FriendsPager.IsExtraPager)
                {
                    int yearInt;
                    int monthInt;
                    string year;
                    string month;
                    if (string.IsNullOrEmpty(ObjFdScraperResponseParameters.FriendsPager.Data))
                    {
                        year = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.YearRegex);
                        month = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.MonthRegex);
                        previousShownTime = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.PreviousShownTimeRegex);
                        scrubberMonth = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.ScrubberMonthRegex);
                        scrubberYear = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.ScrubberYearRegex);
                        var ajaxpipeToken = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.AjaxPipeTokenRegex);
                        previousCursor = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.PreviousCursorRegex);
                        var maxKeyList = Regex.Matches(decodedResponse, "data-key=\"(.*?)\"", RegexOptions.Singleline);

                        if (maxKeyList.Count > 0)
                            maxKey = maxKeyList[maxKeyList.Count - 1].Groups[1].ToString();


                        currentKey = $"month_{year}_{month}";

                        if (Int32.TryParse(year, out yearInt) && Int32.TryParse(month, out monthInt))
                        {
                            Int32.TryParse(year, out yearInt);
                            Int32.TryParse(month, out monthInt);
                            if (monthInt - 1 > 0)
                            {
                                year = yearInt.ToString();
                                month = (monthInt - 1).ToString();
                            }
                            else
                            {
                                month = 12.ToString();
                                year = (yearInt - 1).ToString();
                            }
                        }



                        ObjFdScraperResponseParameters.FriendsPager.CurrentDataKey = currentKey;
                        ObjFdScraperResponseParameters.FriendsPager.MaxDataKey = maxKey;
                        ObjFdScraperResponseParameters.FriendsPager.AjaxPipeToken = ajaxpipeToken;
                    }
                    else
                    {

                        year = FdRegexUtility.FirstMatchExtractor(ObjFdScraperResponseParameters.FriendsPager.Data, FdConstants.YearPageRegex);
                        month = FdRegexUtility.FirstMatchExtractor(ObjFdScraperResponseParameters.FriendsPager.Data, FdConstants.MonthPageRegx);

                        if (!string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(month))
                            currentKey = $"month_{year}_{month}";
                        else
                            HasMoreResults = false;

                        if (decodedResponse.Contains("prev_shown_time="))
                        {
                            previousShownTime = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.PreviousShownTimeRegex);
                            scrubberMonth = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.ScrubberMonthRegex);
                            scrubberYear = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.ScrubberYearRegex);
                            previousCursor = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.PreviousCursorRegex);

                        }

                        if (Int32.TryParse(year, out yearInt) && Int32.TryParse(month, out monthInt))
                        {
                            Int32.TryParse(year, out yearInt);
                            Int32.TryParse(month, out monthInt);
                            if (monthInt - 1 > 0)
                            {
                                year = yearInt.ToString();
                                month = (monthInt - 1).ToString();
                            }
                            else
                            {
                                month = 12.ToString();
                                year = (yearInt - 1).ToString();
                            }
                        }


                    }


                    ObjFdScraperResponseParameters.FriendsPager.CurrentDataKey = currentKey;
                    ObjFdScraperResponseParameters.FriendsPager.Data = "{\"year\":" + year + ",\"month\":" + month + ",\"log_filter\":\"friends\",\"profile_id\":" + userId + "}";
                    ObjFdScraperResponseParameters.FriendsPager.ScrubberMonth = scrubberMonth;
                    ObjFdScraperResponseParameters.FriendsPager.ScrubberYear = scrubberYear;
                }
                else
                {
                    try
                    {

                        previousShownTime = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.PreviousShownTimeRegex);
                        previousCursor = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.PreviousCursorRegex);

                    }
                    catch (ArgumentException)
                    {

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                ObjFdScraperResponseParameters.FriendsPager.ShownTime = previousShownTime;
                ObjFdScraperResponseParameters.FriendsPager.PrevousCursor = Uri.UnescapeDataString(FdFunctions.GetNewPrtialDecodedResponse(previousCursor));

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void GetFriends(List<string> friendSuggetionList, string response)
        {
            string friendId = string.Empty;
            var friendName = string.Empty;

            string partialDecodedResponse = response.Replace("\\u003C", "<");

            HtmlDocument objHtmlDocument = new HtmlDocument();

            try
            {
                foreach (string friendItem in friendSuggetionList)
                {
                    try
                    {
                        objHtmlDocument.LoadHtml(friendItem);

                        var nodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//a[@class=\"profileLink\"])");

                        if (nodeCollection.Count > 1)
                        {
                            if (nodeCollection[1].OuterHtml.Contains("user.php?"))
                            {
                                friendId = FdRegexUtility.FirstMatchExtractor(nodeCollection[1].OuterHtml, "user.php\\?id=(.*?)&");

                                if (friendId.Contains("&"))
                                    friendId = friendId.Split('&')[0];

                                var friendNameDetails = FdRegexUtility.FirstMatchExtractor(partialDecodedResponse, $"id={friendId}(.*?)/a>");

                                friendName = FdRegexUtility.FirstMatchExtractor(friendNameDetails, FdConstants.EntityNameRegex);

                                friendName = FdFunctions.GetNewPrtialDecodedResponse(friendName);

                            }

                        }

                        nodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//span[@class=\"_5shl fss\"])");

                        var interactionDate = nodeCollection[0].InnerText;

                        DateTime friendshipDate;

                        if (!DateTime.TryParse(interactionDate, out friendshipDate))
                            interactionDate = DateTime.Now.ToString(CultureInfo.InvariantCulture);


                        FacebookUser objFacebookUser = new FacebookUser
                        {
                            UserId = friendId,
                            Familyname = friendName,
                            InteractionDate = interactionDate ?? friendshipDate.ToString(CultureInfo.InvariantCulture)
                        };


                        if (ObjFdScraperResponseParameters.ListUser.FirstOrDefault(x => x.UserId == friendId) == null && !string.IsNullOrEmpty(friendId))
                            ObjFdScraperResponseParameters.ListUser.Add(objFacebookUser);
                    }
                    catch (ArgumentException) { }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                if (ObjFdScraperResponseParameters.ListUser.Count > 0)
                    Status = true;

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }



    }
}
