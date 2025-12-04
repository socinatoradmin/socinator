using DominatorHouseCore;
using DominatorHouseCore.Models.SocioPublisher.Settings;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDUtility;
using System;
using System.Net;
using System.Web;

namespace RedditDominatorCore.RDLibrary
{
    public class RdConstants
    {
        public static string HostKey = "Host";
        public static string AuthorizationKey = "Authorization";
        public static string OriginKey = "Origin";
        public static string LoidKey = "x-reddit-loid";
        public static string RdUserIdKey = "reddit-user_id";
        public static string RdSessionKey = "x-reddit-session";
        public static string RefererKey = "Referer";
        public static string AccessControlKey = "Access-Control-Request-Headers";
        public static string AcceptType = "*/*";
        public static string AccessControlValue = string.Empty;
        public static string ContentType = "application/x-www-form-urlencoded";
        public static string AcceptTypeAds = "application/json";
        public static string SuspendedMessage { get; set; } = "Uh oh! We have suspended your account due to suspicious activity. Not to worry. You can continue using Reddit by resetting your password.";
        public static string PermanentlyBanned { get; set; } = "This account has been permanently banned. Check your inbox for a message with more information.";
        public static string LockedMessage { get; set; } = "locked your account after detecting some unusual activity";
        public static string GetUserChatsUrl(string userId) =>
            $"https://sendbirdproxyk8s.chat.redditmedia.com/v3/users/{userId}/my_group_channels?token=&limit=10&order=latest_last_message&show_member=true&show_read_receipt=true&show_delivery_receipt=true&show_empty=true&member_state_filter=joined_only&custom_types=direct,group&super_mode=all&public_mode=all&unread_filter=all&hidden_mode=unhidden_only&show_frozen=true&show_metadata=true";
        public static string GetProfileDetailsAPI(string UserName) => $"https://oauth.reddit.com/user/{UserName}/about.json?redditWebClient=desktop2x&app=desktop2x-client-production&gilding_detail=1&awarded_detail=1&raw_json=1";
        public static string GraphQlAPI => "https://www.reddit.com/svc/shreddit/graphql";
        public static string VoteOAuthUrl =>
            "https://oauth.reddit.com/api/vote?redditWebClient=web2x&app=web2x-client-production&raw_json=1";

        public static string SubmitPageGatewayUrl =>
            "https://gateway.reddit.com/desktopapi/v1/submitpage?redditWebClient=web2x&app=web2x-client-production&allow_over18=";

        public static string SubscribeOAuthUrl => "https://oauth.reddit.com/api/subscribe?raw_json=1";
        public static string ApproveUrl =>
            "https://oauth.reddit.com/api/approve?redditWebClient=web2x&app=web2x-client-production&raw_json=1";

        public static string CommentOAuthUrl =>
            "https://oauth.reddit.com/api/comment.json?rtj=debug&redditWebClient=web2x&app=web2x-client-production&raw_json=1";

        public static string FetchTitleOfUrl =>
            "https://oauth.reddit.com/api/fetch_title?redditWebClient=web2x&app=web2x-client-production&raw_json=1";

        public static string SubmitOAuthUrl =>
            "https://oauth.reddit.com/api/submit?resubmit=true&redditWebClient=web2x&app=web2x-client-production&rtj=debug&raw_json=1";

        public static string EditCommenOAuthtUrl =>
            "https://oauth.reddit.com/api/editusertext?rtj=only&redditWebClient=web2x&app=web2x-client-production&raw_json=1&gilding_detail=1";

        public static string UserSuspendedArticleUrl =>
            "https://www.reddithelp.com/en/categories/reddit-101/rules-reporting/account-and-community-restrictions/suspensions";

