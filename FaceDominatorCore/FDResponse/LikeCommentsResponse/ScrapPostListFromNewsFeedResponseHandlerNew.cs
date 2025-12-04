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

namespace FaceDominatorCore.FDResponse.LikeCommentsResponse
{
    public class ScrapPostListFromNewsFeedResponseHandlerNew : FdResponseHandler, IResponseHandler
    {

        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public ScrapPostListFromNewsFeedResponseHandlerNew
            (IResponseParameter responseParameter, FdScraperResponseParameters objPajination
            , List<KeyValuePair<string, string>> listPostReaction, IHttpHelper httpHelper)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            ObjFdScraperResponseParameters = objPajination ?? new FdScraperResponseParameters();

            ObjFdScraperResponseParameters.ListPostDetails = new List<FacebookPostDetails>();

            ObjFdScraperResponseParameters.ListPostReaction = listPostReaction;

            string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            GetPostIdList(decodedResponse, httpHelper);

            GetPagelet(decodedResponse);

        }


        private void GetPagelet(string decodedResponse)
        {
            try
            {
                ObjFdScraperResponseParameters.ScrollCount++;

                string cursor;
                string pageletSection;
                if (string.IsNullOrEmpty(ObjFdScraperResponseParameters.AjaxToken))
                {
                    cursor = Regex.Matches(decodedResponse, " data-cursor=\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

                    var sectionId = Regex.Matches(decodedResponse, "section_id\":\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();

                    pageletSection = $"{{\"client_stories_count\":{ObjFdScraperResponseParameters.ClientStoriesCount},\"cursor\":\"{cursor}\",\"feed_stream_id\":80846080,\"pager_config\":\"{{\\\"edge\\\":null,\\\"source_id\\\":null,\\\"section_id\\\":\\\"{sectionId}\\\",\\\"pause_at\\\":null,\\\"stream_id\\\":null,\\\"section_type\\\":1,\\\"sizes\\\":null,\\\"most_recent\\\":false,\\\"ranking_model\\\":null,\\\"query_context\\\":[]}}\",\"scroll_count\":{ObjFdScraperResponseParameters.ScrollCount},\"story_id\":null}}";

                    PageletData = pageletSection;

                    var ajaxTokenSection = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.AjaxPipeTokenRegex);

                    ObjFdScraperResponseParameters.AjaxToken = ajaxTokenSection;

                    ObjFdScraperResponseParameters.SectionId = Double.Parse(sectionId);
                }
                else
                {
                    MatchCollection objCursor = Regex.Matches(decodedResponse, " data-cursor=\"(.*?)\"", RegexOptions.Singleline);

                    cursor = objCursor[objCursor.Count - 1].Groups[1].ToString();

                    pageletSection = $"{{\"client_stories_count\":{ObjFdScraperResponseParameters.ClientStoriesCount},\"cursor\":\"{cursor}\",\"pager_config\":\"{{\\\"section_id\\\":\\\"{ObjFdScraperResponseParameters.SectionId}\\\",\\\"stream_id\\\":null,\\\"section_type\\\":1,\\\"most_recent\\\":false,\\\"ranking_model\\\":null,\\\"query_context\\\":[],\\\"use_new_feed\\\":false}}\",\"scroll_count\":{ObjFdScraperResponseParameters.ScrollCount},\"story_id\":null}}";


                    //pageletSection = $"{{\"client_stories_count\":{ObjFdScraperResponseParameters.ClientStoriesCount},\"cursor\":\"{cursor}\",\"feed_stream_id\":80846080,\"pager_config\":\"{{\\\"edge\\\":null,\\\"source_id\\\":null,\\\"section_id\\\":\\\"{ObjFdScraperResponseParameters.SectionId}\\\",\\\"pause_at\\\":null,\\\"stream_id\\\":null,\\\"section_type\\\":1,\\\"sizes\\\":null,\\\"most_recent\\\":false,\\\"ranking_model\\\":null,\\\"query_context\\\":[]}}\",\"scroll_count\":{ObjFdScraperResponseParameters.ScrollCount},\"story_id\":null}}";
                    PageletData = pageletSection;
                }

                HasMoreResults = !string.IsNullOrEmpty(PageletData);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void GetPostIdList(string decodedResponse, IHttpHelper _httpHelper)
        {
            FdFunctions objFdFunctions = new FdFunctions();

            HtmlDocument objHtmlDocument = new HtmlDocument();

            objHtmlDocument.LoadHtml(decodedResponse);

            try
            {

                HtmlNodeCollection objNodecollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@data-testid=\"fbfeed_story\"])");

                List<string> objNodeList = objFdFunctions.GetOuterHtmlListFromNodeCollection(objNodecollection);

                if (objNodeList != null)
                {

                    ObjFdScraperResponseParameters.ClientStoriesCount += objNodeList.Count;

                    foreach (string node in objNodeList)
                    {
                        FacebookPostDetails objFacebookPostDetails = new FacebookPostDetails();

                        try
                        {
                            objHtmlDocument.LoadHtml(node);

                            objFacebookPostDetails.Id = FdRegexUtility.FirstMatchExtractor(node, FdConstants.TopLevelPostIdRegex);
                            //send Friend Request 
                            objFacebookPostDetails.OwnerId = FdRegexUtility.FirstMatchExtractor(node, "\"content_owner_id_new\":\"(.*?)\"");

                            if (string.IsNullOrEmpty(objFacebookPostDetails.Id))
                            {
                                string id = FdRegexUtility.FirstMatchExtractor(node, "photo_id\":\"(.*?)\"");
                                var postResponse = _httpHelper.GetRequest($"{FdConstants.FbHomeUrl}{id}");
                                objFacebookPostDetails.Id = FdRegexUtility.FirstMatchExtractor(FdFunctions.GetDecodedResponse(postResponse.Response), FdConstants.TopLevelPostIdRegex);

                            }

                            //objFacebookPostDetails.Id = postId;

                            string postReactionDetails = ObjFdScraperResponseParameters.ListPostReaction.FirstOrDefault(x => x.Key == objFacebookPostDetails.Id).Value;

                            if (!string.IsNullOrEmpty(postReactionDetails))
                            {
                                var reactionArray = Regex.Split(postReactionDetails, "<:>").ToArray();

                                if (reactionArray.Length > 2)
                                {
                                    objFacebookPostDetails.CommentorCount = reactionArray[0];
                                    objFacebookPostDetails.LikersCount = reactionArray[1];
                                    objFacebookPostDetails.SharerCount = reactionArray[2];
                                }

                                try
                                {
                                    var verifytimeDetails = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_5pcp _5lel _2jyu _232_\"])");
                                    if (verifytimeDetails != null)
                                    {
                                        var timeDetails = objHtmlDocument.DocumentNode.SelectNodes("(//div[@class=\"_5pcp _5lel _2jyu _232_\"])")[0].InnerHtml;

                                        objHtmlDocument.LoadHtml(timeDetails);

                                        var verifyTime = objHtmlDocument.DocumentNode.SelectNodes("//abbr");

                                        if (verifyTime != null)
                                        {
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
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                    objFacebookPostDetails.PostedDateTime = DateTime.Now;
                                }

                                objFacebookPostDetails.PostUrl = FdConstants.FbHomeUrl + objFacebookPostDetails.Id;


                                if (!ObjFdScraperResponseParameters.ListPostDetails.Contains(objFacebookPostDetails))
                                    ObjFdScraperResponseParameters.ListPostDetails.Add(objFacebookPostDetails);
                            }

                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
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
