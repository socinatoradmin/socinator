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

namespace FaceDominatorCore.FDResponse.CommonResponse
{
    public class ScrapGroupPostListResponseHandler : FdResponseHandler, IResponseHandler
    {

        public string EntityId { get; set; }
        public string PageletData { get; set; }
        public bool HasMoreResults { get; set; }
        public bool Status { get; set; }
        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public ScrapGroupPostListResponseHandler(IResponseParameter responseParameter, List<KeyValuePair<string, string>> listPostReaction, string ajaxToken)
            : base(responseParameter)
        {

            if (responseParameter.HasError)
                return;

            ObjFdScraperResponseParameters.ListPostDetails = new List<FacebookPostDetails>();

            ObjFdScraperResponseParameters.ListPostReaction = listPostReaction;

            ObjFdScraperResponseParameters.AjaxToken = ajaxToken;

            string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            GetPostIdList(decodedResponse);

            GetPagelet(decodedResponse);
        }


        private void GetPagelet(string decodedResponse)
        {
            try
            {
                string pageletSection;
                if (string.IsNullOrEmpty(ObjFdScraperResponseParameters.AjaxToken))
                {
                    pageletSection = FdRegexUtility.FirstMatchExtractor(decodedResponse, "GroupEntstreamPagelet\",{(.*?)},");

                    if (!pageletSection.Contains("\"last_view_time\""))
                    {
                        pageletSection = "{\"" + Regex.Replace(pageletSection, ":", "\":").Replace(",", ",\"") + "}";
                        pageletSection = pageletSection.Replace("\"\"", "\"");
                    }
                    else
                        pageletSection = "{" + pageletSection + "}";

                    PageletData = pageletSection;

                    ObjFdScraperResponseParameters.AjaxToken = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.AjaxPipeTokenRegex);
                }
                else
                {
                    pageletSection = FdRegexUtility.FirstMatchExtractor(decodedResponse, "GroupEntstreamPagelet\",{(.*?)},");
                    PageletData = ("{" + pageletSection + "}");
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

                        var postId = FdRegexUtility.FirstMatchExtractor(postDetails, "value=\"(.*?)\"");

                        objFacebookPostDetails.Id = postId;

                        string postReactionDetails = ObjFdScraperResponseParameters.ListPostReaction.FirstOrDefault(x => x.Key == postId).Value;

                        var reactionArray = Regex.Split(postReactionDetails, "<:>").ToArray();

                        if (reactionArray.Length > 2)
                        {
                            objFacebookPostDetails.CommentorCount = reactionArray[0];
                            objFacebookPostDetails.LikersCount = reactionArray[1];
                            objFacebookPostDetails.SharerCount = reactionArray[2];
                        }


                        var timeDetails = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_5pcp _5lel _2jyu _232_\"])")[0].InnerHtml;

                        objHtmlDocument.LoadHtml(timeDetails);

                        var time = objHtmlDocument.DocumentNode.SelectNodes("//abbr")[0].OuterHtml;

                        time = FdRegexUtility.FirstMatchExtractor(time, "title=\"(.*?)\"");
                        try
                        {
                            var timePart = time.Split(' ')[1];

                            var datePart = time.Split(' ')[0];

                            if (timePart.ToLower().Contains("pm"))
                            {
                                timePart = Regex.Split(timePart.ToLower(), "pm")[0];
                                if (timePart.Length < 5)
                                {
                                    timePart = $"0{timePart}";
                                }

                                timePart = $"{timePart} PM";
                            }
                            else if (timePart.ToLower().Contains("am"))
                            {
                                timePart = Regex.Split(timePart.ToLower(), "am")[0];
                                if (timePart.Length < 5)
                                {
                                    timePart = $"0{timePart}";
                                }

                                timePart = $"{timePart} AM";
                            }

                            time = $"{datePart} {timePart}";

                            DateTime dt;

                            DateTime.TryParseExact(time, "MM/dd/yyyy hh:mm tt",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);

                            objFacebookPostDetails.PostedDateTime = dt;
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                            objFacebookPostDetails.PostedDateTime = DateTime.Now;
                        }

                        ObjFdScraperResponseParameters.ListPostDetails.Add(objFacebookPostDetails);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    Status = ObjFdScraperResponseParameters.ListPostDetails.Count > 0;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
