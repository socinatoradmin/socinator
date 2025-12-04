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
    public class ScrapPostListFromNewsFeedResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public List<FacebookPostDetails> ListFacebookPostDetails { get; set; }

        public NewsFeedPagination ObjPajination { get; set; }

        public ScrapPostListFromNewsFeedResponseHandler
            (IResponseParameter responseParameter, NewsFeedPagination objPajination, List<KeyValuePair<string, string>> listPostReaction)
            : base(responseParameter)
        {

            ObjPajination = objPajination ?? new NewsFeedPagination();

            ListFacebookPostDetails = new List<FacebookPostDetails>();

            ObjFdScraperResponseParameters.ListPostReaction = listPostReaction;

            string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            GetPostIdList(decodedResponse);

            GetPagelet(decodedResponse);

        }


        private void GetPagelet(string decodedResponse)
        {
            try
            {
                ObjPajination.ScrollCount++;

                string cursor;
                string pageletSection;
                if (string.IsNullOrEmpty(ObjPajination.AjaxToken))
                {
                    cursor = Regex.Matches(decodedResponse, " data-cursor=\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

                    var sectionId = Regex.Matches(decodedResponse, "section_id\":\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

                    pageletSection = $"{{\"client_stories_count\":{ObjPajination.ClientStoriesCount},\"cursor\":\"{cursor}\",\"feed_stream_id\":80846080,\"pager_config\":\"{{\\\"edge\\\":null,\\\"source_id\\\":null,\\\"section_id\\\":\\\"{sectionId}\\\",\\\"pause_at\\\":null,\\\"stream_id\\\":null,\\\"section_type\\\":1,\\\"sizes\\\":null,\\\"most_recent\\\":false,\\\"ranking_model\\\":null,\\\"query_context\\\":[]}}\",\"scroll_count\":{ObjPajination.ScrollCount},\"story_id\":null}}";

                    ObjPajination.Pagelet = pageletSection;

                    var ajaxTokenSection = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.AjaxPipeTokenRegex);

                    ObjPajination.AjaxToken = ajaxTokenSection;

                    ObjPajination.SectionId = Double.Parse(sectionId);
                }
                else
                {
                    MatchCollection objCursor = Regex.Matches(decodedResponse, " data-cursor=\"(.*?)\"", RegexOptions.Singleline);

                    cursor = objCursor[objCursor.Count - 1].Groups[1].ToString();

                    pageletSection = $"{{\"client_stories_count\":{ObjPajination.ClientStoriesCount},\"cursor\":\"{cursor}\",\"feed_stream_id\":80846080,\"pager_config\":\"{{\\\"edge\\\":null,\\\"source_id\\\":null,\\\"section_id\\\":\\\"{ObjPajination.SectionId}\\\",\\\"pause_at\\\":null,\\\"stream_id\\\":null,\\\"section_type\\\":1,\\\"sizes\\\":null,\\\"most_recent\\\":false,\\\"ranking_model\\\":null,\\\"query_context\\\":[]}}\",\"scroll_count\":{ObjPajination.ScrollCount},\"story_id\":null}}";
                    ObjPajination.Pagelet = pageletSection;
                }

                HasMoreResults = !string.IsNullOrEmpty(ObjPajination.Pagelet);
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

                HtmlNodeCollection objNodecollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@data-testid=\"fbfeed_story\"])");

                List<string> objNodeList = objFdFunctions.GetOuterHtmlListFromNodeCollection(objNodecollection);

                if (objNodeList != null)
                {
                    ObjPajination.ClientStoriesCount += objNodeList.Count;

                    foreach (string node in objNodeList)
                    {
                        FacebookPostDetails objFacebookPostDetails = new FacebookPostDetails();

                        try
                        {
                            objHtmlDocument.LoadHtml(node);

                            postId = FdRegexUtility.FirstMatchExtractor(node, FdConstants.TopLevelPostIdRegex);

                            objFacebookPostDetails.Id = postId;

                            string postReactionDetails = ObjFdScraperResponseParameters.ListPostReaction.FirstOrDefault(x => x.Key == postId).Value;

                            var reactionArray = Regex.Split(postReactionDetails, "<:>").ToArray();

                            if (reactionArray.Length > 2)
                            {
                                objFacebookPostDetails.CommentorCount = reactionArray[0];
                                objFacebookPostDetails.LikersCount = reactionArray[1];
                                objFacebookPostDetails.SharerCount = reactionArray[2];
                            }

                            try
                            {
                                var timeDetails = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_5pcp _5lel _2jyu _232_\"])")[0].InnerHtml;

                                objHtmlDocument.LoadHtml(timeDetails);

                                var time = objHtmlDocument.DocumentNode.SelectNodes("//abbr")[0].OuterHtml;

                                time = Regex.Matches(time, "title=\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

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
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                                objFacebookPostDetails.PostedDateTime = DateTime.Now;
                            }


                            if (!ListFacebookPostDetails.Contains(objFacebookPostDetails))
                                ListFacebookPostDetails.Add(objFacebookPostDetails);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }

                }

                Status = ListFacebookPostDetails.Count > 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
