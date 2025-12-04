using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using TwtDominatorCore.Helper;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response
{
    public class SingleTweetDetailsResponseHandler : TdBaseHtmlResponseHandler
    {
        public List<string> ListImagePath = new List<string>();
        public List<TagDetails> singleTweetsDetails = new List<TagDetails>();
        public TagDetails TweetDetails;

        public SingleTweetDetailsResponseHandler(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;
            var Response = response.Response;
            var tweetContainerHtmlDoc = new HtmlDocument();
            if (Response.Contains("{\"globalObjects\":{\"tweets\":{\""))
                BrowserResponseHandler(Response);
            else if (Response.Contains("{\"data\":{\"threaded_conversation_with_injections_v2"))
                SingleTweetResponseHandler(Response);
            else
                HttpResponsehandler(Response, tweetContainerHtmlDoc);
        }

        private void SingleTweetResponseHandler(string response)
        {
            try
            {
                var Captiondeatails = string.Empty;
                var TwtTimeStamp = 0;
                var tweetId = Utilities.GetBetween(response, "\"", "\"");
                var tweetIdLength = tweetId.Length + 2;
                var jsonResponse = response.Substring(tweetIdLength);
                var Jsonhandler = new Jsonhandler(jsonResponse);
                var tweets = Jsonhandler.GetJToken("data",
                                        "threaded_conversation_with_injections_v2",
                                        "instructions", 0,
                                        "entries").ToList();
                //.SelectToken("content.itemContent.tweet_results","itemContent","tweet_results","result","rest_id").Equals(tweetId)
                var tweet = tweets.FirstOrDefault(x => x.ToString().Contains($"tweet-{tweetId}"));
                var tweetDetail = Jsonhandler.GetJTokenOfJToken(tweet,
                                        "content",
                                        "itemContent",
                                        "tweet_results",
                                        "result");
                if(tweetDetail != null && tweetDetail.HasValues && tweetDetail.ToString().Contains("\"TweetWithVisibilityResults\""))
                    tweetDetail = Jsonhandler.GetJTokenOfJToken(tweet,
                                        "content",
                                        "itemContent",
                                        "tweet_results",
                                        "result", "tweet");
                if(tweetDetail is null || !tweetDetail.HasValues)
                {
                    var nodes = Jsonhandler.GetJToken("data",
                                        "threaded_conversation_with_injections_v2",
                                        "instructions");
                    if(nodes != null && nodes.HasValues)
                    {
                        foreach(var node in nodes)
                        {
                            var type = Jsonhandler.GetJTokenValue(node, "type");
                            if (type == "TimelineAddEntries")
                            {
                                var token = Jsonhandler.GetJTokenOfJToken(node, "entries");
                                tweet = token.FirstOrDefault(x => x.ToString().Contains($"tweet-{tweetId}"));
                                tweetDetail = Jsonhandler.GetJTokenOfJToken(tweet,
                                                        "content",
                                                        "itemContent",
                                                        "tweet_results",
                                                        "result");
                                break;
                            }
                        }
                    }
                }
                var tweetid = Jsonhandler.GetJTokenValue(tweetDetail, "rest_id");
                Captiondeatails = Jsonhandler.GetJTokenValue(tweetDetail, "legacy", "full_text");
                Captiondeatails = Regex.Replace(Captiondeatails, @"https://t.co[^\s]+$", "");
                if (tweetDetail.ToString().Contains("quoted_status_permalink"))
                {
                    var quotedLink = Jsonhandler.GetJTokenValue(tweetDetail, "legacy", "quoted_status_permalink", "url");
                    Captiondeatails += $" {quotedLink}";
                }

                if (tweetid == tweetId)
                {
                    var userId = Jsonhandler.GetJTokenValue(tweetDetail, "legacy", "user_id_str");
                    var listImage = Jsonhandler.GetJTokenOfJToken(tweetDetail, "legacy", "extended_entities", "media");
                    foreach (var imagelist in listImage)
                    {
                        var images = Jsonhandler.GetJTokenValue(imagelist, "media_url_https");
                        
                        
                        ListImagePath.Add(images);
                    }
                    var haveVideos = Jsonhandler.GetJTokenOfJToken(tweetDetail, "legacy", "extended_entities", "media",0, "video_info");
                    if (haveVideos != null && haveVideos.HasValues)
                    {
                        ListImagePath.Clear();
                        foreach (var videosUrl in listImage)
                        {
                            var getType = Jsonhandler.GetJTokenOfJToken(videosUrl, "type");
                            if (getType.ToString().Contains("video"))
                            {
                                var ContentType = Jsonhandler.GetJTokenOfJToken(videosUrl, "video_info", "variants");
                                foreach (var videos in ContentType)
                                {
                                    var contentstype = Jsonhandler.GetJTokenValue(videos, "bitrate");
                                    if (!string.IsNullOrEmpty(contentstype))
                                    {
                                        ListImagePath.Add(Jsonhandler.GetJTokenValue(videos, "url"));
                                        break;
                                    }
                                }
                            }
                            if (getType.ToString().Contains("photo"))
                            {
                                ListImagePath.Add(Jsonhandler.GetJTokenValue(videosUrl, "media_url_https"));
                            }

                        }

                        // ListImagePath.Add(Jsonhandler.GetJTokenValue(singletweet.First, "extended_entities", "media", 0, "video_info", "variants", 1, "url"));
                    }
                    var dt = TdTimeStampUtility.ConvertTimestamp(Jsonhandler,
                                            Jsonhandler.GetJTokenOfJToken(tweetDetail, "legacy"));
                    TwtTimeStamp = dt.ConvertToEpoch();

                    var userid = Jsonhandler.GetJTokenValue(tweetDetail,
                                                    "core",
                                                    "user_results",
                                                    "result",
                                                    "rest_id");
                    var usersdetail = Jsonhandler.GetJTokenOfJToken(tweetDetail,
                                                    "core",
                                                    "user_results",
                                                    "result",
                                                    "legacy");
                    if(userid == userId)
                    {
                        haveVideos = Jsonhandler.GetJTokenOfJToken(tweetDetail, "legacy", "extended_entities", "media", 0, "video_info");
                        TweetDetails = new TagDetails
                        {
                            UserId = userId,
                            Username = Jsonhandler.GetJTokenValue(usersdetail, "screen_name"),
                            Id = tweetid,
                            DateTime = dt,
                            Caption = Captiondeatails,
                            TweetedTimeStamp = TwtTimeStamp,
                            Code = string.Join("\n", ListImagePath),
                            IsAlreadyLiked = Jsonhandler.GetJTokenValue(tweetDetail,
                                                    "legacy", 
                                                    "favorited") == "True",
                            IsAlreadyRetweeted = Jsonhandler.GetJTokenValue(tweetDetail, 
                                                        "legacy",
                                                        "retweeted") == "True",
                            CommentCount = int.Parse(Jsonhandler.GetJTokenValue(tweetDetail,"legacy", "reply_count")),
                            RetweetCount = int.Parse(Jsonhandler.GetJTokenValue(tweetDetail, "legacy","retweet_count")),
                            LikeCount = int.Parse(Jsonhandler.GetJTokenValue(tweetDetail,"legacy", "favorite_count")),
                            FollowStatus = Jsonhandler.GetJTokenValue(usersdetail, "following") == "True",
                            FollowBackStatus = Jsonhandler.GetJTokenValue(usersdetail, "followed_by") == "True",
                            IsTweetContainedVideo = haveVideos != null && haveVideos.HasValues
                        };
                        singleTweetsDetails.Add(TweetDetails);
                    }

                }
            }
            catch(Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
        }

        private void BrowserResponseHandler(string Response)
        {
            try
            {
                var Captiondeatails = string.Empty;
                var TwtTimeStamp = 0;
                var tweetId = Utilities.GetBetween(Response, "\"", "\"");
                var tweetIdLength = tweetId.Length + 2;
                var jsonResponse = Response.Substring(tweetIdLength);
                var Jsonhandler = new Jsonhandler(jsonResponse);
                var tweets = Jsonhandler.GetJToken("globalObjects", "tweets");
                var users = Jsonhandler.GetJToken("globalObjects", "users");

                foreach (var singletweet in tweets)
                {
                    var TweetDetail = singletweet.First;
                    var tweetid = Jsonhandler.GetJTokenValue(TweetDetail, "id_str");
                    Captiondeatails = Jsonhandler.GetJTokenValue(TweetDetail, "full_text");
                    if (tweetid == tweetId)
                    {
                        var UserId = Jsonhandler.GetJTokenValue(TweetDetail, "user_id_str");
                        var ListImage = Jsonhandler.GetJTokenOfJToken(TweetDetail, "extended_entities", "media");
                        var expanded = Jsonhandler.GetJTokenValue(TweetDetail, "quoted_status_permalink", "expanded");
                        if (!string.IsNullOrEmpty(expanded))
                            Captiondeatails += expanded;
                        foreach (var imagelist in ListImage)
                        {
                            var images = Jsonhandler.GetJTokenValue(imagelist, "media_url_https");
                            var url = Jsonhandler.GetJTokenValue(imagelist, "url");
                            if (Captiondeatails.Contains(url))
                                Captiondeatails = Captiondeatails.Replace(url, "");
                            ListImagePath.Add(images);
                        }

                        if (TweetDetail["extended_entities"] != null
                            ? TweetDetail["extended_entities"]["media"][0]["video_info"] != null
                            : false)
                        {
                            ListImagePath.Clear();
                            var ContentType = Jsonhandler.GetJTokenOfJToken(singletweet.First, "extended_entities",
                                "media", 0, "video_info", "variants");
                            foreach (var videos in ContentType)
                            {
                                var contentstype = Jsonhandler.GetJTokenValue(videos, "bitrate");
                                if (!string.IsNullOrEmpty(contentstype))
                                {
                                    ListImagePath.Add(Jsonhandler.GetJTokenValue(videos, "url"));
                                    break;
                                }
                            }

                            // ListImagePath.Add(Jsonhandler.GetJTokenValue(singletweet.First, "extended_entities", "media", 0, "video_info", "variants", 1, "url"));
                        }

                        var dt = TdTimeStampUtility.ConvertTimestamp(Jsonhandler, TweetDetail);
                        TwtTimeStamp = dt.ConvertToEpoch();

                        foreach (var user in users)
                        {
                            var usersdetail = user.First;
                            var userId = Jsonhandler.GetJTokenValue(usersdetail, "id_str");
                            if (UserId == userId)
                            {
                                TweetDetails = new TagDetails
                                {
                                    UserId = userId,
                                    Username = Jsonhandler.GetJTokenValue(usersdetail, "screen_name"),
                                    Id = tweetid,
                                    DateTime = dt,
                                    Caption = Captiondeatails,
                                    TweetedTimeStamp = TwtTimeStamp,
                                    Code = string.Join("\n", ListImagePath),
                                    IsAlreadyLiked = Jsonhandler.GetJTokenValue(TweetDetail, "favorited") == "True",
                                    IsAlreadyRetweeted = Jsonhandler.GetJTokenValue(TweetDetail, "retweeted") == "True",
                                    CommentCount = int.Parse(Jsonhandler.GetJTokenValue(TweetDetail, "reply_count")),
                                    RetweetCount = int.Parse(Jsonhandler.GetJTokenValue(TweetDetail, "retweet_count")),
                                    LikeCount = int.Parse(Jsonhandler.GetJTokenValue(TweetDetail, "favorite_count")),
                                    FollowStatus = Jsonhandler.GetJTokenValue(usersdetail, "following") == "True",
                                    FollowBackStatus = Jsonhandler.GetJTokenValue(usersdetail, "followed_by") == "True",
                                    IsTweetContainedVideo = TweetDetail["extended_entities"] != null
                                        ? TweetDetail["extended_entities"]["media"][0]["video_info"] != null
                                        : false
                                };
                                singleTweetsDetails.Add(TweetDetails);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void HttpResponsehandler(string Response, HtmlDocument tweetContainerHtmlDoc)
        {
            try
            {
                var Emojidict = TdUtility.GetAllEmojis();

                var TweetContainer =
                    HtmlAgilityHelper.getStringInnerHtmlFromClassName(Response,
                        "permalink-inner permalink-tweet-container");
                tweetContainerHtmlDoc.LoadHtml(TweetContainer);
                var UserId =
                    HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(TweetContainer, "follow-bar",
                        "data-user-id", tweetContainerHtmlDoc);
                var UserName =
                    HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(TweetContainer, "follow-bar",
                        "data-screen-name", tweetContainerHtmlDoc);
                var TwtTimeStamp = 0;
                int.TryParse(
                    HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(TweetContainer, "tweet-timestamp",
                        "data-time", tweetContainerHtmlDoc), out TwtTimeStamp);
                var ListImagePath =
                    HtmlAgilityHelper.getListValueWithAttributeNameFromInnerHtml(TweetContainer,
                        "AdaptiveMedia-photoContainer", "src", tweetContainerHtmlDoc);
                var CommentCount = HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(TweetContainer,
                    "ProfileTweet-action--reply", "data-tweet-stat-count", tweetContainerHtmlDoc);
                var RetweetCount = HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(TweetContainer,
                    "ProfileTweet-action--retweet", "data-tweet-stat-count", tweetContainerHtmlDoc);
                var LikeCount = HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(TweetContainer,
                    "ProfileTweet-action--favorite", "data-tweet-stat-count", tweetContainerHtmlDoc);

                /// Getting tweet text with emoji
                var TwtText =
                    TweetHelper.GetEmoji(
                        TweetContainer); // TDUtility.HtmlAgilityHelper.getStringInnerHtmlFromClassName(TweetContainer, "js-tweet-text-container");

                if (string.IsNullOrEmpty(TwtText))
                    TwtText = TdUtility.ReplaceImageWithEmogis(
                        HtmlAgilityHelper.getStringInnerHtmlFromClassName(TweetContainer, "js-tweet-text-container",
                            tweetContainerHtmlDoc),
                        Emojidict);

                if (TwtText.Contains($"pic.{Domain}/"))
                    TwtText = Regex.Split(TwtText, $"pic.{Domain}/")[0];

                TwtText = HttpUtility.HtmlDecode(TwtText).Trim();

                var IsLiked = TweetHelper.IsLikedPost(TweetContainer);


                TweetDetails = new TagDetails
                {
                    UserId = string.IsNullOrEmpty(UserId)
                        ? HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(TweetContainer, "permalink-header",
                            "data-user-id", tweetContainerHtmlDoc)
                        : UserId,
                    Username = string.IsNullOrEmpty(UserName)
                        ? HtmlAgilityHelper
                            .getStringInnerTextFromClassName(TweetContainer, "username u-dir u-textTruncate",
                                tweetContainerHtmlDoc)
                            .Replace("@", "")
                        : UserName,
                    Id = Utilities.GetBetween(TweetContainer, "data-tweet-id=\"", "\""),
                    Caption = TwtText,
                    TweetedTimeStamp = TwtTimeStamp,
                    Code = string.Join("\n", ListImagePath),
                    IsAlreadyLiked = IsLiked,
                    IsAlreadyRetweeted = TweetContainer.Contains("data-my-retweet-id"),
                    IsRetweet = TweetContainer.Contains("Icon--retweeted"),
                    CommentCount = string.IsNullOrEmpty(CommentCount) ? 0 : int.Parse(CommentCount),
                    RetweetCount = string.IsNullOrEmpty(RetweetCount) ? 0 : int.Parse(RetweetCount),
                    LikeCount = string.IsNullOrEmpty(LikeCount) ? 0 : int.Parse(LikeCount),
                    FollowStatus = TweetContainer.Contains("data-you-follow=\"true\""),
                    FollowBackStatus = TweetContainer.Contains("data-follows-you=\"true\""),
                    IsTweetContainedVideo = TweetContainer.Contains("AdaptiveMedia-videoContainer")
                };
                singleTweetsDetails.Add(TweetDetails);
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }
    }
}