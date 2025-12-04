//LoginJsonResponse
// ReSharper disable All

using System.Collections.Generic;

namespace TumblrDominatorCore.Models
{
    public class LoginJsonResponse
    {
        //public Components Components { get; set; }
        public Context Context { get; set; }
    }

    #region Components

    //public class Components
    //{
    //    public Messaging Messaging { get; set; }
    //    public Messagingsharepost MessagingSharePost { get; set; }
    //    public Notificationsoundmanager NotificationSoundManager { get; set; }
    //    public Stickers Stickers { get; set; }
    //    public object[] PostActivity { get; set; }
    //    public Notificationpoller NotificationPoller { get; set; }
    //    public object[] AskForm { get; set; }
    //    public object[] KeycommandGuide { get; set; }
    //    public Abuseform AbuseForm { get; set; }
    //    public Adspaginationhelper AdsPaginationHelper { get; set; }
    //    public Dfpad[] DfpAd { get; set; }
    //    public Rapidrecommendations RapidRecommendations { get; set; }
    //    public Postformbuilder PostFormBuilder { get; set; }
    //    public Posts1 Posts { get; set; }
    //    public object[] SupplyLogging { get; set; }
    //} 

    #endregion

    #region Messages

    //public class Messaging
    //{
    //    public int polling_frequency_inbox { get; set; }
    //    public int polling_frequency_conversation { get; set; }
    //    public int append_favorites_to_inbox_limit { get; set; }
    //    public bool is_desktop_notifications_enabled { get; set; }
    //    public string messaging_notification_icon { get; set; }
    //    public int image_upload_max_bytes { get; set; }
    //    public bool is_new_empty_inbox_enabled { get; set; }
    //    public bool can_modify_safe_mode { get; set; }
    //}

    //public class Messagingsharepost
    //{
    //    public bool is_external_sharing_enabled { get; set; }
    //} 

    #endregion

    public class Notificationsoundmanager
    {
        public Sounds sounds { get; set; }
        public object[] soundpacks { get; set; }
    }

    public class Sounds
    {
        public string notificationsound { get; set; }
        public string receivesound { get; set; }
        public string sendsound { get; set; }
    }

    public class Stickers
    {
        public Sticker[] stickers { get; set; }
    }

    public class Sticker
    {
        public int id { get; set; }
        public string description { get; set; }
        public string media_key { get; set; }
        public Face_Dimensions face_dimensions { get; set; }
        public int sticker_pack_id { get; set; }
        public string url { get; set; }
        public string original_size_url { get; set; }
    }

    public class Face_Dimensions
    {
        public float x_offset { get; set; }
        public float y_offset { get; set; }
        public float scale { get; set; }
    }

    public class Notificationpoller
    {
        public string[] messaging_keys { get; set; }
        public string token { get; set; }
        public int inbox_unread { get; set; }
    }

    public class Abuseform
    {
        public bool language_notice { get; set; }
        public object autolaunch { get; set; }
        public string lang { get; set; }
        public string email { get; set; }
        public object form_token { get; set; }
        public object native_app { get; set; }
        public object prefill_fields { get; set; }
        public Tumblelog[] tumblelogs { get; set; }
        public string recaptcha { get; set; }
    }

    public class Tumblelog
    {
        public string name { get; set; }
        public string title { get; set; }
    }

    public class Adspaginationhelper
    {
        public int nextAdPos { get; set; }
    }

    public class Rapidrecommendations
    {
        public Posts posts { get; set; }
    }

    public class Posts
    {
        public _173334762383 _173334762383 { get; set; }
        public _53673867016 _53673867016 { get; set; }
        public _167303250962 _167303250962 { get; set; }
        public _110977200842 _110977200842 { get; set; }
        public _149892079476 _149892079476 { get; set; }
        public _171872326096 _171872326096 { get; set; }
    }

    public class _173334762383
    {
        public object[] ignore_list { get; set; }
    }

    public class _53673867016
    {
        public object[] ignore_list { get; set; }
    }

    public class _167303250962
    {
        public object[] ignore_list { get; set; }
    }

    public class _110977200842
    {
        public object[] ignore_list { get; set; }
    }

    public class _149892079476
    {
        public object[] ignore_list { get; set; }
    }

