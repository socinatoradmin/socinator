using Newtonsoft.Json;

namespace TumblrDominatorCore.TumblrRequest
{
    public class PostDataElement
    {
        [JsonProperty("determine_email")] public string DetermineEmail { get; set; }

        [JsonProperty("password")] public string Password { get; set; }
        [JsonProperty("grant_type")] public string GrantType { get; set; }
        [JsonProperty("username")] public string Username { get; set; }

        [JsonProperty("user%5Bemail%5D")] public string UserEmail { get; set; }

        [JsonProperty("user%5Bpassword%5D")] public string UserPassword { get; set; }

        [JsonProperty("tumblelog%5Bname%5D")] public string TumblelogName { get; set; }

        [JsonProperty("user%5Bage%5D")] public string UserAge { get; set; }

        [JsonProperty("context")] public string Context { get; set; }


        [JsonProperty("version")] public string Version { get; set; }


        [JsonProperty("follow")] public string Follow { get; set; }


        [JsonProperty("http_referer")] public string HttpReferer { get; set; }


        [JsonProperty("form_key")] public string FormKey { get; set; }


        [JsonProperty("seen_suggestion")] public string SeenSuggestion { get; set; }


        [JsonProperty("used_suggestion")] public string UsedSuggestion { get; set; }


        [JsonProperty("used_auto_suggestion")] public string UsedAutoSuggestion { get; set; }


        [JsonProperty("about_tumblr_slide")] public string AboutTumblrSlide { get; set; }


        [JsonProperty("random_username_suggestions")]
        public string RandomUsernameSuggestions { get; set; }


        [JsonProperty("action")] public string Action { get; set; }


        [JsonProperty("data%5Btumblelog%5D")] public string DataTumblelog { get; set; }


        [JsonProperty("url")] public string DataPageUrl { get; set; }


        [JsonProperty("q")] public string Q { get; set; }

        [JsonProperty("sort")] public string Sort { get; set; }


        [JsonProperty("post_view")] public string PostView { get; set; }


        [JsonProperty("blogs_before")] public string BlogsBefore { get; set; }

        [JsonProperty("num_blogs_shown")] public string NumBlogsShown { get; set; }

        [JsonProperty("num_posts_shown")] public string NumPostsShown { get; set; }

        [JsonProperty("before")] public string Before { get; set; }


        [JsonProperty("blog_page")] public string BlogPage { get; set; }

        [JsonProperty("safe_mode")] public string SafeMode { get; set; }


        [JsonProperty("post_page")] public string PostPage { get; set; }


        [JsonProperty("filter_nsfw")] public string FilterNsfw { get; set; }


        [JsonProperty("filter_post_type")] public string FilterPostType { get; set; }


        [JsonProperty("next_ad_offset")] public string NextAdOffset { get; set; }


        [JsonProperty("ad_placement_id")] public string AdPlacementId { get; set; }


        [JsonProperty("more_blogs")] public string MoreBlogs { get; set; }

        [JsonProperty("more_posts")] public string MorePosts { get; set; }

        [JsonProperty("data%5Bid%5D")] public string DataId { get; set; }


        [JsonProperty("data%5Broot_id%5D")] public string DataRootId { get; set; }


        [JsonProperty("data%5Bkey%5D")] public string DataKey { get; set; }


        [JsonProperty("data%5Bplacement_id%5D")]
        public string DataPlacementId { get; set; }


        [JsonProperty("data%5Bis_recommended%5D")]
        public string DataIsRecommended { get; set; }

        [JsonProperty("data%5Breply_text%5D")] public string DataIsReplytext { get; set; }


        [JsonProperty("data%5Bpost_id%5D")] public string PostId { get; set; }

        [JsonProperty("data%5Bpt%5D")] public string DataPoint { get; set; }

        [JsonProperty("data%5Btumblelog_key%5D")]
        public string UserPostKey { get; set; }

        [JsonProperty("data%5Btumblelog_name%5D")]
        public string DataTumblogName { get; set; }

        [JsonProperty("data%5Bmethod%5D")] public string DataMethod { get; set; }

        [JsonProperty("data%5Bsource%5D")] public string DataSource { get; set; }


    }
    public class ServicePostData
    {
        [JsonProperty("flushTime")] public long FlushTime { get; set; }

        [JsonProperty("krakenClientDetails")] public ClientDetails KrakenCLientdetails { get; set; }
        [JsonProperty("krakenEvents")] public KrackEvents[] KrackEvents { get; set; }
        [JsonProperty("trackers")] public object[] Trackers { get; set; }
    }

    public class ClientDetails
    {
        [JsonProperty("platform")] public string Platform { get; set; }
        [JsonProperty("os_name")] public string OsName { get; set; }
        [JsonProperty("os_version")] public string OsVersion { get; set; }
        [JsonProperty("language")] public string Language { get; set; }
        [JsonProperty("build_version")] public string BuildVersion { get; set; }
        [JsonProperty("form_factor")] public string FormFactor { get; set; }
        [JsonProperty("model")] public string Model { get; set; }
        [JsonProperty("connection")] public string Connection { get; set; }
        [JsonProperty("carrier")] public string Carrier { get; set; }
        [JsonProperty("browser_name")] public string BrowserName { get; set; }
        [JsonProperty("browser_version")] public string BrowserVersion { get; set; }
        [JsonProperty("manufacturer")] public string Manufacturer { get; set; }
    }

    public class KrackEvents
    {
        [JsonProperty("event_name")] public string EventName { get; set; }
        [JsonProperty("experiments")] public object Experiments { get; set; }
        [JsonProperty("event_details")] public EventDetails EventDetails { get; set; }
        [JsonProperty("client_timestamp")] public long TimeStamp { get; set; }
        [JsonProperty("client_session_id")] public string SessionId { get; set; }
        [JsonProperty("page")] public string Page { get; set; }

    }

    public class EventDetails
    {
        [JsonProperty("action")] public string Action { get; set; }
        [JsonProperty("pathname")] public string Pathname { get; set; }
        [JsonProperty("hostname")] public string Hostname { get; set; }

    }

}