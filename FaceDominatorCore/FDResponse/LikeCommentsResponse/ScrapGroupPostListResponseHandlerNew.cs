using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
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

namespace FaceDominatorCore.FDResponse.LikeCommentsResponse
{
    public class ScrapGroupPostListResponseHandlerNew : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }
        public string EntityId { get; set; }
        public string PageletData { get; set; }
        public bool Status { get; set; }
        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; } = new FdScraperResponseParameters();

        public ScrapGroupPostListResponseHandlerNew(IResponseParameter responseParameter, List<KeyValuePair<string, string>> listPostReaction, string ajaxToken)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            ObjFdScraperResponseParameters = new FdScraperResponseParameters
            {
                ListPostDetails = new List<FacebookPostDetails>(),
                ListPostReaction = listPostReaction,
                AjaxToken = ajaxToken
            };

            string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);
            GetPostIdList(decodedResponse);
            GetPagelet(decodedResponse);
        }


        private void GetPagelet(string decodedResponse)
        {
            try
            {
                //string pageletSection;
                if (string.IsNullOrEmpty(ObjFdScraperResponseParameters.AjaxToken))
                {
                    PageletData = FdRegexUtility.FirstMatchExtractor(decodedResponse, "GroupEntstreamPagelet\",{(.*?)},");

                    PageletData = !PageletData.Contains("\"last_view_time\"")
                        ? "{\"" + Regex.Replace(PageletData, ":", "\":").Replace(",", ",\"") + "}"
                        : "{" + PageletData + "}";

                    //if (!pageletSection.Contains("\"last_view_time\""))
                    //{
                    //    pageletSection = "{\"" + Regex.Replace(pageletSection, ":", "\":").Replace(",", ",\"") + "}";
                    //}
                    //else
                    //{
                    //    pageletSection = "{" + pageletSection + "}";
                    //}
                    //PageletData = pageletSection;

                    var ajaxTokenSection = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.AjaxPipeTokenRegex);

                    ObjFdScraperResponseParameters.AjaxToken = ajaxTokenSection;
                }
                else
                {
                    PageletData = FdRegexUtility.FirstMatchExtractor(decodedResponse, "GroupEntstreamPagelet\",{(.*?)},");
                    PageletData = ("{" + PageletData + "}");
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

                        if (objHtmlDocument.DocumentNode.SelectNodes("(//input[@name=\"ft_ent_identifier\"])") != null)
                        {
                            var postDetails = objHtmlDocument.DocumentNode.SelectNodes("(//input[@name=\"ft_ent_identifier\"])")[0].OuterHtml;
                            var postId = FdRegexUtility.FirstMatchExtractor(postDetails, "value=\"(.*?)\"");
                            objFacebookPostDetails.Id = postId;
                            objFacebookPostDetails.OwnerId = Utilities.GetBetween(node, "user.php?id=", "&");

                            string postReactionDetails = ObjFdScraperResponseParameters.ListPostReaction
                                .FirstOrDefault(x => x.Key == postId).Value;

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

                            var timeDetails = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_5pcp _5lel _2jyu _232_\"])")[0].InnerHtml;

                            objHtmlDocument.LoadHtml(timeDetails);

                            if (objHtmlDocument.DocumentNode.SelectNodes("//abbr") != null)
                            {
                                var time = objHtmlDocument.DocumentNode.SelectNodes("//abbr")[0].OuterHtml;

                                time = FdRegexUtility.FirstMatchExtractor(time, "title=\"(.*?)\"");
                                try
                                {
                                    if (time != null)
                                    {
                                        var timePart = time.Split(' ')[1];

                                        var datePart = time.Split(' ')[0];

                                        if (timePart.ToLower().Contains("pm"))
                                        {
                                            timePart = Regex.Split(timePart.ToLower(), "pm")[0];

                                            if (timePart.Length < 5)
                                                timePart = $"0{timePart}";

                                            timePart = $"{timePart} PM";
                                        }
                                        else if (timePart.ToLower().Contains("am"))
                                        {
                                            timePart = Regex.Split(timePart.ToLower(), "am")[0];

                                            if (timePart.Length < 5)
                                                timePart = $"0{timePart}";

                                            timePart = $"{timePart} AM";
                                        }

                                        time = $"{datePart} {timePart}";

                                        DateTime dt;

                                        DateTime.TryParseExact(time, "MM/dd/yyyy hh:mm tt",
                                            CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);

                                        objFacebookPostDetails.PostedDateTime = dt;
                                    }
                                    else
                                        objFacebookPostDetails.PostedDateTime = DateTime.Now;

                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                    objFacebookPostDetails.PostedDateTime = DateTime.Now;
                                }

                                objFacebookPostDetails.PostUrl = FdConstants.FbHomeUrl + objFacebookPostDetails.Id;

                                ObjFdScraperResponseParameters.ListPostDetails.Add(objFacebookPostDetails);
                            }
                        }
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
