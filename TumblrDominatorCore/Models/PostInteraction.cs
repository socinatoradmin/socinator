// ReSharper disable InconsistentNaming

namespace TumblrDominatorCore.Models
{
    public class FeedInteraction
    {
        public Meta meta { get; set; }

        public Response Response { get; set; }
    }

    public class Meta
    {
        public int status { get; set; }
        public string msg { get; set; }
    }


    public class Response
    {
        public Note[] notes { get; set; }
        public Rollup_Notes[] rollup_notes { get; set; }
        public int total_notes { get; set; }
        public int total_likes { get; set; }
        public int total_reblogs { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool can_hide_or_delete_notes { get; set; }
        public bool conversational_notifications_enabled { get; set; }

        public string token { get; set; }
        public Note[] Notes { get; set; }
        public Links Links { get; set; }
        public string uuid { get; set; }
    }


    public class Links
    {
        public Next next { get; set; }
        public Next Next { get; set; }
    }

    public class Next
    {
        public string href { get; set; }
        public string method { get; set; }
        public QueryParams QueryParams { get; set; }
    }


    public class QueryParams
    {
        public string mode { get; set; }
        public string before_timestamp { get; set; }

        public string BeforeTimestamp { get; set; }
    }

    public class Note
    {
        public string type { get; set; }
        public float timestamp { get; set; }
        public string blog_name { get; set; }
        public string blog_uuid { get; set; }
        public string blog_url { get; set; }
        public bool followed { get; set; }
        public string avatar_shape { get; set; }
        public Avatar_Url avatar_url { get; set; }
        public bool blog_is_adult { get; set; }
        public string added_text { get; set; }
        public string post_id { get; set; }
        public string reblog_parent_blog_name { get; set; }
        public string reblog_parent_blog_url { get; set; }
        public object reblog_parent_post_id { get; set; }
    }

    public class Avatar_Url
    {
        public string _24 { get; set; }
        public string _48 { get; set; }
    }

    public class Rollup_Notes
    {
        public Note1[] notes { get; set; }
        public int total_notes { get; set; }
        public int total_likes { get; set; }
        public int total_reblogs { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool can_hide_or_delete_notes { get; set; }
        public bool conversational_notifications_enabled { get; set; }
    }

    public class Note1
    {
        public string type { get; set; }
        public int timestamp { get; set; }
        public string blog_name { get; set; }
        public string blog_uuid { get; set; }
        public string blog_url { get; set; }
        public bool followed { get; set; }
        public string avatar_shape { get; set; }
        public Avatar_Url1 avatar_url { get; set; }
        public bool blog_is_adult { get; set; }
        public string post_id { get; set; }
        public string reblog_parent_blog_name { get; set; }
    }

    public class Avatar_Url1
    {
        public string _24 { get; set; }
        public string _48 { get; set; }
        public string Type { get; set; }
        public string BlogName { get; set; }
    }
}