using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using TumblrDominatorCore.Models;
//using ActType = DominatorHouseCore.PuppeteerBrowser.ActType;
//using ValueTypes = DominatorHouseCore.PuppeteerBrowser.ValueTypes;
//using AttributeType = DominatorHouseCore.PuppeteerBrowser.AttributeType;
namespace TumblrDominatorCore.TmblrUtility
{
    public class ConstantHelpDetails
    {
        public static string TumblrUrl => "https://www.tumblr.com";
        #region Upload Error Response.
        public static string ProcessingMedia = "We are still processing your video, it will take a moment to save your post";
        public static string ProcessingMediaJsonResponse = "Your video is still processing, please wait a few minutes for it to appear.";
        #endregion
        #region Login Module
        public static string BearerToken => "Bearer aIcXSOoTtqrzR8L8YEIOmBeW94c3FmbSNSWAUbxsny9KKx5VFh";
        public static string LoginUrl => "https://www.tumblr.com/login";
        public static string FollowingUrl => "https://www.tumblr.com/following";
        public static string AuthUrl => "https://www.tumblr.com/api/v2/oauth2/token";
        public static string ServiceApiUrl => "https://www.tumblr.com/services/bblog";
        #endregion
        #region Follower Module

        public static string FollowVideoTutorialsLink { get; set; } = "https://www.youtube.com/watch?v=USSjtU2StE";

        public static string FollowKnowledgeBaseLink { get; set; } =
            "https://help.socinator.com/support/solutions/articles/42000034790-tumblr-auto-follow";

        #endregion

        #region UnFollower Module

        public static string UnFollowVideoTutorialsLink { get; set; } = "https://www.youtube.com/watch?v=DdkmmTs0rqI";

        public static string UnFollowKnowledgeBaseLink { get; set; } =
            "https://help.socinator.com/support/solutions/articles/42000034791-tumblr-auto-unfollow";

        #endregion

        #region PostScraper Moduel

        public static string PostScraperVideoTutorialLink { get; } = "https://www.youtube.com/watch?v=fJjW3vri4qU";
        public static string PostScraperKnowledgeBaseLink { get; } =
            "https://help.socinator.com/support/solutions/articles/42000038154-auto-tumblr-post-settings";

        #endregion

        #region UserScraper Module

        public static string UserScraperVideoTutorialLink { get; } = "https://www.youtube.com/watch?v=6JtQFmB7wSQ";
        public static string UserScraperKnowledgeBaseLink { get; } = "https://help.socinator.com/support/home";
        #endregion

        #region LikeModule

        public static string LikeVideoTutorialsLink { get; set; } = "https://www.youtube.com/watch?v=R1fsJh5l5Vc";

        public static string LikeKnowledgeBaseLink { get; set; } =
            "https://help.socinator.com/support/solutions/articles/42000034792-tumblr-auto-like";
        public static string LikeAPIUrl { get; set; } = "https://www.tumblr.com/api/v2/user/like";


        #endregion

        #region UnLike Module

        public static string UnlikeVideoTutorialLink { get; } = "not awailable";
        #endregion

        #region Comment and Comment Scraper

        public static string CommentVideoTutorialsLink { get; set; } = "https://www.youtube.com/watch?v=ZPOxemJQ5Tk";

        public static string CommentKnowledgeBaseLink { get; set; } =
            "https://help.socinator.com/support/solutions/articles/42000034793-tumblr-auto-comment";
        public static string CommentScraperTutorialLink { get; set; } = "https://www.youtube.com/watch?v=2-4a_dj1X2U";



        #endregion

        #region Reblog

        public static string ReblogVideoTutorialsLink { get; set; } = "https://www.youtube.com/watch?v=AK_pt9zP8Yw";

        public static string ReblogKnowledgeBaseLink { get; set; } =
            "https://help.socinator.com/support/solutions/articles/42000034795-tumblr-auto-reblog";

        public static string ReblogContactLink { get; set; } = "https://socinator.com/contact-us/";

        #endregion

        #region BroadcastMessages

        public static string BroadcastMessagesVideoTutorialsLink { get; set; } = "https://www.youtube.com/watch?v=Rs5wb7nRWD8";

        public static string BroadcastMessagesKnowledgeBaseLink { get; set; } =
            "https://help.socinator.com/support/solutions/articles/42000035868-tumblr-auto-broadcast-messages";

        #endregion

        #region BrowserAutomation Constants

        public static string LoginPageKeyword { get; set; } =
            "{\"meta\":{\"status\":200,\"msg\":\"OK\"},\"response\":{\"user\":{\"";