    public class _171872326096
    {
        public object[] ignore_list { get; set; }
    }

    public class Postformbuilder
    {
        public string language { get; set; }
        public string baseRoute { get; set; }
        public bool redirectTo { get; set; }
        public string contextPage { get; set; }
        public string currentChannel { get; set; }
        public bool redirectChannel { get; set; }
        public Limits limits { get; set; }
        public Dashboardsettings dashboardSettings { get; set; }
        public bool supportImageSearch { get; set; }
        public int minFullWidthSize { get; set; }
        public Embedregexes embedRegexes { get; set; }
        public bool reblogActionContext { get; set; }
        public bool updateURL { get; set; }
    }

    public class Limits
    {
        public int maxUploadBytesImage { get; set; }
        public int videoSecondsRemaining { get; set; }
        public int preuploadPhotoUsed { get; set; }
        public int preuploadAudioUsed { get; set; }
        public int inlineEmbedsPerPost { get; set; }
    }

    public class Dashboardsettings
    {
        public string defaultEditorType { get; set; }
        public bool hasRichTextTutorial { get; set; }
        public bool hasInlineControlsTutorial { get; set; }
        public bool hasImageSearchTutorial { get; set; }
        public bool hasGifSearchVariation { get; set; }
    }

    public class Embedregexes
    {
        public string youtubecomuserazAZ09_live { get; set; }
        public string youtubecomwatchvazAZ09_ { get; set; }
        public string youtubecomvevideoembedattribution_linkazAZ09_ { get; set; }
        public string youtubecomvazAZ09_ { get; set; }
        public string youtubecom3Fv3DazAZ09_ { get; set; }
        public string youtubeazAZ09_ { get; set; }
        public string vimeocommoogaloopswfclip_id09 { get; set; }
        public string vimeocomgroupsvideos09 { get; set; }
        public string vimeocomchannels09 { get; set; }
        public string playervimeocomvideo09 { get; set; }
        public string vimeocomondemandazAZ09_ { get; set; }
        public string vimeocom09 { get; set; }
        public string gtyim09 { get; set; }
        public string gettyimagescomdetail09 { get; set; }
        public string gettyimagescomembed09 { get; set; }
        public string metacafecomfplayer09 { get; set; }
        public string metacafecomwatch09 { get; set; }
        public string metacafecomembed09 { get; set; }
        public string funnyordiecomvideosaz09 { get; set; }
        public string funnyordiecomembedaz09 { get; set; }
        public string playerordienetworkscomflashfodplayerswfkeyaz09 { get; set; }
        public string dailymotioncomvideoaz09 { get; set; }
        public string dailymotioncomswfvideoaz09 { get; set; }
        public string dailymotioncomembedvideoaz09 { get; set; }
        public string dailyaz09 { get; set; }
        public string bliptvplayepisodeazAZ09_ { get; set; }
        public string bliptvplayazAZ09_ { get; set; }
        public string bliptvazAZ09azAZ09_ { get; set; }
        public string collegehumorcomvideo09 { get; set; }
        public string collegehumorcome09 { get; set; }
        public string collegehumorcvcdncommoogaloopclip_id09 { get; set; }
        public string hulucomwatchazAZ09_ { get; set; }
        public string hulucomembedazAZ09_ { get; set; }
        public string hulucomembedhtmleidazAZ09_ { get; set; }
        public string videofymeazAZ0909 { get; set; }
        public string jestcomvideo09 { get; set; }
        public string jestcomembed09 { get; set; }
        public string jestcome09 { get; set; }
        public string jestcvcdncommoogaloopclip_id09 { get; set; }
        public string coubcomviewazAZ09 { get; set; }
        public string coubcomembedazAZ09 { get; set; }
        public string viddlercomazAZ09_ { get; set; }
        public string viddlercomembedazAZ09_ { get; set; }
        public string vevocommassetshtmlembedhtmlvideoazAZ09_ { get; set; }
        public string vevocomembedEmbeddedvideoIdazAZ09_ { get; set; }
        public string vevocomwatchplaylistazAZ09_ { get; set; }
        public string vevolyazAZ09_ { get; set; }
        public string sportsyahoocomloopazAZ09_ { get; set; }
        public string sportsreelmyahoocomchannelsazAZ09_azAZ09_ { get; set; }
        public string wyahoocomwwhtml { get; set; }
        public string gifboomcomxaf09 { get; set; }
        public string vinecovazAZ09 { get; set; }
        public string readtapestrycomsazAZ09 { get; set; }
        public string wedgiescomquestionazAZ09_ { get; set; }
        public string wedgiescomcreatesuccessquestionazAZ09_ { get; set; }
        public string isee5cembedthecdnrackcom09 { get; set; }
        public string wwwifccomcommonsbrightcoveAmcnBrightcovejsobjectid_ { get; set; }
        public string cdnlivestreamcomembedazAZ09_ { get; set; }
        public string instagramamcompazAZ09_ { get; set; }
        public string pictureslytrocomwpicturesalbums09embed { get; set; }
        public string issuucomazAZ09_ { get; set; }
        public string video_file09azAZ09_ { get; set; }
        public string video_fileAZaz09_09azAZ09_ { get; set; }
        public string geminisazAZ09_ { get; set; }
        public string kickstartercomprojectsazAZ09_ { get; set; }
        public string nbcsportscompBxmELCnbcsports_embedshareselectazAZ09_ { get; set; }
        public string linkbrightcovecomservicesplayerbcpidazAZ09_ { get; set; }
        public string bcovemeazAZ09_ { get; set; }
        public string interludefmvw { get; set; }
        public string vinterludefmembedw { get; set; }
        public string infmvw { get; set; }
        public string infmembedw { get; set; }
        public string stageinterludefmw { get; set; }
        public string helloekocomazazAZ09_ { get; set; }
        public string flickrpazAZ09_ { get; set; }
        public string flickrcomphotosazAZ09_albumssetsgalleries09 { get; set; }
        public string praduxcomembed2w_ { get; set; }
        public string praduxcomw_photod { get; set; }
        public string praduxcomw_ { get; set; }
        public string embedvhxtvpackagesd { get; set; }
        public string wvhxtvw_ { get; set; }
        public string whipclipcomvideoazAZ09_ { get; set; }
        public string whipclipcomembedazAZ09_ { get; set; }
        public string getkanvascometazAZ09_ { get; set; }
        public string kanvasazAZ09_ { get; set; }
        public string upclosemed { get; set; }
        public string younowcom { get; set; }
        public string sketchfabcommodelsazAZ09_ { get; set; }
        public string syimgcomkpvideomodevideomodejs { get; set; }
    }

