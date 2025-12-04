using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class CommentScraperResponse : ResponseHandler
    {
        public TumblrPost post = new TumblrPost() { ListComments = new List<TumblrComments>() };
        public CommentScraperResponse(IResponseParameter responeParameter, bool IsBrowser = false) : base(responeParameter)
        {
            try
            {
                var searchBlogsRowPageSource = Response?.Response;
                if (IsBrowser || responeParameter.Response.Contains("{\"meta\":{\"status\":200,\"msg\":\"OK\"}"))
                    Success = true;
                if (Response?.Response is null ? false : !searchBlogsRowPageSource.IsValidJson())
                {
                    searchBlogsRowPageSource = Regex.Replace(Response?.Response, "}}csrf :(.*)?", "") + "}}";
                }
                var jObject = parser.ParseJsonToJObject(searchBlogsRowPageSource);
                var Posts = parser.GetJArrayElement(parser.GetJTokenValue(jObject, "response", "notes"));
                if (Posts != null && Posts.HasValues)
                {
                    foreach (var eachUser in Posts)
                        try
                        {
                            var tumblrComment = new TumblrComments() { commenter = new TumblrUser() };
                            tumblrComment.Type = eachUser["type"]?.ToString();
                            var addedText = eachUser["added_text"]?.ToString();
                            tumblrComment.AddedText = !String.IsNullOrEmpty(addedText) ? addedText : "N/A";
                            var replyText = eachUser["reply_text"]?.ToString();
                            tumblrComment.CommentText = !String.IsNullOrEmpty(replyText) ? replyText : "N/A";
                            tumblrComment.commenter.Username = eachUser["blog_name"]?.ToString();
                            tumblrComment.CommentId = eachUser["blog_uuid"]?.ToString();
                            tumblrComment.CommenterID = eachUser["post_id"]?.ToString();
                            tumblrComment.commenter.PageUrl = ConstantHelpDetails.BlogViewUrl(tumblrComment.commenter.Username);
                            var timeStamp = 0.0;
                            if (double.TryParse(eachUser["timestamp"]?.ToString(), out timeStamp))
                                tumblrComment.CommentTimeWithDate = DateTimeUtilities.EpochToDateTimeUtc(timeStamp).ToString();
                            post.ListComments.Add(tumblrComment);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                }
            }
            catch (Exception e)
            {
                e.DebugLog();
            }
        }
        public CommentScraperResponse(IResponseParameter responeParameter, List<string> jsonResponseList) : base(responeParameter)
        {
            foreach (var response in jsonResponseList)
            {
                JObject jObject = null;
                var elements = new JArray();
                if (!response.IsValidJson())
                {
                    var decodedResponse = TumblrUtility.GetDecodedResponseOfJson(response);
                    jObject = parser.ParseJsonToJObject(decodedResponse);
                }
                else
                    jObject = parser.ParseJsonToJObject(response);

                elements = parser.GetJArrayElement(parser.GetJTokenValue(jObject, "response", "timeline", "elements"));
                foreach (var reply in elements)
                {
                    try
                    {
                        var tumblrComment = new TumblrComments() { commenter = new TumblrUser() };
                        tumblrComment.Type = parser.GetJTokenValue(reply, "type");
                        var addedText = parser.GetJTokenValue(reply, "content", 0, "text");
                        tumblrComment.AddedText = !String.IsNullOrEmpty(addedText) && tumblrComment.Type == "reblog" ? addedText : "N/A";
                        tumblrComment.CommentText = !String.IsNullOrEmpty(addedText) && tumblrComment.Type == "reply" ? addedText : "N/A";
                        tumblrComment.commenter.Username = parser.GetJTokenValue(reply, "blog", "name");
                        if (string.IsNullOrEmpty(tumblrComment.commenter.Username))
                            tumblrComment.commenter.Username = parser.GetJTokenValue(reply, "blogName");
                        tumblrComment.commenter.PageUrl = ConstantHelpDetails.BlogViewUrl(tumblrComment.commenter.Username);
                        tumblrComment.CommentId = tumblrComment.Type == "like" ? parser.GetJTokenValue(reply, "blogUuid")
                            : parser.GetJTokenValue(reply, "id").Split(':').LastOrDefault();
                        var timeStamp = 0.0;
                        if (double.TryParse(parser.GetJTokenValue(reply, "timestamp"), out timeStamp))
                            tumblrComment.CommentTimeWithDate = DateTimeUtilities.EpochToDateTimeUtc(timeStamp).ToString();
                        post.ListComments.Add(tumblrComment);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                }
            }
        }

    }
}