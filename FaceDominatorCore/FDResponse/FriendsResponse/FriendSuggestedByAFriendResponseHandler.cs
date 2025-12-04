using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace FaceDominatorCore.FDResponse.FriendsResponse
{
    public class FriendSuggestedByAFriendResponseHandler : FdResponseHandler, IResponseHandler
    {

        public bool HasMoreResults { get; set; } = false;

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public FriendSuggestedByAFriendResponseHandler(IResponseParameter responseParameter, List<FacebookUser> lstFacebookUser)
            : base(responseParameter)
        {

            if (responseParameter.HasError)
                return;

            HtmlDocument objHtmlDocument = new HtmlDocument();

            List<string> friendSuggetionList = new List<string>();

            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);


            try
            {

                objHtmlDocument.LoadHtml(decodedResponse);

                if (lstFacebookUser == null)
                {
                    ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();

                    HtmlNodeCollection objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class, 'mtl _30d _5ewg')]");

                    if (objHtmlNodeCollection != null)
                    {
                        foreach (var objNode in objHtmlNodeCollection)
                        {
                            if (objNode.OuterHtml.Contains("fbSearchResultsBox"))
                            {
                                var suggestedFriendContent = objNode.InnerHtml;

                                objHtmlDocument.LoadHtml(suggestedFriendContent);

                                objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//li[starts-with(@class, 'friendBrowserListUnit')]");

                                if (objHtmlNodeCollection != null)
                                    foreach (var objChildNode in objHtmlNodeCollection)
                                        friendSuggetionList.Add(objChildNode.InnerHtml);

                                break;
                            }
                        }
                    }
                }
                else
                {
                    ObjFdScraperResponseParameters.ListUser = lstFacebookUser;

                    HtmlNodeCollection objHtmlNewNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//li[starts-with(@class, 'friendBrowserListUnit')]");

                    if (objHtmlNewNodeCollection != null)
                        foreach (var objChildNode in objHtmlNewNodeCollection)
                            friendSuggetionList.Add(objChildNode.InnerHtml);
                }

                GetFriends(friendSuggetionList);

                GetPaginationData(decodedResponse);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void GetPaginationData(string decodedResponse)
        {
            try
            {
                HtmlDocument objHtmlDocument = new HtmlDocument();

                ObjFdScraperResponseParameters.PaginationUserIds = ObjFdScraperResponseParameters.ListUser.Select(x => x.UserId).ToList();

                objHtmlDocument.LoadHtml(decodedResponse);

                HtmlNodeCollection objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class, 'mtl _30d _5ewg')]");

                if (objHtmlNodeCollection != null)
                {
                    foreach (var objNode in objHtmlNodeCollection)
                    {
                        if (objNode.OuterHtml.Contains("fbSearchResultsBox"))
                        {
                            objHtmlDocument.LoadHtml(objNode.InnerHtml);

                            objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//input[starts-with(@id, 'extra_data')]");

                            ObjFdScraperResponseParameters.ExtraData = FdRegexUtility.FirstMatchExtractor(objHtmlNodeCollection[0].OuterHtml, "value=\"(.*?)\"");

                            break;
                        }
                    }
                }

                else
                {
                    objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//input[starts-with(@id, 'extra_data')]");

                    if (objHtmlNodeCollection != null)
                        ObjFdScraperResponseParameters.ExtraData = FdRegexUtility.FirstMatchExtractor(objHtmlNodeCollection[0].OuterHtml, "value=\"(.*?)\"");

                }


            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void GetFriends(List<string> friendSuggetionList)
        {
            HtmlDocument objHtmlDocument = new HtmlDocument();


            try
            {
                foreach (string friendItem in friendSuggetionList)
                {
                    FacebookUser objFacebookUser = new FacebookUser();

                    objHtmlDocument.LoadHtml(friendItem);

                    var friendDetails = objHtmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class, 'friendBrowserNameTitle')]")[0].InnerHtml;

                    objHtmlDocument.LoadHtml(friendDetails);

                    objFacebookUser.UserId = FdRegexUtility.FirstMatchExtractor(friendDetails, "\"eng_tid\":\"(.*?)\"");

                    objFacebookUser.Familyname = WebUtility.HtmlDecode(objHtmlDocument.DocumentNode.InnerText);

                    if (ObjFdScraperResponseParameters.ListUser.FirstOrDefault(x => x.UserId == objFacebookUser.UserId) == null)
                        ObjFdScraperResponseParameters.ListUser.Add(objFacebookUser);

                    Status = ObjFdScraperResponseParameters.ListUser.Count > 0;
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