    #region Unused codes

    //public class Posts1
    //{
    //    public Posts2 posts { get; set; }
    //}

    //public class Posts2
    //{
    //    public _173830475941 _173830475941 { get; set; }
    //    public _173830470319 _173830470319 { get; set; }
    //    public _173830443034 _173830443034 { get; set; }
    //    public _173830442581 _173830442581 { get; set; }
    //    public _173830442013 _173830442013 { get; set; }
    //    public _173830441318 _173830441318 { get; set; }
    //    public _173830439125 _173830439125 { get; set; }
    //    public _173830439037 _173830439037 { get; set; }
    //    public _173830395871 _173830395871 { get; set; }
    //    public _173830392550 _173830392550 { get; set; }
    //}

    //public class _173830475941
    //{
    //    public long id { get; set; }
    //    public string type { get; set; }
    //    public string root_id { get; set; }
    //    public string tumblelog { get; set; }
    //    public string tumblelogkey { get; set; }
    //    public TumblelogData tumblelogdata { get; set; }
    //    public TumblelogParentData tumblelogparentdata { get; set; }
    //    public TumblelogRootData tumblelogrootdata { get; set; }
    //    public string reblog_key { get; set; }
    //    public bool is_reblog { get; set; }
    //    public bool is_mine { get; set; }
    //    public bool liked { get; set; }
    //    public string sponsored { get; set; }
    //    public object premium_tracked { get; set; }
    //    public bool is_recommended { get; set; }
    //    public object placement_id { get; set; }
    //    public string reblog_source { get; set; }
    //    public Share_Popover_Data share_popover_data { get; set; }
    //    public object recommendation_reason { get; set; }
    //    public bool owner_appeal_nsfw { get; set; }
    //} 

    #endregion

