using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using TwtDominatorCore.TDEnums;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response
{
    public class UserDetailsResponseHandler : TdBaseHtmlResponseHandler
    {
        private static readonly Regex MuteCssRegex = new Regex("user-actions btn-group(.*?)following(.*?)muting");

        private static readonly Regex BioEmojRegex =
            new Regex("<img class=.Emoji Emoji--forText(.*?)alt=\"(.*?)\"(.*?)>");

        private static readonly Regex BioEmojReplace1Regex = new Regex("<.*?>");
        private static readonly Regex BioEmojReplace2Regex = new Regex(@"\s+");
        public bool following, followBack, Ismute, Isverfied, Isprotected, IsLiked, IsRetweeted;
        public List<string> ListImagePath = new List<string>();

        public UserDetailsResponseHandler(IResponseParameter response, bool isScrapeAllTweet) : base(response)
        {
            UserDetail = new TwitterUser();

            if (!Success)
            {
                if (Issue?.Error != null)
                    switch (Issue?.Error)
                    {
                        case TwitterError.PageNotFound:
                        case TwitterError.Challenge_AccountSuspended:
                            UserDetail.UserStatus = TdConstants.AccountSuspended;
                            break;
                    }

                return;
            }

            var emojiDict = TdUtility.GetAllEmojis();
            var respTextHtmlDoc = new HtmlDocument();
            UserDetail = new TwitterUser();
            var respText = response.Response;
            var hasMoreItems = string.Empty;
            try
            {
                if (respText.Contains("<!DOCTYPE html>")) //no need to scrape bio for json response
                {
                    if (GetAccountStatus(respText))
                        return;

                    #region Scrape Users Bio Info

                    var htmlDoc = new HtmlDocument();

                    respTextHtmlDoc.LoadHtml(respText);

                    //var blockedTag = HtmlAgilityHelper.getStringInnerTextFromClassName(respText,
                    //    "ProfileWarningTimeline-heading");
                    //if (blockedTag.Contains("You blocked"))
                    //    UserDetail.UserStatus = TdConstants.AccountBlockedByYou;


                    MinPosition = Utilities.GetBetween(respText, "data-min-position=\"", "\"");
                    var profileNav =
                        HtmlAgilityHelper.getStringInnerHtmlFromClassName(respText, "ProfileNav", respTextHtmlDoc);
                    var profileAvtar =
                        HtmlAgilityHelper.getStringInnerHtmlFromClassName(respText, "ProfileAvatar", respTextHtmlDoc);
                    var profileHeader =
                        HtmlAgilityHelper.getStringInnerHtmlFromClassName(respText, "ProfileHeaderCard",
                            respTextHtmlDoc);
                    var followStatusCheck =
                        HtmlAgilityHelper.getStringInnerHtmlFromClassName(respText, "ProfileNav-item--userActions",
                            respTextHtmlDoc);

                    // profileNav
                    htmlDoc.LoadHtml(profileNav);
                    var tweetCount =
                        HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(profileNav, "ProfileNav-item--tweets",
                            "data-count", htmlDoc);
                    var followingCount =
                        HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(profileNav,
                            "ProfileNav-item--following", "data-count", htmlDoc);
                    var followersCount =
                        HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(profileNav,
                            "ProfileNav-item--followers", "data-count", htmlDoc);
                    var likesCount =
                        HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(profileNav,
                            "ProfileNav-item--favorites", "data-count", htmlDoc);

                    //profileHeader
                    htmlDoc.LoadHtml(profileHeader);

                    var userName =
                        HtmlAgilityHelper.getStringInnerTextFromClassName(profileHeader,
                            "ProfileHeaderCard-screennameLink", htmlDoc);
                    var joiningDate =
                        HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(profileHeader,
                            "ProfileHeaderCard-joinDate", "title", htmlDoc);

                    var profileHeaderCardName =
                        HtmlAgilityHelper.getStringInnerHtmlFromClassName(profileHeader, "ProfileHeaderCard-name",
                            htmlDoc);
                    var location =
                        HtmlAgilityHelper.getStringInnerTextFromClassName(profileHeader, "ProfileHeaderCard-location",
                            htmlDoc);
                    var bio = HtmlAgilityHelper.getStringInnerTextFromClassName(profileHeader, "ProfileHeaderCard-bio",
                        htmlDoc);

                    #region Get Bio with their emoji

                    try
                    {
                        bio = HtmlAgilityHelper.getStringInnerHtmlFromClassName(profileHeader, "ProfileHeaderCard-bio",
                            htmlDoc);
                        var matches = BioEmojRegex.Matches(bio);
                        foreach (Match match in matches)
                            bio = bio.Replace(match.Groups[0].ToString(), match.Groups[2].ToString());

                        bio = BioEmojReplace1Regex.Replace(bio, string.Empty).Replace("\n", " ").Replace(@"\s+", " ");
                        bio = BioEmojReplace2Regex.Replace(bio, " ");
                        bio = WebUtility.HtmlDecode(bio).Trim();
                    }
                    catch (Exception exception)
                    {
                        if (!string.IsNullOrEmpty(userName))
                            exception.DebugLog($"unable to get bio for {userName}");
                    }

                    #endregion

                    UserDetail.UserId = Utilities.GetBetween(respText, "role=\"navigation\" data-user-id=\"", "\"");
                    UserDetail.Username = userName.Replace("@", string.Empty);
                    UserDetail.FullName =
                        WebUtility.HtmlDecode(HtmlAgilityHelper.getStringInnerTextFromClassName(profileHeaderCardName,
                            "ProfileHeaderCard-nameLink"));
                    UserDetail.UserLocation = string.IsNullOrEmpty(location) ? "" : WebUtility.HtmlDecode(location);
                    UserDetail.UserBio = string.IsNullOrEmpty(bio) ? "" : WebUtility.HtmlDecode(bio);
                    UserDetail.WebPageURL =
                        HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(profileHeader, "ProfileHeaderCard-url",
                            "title", htmlDoc);
                    UserDetail.ProfilePicUrl =
                        HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(profileAvtar,
                            "ProfileAvatar-container", "src");
                    UserDetail.JoiningDate = TdUtility.GetDateFormatFromString(joiningDate);
                    UserDetail.FollowBackStatus = profileHeader.Contains("FollowStatus");
                    UserDetail.FollowStatus = !followStatusCheck.Contains("not-following") &&
                                              followStatusCheck.Contains("following");
                    UserDetail.IsPrivate = profileHeaderCardName.Contains("Icon--protected");

                    UserDetail.IsVerified = profileHeaderCardName.Contains("/help/verified");

                    #region  check IsMute

                    try
                    {
                        var match = MuteCssRegex.Match(respText);
                        if (match.Success)
                        {
                            var checkMute = match.Groups[2].ToString().Trim();
                            UserDetail.IsMuted = !checkMute.Equals("not-");
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.ErrorLog();
                    }

                    #endregion

                    if (string.IsNullOrEmpty(UserDetail.UserId))
                        UserDetail.UserId = HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(respText,
                            "ProfileNav-item--userActions", "data-user-id", respTextHtmlDoc);
                    if (!string.IsNullOrEmpty(UserDetail?.ProfilePicUrl) &&
                        !UserDetail.ProfilePicUrl.Contains("default_profile_images"))
                        UserDetail.HasProfilePic = true;
                    else
                        UserDetail.ProfilePicUrl = string.Empty;
                    try
                    {
                        UserDetail.TweetsCount = string.IsNullOrEmpty(tweetCount) ? 0 : int.Parse(tweetCount);
                        UserDetail.FollowersCount =
                            string.IsNullOrEmpty(followersCount) ? 0 : int.Parse(followersCount);
                        UserDetail.FollowingsCount =
                            string.IsNullOrEmpty(followingCount) ? 0 : int.Parse(followingCount);
                        UserDetail.LikesCount = string.IsNullOrEmpty(likesCount) ? 0 : int.Parse(likesCount);
                    }
                    catch (Exception exception)
                    {
                        if (!string.IsNullOrEmpty(UserDetail?.Username))
                            exception.DebugLog($"Error in tweet count {UserDetail.Username}");
                    }

                    #endregion
                }
                else
                {
                    var userName = Utilities.GetBetween(response.Response, "", "\"");
                    var UsernameLength = userName.Length + 1;
                    var Response = response.Response.Substring(UsernameLength);
                    var obj = handler.ParseJsonToJObject(Response);
                    var jsonObject = handler.GetJTokenOfJToken(obj, "inner");
                    if (jsonObject == null || !jsonObject.HasValues)
                    {
                        NewUIJsonResponse(Response, userName);
                    }
                    else
                    {
                        hasMoreItems = handler.GetJTokenValue(jsonObject, "has_more_items");
                        respText = handler.GetJTokenValue(jsonObject, "items_html");
                        MinPosition = handler.GetJTokenValue(jsonObject, "min_position");
                    }
                }


                #region Scrape users Tweet

                var listTweetDetails =
                    HtmlAgilityHelper.getListInnerHtmlFromClassName(respText, "js-stream-item stream-item stream-item",
                        respTextHtmlDoc);
                if (!UserDetail.IsPrivate && !respText.Contains("ProfilePage-emptyModule"))
                {
                    foreach (var item in listTweetDetails)
                    {
                        var itemHtmlDoc = new HtmlDocument();
                        itemHtmlDoc.LoadHtml(item);

                        #region Scrape all tweets

                        int timestamp, commentCount, retweetCount, likeCount;
                        if (isScrapeAllTweet)
                        {
                            var listImagePath =
                                HtmlAgilityHelper.getListValueWithAttributeNameFromInnerHtml(item,
                                    "AdaptiveMedia-photoContainer", "src", itemHtmlDoc);
                            int.TryParse(
                                HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item, "tweet-timestamp",
                                    "data-time", itemHtmlDoc), out timestamp);
                            var profileImage = HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item,
                                "js-action-profile js-user-profile-link", "src", itemHtmlDoc);
                            var twtText =
                                HtmlAgilityHelper.getStringInnerHtmlFromClassName(item, "js-tweet-text-container",
                                    itemHtmlDoc);
                            twtText = TdUtility.ReplaceImageWithEmogis(twtText, emojiDict);
                            if (twtText.Contains($"pic.{Domain}/"))
                                twtText = twtText.Split(new[] { $"pic.{Domain}/" },
                                    StringSplitOptions.RemoveEmptyEntries)[0];
                            int.TryParse(
                                HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item,
                                    "ProfileTweet-action--favorite", "data-tweet-stat-count", itemHtmlDoc),
                                out likeCount);
                            int.TryParse(
                                HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item,
                                    "ProfileTweet-action--retweet", "data-tweet-stat-count", itemHtmlDoc),
                                out retweetCount);
                            int.TryParse(
                                HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item,
                                    "ProfileTweet-action--reply", "data-tweet-stat-count", itemHtmlDoc),
                                out commentCount);
                            var singleTag = new TagDetails
                            {
                                UserId = HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item,
                                    "stream-item-header", "data-user-id", itemHtmlDoc),
                                Username = HtmlAgilityHelper
                                    .getStringInnerTextFromClassName(item, "username u-dir u-textTruncate")
                                    .Replace("@", ""),
                                Id = Utilities.GetBetween(item, "/status/", "\""),
                                Code = string.Join("\n", listImagePath),
                                Caption = HttpUtility.HtmlDecode(twtText),
                                TweetedTimeStamp = timestamp,
                                IsAlreadyLiked = item.Contains("favorited"),
                                IsAlreadyRetweeted = item.Contains("data-my-retweet-id"),
                                //  IsRetweet = item.Contains("data-my-retweet-id"),
                                IsRetweet = item.Contains("Icon--retweeted"),
                                LikeCount = likeCount,
                                IsMuted = item.Contains("   muting "),
                                CommentCount = commentCount,
                                RetweetCount = retweetCount,
                                FollowStatus = item.Contains("data-you-follow=\"true\""),
                                FollowBackStatus = item.Contains("data-follows-you=\"true\""),
                                IsPrivate = item.Contains("Icon--protected"),
                                HasProfilePic = profileImage.Contains("default_profile_images"),
                                IsVerified = item.Contains("Icon--verified"),
                                IsTweetContainedVideo = item.Contains("AdaptiveMedia-videoContainer")
                            };
                            UserDetail.ListTag.Add(singleTag);
                        }

                        #endregion

                        #region Scrape latest tweet

                        else
                        {
                            if (item.Contains("js-retweet-text") || item.Contains("js-pinned-text"))
                                continue;
                            var profileImage = HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item,
                                "js-action-profile js-user-profile-link", "src", itemHtmlDoc);
                            int.TryParse(
                                HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item, "tweet-timestamp",
                                    "data-time", itemHtmlDoc), out timestamp);
                            UserDetail.LastTweetedDaycount = TdUtility.GetDateDifferenceFromTimeStamp(timestamp);
                            var twtText =
                                HtmlAgilityHelper.getStringInnerHtmlFromClassName(item, "js-tweet-text-container",
                                    itemHtmlDoc);
                            twtText = TdUtility.ReplaceImageWithEmogis(twtText, emojiDict);
                            if (twtText.Contains($"pic.{Domain}/"))
                                twtText = twtText.Split(new[] { $"pic.{Domain}/" },
                                    StringSplitOptions.RemoveEmptyEntries)[0];
                            int.TryParse(
                                HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item,
                                    "ProfileTweet-action--favorite", "data-tweet-stat-count", itemHtmlDoc),
                                out likeCount);
                            int.TryParse(
                                HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item,
                                    "ProfileTweet-action--retweet", "data-tweet-stat-count", itemHtmlDoc),
                                out retweetCount);
                            int.TryParse(
                                HtmlAgilityHelper.getValueWithAttributeNameFromInnerHtml(item,
                                    "ProfileTweet-action--reply", "data-tweet-stat-count", itemHtmlDoc),
                                out commentCount);
                            UserDetail.TagDetail = new TagDetails
                            {
                                UserId = UserDetail.UserId,
                                Username = UserDetail.Username,
                                Id = Utilities.GetBetween(item, "/status/", "\""),
                                Caption = HttpUtility.HtmlDecode(twtText),
                                TweetedTimeStamp = timestamp,
                                LikeCount = likeCount,
                                CommentCount = commentCount,
                                RetweetCount = retweetCount,
                                FollowStatus = item.Contains("data-you-follow=\"true\""),
                                FollowBackStatus = item.Contains("data-follows-you=\"true\""),
                                IsPrivate = item.Contains("Icon--protected"),
                                HasProfilePic = profileImage.Contains("default_profile_images"),
                                IsTweetContainedVideo = item.Contains("AdaptiveMedia-videoContainer")
                            };
                            break;
                        }

                        #endregion
                    }

                    if (isScrapeAllTweet)
                    {
                        UserDetail.TagDetail = UserDetail.ListTag.FirstOrDefault(x => !x.IsRetweet);
                        UserDetail.LastTweetedDaycount = UserDetail.TagDetail != null ? UserDetail.TagDetail.DaysCount :0;
                    }
                }

                #endregion

                if (!string.IsNullOrEmpty(MinPosition))
                    Hasmore = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public TwitterUser UserDetail { get; set; }
        public bool Hasmore { get; set; }
        public string MinPosition { get; set; }

        private void NewUIJsonResponse(string response, string Username)
        {
            if (response.Contains("{\"globalObjects\":{\"") || response.StartsWith("{\"data\":{\"user\":"))
            {
                BrowserResponsehandler(response, Username);
            }
            else
            {
                UserDetail = new TwitterUser();
                var jHand = new Jsonhandler(response);
                var legacy = new Jsonhandler(jHand.GetJToken("data", "user", "legacy").ToString());
                var dt = TdTimeStampUtility.ConvertTimestamp(jHand, jHand.GetJToken("data", "user", "legacy"));
                var TwtTimeStamp = dt.ConvertToEpoch();
                UserDetail = new TwitterUser
                {
                    UserId = jHand.GetElementValue("data", "user", "rest_id"),
                    UserBio = legacy.GetElementValue("description"),
                    Username = legacy.GetElementValue("screen_name"),
                    FollowersCount = int.Parse(legacy.GetElementValue("followers_count")),
                    FollowingsCount = int.Parse(legacy.GetElementValue("friends_count")),
                    FollowBackStatus = legacy.GetElementValue("followed_by") == "True",
                    FollowStatus = legacy.GetElementValue("following") == "True",
                    UserLocation = legacy.GetElementValue("location"),
                    LikesCount = int.Parse(legacy.GetElementValue("favourites_count")),
                    FullName = legacy.GetElementValue("name"),
                    IsVerified = legacy.GetElementValue("verified") == "True",
                    IsMuted = legacy.GetElementValue("muting") == "True",
                    TweetsCount = int.Parse(legacy.GetElementValue("statuses_count")),
                    IsPrivate = legacy.GetElementValue("protected") == "True",
                    HasProfilePic = legacy.GetElementValue("profile_image_url_https") != null,
                    ProfilePicUrl = legacy.GetElementValue("profile_image_url_https"),
                    JoiningDate = dt
                };
            }
        }

        private void BrowserResponsehandler(string response, string Username)
        {
            try
            {
                if (response.StartsWith("{\"data\":{\"user\":"))
                {
                    var jsonHand = new Jsonhandler(response);

                    var bottomCursor = jsonHand.GetJToken("data", "user", "result", "timeline_v2", "timeline", "instructions")?.Last();

                    var cursorValue =
                        jsonHand.GetJTokenValue(
                            jsonHand.GetJTokenOfJToken(bottomCursor,"entries").Last(), "content", "value") ?? string.Empty;
                    MinPosition = Uri.EscapeDataString(cursorValue);

                    //var pinnedTweet = jsonHand.GetJToken("data", "user", "result", "timeline", "timeline", "instructions", 1, "entry");
                    var timelineTweets = jsonHand.GetJTokenOfJToken(bottomCursor,"entries");

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
                            tweetDetails = tweetDetails is null || !tweetDetails.HasValues ? jsonHand.GetJToken("content", "itemContent", "tweet_results", "result", "tweet", "legacy") : tweetDetails;
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
                            var HaveVideo = jsonHand.GetJTokenOfJToken(tweetDetails, "extended_entities", "media",0, "video_info");
                            if (HaveVideo != null && HaveVideo.HasValues)
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
                            IsLiked = !IsLiked ? jsonHand.GetJTokenValue(tweetDetails, "retweeted_status_result", "result", "legacy", "favorited")=="True" : IsLiked;
                            IsRetweeted = jsonHand.GetJTokenValue(tweetDetails, "retweeted") == "True";
                            IsRetweeted = !IsRetweeted ? jsonHand.GetJTokenValue(tweetDetails, "retweeted_status_result", "result", "legacy", "retweeted")=="True" : IsRetweeted;
                            var CommentCount = jsonHand.GetJTokenValue(tweetDetails, "reply_count");
                            CommentCount = string.IsNullOrEmpty(CommentCount)||CommentCount=="0" ? jsonHand.GetJTokenValue(tweetDetails, "retweeted_status_result", "result", "legacy", "reply_count") : CommentCount;
                            var TweetId = jsonHand.GetJTokenValue(tweetDetails, "id_str");
                            var LikeCount = jsonHand.GetJTokenValue(tweetDetails, "favorite_count");
                            LikeCount = string.IsNullOrEmpty(LikeCount) || LikeCount == "0" ? jsonHand.GetJTokenValue(tweetDetails, "retweeted_status_result", "result", "legacy", "favorite_count") : LikeCount;
                            var Userid = jsonHand.GetJTokenValue(tweetDetails, "user_id_str");
                            var originalTweetId=string.Empty;
                            var isRetweeted = !string.IsNullOrEmpty(originalTweetId = jsonHand.GetJTokenValue(tweetDetails, "retweeted_status_result", "result", "rest_id"));
                            var userDetails = jsonHand.GetJToken("content", "itemContent", "tweet_results", "result", "core", "user_results", "result", "legacy");
                            userDetails = userDetails is null || !userDetails.HasValues ? jsonHand.GetJToken("content", "itemContent", "tweet_results", "result", "tweet", "core", "user_results", "result", "legacy") : userDetails;
                            userName = jsonHand.GetJTokenValue(userDetails, "screen_name");
                            profileImage = jsonHand.GetJTokenValue(userDetails, "profile_image_url_https");
                            following = jsonHand.GetJTokenValue(userDetails, "following") == "True";
                            followBack = jsonHand.GetJTokenValue(userDetails, "followed_by") == "True";
                            Ismute = jsonHand.GetJTokenValue(userDetails, "muting") == "True";
                            var isVerified = jsonHand.GetJTokenValue(tweet, "content", "itemContent", "tweet_results", "result", "core", "user_results", "result", "is_blue_verified");
                            Isverfied = string.IsNullOrEmpty(isVerified)? jsonHand.GetJTokenValue(tweet,"content", "itemContent", "tweet_results", "result", "tweet", "core", "user_results", "result", "is_blue_verified") == "True":isVerified=="True";
                            Isprotected = jsonHand.GetJTokenValue(userDetails, "protected") == "True";
                            var username = jsonHand.GetJTokenValue(userDetails, "screen_name");
                            if (username == Username)
                            {
                                var userBio = jsonHand.GetJTokenValue(userDetails, "description");
                                var followerCount = int.Parse(jsonHand.GetJTokenValue(userDetails, "followers_count"));
                                var FollowingCount = int.Parse(jsonHand.GetJTokenValue(userDetails, "friends_count"));
                                var userLocation = jsonHand.GetJTokenValue(userDetails, "location");
                                var likeCount = int.Parse(jsonHand.GetJTokenValue(userDetails, "favourites_count"));
                                var Fullname = jsonHand.GetJTokenValue(userDetails, "name");
                                var tweetCount = int.Parse(jsonHand.GetJTokenValue(userDetails, "statuses_count"));
                                var hasProfilePic = jsonHand.GetJTokenValue(userDetails, "profile_image_url_https") != null;
                                var userJoinedDate = TdTimeStampUtility.ConvertTimestamp(jsonHand, userDetails);
                                UserDetail.UserId = Userid;
                                UserDetail.UserBio = userBio;
                                UserDetail.Username = userName;
                                UserDetail.FollowersCount = followerCount;
                                UserDetail.FollowingsCount = FollowingCount;
                                UserDetail.FollowBackStatus = followBack;
                                UserDetail.FollowStatus = following;
                                UserDetail.UserLocation = userLocation;
                                UserDetail.LikesCount = likeCount;
                                UserDetail.FullName = Fullname;
                                UserDetail.IsVerified = Isverfied;
                                UserDetail.IsMuted = Ismute;
                                UserDetail.TweetsCount = tweetCount;
                                UserDetail.IsPrivate = Isprotected;
                                UserDetail.HasProfilePic = hasProfilePic;
                                UserDetail.ProfilePicUrl = profileImage;
                                UserDetail.JoiningDate = userJoinedDate;
                            }


                            UserDetail.ListTag.Add(new TagDetails
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
                                IsRetweet = isRetweeted,
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
                                IsTweetContainedVideo = HaveVideo != null && HaveVideo.HasValues
                            });
                        }
                        else if (entryId.StartsWith("homeConversation"))
                        {
                            var ConversationTweets = jsonHand.GetJToken("content", "items");
                            foreach (var eachConvTwt in ConversationTweets)
                            {
                                jsonHand = new Jsonhandler(eachConvTwt);
                                var tweetDetails = jsonHand.GetJToken("item", "itemContent", "tweet_results", "result", "legacy");
                                tweetDetails = tweetDetails is null || !tweetDetails.HasValues ? jsonHand.GetJToken("content", "itemContent", "tweet_results", "result", "tweet", "legacy") : tweetDetails;
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

                                var userDetails = jsonHand.GetJToken("item", "itemContent", "tweet_results", "result", "core", "user_results", "result", "legacy");
                                userDetails = userDetails is null || !userDetails.HasValues ? jsonHand.GetJToken("content", "itemContent", "tweet_results", "result", "tweet", "core", "user_results", "result", "legacy") : userDetails;
                                userName = jsonHand.GetJTokenValue(userDetails, "screen_name");
                                profileImage = jsonHand.GetJTokenValue(userDetails, "profile_image_url_https");
                                following = jsonHand.GetJTokenValue(userDetails, "following") == "True";
                                followBack = jsonHand.GetJTokenValue(userDetails, "followed_by") == "True";
                                Ismute = jsonHand.GetJTokenValue(userDetails, "muting") == "True";
                                var isVerified = jsonHand.GetJTokenValue(tweet, "content", "itemContent", "tweet_results", "result", "core", "user_results", "result", "is_blue_verified");
                                Isverfied = string.IsNullOrEmpty(isVerified) ? jsonHand.GetJTokenValue(tweet, "content", "itemContent", "tweet_results", "result", "tweet", "core", "user_results", "result", "is_blue_verified") == "True" : isVerified == "True";
                                Isprotected = jsonHand.GetJTokenValue(userDetails, "protected") == "True";
                                var username = jsonHand.GetJTokenValue(userDetails, "screen_name");
                                if (username == Username)
                                {
                                    var userBio = jsonHand.GetJTokenValue(userDetails, "description");
                                    var followerCount = int.Parse(jsonHand.GetJTokenValue(userDetails, "followers_count"));
                                    var FollowingCount = int.Parse(jsonHand.GetJTokenValue(userDetails, "friends_count"));
                                    var userLocation = jsonHand.GetJTokenValue(userDetails, "location");
                                    var likeCount = int.Parse(jsonHand.GetJTokenValue(userDetails, "favourites_count"));
                                    var Fullname = jsonHand.GetJTokenValue(userDetails, "name");
                                    var tweetCount = int.Parse(jsonHand.GetJTokenValue(userDetails, "statuses_count"));
                                    var hasProfilePic = jsonHand.GetJTokenValue(userDetails, "profile_image_url_https") != null;
                                    UserDetail.UserId = Userid;
                                    UserDetail.UserBio = userBio;
                                    UserDetail.Username = userName;
                                    UserDetail.FollowersCount = followerCount;
                                    UserDetail.FollowingsCount = FollowingCount;
                                    UserDetail.FollowBackStatus = followBack;
                                    UserDetail.FollowStatus = following;
                                    UserDetail.UserLocation = userLocation;
                                    UserDetail.LikesCount = likeCount;
                                    UserDetail.FullName = Fullname;
                                    UserDetail.IsVerified = Isverfied;
                                    UserDetail.IsMuted = Ismute;
                                    UserDetail.TweetsCount = tweetCount;
                                    UserDetail.IsPrivate = Isprotected;
                                    UserDetail.HasProfilePic = hasProfilePic;
                                    UserDetail.ProfilePicUrl = profileImage;
                                }

                                UserDetail.ListTag.Add(new TagDetails
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
                                    IsTweetContainedVideo = tweetDetails["extended_entities"] != null
                                    ? tweetDetails["extended_entities"]["media"][0]["video_info"] != null
                                    : false
                                });
                            }

                        }
                    }
                }
                else if (response.Contains("{\"globalObjects\":{\""))
                {
                    var jsonHand = new Jsonhandler(response);
                    var tweets = jsonHand.GetJToken("globalObjects", "tweets");
                    var users = jsonHand.GetJToken("globalObjects", "users");
                    var cursorValue =
                        jsonHand.GetJTokenValue(
                            jsonHand.GetJToken("timeline", "instructions", 0, "addEntries", "entries").Last(), "content",
                            "operation", "cursor", "value");
                    MinPosition = Uri.EscapeDataString(cursorValue);
                    var userName = string.Empty;
                    var profileImage = string.Empty;


                    foreach (var item in tweets)
                    {
                        ListImagePath.Clear();
                        var token = item.First();
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
                        var TwtTimeStamp = dt.ConvertToEpoch();
                        if (token["extended_entities"] != null
                            ? token["extended_entities"]["media"][0]["video_info"] != null
                            : false)
                        {
                            ListImagePath.Clear();
                            ListImagePath.Add(jsonHand.GetJTokenValue(item.First(), "extended_entities", "media", 0,
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
                            var jToken = userdetail.First();
                            var userId = jsonHand.GetJTokenValue(jToken, "id_str");
                            var username = jsonHand.GetJTokenValue(jToken, "screen_name");
                            if (username == Username)
                            {
                                userName = jsonHand.GetJTokenValue(jToken, "screen_name");
                                profileImage = jsonHand.GetJTokenValue(jToken, "profile_image_url_https");
                                following = jsonHand.GetJTokenValue(jToken, "following") == "True";
                                followBack = jsonHand.GetJTokenValue(jToken, "followed_by") == "True";
                                Ismute = jsonHand.GetJTokenValue(jToken, "muting") == "True";
                                Isverfied = jsonHand.GetJTokenValue(jToken, "is_blue_verified") == "True";
                                Isprotected = jsonHand.GetJTokenValue(jToken, "protected") == "True";
                                var userBio = jsonHand.GetJTokenValue(jToken, "description");
                                var followerCount = int.Parse(jsonHand.GetJTokenValue(jToken, "followers_count"));
                                var FollowingCount = int.Parse(jsonHand.GetJTokenValue(jToken, "friends_count"));
                                var userLocation = jsonHand.GetJTokenValue(jToken, "location");
                                var likeCount = int.Parse(jsonHand.GetJTokenValue(jToken, "favourites_count"));
                                var Fullname = jsonHand.GetJTokenValue(jToken, "name");
                                var tweetCount = int.Parse(jsonHand.GetJTokenValue(jToken, "statuses_count"));
                                var hasProfilePic = jsonHand.GetJTokenValue(jToken, "profile_image_url_https") != null;
                                UserDetail.UserId = userId;
                                UserDetail.UserBio = userBio;
                                UserDetail.Username = userName;
                                UserDetail.FollowersCount = followerCount;
                                UserDetail.FollowingsCount = FollowingCount;
                                UserDetail.FollowBackStatus = followBack;
                                UserDetail.FollowStatus = following;
                                UserDetail.UserLocation = userLocation;
                                UserDetail.LikesCount = likeCount;
                                UserDetail.FullName = Fullname;
                                UserDetail.IsVerified = Isverfied;
                                UserDetail.IsMuted = Ismute;
                                UserDetail.TweetsCount = tweetCount;
                                UserDetail.IsPrivate = Isprotected;
                                UserDetail.HasProfilePic = hasProfilePic;
                                UserDetail.ProfilePicUrl = profileImage;
                                break;
                            }
                        }

                        var singletweet = new TagDetails
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
                        };
                        UserDetail.ListTag.Add(singletweet);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }


            if (UserDetail.ListTag.Count > 0)
                Hasmore = true;
        }

        private bool GetAccountStatus(string respText)
        {
            try
            {
                var resp = HtmlAgilityHelper.getStringInnerHtmlFromClassName(respText,
                    "flex-module error-page clearfix");
                if (resp.Contains(TdConstants.AccountSuspended))
                {
                    UserDetail.UserStatus = TdConstants.AccountSuspended;
                    return true;
                }

                if (respText.Contains("temporarily unavailable"))
                {
                    UserDetail.UserStatus = TdConstants.AccountTemporaryUnAvailable;
                    return true;
                }
            }
            catch (Exception e)
            {
                e.ErrorLog();
            }

            return false;
        }
    }
}