        public static string ReblogButtonScript =>
          $"[...document.querySelectorAll('button>span')].filter(x=>x.innerText?.toLowerCase().includes(\"reblog\")||x.className?.includes(\"TRX6J VxmZd\"))[0]";
        public static string SubmitClass { get; set; } = "k76lX";
        public static string TextAreaClass { get; set; } = "xXTjk";
        public static string posTextClickScript =>
            "Array.from(document.querySelectorAll('p[role=\"document\"]')).filter(x=>x.ariaLabel?.toLowerCase().includes(\"start writing or type forward slash\"))";
        public static string CommentClass { get; set; } = "TRX6J v6i4P";

        public static string replyTextAreaScript = "document.querySelectorAll('textarea[aria-label=\"Reply\"]')[0]";

        public static string FollowButtonScript = "document.querySelectorAll('div>button[aria-label=\"Follow\"]')[0]";

        public static string PostReplyScript = "document.querySelectorAll('div[aria-label=\"Reply\"]')";

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
        public static KeyValuePair<int, int> GetScreenResolution()
        {
            int height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            int width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            return new KeyValuePair<int, int>(width, height);
        }
        #endregion
        #region general
        public static string Dashboard { get; set; } = "Dashboard";
        public static string following { get; set; } = "following";
        public static string HomePageUrl => "https://www.tumblr.com/dashboard";
        public static string BlogViewUrl(string username) => $"https://www.tumblr.com/blog/{username}";

        public static string GetPostUrlByUserNameAndPostId(string username, string postId) => $"https://www.tumblr.com/{username}/{postId}";

        public static string OwnFollowersUrl(string username) => $"https://www.tumblr.com/blog/{username}/followers";

        public static string SomeOnesFollowingsUrl(string username) => $"https://www.tumblr.com/{username}/following";

        public static string GetUserStarterInfo() =>
             "https://www.tumblr.com/api/v2/user/info?fields%5Bblogs%5D=avatar%2Cname%2Ctitle%2Curl%2Ccan_message%2Cdescription%2Cis_adult%2Cuuid%2Cis_private_channel%2Cposts%2Cis_group_channel%2C%3Fprimary%2C%3Fadmin%2C%3Fdrafts%2C%3Ffollowers%2C%3Fqueue%2C%3Fhas_flagged_posts%2C%3Ftweet%2Cmention_key%2C%3Ftimezone_offset%2C%3Fanalytics_url";


        public static string GetUSerUnfollowAPI(string username) => $"https://www.tumblr.com/{username}";

        public static string GetUserDetailsAPIByUserName(string username) =>
         $"https://www.tumblr.com/api/v2/blog/{username}/posts?fields%5Bblogs%5D=name%2Cavatar%2Ctitle%2Curl%2Cblog_view_url%2Cis_adult%2C%3Fis_member%2Cdescription_npf%2Cuuid%2Ccan_be_followed%2C%3Ffollowed%2C%3Fadvertiser_name%2Ctheme%2C%3Fprimary%2C%3Fis_paywall_on%2C%3Fpaywall_access%2C%3Fsubscription_plan%2Ctumblrmart_accessories%2Ccan_show_badges%2Cshare_likes%2Cshare_following%2Ccan_subscribe%2Csubscribed%2Cask%2C%3Fcan_submit%2C%3Fis_blocked_from_primary%2C%3Fis_blogless_advertiser%2C%3Ftweet%2Cis_password_protected%2C%3Fadmin%2Ccan_message%2Cask_page_title%2C%3Fanalytics_url%2C%3Ftop_tags%2C%3Fallow_search_indexing%2Cis_hidden_from_blog_network%2C%3Fshould_show_tip%2C%3Fshould_show_gift%2C%3Fshould_show_tumblrmart_gift%2C%3Fcan_add_tip_message&npf=true&reblog_info=true&include_pinned_posts=true";