    public class TumblelogData
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class TumblelogParentData
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class TumblelogRootData
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class Share_Popover_Data
    {
        public string tumblelog_name { get; set; }
        public string embed_key { get; set; }
        public string embed_did { get; set; }
        public long post_id { get; set; }
        public string root_id { get; set; }
        public string post_url { get; set; }
        public string post_tiny_url { get; set; }
        public int is_private { get; set; }
        public bool has_user { get; set; }
        public bool has_facebook { get; set; }
        public string twitter_username { get; set; }
        public string permalink_label { get; set; }
        public bool show_reporting_links { get; set; }
        public string abuse_url { get; set; }
        public bool show_pinterest { get; set; }
        public string pinterest_share_window { get; set; }
        public bool show_reddit { get; set; }
        public bool show_flagging { get; set; }
    }

    public class _173830470319
    {
        public long id { get; set; }
        public string type { get; set; }
        public string root_id { get; set; }
        public string tumblelog { get; set; }
        public string tumblelogkey { get; set; }
        public TumblelogData1 tumblelogdata { get; set; }
        public TumblelogParentData1 tumblelogparentdata { get; set; }
        public TumblelogRootData1 tumblelogrootdata { get; set; }
        public string reblog_key { get; set; }
        public bool is_reblog { get; set; }
        public bool is_mine { get; set; }
        public bool liked { get; set; }
        public string sponsored { get; set; }
        public object premium_tracked { get; set; }
        public bool is_recommended { get; set; }
        public object placement_id { get; set; }
        public string reblog_source { get; set; }
        public Share_Popover_Data1 share_popover_data { get; set; }
        public object recommendation_reason { get; set; }
        public bool owner_appeal_nsfw { get; set; }
    }

    public class TumblelogData1
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class TumblelogParentData1
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class TumblelogRootData1
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class Share_Popover_Data1
    {
        public string tumblelog_name { get; set; }
        public string embed_key { get; set; }
        public string embed_did { get; set; }
        public long post_id { get; set; }
        public string root_id { get; set; }
        public string post_url { get; set; }
        public string post_tiny_url { get; set; }
        public int is_private { get; set; }
        public bool has_user { get; set; }
        public bool has_facebook { get; set; }
        public string twitter_username { get; set; }
        public string permalink_label { get; set; }
        public bool show_reporting_links { get; set; }
        public string abuse_url { get; set; }
        public bool show_pinterest { get; set; }
        public Pinterest_Share_Window pinterest_share_window { get; set; }
        public bool show_reddit { get; set; }
        public bool show_flagging { get; set; }
    }

    public class Pinterest_Share_Window
    {
        public string url { get; set; }
        public string name { get; set; }
        public string dimensions { get; set; }
    }

    public class _173830443034
    {
        public string id { get; set; }
        public string type { get; set; }
        public string root_id { get; set; }
        public string tumblelog { get; set; }
        public string tumblelogkey { get; set; }
        public TumblelogData2 tumblelogdata { get; set; }
        public TumblelogParentData2 tumblelogparentdata { get; set; }
        public bool tumblelogrootdata { get; set; }
        public string reblog_key { get; set; }
        public bool is_reblog { get; set; }
        public bool is_mine { get; set; }
        public bool liked { get; set; }
        public string sponsored { get; set; }
        public object premium_tracked { get; set; }
        public bool is_recommended { get; set; }
        public object placement_id { get; set; }
        public string reblog_source { get; set; }
        public Share_Popover_Data2 share_popover_data { get; set; }
        public object recommendation_reason { get; set; }
        public bool owner_appeal_nsfw { get; set; }
    }

    public class TumblelogData2
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class TumblelogParentData2
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class Share_Popover_Data2
    {
        public string tumblelog_name { get; set; }
        public string embed_key { get; set; }
        public string embed_did { get; set; }
        public string post_id { get; set; }
        public string root_id { get; set; }
        public string post_url { get; set; }
        public string post_tiny_url { get; set; }
        public int is_private { get; set; }
        public bool has_user { get; set; }
        public bool has_facebook { get; set; }
        public string twitter_username { get; set; }
        public string permalink_label { get; set; }
        public bool show_reporting_links { get; set; }
        public string abuse_url { get; set; }
        public bool show_pinterest { get; set; }
        public Pinterest_Share_Window1 pinterest_share_window { get; set; }
        public bool show_reddit { get; set; }
        public bool show_flagging { get; set; }
    }

