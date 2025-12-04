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

namespace FaceDominatorCore.FDResponse.CommonResponse
{
    public class ScrapPostListFromTimelineResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();


        public ScrapPostListFromTimelineResponseHandler(IResponseParameter responseParameter, string ajaxpipeToken, List<KeyValuePair<string, string>> ListPostReaction)
            : base(responseParameter)
        {

            if (responseParameter.HasError)
                return;

            ObjFdScraperResponseParameters = new FdScraperResponseParameters();

            ObjFdScraperResponseParameters.AjaxToken = ajaxpipeToken;

            ObjFdScraperResponseParameters.ListPostDetails = new List<FacebookPostDetails>();

            ObjFdScraperResponseParameters.ListPostReaction = ListPostReaction;

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
                    pageletSection = Regex.Matches(decodedResponse, "ProfileTimelineJumperStoriesPagelet\",{(.*?)\"},", RegexOptions.Singleline)[0].Groups[1].ToString();

                    var cursor = FdRegexUtility.FirstMatchExtractor(pageletSection, "cursor:\"(.*?)\"");

                    pageletSection = pageletSection.Replace(",\"", ",").Replace(",", ",\"");

                    pageletSection = pageletSection.Replace(":", "\":");

                    PageletData = "{\"" + pageletSection + "\",\"pager_fired_on_init\":true}";

                    var stringToRemove = FdRegexUtility.FirstMatchExtractor(PageletData, "cursor\":\"(.*?)\",\"");

                    PageletData = Regex.Replace(PageletData, stringToRemove
                        , cursor);

                    var ajaxTokenSection = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.AjaxPipeTokenRegex);

                    ObjFdScraperResponseParameters.AjaxToken = ajaxTokenSection;
                }
                else
                {
                    pageletSection = Regex.Matches(decodedResponse, "ProfileTimelineJumperStoriesPagelet\",{(.*?)\"},", RegexOptions.Singleline)[0].Groups[1].ToString();
                    PageletData = ("{" + pageletSection + "\"}");
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
            var postId = string.Empty;

            var postDetails = string.Empty;

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
                        postDetails = objHtmlDocument.DocumentNode.SelectNodes("(//input[@name=\"ft_ent_identifier\"])")[0].OuterHtml;

                        postId = Regex.Matches(postDetails, "value=\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

                        objFacebookPostDetails.Id = postId;
                        //for sending friend Request
                        objFacebookPostDetails.OwnerId = Utilities.GetBetween(node, "user.php?id=", "&");

                        string postReactionDetails = ObjFdScraperResponseParameters.ListPostReaction.FirstOrDefault(x => x.Key == postId).Value;

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

                        string time = string.Empty;

                        var timeDetails = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_5pcp _5lel _2jyu _232_\"])")[0].InnerHtml;

                        objHtmlDocument.LoadHtml(timeDetails);

                        time = objHtmlDocument.DocumentNode.SelectNodes("//abbr")[0].OuterHtml;

                        time = Regex.Matches(time, "title=\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

                        try
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

                            DateTime dt = new DateTime();

                            DateTime.TryParseExact(time, "MM/dd/yyyy hh:mm tt",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);

                            objFacebookPostDetails.PostedDateTime = dt;
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                            objFacebookPostDetails.PostedDateTime = DateTime.Now;
                        }

                        objFacebookPostDetails.PostUrl = FdConstants.FbHomeUrl + objFacebookPostDetails.Id;

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