        public static string GetUserPostDetailsAPIByName(string username, string postId)
        {
            return $"https://www.tumblr.com/api/v2/blog/{username}/posts/{postId}/permalink?fields%5Bblogs%5D=name%2Cavatar%2Ctitle%2Curl%2Cblog_view_url%2Cis_adult%2C%3Fis_member%2Cdescription_npf%2Cuuid%2Ccan_be_followed%2C%3Ffollowed%2C%3Fadvertiser_name%2Ctheme%2C%3Fprimary%2C%3Fis_paywall_on%2C%3Fpaywall_access%2C%3Fsubscription_plan%2Ctumblrmart_accessories%2Ccan_show_badges%2Cshare_likes%2Cshare_following%2Ccan_subscribe%2Csubscribed%2C%3Fallow_search_indexing%2Cask%2C%3Fcan_submit%2C%3Fis_blocked_from_primary%2C%3Fanalytics_url&reblog_info=true";
            //return $"https://www.tumblr.com/api/v2/blog/{username}/posts/{postId}/permalink?fields%5Bblogs%5D=name%2Cavatar%2Ctitle%2Curl%2Cblog_view_url%2Cis_adult%2C%3Fis_member%2Cdescription_npf%2Cuuid%2Ccan_be_followed%2C%3Ffollowed%2C%3Fadvertiser_name%2Ctheme%2C%3Fprimary%2C%3Fis_paywall_on%2C%3Fpaywall_access%2C%3Fsubscription_plan%2Ctumblrmart_accessories%2C%3Flive_now%2Ccan_show_badges%2Cshare_likes%2Cshare_following%2Ccan_subscribe%2Csubscribed%2Cask%2C%3Fcan_submit%2C%3Fis_blocked_from_primary%2C%3Fanalytics_url&reblog_info=true";
        }
        public static string GetUsersAPIByKeyWordOrHashTagValue(string keywordorHashTagValue)
        {
            return $"https://www.tumblr.com/api/v2/timeline/search?limit=20&days=0&query={Uri.EscapeDataString(keywordorHashTagValue)}&mode=top&timeline_type=post&skip_component=related_tags%2Cblog_search&reblog_info=true&query_source=search_box_typed_query&fields%5Bblogs%5D=name%2Cavatar%2Ctitle%2Curl%2Cblog_view_url%2Cis_adult%2C%3Fis_member%2Cdescription_npf%2Cuuid%2Ccan_be_followed%2C%3Ffollowed%2Ccan_message%2C%3Fadvertiser_name%2Ctheme%2C%3Fprimary%2C%3Fis_paywall_on%2C%3Fpaywall_access%2C%3Fsubscription_plan%2Ctumblrmart_accessories%2Ccan_show_badges%2C%3Fcan_be_booped%2Cshare_following%2Cshare_likes%2Cask";
        }
        public static string GetUsersAPIByUSingKeyWordOrHashTagValueAndCursor(string keywordorHashTagValue, string cursor)
        {
            return $"https://www.tumblr.com/api/v2/timeline/search?limit=20&days=0&query={Uri.EscapeDataString(keywordorHashTagValue)}&mode=top&timeline_type=post&skip_component=related_tags%2Cblog_search&reblog_info=true&fields%5Bblogs%5D=name%2Cavatar%2Ctitle%2Curl%2Cblog_view_url%2Cis_adult%2C%3Fis_member%2Cdescription_npf%2Cuuid%2Ccan_be_followed%2C%3Ffollowed%2Ccan_message%2C%3Fadvertiser_name%2Ctheme%2C%3Fprimary%2C%3Fis_paywall_on%2C%3Fpaywall_access%2C%3Fsubscription_plan%2Ctumblrmart_accessories%2Ccan_show_badges%2C%3Flive_now%2Cshare_following%2Cshare_likes%2Cask&cursor={cursor}";
        }
        public static string GetUserFollowersAPIByUserName(string username)
        {
            return $"https://www.tumblr.com/api/v2/blog/{username}/followers?fields%5Bblogs%5D=name%2Cavatar%2Ctitle%2Curl%2Cuuid%2Cdescription_npf%2Ccan_be_followed%2C%3Ffollowed%2Ccan_message%2C%3Fduration_blog_following_you%2C%3Fduration_following_blog";
        }
        public static string GetUsersOwnFollowingsAPI =>
        "https://www.tumblr.com/api/v2/user/following?fields%5Bblogs%5D=name%2Cavatar%2Ctitle%2Curl%2Cblog_view_url%2Cuuid%2Cdescription_npf%2Cupdated%2C%3Fis_following_you%2C%3Ffollowed%2Ccan_message%2C%3Fduration_blog_following_you%2C%3Fduration_following_blog";

        public static string GetSomeOnesFollowingsAPIByUserName(string username)
        {
            return $"https://www.tumblr.com/api/v2/blog/{username}/following?fields%5Bblogs%5D=name%2Cavatar%2Ctitle%2Curl%2Cblog_view_url%2Cuuid%2Cdescription_npf%2Cupdated%2Ccan_be_followed%2C%3Fis_following_you%2C%3Ffollowed%2Ccan_message%2C%3Fduration_blog_following_you%2C%3Fduration_following_blog";
        }
        public static string GetPublishPostAPI(string CrmUUid)
        {
            return $"https://www.tumblr.com/api/v2/blog/{CrmUUid}/posts";

        }
        public static string GetLikedPostDetailsofAnyUserAPI() =>
             $"https://www.tumblr.com/api/v2/user/likes?fields%5Bblogs%5D=name%2Cavatar%2Ctitle%2Curl%2Cblog_view_url%2Cis_adult%2C%3Fis_member%2Cdescription_npf%2Cuuid%2Ccan_be_followed%2C%3Ffollowed%2C%3Fadvertiser_name%2Ctheme%2C%3Fprimary%2C%3Fis_paywall_on%2C%3Fpaywall_access%2C%3Fsubscription_plan%2Ctumblrmart_accessories%2Ccan_show_badges%2C%3Fcan_be_booped&limit=21&reblog_info=true&before={DateTimeUtilities.GetCurrentEpochTimeMilliSeconds(DateTime.Now)}";


