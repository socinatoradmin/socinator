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

    public class ScrapPostListFromFanpageResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
        = new FdScraperResponseParameters();


        public ScrapPostListFromFanpageResponseHandler(IResponseParameter responseParameter, List<KeyValuePair<string, string>> listPostReaction)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            ObjFdScraperResponseParameters.ListPostDetails = new List<FacebookPostDetails>();

            ObjFdScraperResponseParameters.ListPostReaction = listPostReaction;

            string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            GetPostIdList(decodedResponse);

            GetPagelet(responseParameter.Response);

        }


        private void GetPagelet(string response)
        {
            try
            {
                if (response.Contains("cursor="))
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
            string postId;

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

                        if (objHtmlDocument.DocumentNode.SelectNodes("(//input[@name=\"ft_ent_identifier\"])") != null)
                        {
                            var postDetails = objHtmlDocument.DocumentNode.SelectNodes("(//input[@name=\"ft_ent_identifier\"])")[0].OuterHtml;

                            postId = Regex.Matches(postDetails, "value=\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

                            objFacebookPostDetails.Id = postId;

                            string postReactionDetails = ObjFdScraperResponseParameters.ListPostReaction.FirstOrDefault(x => x.Key == postId).Value;
                            //some time values are not matching so giving null value from that exception is throwing
                            if (!string.IsNullOrEmpty(postReactionDetails))
                            {
                                var reactionArray = Regex.Split(postReactionDetails, "<:>").ToArray();
                                if (reactionArray.Length > 2)
                                {
                                    objFacebookPostDetails.CommentorCount = reactionArray[0];
                                    objFacebookPostDetails.LikersCount = reactionArray[1];
                                    objFacebookPostDetails.SharerCount = reactionArray[2];
                                }
                            }
                        }

                        if (ObjFdScraperResponseParameters.ListPostDetails.FirstOrDefault(x => x.Id == objFacebookPostDetails.Id) == null)
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