    public class Pinterest_Share_Window1
    {
        public string url { get; set; }
        public string name { get; set; }
        public string dimensions { get; set; }
    }

    public class _173830442581
    {
        public long id { get; set; }
        public string type { get; set; }
        public string root_id { get; set; }
        public string tumblelog { get; set; }
        public string tumblelogkey { get; set; }
        public TumblelogData3 tumblelogdata { get; set; }
        public TumblelogParentData3 tumblelogparentdata { get; set; }
        public TumblelogRootData2 tumblelogrootdata { get; set; }
        public string reblog_key { get; set; }
        public bool is_reblog { get; set; }
        public bool is_mine { get; set; }
        public bool liked { get; set; }
        public string sponsored { get; set; }
        public object premium_tracked { get; set; }
        public bool is_recommended { get; set; }
        public object placement_id { get; set; }
        public string reblog_source { get; set; }
        public Share_Popover_Data3 share_popover_data { get; set; }
        public object recommendation_reason { get; set; }
        public bool owner_appeal_nsfw { get; set; }
    }

    public class TumblelogData3
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class TumblelogParentData3
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class TumblelogRootData2
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class Share_Popover_Data3
    {
        public string tumblelog_name { get; set; }
        public string embed_key { get; set; }
        public string embed_did { get; set; }
        public long post_id { get; set; }
        public string root_id { get; set; }
        public string post_url { get; set; }
        public string post_tiny_url { get; set; }
        public int is_private { get; set; }
        public bool has_user { get; set; }
        public bool has_facebook { get; set; }
        public string twitter_username { get; set; }
        public string permalink_label { get; set; }
        public bool show_reporting_links { get; set; }
        public string abuse_url { get; set; }
        public bool show_pinterest { get; set; }
        public Pinterest_Share_Window2 pinterest_share_window { get; set; }
        public bool show_reddit { get; set; }
        public bool show_flagging { get; set; }
    }

    public class Pinterest_Share_Window2
    {
        public string url { get; set; }
        public string name { get; set; }
        public string dimensions { get; set; }
    }

    public class _173830442013
    {
        public long id { get; set; }
        public string type { get; set; }
        public string root_id { get; set; }
        public string tumblelog { get; set; }
        public string tumblelogkey { get; set; }
        public TumblelogData4 tumblelogdata { get; set; }
        public TumblelogParentData4 tumblelogparentdata { get; set; }
        public TumblelogRootData3 tumblelogrootdata { get; set; }
        public string reblog_key { get; set; }
        public bool is_reblog { get; set; }
        public bool is_mine { get; set; }
        public bool liked { get; set; }
        public string sponsored { get; set; }
        public object premium_tracked { get; set; }
        public bool is_recommended { get; set; }
        public object placement_id { get; set; }
        public string reblog_source { get; set; }
        public Share_Popover_Data4 share_popover_data { get; set; }
        public object recommendation_reason { get; set; }
        public bool owner_appeal_nsfw { get; set; }
    }

    public class TumblelogData4
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class TumblelogParentData4
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class TumblelogRootData3
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class Share_Popover_Data4
    {
        public string tumblelog_name { get; set; }
        public string embed_key { get; set; }
        public string embed_did { get; set; }
        public long post_id { get; set; }
        public string root_id { get; set; }
        public string post_url { get; set; }
        public string post_tiny_url { get; set; }
        public int is_private { get; set; }
        public bool has_user { get; set; }
        public bool has_facebook { get; set; }
        public string twitter_username { get; set; }
        public string permalink_label { get; set; }
        public bool show_reporting_links { get; set; }
        public string abuse_url { get; set; }
        public bool show_pinterest { get; set; }
        public Pinterest_Share_Window3 pinterest_share_window { get; set; }
        public bool show_reddit { get; set; }
        public bool show_flagging { get; set; }
    }

    public class Pinterest_Share_Window3
    {
        public string url { get; set; }
        public string name { get; set; }
        public string dimensions { get; set; }
    }

