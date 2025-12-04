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
using FdHtmlParseUtility = FaceDominatorCore.FDLibrary.FdFunctions.FdHtmlParseUtility;

namespace FaceDominatorCore.FDResponse.CommonResponse
{
    public class FanpageLikersResponseHandler : FdResponseHandler, IResponseHandler
    {

        public bool HasMoreResults { get; set; }

        // public FdPageLikersParameters ObjFdPageLikersParameters { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public FanpageLikersResponseHandler(IResponseParameter responseParameter, FdPageLikersParameters objFdPageLikersParameters)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            if (FbErrorDetails == null)
            {
                if (objFdPageLikersParameters == null)
                    ObjFdScraperResponseParameters.FdPageLikersParameters = new FdPageLikersParameters();
                else
                {
                    ObjFdScraperResponseParameters.FdPageLikersParameters = objFdPageLikersParameters;
                    ObjFdScraperResponseParameters.FdPageLikersParameters.LstFacebookUser
                        = new List<FacebookUser>();
                }

                string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

                try
                {

                    if (!ObjFdScraperResponseParameters.FdPageLikersParameters.IsPagination)
                    {
                        //userId = Regex.Matches(DecodedResponse, "ORIGINAL_USER_ID:\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

                        GetpageLikers(decodedResponse);

                        GetEncodedQuery(responseParameter);

                        if (!Status)
                            GetpageLikersPartials(decodedResponse);
                    }
                    else
                        GetpageLikers(decodedResponse);

                    UpadetePaginationData(responseParameter);
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
            }
        }

        private void GetpageLikersPartials(string decodedResponse)
        {
            List<string> nodeCollection;
            using (var objHtmlParseUtility = new FdHtmlParseUtility())
            {
                nodeCollection =
                    objHtmlParseUtility.GetListInnerHtmlFromPartialTagName(decodedResponse, "div", "class",
                        "uiProfileBlockContent");
            }

            try
            {
                foreach (var node in nodeCollection)
                {
                    FacebookUser objFacebookUser = new FacebookUser();

                    try
                    {
                        var memberNode = node;

                        var pageLikersId = FdRegexUtility.FirstMatchExtractor(memberNode, "eng_tid\":\"(.*?)\"");

                        pageLikersId = FdFunctions.GetIntegerOnlyString(pageLikersId);

                        objFacebookUser.UserId = pageLikersId;

                        var scrapedUrl =
                            FdRegexUtility.FirstMatchExtractor(node, FdConstants.ScrapedUrlRegx);

                        objFacebookUser.ScrapedProfileUrl = scrapedUrl;

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }

                    ObjFdScraperResponseParameters.FdPageLikersParameters.LstFacebookUser.Add(objFacebookUser);
                }

                Status = ObjFdScraperResponseParameters.FdPageLikersParameters.LstFacebookUser.Count > 0;
                ObjFdScraperResponseParameters.ListUser = ObjFdScraperResponseParameters.FdPageLikersParameters.LstFacebookUser;
                ObjFdScraperResponseParameters.FacebookUser = new FacebookUser()
                {
                    UserId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.EntityIdRegex)
                };

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void GetpageLikers(string pageLikersData)
        {
            List<string> nodeCollection;

            try
            {

                using (var objHtmlParseUtility = new FdHtmlParseUtility())
                {
                    nodeCollection =
                        objHtmlParseUtility.GetListInnerHtmlFromPartialTagName(pageLikersData, "div", "class", "_3u1 _gli");
                }

                foreach (var node in nodeCollection)
                {
                    FacebookUser objFacebookUser = new FacebookUser();

                    var memberName = string.Empty;

                    try
                    {
                        var memberNode = node;
                        var pageLikersId = Regex.Matches(memberNode, "\"id\":(.*?),", RegexOptions.Singleline)[0].Groups[1].ToString();
                        pageLikersId = FdFunctions.GetIntegerOnlyString(pageLikersId);

                        objFacebookUser.UserId = pageLikersId;
                        try
                        {
                            var memberArray = Regex.Split(node, "_52eh _5bcu");

                            if (memberArray.Length > 1)
                                memberName = memberArray[1];

                            var scrapedUrl =
                                    FdRegexUtility.FirstMatchExtractor(memberName, FdConstants.ScrapedUrlRegx);

                            objFacebookUser.ScrapedProfileUrl = scrapedUrl;

                            memberName =
                                FdRegexUtility.FirstMatchExtractor(memberNode, FdConstants.UserNameModRegx);

                            if (memberName.Contains("<span"))
                                memberName = Regex.Split(memberName, "<span")[0];

                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        objFacebookUser.Familyname = memberName;

                        if (ObjFdScraperResponseParameters.FdPageLikersParameters.LstFacebookUser.FirstOrDefault(x => x.UserId == objFacebookUser.UserId) == null)
                            ObjFdScraperResponseParameters.FdPageLikersParameters.LstFacebookUser.Add(objFacebookUser);

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }


                }

                Status = ObjFdScraperResponseParameters.FdPageLikersParameters.LstFacebookUser.Count > 0;
            }

            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }

        }


        private void GetEncodedQuery(IResponseParameter responseParameter)
        {
            try
            {

                var encodedQuery = Regex.Split(responseParameter.Response, "{encoded_query:\"(.*?)encoded_query_no_rewrite", RegexOptions.Singleline)[1];

                encodedQuery = "{\"view\":\"list\",\"encoded_query\":\"" + encodedQuery + "\"encoded_title\":";

                var encodedTitle = Regex.Split(responseParameter.Response, "encoded_title:(.*?)},prefetchPixels", RegexOptions.Singleline)[1];

                ObjFdScraperResponseParameters.FdPageLikersParameters.FinalEncodedQuery = encodedQuery + encodedTitle + ",\"cursor\":";

            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
        }



        private void UpadetePaginationData(IResponseParameter responseParameter)
        {
            try
            {
                //pagination for friends
                string[] cursorDataArray = Regex.Split(responseParameter.Response, "BrowseScrollingPager");

                string cursorData = string.Empty;
                if (!ObjFdScraperResponseParameters.FdPageLikersParameters.IsPagination)
                {
                    if (cursorDataArray.Length >= 2)
                    {
                        cursorData = Regex.Split(cursorDataArray[2], "{cursor:(.*?),display_params", RegexOptions.Singleline)[1];
                        cursorData = cursorData.Replace(",", ",\"").Replace(":", "\":");
                    }
                }
                else
                    cursorData = Regex.Split(cursorDataArray[1], "{\"cursor\":(.*?),\"display_params", RegexOptions.Singleline)[1];

                string fullQueryParameters;
                if (string.IsNullOrEmpty(cursorData))
                {
                    HasMoreResults = false;
                    fullQueryParameters = string.Empty;
                }
                else
                {
                    HasMoreResults = true;
                    fullQueryParameters = $"{ObjFdScraperResponseParameters.FdPageLikersParameters.FinalEncodedQuery}{cursorData}}}";
                }

                //Pagination for followers
                if (string.IsNullOrEmpty(fullQueryParameters))
                {
                    var decodeResponse = FdFunctions.GetNewPrtialDecodedResponse(responseParameter.Response);

                    fullQueryParameters = $"ajax{FdRegexUtility.FirstMatchExtractor(decodeResponse, "a href=\"/ajax(.*?)\"")}";

                    HasMoreResults = !string.IsNullOrEmpty(fullQueryParameters);
                }
                PageletData = fullQueryParameters;
                ObjFdScraperResponseParameters.FdPageLikersParameters.PaginationData = fullQueryParameters;
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
        }
    }
}
