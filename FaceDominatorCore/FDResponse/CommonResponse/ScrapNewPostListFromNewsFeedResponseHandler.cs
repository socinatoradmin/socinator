
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
    public class ScrapNewPostListFromNewsFeedResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public List<FacebookAdsDetails> ListFacebookAdsDetails { get; set; }

        public NewsFeedPagination ObjPajination { get; set; }

        public ScrapNewPostListFromNewsFeedResponseHandler
            (IResponseParameter responseParameter, NewsFeedPagination objPajination, List<KeyValuePair<string, string>> listPostReaction)
            : base(responseParameter)
        {

            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            ObjPajination = objPajination ?? new NewsFeedPagination();

            ListFacebookAdsDetails = new List<FacebookAdsDetails>();

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

                string cursor = string.Empty;
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

                    if (objCursor.Count != 0)
                        cursor = objCursor[objCursor.Count - 1].Groups[1].ToString();

                    pageletSection = $"{{\"client_stories_count\":{ObjPajination.ClientStoriesCount},\"cursor\":\"{cursor}\",\"feed_stream_id\":80846080,\"pager_config\":\"{{\\\"edge\\\":null,\\\"source_id\\\":null,\\\"section_id\\\":\\\"{ObjPajination.SectionId}\\\",\\\"pause_at\\\":null,\\\"stream_id\\\":null,\\\"section_type\\\":1,\\\"sizes\\\":null,\\\"most_recent\\\":false,\\\"ranking_model\\\":null,\\\"query_context\\\":[]}}\",\"scroll_count\":{ObjPajination.ScrollCount},\"story_id\":null}}";
                    ObjPajination.Pagelet = pageletSection;
                }

                HasMoreResults = !string.IsNullOrEmpty(cursor);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
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

                HtmlNodeCollection objNodecollection = objHtmlDocument.DocumentNode.SelectNodes("(//div[@data-testid=\"fbfeed_story\"])");

                List<string> objNodeList = objFdFunctions.GetOuterHtmlListFromNodeCollection(objNodecollection);

                if (objNodeList != null)
                {
                    ObjPajination.ClientStoriesCount += objNodeList.Count;

                    foreach (string node in objNodeList)
                    {
                        FacebookAdsDetails objFacebookAdsDetails = new FacebookAdsDetails();

                        try
                        {
                            objHtmlDocument.LoadHtml(node);

                            var postId = FdRegexUtility.FirstMatchExtractor(node, FdConstants.TopLevelPostIdRegex);

                            objFacebookAdsDetails.Id = postId;

                            string postReactionDetails = ObjFdScraperResponseParameters.ListPostReaction.FirstOrDefault(x => x.Key == postId).Value;

                            if (!string.IsNullOrEmpty(postReactionDetails))
                            {
                                var reactionArray = Regex.Split(postReactionDetails, "<:>").ToArray();

                                if (reactionArray.Length > 2)
                                {
                                    objFacebookAdsDetails.CommentorCount = reactionArray[0];
                                    objFacebookAdsDetails.LikersCount = reactionArray[1];
                                    objFacebookAdsDetails.SharerCount = reactionArray[2];

                                    if (reactionArray.Length > 3)
                                        objFacebookAdsDetails.AdId = reactionArray[3];
                                }


                                if (!ListFacebookAdsDetails.Contains(objFacebookAdsDetails))
                                    ListFacebookAdsDetails.Add(objFacebookAdsDetails);
                            }
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    }

                    Status = ListFacebookAdsDetails.Count > 0;

                }

            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}


