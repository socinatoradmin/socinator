/*
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.Interface;

namespace FaceDominatorCore.FDResponse.NewResponseHandler
{
    public class FanpageLikersResponseHandlerNew : FdResponseHandler, IResponseHandler
    {

        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }


        public FanpageLikersResponseHandlerNew(IResponseParameter responseParameter, FdScraperResponseParameters objFdPageLikersParameters)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            if (FbErrorDetails == null)
            {

                if (objFdPageLikersParameters == null)
                {
                    ObjFdScraperResponseParameters = new FdScraperResponseParameters();
                }
                else
                {
                    ObjFdScraperResponseParameters = objFdPageLikersParameters;
                    ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();
                }


                string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

                

                try
                {


                    if (!ObjFdScraperResponseParameters.IsPagination)
                    {
                        //userId = Regex.Matches(DecodedResponse, "ORIGINAL_USER_ID:\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

                        

                        GetpageLikers(decodedResponse);

                        GetEncodedQuery(responseParameter);
                    }
                    else
                    {
                        GetpageLikers(decodedResponse);
                    }


                    UpadetePaginationData(responseParameter);
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
            }
        }


        private void GetpageLikers(string pageLikersData)
        {
            HtmlDocument objHtmlDocument = new HtmlDocument();

            objHtmlDocument.LoadHtml(pageLikersData);
            HtmlNodeCollection objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class, '_3u1 _gli')]");

            FdFunctions objFdFunctions = new FdFunctions();

            var nodeCollection = objFdFunctions.GetInnerHtmlListFromNodeCollection(objHtmlNodeCollection);

            try
            {
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
                            objHtmlDocument.LoadHtml(node);

                            objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_52eh _5bcu\"])");

                            memberName = objHtmlNodeCollection[0].InnerHtml;

                            memberName = FdRegexUtility.FirstMatchExtractor(memberNode, FdConstants.UserNameModRegx);

                            if (memberName.Contains("<span"))
                                memberName = Regex.Split(memberName, "<span")[0];

                            try
                            {
                                var friendButtonDetails = objHtmlDocument.DocumentNode.SelectNodes("(//button[@class=\"_42ft _4jy0 FriendRequestAdd addButton _4jy3 _517h _51sy\"])")[0].OuterHtml;

                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                                objFacebookUser.IsAlreadyFriend = true;
                            }

                        }
                        catch (Exception ex)
                        {
                            try
                            {

                                memberName = objHtmlNodeCollection[0].InnerHtml;

                                memberName = FdRegexUtility.FirstMatchExtractor(memberNode, FdConstants.MemberNameModRegx);

                            }
                            catch (Exception e)
                            {
                                e.DebugLog();
                            }
                            ex.DebugLog();
                        }

                        objFacebookUser.Familyname = memberName;


                        if (ObjFdScraperResponseParameters.ListUser.FirstOrDefault(x => x.UserId == objFacebookUser.UserId) == null)
                        {
                            ObjFdScraperResponseParameters.ListUser.Add(objFacebookUser);
                        }


                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }


                }

                if (ObjFdScraperResponseParameters.ListUser.Count > 0)
                    Success = true;
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

                var encodedTitle = Regex.Split(responseParameter.Response, "encoded_title:(.*?)},null", RegexOptions.Singleline)[1];

                ObjFdScraperResponseParameters.FinalEncodedQuery = encodedQuery + encodedTitle + ",\"cursor\":";

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
                string[] cursorDataArray = Regex.Split(responseParameter.Response, "BrowseScrollingPager");

                string cursorData;
                if (!ObjFdScraperResponseParameters.IsPagination)
                {
                    cursorData = Regex.Split(cursorDataArray[2], "{cursor:(.*?),display_params", RegexOptions.Singleline)[1];
                    cursorData = cursorData.Replace(",", ",\"").Replace(":", "\":");
                }
                else
                {
                    cursorData = Regex.Split(cursorDataArray[1], "{\"cursor\":(.*?),\"display_params", RegexOptions.Singleline)[1];
                }

                string fullQueryParameters;
                if (string.IsNullOrEmpty(cursorData))
                {
                    HasMoreResults = false;
                    fullQueryParameters = string.Empty;
                }
                else
                {
                    HasMoreResults = true;
                    fullQueryParameters = $"{ObjFdScraperResponseParameters.FinalEncodedQuery}{cursorData}}}";
                }

                PageletData = fullQueryParameters;
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
        }
    }
}
*/
