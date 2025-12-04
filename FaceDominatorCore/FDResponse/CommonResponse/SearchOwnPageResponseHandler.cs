using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.CommonResponse
{

    public class SearchOwnPageResponseHandler : FdResponseHandler, IResponseHandler
    {

        public string PageletData { get; set; }

        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();


        public SearchOwnPageResponseHandler(IResponseParameter responseParameter, List<FanpageDetails> lstFanpageDetails, bool isClassicVersion = true)
        : base(responseParameter)
        {

            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            ObjFdScraperResponseParameters.ListPage = lstFanpageDetails ?? new List<FanpageDetails>();


            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);


            try
            {

                string bookMarkResponse = decodedResponse;

                if (isClassicVersion)
                {
                    if (bookMarkResponse.Contains("BookmarkSeeAllEntsSectionController"))
                    {
                        bookMarkResponse = Regex.Matches(decodedResponse, "BookmarkSeeAllEntsSectionController(.*?)</script>", RegexOptions.Singleline)[0].Groups[1].ToString();

                        List<string> lstResponse = Regex.Split(bookMarkResponse, "{id:\"").ToList();

                        GetFanpageId(lstResponse);
                    }
                }
                else
                {
                    if (bookMarkResponse.Contains("profile_switcher_eligible_profiles"))
                    {
                        var respList = Regex.Matches(decodedResponse, "\"actor\":{(.*?)}]]]}").Cast<Match>()
                            .Select(match => match.Value).ToList();
                        bookMarkResponse = respList.FirstOrDefault(x => x.Contains("profile_switcher_eligible_profiles\":{\"nodes\""));
                        if (bookMarkResponse.Contains("if_viewer_can_login_as_profile_plus"))
                            bookMarkResponse = Regex.Matches(decodedResponse, "\"actor\":{(.*?)}}]]]}")[3].ToString();
                        var pageData = DominatorHouseCore.Utility.Utilities.GetBetween(bookMarkResponse, "\"actor\":", ",\"admined_pages");
                        if (string.IsNullOrEmpty(pageData))
                        {
                            bookMarkResponse = Regex.Match(decodedResponse, "\"profile_switcher_eligible_profiles\":{(.*?)}]}}},").ToString();
                            pageData = DominatorHouseCore.Utility.Utilities.GetBetween(bookMarkResponse, "\"profile_switcher_eligible_profiles\":", "}}},") + "}";
                        }

                        GetFanpageIdNewUI(pageData);
                    }
                }

                var fanpageData = ObjFdScraperResponseParameters.ListPage;
                if (fanpageData.Count() > 0)
                {
                    var count = 0;
                    foreach (var data in fanpageData)
                    {
                        if (data.FanPageName.Contains("u0e"))
                        {

                            using (FdHtmlParseUtility utility = new FdHtmlParseUtility())
                            {
                                var listData = utility.GetInnerHtmlFromTagName(decodedResponse, "div", "class",
                                                "x1n2onr6 x1ja2u2z x9f619 x78zum5 xdt5ytf x193iq5w x1l7klhg x1iyjqo2 xs83m0k x2lwn1j xh8yej3");
                                var lstData = utility.GetListOuterHtmlFromPartialTagName(listData,
                                                    "div", "class", "x78zum5 x1n2onr6 xh8yej3 x1iyjqo2");
                                data.FanPageName = utility.GetInnerTextFromPartialTagNameContains(lstData[count], "span",
                                    "class", "x3x7a5m x1603h9y x1u7k74 x1xlr1w8 xzsf02u");
                            }
                            count++;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void GetFanpageIdNewUI(string fanpageResponse)
        {
            DominatorHouseCore.Utility.JsonHandler fanPageData = null;

            try
            {
                fanPageData = new DominatorHouseCore.Utility.JsonHandler(fanpageResponse);
            }
            catch (Exception)
            {
                fanpageResponse = Regex.Split(fanpageResponse, ",\"admined_pages_for_acting_account\":").FirstOrDefault();
                fanPageData = new DominatorHouseCore.Utility.JsonHandler(fanpageResponse);
            }
            var lstData = fanPageData.GetJToken("profile_switcher_eligible_profiles", "nodes");
            foreach (var data in lstData)
            {
                try
                {
                    FanpageDetails objFanpageDetails = new FanpageDetails();
                    objFanpageDetails.FanPageID = fanPageData.GetJTokenValue(data, "profile", "id");
                    objFanpageDetails.FanPageName = fanPageData.GetJTokenValue(data, "profile", "name");
                    objFanpageDetails.FanPageUrl = fanPageData.GetJTokenValue(data, "profile", "url");
                    objFanpageDetails.FanPageProfilePicurl = fanPageData.GetJTokenValue(data, "profile", "profile_picture", "uri");
                    if (string.IsNullOrEmpty(objFanpageDetails.FanPageUrl))
                    {
                        objFanpageDetails.FanPageUrl = objFanpageDetails.FanPageUrl = $"{FdConstants.FbHomeUrl}{objFanpageDetails.FanPageID}";
                    }

                    if (ObjFdScraperResponseParameters.ListPage.FirstOrDefault(x => x.FanPageID == objFanpageDetails.FanPageID) == null)
                        ObjFdScraperResponseParameters.ListPage.Add(objFanpageDetails);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            }

            Status = ObjFdScraperResponseParameters.ListPage.Count > 0;


        }

        private void GetFanpageId(List<string> fanpageResponseList)
        {
            string fanpageId;


            foreach (string response in fanpageResponseList)
            {
                try
                {
                    FanpageDetails objFanpageDetails = new FanpageDetails();

                    if (!response.Contains("\"bmid\":\""))
                        continue;

                    fanpageId = Regex.Matches(response, "\"bmid\":\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

                    objFanpageDetails.FanPageID = fanpageId;

                    //var fanpageName = Regex.Matches(response, "title:\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

                    //objFanpageDetails.FanPageName = fanpageName;

                    objFanpageDetails.FanPageUrl = $"{FdConstants.FbHomeUrl}{fanpageId}";


                    if (ObjFdScraperResponseParameters.ListPage.FirstOrDefault(x => x.FanPageID == fanpageId) == null)
                        ObjFdScraperResponseParameters.ListPage.Add(objFanpageDetails);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            Status = ObjFdScraperResponseParameters.ListPage.Count > 0;


        }

    }
}
