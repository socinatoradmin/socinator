using DominatorHouseCore;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    public static class FdConstants
    {

        #region Facebook Urls

        public static bool IsWebFacebook = false;
        public static string FbHomeUrl => IsWebFacebook
            ? "https://web.facebook.com/"
            : "https://www.facebook.com/";

        public static string FbWatchVideoUrl = "https://fb.watch/";

        public static string FbHomeUrlMobile => "https://m.facebook.com/";



        public static string AdsPreferenceUrl => $"{FbHomeUrl}ads/preferences/dialog/";

        public static string SaveAdsUrlMain => "https://api.poweradspy.com/";


        public static string FbLoginPhpUrl => $"{FbHomeUrl}login/device-based/regular/login/";

        public static string FbPeopleSearchUrl => $"{FbHomeUrl}search/people/";


        public static string FbGroupSearchUrl => $"{FbHomeUrl}search/groups/";
        public static string FbPostSharerUrl => $"{FbHomeUrl}ajax/shares/view";
        public static string FbPostLikersUrl => $"{FbHomeUrl}ufi/reaction/profile/dialog/";

        public static string FbGroupAnswerUrl => $"{FbHomeUrl}groups/membership_criteria_answer/edit/";

        public static string FbJoinGroupUrl => $"{FbHomeUrl}groups/membership/r2j?dpr=1";


        public static string FbPostLikersPageletUrl => $"{FbHomeUrl}ufi/reaction/profile/browser/fetch/";

        public static string FbPostSharerPgeletUrl => $"{FbHomeUrl}ajax/pagelet/generic.php/ResharesPagelet";



        public static string FbGroupPageletUrl => $"{FbHomeUrl}ajax/browser/list/group_confirmed_members/";

        public static string FbGroupDiscoverUrl => $"{FbHomeUrl}ajax/browser/list/group_member_discovery/";

        public static string FbGroupAdminModeratorUrl => $"{FbHomeUrl}ajax/browser/list/group_admins_moderators/";

        public static string FbFriendsPageletUrl => $"{FbHomeUrl}ajax/pagelet/generic.php/AllFriendsAppCollectionPagelet";

        public static string FbFanpageLikerPageleUrl => $"{FbHomeUrl}ajax/pagelet/generic.php/BrowseScrollingSetPagelet";

        public static string PageLikedByFriendsPageleUrl => $"{FbHomeUrl}ajax/pagelet/generic.php/LikesWithFollowCollectionPagelet";

        public static string JoinedGroupPageletUrl => $"{FbHomeUrl}groups/discover/more/";


        public static string FbCommentPaginationUrl => $"{FbHomeUrl}ajax/ufi/comment_fetch.php?dpr=1";

        public static string FbSendFriendRequestUrl => $"{FbHomeUrl}ajax/add_friend/action.php?dpr=1";

        public static string FbFindFriendUrl => $"{FbHomeUrl}friends/requests/";

        public static string FriendSuggestedByFriendUrl => $"{FbHomeUrl}ajax/growth/friend_browser/checkbox.php?dpr=1";

        public static string CheckLastMessageUrl => $"{FbHomeUrl}api/graphqlbatch/?dpr=1";

        public static string IncomingFriendPagerUrl => $"{FbHomeUrl}ajax/friends/requests/pager/";



        public static string SentFriendExtraPager => $"{FbHomeUrl}ajax/timeline/all_activity/scroll.php";

        public static string FriendActivityNextPager => $"{FdConstants.FbHomeUrl}ajax/pagelet/generic.php/TimelineEntStoryActivityLogPagelet";


        public static string BirthDayUrl => "https://m.facebook.com/events/calendar/birthdays/";

        public static string BirthDayPaginationUrl => "https://m.facebook.com/events/ajax/dashboard/calendar/birthdays/";


        public static string CancelFriendRequest => $"{FbHomeUrl}ajax/friends/requests/cancel.php?dpr=1";

        public static string CancelIncomingRequestUrl => $"{FbHomeUrl}requests/friends/ajax/?dpr=1";

        public static string AcceptRequestUrl => $"{FbHomeUrl}requests/friends/ajax/?dpr=1";

        public static string PlaceScraperUrl => $"{FbHomeUrl}browse/async/places/";

        public static string FanpageLikerUrl => $"{FbHomeUrl}ajax/pages/fan_status.php";

        public static string PostLikerUrl => $"{FbHomeUrl}ufi/reaction/?dpr=1";

        public static string TimeLinePaginationUrl => $"{FbHomeUrl}ajax/pagelet/generic.php/ProfileTimelineJumperStoriesPagelet";

        public static string NewsFeedPaginationUrl => $"{FbHomeUrl}ajax/pagelet/generic.php/LitestandTailLoadPagelet";

        public static string FanpagePostPaginationUrl => $"{FbHomeUrl}pages_reaction_units/more/";

        public static string GroupPostPaginationUrl => $"{FbHomeUrl}ajax/pagelet/generic.php/GroupEntstreamPagelet";

        public static string Unfriend => $"{FbHomeUrl}ajax/profile/removefriendconfirm.php?dpr=1";

        public static string UpdateJoinedGroupUrl => $"{FbHomeUrl}groups/";

        public static string UpdateOwnPageUrl => $"{FbHomeUrl}bookmarks/pages";

        public static string UpdateLikedPageUrl => $"{FbHomeUrl}search/me/pages-liked";



        public static string PostCommentUrl => $"{FbHomeUrl}ufi/add/comment/";

        public static string SendDirectMessageUrl => $"{FbHomeUrl}messaging/send/";

        public static string SendLinkMessageUrl => $"{FbHomeUrl}message_share_attachment/fromURI/";

        public static string MyGroupUrl => $"{FbHomeUrl}search/me/groups";



        public static string LeaveGroupUrl => $"{FbHomeUrl}ajax/groups/membership/leave.php";

        public static string GroupInviteUrl => $"{FbHomeUrl}ajax/groups/members/add_post";

        public static string PageInviteUrl => $"{FbHomeUrl}pages/batch_invite_send/";

        public static string SinglePageInviteUrl => $"{FbHomeUrl}pages/friend_invite/send/";

        public static string InvitePageLikerUrl => $"{FbHomeUrl}pages/post_like_invite/send/";

        public static string EventInviteUrl => $"{FbHomeUrl}ajax/events/permalink/invite.php";

        public static string ShareAsMessageUrl => $"{FbHomeUrl}share/dialog/submit/";

        public static string GetMessagesUrl => $"{FbHomeUrl}api/graphqlbatch/";

        public static string ApiGraphql = $"{FbHomeUrl}api/graphql/";

        public static string GetCommentUrl => $"{FbHomeUrl}api/graphql/";

        public static string LikeCommentsUrl => $"{FbHomeUrl}ufi/comment/reaction/?dpr=1";

        public static string PageMembersUrl => $"{FbHomeUrl}api/graphql/";

        public static string ChangeLanguageUrl => $"{FbHomeUrl}ajax/settings/language/account.php?dpr=1";
        public static string HidePostFromPageUrl => $"{FbHomeUrl}ajax/feed/filter_action/dialog_direct_action/?dpr=1";



        public static string GetLocationUrl => "https://app.multiloginapp.com/WhatIsMyIP";
        public static string GetLocationApiUrlFree() =>
            $"http://ip-api.com/json/";

        public static string GetLocationApiUrl() =>
            $"https://api.db-ip.com/v2/9ace4c1b65f77a7fa6c57f4de28cca79f80ee68f/self";

        public static string ComposerSessionId => "b4f5f4c4-88a8 - 487d - bc46 - 3abbcfbfd128";

        public static int ThreadCountUpdateAccount = 0;



        public static string LoggingSessionId => "ef77b538-18c5-41eb-946b-5353b76d3520";

        public static string FanpagePostUrl(string pageId)
            => FbHomeUrl + $"{pageId}/posts";

        public static string FbFanpageUrl(string pageId)
            => FbHomeUrl + $"{pageId}";

        public static string FbGroupUrl(string groupId)
           => FbHomeUrl + $"{groupId}";

        public static string FbFanpageLikersUrl(string pageId)
           => FbHomeUrl + $"search/{pageId}/likers";

        public static string FbUserFollowersUrl(string userId)
            => FbHomeUrl + $"{userId}/followers";



        public static string FbScrapPostByKeyWordUrl(string keyWord, string filters)
            => string.IsNullOrEmpty(filters)
                ? FbHomeUrl + $"search/str/{keyWord}/keywords_blended_posts"
                : FbHomeUrl + $"search/str/{keyWord}/keywords_blended_posts?filters=" + filters;


        public static string FbFriendsPageUrl(string userId)
            => FbHomeUrl + $"{userId}?sk=friends";

        public static string FbGroupMemberPageUrl(string groupId)
         => FbHomeUrl + $"groups/{groupId}/members/";

        public static string FbGroupMemberFrindsUrl(string groupId)
            => FbHomeUrl + $"groups/{groupId}/friends/";

        public static string FbGroupMemberWithCommonUrl(string groupId)
            => FbHomeUrl + $"groups/{groupId}/members_with_things_in_common/";

        public static string FbGroupLocalMemberUrl(string groupId)
           => FbHomeUrl + $"groups/{groupId}/local_members/";

        public static string FbGroupAdminsAndModeratorUrl(string groupId)
           => FbHomeUrl + $"groups/{groupId}/admins/";

        public static string FbPostUrl(string postId)
            => FbHomeUrl + $"{postId}";

        public static string FbGroupDiscussionPage(string groupId)
            => FbHomeUrl + $"groups/{groupId}/?ref=direct";

        public static string FbOwnTimelineUrl(string userId)
            => FbHomeUrl + $"{userId}/";



        public static string FbFriendAboutPageUrl(string userId)
            => FbHomeUrl + $"{userId}/about";

        public static string FbFriendAboutPageUrlMobile(string userId)
           => FbHomeUrlMobile + $"profile.php";

        public static string FbFriendAboutPageUrlMobileUsername(string userId)
            => FbHomeUrlMobile + $"{userId}/about/";





        public static string PostVideoCommentUrl(string postId)
            => FbHomeUrl + $"video/tahoe/async/{postId}/";

        public static string FbSentRequestUrl(string userId)
            => FbHomeUrl + $"{userId}/allactivity";


        public static string InviterDialogUrl(string pageId)
            => $"{FbHomeUrl}{pageId}/friend_inviter_v2/";

        public static string FbEventInviterId(string userId)
            => "{\"" + userId + "\":1}";

        public static string LocationUrl(string location)
          => $"{FbHomeUrl}search/str/{location}/keywords_places/";

        public static string FanpageSearchUrlNew(string searchKeyword)
            => $"{FbHomeUrl}search/pages/?q={searchKeyword}";

        public static string PageLikedByFrendsUrl(string friendId) =>
            $"{FbHomeUrl}{friendId}/likes_all";
        public static string PlaceSearchUrl(string searchKeyword) =>
            $"{FbHomeUrl}search/places/?q={searchKeyword}";
        public static string GetKeyWordSearchUrl(string searchKeyword) =>
            $"{FbHomeUrl}search/top/?q={searchKeyword.Replace(" ", "%20")}";


        public static string FbPeopleSearchByLocation(string location) =>
            $"{FbHomeUrl}search/{location}/residents/present";


        public static string MutualFriendUrl(string mutualFriendId) =>
            $"{FbHomeUrl}browse/mutual_friends/?uid={mutualFriendId}";

        public static string EventGuestUrl =>
            $"{FbHomeUrl}events/typeahead/guest_list/";
        public static string FbEventInviterParameters => "[\"EventsPermalinkWorkplaceContentBannerPagelet\",\"EventsCohostAcceptancePagelet\",\"EventPermalinkEventTicketsPurchaseCardPagelet\",\"EventPublicProdGuestsPagelet\",\"EventPublicProdDetailsPagelet\",\"InviteOffFBInfoBoxPagelet\",\"EventPermalinkAdsSectionPagelet\",\"EventInsightsPagelet\",\"EventPermalinkFeedbackSurvey\",\"SaleEventJoinPagelet\",\"EventPermalinkEventTipsPagelet\",\"EventPublicProdReactionPagelet\",\"EventPublicProdFeedRelatedEventsPagelet\"]";

        public static string GetIndexGlobalFdDir()
        {
            string dir = ConstantVariable.GetPlatformBaseDirectory() + @"\Index\Global\FdAccountDetails";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }

        public static string GetOwnGroupVariables
           => "{\"count\":5000}";
        public static string ActivityLogGroupPost(string userId) =>
            $"{FbHomeUrl}{userId}/allactivity?privacy_source=activity_log&log_filter=groupposts&category_key=groupposts";

        public static string GetMessageQueries(string limit, string timestamp, MessageType messageType)
            => "{\"o0\":{\"doc_id\":\"1956789641011375\",\"query_params\":{\"limit\":" + limit + ",\"before\":" + timestamp + ",\"tags\":[\"" + messageType + "\"],\"isWorkUser\":false,\"includeDeliveryReceipts\":true,\"includeSeqID\":false}}}";

        public static string GetMessageQueriesForUserPagination(string limit, string timestamp, string userId)
           => "{\"o0\":{\"doc_id\":\"1864228073656845\",\"query_params\":{\"id\":\"" + userId + "\",\"message_limit\":" + limit + ",\"load_messages\":true,\"load_read_receipts\":true,\"before\":" + timestamp + "}}}";



        public static string GetMessageQueriesForUser(string limit, string userId)
          => "{\"o0\":{\"doc_id\":\"1864228073656845\",\"query_params\":{\"id\":\"" + userId + "\",\"message_limit\":" + limit + ",\"load_messages\":true,\"load_read_receipts\":false}}}";

        public static string GetLastMessageQueriesForUser(string userId, string limit)
            => "{\"o0\":{\"doc_id\":\"1955136014576208\",\"query_params\":{\"id\":\"" + userId + "\",\"message_limit\":" + limit +
               ",\"load_messages\":true,\"load_read_receipts\":true,\"load_delivery_receipts\":true}}}";

        public static string FbGetStoryQueries
        => "{\"o0\":{\"doc_id\":\"2216926728368868\"}}";

        public static string FbDeleteStoryQueries(string userId, string storyId)
            => "{\"o0\":{\"doc_id\":\"1340045906066120\",\"query_params\":{\"input\":{\"client_mutation_id\":\"js_cm\",\"actor_id\":\""
            + userId + "\",\"story_thread_ids\":[\"" + storyId + "\"]}}}}";

        public static bool DeleteAccountDetailsBin()
        {
            string filepath = GetIndexFdAccountDetailsFile();
            try
            {
                if (File.Exists(filepath))
                    File.Delete(filepath);

            }
            catch (IOException ex)
            {
                ex.DebugLog();
                throw;
            }
            if (File.Exists(filepath))
                return false;
            else
                return true;
        }

        public static string GetIndexFdAccountDetailsFile() => GetIndexGlobalFdDir() + @"\FdAccountDetails.bin";

        public static Dictionary<string, string> AccountLanguage = new Dictionary<string, string>();

        public static Dictionary<string, string> RunningAdAccounts = new Dictionary<string, string>();

        #endregion


        #region Facebook Regex Patterns

        public static readonly Regex YearRegex = new Regex("year:(.*?),", RegexOptions.Singleline);

        public static readonly Regex MonthRegex = new Regex("month:(.*?),", RegexOptions.Singleline);

        public static readonly Regex PreviousShownTimeRegex = new Regex("prev_shown_time=(.*?)&", RegexOptions.Singleline);

        public static readonly Regex ScrubberMonthRegex = new Regex("scrubber_month=(.*?)&", RegexOptions.Singleline);

        public static readonly Regex ScrubberYearRegex = new Regex("scrubber_year=(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex PreviousCursorRegex = new Regex("prev_cursor=(.*?)&", RegexOptions.Singleline);

        //        public static readonly Regex DataKeyRegex = new Regex("data-key=\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex EntityNameRegex = new Regex(">(.*?)<", RegexOptions.Singleline);

        public static readonly Regex GroupMemberCountDetailsRegex = new Regex("pam _grm uiBoxWhite noborder(.*?)</div>", RegexOptions.Singleline);

        public static readonly Regex GroupMemberCountRegex = new Regex("_grt _50f8\">(.*?)</span>", RegexOptions.Singleline);

        public static readonly Regex UsernameRegex = new Regex("facebook.com/(.*?)\\?", RegexOptions.Singleline);

        public static readonly Regex UserNameRegex = new Regex("username=(.*?)&", RegexOptions.Singleline);

        public static readonly Regex GroupIdRegex = new Regex("groupID:(.*?),", RegexOptions.Singleline);

        public static readonly Regex PageIdRegex = new Regex("{\"pageID\":\"(.*?)\",", RegexOptions.Singleline);

        public static readonly Regex EntityIdRegex = new Regex("entity_id\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex FbIdRegex = new Regex("fbId\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex FriendIdRegex = new Regex("entity_id:(.*?),", RegexOptions.Singleline);

        public static readonly Regex UserIdRegex = new Regex("user_id\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex UserId2Regex = new Regex("userID\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex StoryIdentifierRegex = new Regex("ref=tahoe\",\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex VideoPostRegex = new Regex("/videos/(.*?)/", RegexOptions.Singleline);

        public static readonly Regex VideoPostIdRegex = new Regex("/videos/(.*?)[?]", RegexOptions.Singleline);
        public static readonly Regex VideoPostIdRegex2 = new Regex("\"videoId\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex ProfileIdRegex = new Regex("profileid=(.*?)&", RegexOptions.Singleline);

        public static readonly Regex FamilyNameRegex = new Regex(">(.*?)</a>", RegexOptions.Singleline);

        public static readonly Regex ComposerIdRegex = new Regex("composerID:\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex TopLevelPostIdRegex = new Regex("top_level_post_id\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex LegacyPostIdRegex = new Regex("legacy_token:\"(.*?)_", RegexOptions.Singleline);

        public static readonly Regex EntIdentifierPostIdRegex = new Regex("ft_ent_identifier\" value=\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex SharePostIdRegex = new Regex("share_fbid\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex DataTargetIdRegex = new Regex("data-targetid=\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex MediaIdRegex = new Regex("data-full-size-href=\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex AjaxPipeTokenRegex = new Regex("ajaxpipe_token\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex LanguageCodeRegex = new Regex("{\"locale\":\"(.*?)\"", RegexOptions.Singleline);
        public static readonly Regex LanguageCodeRegex2 = new Regex("{\\\"locale\\\":\\\"(.*?)\\\"", RegexOptions.Singleline);

        public static readonly Regex LanguageCodeUpdatedRegex = new Regex("{locale:\"(.*?)\"", RegexOptions.Singleline);
        public static readonly Regex LanguageCodeUpdatedRegex2 = new Regex("{locale:\\\"(.*?)\\\"", RegexOptions.Singleline);

        public static readonly Regex AdDetailsRegex = new Regex("(.*?)</script>", RegexOptions.Singleline);

        public static readonly Regex AdNavigationUrlRegex = new Regex("php\\?u=(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex ReferenceDetailsRegex = new Regex("location.replace\\(\"(.*?)\"", RegexOptions.Singleline);

        //        public static readonly Regex AdNavigatinUrlModRegex = new Regex("(.*?)&h=", RegexOptions.Singleline);

        public static readonly Regex PublishedIdRegex = new Regex("post_fbid\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex PublishedIdModRegex = new Regex("post_fbid\":(.*?)}", RegexOptions.Singleline);

        public static readonly Regex PublishedPostIdRegex = new Regex("\"contentID\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex PublishedPostPrivacyIdRegex = new Regex("privacy_fbid\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex SellPostRegex = new Regex("post_id=(.*?)&", RegexOptions.Singleline);

        public static readonly Regex VideoIdScrapeRegex = new Regex("video_id\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex PhotoIdRegex = new Regex("\"photoID\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex MediaIdModRegex = new Regex("fbid\":(.*?)}", RegexOptions.Singleline);

        public static readonly Regex YearPageRegex = new Regex("\"year\":(.*?),", RegexOptions.Singleline);

        public static readonly Regex MonthPageRegx = new Regex("\"month\":(.*?),", RegexOptions.Singleline);

        public static readonly Regex WebPrivacyRegex = new Regex("name=\"privacyx\" value=\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex PublishedIdRegx = new Regex("post_fbid\":(.*?)},", RegexOptions.Singleline);

        public static readonly Regex UniqueLocationRegx = new Regex("uniqueID\":(.*?),", RegexOptions.Singleline);

        public static readonly Regex CanCommentRegx = new Regex("cancomment\":(.*?),", RegexOptions.Singleline);

        public static readonly Regex CurrencyRegx = new Regex("currency\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex MessangerAppIdRegx = new Regex("MessengerAppID:\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex MessangerAppIdModRegx = new Regex("MessengerAppID\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex ScrapedUrlRegx = new Regex("href=\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex UserNameModRegx = new Regex("<span>(.*?)</span>", RegexOptions.Singleline);

        public static readonly Regex CategoryRegx = new Regex("<span> · </span>(.*?)/a>", RegexOptions.Singleline);

        public static readonly Regex CategoryModRegx = new Regex("<span> · </span>(.*?)/span>", RegexOptions.Singleline);

        public static readonly Regex DataProfileIdRegx = new Regex("data-profileid=\"(.*?)\"", RegexOptions.Singleline);

        //        public static readonly Regex FanpageNameModRegx = new Regex("span>(.*?)<", RegexOptions.Singleline);

        public static readonly Regex MemberNameModRegx = new Regex("<span>(.*?)<span>", RegexOptions.Singleline);

        public static readonly Regex PageIdModRegx = new Regex("pageID\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex PageIdAmberRegx = new Regex("page_id=(.*?)&", RegexOptions.Singleline);

        public static readonly Regex PageIDRegx = new Regex("\"pageID\":(.*?)}", RegexOptions.Singleline);

        public static readonly Regex FanpageNameRegx = new Regex("pageName\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex PageNameRegx = new Regex("page_name:\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex RatingValueRegx = new Regex("ratingValue\":(.*?),", RegexOptions.Singleline);

        public static readonly Regex AggregateRatingRegx = new Regex("aggregateRating\":{(.*?)}", RegexOptions.Singleline);

        public static readonly Regex PageTitleRegx = new Regex("pageTitle\">(.*?)<", RegexOptions.Singleline);

        public static readonly Regex CategoryLabelRegx = new Regex("categoryLabel:(.*?),", RegexOptions.Singleline);

        public static readonly Regex CategoryNameRegx = new Regex("categoryName\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex PageLocationRegx = new Regex("locationLabel:\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex AdTitleRegx = new Regex("aria-label=\"Video(.*?)duration", RegexOptions.Singleline);

        public static readonly Regex ImageSrcRegex = new Regex("src=\"(.*?)\"", RegexOptions.Singleline);
        public static readonly Regex ImageSrc2Regex = new Regex("xlink:href=\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex ActorIdRegex = new Regex("data-test-actorid=\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex CurrentActorIdRegex = new Regex("actorID\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex CommentReplyIdRegex = new Regex("hide_comment_id=(.*?)&", RegexOptions.Singleline);

        public static readonly Regex ReplyCommentIdRegex = new Regex("reply_comment_id=(.*?)&", RegexOptions.Singleline);

        public static readonly Regex ReplyCommenterIdRegex = new Regex("actor_id=(.*?)&", RegexOptions.Singleline);

        public static readonly Regex CommentTextRegex = new Regex("body:{text:\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex CommentTimeRegex = new Regex("time:(.*?),", RegexOptions.Singleline);

        public static readonly Regex DateTimeRegex = new Regex("data-utime=\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex DescriptionRegex = new Regex("summary\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex ProfileUrlRegex = new Regex("profile_url\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex OwnerIdRegex = new Regex("uri_token\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex OwnerId2Regex = new Regex("content_owner_id_new\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex OwnerId3Regex = new Regex(":\"Page\",\"id\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex CreationTimeRegex = new Regex("creation_time\":(.*?),\"", RegexOptions.Singleline);

        public static readonly Regex AdIdRegex = new Regex("{\"ad_id\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex AdId2Regex = new Regex("\"ad_id\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex PhotoId = new Regex("photo_id\":\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex AriaLabelRegex = new Regex("aria-label=\"(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex CommentID = new Regex("comment_id=(.*?)\"", RegexOptions.Singleline);

        public static readonly Regex Comment2ID = new Regex("comment_id=(.*?)&", RegexOptions.Singleline);

        #endregion

        #region Web Header

        public static string Referer => $"{FbHomeUrl}";

        public static bool KeepAlive => true;

        public static string Host => "www.facebook.com";

        public static string FullInfoHost => "fullip.info";

        public static string UpgradeInSecureRequest => "1";

        public static string AcceptLanguage => "en-us,en;q=0.5";

        public static string AcceptCharset => "ISO-8859-1,utf-8;q=0.7,*;q=0.7";

        public static string AcceptType => "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";

        public static string ContentType => "application/x-www-form-urlencoded";

        public static string AcceptCharsetKey => "Accept-Charset";


        public static string AcceptLanguageKey => "Accept-Language";

        public static string UpgradeInsecureRequestkey => "Upgrade-Insecure-Requests";

        public static string HostKey => "Host";

        #endregion

        #region ConstantRequestParameters

        public static string DynParameter => "7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE";

        public static string PublicPostFilters => "eyJycF9hdXRob3IiOiJ7XCJuYW1lXCI6XCJtZXJnZWRfcHVibGljX3Bvc3RzXCIsXCJhcmdzXCI6XCJcIn0ifQ==";

        public static string JazoestParameterGender => "26581721229097781106845697858658171114102508185110764851";

        public static string FriendLinkDataGt(string friendId)
            => "48.{\"event\":\"add_friend\",\"intent_status\":null,\"intent_type\":null,\"profile_id\":" + friendId + ",\"ref\":1}";

        #endregion

        #region SendFreindRequest  

        public static string SendRequestVideoTutorialsLink => "https://www.youtube.com/watch?v=3TdwomVwPLs&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF";
        public static string SendRequestKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000034554-facebook-auto-send-friend-requests";


        #endregion

        #region ManageFriendRequest

        public static string IncommingFriendsVideoTutorialsLink => "https://www.youtube.com/watch?v=KfpyIWajOTg&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF&index=2";
        public static string IncommingFriendsKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000034588-facebook-auto-accept-friend-requests";


        #endregion

        #region Unfriend

        public static string UnfriendVideoTutorialsLink => "https://www.youtube.com/watch?v=KQBksQCg-ZM&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF&index=3";
        public static string UnfriendKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000034590-facebook-auto-unfriend";


        #endregion

        #region WithDraw

        public static string WithDrawVideoTutorialsLink => "https://www.youtube.com/watch?v=znJ3DvTWp_U&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF&index=4";
        public static string WithDrawKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000034592-facebook-auto-withdraw-sent-requests";


        #endregion

        #region ProfileScraper

        public static string ProfileScraperVideoTutorialsLink => "https://www.youtube.com/watch?v=cH5WKg11Ipk&index=17&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF";

        public static string ProfileScraperKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000034604-facebook-auto-scrape-users";



        #endregion

        #region FanpageScraper

        public static string FanpageScraperVideoTutorialsLink => "https://www.youtube.com/watch?v=j6uwoUK4LbE&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF&index=13";
        public static string FanpageScraperKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000034605-facebook-auto-scrape-fanpages";


        #endregion

        #region GroupScraper

        public static string GroupScraperVideoTutorialsLink => "https://www.youtube.com/watch?v=_W3W0caHeA4&index=11&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF";
        public static string GroupScraperKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000034606-facebook-auto-scrape-groups";


        #endregion

        #region CommentScraper

        public static string CommentScraperVideoTutorialsLink => "https://www.youtube.com/watch?v=nDPnbjGNSG4&index=22&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF";
        public static string CommentScraperKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000034608-facebook-auto-scrape-comments";


        #endregion

        #region PostScraper

        public static string PostScraperVideoTutorialsLink => "https://www.youtube.com/watch?v=Bfaf1Tdpznw&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF&index=16";
        public static string PostScraperKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000034610-facebook-auto-scrape-posts";


        #endregion

        public static string DownloadMediaVideoTutorialLInk => "https://www.youtube.com/watch?v=cyWgZzbaTs0&index=23&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF";

        #region GroupJoiner

        public static string GroupJoinerVideoTutorialsLink => "https://www.youtube.com/watch?v=w9WoqOkFgHA&index=15&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF";
        public static string GroupJoinerKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000034598-facebook-auto-join-groups";


        #endregion




        #region FanpageLiker

        public static string FanpageLikerVideoTutorialsLink = "https://www.youtube.com/watch?v=a6Sg8pSeYk4&index=12&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF";

        public static string FanpageLikerKnowledgeBaseLink = "https://help.socinator.com/support/solutions/articles/42000034602-facebook-auto-like-fanpages";




        #endregion


        #region PostLiker

        public static string PostLikerVideoTutorialsLink => "https://www.youtube.com/watch?v=f4_7gqRdWqU&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF&index=7";

        public static string PostLikerKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000034603-facebook-auto-like-and-comment-posts";

        public static string PostLikerContactLink => "";

        #endregion

        #region PostCommentor

        public static string PostCommentorVideoTutorialsLink => "https://www.youtube.com/watch?v=x9kSJiAAQEk&index=8&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF";

        public static string PostCommentorKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000034603-facebook-auto-like-and-comment-posts";

        #endregion

        #region GroupInviter

        public static string GroupInviterVideoTutorialsLink => "https://www.youtube.com/watch?v=eNgCFTaGKKA&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF&index=18";

        public static string GroupInviterKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000043887-facebook-auto-group-inviter";


        #endregion


        #region EventInviter

        public static string EventInviterVideoTutorialsLink => "";

        public static string EventInviterKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000049625-facebook-auto-event-inviter-";



        public static string EventUrl => $"{FbHomeUrl}events/";

        #endregion

        #region FanpageInvirer

        public static string PageInviterVideoTutorialsLink => "https://www.youtube.com/watch?v=-ppBuM2nfok&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF&index=19";

        public static string PageInviterKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000043888-facebook-auto-page-inviter";



        #endregion

        public static string WatcPartyInviterVideoTutorialLink => "https://www.youtube.com/watch?v=by72DKqbhjU&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF&index=21";




        #region MarketplaceScraper
        public static string MarketplaceScraperVideoTutorialsLink => "";

        public static string MarketplaceScraperKnowledgeBaseLink => "";


        #endregion

        public static string GroupUnJoinerKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000034600-facebook-auto-unjoin-groups";

        public static string GroupUnJoinerVideoTutorialsLink => "https://www.youtube.com/watch?v=sn4zxT3zT0c&index=14&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF";



        public static string BrodcastMessageVideoTutorialsLink => "https://www.youtube.com/watch?v=HZhAu6qltUE&index=5&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF";
        public static string BrodcastMessageKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000034594-facebook-auto-broadcast-messages";


        public static string PlaceScrapperVideoTutorialsLink => "https://www.youtube.com/watch?v=sHSeY8W3T28&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF&index=30";



        public static string ReplyMessageVideoTutorialsLink => "https://www.youtube.com/watch?v=TBTS3-o1phE&index=6&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF";
        public static string ReplyMessageKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000034596-facebook-auto-reply-to-new-messages";


        public static string SendMessageToNewFriendVideoTutorialsLink => "https://www.youtube.com/watch?v=1n5g9AUcXEU&index=9&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF";



        public static string SendGreetingsToFriendVideoTutorialsLink => "https://www.youtube.com/watch?v=DnsL-JpqrSY&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF&index=10";


        public static string AccountReport = "AccountReport";

        public static string ExtractPhotoIdUrl
                  => "https://upload.facebook.com/ajax/react_composer/attachments/photo/upload";

        public static string ExtractPhotoIdUrlForMessage
                  => "https://upload.facebook.com/ajax/mercury/upload.php";

        public static string ExtractVideoId
                 => "https://vupload-edge.facebook.com/ajax/video/upload/requests/receive/";

        public static string TagFriendsUrl
            => $"{FbHomeUrl}ajax/metacomposer/social_text.php";

        public static string BuySellPostComposerUrl
            => $"{FbHomeUrl}react_composer/sell/bootstrap/";

        public static string DeleteFromGroupUrl
            => $"{FbHomeUrl}ajax/groups/mall/delete/";

        public static string DeleteFromPageUrl
          => $"{FbHomeUrl}ajax/timeline/delete";

        public static string StopCommentingUrl
         => $"{FbHomeUrl}feed/ufi/disable_comments/";


        public static string StopNotificationUrl
         => $"{FbHomeUrl}ajax/litestand/follow_post";

        public static string LocationIdUrl
            => $"{FbHomeUrl}ajax/places/typeahead";

        public static string VideoUploadUrl
            => "https://vupload-edge.facebook.com/ajax/video/upload/requests/start/";

        public static string DownloadFolderPath
            => ConstantVariable.GetPlatformBaseDirectory() + @"\Other\FdVideos";



        public static string DownloadFolderPathDocuments
            => GetPlatformBaseDirectory() + $@"\{ConstantVariable.ApplicationName}\Fd\MediaFiles\{DateTime.Now:dd-MMM-yyyy}\";

        public static string DownloadFolderPathAlbums(string ownerName, string albumName)
            => GetPlatformBaseDirectory() + $@"\{ConstantVariable.ApplicationName}\Fd\MediaFiles\{ownerName}_{albumName}\";

        public static string GetPlatformBaseDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public static string FailedReponsePath
            => ConstantVariable.GetPlatformBaseDirectory() + @"\Other\FdLoginResponseCheck";

        public static string ContentTypeJson { get; internal set; } = "application/json";

        public static string SaveAdsPath { get; internal set; } =
           ConstantVariable.GetPlatformBaseDirectory() + @"\adsDataDevelopers.txt";

        public static string MessageToNewFriendsKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000049621-facebook-auto-message-to-new-friends-";

        public static string SendGreetingsKnowledgeBaseLink =>
            "https://help.socinator.com/support/solutions/articles/42000049622-facebook-auto-send-greetings-to-friends-";

        public static string CommentLikerKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000049623-facebook-auto-comment-liker-";


        public static string CommentLikerVideoTutoorialLink => "https://www.youtube.com/watch?v=GEczQFc9dKQ&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF&index=26";


        public static string ReplyToCommentKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000049624-facebook-auto-reply-to-comments-";

        public static string ReplyToCommentVideoTutorialLink => "https://www.youtube.com/watch?v=I1UGk0Gy6yw&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF&index=20";

        public static string WatchPartyInviterKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000049626-facebook-auto-watchparty-inviter-";

        public static string WatchPartyInviterVideoTutorialLink => "https://help.socinator.com/support/solutions/articles/42000049626-facebook-auto-watchparty-inviter-";

        public static string DownloadMediaKnowledgeBaseLink => "https://help.socinator.com/support/solutions/articles/42000043929-facebook-auto-download-media";

        public static string SendMessageToBasedOnLocationVideoTutorialsLink => "https://www.youtube.com/watch?v=CcxnL4uH3c4&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF&index=28";

        public static string MassageToFanPagesVideoTutorialLink => "https://www.youtube.com/watch?v=IV4gWA5YpCE&list=PL60e8mIWfxoZjNai77IyB7GViaxcV9EwF&index=27";


        public static string FdDefaultLanguage => "en_GB";

        public static string MarketplaceGraphUrl =>
             $"{FbHomeUrl}api/graphql/";

        public static string ChangeMarketplaceLocationUrl(string userId, string latitude, string longitude) =>
           "{\"input\":{\"client_mutation_id\":\"2\",\"actor_id\":\"" +
                  userId + "\",\"latitude\":" + latitude +
                       ",\"longitude\":" + longitude + "}}";

        public static string LocationDistanceVariables(string userId, string radius) =>
          "{\"input\":{\"client_mutation_id\":\"6\",\"actor_id\":\"" + userId + "\",\"browse_radius\":" + radius + "}}";

        public static long[] MarketplaceCategoryValues(string selectedCategory)
        {
            switch (selectedCategory)
            {
                case "All Marketplace":
                    return null;
                case "Vehicles":
                    return new long[1] { 807311116002614 };
                case "Home And Garden":
                    return new long[5] { 1670493229902393, 1583634935226685, 800089866739547, 678754142233400, 1569171756675761 };
                case "Housing":
                    return new long[2] { 821056594720130, 1468271819871448 };
                case "Entertainment":
                    return new long[2] { 613858625416355, 686977074745292 };
                case "Clothing And Accessories":
                    return new long[4] { 214968118845643, 1567543000236608, 931157863635831, 1266429133383966 };
                case "Family":
                    return new long[4] { 606456512821491, 624859874282116, 1550246318620997, 1555452698044988 };
                case "Electronics":
                    return new long[2] { 1557869527812749, 1792291877663080 };
                case "Hobbies":
                    return new long[6] { 1383948661922113, 757715671026531, 1658310421102081, 393860164117441, 676772489112490, 1534799543476160 };
                case "Classifieds":
                    return new long[2] { 1834536343472201, 895487550471874 };
                default:
                    return null;
            }
        }

        public static string MarketPlaceSortOptionValue(string sortByOption)
        {
            switch (sortByOption)
            {
                case "Recommended":
                    return "CREATION_TIME_DESCEND";
                case "Price High To Low":
                    return "PRICE_DESCEND";
                case "Price Low To High":
                    return "PRICE_ASCEND";
                case "Newest":
                    return "BEST_MATCH";
                default:
                    return "BEST_MATCH";
            }
        }

        #region UserClassNames
        public static string KeywordSectionalClass => "x78zum5 xdt5ytf xq8finb x1xmf6yo x1e56ztr x1n2onr6 xqcrz7y";

        public static string KeywordSectionalClass2 => "h8391g91 m0cukt09 kpwa50dg ta68dy8c b6ax4al1 k0wzcmhh";
        public static string KeywordSectionalClass4 => "x193iq5w x1gslohp x12nagc xzboxd6 x14l7nz5";
        public static string KeywordEntityOptionClass2 => "i85zmo3j jl2a5g8c b0eko5f3 fwlpnqze il7dmu95";
        public static string KeywordEntityOptionClass4 => "x1a2a7pz x1lq5wgf xgqcy7u x30kzoy x9jhf4c x1lliihq";

        public static string FriendUser2Element => "g4qalytl r227ecj6 ez8dtbzv gt60zsk1 hgcwkpcn";
        public static string FriendUser3Element => "xyamay9 x1pi30zi x1l90r2v x1swvt13 x1gefphp";


        public static string ReactionUserElement => "x1lq5wgf xgqcy7u x30kzoy x9jhf4c x1lliihq";
        public static string ReactionUserElement3 => "x1lq5wgf xgqcy7u x30kzoy x9jhf4c x1lliihq";

        public static string EventGuestUserElement => "ue3kfks5 pw54ja7n uo3d90p7 l82x9zwi";

        public static string SuggestedFriendsUserElement => "ue3kfks5 pw54ja7n uo3d90p7 l82x9zwi";
        public static string SuggestedFriendsUserElement2 => "x1lq5wgf xgqcy7u x30kzoy x9jhf4c x1lliihq";
        public static string FriendsCount => "x193iq5w xeuugli x13faqbe x1vvkbs x1xmvt09 x1lliihq x1s928wv xhkezso x1gmr53x x1cpjm7i x1fgarty x1943h6x xudqn12 x3x7a5m x1lkfr7t x1lbecb7 x1s688f xzsf02u x1yc453h";
        public static string FriendsCount2 => "x1heor9g x1qlqyl8 x1pd3egz x1a2a7pz x193iq5w xeuugli";
        public static string GroupsCount1 => "x1n2onr6 x1ja2u2z x9f619 x78zum5 xdt5ytf x2lah0s x193iq5w xjkvuk6 x1cnzs8";
        public static string GroupsCount2 => "x9f619 x1n2onr6 x1ja2u2z x78zum5 x2lah0s x1qughib x6s0dn4 xozqiw3 x1q0g3np xzt5al7";
        public static string GroupsCount3 => "x9f619 x1n2onr6 x1ja2u2z x78zum5 xdt5ytf x193iq5w xeuugli x1r8uery x1iyjqo2 xs83m0k";
        public static string ShareUserElementNewUi3 => "x1yztbdb";

        public static string NewsFeedPostElement => "_5jmm _5pat _3lb4";

        public static string CommentArea3Element => "x1r8uery x1iyjqo2 x6ikm8r x10wlt62 x1pi30zi";
        public static string CommentReplyText2Element => "x1n2onr6 x46jau6";
        public static string IncommingFriend2Element => "x1lq5wgf xgqcy7u x30kzoy x9jhf4c x1lliihq";
        public static string IncommingFriendPaginationElement => "hu5pjgll lzf7d6o1";
        public static string SentFriendElement => "ue3kfks5 pw54ja7n uo3d90p7 l82x9zwi";
        public static string SentFriend2Element => "x1mh8g0r x1y1aw1k x1sxyh0 xwib8y2 xurb0ha";
        public static string SentFriend3Element => "x1lq5wgf xgqcy7u x30kzoy x9jhf4c x1lliihq";


        public static string AddedFriend2 => "g4qalytl r227ecj6 ez8dtbzv gt60zsk1 hgcwkpcn";
        public static string AddedFriend3 => "xyamay9 x1pi30zi x1l90r2v x1swvt13 x1gefphp";

        public static string SentFriendPaginationElement => "pam uiBoxLightblue _5cz uiMorePagerPrimary";

        public static string AcceptRequestElement => "ni8dbmo4 stjgntxs l9j0dhe7 ltmttdrg";

        public static string CancelRequestElement => "ni8dbmo4 stjgntxs l9j0dhe7 ltmttdrg";
        public static string FriendsFanpageLikes2Element => "x1hl2dhg xggy1nq x1a2a7pz x1heor9g xt0b8zv";
        public static string FriendsFanpageLikes3Element => "x1pi30zi x1sy10c2 x1a02dak x1iyjqo2 xdt5ytf x78zum5 x1cy8zhl";

        public static string PlaceElement2 => "xt3gfkd xh8yej3 x6ikm8r x10wlt62 xquyuld";
        public static string SendGreetings2Element => "xt3gfkd xh8yej3 x6ikm8r x10wlt62 xquyuld";



        #region New Ui
        public static string PostElementClass => "du4w35lb k4urcfbm l9j0dhe7 sjgh65i0";
        public static string PostElement4Class => "x78zum5 x1n2onr6 xh8yej3";
        public static string PostElement5Class => "x1ja2u2z xh8yej3 x1n2onr6 x1yztbdb";


        public static string PostingTextLocClass => "rq0escxv datstx6m k4urcfbm a8c37x1j";
        public static string PostingTextLoc2Class => "bdao358l b6ax4al1 pytsy3co mfclru0v";
        public static string PostingTextLoc3Class => "x4uap5 xwib8y2 xkhd6sd xh8yej3 xha3pab";
        public static string PostCreate3Class => "x1ba4aug x1y1aw1k xn6708d xwib8y2 x1ye3gou";
        public static string KeywordClassElement2 => "x1yztbdb";
        public static string KeywordClassElement3 => "x78zum5 x1n2onr6 xh8yej3";
        public static string TextBoxElement => "_1mf _1mj";
        public static string PageNameElement => "x1e56ztr x1xmf6yo";
        public static string BuySellText2Element => "x1n2onr6 x1ja2u2z x1egnk41 x1ed109x x1a2a7pz";

        public static string MessageTextElement => "l9j0dhe7 i09qtzwb esma6hys j83agx80";
        public static string MessageText3Element => "x78zum5 x1iyjqo2 x1gja9t x16n37ib x1xmf6yo x1e56ztr xeuugli x1n2onr6";

        public static string ImagePositionElement => "taijpn5t l9j0dhe7 tdjehn4e qypqp5cg q676j6op";
        public static string UnfriendElement => "x1gefphp xyamay9 x1pi30zi x1l90r2v x1swvt13";

        #endregion


        public static string GroupUpdateUrl =>
            $"{FbHomeUrl}groups/?ref=bookmarks";
        public static string GroupJoinedUrl =>
            $"{FbHomeUrl}groups/joins/";
        #endregion

        #region CustomScripts
        

        public static string UploadPhotoScriptWithIndex =>
            "[...document.querySelectorAll('div[role=\"button\"],div[class*=\"x18d9i69 xkhd6sd x1n2onr6 x16tdsg8 x1ja2u2z\"]')].filter(x=>x.ariaLabel?.toLowerCase().includes(\"photo/video\"))[[...document.querySelectorAll('div[role=\"button\"],div[class*=\"x18d9i69 xkhd6sd x1n2onr6 x16tdsg8 x1ja2u2z\"]')].filter(x=>x.ariaLabel?.toLowerCase().includes(\"photo/video\")).length-1]";


        public static string GetlocFromRoleOptionXCoordinate(int index = 0) =>
            $"document.querySelectorAll('[role=\"option\"]')[{index}].getBoundingClientRect().x";
        public static string GetlocFromRoleOptionYCoordinate(int index = 0) =>
            $"document.querySelectorAll('[role=\"option\"]')[{index}].getBoundingClientRect().y";

        public static string ScriptforCreatePost =>
            "[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.innerText?.toLowerCase().includes(\"{0}\")||x.className?.includes(\"x1y1aw1k xn6708d xwib8y2 x1ye3gou\"))";
        public static string customScriptforWriteTextPost =>
            "[...document.querySelectorAll('div[role=\"textbox\"]')].filter(x=>x.ariaLabel?.toLowerCase().includes(\"{0}\")||x.className?.includes(\"xzsf02u x1a2a7pz x1n2onr6 x14wi4xw x9f619 x1lliihq x5yr21d xh8yej3 notranslate\"))";
        public static string CommentFilterLocScript =>
            "[...document.querySelectorAll('div[role=\"button\"],div[class*=\"x13rtm0m x1n2onr6 x87ps6o x1lku1pv x1a2a7pz\"]')].filter(x=>x.innerText?.includes(\"Most relevant\")||x.innerText?.includes(\"Most applicable\")||x.innerText?.includes(\"Most recent\")||x.innerText?.includes(\"All comments\")||x.innerText?.includes(\"Top comments\"))";
        public static string CommentPreviousReplyScript =>
            "[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.innerText?.includes(\"View previous replies\"))";
        public static string PostDateTimeScriptScriptByPostIndex(int postIndex) =>
            $"Array.from(document.querySelectorAll(':not([aria-label])[role=\"article\"]')[{postIndex}].querySelectorAll('span>a[role=\"link\"]')).filter(x=>x.href.includes(\"#\"))";
        public static string ShareLocScriptScriptByPostIndex(int postIndex) =>
            $"document.querySelectorAll('[aria-label=\"Send this to friends or post it on your profile.\"]')[{postIndex}]";
        public static string PostArticleScript =>
            $"[...document.querySelectorAll(':not([aria-label])[role=\"article\"]')]";
        public static string CommentTextBoxScript =>
            $"[...document.querySelectorAll('div')].filter(x=>x.ariaLabel===\"Write a comment…\"||x.ariaLabel===\"Write a public comment…\"||x.ariaLabel===\"Leave a comment\"||x.ariaLabel===\"Comment\")";
        public static string CommentButtonBoxScript =>
            $"[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.ariaLabel?.includes(\"Leave a comment\")||x.ariaLabel?.includes(\"Comment\"))[0]";
        public static string CommentWriteTextBoxScript =>
            $"[...document.querySelectorAll('div[role=\"textbox\"]')].filter(x=>x.ariaLabel===\"Write a comment…\"||x.ariaLabel===\"Write a public comment…\"||x.ariaLabel?.includes(\"Comment as \"))";
        public static string LikeButtonScript =>
            $"[...document.querySelectorAll('div[role=\"button\"],div[class*=\"x1q0g3np x87ps6o x1lku1pv x1a2a7pz x5ve5x3\"]')].filter(x=> x.ariaLabel===\"Like\"&&(x.innerHTML?.includes(\"data-visualcompletion\")||x.innerHTML?.includes(\"background-image\")))";

        public static string AlreadyLikedButtonScript(string attribute) =>
           $"[...document.querySelectorAll('div[role=\"button\"],div[class*=\"x1q0g3np x87ps6o x1lku1pv x1a2a7pz x5ve5x3\"]')].filter(x=> x.ariaLabel===\"{attribute}\"&&(x.innerHTML?.includes(\"data-visualcompletion\")||x.innerHTML?.includes(\"background-image\")||(x.innerHTML?.includes(\"<img\")&&x.innerHTML?.includes(\"src=\"))))";
        public static string OtherReactionButtonScript =>
            $"[...document.querySelectorAll('div[role=\"button\"],div[class*=\"x1q0g3np x87ps6o x1lku1pv x1a2a7pz x5ve5x3\"]')].filter(x=> (x.ariaLabel===\"Remove Care\"||x.ariaLabel===\"Remove Love\"||x.ariaLabel===\"Remove Sad\"||x.ariaLabel===\"Remove Wow\"||x.ariaLabel===\"Remove Haha\"||x.ariaLabel===\"Remove Angry\")&&(x.innerHTML?.includes(\"<img alt=\\\"Angry\\\"\")||x.innerHTML?.includes(\"<img alt=\\\"Sad\\\"\")||x.innerHTML?.includes(\"<img alt=\\\"Wow\\\"\")||x.innerHTML?.includes(\"<img alt=\\\"Haha\\\"\")||x.innerHTML?.includes(\"<img alt=\\\"Care\\\"\")||x.innerHTML?.includes(\"<img alt=\\\"Love\\\"\")))";
        public static string GetStartedButtonScript =>
            $"[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.innerText?.includes(\"Get Started\"))";
        public static string feedButtonScript =>
            $"[...document.querySelectorAll('div>a[role=\"link\"]')].filter(x=>x.innerText?.includes(\"Feeds\"))";
        public static string AddFriendScript =>
            "[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.ariaLabel?.includes(\"Add friend\")||x.ariaLabel?.includes(\"Add Friend\"))";
        public static string SHareButtonScript =>
            $"[...[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.textContent?.toLowerCase().includes(\"shares\")),...[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>(x.className?.includes(\"x16tdsg8 xt0b8zv x1hl2dhg x1ja2u2z\")||x.className?.includes(\"xkhd6sd x1n2onr6 x16tdsg8 x1ja2u2z\")||x.innerHTML?.includes(\"background-image: url(&quot;https://static.xx.fbcdn.net/rsrc.php/v3/y3/r/JC3GCxn_mNT.png&quot;)\"))&&x.id===\"\")]";

        public static string ByTagRoleAttributeButtonScript =>
           "[...document.querySelectorAll('{0}[role=\"{1}\"]')].filter(x=>x.{2}?.includes(\"{3}\"))";
        
        public static string ByTagAttributeValueButtonScript =>
           "[...document.querySelectorAll('{0}[{1}=\"{2}\"]')].filter(x=>x.{3}?.toLowerCase().includes(\"{4}\"))";
        public static string ByTag2AttributeValueButtonScript =>
           "[...document.querySelectorAll('{0}[{1}=\"{2}\"]')].filter(x=>x.{3}?.toLowerCase().includes(\"{4}\")||x.{5}?.toLowerCase().includes(\"{6}\"))";
        public static string StoryCreateButtonScript =>
           $"[...document.querySelectorAll('div>a[role=\"link\"]')].filter(x=>x.ariaLabel?.toLowerCase().includes(\"create story\")||x.innerText?.toLowerCase().includes(\"create story\")||x.innerText?.toLowerCase().includes(\"share a photo\")||x.innerText?.toLowerCase().includes(\"story\"))";
        public static string UploadPhotoButtonScript =>
           $"[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.ariaLabel?.toLowerCase().includes(\"upload photo\")||x.innerText?.toLowerCase().includes(\"create a photo story\"))";
        public static string CreateTextStoryButtonScript =>
          $"[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.innerText?.toLowerCase().includes(\"create a text story\")||x.innerText?.toLowerCase().includes(\"text story\"))";
        public static string WriteTextButtonForImageScript =>
          $"[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.innerText?.toLowerCase().includes(\"add text\"))";
        public static string StartTypingButtonForImageScript =>
          $"[...document.querySelectorAll('div[role=\"textbox\"]')].filter(x=>x.ariaLabel?.toLowerCase().includes(\"start typing\")||x.ariaLabel?.toLowerCase().includes(\"typing\"))";
        public static string StartTypingButtonForTextScript =>
          $"[...document.querySelectorAll('div>textarea')]";
        public static string AddPhotoorVedioScript =>
          $"[...document.querySelectorAll('div[role=\"button\"],div[class*=\"x3nfvp2 x1q0g3np x87ps6o x1lku1pv x1a2a7pz\"]')].filter(x=>(x.innerText?.toLowerCase().includes(\"add photos/videos\")&&x.innerText?.toLowerCase().includes(\"or drag and drop\"))||(x.textContent?.toLowerCase().includes(\"add photos/videos\")&&x.textContent?.toLowerCase().includes(\"or drag and drop\")))";
        public static string MessageBoxScript =>
          $"[...document.querySelectorAll('div[role=\"textbox\"]')].filter(x=>x.ariaLabel?.toLowerCase().includes(\"message\")||x.getAttribute(\"aria-describedby\")?.toLowerCase().includes(\"write to \"))";

        public static string AudianceButtonScript =>
            "[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.ariaLabel?.toLowerCase().includes(\"edit privacy. sharing with\")).filter(x=>x.innerText!='')";

        public static string SendInMessageButtonScript =>
       "[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.innerText?.toLowerCase().includes(\"send in messenger\") ||x.innerText?.toLowerCase().includes(\"messenger\"))";
        public static string InviteEventButtonScript =>
            "document.querySelectorAll('div[role=\"button\"][aria-label=\"Invite\"')[0].click()";
        public static string UserSendButtonScript(string userName) =>
            $"[...document.querySelectorAll('div[role=\"listitem\"]')].filter(x=>x.innerText?.includes(\"{userName}\\nSend\"))[0].querySelectorAll('div[role=\"button\"][aria-label=\"Send\"]')[0].click()";
        public static string CancelSendReqButtonScript(string userName, string userId)
        {
            if (!string.IsNullOrEmpty(userName))
                return $"[...document.querySelectorAll('div>a[role=\"link\"]')].filter(x=>x.innerText?.includes(\"{userName}\")&&x.innerText?.includes(\"Cancel request\"))[0].querySelectorAll('div[role=\"button\"][aria-label=\"Cancel request\"]')";
            else
                return $"[...document.querySelectorAll('div>a[role=\"link\"]')].filter(x=>x.href?.includes(\"{userId}\")&&x.innerText?.includes(\"Cancel request\"))[0].querySelectorAll('div[role=\"button\"][aria-label=\"Cancel request\"]')";
        }
        public static string MemberCountScript2 =>
            $"[...document.querySelectorAll('div>h2>span')].filter(x=>x.textContent?.includes(\"Members\")||x.textContent?.includes(\"total members\"))[0].textContent";
        public static string MemberCountScript =>
            $"[...document.querySelectorAll('div>h2>span')].filter(x=>(x.textContent?.includes(\"Members\")||x.textContent?.includes(\"total members\"))&&/\\d/.test(x.textContent))[0].textContent";

        public static string SwitchProfileButton =>
            $"[...document.querySelectorAll('div[aria-label=\"Account controls and settings\"]')[0].querySelectorAll('div[role=\"button\"]')].filter(x=>x.innerHTML.includes(\"aria-label=\\\"Your profile\"))";
        public static string UserSwitchButtonScript(string userName) =>
            $"[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.ariaLabel?.includes(\"Switch to {userName}\"))";

        public static string PageSwitchButtonScript =>
           $"[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.ariaLabel?.includes(\"Available Voices\")||x.ariaLabel?.includes(\"Voice Selector\"))";

        public static string PageButtonScriptBYName(string userName) =>
          $"[...document.querySelectorAll('div[role=\"button\"],div[role=\"listitem\"],div[role=\"radio\"],div[role=\"menuitemradio\"]')].filter(x=>x.textContent?.includes(\"{userName}\"))";
        public static string LoginButtonScriptBYClass(string className) =>
          $"[...document.querySelectorAll('div>button')].filter(x=>x.name?.includes(\"login\")||x.id?.includes(\"loginbutton\")||x.className?.includes(\"{className}\"))";
        public static string PagesButtonScript =>
          $"[...document.querySelectorAll('div>a[role=\"link\"]')].filter(x=>x.href?.includes(\"{FbHomeUrl}pages/\"))";
        public static string ReplytoCommentButtonScript =>
     $"[...document.querySelectorAll('div[role=\"textbox\"]')].filter(x=>x.ariaLabel?.toLowerCase().includes(\"reply to \"))";
        public static string TagButtonScript =>
     $"[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.ariaLabel?.toLowerCase().includes(\"tag people\")||x.ariaLabel?.toLowerCase().includes(\"tag friends\"))";
        public static string SearchTagPeopleButtonScript =>
        $"[...document.querySelectorAll('[role=\"textbox\"][type=\"search\"]')].filter(x=>x.ariaLabel?.toLowerCase().includes(\"search for friends\")||x.ariaLabel?.toLowerCase().includes(\"search for people\"))";
        public static string DoneTagPeopleButtonScript =>
        $"[...document.querySelectorAll('div')].filter(x=>x.role===\"button\"&&x.textContent?.toLowerCase().includes(\"done\")||x.className?.includes(\"xexx8yu x1sxyh0 x18d9i69 x5ib6vp\"))";
        public static string SendMessageButtonScript =>
        $"[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.ariaLabel?.toLowerCase()===\"press enter to send\"||x.ariaLabel?.toLowerCase()===\"send\")";

        public static string ConfirmReqButtonScript =>
        $"[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>(x.ariaLabel?.toLowerCase().includes(\"confirm request\")||x.ariaLabel?.toLowerCase().includes(\"confirm\"))&&x.ariaDisabled!=\"true\")";
        public static string DeleteReqButtonScript =>
       $"[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>(x.ariaLabel?.toLowerCase().includes(\"delete request\")||x.ariaLabel?.toLowerCase().includes(\"delete\"))&&x.ariaDisabled!=\"true\")";

        public static string SearchFacebookButtonScript =>
      $"[...document.querySelectorAll('input[type=\"search\"]')].filter(x=>x.ariaLabel?.toLowerCase()===\"search facebook\"||x.placeholder?.toLowerCase()===\"search facebook\")";

        public static string MessageWindowButtonScript =>
      $"[...document.querySelectorAll('div[role=\"button\"]')].filter(x=>x.ariaLabel?.toLowerCase()===\"message\"||x.ariaLabel?.toLowerCase()===\"send message\")";
        public static string FilterButtonScript =>
          "[...document.querySelectorAll('{0}[role=\"{1}\"]')].filter(x=>x.ariaLabel?.toLowerCase()===\"{2}\")";
        public static string MessageChatsScript =>
          "[...document.querySelectorAll('div>a[role=\"link\"]')].filter(x=>x.href?.includes(\"/messages/t/\"))[[...document.querySelectorAll('div>a[role=\"link\"]')].filter(x=>x.href?.includes(\"/messages/t/\")).length-1]";
        public static string reactionButtonScript =>
            $"[...document.querySelectorAll('span>div[role=\"button\"]')].filter(x=>x.innerText?.toLowerCase().includes(\"all reactions:\")||x.className?.includes(\"x6ikm8r x10wlt62 x1vjfegm x1lliihq\"))";
        public static string PostCommentScript =>
           "[...document.querySelectorAll('div[role=\"article\"]')].filter(x=>x.ariaLabel?.includes(\"Comment by {0}\"))";
        public static string PostCommentReplyScript =>
          "[...document.querySelectorAll('div[role=\"article\"]')].filter(x=>x.ariaLabel?.includes(\"Reply by \"))";
        public static string GroupDiscussionScript =>
           "[...document.querySelectorAll('[role=\"tab\"]')].find(x=>x.innerText===\"Discussion\")";

        public static string PostFromInputBoxButton =>
          "[...document.querySelectorAll('div>input[role=\"combobox\"]')].filter(x=>x.ariaLabel===\"Posts from\")";
        public static string PublicPostOptionsButton =>
          "[...document.querySelectorAll('ul>li[role=\"option\"]')].filter(x=>x.id===\"Public posts\")";
        #endregion

        public static ConcurrentDictionary<string, FacebookAdsDetails> ListOfAds = new ConcurrentDictionary<string, FacebookAdsDetails>();
        public static string GetScriptforTwoAttributesAndValue(ActType actType, AttributeType attributeType1,
            string attributeValue1, AttributeType attributeType2,
            string attributeValue2, ValueTypes valueType = ValueTypes.InnerHtml, int clickIndex = 0, string value = "")
        {
            var descrptionforAttribute1 = attributeType1 == AttributeType.ClassName ? "class" : attributeType1.GetDescriptionAttr();
            var z = $"document.querySelectorAll('[{descrptionforAttribute1}=\"{attributeValue1}\"][{attributeType2.GetDescriptionAttr()}=\"{attributeValue2}\"]')[{clickIndex}].{valueType.GetDescriptionAttr()}";
            switch (actType)
            {
                case ActType.Click:
                    z = $"document.querySelectorAll('[{descrptionforAttribute1}=\"{attributeValue1}\"][{attributeType2.GetDescriptionAttr()}=\"{attributeValue2}\"]')[{clickIndex}].click()";
                    break;
                case ActType.GetValue:
                    z = $"document.querySelectorAll('[{descrptionforAttribute1}=\"{attributeValue1}\"][{attributeType2.GetDescriptionAttr()}=\"{attributeValue2}\"]')[{clickIndex}].{valueType.GetDescriptionAttr()}";
                    break;

                case ActType.GetLength:
                    z = $"document.querySelectorAll('[{descrptionforAttribute1}=\"{attributeValue1}\"][{attributeType2.GetDescriptionAttr()}=\"{attributeValue2}\"]').length";
                    break;
            }
            return z;
        }
        public static FacebookUser getFaceBookUserFromUrlOrIdOrUserName(string urlorId)
        {
            var facebookUser = new FacebookUser();
            try
            {

                if (urlorId.StartsWith(FbHomeUrl))
                {
                    facebookUser.ProfileUrl = urlorId;
                    if (urlorId.Contains("profile.php?id="))
                        facebookUser.UserId = Regex.Split(urlorId, "id=")?.LastOrDefault();
                    else if (!urlorId.EndsWith("/"))
                    {
                        facebookUser.Username = Regex.Split(urlorId, "/")?.LastOrDefault();
                        facebookUser.ProfileId = facebookUser.Username;
                    }
                    else if (Regex.IsMatch(urlorId, "facebook.com/(.*?)/"))
                    {
                        facebookUser.Username = Utilities.GetBetween(urlorId, "facebook.com/", "/");
                        facebookUser.ProfileId = facebookUser.Username;
                    }

                }
                if (!urlorId.StartsWith(FbHomeUrl) && FdFunctions.FdFunctions.IsIntegerOnly(urlorId))
                {
                    facebookUser.UserId = urlorId;
                    facebookUser.ProfileId = urlorId;
                    facebookUser.ProfileUrl = $"{FbHomeUrl}profile.php?id={urlorId}";
                }
                if (!urlorId.StartsWith(FbHomeUrl) && !FdFunctions.FdFunctions.IsIntegerOnly(urlorId))
                {
                    facebookUser.UserId = urlorId;
                    facebookUser.Username = urlorId;
                    facebookUser.ProfileUrl = $"{FbHomeUrl}{urlorId}";
                    facebookUser.ProfileId = facebookUser.Username;
                }
                facebookUser.ScrapedProfileUrl = facebookUser.ProfileUrl;
            }
            catch (Exception e) { e.DebugLog(); }
            return facebookUser;
        }
        public static FanpageDetails getFanPageFromUrlOrIdOrUserName(string urlorId)
        {
            var fanpageUser = new FanpageDetails();
            try
            {

                if (urlorId.StartsWith(FbHomeUrl))
                {
                    fanpageUser.FanPageUrl = urlorId;
                    if (urlorId.Contains("profile.php?id="))
                        fanpageUser.FanPageID = Regex.Split(urlorId, "id=")?.LastOrDefault();
                    else if (!urlorId.EndsWith("/"))
                        fanpageUser.FanPageID = Regex.Split(urlorId, "/")?.LastOrDefault();
                    else if (Regex.IsMatch(urlorId, "facebook.com/(.*?)/"))
                        fanpageUser.FanPageID = Utilities.GetBetween(urlorId, "facebook.com/", "/");

                }
                if (!urlorId.StartsWith(FbHomeUrl) && FdFunctions.FdFunctions.IsIntegerOnly(urlorId))
                {
                    fanpageUser.FanPageID = urlorId;
                    fanpageUser.FanPageUrl = $"{FbHomeUrl}profile.php?id={urlorId}";
                }
                if (!urlorId.StartsWith(FbHomeUrl) && !FdFunctions.FdFunctions.IsIntegerOnly(urlorId))
                {
                    fanpageUser.FanPageID = urlorId;
                    fanpageUser.FanPageUrl = $"{FbHomeUrl}{urlorId}";
                }
            }
            catch (Exception e) { e.DebugLog(); }
            return fanpageUser;
        }
        public static GroupDetails getFacebookGroupFromUrlOrId(string urlorId)
        {
            var facebookGroup = new GroupDetails();
            try
            {
                if (urlorId.StartsWith(FbHomeUrl))
                {
                    facebookGroup.GroupUrl = urlorId;
                    if (!urlorId.EndsWith("/"))
                        facebookGroup.GroupId = Regex.Split(urlorId, "/")?.LastOrDefault();
                    else if (Regex.IsMatch(urlorId, "/groups/(.*?)/"))
                        facebookGroup.GroupId = Utilities.GetBetween(urlorId + "/", "/groups/", "/");

                }
                if (!urlorId.StartsWith(FbHomeUrl))
                {
                    facebookGroup.GroupId = urlorId;
                    facebookGroup.GroupUrl = $"{FbHomeUrl}groups/{urlorId}";
                }
            }
            catch (Exception e) { e.DebugLog(); }
            return facebookGroup;
        }
        public static FacebookPostDetails getFaceBookPOstFromUrlOrId(string urlorId)
        {
            var facebookpost = new FacebookPostDetails();
            try
            {
                facebookpost.QueryValue = urlorId;
                if (urlorId.StartsWith(FbHomeUrl))
                {
                    var id = "";
                    if (urlorId.Contains("/posts/") || urlorId.Contains("/vedios/"))
                    {
                        if (urlorId.Contains("/vedios/")) urlorId.Replace("/vedios/", "/posts/");
                        if (urlorId.StartsWith($"{FbHomeUrl}groups/"))
                        {
                            facebookpost.OwnerId = Utilities.GetBetween(urlorId, $"{FbHomeUrl}groups/", "/posts/");
                        }
                        else
                        {
                            facebookpost.OwnerId = Utilities.GetBetween(urlorId, $"{FbHomeUrl}", "/posts/");
                        }
                        id = Utilities.GetBetween(urlorId + "/end", "/posts/", "/end").TrimEnd('/');
                    }
                    if (urlorId.Contains("watch/?v="))
                    {
                        id = Utilities.GetBetween(urlorId + "/end", "watch/?v=", "/end").TrimEnd('/');
                    }

                    if (urlorId.Contains("permalink.php?story_fbid"))
                    {
                        facebookpost.OwnerId = Utilities.GetBetween(urlorId + "/end", "&id=", "/end");
                        id = Utilities.GetBetween(urlorId, $"permalink.php?story_fbid=", "&id=");

                    }
                    if (urlorId.Contains("/reel/"))
                        id = Utilities.GetBetween(urlorId + "/end", "/reel/", "/end").TrimEnd('/');

                    if (!string.IsNullOrEmpty(id))
                        facebookpost.Id = id;

                    facebookpost.PostUrl = urlorId;
                    facebookpost.EntityId = facebookpost.OwnerId;
                }

                if (!urlorId.StartsWith(FbHomeUrl))
                {
                    facebookpost.Id = urlorId;
                    facebookpost.PostUrl = $"{FbHomeUrl}{facebookpost.Id}";

                }
                facebookpost.ScapedUrl = facebookpost.PostUrl;
            }
            catch (Exception e) { e.DebugLog(); }
            return facebookpost;
        }
        public static string getCountryFromAddress(string address)
        {
            string country = "";
            string[] addressArray = address.Replace(",", " ").Split(' ');
            if (addressArray.Length > 0)
            {
                addressArray.Reverse().ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x) && IsCountryNameValid(x))
                    {
                        country = x;
                        return;
                    }
                });
            }
            return country;
        }
        public static bool IsCountryNameValid(string countryName)
        {
            List<string> countryNames = CultureInfo
                           .GetCultures(CultureTypes.SpecificCultures)
                           .Where(x => !x.IsNeutralCulture)
                           .Select(x =>
                           {
                               try
                               {
                                   return new RegionInfo(x.Name).EnglishName;
                               }
                               catch (ArgumentException)
                               {
                                   return null;
                               }
                           })
                           .Where(name => name != null && !string.IsNullOrEmpty(name))
                           .Distinct()
                           .ToList();
            return countryNames.Contains(countryName, StringComparer.OrdinalIgnoreCase);
        }

    }
}
