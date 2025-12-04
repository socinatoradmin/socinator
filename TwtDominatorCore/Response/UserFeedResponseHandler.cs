using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response
{
    public class UserFeedResponseHandler : TdBaseHtmlResponseHandler
    {
        public bool following, followBack, Ismute, Isverfied, Isprotected;
        public bool hasmore;
        public List<string> ListImagePath = new List<string>();
        public string MinPosition;
        public int usersCount;
        public List<TagDetails> UserTweetsDetail = new List<TagDetails>();

        public UserFeedResponseHandler(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;
            var HasMoreItems = "True";

            hasmore = false;
            try
            {
                var Response = response.Response;
                var EmojiDict = TdUtility.GetAllEmojis();
                int TwtTimeStamp = 0, CommentCount = 0, RetweetCount = 0, LikeCount = 0;
                MinPosition = Utilities.GetBetween(Response, "data-min-position=\"", "\"");
                if (Response.Contains("{\"globalObjects\":{\"tweets\":{") || Response.StartsWith("{\"data\":{\"user\":"))
                {
                    BrowserResponsehandler(response, ref HasMoreItems, ref TwtTimeStamp, ref CommentCount,
                        ref RetweetCount, ref LikeCount);
                }

                else
                {
                    if (!Response.Contains("<!DOCTYPE html>"))
                    {
                        var JsonObject = new Jsonhandler(response.Response);
                        var ChildernToken = JsonObject.GetJToken("inner");
                        var IsGetData = false;
                        try
                        {
                            Response = JsonObject.GetJTokenValue(ChildernToken, "items_html");
                            MinPosition = JsonObject.GetJTokenValue(ChildernToken, "min_position");
                            HasMoreItems = JsonObject.GetJTokenValue(ChildernToken, "has_more_items");
                            IsGetData = true;
                        }

                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        if (ChildernToken.Count() == 0 && string.IsNullOrEmpty(MinPosition))
                        {
                            Response = JsonObject.GetElementValue("items_html");
                            MinPosition = JsonObject.GetElementValue("min_position");
                            HasMoreItems = JsonObject.GetElementValue("has_more_items");
                        }

                        if (!IsGetData)
                        {
                            JsonObject = new Jsonhandler(response.Response);
                            ChildernToken = JsonObject.GetJToken("inner");
                            Response = JsonObject.GetJTokenValue(ChildernToken, "items_html");
                            MinPosition = JsonObject.GetJTokenValue(ChildernToken, "min_position");
                            if (string.IsNullOrEmpty(Response.Trim()))
                                HasMoreItems = "False";
                        }
                    }


                    var SplitTagDetails = Regex.Split(Response, "tweet js-stream-tweet js-actionable-tweet").ToList();
                    foreach (var item in SplitTagDetails)
                    {
                        if (item.Contains("<!DOCTYPE html>") || !item.Contains("data-tweet-id"))
                            continue;

                        var itemHtmlDoc = new HtmlDocument();
                        itemHtmlDoc.LoadHtml(item);

                        var TwtText =
                            HtmlAgilityHelper.getStringInnerHtmlFromClassName(item, "js-tweet-text-container",
                                itemHtmlDoc);
                        TwtText = TdUtility.ReplaceImageWithEmogis(TwtText, EmojiDict);
                        if (TwtText.Contains($"pic.{Domain}")) TwtText = Regex.Split(TwtText, $"pic.{Domain}/")[0];
                        TwtText = HttpUtility.HtmlDecode(TwtText);
                        int.TryParse(
                            HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item, "tweet-timestamp",
                                "data-time", itemHtmlDoc),
                            out TwtTimeStamp);
                        var isComment = item.Contains("data-is-reply-to=\"true\"");
                        int.TryParse(
                            HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item, "ProfileTweet-action--reply",
                                "data-tweet-stat-count", itemHtmlDoc), out CommentCount);
                        int.TryParse(
                            HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item,
                                "ProfileTweet-action--retweet",
                                "data-tweet-stat-count", itemHtmlDoc), out RetweetCount);
                        int.TryParse(
                            HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item,
                                "ProfileTweet-action--favorite",
                                "data-tweet-stat-count", itemHtmlDoc), out LikeCount);
                        UserTweetsDetail.Add(new TagDetails
                        {
                            Id = Utilities.GetBetween(item, "data-tweet-id=\"", "\""),
                            Username = Utilities.GetBetween(item, "data-screen-name=\"", "\""),
                            UserId = Utilities.GetBetween(item, "data-user-id=\"", "\""),
                            Caption = TwtText,
                            TweetedTimeStamp = TwtTimeStamp,
                            Code = Utilities.GetBetween(item, "data-image-url=\"", "\""),
                            IsAlreadyLiked = item.Contains("favorited"),
                            IsAlreadyRetweeted = item.Contains("data-my-retweet-id"), //data-my-retweet-id retweeted
                            CommentCount = CommentCount,
                            RetweetCount = RetweetCount,
                            LikeCount = LikeCount,
                            IsRetweet = item.Contains("Icon--retweeted"),
                            FollowStatus = item.Contains("data-you-follow=\"true\""),
                            FollowBackStatus = item.Contains("data-follows-you=\"true\""),
                            IsComment = isComment,
                            CommentedOnTweetId =
                                isComment ? Utilities.GetBetween(item, "data-conversation-id=\"", "\"") : "",
                            CommentedOnTweetOwner =
                                isComment ? Utilities.GetBetween(item, "data-mentions=\"", "\"") : "",
                            IsTweetContainedVideo = item.Contains("AdaptiveMedia-videoContainer")
                        });
                    }

                    if (HasMoreItems.Equals("True"))
                        hasmore = true;
                }
            }

            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void BrowserResponsehandler(IResponseParameter response, ref string HasMoreItems, ref int TwtTimeStamp,
            ref int CommentCount, ref int RetweetCount, ref int LikeCount)
        {
            try
            {

                if (response.Response.StartsWith("{\"data\":{\"user\":"))
                {
                    try
                    {
                        var jsonHand = new Jsonhandler(response.Response);
                        var instructions = jsonHand.GetJToken("data", "user", "result", "timeline", "timeline", "instructions");
                        instructions = instructions is null || !instructions.HasValues ? jsonHand.GetJToken("data", "user", "result", "timeline_v2", "timeline", "instructions") : instructions;
                        var userName = string.Empty;
                        var profileImage = string.Empty;
                        var timelineTweets = new List<JToken>();
                        JToken pinnedTweet = null;
                        foreach (var instruction in instructions)
                        {
                            try
                            {
                                var entries = jsonHand.GetJTokenOfJToken(instruction, "entries");
                                if (entries.Count() > 0)
                                {
                                    var lastEntries = jsonHand.GetJTokenOfJToken(instruction, "entries").Last();
                                    var myCursorValue = jsonHand.GetJTokenValue(lastEntries, "content", "value");
                                    MinPosition = Uri.EscapeDataString(myCursorValue);
                                    pinnedTweet = jsonHand.GetJToken(instruction, "entry");
                                    timelineTweets = jsonHand.GetJTokenOfJToken(entries).ToList();
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                        foreach (var tweet in timelineTweets)
                        {
                            var iscomment = false;

                            ListImagePath.Clear();
                            jsonHand = new Jsonhandler(tweet);
                            var entryId = jsonHand.GetJToken("entryId").ToString();
                            var username = string.Empty;
                            if (entryId.StartsWith("tweet"))
                            {

                                var tweetDetails = jsonHand.GetJToken("content", "itemContent", "tweet_results", "result", "legacy");
                                var TwtText = jsonHand.GetJTokenValue(tweetDetails, "full_text");
                                var ListImagePathtoken = jsonHand.GetJTokenOfJToken(tweetDetails, "extended_entities", "media").ToList();
                                foreach (var Imagepath in ListImagePathtoken)
                                {
                                    var images = jsonHand.GetJTokenValue(Imagepath, "media_url_https");
                                    var url = jsonHand.GetJTokenValue(Imagepath, "url");
                                    if (TwtText.Contains(url))
                                        TwtText = TwtText.Replace(url, "");
                                    ListImagePath.Add(images);
                                }
                                if (tweetDetails["extended_entities"] != null
                                    ? tweetDetails["extended_entities"]["media"][0]["video_info"] != null
                                    : false)
                                {
                                    ListImagePath.Clear();
                                    var ContentType = jsonHand.GetJTokenOfJToken(tweetDetails.First(), "extended_entities", "media", 0,
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
                                var dt = TdTimeStampUtility.ConvertTimestamp(jsonHand, tweetDetails);
                                TwtTimeStamp = dt.ConvertToEpoch();

                                int.TryParse(jsonHand.GetJTokenValue(tweetDetails, "reply_count"), out CommentCount);
                                int.TryParse(jsonHand.GetJTokenValue(tweetDetails, "retweet_count"), out RetweetCount);
                                int.TryParse(jsonHand.GetJTokenValue(tweetDetails, "favorite_count"), out LikeCount);
                                var tweetId = jsonHand.GetJTokenValue(tweetDetails, "id_str");
                                var SourceTweetId = jsonHand.GetJTokenValue(tweetDetails, "retweeted_status_result", "result", "rest_id");
                                var isRetweetedOwnTweet = !string.IsNullOrEmpty(SourceTweetId) && tweetId?.Trim() == SourceTweetId?.Trim();
                                var commenteduserid = jsonHand.GetJTokenValue(tweetDetails, "in_reply_to_user_id_str");
                                iscomment = !string.IsNullOrEmpty(commenteduserid);
                                var CommentedTweetId = jsonHand.GetJTokenValue(tweetDetails, "in_reply_to_status_id_str");
                                var userDetails = jsonHand.GetJToken("content", "itemContent", "tweet_results", "result", "core", "user_results", "result", "legacy");
                                username = jsonHand.GetJTokenValue(userDetails, "screen_name");
                                following = jsonHand.GetJTokenValue(userDetails, "following") == "True";
                                followBack = jsonHand.GetJTokenValue(userDetails, "followed_by") == "True";
                                Ismute = jsonHand.GetJTokenValue(userDetails, "muting") == "True";
                                Isverfied = jsonHand.GetJTokenValue(userDetails, "verified") == "True";
                                Isprotected = jsonHand.GetJTokenValue(userDetails, "protected") == "True";

                                UserTweetsDetail.Add(new TagDetails
                                {
                                    Id = tweetId,
                                    Username = username,
                                    UserId = jsonHand.GetJTokenValue(tweetDetails, "user_id_str"),
                                    Caption = TwtText,
                                    DateTime = dt,
                                    TweetedTimeStamp = TwtTimeStamp,
                                    Code = string.Join("\n", ListImagePath),
                                    IsAlreadyLiked = jsonHand.GetJTokenValue(tweetDetails, "favorited") == "True",
                                    IsAlreadyRetweeted =
                                        jsonHand.GetJTokenValue(tweetDetails, "retweeted") == "True", //data-my-retweet-id retweeted
                                    CommentCount = CommentCount,
                                    RetweetCount = RetweetCount,
                                    LikeCount = LikeCount,
                                    IsRetweet = jsonHand.GetJTokenValue(tweetDetails, "retweeted") == "True",
                                    FollowStatus = following,
                                    FollowBackStatus = followBack,
                                    IsComment = iscomment,
                                    CommentedOnTweetId = CommentedTweetId,
                                    OriginalTweetId=SourceTweetId,
                                    IsRetweetedOwnTweet = isRetweetedOwnTweet,
                                    // isComment ? jHand.GetJTokenValue(token, "conversation_id_str") : "",
                                    // CommentedOnTweetOwner = isComment ? jHand.GetJTokenValue(token, "in_reply_to_screen_name") : "",
                                    IsTweetContainedVideo = tweetDetails["extended_entities"] != null
                                    ? tweetDetails["extended_entities"]["media"][0]["video_info"] != null
                                    : false
                                });
                            }
                            else if (entryId.StartsWith("homeConversation") || entryId.StartsWith("profile-conversation"))
                            {
                                var ConversationTweets = jsonHand.GetJToken("content", "items").ToList();
                                foreach (var eachConvTwt in ConversationTweets)
                                {

                                    jsonHand = new Jsonhandler(eachConvTwt);
                                    var tweetDetails = jsonHand.GetJToken("item", "itemContent", "tweet_results", "result", "legacy");
                                    var TwtText = jsonHand.GetJTokenValue(tweetDetails, "full_text");
                                    var ListImagePathtoken = jsonHand.GetJTokenOfJToken(tweetDetails, "extended_entities", "media").ToList();
                                    foreach (var Imagepath in ListImagePathtoken)
                                    {
                                        var images = jsonHand.GetJTokenValue(Imagepath, "media_url_https");
                                        var url = jsonHand.GetJTokenValue(Imagepath, "url");
                                        if (TwtText.Contains(url))
                                            TwtText = TwtText.Replace(url, "");
                                        ListImagePath.Add(images);
                                    }
                                    if (TwtText.Contains($"pic.{Domain}/")) TwtText = Regex.Split(TwtText, $"pic.{Domain}/")[0];
                                    var dt = TdTimeStampUtility.ConvertTimestamp(jsonHand, tweetDetails);
                                    TwtTimeStamp = dt.ConvertToEpoch();

                                    int.TryParse(jsonHand.GetJTokenValue(tweetDetails, "reply_count"), out CommentCount);
                                    int.TryParse(jsonHand.GetJTokenValue(tweetDetails, "retweet_count"), out RetweetCount);
                                    int.TryParse(jsonHand.GetJTokenValue(tweetDetails, "favorite_count"), out LikeCount);
                                    var tweetId = jsonHand.GetJTokenValue(tweetDetails, "id_str");
                                    var SourceTweetId = jsonHand.GetJTokenValue(tweetDetails, "retweeted_status_result", "result", "rest_id");
                                    var isRetweetedOwnTweet = !string.IsNullOrEmpty(SourceTweetId) && tweetId?.Trim() == SourceTweetId?.Trim();
                                    var commenteduserid = jsonHand.GetJTokenValue(tweetDetails, "in_reply_to_user_id_str");
                                    iscomment = !string.IsNullOrEmpty(commenteduserid);
                                    var CommentedTweetId = jsonHand.GetJTokenValue(tweetDetails, "in_reply_to_status_id_str");
                                    var userDetails = jsonHand.GetJToken("item", "itemContent", "tweet_results", "result", "core", "user_results", "result", "legacy");
                                    username = jsonHand.GetJTokenValue(userDetails, "screen_name");
                                    following = jsonHand.GetJTokenValue(userDetails, "following") == "True";
                                    followBack = jsonHand.GetJTokenValue(userDetails, "followed_by") == "True";
                                    Ismute = jsonHand.GetJTokenValue(userDetails, "muting") == "True";
                                    Isverfied = jsonHand.GetJTokenValue(userDetails, "verified") == "True";
                                    Isprotected = jsonHand.GetJTokenValue(userDetails, "protected") == "True";

                                    UserTweetsDetail.Add(new TagDetails
                                    {
                                        Id = tweetId,
                                        Username = username,
                                        UserId = jsonHand.GetJTokenValue(tweetDetails, "user_id_str"),
                                        Caption = TwtText,
                                        DateTime = dt,
                                        TweetedTimeStamp = TwtTimeStamp,
                                        Code = string.Join("\n", ListImagePath),
                                        IsAlreadyLiked = jsonHand.GetJTokenValue(tweetDetails, "favorited") == "True",
                                        IsAlreadyRetweeted =
                                        jsonHand.GetJTokenValue(tweetDetails, "retweeted") == "True", //data-my-retweet-id retweeted
                                        CommentCount = CommentCount,
                                        RetweetCount = RetweetCount,
                                        LikeCount = LikeCount,
                                        IsRetweet = jsonHand.GetJTokenValue(tweetDetails, "retweeted") == "True",
                                        FollowStatus = following,
                                        FollowBackStatus = followBack,
                                        IsComment = iscomment,
                                        CommentedOnTweetId = CommentedTweetId,
                                        OriginalTweetId= SourceTweetId,
                                        IsRetweetedOwnTweet = isRetweetedOwnTweet,
                                        // isComment ? jHand.GetJTokenValue(token, "conversation_id_str") : "",
                                        // CommentedOnTweetOwner = isComment ? jHand.GetJTokenValue(token, "in_reply_to_screen_name") : "",
                                        IsTweetContainedVideo = tweetDetails["extended_entities"] != null
                                    ? tweetDetails["extended_entities"]["media"][0]["video_info"] != null
                                    : false
                                    });
                                }

                            }
                        }
                        usersCount = UserTweetsDetail.Count();
                        hasmore =  !string.IsNullOrEmpty(MinPosition) && usersCount > 0;
                        HasMoreItems = hasmore ? "True" : "False";
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                else if (response.Response.Contains("{\"globalObjects\":{\""))
                {
                    var jHand = new Jsonhandler(response.Response);
                    hasmore = true;
                    var username = "";

                    try
                    {
                        var users = jHand.GetJToken("globalObjects", "tweets");
                        var userdetail = jHand.GetJToken("globalObjects", "users");
                        usersCount = users.Children().Count();


                        var cursorValue =
                            jHand.GetJTokenValue(jHand.GetJToken("timeline", "instructions", 0, "addEntries", "entries").Last(),
                                "content", "operation", "cursor", "value");
                        MinPosition = cursorValue;
                        var pinnedTweet = jHand.GetJToken("data", "user", "result", "timeline", "timeline", "instructions", 1, "entry");
                        var timelineTweets = jHand.GetJToken("data", "user", "result", "timeline", "timeline", "instructions", 0, "entries");



                        foreach (var items in users)
                        {
                            var token = items.First();
                            var dt = TdTimeStampUtility.ConvertTimestamp(jHand, token);
                            TwtTimeStamp = dt.ConvertToEpoch();
                            var TwtText = jHand.GetJTokenValue(items.First(), "full_text");
                            var ListImagePathtoken = jHand.GetJTokenOfJToken(token, "extended_entities", "media");
                            var iscomment = false;

                            foreach (var Imagepath in ListImagePathtoken)
                            {
                                var images = jHand.GetJTokenValue(Imagepath, "media_url_https");
                                ListImagePath.Add(images);
                            }

                            if (token["extended_entities"] != null &&
                                token["extended_entities"]["media"][0]["video_info"] != null)
                            {
                                ListImagePath.Clear();
                                ListImagePath.Add(jHand.GetJTokenValue(items.First(), "extended_entities", "media", 0,
                                    "video_info", "variants", 1, "url"));
                            }

                            int.TryParse(jHand.GetJTokenValue(token, "reply_count"), out CommentCount);
                            int.TryParse(jHand.GetJTokenValue(token, "retweet_count"), out RetweetCount);
                            int.TryParse(jHand.GetJTokenValue(token, "favorite_count"), out LikeCount);
                            var userId = jHand.GetJTokenValue(token, "user_id_str");
                            var tweetId = jHand.GetJTokenValue(token, "id_str");
                            var SourceTweetId = jHand.GetJTokenValue(token, "retweeted_status_result", "result", "rest_id");
                            var isRetweetedOwnTweet = !string.IsNullOrEmpty(SourceTweetId) && tweetId?.Trim() == SourceTweetId?.Trim();
                            var commenteduserid = jHand.GetJTokenValue(token, "in_reply_to_user_id_str");
                            iscomment = !string.IsNullOrEmpty(commenteduserid);
                            var CommentedTweetId = jHand.GetJTokenValue(token, "in_reply_to_status_id_str");
                            foreach (var details in userdetail)
                            {
                                var jToken = details.First();
                                var userID = jHand.GetJTokenValue(jToken, "id_str");
                                username = jHand.GetJTokenValue(jToken, "screen_name");
                                if (userId == userID)
                                {
                                    username = jHand.GetJTokenValue(jToken, "screen_name");
                                    following = jHand.GetJTokenValue(jToken, "following") == "True";
                                    followBack = jHand.GetJTokenValue(jToken, "followed_by") == "True";
                                    Ismute = jHand.GetJTokenValue(jToken, "muting") == "True";
                                    Isverfied = jHand.GetJTokenValue(jToken, "verified") == "True";
                                    Isprotected = jHand.GetJTokenValue(jToken, "protected") == "True";
                                    break;
                                }
                            }

                            UserTweetsDetail.Add(new TagDetails
                            {
                                Id = tweetId,
                                Username = username,
                                UserId = jHand.GetJTokenValue(token, "user_id_str"),
                                Caption = TwtText,
                                DateTime = dt,
                                TweetedTimeStamp = TwtTimeStamp,
                                Code = string.Join("\n", ListImagePath),
                                IsAlreadyLiked = jHand.GetJTokenValue(token, "favorited") == "True",
                                IsAlreadyRetweeted =
                                    jHand.GetJTokenValue(token, "retweeted") == "True", //data-my-retweet-id retweeted
                                CommentCount = CommentCount,
                                RetweetCount = RetweetCount,
                                LikeCount = LikeCount,
                                IsRetweet = jHand.GetJTokenValue(token, "retweeted") == "True",
                                FollowStatus = following,
                                FollowBackStatus = followBack,
                                IsComment = iscomment,
                                CommentedOnTweetId = CommentedTweetId,
                                OriginalTweetId= SourceTweetId,
                                IsRetweetedOwnTweet= isRetweetedOwnTweet,
                                // isComment ? jHand.GetJTokenValue(token, "conversation_id_str") : "",
                                // CommentedOnTweetOwner = isComment ? jHand.GetJTokenValue(token, "in_reply_to_screen_name") : "",
                                IsTweetContainedVideo = token["extended_entities"] != null
                                    ? token["extended_entities"]["media"][0]["video_info"] != null
                                    : false
                            });
                        }

                        MinPosition = Uri.EscapeDataString(MinPosition);

                        if (string.IsNullOrEmpty(response.Response.Trim()) && MinPosition == null || usersCount == 0)
                        {
                            HasMoreItems = "False";
                            hasmore = false;
                        }
                    }

                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }
            catch (StackOverflowException)
            {
                ListImagePath.Clear();
                UserTweetsDetail.Clear();
                GC.Collect();
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }

        }
    }
}