        public static string QuarantineUrl =>
            "https://oauth.reddit.com/api/quarantine_optin?raw_json=1&gilding_detail=1";
        //public static string RedditAdsDataMain=> "https://red.poweradspy.com/redditAdsData";
        //public static string RedditAdsDataDev => "https://reddit-dev.poweradspy.com/redditAdsData";
        public static string RedditAdsDataMain => "https://red.poweradspy.com/redAdsData";
        public static string RedditAdsDataDev => "https://reddit-dev.poweradspy.com/redAdsData";
        public static string SaveUserDetailsInDev => string.Empty;//Save Reddit User At PowerAds Server Api.
        public static string CommunityListUrl =>
            "https://www.reddit.com/svc/shreddit/left-nav-communities-section";
        //"https://gateway.reddit.com/desktopapi/v1/subscriptions?allow_over18=&include=identity";
        public static bool KeepAlive => true;

        public static string UserAgentValue => "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36";

        public string PostSearchUrlByKeyword(string keyword) =>
            $"{GetRedditHomePageAPI}/search?q={keyword}&type=link";

        public static string UserProfileUrlByUsername(string username) =>
            $"{GetRedditHomePageAPI}/user/{username}";

        public static string GetGroupDetailsApi(string groupname) =>
            $"{GetRedditHomePageAPI}/r/{groupname}/submit/?type=TEXT";
        public static string GetCommunityUrlByUsername(string username) =>
            $"{GetRedditHomePageAPI}/r/{username}";
        public static string GetRedditHomePageAPI { get; set; } = "https://www.reddit.com";
        public static string NewRedditHomePageAPI { get; set; } = "https://www.reddit.com";
        public static string GetTimeStamp => DateTime.Now.Ticks.ToString();
        public static string GetJsonPageResponse(string response)
        {
            var jsonPageResponse = string.Empty;
            try
            {
                jsonPageResponse = Utilities.GetBetween(GetDecodedResponse(response, true, true), "window.___r = ", ";</script><script>");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jsonPageResponse;
        }
        public static string GetDecodedResponse(string Response, bool IsHtmlDecode = true, bool IsUrlDecode = false) => string.IsNullOrEmpty(Response) ? string.Empty : IsHtmlDecode && IsUrlDecode ? WebUtility.UrlDecode(WebUtility.HtmlDecode(Response)) : IsHtmlDecode ? WebUtility.HtmlDecode(Response) : WebUtility.UrlDecode(Response);
        public static string CreateChatRoomAPI => $"https://matrix.redditspace.com/_matrix/client/r0/createRoom";//Post API.
        public static string CreateChatRoomPostData(string UserId) => $"{{\"preset\":\"reddit_dm\",\"invite\":[\"@{UserId}:reddit.com\"]}}";
        public static string GetSendMessageAPI(string RoomID) => $"https://matrix.redditspace.com/_matrix/client/r0/rooms/{RoomID?.Replace(":", "%3A")}/send/m.room.message/m{GetTimeStamp}.0";//PUT API.
        public static string GetSendMessagePostData(string Message) => $"{{\"msgtype\":\"m.text\",\"body\":\"{Message}\"}}";
        public static string GetMarkAsReadAPI(string RoomID) => $"https://matrix.redditspace.com/_matrix/client/r0/rooms/{RoomID?.Replace(":", "%3A")}/read_markers";//POST Api.
        public static string MarkAsReadPostData(string SendMessageEventID) => $"{{\"m.fully_read\":\"{SendMessageEventID}\"}}";
        public static string NewMessageAPI => $"https://matrix.redditspace.com/_matrix/client/r0/sync?filter=%7B%22room%22%3A%7B%22state%22%3A%7B%22lazy_load_members%22%3Atrue%7D%2C%22timeline%22%3A%7B%22limit%22%3A10%7D%7D%7D&timeout=0&_cacheBuster={GetTimeStamp}";
        public static string GetMyIPAPI => "https://app.multiloginapp.com/WhatIsMyIP";
        public static string GetLocationAPI(string Ip) => $"http://ip-api.com/json/{Ip.Trim()}";
        #region VideoUploadAPI.
        public static string UploadMediaOAuthUrl => "https://oauth.reddit.com/api/media/asset.json?raw_json=1&gilding_detail=1";
        public static string UploadMediaOAuthUrl1 => "https://www.reddit.com/svc/shreddit/graphql";
        public static string GetMediaUploadAPI(bool IsVideo = true) => IsVideo ? $"https://reddit-uploaded-video.s3-accelerate.amazonaws.com/" : "https://reddit-uploaded-media.s3-accelerate.amazonaws.com/";
        public static string CrossPostPublishUrl =>
            "https://oauth.reddit.com/api/submit?resubmit=true&redditWebClient=desktop2x&app=desktop2x-client-production&rtj=only&raw_json=1&gilding_detail=1";
        public static string ApprovePostAPI => "https://gql.reddit.com/";//Post API.
        public static string ApprovePostData(string PostID) => $"{{\"id\":\"660e0733e963\",\"variables\":{{\"input\":{{\"id\":\"{PostID}\"}}}}}}";
        public static string GetFinalSubmitPostData(string Username, string MediaID, string Title, string descrption, bool IsSpoiler = false, bool IsNsfw = false, bool IsOriginalContent = false, bool SendReplies = true, string kind = "self", string submitType = "profile", bool IsVideo = true, string flairId = "", string flairText = "", bool NotAMedia = false, string ShareUrl = "") => NotAMedia ?
            !string.IsNullOrEmpty(ShareUrl) && !string.IsNullOrEmpty(Title) ? $"sr={Username}&submit_type={submitType}&api_type=json&show_error_list=true&title={Title}&spoiler={IsSpoiler}&nsfw={IsNsfw}&kind=link&original_content={IsOriginalContent}&post_to_twitter=false&sendreplies={SendReplies}&url={ShareUrl}&validate_on_submit=true"
            : $"sr={Username}&submit_type={submitType}&api_type=json&show_error_list=true&title={Title}&spoiler={IsSpoiler}&nsfw={IsNsfw}&kind={kind}&original_content={IsOriginalContent}&post_to_twitter=false&sendreplies={SendReplies}&richtext_json={WebUtility.UrlEncode($"{{\"document\":[{Utils.GenerateRichText(descrption)}]}}")}{GetFlairData(flairId, flairText)}&validate_on_submit=true"
            : IsVideo ? $"sr={Username}&submit_type={submitType}&api_type=json&show_error_list=true&title={Title}&spoiler={IsSpoiler}&nsfw={IsNsfw}&kind={kind}&original_content={IsOriginalContent}&post_to_twitter=false&sendreplies={SendReplies}&richtext_json={{\"document\":[{{\"e\":\"par\",\"c\":[]}},{{\"e\":\"video\",\"id\":\"{MediaID}\",\"c\":\"{descrption}\"}}]}}{GetFlairData(flairId, flairText)}&validate_on_submit=true" :
            $"sr={Username}&submit_type={submitType}&api_type=json&show_error_list=true&title={Title}&spoiler={IsSpoiler}&nsfw={IsNsfw}&kind={kind}&original_content={IsOriginalContent}&post_to_twitter=false&sendreplies={SendReplies}&richtext_json={{\"document\":[{{\"e\":\"par\",\"c\":[]}},{{\"e\":\"img\",\"id\":\"{MediaID}\",\"c\":\"{descrption}\"}}]}}{GetFlairData(flairId, flairText)}&validate_on_submit=true";
        public static string MarkAsOriginalContentAPI => $"https://oauth.reddit.com/api/set_original_content?redditWebClient=desktop2x&app=desktop2x-client-production&raw_json=1&gilding_detail=1";
        public static string MarkAsOriginalContentPostData(string PostID) => $"fullname={PostID}&should_set_oc=true";
        public static string GetGroupPostData(string GroupName, RedditPostSetting postSetting, string Title, bool IsVideo = false, string VideoUploadUrl = "", string ThumbNailUrl = "", string Kind = "", string Link = "", bool IsImage = false, string flairId = "", string flairText = "") => IsVideo ?
            $"sr={GroupName}&submit_type=subreddit&api_type=json&show_error_list=true&title={Title}&spoiler={postSetting.IsSpoiler}&nsfw={postSetting.IsNsfw}&kind=video&original_content={postSetting.IsOriginalContent}&post_to_twitter=false&sendreplies={!postSetting.IsDisableSendingReplies}&url={VideoUploadUrl}&video_poster_url={ThumbNailUrl}{GetFlairData(flairId, flairText)}&validate_on_submit=true" : IsImage ?
            $"sr={GroupName}&submit_type=subreddit&api_type=json&show_error_list=true&title={Title}&spoiler={postSetting.IsSpoiler}&nsfw={postSetting.IsNsfw}&kind=image&original_content={postSetting.IsOriginalContent}&post_to_twitter=false&sendreplies={!postSetting.IsDisableSendingReplies}&url={ThumbNailUrl}{GetFlairData(flairId, flairText)}&validate_on_submit=true" :
            $"sr={GroupName}&submit_type=subreddit&api_type=json&show_error_list=true&title={Title}&spoiler={postSetting.IsSpoiler}&nsfw={postSetting.IsNsfw}&kind={Kind}&original_content={postSetting.IsOriginalContent}&post_to_twitter=false&sendreplies={!postSetting.IsDisableSendingReplies}&url={Link}{GetFlairData(flairId, flairText)}&validate_on_submit=true";
        public static string GetGroupPostSubmitAPI(string UserId, string SubRedditName) => $"https://gateway.reddit.com/desktopapi/v1/postcomments/{UserId}?rtj=only&emotes_as_images=true&redditWebClient=web2x&app=web2x-client-production&profile_img=true&allow_over18=1&include=identity&subredditName={SubRedditName}&hasSortParam=false&instanceId&include_categories=true&onOtherDiscussions=false&comment_awardings_by_current_user=true";
        public static string GetSubRedditInfoAPI(string SubRedditName) => $"https://gateway.reddit.com/desktopapi/v1/subreddits/{SubRedditName}?rtj=only&redditWebClient=web2x&app=web2x-client-production&allow_over18=1&include=identity,structuredStyles,prefsSubreddit&layout=card";
        public static string GetFlairData(string Id, string text) => string.IsNullOrEmpty(Id) && string.IsNullOrEmpty(text) ? string.Empty : $"&flair_id={Id}&flair_text={text}";
        public static string PostDataForMediaUpload(string csrftoken, bool isVideo) =>
            !isVideo ? $"{{\"operation\":\"CreateMediaUploadLease\",\"variables\":{{\"input\":{{\"mimetype\":\"JPEG\"}}}},\"csrf_token\":\"{csrftoken}\"}}"
            : $"{{\"operation\":\"CreateMediaUploadLease\",\"variables\":{{\"input\":{{\"mimetype\":\"MP4\"}}}},\"csrf_token\":\"{csrftoken}\"}}";
        public static string GetFinalPostSubmitApi => "https://www.reddit.com/svc/shreddit/graphql";
        public static string GetPostDataForUserIdentity(string csrftoken)
        {
            return $"{{\"operation\":\"IdentityUserPreferences\",\"variables\":{{\"includeNightMode\":true}},\"csrf_token\":\"{csrftoken}\"}}";
        }
        public static string GetPostDataForChannelSubscription(string Id, string csrftoken, bool isSubscribe = false)
        {
            return isSubscribe ? $"{{\"operation\":\"UpdateSubredditSubscriptions\",\"variables\":{{\"input\":{{\"inputs\":[{{\"subredditId\":\"{Id}\",\"subscribeState\":\"SUBSCRIBED\"}}]}}}},\"csrf_token\":\"{csrftoken}\"}}" :
                                $"{{\"operation\":\"UpdateSubredditSubscriptions\",\"variables\":{{\"input\":{{\"inputs\":[{{\"subredditId\":\"{Id}\",\"subscribeState\":\"NONE\"}}]}}}},\"csrf_token\":\"{csrftoken}\"}}";
        }
        public static string GetPostDataForFollowingUser(string Id, string csrftoken, bool isFollow = true)
        {
            return isFollow ? $"{{\"operation\":\"UpdateProfileFollowState\",\"variables\":{{\"input\":{{\"accountId\":\"{Id}\",\"state\":\"FOLLOWED\"}}}},\"csrf_token\":\"{csrftoken}\"}}" :
                       $"{{\"operation\":\"UpdateProfileFollowState\",\"variables\":{{\"input\":{{\"accountId\":\"{Id}\",\"state\":\"NONE\"}}}},\"csrf_token\":\"{csrftoken}\"}}";
        }
        public static string GetPostDataForVote(string Id, string csrftoken, bool isUpvote = true, bool isDownVote = false, bool isComment = false)
        {
            if (!isComment)
                return isUpvote ? $"{{\"operation\":\"UpdatePostVoteState\",\"variables\":{{\"input\":{{\"postId\":\"{Id}\",\"voteState\":\"UP\"}}}},\"csrf_token\":\"{csrftoken}\"}}" :
                      isDownVote ? $"{{\"operation\":\"UpdatePostVoteState\",\"variables\":{{\"input\":{{\"postId\":\"{Id}\",\"voteState\":\"DOWN\"}}}},\"csrf_token\":\"{csrftoken}\"}}" :
                      $"{{\"operation\":\"UpdatePostVoteState\",\"variables\":{{\"input\":{{\"postId\":\"{Id}\",\"voteState\":\"NONE\"}}}},\"csrf_token\":\"{csrftoken}\"}}";
            return isUpvote ? $"{{\"operation\":\"UpdateCommentVoteState\",\"variables\":{{\"input\":{{\"commentId\":\"{Id}\",\"voteState\":\"UP\"}}}},\"csrf_token\":\"{csrftoken}\"}}" :
                       isDownVote ? $"{{\"operation\":\"UpdateCommentVoteState\",\"variables\":{{\"input\":{{\"commentId\":\"{Id}\",\"voteState\":\"DOWN\"}}}},\"csrf_token\":\"{csrftoken}\"}}" :
                       $"{{\"operation\":\"UpdateCommentVoteState\",\"variables\":{{\"input\":{{\"commentId\":\"{Id}\",\"voteState\":\"NONE\"}}}},\"csrf_token\":\"{csrftoken}\"}}";
        }
        public static string GetFinalSubmitPostData1(string token, string MediaID, string Title, string descrption, bool IsSpoiler = false, bool IsNsfw = false, bool IsOriginalContent = false, bool SendReplies = true, string kind = "self", string submitType = "profile", bool IsVideo = true, string flairId = "", string flairText = "", bool NotAMedia = false, string ShareUrl = "", string ImageUrl="")
        {
            if (NotAMedia && !string.IsNullOrEmpty(Title))
                return $"{{\"operation\":\"CreateProfilePost\",\"variables\":{{\"input\":{{\"isNsfw\":{IsNsfw.ToString().ToLower()},\"isSpoiler\":{IsSpoiler.ToString().ToLower()},\"content\":{{\"richText\":\"{{\\\"document\\\":[{{\\\"e\\\":\\\"par\\\",\\\"c\\\":[{{\\\"e\\\":\\\"text\\\",\\\"t\\\":\\\"{descrption}\\\"}}]}}]}}\"}},\"title\":\"{Title}\",\"isCommercialCommunication\":false,\"targetLanguage\":\"\"}}}},\"csrf_token\":\"{token}\"}}";
            if (IsVideo)
                return $"{{\"operation\":\"CreateProfilePost\",\"variables\":{{\"input\":{{\"isNsfw\":{IsNsfw.ToString().ToLower()},\"isSpoiler\":{IsSpoiler.ToString().ToLower()},\"content\":{{\"richText\":\"{{\\\"document\\\":[{{\\\"e\\\":\\\"video\\\",\\\"id\\\":\\\"{MediaID}\\\"}},{{\\\"e\\\":\\\"par\\\",\\\"c\\\":[{{\\\"e\\\":\\\"text\\\",\\\"t\\\":\\\"{descrption}\\\",\\\"f\\\":[[0,0,{descrption.Length}]]}}]}}]}}\"}},\"title\":\"{Title}\",\"isCommercialCommunication\":false,\"targetLanguage\":\"\"}}}},\"csrf_token\":\"{token}\"}}";
            else
                return $"{{\"operation\":\"CreateProfilePost\",\"variables\":{{\"input\":{{\"isNsfw\":{IsNsfw.ToString().ToLower()},\"isSpoiler\":{IsSpoiler.ToString().ToLower()},\"content\":{{\"richText\":\"{{\\\"document\\\":[{{\\\"e\\\":\\\"par\\\",\\\"c\\\":[{{\\\"e\\\":\\\"text\\\",\\\"t\\\":\\\"{descrption}\\\",\\\"f\\\":[[0,0,{descrption.Length}]]}}]}}]}}\"}},\"image\":{{\"url\":\"{ImageUrl}\"}},\"title\":\"{Title}\",\"isCommercialCommunication\":false,\"targetLanguage\":\"\"}}}},\"csrf_token\":\"{token}\"}}";
        }
        public static string GetGroupPostData1(string token, string MediaID, RedditPostSetting postSetting, string GroupName, string Title = "", string description = "", bool IsVideo = false, string VideoUploadUrl = "", string ThumbNailUrl = "", string Kind = "", string Link = "", bool IsImage = false, string flairId = "", string flairText = "", bool NotAMedia = false)
        {
            if (!string.IsNullOrEmpty(Link) && !string.IsNullOrEmpty(Title))
                return $"{{\"operation\":\"CreatePost\",\"variables\":{{\"input\":{{\"isNsfw\":{postSetting.IsNsfw.ToString().ToLower()},\"isSpoiler\":{postSetting.IsSpoiler.ToString().ToLower()},\"link\":{{\"url\":\"{Link}\"}},\"title\":\"{Title}\",\"isCommercialCommunication\":false,\"subredditName\":\"{GroupName}\",\"flair\":{{\"id\":\"{flairId}\",\"text\":\"{flairText}\"}},\"targetLanguage\":\"\"}}}},\"csrf_token\":\"{token}\"}}";
            if (NotAMedia && !string.IsNullOrEmpty(Title))
                return $"{{\"operation\":\"CreatePost\",\"variables\":{{\"input\":{{\"isNsfw\":{postSetting.IsNsfw.ToString().ToLower()},\"isSpoiler\":{postSetting.IsSpoiler.ToString().ToLower()},\"content\":{{\"richText\":\"{{\\\"document\\\":[{{\\\"e\\\":\\\"par\\\",\\\"c\\\":[{{\\\"e\\\":\\\"text\\\",\\\"t\\\":\\\"{description}\\\",\\\"f\\\":[[0,0,{description.Length}]]}}]}}]}}\"}},\"title\":\"{Title}\",\"isCommercialCommunication\":false,\"subredditName\":\"{GroupName}\",\"flair\":{{\"id\":\"{flairId}\",\"text\":\"{flairText}\"}},\"targetLanguage\":\"\"}}}},\"csrf_token\":\"{token}\"}}";
            else if (IsVideo)
                return $"{{\"operation\":\"CreatePost\",\"variables\":{{\"input\":{{\"isNsfw\":{postSetting.IsNsfw.ToString().ToLower()},\"isSpoiler\":{postSetting.IsSpoiler.ToString().ToLower()},\"content\":{{\"richText\":\"{{\\\"document\\\":[{{\\\"e\\\":\\\"video\\\",\\\"id\\\":\\\"{MediaID}\\\"}},{{\\\"e\\\":\\\"par\\\",\\\"c\\\":[{{\\\"e\\\":\\\"text\\\",\\\"t\\\":\\\"{description}\\\",\\\"f\\\":[[0,0,{description.Length}]]}}]}}]}}\"}},\"title\":\"{Title}\",\"isCommercialCommunication\":false,\"subredditName\":\"{GroupName}\",\"flair\":{{\"id\":\"{flairId}\",\"text\":\"{flairText}\"}},\"targetLanguage\":\"\"}}}},\"csrf_token\":\"{token}\"}}";
            else
                return $"{{\"operation\":\"CreatePost\",\"variables\":{{\"input\":{{\"isNsfw\":{postSetting.IsNsfw.ToString().ToLower()},\"isSpoiler\":{postSetting.IsSpoiler.ToString().ToLower()},\"content\":{{\"richText\":\"{{\\\"document\\\":[{{\\\"e\\\":\\\"img\\\",\\\"id\\\":\\\"{MediaID}\\\"}},{{\\\"e\\\":\\\"par\\\",\\\"c\\\":[{{\\\"e\\\":\\\"text\\\",\\\"t\\\":\\\"{description}\\\",\\\"f\\\":[[0,0,{description.Length}]]}}]}}]}}\"}},\"title\":\"{Title}\",\"isCommercialCommunication\":false,\"subredditName\":\"{GroupName}\",\"flair\":{{\"id\":\"{flairId}\",\"text\":\"{flairText}\"}},\"targetLanguage\":\"\"}}}},\"csrf_token\":\"{token}\"}}";
        }

        public static string PostDataToUpdateToken(string csrfToken, string postId) =>
            $"{{\"operation\":\"GetIsCommentGuidanceAvailableFromPostId\",\"variables\":{{\"postId\":\"{postId}\"}},\"csrf_token\":\"{csrfToken}\"}}";

        public static string PostDataForCommentOnPost(string csrfToken, string txtMsg, bool isContainLink = false)
        {
            return !isContainLink ? $"content={WebUtility.UrlEncode($"{{\"document\":[{Utils.GenerateRichText(txtMsg)}]}}")}&mode=richText&richTextMedia=%5B%5D&csrf_token={csrfToken}"
                : $"content={{\"document\":[{Utils.GenerateRichText(txtMsg)}]}}&mode=richText&richTextMedia=%5B%5D&csrf_token={csrfToken}";
        }
        #endregion
        #region For Language Translation..
        public static string Translate(string input, string from, string to)
        {
            try
            {
                var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={from}&tl={to}&dt=t&q={HttpUtility.UrlEncode(input)}";
                var webclient = new WebClient
                {
                    Encoding = System.Text.Encoding.UTF8
                };
                var result = webclient.DownloadString(url);
                result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
                return result;
            }
            catch (Exception)
            {
                return "";
            }
        }
        #endregion
    }

    public class RdParameters
    {
        public string AuthorizationValue = string.Empty;
        public string LoidValue = string.Empty;
        public string OriginValue = RdConstants.GetRedditHomePageAPI;//"https://www.reddit.com";
        public string RdHostValue = string.Empty;
        public string RdUserIdValue = "desktop2x";
        public string RefererValue = string.Empty;
        public string SessionValue = string.Empty;
    }

    public class PaginationParameter
    {
        public string LastPaginationId { get; set; }
        public string SessionTracker { get; set; }
        public string Reddaid { get; set; }
        public string Loid { get; set; }
        public string AccessToken { get; set; }
        public bool HasNextPage { get; set; }
    }
}