using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.AccountSetting.Activity;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response
{
    public class MediaCommentsResponseHandler : MediaInteractionResponseHandler
    {
        public bool following, followBack, Ismute, Isverfied, Isprotected, IsLiked, IsRetweeted;
        public List<string> ListImagePath = new List<string>();
        public int TweetCount;

        public MediaCommentsResponseHandler(IResponseParameter response) : base(response)
        {
            CommentList = new List<TagDetails>();
            if (!Success)
                return;
            try
            {
                var EmojiDict = TdUtility.GetAllEmojis();
                var Response = response.Response;
                var hasMore = string.Empty;
                MinPosition = Utilities.GetBetween(response.Response, "data-min-position=\"", "\"");
                if (Response.Contains("{\"globalObjects\":{\"tweets\":{"))
                {
                    var jsonHand = new Jsonhandler(Response);
                    var tweets = jsonHand.GetJToken("globalObjects", "tweets");
                    TweetCount = tweets.Children().Count();
                    var users = jsonHand.GetJToken("globalObjects", "users");
                    var cursorValue = jsonHand.GetJTokenValue(
                        jsonHand.GetJToken("timeline", "instructions", 0, "addEntries", "entries").Last, "content",
                        "operation", "cursor", "value");
                    MinPosition = Uri.EscapeDataString(cursorValue);
                    var TwtTimeStamp = 0;
                    var userName = string.Empty;
                    var profileImage = string.Empty;

                    foreach (var item in tweets)
                    {
                        ListImagePath.Clear();
                        var token = item.First;
                        var TwtText = jsonHand.GetJTokenValue(token, "full_text");
                        var ListImagePathtoken = jsonHand.GetJTokenOfJToken(token, "extended_entities", "media");
                        foreach (var Imagepath in ListImagePathtoken)
                        {
                            var images = jsonHand.GetJTokenValue(Imagepath, "media_url_https");
                            ListImagePath.Add(images);
                        }

                        if (TwtText.Contains($"pic.{Domain}/")) TwtText = Regex.Split(TwtText, $"pic.{Domain}/")[0];
                        var RetweetCount = jsonHand.GetJTokenValue(token, "retweet_count");
                        var dt = TdTimeStampUtility.ConvertTimestamp(jsonHand, token);
                        TwtTimeStamp = dt.ConvertToEpoch();
                        if (token["extended_entities"] != null
                            ? token["extended_entities"]["media"][0]["video_info"] != null
                            : false)
                        {
                            ListImagePath.Clear();
                            ListImagePath.Add(jsonHand.GetJTokenValue(item.First, "extended_entities", "media", 0,
                                "video_info", "variants", 1, "url"));
                        }

                        IsLiked = jsonHand.GetJTokenValue(token, "favorited") == "True";
                        IsRetweeted = jsonHand.GetJTokenValue(token, "retweeted") == "True";
                        var CommentCount = jsonHand.GetJTokenValue(token, "reply_count");
                        var TweetId = jsonHand.GetJTokenValue(token, "id_str");
                        var LikeCount = jsonHand.GetJTokenValue(token, "favorite_count");
                        var Userid = jsonHand.GetJTokenValue(token, "user_id_str");
                        foreach (var userdetail in users)
                        {
                            var jToken = userdetail.First;
                            var userId = jsonHand.GetJTokenValue(jToken, "id_str");
                            if (userId == Userid)
                            {
                                userName = jsonHand.GetJTokenValue(jToken, "screen_name");
                                profileImage = jsonHand.GetJTokenValue(jToken, "profile_image_url");
                                following = jsonHand.GetJTokenValue(jToken, "following") == "True";
                                followBack = jsonHand.GetJTokenValue(jToken, "followed_by") == "True";
                                Ismute = jsonHand.GetJTokenValue(jToken, "muting") == "True";
                                Isverfied = jsonHand.GetJTokenValue(jToken, "verified") == "True";
                                Isprotected = jsonHand.GetJTokenValue(jToken, "protected") == "True";
                                break;
                            }
                        }

                        CommentList.Add(new TagDetails
                        {
                            Id = TweetId,
                            Username = userName,
                            UserId = Userid,
                            Caption = HttpUtility.HtmlDecode(TwtText),
                            DateTime = dt,
                            TweetedTimeStamp = TwtTimeStamp,
                            Code = string.Join("\n", ListImagePath),
                            IsAlreadyLiked = IsLiked,
                            IsAlreadyRetweeted = IsRetweeted,
                            ProfilePicUrl = profileImage,
                            //IsRetweet = item.Contains("Icon--retweeted"),
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

                    if (TweetCount > 0 || !string.IsNullOrEmpty(MinPosition))
                        HasMoreResults = true;
                }
                else if (Response.Contains("{\"data\":{\"threaded_conversation_with_injections_v2"))
                {
                    try
                    {
                        var jsonHand = new Jsonhandler(Response);
                        var tweets = jsonHand.GetJToken("data",
                                        "threaded_conversation_with_injections_v2",
                                        "instructions",
                                        0,
                                        "entries");
                        if (tweets is null || !tweets.HasValues)
                        {
                            var nodes = jsonHand.GetJToken("data",
                                                "threaded_conversation_with_injections_v2",
                                                "instructions");
                            if (nodes != null && nodes.HasValues)
                            {
                                foreach (var node in nodes)
                                {
                                    var type = jsonHand.GetJTokenValue(node, "type");
                                    if (type == "TimelineAddEntries")
                                    {
                                        tweets = jsonHand.GetJTokenOfJToken(node, "entries");
                                        break;
                                    }
                                }
                            }
                        }
                        TweetCount = tweets.Count();
                        var cursorValue = jsonHand.GetJTokenValue(tweets.Last(),
                                            "content",
                                            "itemContent",
                                            "value");
                        cursorValue = string.IsNullOrEmpty(cursorValue) ? jsonHand.GetJTokenValue(tweets.Last(),
                                            "content",
                                            "value") : cursorValue;
                        MinPosition = Uri.EscapeDataString(cursorValue);
                        var TwtTimeStamp = 0;
                        var userName = string.Empty;
                        var profileImage = string.Empty;

                        foreach (var tweet in tweets)
                        {
                            // Post Details
                            ListImagePath.Clear();
                            var entryId = jsonHand.GetJTokenValue(tweet, "entryId");
                            if (entryId.Contains("conversationthread"))
                            {
                                var token = jsonHand.GetJTokenOfJToken(tweet,
                                            "content",
                                            "items",0,
                                            "item",
                                            "itemContent",
                                            "tweet_results",
                                            "result");

                                var tweetId = jsonHand.GetJTokenValue(token, "rest_id");
                                var bookmarkCount = jsonHand.GetJTokenValue(token, "legacy", "bookmark_count");
                                var bookmarked = jsonHand.GetJTokenValue(token, "legacy", "bookmarked");
                                var createdAt = TdTimeStampUtility.ConvertTimestamp(jsonHand, jsonHand.GetJTokenOfJToken(token, "legacy"));
                                TwtTimeStamp = createdAt.ConvertToEpoch();
                                var conversationId = jsonHand.GetJTokenValue(token, "legacy", "conversation_id_str");
                                var ListImagePathtoken = jsonHand.GetJTokenOfJToken(token, "legacy", "extended_entities", "media");
                                foreach (var Imagepath in ListImagePathtoken)
                                {
                                    var images = jsonHand.GetJTokenValue(Imagepath, "media_url_https");
                                    ListImagePath.Add(images);
                                }
                                var favoriteCount = jsonHand.GetJTokenValue(token, "legacy", "favorite_count");
                                var favorited = jsonHand.GetJTokenValue(token, "legacy", "favorited");
                                var fullText = jsonHand.GetJTokenValue(token, "legacy", "full_text");
                                var isQuoteStatus = jsonHand.GetJTokenValue(token, "legacy", "is_quote_status");
                                var lang = jsonHand.GetJTokenValue(token, "legacy", "lang");
                                var possiblySensitive = jsonHand.GetJTokenValue(token, "legacy", "possibly_sensitive");
                                var possiblySensitiveEditable = jsonHand.GetJTokenValue(token, "legacy", "possibly_sensitive_editable");
                                var quoteCount = jsonHand.GetJTokenValue(token, "legacy", "quote_count");
                                var replyCount = jsonHand.GetJTokenValue(token, "legacy", "reply_count");
                                var retweetCount = jsonHand.GetJTokenValue(token, "legacy", "retweet_count");
                                var retweeted = jsonHand.GetJTokenValue(token, "legacy", "retweeted");
                                var userId = jsonHand.GetJTokenValue(token, "legacy", "user_id_str");
                                var viewsCount = jsonHand.GetJTokenValue(token, "views", "count");

                                // User Details
                                var userToken = jsonHand.GetJTokenOfJToken(token, "core", "user_results", "result");
                                var hasGraduatedAccess = jsonHand.GetJTokenValue(userToken, "has_graduated_access");
                                var isBlueVerified = jsonHand.GetJTokenValue(userToken, "is_blue_verified");
                                var profileImageShape = jsonHand.GetJTokenValue(userToken, "profile_image_shape");
                                var canDm = jsonHand.GetJTokenValue(userToken, "legacy", "can_dm");
                                var canMediaTag = jsonHand.GetJTokenValue(userToken, "legacy", "can_media_tag");
                                var userCreatedAt = TdTimeStampUtility.ConvertTimestamp(jsonHand,
                                                        jsonHand.GetJTokenOfJToken(userToken, "legacy"));
                                var defaultProfile = jsonHand.GetJTokenValue(userToken, "legacy", "default_profile");
                                var defaultProfileImage = jsonHand.GetJTokenValue(userToken, "legacy", "default_profile_image");
                                var userDescription = jsonHand.GetJTokenValue(userToken, "legacy", "description");
                                var fastFollowersCount = jsonHand.GetJTokenValue(userToken, "legacy", "fast_followers_count");
                                var favouritesCount = jsonHand.GetJTokenValue(userToken, "legacy", "favourites_count");
                                var followersCount = jsonHand.GetJTokenValue(userToken, "legacy", "followers_count");
                                var friendsCount = jsonHand.GetJTokenValue(userToken, "legacy", "friends_count");
                                var hasCustomTimelines = jsonHand.GetJTokenValue(userToken, "legacy", "has_custom_timelines");
                                var isTranslator = jsonHand.GetJTokenValue(userToken, "legacy", "is_translator");
                                var listedCount = jsonHand.GetJTokenValue(userToken, "legacy", "listedCount");
                                var location = jsonHand.GetJTokenValue(userToken, "legacy", "location");
                                var mediaCount = jsonHand.GetJTokenValue(userToken, "legacy", "media_count");
                                var name = jsonHand.GetJTokenValue(userToken, "legacy", "name");
                                var pinnedTweetId = jsonHand.GetJTokenValue(userToken, "legacy", "pinned_tweet_ids_str", 0);
                                var userPossiblySensitive = jsonHand.GetJTokenValue(userToken, "legacy", "possibly_sensitive");
                                var profileBannerUrl = jsonHand.GetJTokenValue(userToken, "legacy", "profile_banner_url");
                                profileImage = jsonHand.GetJTokenValue(userToken, "legacy", "profile_image_url_https");
                                userName = jsonHand.GetJTokenValue(userToken, "legacy", "screen_name");
                                var statusesCount = jsonHand.GetJTokenValue(userToken, "legacy", "statuses_count");
                                var url = jsonHand.GetJTokenValue(userToken, "legacy", "url");
                                var verified = jsonHand.GetJTokenValue(userToken, "legacy", "verified");
                                var wantRetweets = jsonHand.GetJTokenValue(userToken, "legacy", "want_retweets");
                                var superFolloweligible = jsonHand.GetJTokenValue(userToken, "super_follow_eligible");
                                var verifiedPhoneStatus = jsonHand.GetJTokenValue(userToken, "verified_phone_status");

                                CommentList.Add(new TagDetails
                                {
                                    Id = tweetId,
                                    Username = userName,
                                    UserId = userId,
                                    Caption = HttpUtility.HtmlDecode(fullText),
                                    DateTime = createdAt,
                                    TweetedTimeStamp = TwtTimeStamp,
                                    Code = string.Join("\n", ListImagePath),
                                    IsAlreadyLiked = favorited == "True",
                                    IsAlreadyRetweeted = retweeted == "True",
                                    ProfilePicUrl = profileImage,
                                    //IsRetweet = item.Contains("Icon--retweeted"),
                                    IsVerified = verified == "True",
                                    IsMuted = Ismute,
                                    CommentCount = string.IsNullOrEmpty(replyCount) ? 0 : int.Parse(replyCount),
                                    RetweetCount = string.IsNullOrEmpty(retweetCount) ? 0 : int.Parse(retweetCount),
                                    LikeCount = string.IsNullOrEmpty(favoriteCount) ? 0 : int.Parse(favoriteCount),
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
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }
                    if (TweetCount > 0 || !string.IsNullOrEmpty(MinPosition))
                        HasMoreResults = true;
                }
                else
                {
                    if (!Response.Contains("<!DOCTYPE html>"))
                    {
                        var JsonObject = JObject.Parse(response.Response)["descendants"];
                        MinPosition = JsonObject["min_position"].ToString();
                        hasMore = JsonObject["has_more_items"].ToString();
                        Response = JsonObject["items_html"].ToString();
                    }


                    // response for show all comments

                    var UserDetails =
                        HtmlAgilityHelper.getListInnerHtmlFromClassName(Response,
                            "js-stream-item stream-item stream-item");
                    CommentList = new List<TagDetails>();
                    var itemHtmlDoc = new HtmlDocument();
                    foreach (var item in UserDetails)
                    {
                        itemHtmlDoc.LoadHtml(item);
                        var TwtText =
                            HtmlAgilityHelper.getStringInnerHtmlFromClassName(item, "js-tweet-text-container",
                                itemHtmlDoc);
                        TwtText = TdUtility.ReplaceImageWithEmogis(TwtText, EmojiDict);
                        if (TwtText.Contains($"pic.{Domain}/")) TwtText = Regex.Split(TwtText, $"pic.{Domain}/")[0];
                        var TwtTimeStamp = 0;
                        int.TryParse(
                            HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item, "tweet-timestamp",
                                "data-time", itemHtmlDoc),
                            out TwtTimeStamp);
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
                        CommentList.Add(new TagDetails
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
                            IsMuted = item.Contains("muting"),
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
                    //  CommentList = CommentList.Distinct().ToList();

                    if (string.IsNullOrEmpty(MinPosition.Trim()))
                        try
                        {
                            var match = Regex.Match(Response, "data-min-position=\"(.*?)\"");
                            if (match.Success)
                                MinPosition = match.Groups[1].ToString();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog("Not Found min position.");
                            // DominatorHouseCore.LogHelper.GlobusLogHelper.log.Error("Not Found min position.");
                        }

                    if (hasMore.Equals("True") || !string.IsNullOrEmpty(MinPosition))
                        HasMoreResults = true;
                }
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        public List<TagDetails> CommentList { get; }
    }
}