    public class _173830441318
    {
        public long id { get; set; }
        public string type { get; set; }
        public string root_id { get; set; }
        public string tumblelog { get; set; }
        public string tumblelogkey { get; set; }
        public TumblelogData5 tumblelogdata { get; set; }
        public TumblelogParentData5 tumblelogparentdata { get; set; }
        public TumblelogRootData4 tumblelogrootdata { get; set; }
        public string reblog_key { get; set; }
        public bool is_reblog { get; set; }
        public bool is_mine { get; set; }
        public bool liked { get; set; }
        public string sponsored { get; set; }
        public object premium_tracked { get; set; }
        public bool is_recommended { get; set; }
        public object placement_id { get; set; }
        public string reblog_source { get; set; }
        public Share_Popover_Data5 share_popover_data { get; set; }
        public object recommendation_reason { get; set; }
        public bool owner_appeal_nsfw { get; set; }
    }

    public class TumblelogData5
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class TumblelogParentData5
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class TumblelogRootData4
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class Share_Popover_Data5
    {
        public string tumblelog_name { get; set; }
        public string embed_key { get; set; }
        public string embed_did { get; set; }
        public long post_id { get; set; }
        public string root_id { get; set; }
        public string post_url { get; set; }
        public string post_tiny_url { get; set; }
        public int is_private { get; set; }
        public bool has_user { get; set; }
        public bool has_facebook { get; set; }
        public string twitter_username { get; set; }
        public string permalink_label { get; set; }
        public bool show_reporting_links { get; set; }
        public string abuse_url { get; set; }
        public bool show_pinterest { get; set; }
        public Pinterest_Share_Window4 pinterest_share_window { get; set; }
        public bool show_reddit { get; set; }
        public bool show_flagging { get; set; }
    }

    public class Pinterest_Share_Window4
    {
        public string url { get; set; }
        public string name { get; set; }
        public string dimensions { get; set; }
    }

    public class _173830439125
    {
        public long id { get; set; }
        public string type { get; set; }
        public string root_id { get; set; }
        public string tumblelog { get; set; }
        public string tumblelogkey { get; set; }
        public TumblelogData6 tumblelogdata { get; set; }
        public TumblelogParentData6 tumblelogparentdata { get; set; }
        public TumblelogRootData5 tumblelogrootdata { get; set; }
        public string reblog_key { get; set; }
        public bool is_reblog { get; set; }
        public bool is_mine { get; set; }
        public bool liked { get; set; }
        public string sponsored { get; set; }
        public object premium_tracked { get; set; }
        public bool is_recommended { get; set; }
        public object placement_id { get; set; }
        public string reblog_source { get; set; }
        public Share_Popover_Data6 share_popover_data { get; set; }
        public object recommendation_reason { get; set; }
        public bool owner_appeal_nsfw { get; set; }
    }

    public class TumblelogData6
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class TumblelogParentData6
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class TumblelogRootData5
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class Share_Popover_Data6
    {
        public string tumblelog_name { get; set; }
        public string embed_key { get; set; }
        public string embed_did { get; set; }
        public long post_id { get; set; }
        public string root_id { get; set; }
        public string post_url { get; set; }
        public string post_tiny_url { get; set; }
        public int is_private { get; set; }
        public bool has_user { get; set; }
        public bool has_facebook { get; set; }
        public string twitter_username { get; set; }
        public string permalink_label { get; set; }
        public bool show_reporting_links { get; set; }
        public string abuse_url { get; set; }
        public bool show_pinterest { get; set; }
        public Pinterest_Share_Window5 pinterest_share_window { get; set; }
        public bool show_reddit { get; set; }
        public bool show_flagging { get; set; }
    }

    public class Pinterest_Share_Window5
    {
        public string url { get; set; }
        public string name { get; set; }
        public string dimensions { get; set; }
    }

    public class _173830439037
    {
        public long id { get; set; }
        public string type { get; set; }
        public string root_id { get; set; }
        public string tumblelog { get; set; }
        public string tumblelogkey { get; set; }
        public TumblelogData7 tumblelogdata { get; set; }
        public TumblelogParentData7 tumblelogparentdata { get; set; }
        public TumblelogRootData6 tumblelogrootdata { get; set; }
        public string reblog_key { get; set; }
        public bool is_reblog { get; set; }
        public bool is_mine { get; set; }
        public bool liked { get; set; }
        public string sponsored { get; set; }
        public object premium_tracked { get; set; }
        public bool is_recommended { get; set; }
        public object placement_id { get; set; }
        public string reblog_source { get; set; }
        public Share_Popover_Data7 share_popover_data { get; set; }
        public object recommendation_reason { get; set; }
        public bool owner_appeal_nsfw { get; set; }
    }

