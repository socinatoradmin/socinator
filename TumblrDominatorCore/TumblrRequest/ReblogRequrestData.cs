using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace TumblrDominatorCore.TumblrRequest
{
    public class ReblogPostData
    {
        [JsonProperty(PropertyName = "placement_id")]
        public string PlacementId { get; set; }
        [JsonProperty(PropertyName = "layout")]
        public List<Layout> Layout { get; set; }
        //[JsonProperty(PropertyName = "send_to_twitter")]
        //public bool SendToTwitter { get; set; }
        [JsonProperty(PropertyName = "reblog_key")]
        public string ReblogKey { get; set; }
        [JsonProperty(PropertyName = "hide_trail")]
        public bool HideTrail { get; set; }
        [JsonProperty(PropertyName = "parent_tumblelog_uuid")]
        public string ParentTumblelogUuid { get; set; }
        [JsonProperty(PropertyName = "parent_post_id")]
        public string ParentPostId { get; set; }
        [JsonProperty(PropertyName = "content")]
        public List<Content> Content { get; set; }
        [JsonProperty(PropertyName = "can_be_tipped")]
        public object CanBeTipped { get; set; }
        [JsonProperty(PropertyName = "interactability_reblog")]
        public string InteractabilityReblog { get; set; }
        [JsonProperty(PropertyName = "tags")]
        public string Tags { get; set; }
        [JsonProperty(PropertyName = "has_community_label")]
        public bool HasCommunityLabel { get; set; }
        [JsonProperty(PropertyName = "community_label_categories")]
        public List<object> CommunityLabelCategories { get; set; }

    }
    public class PublishPostData
    {

        [JsonProperty(PropertyName = "layout")]
        public List<Layout> Layout { get; set; }
        //[JsonProperty(PropertyName = "send_to_twitter")]
        //public bool SendToTwitter { get; set; }
        [JsonProperty(PropertyName = "hide_trail")]
        public bool HideTrail { get; set; }
        [JsonProperty(PropertyName = "content")]
        public ArrayList Content { get; set; }
        [JsonProperty(PropertyName = "can_be_tipped")]
        public object CanBeTipped { get; set; }
        [JsonProperty(PropertyName = "tags")]
        public string Tags { get; set; }
        [JsonProperty(PropertyName = "has_community_label")]
        public bool HasCommunityLabel { get; set; }
        [JsonProperty(PropertyName = "community_label_categories")]
        public List<object> CommunityLabelCategories { get; set; }

    }
    public class CommentPostData
    {
        [JsonProperty(propertyName: "post_id")]
        public string PostId { get; set; }
        [JsonProperty(propertyName: "reply_text")]
        public string ReplyText { get; set; }
        [JsonProperty(PropertyName = "reblog_key")]
        public string ReblogKey { get; set; }
        [JsonProperty(propertyName: "tumblelog")]
        public string TumbleLog { get; set; }
        [JsonProperty(PropertyName = "placement_id")]
        public string PlacementId { get; set; }
        [JsonProperty(PropertyName = "reply_as")]
        public string ReplyAs { get; set; }
        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; }
    }
    public class Content
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        //[JsonProperty(PropertyName ="subtype")]
        //public string SubType { get; set; }
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
        //[JsonProperty(PropertyName ="media")]
        //public List<Media> Media { get; set; }
        //[JsonProperty(PropertyName ="file")]
        //public file File { get; set; }

    }
    public class file
    {

    }
    public class Display
    {
        [JsonProperty(PropertyName = "blocks")]
        public List<int> Blocks { get; set; }
    }

    public class Layout
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "display")]
        public List<Display> Display { get; set; }
    }


    public class PostDescrptionAndTitle
    {
        [JsonProperty(PropertyName = "channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty(PropertyName = "post[date]")]
        public string Postdate { get; set; }

        [JsonProperty(PropertyName = "post[one]")]
        public string PostOne { get; set; }

        [JsonProperty(PropertyName = "post[publish_on]")]
        public string PostpublishOn { get; set; }

        [JsonProperty(PropertyName = "post[slug]")]
        public string Postslug { get; set; }

        [JsonProperty(PropertyName = "post[tags]")]
        public string Posttags { get; set; }

        [JsonProperty(PropertyName = "post[two]")]
        public string Posttwo { get; set; }

        [JsonProperty(PropertyName = "editor_type")]
        public string EditorType { get; set; }

        [JsonProperty(PropertyName = "send_to_twitter")]
        public bool SendToTwitter { get; set; }


        [JsonProperty(PropertyName = "loggingData")]
        public Loggingdata LoggingData { get; set; }

        [JsonProperty(PropertyName = "carousel_display")]
        public bool CarouselDisplay { get; set; }

        [JsonProperty(PropertyName = "members_only")]
        public bool MembersOnly { get; set; }

        [JsonProperty(PropertyName = "owner_flagged_nsfw")]
        public bool OwnerFlaggedNsfw { get; set; }

        [JsonProperty(PropertyName = "content_stats")]
        public ContentStats ContentStats { get; set; }

        [JsonProperty(PropertyName = "can_be_liked")]
        public bool CanBeLiked { get; set; }

        [JsonProperty(PropertyName = "can_be_reblogged")]
        public bool CanBeReblogged { get; set; }

        [JsonProperty(PropertyName = "enable_cta")]
        public bool EnableCta { get; set; }

        [JsonProperty(PropertyName = "cta_text_code")]
        public string CtaTextCode { get; set; }

        [JsonProperty(PropertyName = "enable_redirect_urls")]
        public bool EnableRedirectUrls { get; set; }

        [JsonProperty(PropertyName = "redirect_url_primary")]
        public string RedirectUrlPrimary { get; set; }

        [JsonProperty(PropertyName = "redirect_url_ios")]
        public string RedirectUrlIos { get; set; }

        [JsonProperty(PropertyName = "redirect_url_android")]
        public string RedirectUrlAndroid { get; set; }

        [JsonProperty(PropertyName = "post[source_url]")]
        public string PostsourceUrl { get; set; }

        [JsonProperty(PropertyName = "post[state]")]
        public string Poststate { get; set; }

        [JsonProperty(PropertyName = "post[type]")]
        public string Posttype { get; set; }

        [JsonProperty(PropertyName = "context_id")]
        public string ContextId { get; set; }

        [JsonProperty(PropertyName = "context_page")]
        public string ContextPage { get; set; }

        [JsonProperty(PropertyName = "context_bundle")]
        public string ContextBundle { get; set; }

        [JsonProperty(PropertyName = "is_rich_text[one]")]
        public string IsRichTextone { get; set; }

        [JsonProperty(PropertyName = "is_rich_text[two]")]
        public string IsRichTexttwo { get; set; }

        [JsonProperty(PropertyName = "is_rich_text[three]")]
        public string IsRichTextthree { get; set; }
    }

    public class Loggingdata
    {
    }

    public class ContentStats
    {
        public int Elapsed { get; set; }
        public int HtmlLength { get; set; }
        public int TextLength { get; set; }
        public Texttools TextTools { get; set; }
        public Embeds Embeds { get; set; }
        public Images Images { get; set; }
        public Mentions Mentions { get; set; }
        public Gifs Gifs { get; set; }
        public Gifsearch GifSearch { get; set; }
        public int sup { get; set; }
        public int sub { get; set; }
        public int small { get; set; }
        public int blockquote { get; set; }
        public int pre { get; set; }
        public int code { get; set; }
    }

    public class Texttools
    {
        public int Hr { get; set; }
        public int H2 { get; set; }
        public int More { get; set; }
        public int Mentions { get; set; }
    }

    public class Embeds
    {
    }

    public class Images
    {
    }

    public class Mentions
    {
    }

    public class Gifs
    {
        public int Searches { get; set; }
        public int Added { get; set; }
        public int Kept { get; set; }
    }

    public class Gifsearch
    {
    }

    #region 
    //video post data model 

    public class VideoPublishedPostData
    {
        [JsonProperty(PropertyName = "channel_id")]
        public string channel_id { get; set; }

        [JsonProperty(PropertyName = "context_id")]
        public string context_id { get; set; }

        [JsonProperty(PropertyName = "context_page")]
        public string context_page { get; set; }

        [JsonProperty(PropertyName = "context_bundle")]
        public string context_bundle { get; set; }

        [JsonProperty(PropertyName = "send_to_twitter")]
        public bool send_to_twitter { get; set; }

        [JsonProperty(PropertyName = "carousel_display")]
        public bool carousel_display { get; set; }

        [JsonProperty(PropertyName = "members_only")]
        public bool members_only { get; set; }

        [JsonProperty(PropertyName = "owner_flagged_nsfw")]
        public bool owner_flagged_nsfw { get; set; }

        [JsonProperty(PropertyName = "content_stats")]
        public ContentStatsClass content_stats { get; set; }

        [JsonProperty(PropertyName = "can_be_liked")]
        public bool can_be_liked { get; set; }

        [JsonProperty(PropertyName = "can_be_reblogged")]
        public bool can_be_reblogged { get; set; }

        [JsonProperty(PropertyName = "enable_cta")]
        public bool enable_cta { get; set; }

        [JsonProperty(PropertyName = "tsp_skip_lightbox")]
        public bool tsp_skip_lightbox { get; set; }

        [JsonProperty(PropertyName = "cta_text_code")]
        public string cta_text_code { get; set; }

        [JsonProperty(PropertyName = "enable_redirect_urls")]
        public bool enable_redirect_urls { get; set; }

        [JsonProperty(PropertyName = "redirect_url_primary")]
        public string redirect_url_primary { get; set; }

        [JsonProperty(PropertyName = "redirect_url_ios")]
        public string redirect_url_ios { get; set; }

        [JsonProperty(PropertyName = "redirect_url_android")]
        public string redirect_url_android { get; set; }

        [JsonProperty(PropertyName = "is_rich_text_one")]
        public string is_rich_text_one { get; set; }

        [JsonProperty(PropertyName = "is_rich_text_two")]
        public string is_rich_text_two { get; set; }

        [JsonProperty(PropertyName = "is_rich_text_three")]

        public string is_rich_text_three { get; set; }

        [JsonProperty(PropertyName = "loggingData")]
        public Dictionary<string, object> loggingData { get; set; }

        [JsonProperty(PropertyName = "preuploaded_url")]
        public string preuploaded_url { get; set; }

        [JsonProperty(PropertyName = "preuploaded_ch")]
        public string preuploaded_ch { get; set; }

        [JsonProperty(PropertyName = "confirm_tos")]
        public bool confirm_tos { get; set; }

        [JsonProperty(PropertyName = "editor_type")]
        public string editor_type { get; set; }

        [JsonProperty(PropertyName = "post[date]")]
        public string post_date { get; set; }

        [JsonProperty(PropertyName = "post[public_on]")]
        public string post_publish_on { get; set; }

        [JsonProperty(PropertyName = "post[slug]")]
        public string post_slug { get; set; }

        [JsonProperty(PropertyName = "post[tags]")]
        public string post_tags { get; set; }

        [JsonProperty(PropertyName = "post[two]")]
        public string post_two { get; set; }

        [JsonProperty(PropertyName = "post[source_url]")]
        public string post_source_url { get; set; }

        [JsonProperty(PropertyName = "post[state]")]
        public string post_state { get; set; }

        [JsonProperty(PropertyName = "post[type]")]
        public string post_type { get; set; }

        [JsonProperty(PropertyName = "pt")]
        public string pt { get; set; }
    }



    public class TextTools
    {
        public int hr { get; set; }
        public int h2 { get; set; }
        public int more { get; set; }
        public int mentions { get; set; }
        public int sup { get; set; }
        public int sub { get; set; }
        public int small { get; set; }
        public int blockquote { get; set; }
        public int pre { get; set; }
        public int code { get; set; }
    }

    public class ContentStatsClass
    {
        public int elapsed { get; set; }
        public int htmlLength { get; set; }
        public int textLength { get; set; }
        public TextTools textTools { get; set; }
        public Dictionary<string, object> embeds { get; set; }
        public Dictionary<string, object> images { get; set; }
        public Dictionary<string, object> mentions { get; set; }
        public GifsVideo gifs { get; set; }
        public Dictionary<string, object> gifSearch { get; set; }
    }

    public class GifsVideo
    {
        public int added { get; set; }
        public int kept { get; set; }
        public int searches { get; set; }
    }

    #endregion


    #region 
    //PhotoPostData

    public class PhotoPostData
    {
        [JsonProperty(PropertyName = "channel_id")]
        public string channel_id { get; set; }

        [JsonProperty(PropertyName = "editor_type")]
        public string editor_type { get; set; }

        [JsonProperty(PropertyName = "send_to_twitter")]
        public bool send_to_twitter { get; set; }

        [JsonProperty(PropertyName = "loggingData")]
        public object loggingData { get; set; }

        [JsonProperty(PropertyName = "carousel_display")]
        public bool carousel_display { get; set; }

        [JsonProperty(PropertyName = "members_only")]
        public bool members_only { get; set; }

        [JsonProperty(PropertyName = "owner_flagged_nsfw")]
        public bool owner_flagged_nsfw { get; set; }

        [JsonProperty(PropertyName = "content_stats")]
        public ContentStatsPostPhoto content_stats { get; set; }

        [JsonProperty(PropertyName = "can_be_like")]
        public bool can_be_liked { get; set; }

        [JsonProperty(PropertyName = "can_be_reblogged")]
        public bool can_be_reblogged { get; set; }

        [JsonProperty(PropertyName = "enable_cta")]
        public bool enable_cta { get; set; }

        [JsonProperty(PropertyName = "tsp_skip_lightbox")]
        public bool tsp_skip_lightbox { get; set; }

        [JsonProperty(PropertyName = "cta_text_code")]
        public string cta_text_code { get; set; }

        [JsonProperty(PropertyName = "enable_redirect_urls")]
        public bool enable_redirect_urls { get; set; }

        [JsonProperty(PropertyName = "redirect_url_primary")]
        public string redirect_url_primary { get; set; }

        [JsonProperty(PropertyName = "redirect_url_ios")]
        public string redirect_url_ios { get; set; }

        [JsonProperty(PropertyName = "redirect_url_android")]
        public string redirect_url_android { get; set; }

        [JsonProperty(PropertyName = "pt")]
        public string pt { get; set; }

        [JsonProperty(PropertyName = "context_id")]
        public string context_id { get; set; }

        [JsonProperty(PropertyName = "context_page")]
        public string context_page { get; set; }

        [JsonProperty(PropertyName = "context_bundle")]
        public string context_bundle { get; set; }

        [JsonProperty(PropertyName = "is_rich_text_one")]
        public string is_rich_text_one { get; set; }

        [JsonProperty(PropertyName = "is_rich_text_two")]
        public string is_rich_text_two { get; set; }

        [JsonProperty(PropertyName = "is_rich_text_three")]
        public string is_rich_text_three { get; set; }

        [JsonProperty(PropertyName = "post[date]")]
        public string post_date { get; set; }

        [JsonProperty(PropertyName = "post[one]")]
        public string post_one { get; set; }

        [JsonProperty(PropertyName = "post[publish_on]")]
        public string post_publish_on { get; set; }

        [JsonProperty(PropertyName = "post[slug]")]
        public string post_slug { get; set; }

        [JsonProperty(PropertyName = "post[tags]")]
        public string post_tags { get; set; }

        [JsonProperty(PropertyName = "post[two]")]
        public string post_two { get; set; }

        [JsonProperty(PropertyName = "post[source_url]")]
        public string post_source_url { get; set; }

        [JsonProperty(PropertyName = "post[state]")]
        public string post_state { get; set; }

        [JsonProperty(PropertyName = "post[type]")]
        public string post_type { get; set; }
    }

    public class ContentStatsPostPhoto
    {
        public int elapsed { get; set; }
        public int htmlLength { get; set; }
        public int textLength { get; set; }
        public TextTools textTools { get; set; }
        public object embeds { get; set; }
        public ImagesClass images { get; set; }
        public object mentions { get; set; }
        public GifsVideo gifs { get; set; }
        public Gifsearch gifSearch { get; set; }
    }

    public class ImagesClass
    {
        public int jpg { get; set; }
    }
    public class Media
    {
        [JsonProperty(PropertyName = "height")]
        public int Height { get; set; }
        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
    public class AudioMedia
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

    }
    public class ImageContent
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "file")]
        public file File { get; set; }
        [JsonProperty(PropertyName = "media")]
        public ArrayList Media { get; set; }

    }

    public class VideoContent
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "media")]
        public Media Media { get; set; }
        [JsonProperty(PropertyName = "provider")]
        public string Provider { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

    }
    public class AudioContent
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "provider")]
        public string Provider { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "media")]
        public ArrayList Media { get; set; }
    }
    public class TitleContent
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "subtype")]
        public string SubType { get; set; }
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }
    public class DescriptionContent
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }
    public class LinkDescriptionContent
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
        [JsonProperty(PropertyName = "formatting")]
        public ArrayList formatting { get; set; }
    }

    public class FormattingType
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "start")]
        public int start { get; set; }
        [JsonProperty(PropertyName = "end")]
        public int end { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string url { get; set; }
    }


    #endregion

}