        public static string GetUserMessageAPI()
        {
            return "https://www.tumblr.com/api/v2/conversations/messages?fields%5Bblogs%5D=avatar%2Cname%2Cseconds_since_last_activity%2Curl%2Cblog_view_url%2Cuuid%2Ctheme%2Cdescription_npf%2Cis_adult ";
        }
        public static string GetCursorValue(string CursorValue) => string.IsNullOrEmpty(CursorValue) ? string.Empty
            : $"&cursor={CursorValue}";
        public static (string, string) GetSearchFilterUrl(string Keyword, SearchFilterModel searchFilterModel, bool IsBrowser = false, bool IsHastag = false, string CursorValue = "")
        {
            var days = 0;
            var extra = "?src=typed_query";
            var FilterActive = searchFilterModel?.SelectedPostType != null && searchFilterModel?.SelectedPostType != "All posts" && !IsHastag;
            if (searchFilterModel.IsCheckTop && !IsHastag)
            {
                switch (searchFilterModel.SelectedTime)
                {
                    case "All Time":
                        if (FilterActive)
                            extra = $"/{searchFilterModel.SelectedPostType.ToLower()}";
                        break;
                    case "Last year":
                        days = 365;
                        if (FilterActive)
                            extra = $"/{searchFilterModel.SelectedPostType.ToLower()}" + "?t=" + days;
                        else extra = "?t=" + days;
                        break;
                    case "Last 6 months":
                        days = 180;
                        if (FilterActive)
                            extra = $"/{searchFilterModel.SelectedPostType.ToLower()}" + "?t=" + days;
                        else extra = "?t=" + days;
                        break;
                    case "Last month":
                        days = 30;
                        if (FilterActive)
                            extra = $"/{searchFilterModel.SelectedPostType.ToLower()}" + "?t=" + days;
                        else extra = "?t=" + days;
                        break;
                    case "Last week":
                        days = 7;
                        if (FilterActive)
                            extra = $"/{searchFilterModel.SelectedPostType.ToLower()}" + "?t=" + days;
                        else extra = "?t=" + days;
                        break;
                    case "Today":
                        days = 1;
                        if (FilterActive)
                            extra = $"/{searchFilterModel.SelectedPostType.ToLower()}" + "?t=" + days;
                        else extra = "?t=" + days;
                        break;
                }
            }
            else if (searchFilterModel.IsCheckLatest)
            {
                days = 0;
                if (FilterActive)
                    extra = $"/recent/{searchFilterModel.SelectedPostType.ToLower()}";
                else extra = IsHastag ? extra + "&sort=recent" : "/recent";
            }
            var searchType = IsHastag ? "tagged" : "search";
            string url = FilterActive ?
                $"https://www.tumblr.com/api/v2/timeline/search?limit=20&days={days}&query={Keyword}&mode=top&timeline_type=post&skip_component=related_tags%2Cblog_search&reblog_info=true&post_type_filter={searchFilterModel.SelectedPostType.ToLower()}&fields%5Bblogs%5D=name%2Cavatar%2Ctitle%2Curl%2Cblog_view_url%2Cis_adult%2C%3Fis_member%2Cdescription_npf%2Cuuid%2Ccan_be_followed%2C%3Ffollowed%2C%3Fadvertiser_name%2Ctheme%2C%3Fprimary%2C%3Fis_paywall_on%2C%3Fpaywall_access%2C%3Fsubscription_plan%2Ctumblrmart_accessories%2Ccan_show_badges%2C%3Fcan_be_booped%2Ccan_message%2Cshare_following%2Cshare_likes%2Cask{GetCursorValue(CursorValue)}"
                : $"https://www.tumblr.com/api/v2/timeline/search?limit=20&days={days}&query={Keyword}&mode=top&timeline_type=post&skip_component=related_tags%2Cblog_search&reblog_info=true&fields%5Bblogs%5D=name%2Cavatar%2Ctitle%2Curl%2Cblog_view_url%2Cis_adult%2C%3Fis_member%2Cdescription_npf%2Cuuid%2Ccan_be_followed%2C%3Ffollowed%2C%3Fadvertiser_name%2Ctheme%2C%3Fprimary%2C%3Fis_paywall_on%2C%3Fpaywall_access%2C%3Fsubscription_plan%2Ctumblrmart_accessories%2Ccan_show_badges%2C%3Fcan_be_booped%2Ccan_message%2Cshare_following%2Cshare_likes%2Cask{GetCursorValue(CursorValue)}";
            string referer = $"https://www.tumblr.com/{searchType}/{Keyword}{extra}";
            return (IsBrowser ? referer : url, referer);
        }
        #endregion
    }


}