    public class TumblelogData7
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class TumblelogParentData7
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class TumblelogRootData6
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class Share_Popover_Data7
    {
        public string tumblelog_name { get; set; }
        public string embed_key { get; set; }
        public string embed_did { get; set; }
        public long post_id { get; set; }
        public string root_id { get; set; }
        public string post_url { get; set; }
        public string post_tiny_url { get; set; }
        public int is_private { get; set; }
        public bool has_user { get; set; }
        public bool has_facebook { get; set; }
        public string twitter_username { get; set; }
        public string permalink_label { get; set; }
        public bool show_reporting_links { get; set; }
        public string abuse_url { get; set; }
        public bool show_pinterest { get; set; }
        public Pinterest_Share_Window6 pinterest_share_window { get; set; }
        public bool show_reddit { get; set; }
        public bool show_flagging { get; set; }
    }

    public class Pinterest_Share_Window6
    {
        public string url { get; set; }
        public string name { get; set; }
        public string dimensions { get; set; }
    }

    public class _173830395871
    {
        public string id { get; set; }
        public string type { get; set; }
        public string root_id { get; set; }
        public string tumblelog { get; set; }
        public string tumblelogkey { get; set; }
        public TumblelogData8 tumblelogdata { get; set; }
        public TumblelogParentData8 tumblelogparentdata { get; set; }
        public TumblelogRootData7 tumblelogrootdata { get; set; }
        public string reblog_key { get; set; }
        public bool is_reblog { get; set; }
        public bool is_mine { get; set; }
        public bool liked { get; set; }
        public string sponsored { get; set; }
        public object premium_tracked { get; set; }
        public bool is_recommended { get; set; }
        public object placement_id { get; set; }
        public string reblog_source { get; set; }
        public Share_Popover_Data8 share_popover_data { get; set; }
        public object recommendation_reason { get; set; }
        public bool owner_appeal_nsfw { get; set; }
    }

