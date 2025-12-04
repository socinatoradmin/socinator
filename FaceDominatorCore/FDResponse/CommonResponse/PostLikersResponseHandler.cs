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
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.CommonResponse
{
    public class PostLikersResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public PostLikersResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();

            string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);


            try
            {
                GetPostLikers(decodedResponse);

                UpadetePaginationData(decodedResponse);

            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
        }


        private void GetPostLikers(string pageLikersData)
        {
            HtmlDocument objHtmlDocument = new HtmlDocument();

            objHtmlDocument.LoadHtml(pageLikersData);
            HtmlNodeCollection objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("//li");

            try
            {
                foreach (var node in objHtmlNodeCollection)
                {
                    FacebookUser objFacebookUser = new FacebookUser();

                    try
                    {

                        if (!node.InnerHtml.Contains("page.php?id"))
                        {
                            var memberNode = node.InnerHtml;
                            var postLikersId = Regex.Matches(memberNode, "eng_tid\":\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();
                            postLikersId = FdFunctions.GetIntegerOnlyString(postLikersId);

                            var scrapedUrl = FdRegexUtility.FirstMatchExtractor(memberNode, FdConstants.ScrapedUrlRegx);

                            objFacebookUser.ScrapedProfileUrl = scrapedUrl;

                            objFacebookUser.UserId = postLikersId;

                            objFacebookUser.Familyname = FdRegexUtility.FirstMatchExtractor(node.InnerHtml, "aria-label=\"(.*?)\"");

                            if (node.InnerHtml.Contains("class=\"_42ft _4jy0 FriendRequestAdd addButton hidden_elem _4jy3 _517h _51sy\""))
                                objFacebookUser.IsAlreadyFriend = "true";

                            if (ObjFdScraperResponseParameters.ListUser.FirstOrDefault(x => x.UserId == objFacebookUser.UserId) == null)
                                ObjFdScraperResponseParameters.ListUser.Add(objFacebookUser);

                        }

                        Status = ObjFdScraperResponseParameters.ListUser.Count > 0;

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }

        }




        private void UpadetePaginationData(string responseParameter)
        {
            try
            {
                ObjFdScraperResponseParameters.ShownIds = (Regex.Matches(responseParameter, "shown_ids=(.*?)&", RegexOptions.Singleline).Count > 0 && Regex.Matches(responseParameter, "shown_ids=(.*?)&", RegexOptions.Singleline)[0].Groups.Count > 0)
                    ? Regex.Matches(responseParameter, "shown_ids=(.*?)&", RegexOptions.Singleline)[0].Groups[1].ToString()
                    : string.Empty;
                ObjFdScraperResponseParameters.TotalCount = (Regex.Matches(responseParameter, "total_count=(.*?)&", RegexOptions.Singleline).Count > 0 && Regex.Matches(responseParameter, "total_count=(.*?)&", RegexOptions.Singleline)[0].Groups.Count > 0)
                    ? Regex.Matches(responseParameter, "total_count=(.*?)&", RegexOptions.Singleline)[0].Groups[1].ToString()
                    : string.Empty;

                HasMoreResults = !string.IsNullOrEmpty(ObjFdScraperResponseParameters.TotalCount);
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
        }
    }
}
