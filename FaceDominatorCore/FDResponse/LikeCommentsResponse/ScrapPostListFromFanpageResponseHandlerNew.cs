using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.LikeCommentsResponse
{
    public class ScrapPostListFromFanpageResponseHandlerNew : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; } = new FdScraperResponseParameters();

        public ScrapPostListFromFanpageResponseHandlerNew(IResponseParameter responseParameter, List<KeyValuePair<string, string>> listPostReaction)
            : base(responseParameter)
        {

            if (responseParameter.HasError)
                return;
            try
            {
                ObjFdScraperResponseParameters = new FdScraperResponseParameters
                {
                    ListPostDetails = new List<FacebookPostDetails>(),
                    ListPostReaction = listPostReaction
                };




                string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

                GetPostIdList(decodedResponse);

                GetPagelet(responseParameter.Response);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void GetPagelet(string response)
        {
            try
            {
                if (Regex.Matches(response, "cursor=(.*?)&", RegexOptions.Singleline)[0] != null && Regex.Matches(response, "cursor=(.*?)&", RegexOptions.Singleline)[0].Groups.Count > 1)
                {
                    var pageletSection = Regex.Matches(response, "cursor=(.*?)&", RegexOptions.Singleline)[0].Groups[1].ToString();

                    pageletSection = FdFunctions.GetUnicodeDecodedResponse(pageletSection);

                    pageletSection = Uri.UnescapeDataString(pageletSection);

                    PageletData = pageletSection;

                }
                HasMoreResults = !string.IsNullOrEmpty(PageletData);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void GetPostIdList(string decodedResponse)
        {
            FdFunctions objFdFunctions = new FdFunctions();

            HtmlDocument objHtmlDocument = new HtmlDocument();

            objHtmlDocument.LoadHtml(decodedResponse);

            try
            {
                //"(//div[@class=\"_4-u2 mbm _4mrt _5jmm _5pat _5v3q _4-u8\"])"

                HtmlNodeCollection objNodecollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_4-u2 mbm _4mrt _5jmm _5pat _5v3q _7cqq _4-u8\"])");

                List<string> objNodeList = objFdFunctions.GetInnerHtmlListFromNodeCollection(objNodecollection);

                foreach (string node in objNodeList)
                {
                    FacebookPostDetails objFacebookPostDetails = new FacebookPostDetails();

                    try
                    {
                        objHtmlDocument.LoadHtml(node);
                        var postDetails = objHtmlDocument.DocumentNode.SelectNodes("(//input[@name=\"ft_ent_identifier\"])")[0].OuterHtml;

                        var postId = Regex.Matches(postDetails, "value=\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

                        objFacebookPostDetails.Id = postId;


                        objFacebookPostDetails.PostUrl = FdConstants.FbHomeUrl + objFacebookPostDetails.Id;


                        if (FdFunctions.IsIntegerOnly(postId))
                            ObjFdScraperResponseParameters.ListPostDetails.Add(objFacebookPostDetails);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                Status = ObjFdScraperResponseParameters.ListPostDetails.Count > 0;

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