    public class TumblelogData8
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class TumblelogParentData8
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class TumblelogRootData7
    {
        public string avatar_url { get; set; }
        public string dashboard_url { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string cname { get; set; }
        public string description { get; set; }
        public string description_sanitized { get; set; }
        public string title { get; set; }
        public bool likes { get; set; }
        public bool share_following { get; set; }
        public bool is_blogless_advertiser { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public bool customizable { get; set; }
        public bool following { get; set; }
        public bool premium_partner { get; set; }
        public bool can_receive_messages { get; set; }
        public bool can_send_messages { get; set; }
        public string uuid { get; set; }
        public bool can_be_followed { get; set; }
        public bool has_default_header { get; set; }
        public bool can_pixelate_avatar { get; set; }
        public bool nsfw { get; set; }
        public bool asks { get; set; }
        public int anonymous_asks { get; set; }
        public bool submissions { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
        public bool is_blocking { get; set; }
    }

    public class Share_Popover_Data8
    {
        public string tumblelog_name { get; set; }
        public string embed_key { get; set; }
        public string embed_did { get; set; }
        public string post_id { get; set; }
        public string root_id { get; set; }
        public string post_url { get; set; }
        public string post_tiny_url { get; set; }
        public int is_private { get; set; }
        public bool has_user { get; set; }
        public bool has_facebook { get; set; }
        public string twitter_username { get; set; }
        public string permalink_label { get; set; }
        public bool show_reporting_links { get; set; }
        public string abuse_url { get; set; }
        public bool show_pinterest { get; set; }
        public string pinterest_share_window { get; set; }
        public bool show_reddit { get; set; }
        public bool show_flagging { get; set; }
    }

    public class _173830392550
    {
        public string id { get; set; }
        public string type { get; set; }
        public long root_id { get; set; }
        public string tumblelog { get; set; }
        public string tumblelogkey { get; set; }

        public bool tumblelogrootdata { get; set; }
        public string reblog_key { get; set; }
        public bool is_reblog { get; set; }
        public bool is_mine { get; set; }
        public bool liked { get; set; }
        public string sponsored { get; set; }
        public object premium_tracked { get; set; }
        public bool is_recommended { get; set; }
        public object placement_id { get; set; }
        public string reblog_source { get; set; }
        public Share_Popover_Data9 share_popover_data { get; set; }
        public object recommendation_reason { get; set; }
        public bool owner_appeal_nsfw { get; set; }
    }


    public class Share_Popover_Data9
    {
        public string tumblelog_name { get; set; }
        public string embed_key { get; set; }
        public string embed_did { get; set; }
        public string post_id { get; set; }
        public string root_id { get; set; }
        public string post_url { get; set; }
        public string post_tiny_url { get; set; }
        public int is_private { get; set; }
        public bool has_user { get; set; }
        public bool has_facebook { get; set; }
        public string twitter_username { get; set; }
        public string permalink_label { get; set; }
        public bool show_reporting_links { get; set; }
        public string abuse_url { get; set; }
        public bool show_pinterest { get; set; }
        public Pinterest_Share_Window7 pinterest_share_window { get; set; }
        public bool show_reddit { get; set; }
        public bool show_flagging { get; set; }
    }

    public class Pinterest_Share_Window7
    {
        public string url { get; set; }
        public string name { get; set; }
        public string dimensions { get; set; }
    }

    public class Dfpad
    {
        public string serve_id { get; set; }
        public int dfp_id { get; set; }
        public string dfp_order { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string gemini_section_code { get; set; }
    }

    //public class Flags
    //{
    //    public string doods { get; set; }
    //}

    public class Context
    {
        public Userinfo userinfo { get; set; }
    }

    public class Userinfo
    {
        public int friend_count { get; set; }
        public List<Channel> channels { get; set; }
    }

    public class Channel
    {
        public string avatar_url { get; set; }
        public string blog_url { get; set; }
        public string directory_safe_title { get; set; }
        public bool is_legacy_private_channel { get; set; }
        public bool is_private_group_channel { get; set; }
        public bool is_password_protected { get; set; }
        public string name { get; set; }

        public string mention_key { get; set; }

        //public string uuid { get; set; }
        //public bool show_author_avatar { get; set; }
        //public object[] tags { get; set; }
        //public bool can_send_to_twitter { get; set; }
        //public bool twitter_send_posts { get; set; }
        //public bool can_send_to_facebook { get; set; }
        //public bool facebook_opengraph_send_posts { get; set; }
        //public int unread_count { get; set; }
        //public bool is_current { get; set; }
        //public bool can_send_messages { get; set; }
        //public bool can_receive_messages { get; set; }
        //public int unread_messages_count_sum { get; set; }
        //public bool is_messaging_bot { get; set; }
        //public bool is_nsfw { get; set; }
        //public bool is_adult { get; set; }
        public int post_count { get; set; }

        public int follower_count { get; set; }

        //public int member_count { get; set; }
        //public int queued_post_count { get; set; }
        //public int draft_count { get; set; }
        //public int transcoding_count { get; set; }
        //public string dashboard_url { get; set; }
        //public bool radar_feature_premium { get; set; }
        //public bool is_visible_premium_partner { get; set; }
        //public string analytics_url { get; set; }
        //public bool can_view_customize { get; set; }
        public Activity_Data activity_data { get; set; }
    }

    public class Activity_Data
    {
        public string sparkline { get; set; }
        public bool can_show_activity_page { get; set; }
    }

    public class Hosts
    {
        public string assets_host { get; set; }
        public string secure_assets_host { get; set; }
        public string www_host { get; set; }
        public string secure_www_host { get; set; }
        public string embed_host { get; set; }
        public string safe_host { get; set; }
        public string platform_host { get; set; }
    }

    public class Translations
    {
        public string _1sReport2sspost3sIfitviolatesourcommunityguidelineswellremoveit4s { get; set; }
        public string _1sReport2ssreply3sIfitviolatesourcommunityguidelineswellremoveit4s { get; set; }
        public string Trytheseeh { get; set; }
        public string PrivateUrl { get; set; }
        public string Letthemreplyto { get; set; }
        public string Letthemreplytospan { get; set; }
        public string Sadlynothing { get; set; }
        public string Tragicallynothing { get; set; }
    }
}