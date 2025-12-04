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
    public class PostSharerResponseHandler : FdResponseHandler, IResponseHandler
    {

        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
        = new FdScraperResponseParameters();

        public PostSharerResponseHandler(IResponseParameter responseParameter, bool isFirstPageLet)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            ObjFdScraperResponseParameters.ListUser = new List<FacebookUser>();

            string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);


            try
            {
                GetPostSharers(decodedResponse, isFirstPageLet);

                UpadetePaginationData(decodedResponse, isFirstPageLet);

            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
        }


        private void GetPostSharers(string pageLikersData, bool isFirstPagelet)
        {
            try
            {
                HtmlNodeCollection objHtmlNodeCollection;

                HtmlDocument objHtmlDocument = new HtmlDocument();
                objHtmlDocument.LoadHtml(pageLikersData);

                if (isFirstPagelet)
                {
                    objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"repost_view_dialog\"])");

                    objHtmlDocument.LoadHtml(objHtmlNodeCollection[0].InnerHtml);
                }

                objHtmlNodeCollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_4-u2 mbm _4mrt _5jmm _5pat _5v3q _7cqq _4-u8\"])");//_4-u2 mbm _4mrt _5jmm _5pat _5v3q _4-u8


                foreach (var node in objHtmlNodeCollection)
                {
                    try
                    {

                        FacebookUser objFacebookUser = new FacebookUser();

                        var memberNode = node.OuterHtml;

                        HtmlDocument objNewDocument = new HtmlDocument();

                        objNewDocument.LoadHtml(memberNode);

                        var postSharerId = Regex.Matches(memberNode, "data-hovercard=\"(.*?) ", RegexOptions.Singleline)[0].Groups[1].ToString();
                        if (postSharerId.Contains("user.php"))
                        {
                            postSharerId = FdRegexUtility.FirstMatchExtractor(postSharerId, "user.php\\?id=(.*?)&");

                            var scrapedUrlDetails = objNewDocument.DocumentNode.SelectNodes("//a[@class=\"profileLink\"]");

                            var scrapedUrl = FdRegexUtility.FirstMatchExtractor(scrapedUrlDetails[0].OuterHtml, FdConstants.ScrapedUrlRegx);

                            var memberName = scrapedUrlDetails[0].InnerHtml;

                            objFacebookUser.FullName = memberName;

                            objFacebookUser.ScrapedProfileUrl = scrapedUrl;
                        }
                        else
                            continue;

                        postSharerId = FdFunctions.GetIntegerOnlyString(postSharerId);

                        objFacebookUser.UserId = postSharerId;

                        if (ObjFdScraperResponseParameters.ListUser.FirstOrDefault(x => x.UserId == objFacebookUser.UserId) == null)
                            ObjFdScraperResponseParameters.ListUser.Add(objFacebookUser);

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }

                    Status = ObjFdScraperResponseParameters.ListUser.Count > 0;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }

        }




        private void UpadetePaginationData(string responseParameter, bool isFirstPagelet)
        {
            try
            {
                var targetfbid = Regex.Matches(responseParameter, "target_fbid\":(.*?),", RegexOptions.Singleline)[0].Groups[1].ToString();
                var cursor = Regex.Matches(responseParameter, "cursor\":\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

                PageletData = isFirstPagelet
                    ? "{" + $"\"target_fbid\":{targetfbid},\"cursor\":\"{cursor}\",\"pager_fired_on_init\":true" + "}"
                    : "{" + $"\"target_fbid\":{targetfbid},\"cursor\":\"{cursor}\"" + "}";

                HasMoreResults = !string.IsNullOrEmpty(PageletData);
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
        }
    }
}
