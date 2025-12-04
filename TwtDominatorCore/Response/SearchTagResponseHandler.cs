using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.AccountSetting.Activity;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using TwtDominatorCore.Helper;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response
{
    public class SearchTagResponseHandler : TdResponseHandler
    {
        public bool following, followBack, Ismute, Isverfied, Isprotected, IsLiked, IsRetweeted;
        public List<string> ListImagePath = new List<string>();
        public List<TagDetails> ListTagDetails = new List<TagDetails>();
        public bool NoResultStatus;



        public SearchTagResponseHandler(IResponseParameter Response,bool UserSearch=false) : base(Response)
        {
            try
            {
                if (!string.IsNullOrEmpty(Response.Response) )
                    Success = true;
                if (Response.Response.Contains("Rate limit exceeded"))
                    Success = false;
                if (!Success)
                    return;
                if (Response.Response.Contains("SearchEmptyTimeline-emptyTitle") ||
                    Response.Response.Equals("{\"message\":\"This user does not exist.\"}"))
                {
                    if (Response.Response.Equals("{\"message\":\"This user does not exist.\"}"))
                        Issue = new TwitterIssue { Message = "user does not exist" };
                    NoResultStatus = true;
                    return;
                }
                var response = Response.Response;
                MinPosition = GetCursor(response, UserSearch);
                MinPosition = string.IsNullOrEmpty(MinPosition) ? Utilities.GetBetween(response, "\"__typename\":\"TimelineTimelineCursor\",\"value\":\"", "\"") : MinPosition;
                if (response.Contains("{\"globalObjects\":{\"tweets\":{\"") || response.StartsWith("{\"data\":{\"user\":"))
                    BrowserResponsehandler(response);
                else if (response.Contains("{\"data\":{\"search_by_raw_query\":"))
                    TimelineResponseHandler(response, UserSearch);
                else
                    HttpResponseHandler(response);

                if (!string.IsNullOrEmpty(Response.Response))
                    Success = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private string GetCursor(string response,bool UserSearch=false)
        {
            try
            {
                var obj = handler.ParseJsonToJObject(response);
                var objArray = handler.GetJArrayElement(handler.GetJTokenValue(obj, "data", "search_by_raw_query", "search_timeline", "timeline", "instructions",(UserSearch ? 1:0), "entries"));
                if (UserSearch && objArray is null)
                    objArray = handler.GetJArrayElement(handler.GetJTokenValue(obj, "data", "search_by_raw_query", "search_timeline", "timeline", "instructions", 0, "entries"));
                var reversed = objArray.Reverse();
                foreach(var data in reversed)
                {
                    var type = handler.GetJTokenValue(data, "entryId");
                    if (type == "cursor-bottom-0" || (!string.IsNullOrEmpty(type) && type.Contains("cursor-bottom")))
                    {
                        MinPosition = handler.GetJTokenValue(data, "content", "value");
                        break;
                    }
                }
                if (string.IsNullOrEmpty(MinPosition))
                {
                    objArray = handler.GetJArrayElement(handler.GetJTokenValue(obj, "data", "search_by_raw_query", "search_timeline", "timeline", "instructions"));
                    for(int a = 0;a< objArray.Count; a++)
                    {
                        var typeData = handler.GetJTokenValue(objArray[a], "entry_id_to_replace");
                        if(!string.IsNullOrEmpty(typeData) && typeData == "cursor-bottom-0")
                        {
                            MinPosition = handler.GetJTokenValue(objArray[a], "entry", "content","value");
                            break;
                        }
                    }
                }
            }
            catch { }
            return MinPosition;
        }

        public string MinPosition { get; set; }
        public bool HasMore { get; set; }

        private void BrowserResponsehandler(string response)
        {
            try
            {
                if (response.StartsWith("{\"data\":{\"user\":"))
                {
                    var jsonHand = new Jsonhandler(response);

                    var instructions = jsonHand.GetJToken("data",
                                                    "user",
                                                    "result",
                                                    "timeline_v2",
                                                    "timeline",
                                                    "instructions").LastOrDefault();

                    var cursorValue =
                        jsonHand.GetJTokenValue(
                            jsonHand.GetJTokenOfJToken(instructions, "entries").Last(), "content", "value");
                    MinPosition = Uri.EscapeDataString(cursorValue);

                    var pinnedTweet = jsonHand.GetJToken("data", "user", "result", "timeline_v2", "timeline", "instructions", 0, "entry");
                    var timelineTweets = jsonHand.GetJTokenOfJToken(instructions, "entries");

                    var userName = string.Empty;
                    var profileImage = string.Empty;
                    foreach (var tweet in timelineTweets)
                    {

                        ListImagePath.Clear();
                        jsonHand = new Jsonhandler(tweet);
                        var entryId = jsonHand.GetJToken("entryId").ToString();
                        if (entryId.StartsWith("tweet"))
                        {
                            var tweetDetails = jsonHand.GetJToken("content", "itemContent", "tweet_results", "result", "legacy");
                            var TwtText = jsonHand.GetJTokenValue(tweetDetails, "full_text");
                            TwtText = TwtText.Contains("t.co") ? Regex.Replace(TwtText, @"https://t.co[^\s]+", "") : TwtText;
                            if (tweetDetails.ToString().Contains("quoted_status_permalink"))
                            {
                                var quotedLink = jsonHand.GetJTokenValue(tweetDetails, "quoted_status_permalink", "url");
                                TwtText += $" {quotedLink}";
                            }

                            var ListImagePathtoken = jsonHand.GetJTokenOfJToken(tweetDetails, "extended_entities", "media");
                            foreach (var Imagepath in ListImagePathtoken)
                            {
                                var images = jsonHand.GetJTokenValue(Imagepath, "media_url_https");
                                var url = jsonHand.GetJTokenValue(Imagepath, "url");
                                if (TwtText.Contains(url))
                                    TwtText = TwtText.Replace(url, "");
                                ListImagePath.Add(images);
                            }
                            if (TwtText.Contains($"pic.{Domain}/")) TwtText = Regex.Split(TwtText, $"pic.{Domain}/")[0];
                            var RetweetCount = jsonHand.GetJTokenValue(tweetDetails, "retweet_count");
                            var dt = TdTimeStampUtility.ConvertTimestamp(jsonHand, tweetDetails);
                            var TwtTimeStamp = dt.ConvertToEpoch();
                            if (tweetDetails["extended_entities"] != null
                                ? tweetDetails["extended_entities"]["media"][0]["video_info"] != null
                                : false)
                            {
                                ListImagePath.Clear();
                                foreach(var videosUrl in ListImagePathtoken)
                                {
                                    var getType = jsonHand.GetJTokenOfJToken(videosUrl, "type");
                                    if (getType.ToString().Contains("video"))
                                    {
                                        var ContentType = jsonHand.GetJTokenOfJToken(videosUrl, "video_info", "variants");
                                        foreach (var videos in ContentType)
                                        {
                                            var contentstype = jsonHand.GetJTokenValue(videos, "bitrate");
                                            if (!string.IsNullOrEmpty(contentstype))
                                            {
                                                ListImagePath.Add(jsonHand.GetJTokenValue(videos, "url"));
                                                break;
                                            }
                                        }
                                    }
                                    if (getType.ToString().Contains("photo"))
                                    {
                                        ListImagePath.Add(jsonHand.GetJTokenValue(videosUrl, "media_url_https"));
                                    }
                                    
                                }
                                

                                // ListImagePath.Add(jsonHand.GetJTokenValue(item.First(), "extended_entities", "media", 0, "video_info", "variants", 1, "url"));
                            }

                            IsLiked = jsonHand.GetJTokenValue(tweetDetails, "favorited") == "True";
                            IsRetweeted = jsonHand.GetJTokenValue(tweetDetails, "retweeted") == "True";
                            var originalTweetId = string.Empty;
                            var token = jsonHand.GetJToken("content", "itemContent", "tweet_results", "result");
                            var isReTweeted = !string.IsNullOrEmpty(originalTweetId = jsonHand.GetJTokenValue(token, "rest_id"));
                            var CommentCount = jsonHand.GetJTokenValue(tweetDetails, "reply_count");
                            var TweetId = jsonHand.GetJTokenValue(tweetDetails, "id_str");
                            var LikeCount = jsonHand.GetJTokenValue(tweetDetails, "favorite_count");
                            var Userid = jsonHand.GetJTokenValue(tweetDetails, "user_id_str");

                            var userDetails = jsonHand.GetJToken("content", "itemContent", "tweet_results", "result", "core", "user_results", "result", "legacy");
                            userName = jsonHand.GetJTokenValue(userDetails, "screen_name");
                            profileImage = jsonHand.GetJTokenValue(userDetails, "profile_image_url_https");
                            following = jsonHand.GetJTokenValue(userDetails, "following") == "True";
                            followBack = jsonHand.GetJTokenValue(userDetails, "followed_by") == "True";
                            Ismute = jsonHand.GetJTokenValue(userDetails, "muting") == "True";
                            Isverfied = jsonHand.GetJTokenValue(userDetails, "verified") == "True";
                            Isprotected = jsonHand.GetJTokenValue(userDetails, "protected") == "True";

                            ListTagDetails.Add(new TagDetails
                            {
                                Id = TweetId,
                                Username = userName,
                                UserId = Userid,
                                DateTime = dt,
                                Caption = HttpUtility.HtmlDecode(TwtText),
                                TweetedTimeStamp = TwtTimeStamp,
                                Code = string.Join("\n", ListImagePath),
                                IsAlreadyLiked = IsLiked,
                                IsAlreadyRetweeted = IsRetweeted,
                                ProfilePicUrl = profileImage,
                                IsVerified = Isverfied,
                                IsRetweet = isReTweeted || IsRetweeted,
                                OriginalTweetId = originalTweetId,
                                IsMuted = Ismute,
                                CommentCount = string.IsNullOrEmpty(CommentCount) ? 0 : int.Parse(CommentCount),
                                RetweetCount = string.IsNullOrEmpty(RetweetCount) ? 0 : int.Parse(RetweetCount),
                                LikeCount = string.IsNullOrEmpty(LikeCount) ? 0 : int.Parse(LikeCount),
                                FollowStatus = following,
                                FollowBackStatus = followBack,
                                IsPrivate = Isprotected,
                                HasProfilePic = profileImage.Contains("profile_images"),
                                IsTweetContainedVideo = tweetDetails["extended_entities"] != null
                                ? tweetDetails["extended_entities"]["media"][0]["video_info"] != null
                                : false
                            });
                        }
                        else if (entryId.StartsWith("homeConversation") || entryId.StartsWith("profile-conversation"))
                        {
                            var ConversationTweets = jsonHand.GetJToken("content", "items");
                            foreach (var eachConvTwt in ConversationTweets)
                            {
                                jsonHand = new Jsonhandler(eachConvTwt);
                                var tweetDetails = jsonHand.GetJToken("item", "itemContent", "tweet_results", "result", "legacy");
                                var TwtText = jsonHand.GetJTokenValue(tweetDetails, "full_text");
                                var ListImagePathtoken = jsonHand.GetJTokenOfJToken(tweetDetails, "extended_entities", "media");
                                foreach (var Imagepath in ListImagePathtoken)
                                {
                                    var images = jsonHand.GetJTokenValue(Imagepath, "media_url_https");
                                    var url = jsonHand.GetJTokenValue(Imagepath, "url");
                                    if (TwtText.Contains(url))
                                        TwtText = TwtText.Replace(url, "");
                                    ListImagePath.Add(images);
                                }
                                if (TwtText.Contains($"pic.{Domain}/")) TwtText = Regex.Split(TwtText, $"pic.{Domain}/")[0];
                                var RetweetCount = jsonHand.GetJTokenValue(tweetDetails, "retweet_count");
                                var dt = TdTimeStampUtility.ConvertTimestamp(jsonHand, tweetDetails);
                                var TwtTimeStamp = dt.ConvertToEpoch();
                                if (tweetDetails["extended_entities"] != null
                                    ? tweetDetails["extended_entities"]["media"][0]["video_info"] != null
                                    : false)
                                {
                                    ListImagePath.Clear();
                                    var ContentType = jsonHand.GetJTokenOfJToken(tweetDetails.First(), "extended_entities", "media", 0,
                                        "video_info", "variants");
                                    foreach (var videos in ContentType)
                                    {
                                        var contentstype = jsonHand.GetJTokenValue(videos, "bitrate");
                                        if (!string.IsNullOrEmpty(contentstype))
                                        {
                                            ListImagePath.Add(jsonHand.GetJTokenValue(videos, "url"));
                                            break;
                                        }
                                    }

                                    // ListImagePath.Add(jsonHand.GetJTokenValue(item.First(), "extended_entities", "media", 0, "video_info", "variants", 1, "url"));
                                }

                                IsLiked = jsonHand.GetJTokenValue(tweetDetails, "favorited") == "True";
                                IsRetweeted = jsonHand.GetJTokenValue(tweetDetails, "retweeted") == "True";
                                var CommentCount = jsonHand.GetJTokenValue(tweetDetails, "reply_count");
                                var TweetId = jsonHand.GetJTokenValue(tweetDetails, "id_str");
                                var LikeCount = jsonHand.GetJTokenValue(tweetDetails, "favorite_count");
                                var Userid = jsonHand.GetJTokenValue(tweetDetails, "user_id_str");
                                var originalTweetId = string.Empty;
                                var token = jsonHand.GetJToken("content", "itemContent", "tweet_results", "result");
                                var isReTweeted = !string.IsNullOrEmpty(originalTweetId = jsonHand.GetJTokenValue(token, "rest_id"));
                                var userDetails = jsonHand.GetJToken("item", "itemContent", "tweet_results", "result", "core", "user_results", "result", "legacy");
                                userName = jsonHand.GetJTokenValue(userDetails, "screen_name");
                                profileImage = jsonHand.GetJTokenValue(userDetails, "profile_image_url_https");
                                following = jsonHand.GetJTokenValue(userDetails, "following") == "True";
                                followBack = jsonHand.GetJTokenValue(userDetails, "followed_by") == "True";
                                Ismute = jsonHand.GetJTokenValue(userDetails, "muting") == "True";
                                Isverfied = jsonHand.GetJTokenValue(userDetails, "verified") == "True";
                                Isprotected = jsonHand.GetJTokenValue(userDetails, "protected") == "True";

                                ListTagDetails.Add(new TagDetails
                                {
                                    Id = TweetId,
                                    Username = userName,
                                    UserId = Userid,
                                    DateTime = dt,
                                    Caption = HttpUtility.HtmlDecode(TwtText),
                                    TweetedTimeStamp = TwtTimeStamp,
                                    Code = string.Join("\n", ListImagePath),
                                    IsAlreadyLiked = IsLiked,
                                    IsAlreadyRetweeted = IsRetweeted,
                                    ProfilePicUrl = profileImage,
                                    IsRetweet = isReTweeted || IsRetweeted,
                                    OriginalTweetId = originalTweetId,
                                    IsVerified = Isverfied,
                                    IsMuted = Ismute,
                                    CommentCount = string.IsNullOrEmpty(CommentCount) ? 0 : int.Parse(CommentCount),
                                    RetweetCount = string.IsNullOrEmpty(RetweetCount) ? 0 : int.Parse(RetweetCount),
                                    LikeCount = string.IsNullOrEmpty(LikeCount) ? 0 : int.Parse(LikeCount),
                                    FollowStatus = following,
                                    FollowBackStatus = followBack,
                                    IsPrivate = Isprotected,
                                    HasProfilePic = profileImage.Contains("profile_images"),
                                    IsTweetContainedVideo = tweetDetails["extended_entities"] != null
                                    ? tweetDetails["extended_entities"]["media"][0]["video_info"] != null
                                    : false
                                });
                            }

                        }


                    }
                }
                else
                {
                    try
                    {
                        var jsonHand = new Jsonhandler(response);
                        var tweets = jsonHand.GetJToken("globalObjects", "tweets").ToList();
                        var users = jsonHand.GetJToken("globalObjects", "users").ToList();
                        var checkEntries = jsonHand.GetJToken("timeline", "instructions").ToList();
                        var index = checkEntries.IndexOf(checkEntries.FirstOrDefault(x => x.ToString().Contains("addEntries")));
                        var cursorValue =
                            jsonHand.GetJTokenValue(
                                jsonHand.GetJToken("timeline", "instructions", index, "addEntries", "entries").Last(), "content",
                                "operation", "cursor", "value");
                        if (string.IsNullOrEmpty(cursorValue))
                        {
                            index = checkEntries.IndexOf(checkEntries.LastOrDefault());
                            cursorValue = jsonHand.GetJTokenValue(checkEntries[index], "replaceEntry", "entry", "content", "operation",
                                "cursor", "value");
                        }
                        MinPosition = Uri.EscapeDataString(cursorValue);
                        var userName = string.Empty;
                        var profileImage = string.Empty;


                        foreach (var item in tweets)
                        {
                            ListImagePath.Clear();
                            var token = item.First();
                            var TwtText = jsonHand.GetJTokenValue(token, "full_text");
                            var ListImagePathtoken = jsonHand.GetJTokenOfJToken(token, "extended_entities", "media").ToList();
                            foreach (var Imagepath in ListImagePathtoken)
                            {
                                var images = jsonHand.GetJTokenValue(Imagepath, "media_url_https");
                                var url = jsonHand.GetJTokenValue(Imagepath, "url");
                                if (TwtText.Contains(url))
                                    TwtText = TwtText.Replace(url, "");
                                ListImagePath.Add(images);
                            }

                            if (TwtText.Contains($"pic.{Domain}/")) TwtText = Regex.Split(TwtText, $"pic.{Domain}/")[0];
                            var RetweetCount = jsonHand.GetJTokenValue(token, "retweet_count");
                            var dt = TdTimeStampUtility.ConvertTimestamp(jsonHand, token);
                            var TwtTimeStamp = dt.ConvertToEpoch();
                            if (token["extended_entities"] != null
                                ? token["extended_entities"]["media"][0]["video_info"] != null
                                : false)
                            {
                                ListImagePath.Clear();
                                var ContentType = jsonHand.GetJTokenOfJToken(item.First(), "extended_entities", "media", 0,
                                    "video_info", "variants").ToList();
                                foreach (var videos in ContentType)
                                {
                                    var contentstype = jsonHand.GetJTokenValue(videos, "bitrate");
                                    if (!string.IsNullOrEmpty(contentstype))
                                    {
                                        ListImagePath.Add(jsonHand.GetJTokenValue(videos, "url"));
                                        break;
                                    }
                                }

                                // ListImagePath.Add(jsonHand.GetJTokenValue(item.First(), "extended_entities", "media", 0, "video_info", "variants", 1, "url"));
                            }

                            IsLiked = jsonHand.GetJTokenValue(token, "favorited") == "True";
                            IsRetweeted = jsonHand.GetJTokenValue(token, "retweeted") == "True";
                            var CommentCount = jsonHand.GetJTokenValue(token, "reply_count");
                            var TweetId = jsonHand.GetJTokenValue(token, "id_str");
                            var LikeCount = jsonHand.GetJTokenValue(token, "favorite_count");
                            var Userid = jsonHand.GetJTokenValue(token, "user_id_str");
                            var originalTweetId = string.Empty;
                            var token1 = jsonHand.GetJToken("content", "itemContent", "tweet_results", "result");
                            var isReTweeted = !string.IsNullOrEmpty(originalTweetId = jsonHand.GetJTokenValue(token1, "rest_id"));
                            foreach (var userdetail in users)
                            {
                                var jToken = userdetail.First();
                                var userId = jsonHand.GetJTokenValue(jToken, "id_str");
                                if (userId == Userid)
                                {
                                    userName = jsonHand.GetJTokenValue(jToken, "screen_name");
                                    profileImage = jsonHand.GetJTokenValue(jToken, "profile_image_url_https");
                                    following = jsonHand.GetJTokenValue(jToken, "following") == "True";
                                    followBack = jsonHand.GetJTokenValue(jToken, "followed_by") == "True";
                                    Ismute = jsonHand.GetJTokenValue(jToken, "muting") == "True";
                                    Isverfied = jsonHand.GetJTokenValue(jToken, "verified") == "True";
                                    Isprotected = jsonHand.GetJTokenValue(jToken, "protected") == "True";
                                    break;
                                }
                            }

                            ListTagDetails.Add(new TagDetails
                            {
                                Id = TweetId,
                                Username = userName,
                                UserId = Userid,
                                DateTime = dt,
                                Caption = HttpUtility.HtmlDecode(TwtText),
                                TweetedTimeStamp = TwtTimeStamp,
                                Code = string.Join("\n", ListImagePath),
                                IsAlreadyLiked = IsLiked,
                                IsAlreadyRetweeted = IsRetweeted,
                                ProfilePicUrl = profileImage,
                                IsRetweet = isReTweeted || IsRetweeted,
                                OriginalTweetId = originalTweetId,
                                IsVerified = Isverfied,
                                IsMuted = Ismute,
                                CommentCount = string.IsNullOrEmpty(CommentCount) ? 0 : int.Parse(CommentCount),
                                RetweetCount = string.IsNullOrEmpty(RetweetCount) ? 0 : int.Parse(RetweetCount),
                                LikeCount = string.IsNullOrEmpty(LikeCount) ? 0 : int.Parse(LikeCount),
                                FollowStatus = following,
                                FollowBackStatus = followBack,
                                IsPrivate = Isprotected,
                                HasProfilePic = profileImage.Contains("profile_images"),
                                IsTweetContainedVideo = token["extended_entities"] != null
                                    ? token["extended_entities"]["media"][0]["video_info"] != null
                                    : false
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        e.DebugLog(e.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }


            if (ListTagDetails.Count > 0)
                HasMore = true;
        }

        private void HttpResponseHandler(string response)
        {
            if (!response.Contains("<!DOCTYPE html>"))
            {
                var JsonObject = JObject.Parse(response)["inner"];
                if (JsonObject == null)
                    JsonObject = JObject.Parse(response);

                response = JsonObject["items_html"].ToString();
                MinPosition = JsonObject["min_position"].ToString();
            }

            if (!string.IsNullOrEmpty(MinPosition))
                MinPosition = HttpUtility.UrlEncode(MinPosition);

            var SplitTagDetails = Regex.Split(response, "tweet js-stream-tweet js-actionable-tweet").ToList();

            var itemHtmlDoc = new HtmlDocument();

            foreach (var item in SplitTagDetails)
            {
                if (item.Contains("<!DOCTYPE html>") || !item.Contains("data-tweet-id"))
                    continue;

                itemHtmlDoc.LoadHtml(item);

                var TwtText = TweetHelper.GetEmoji(item); //TdUtility.ReplaceImageWithEmogis(TwtText, EmojiDict);
                if (TwtText.Contains($"pic.{Domain}/")) TwtText = Regex.Split(TwtText, $"pic.{Domain}/")[0];
                int.TryParse(
                    HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item, "tweet-timestamp", "data-time",
                        itemHtmlDoc),
                    out var TwtTimeStamp);
                var ListImagePath =
                    HtmlAgilityHelper.getListValueWithAttributeNameFromInnerHtml(item,
                        "AdaptiveMedia-photoContainer", "src", itemHtmlDoc);
                var CommentCount = HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item,
                    "ProfileTweet-action--reply", "data-tweet-stat-count", itemHtmlDoc);
                var RetweetCount = HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item,
                    "ProfileTweet-action--retweet", "data-tweet-stat-count", itemHtmlDoc);
                var LikeCount = HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item,
                    "ProfileTweet-action--favorite", "data-tweet-stat-count", itemHtmlDoc);
                var profileImage =
                    HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item,
                        "js-action-profile js-user-profile-link", "src", itemHtmlDoc);
                ListTagDetails.Add(new TagDetails
                {
                    Id = Utilities.GetBetween(item, "data-tweet-id=\"", "\""),
                    Username = Utilities.GetBetween(item, "data-screen-name=\"", "\""),
                    UserId = Utilities.GetBetween(item, "data-user-id=\"", "\""),
                    Caption = HttpUtility.HtmlDecode(TwtText),
                    TweetedTimeStamp = TwtTimeStamp,
                    Code = string.Join("\n", ListImagePath),
                    IsAlreadyLiked = item.Contains("favorited"),
                    IsAlreadyRetweeted = item.Contains("data-my-retweet-id"),
                    IsRetweet = item.Contains("Icon--retweeted"),
                    IsVerified = item.Contains("Icon--verified"),
                    IsMuted = item.Contains("   muting "),
                    CommentCount = string.IsNullOrEmpty(CommentCount) ? 0 : int.Parse(CommentCount),
                    RetweetCount = string.IsNullOrEmpty(RetweetCount) ? 0 : int.Parse(RetweetCount),
                    LikeCount = string.IsNullOrEmpty(LikeCount) ? 0 : int.Parse(LikeCount),
                    FollowStatus = item.Contains("data-you-follow=\"true\""),
                    FollowBackStatus = item.Contains("data-follows-you=\"true\""),
                    IsPrivate = item.Contains("Icon--protected"),
                    HasProfilePic = !profileImage.Contains("default_profile_images"),
                    IsTweetContainedVideo = item.Contains("AdaptiveMedia-videoContainer")
                });
            }

            if (ListTagDetails.Count > 0)
                HasMore = true;
        }

        private void TimelineResponseHandler(string response,bool UserSearch=false)
        {
            try
            {
                if (response.Contains("{\"data\":{\"search_by_raw_query\":"))
                {
                    var handler = new Jsonhandler(response);
                    var tweets = handler.GetJToken("data",
                                                    "search_by_raw_query",
                                                    "search_timeline",
                                                    "timeline",
                                                    "instructions", 0, "entries").ToList();
                    if(UserSearch && (tweets is null || tweets.Count == 0))
                        tweets = handler.GetJToken("data",
                                                    "search_by_raw_query",
                                                    "search_timeline",
                                                    "timeline",
                                                    "instructions", 1, "entries").ToList();
                    var cursorValue = handler.GetJTokenValue(tweets.Last(), "content", "value");
                    MinPosition =string.IsNullOrEmpty(MinPosition)? Uri.EscapeDataString(cursorValue):MinPosition;

                    foreach (var tweet in tweets)
                    {
                        var jsonHand = new Jsonhandler(tweet);
                        var entryId = jsonHand.GetJToken("entryId").ToString();
                        if (entryId.StartsWith("tweet"))
                        {
                            var tweetResult = jsonHand.GetJToken("content", "itemContent", "tweet_results", "result");

                            // User Info
                            var userResult = jsonHand.GetJTokenOfJToken(tweetResult, "core", "user_results", "result");
                            if(userResult is  null || !userResult.HasValues)
                            {
                                userResult = jsonHand.GetJTokenOfJToken(tweetResult,
                                                        "tweet",
                                                        "core",
                                                        "user_results",
                                                        "result");
                                tweetResult = jsonHand.GetJTokenOfJToken(tweetResult, "tweet");
                            }
                                
                            var userId = jsonHand.GetJTokenValue(userResult, "rest_id");
                            var username = jsonHand.GetJTokenValue(userResult, "legacy", "screen_name");
                            var fullName = jsonHand.GetJTokenValue(userResult, "legacy", "name");
                            var profilePicUrl = jsonHand.GetJTokenValue(userResult, "legacy", "profile_image_url_https");
                            var isBlueVerified = jsonHand.GetJTokenValue(userResult, "is_blue_verified");
                            var hasGraduatedAccess = jsonHand.GetJTokenValue(userResult, "has_graduated_access");
                            var profileImageShape = jsonHand.GetJTokenValue(userResult, "profile_image_shape");
                            var canDm = jsonHand.GetJTokenValue(userResult, "legacy", "can_dm");
                            var canMediaTag = jsonHand.GetJTokenValue(userResult, "legacy", "can_media_tag");
                            var userCreationTime = TdTimeStampUtility.ConvertTimestamp(jsonHand,
                                                        jsonHand.GetJTokenOfJToken(userResult, "legacy"));
                            var userTimeStamp = userCreationTime.ConvertToEpoch();
                            var defaultProfile = jsonHand.GetJTokenValue(userResult, "legacy", "default_profile");
                            var defaultProfileImage = jsonHand.GetJTokenValue(userResult, "legacy", "default_profile_image");
                            var userDescription = jsonHand.GetJTokenValue(userResult, "legacy", "description");
                            var fastFollowersCount = jsonHand.GetJTokenValue(userResult, "legacy", "fast_followers_count");
                            var favouritesCount = jsonHand.GetJTokenValue(userResult, "legacy", "favourites_count");
                            var followersCount = jsonHand.GetJTokenValue(userResult, "legacy", "followers_count");
                            var friendsCount = jsonHand.GetJTokenValue(userResult, "legacy", "friends_count");
                            var hasCustomTimelines = jsonHand.GetJTokenValue(userResult, "legacy", "has_custom_timelines");
                            var isTranslator = jsonHand.GetJTokenValue(userResult, "legacy", "isTranslator");
                            var listedCount = jsonHand.GetJTokenValue(userResult, "legacy", "listed_count");
                            var location = jsonHand.GetJTokenValue(userResult, "legacy", "location");
                            var mediaCount = jsonHand.GetJTokenValue(userResult, "legacy", "media_count");
                            var pinnedTweetId = jsonHand.GetJTokenValue(userResult, "legacy", "pinned_tweet_ids_str", 0);
                            var isSensitive = jsonHand.GetJTokenValue(userResult, "legacy", "possibly_sensitive");
                            var profileBannerUrl = jsonHand.GetJTokenValue(userResult, "legacy", "profile_banner_url");
                            var statusesCount = jsonHand.GetJTokenValue(userResult, "legacy", "statuses_count");
                            var translatorType = jsonHand.GetJTokenValue(userResult, "legacy", "translator_type");
                            var isVerified = jsonHand.GetJTokenValue(userResult, "legacy", "verified");
                            var wantRetweets = jsonHand.GetJTokenValue(userResult, "legacy", "want_retweets");
                            var VerifiedPhoneStatus = jsonHand.GetJTokenValue(userResult, "legacy", "verified_phone_status");

                            // Tweet Info
                            ListImagePath.Clear();
                            var listImagePathToken = jsonHand.GetJTokenOfJToken(tweetResult, "legacy", "extended_entities", "media");
                            foreach (var Imagepath in listImagePathToken)
                            {
                                var images = jsonHand.GetJTokenValue(Imagepath, "media_url_https");
                                var expandedUrl = jsonHand.GetJTokenValue(Imagepath, "expanded_url");
                                var url = jsonHand.GetJTokenValue(Imagepath, "url");
                                var type = jsonHand.GetJTokenValue(Imagepath, "type");
                                ListImagePath.Add(images);
                            }
                            var tweetId = jsonHand.GetJTokenValue(tweetResult, "rest_id");
                            var bookmarkCount = jsonHand.GetJTokenValue(tweetResult, "legacy", "bookmark_count");
                            var bookmarked = jsonHand.GetJTokenValue(tweetResult, "legacy", "bookmarked");
                            var tweetCreatedDt = TdTimeStampUtility.ConvertTimestamp(jsonHand,
                                                    jsonHand.GetJTokenOfJToken(tweetResult, "legacy"));
                            var tweetTimeStamp = tweetCreatedDt.ConvertToEpoch();
                            var converstionId = jsonHand.GetJTokenValue(tweetResult, "legacy", "conversation_id_str");
                            var favoriteCount = jsonHand.GetJTokenValue(tweetResult, "legacy", "favorite_count");
                            var favorited = jsonHand.GetJTokenValue(tweetResult, "legacy", "favorited");
                            var fullText = jsonHand.GetJTokenValue(tweetResult, "legacy", "full_text");
                            if(!string.IsNullOrEmpty(fullText) && Regex.IsMatch(fullText, @"http[^\s]+"))
                                fullText = Regex.Replace(fullText, @"http[^\s]+$", "");
                            var inReplyToScreenName = jsonHand.GetJTokenValue(tweetResult, "legacy", "in_reply_to_screen_name");
                            var inReplyToStatusId = jsonHand.GetJTokenValue(tweetResult, "legacy", "in_reply_to_status_id_str");
                            var inReplyToUserId = jsonHand.GetJTokenValue(tweetResult, "legacy", "in_reply_to_user_id_str");
                            var isQuoteStatus = jsonHand.GetJTokenValue(tweetResult, "legacy", "is_quote_status");
                            var lang = jsonHand.GetJTokenValue(tweetResult, "legacy", "lang");
                            var quoteCount = jsonHand.GetJTokenValue(tweetResult, "legacy", "quote_count");
                            var replyCount = jsonHand.GetJTokenValue(tweetResult, "legacy", "reply_count");
                            var retweetCount = jsonHand.GetJTokenValue(tweetResult, "legacy", "retweet_count");
                            var retweeted = jsonHand.GetJTokenValue(tweetResult, "legacy", "retweeted");
                            int.TryParse(replyCount, out int commentCount);
                            int.TryParse(retweetCount, out int retweetcount);
                            int.TryParse(favoriteCount, out int likeCount);
                            ListTagDetails.Add(new TagDetails
                            {
                                Id = tweetId,
                                Username = username,
                                UserId = userId,
                                DateTime = tweetCreatedDt,
                                Caption = HttpUtility.HtmlDecode(fullText),
                                TweetedTimeStamp = tweetTimeStamp,
                                Code = string.Join("\n", ListImagePath),
                                IsAlreadyLiked = favorited == "True",
                                IsAlreadyRetweeted = retweeted == "True",
                                ProfilePicUrl = profilePicUrl,
                                IsVerified = isVerified == "True",
                                CommentCount = commentCount,
                                RetweetCount = retweetcount,
                                LikeCount = likeCount,
                                HasProfilePic = !string.IsNullOrEmpty(profilePicUrl) && profilePicUrl.Contains("profile_images")
                            });

                        }
                        else if (entryId.StartsWith("user"))
                        {
                            var userResult = jsonHand.GetJTokenOfJToken(tweet, "content", "itemContent", "user_results", "result");
                            var userId = jsonHand.GetJTokenValue(userResult, "rest_id");
                            var username = jsonHand.GetJTokenValue(userResult, "legacy", "screen_name");
                            var fullName = jsonHand.GetJTokenValue(userResult, "legacy", "name");
                            var profilePicUrl = jsonHand.GetJTokenValue(userResult, "legacy", "profile_image_url_https");
                            var isBlueVerified = jsonHand.GetJTokenValue(userResult, "is_blue_verified");
                            var hasGraduatedAccess = jsonHand.GetJTokenValue(userResult, "has_graduated_access");
                            var profileImageShape = jsonHand.GetJTokenValue(userResult, "profile_image_shape");
                            var canDm = jsonHand.GetJTokenValue(userResult, "legacy", "can_dm");
                            var canMediaTag = jsonHand.GetJTokenValue(userResult, "legacy", "can_media_tag");
                            var userCreationTime = TdTimeStampUtility.ConvertTimestamp(jsonHand,
                                                        jsonHand.GetJTokenOfJToken(userResult, "legacy"));
                            var userTimeStamp = userCreationTime.ConvertToEpoch();
                            var defaultProfile = jsonHand.GetJTokenValue(userResult, "legacy", "default_profile");
                            var defaultProfileImage = jsonHand.GetJTokenValue(userResult, "legacy", "default_profile_image");
                            var userDescription = jsonHand.GetJTokenValue(userResult, "legacy", "description");
                            var fastFollowersCount = jsonHand.GetJTokenValue(userResult, "legacy", "fast_followers_count");
                            var favouritesCount = jsonHand.GetJTokenValue(userResult, "legacy", "favourites_count");
                            var followersCount = jsonHand.GetJTokenValue(userResult, "legacy", "followers_count");
                            int.TryParse(followersCount, out var fastFollowers);
                            var friendsCount = jsonHand.GetJTokenValue(userResult, "legacy", "friends_count");
                            int.TryParse(friendsCount, out var followingCount);
                            var hasCustomTimelines = jsonHand.GetJTokenValue(userResult, "legacy", "has_custom_timelines");
                            var isTranslator = jsonHand.GetJTokenValue(userResult, "legacy", "isTranslator");
                            var listedCount = jsonHand.GetJTokenValue(userResult, "legacy", "listed_count");
                            var likesCount = jsonHand.GetJTokenValue(userResult, "legacy", "favourites_count");
                            int.TryParse(likesCount, out var likes);
                            var location = jsonHand.GetJTokenValue(userResult, "legacy", "location");
                            var mediaCount = jsonHand.GetJTokenValue(userResult, "legacy", "media_count");
                            int.TryParse (mediaCount, out var media);
                            var pinnedTweetId = jsonHand.GetJTokenValue(userResult, "legacy", "pinned_tweet_ids_str", 0);
                            var isSensitive = jsonHand.GetJTokenValue(userResult, "legacy", "possibly_sensitive");
                            var profileBannerUrl = jsonHand.GetJTokenValue(userResult, "legacy", "profile_banner_url");
                            var statusesCount = jsonHand.GetJTokenValue(userResult, "legacy", "statuses_count");
                            var translatorType = jsonHand.GetJTokenValue(userResult, "legacy", "translator_type");
                            var isVerified = jsonHand.GetJTokenValue(userResult, "legacy", "verified");
                            var wantRetweets = jsonHand.GetJTokenValue(userResult, "legacy", "want_retweets");
                            var VerifiedPhoneStatus = jsonHand.GetJTokenValue(userResult, "legacy", "verified_phone_status");
                            bool.TryParse(jsonHand.GetJTokenValue(userResult, "legacy", "following"), out bool following);
                            ListTagDetails.Add(new TagDetails
                            {
                                Username = username,
                                UserId = userId,
                                ProfilePicUrl = profilePicUrl,
                                IsVerified = isVerified == "True",
                                HasProfilePic = !string.IsNullOrEmpty(profilePicUrl) && profilePicUrl.Contains("profile_images"),
                                twitterUser = new TwitterUser
                                {
                                    Username = username,
                                    UserId = userId,
                                    ProfilePicUrl = profilePicUrl,
                                    IsVerified = isVerified == "True",
                                    FollowersCount = fastFollowers,
                                    FollowingsCount = followingCount,
                                    JoiningDate = userCreationTime,
                                    LikesCount = likes,
                                    UserLocation = location,
                                    TweetsCount = media,
                                    FollowStatus = following,
                                    UserBio = userDescription
                                }
                            });
                        }
                    }
                }
            }
            catch
            {
                //ex.DebugLog(ex.Message);
            }
            if (ListTagDetails.Count > 0)
                HasMore = true;
        }
    }
}