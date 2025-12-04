using Newtonsoft.Json;

namespace PinDominatorCore.PDModel
{
    public class JsonElements
    {
        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        [JsonProperty(PropertyName = "activity_module")]
        public string ActivityModule { get; set; }

        [JsonProperty(PropertyName = "adid")] public string Adid { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "av_latitude")]
        public double AvLatitude { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "av_longitude")]
        public double AvLongitude { get; set; }

        [JsonProperty(PropertyName = "battery_level")]
        public string BatteryLevel { get; set; }

        [JsonProperty(PropertyName = "biography")]
        public string Biography { get; set; }

        [JsonProperty(PropertyName = "caption")]
        public string Caption { get; set; }

        [JsonProperty(PropertyName = "client_shared_at")]
        public string ClientSharedAt { get; set; }

        [JsonProperty(PropertyName = "client_timestamp")]
        public string ClientTimestamp { get; set; }

        [JsonProperty(PropertyName = "comment_text")]
        public string CommentText { get; set; }

        [JsonProperty(PropertyName = "configure_mode")]
        public string ConfigureMode { get; set; }

        [JsonProperty(PropertyName = "containermodule")]
        public string Containermodule { get; set; }

        [JsonProperty(PropertyName = "_csrftoken")]
        public string Csrftoken { get; set; }

        [JsonProperty(PropertyName = "device")]
        public DeviceJson Device { get; set; }

        [JsonProperty(PropertyName = "device_id")]
        public string DeviceId { get; set; }

        [JsonProperty(PropertyName = "edits")] public EditsJson Edits { get; set; }

        [JsonProperty(PropertyName = "email")] public string Email { get; set; }

        [JsonProperty(PropertyName = "experiments")]
        public string Experiments { get; set; }

        [JsonProperty(PropertyName = "external_url")]
        public string ExternalUrl { get; set; }

        [JsonProperty(PropertyName = "extra")] public ExtraJson Extra { get; set; }

        [JsonProperty(PropertyName = "feed_view_info")]
        public string FeedViewInfo { get; set; }

        [JsonProperty(PropertyName = "full_name")]
        public string FullName { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "gender")]
        public int Gender { get; set; }

        [JsonProperty(PropertyName = "geotag_enabled")]
        public string GeotagEnabled { get; set; }

        [JsonProperty(PropertyName = "guid")] public string Guid { get; set; }

        [JsonProperty(PropertyName = "id")] public string Id { get; set; }

        [JsonProperty(PropertyName = "idempotence_token")]
        public string IdempotenceToken { get; set; }

        [JsonProperty(PropertyName = "image_compression")]
        public string ImageCompression { get; set; }

        [JsonProperty(PropertyName = "is_charging")]
        public string IsCharging { get; set; }

        [JsonProperty(PropertyName = "is_prefetch")]
        public string IsPrefetch { get; set; }

        [JsonProperty(PropertyName = "is_pull_to_refresh")]
        public string IsPullToRefresh { get; set; }

        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "login_attempt_count")]
        public string LoginAttemptCount { get; set; }

        [JsonProperty(PropertyName = "mas_opt_in")]
        public string MasOptIn { get; set; }

        [JsonProperty(PropertyName = "max_id")]
        public string MaxId { get; set; }

        [JsonProperty(PropertyName = "media_folder")]
        public string MediaFolder { get; set; }

        [JsonProperty(PropertyName = "media_id")]
        public string MediaId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "media_latitude")]
        public double MediaLatitude { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "media_longitude")]
        public double MediaLongitude { get; set; }

        [JsonProperty(PropertyName = "module_name")]
        public string ModuleName { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "phone_id")]
        public string PhoneId { get; set; }

        [JsonProperty(PropertyName = "phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "posting_latitude")]
        public double PostingLatitude { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "posting_longitude")]
        public double PostingLongitude { get; set; }

        [JsonProperty(PropertyName = "query")] public string Query { get; set; }

        [JsonProperty(PropertyName = "radio_type")]
        public string RadioType { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "ranked_content")]
        public bool RankedContent { get; set; }

        [JsonProperty(PropertyName = "rank_token")]
        public string RankToken { get; set; }

        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }

        [JsonProperty(PropertyName = "recipient_users")]
        public object RecipientUsers { get; set; }

        [JsonProperty(PropertyName = "replied_to_comment_id")]
        public string RepliedToCommentId { get; set; }

        [JsonProperty(PropertyName = "seen_posts")]
        public string SeenPosts { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "show_threads")]
        public bool ShowThreads { get; set; }

        [JsonProperty(PropertyName = "source_type")]
        public string SourceType { get; set; }

        [JsonProperty(PropertyName = "story_hashtags")]
        public object StoryHashtags { get; set; }

        [JsonProperty(PropertyName = "surface_param")]
        public string SurfaceParam { get; set; }

        [JsonProperty(PropertyName = "thread_ids")]
        public object ThreadIds { get; set; }

        [JsonProperty(PropertyName = "thread_title")]
        public string ThreadTitle { get; set; }

        [JsonProperty(PropertyName = "timezone_offset")]
        public string TimezoneOffset { get; set; }

        [JsonProperty(PropertyName = "two_factor_identifier")]
        public string TwoFactorIdentifier { get; set; }

        [JsonProperty(PropertyName = "type")] public string Type { get; set; }

        [JsonProperty(PropertyName = "_uid")] public string Uid { get; set; }

        [JsonProperty(PropertyName = "unseen_posts")]
        public string UnseenPosts { get; set; }

        [JsonProperty(PropertyName = "upload_id")]
        public string UploadId { get; set; }

        [JsonProperty(PropertyName = "user_breadcrumb")]
        public string UserBreadcrumb { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "user_ids")]
        public string UserIds { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "usertags")]
        public string Usertags { get; set; }

        [JsonProperty(PropertyName = "_uuid")] public string Uuid { get; set; }

        [JsonProperty(PropertyName = "uuid")] public string UuidS { get; set; }

        [JsonProperty(PropertyName = "vc_policy")]
        public string VcPolicy { get; set; }

        [JsonProperty(PropertyName = "verification_code")]
        public string VerificationCode { get; set; }

        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "Content-Type")]

        public string ContentType { get; set; }

        [JsonProperty(PropertyName = "formkey")]
        public string Formkey { get; set; }

        [JsonProperty(PropertyName = "postkey")]
        public string Postkey { get; set; }

        [JsonProperty(PropertyName = "window_id")]
        public string WindowId { get; set; }

        [JsonProperty(PropertyName = "parent_domid")]
        public string ParentDomId { get; set; }

        [JsonProperty(PropertyName = "parent_cid")]
        public string ParentCid { get; set; }

        [JsonProperty(PropertyName = "usage")] public string Usage { get; set; }

        [JsonProperty(PropertyName = "security_code")]
        public string SecurityCode { get; set; }

        [JsonProperty(PropertyName = "choice")]
        public string Choice { get; set; }

        [JsonProperty(PropertyName = "media_type")]
        public string MediaType { get; set; }

        [JsonProperty(PropertyName = "filter_type")]
        public string FilterType { get; set; }

        [JsonProperty(PropertyName = "video_result")]
        public string VideoResult { get; set; }

        [JsonProperty(PropertyName = "clips")] public ClipJson Clips { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "audio_muted")]
        public bool AudioMuted { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "is_suggested_venue")]
        public bool IsSuggestedVenue { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "poster_frame_index")]
        public int PosterFrameIndex { get; set; }

        [JsonProperty(PropertyName = "upload_media_duration_ms")]
        public string UploadMediaDurationMs { get; set; }

        [JsonProperty(PropertyName = "upload_media_height")]
        public string UploadMediaHeight { get; set; }

        [JsonProperty(PropertyName = "upload_media_width")]
        public string UploadMediaWidth { get; set; }


        public class ClipJson
        {
            [JsonProperty(PropertyName = "length")]
            public int Length { get; set; }

            [JsonProperty(PropertyName = "source_type")]
            public int SourceType { get; set; }
        }


        public class DeviceJson
        {
            [JsonProperty(PropertyName = "android_release")]
            public string AndroidRelease { get; set; }

            [JsonProperty(PropertyName = "android_version")]
            public string AndroidVersion { get; set; }

            [JsonProperty(PropertyName = "manufacturer")]
            public string Manufacturer { get; set; }

            [JsonProperty(PropertyName = "model")] public string Model { get; set; }
        }

        public class EditsJson
        {
            [JsonProperty(PropertyName = "crop_center")]
            public double[] CropCenter { get; set; }

            [JsonProperty(PropertyName = "crop_original_size")]
            public int[] CropOriginalSize { get; set; }

            [JsonProperty(PropertyName = "crop_zoom")]
            public int CropZoom { get; set; }
        }

        public class ExtraJson
        {
            [JsonProperty(PropertyName = "source_height")]
            public int SourceHeight { get; set; }

            [JsonProperty(PropertyName = "source_width")]
            public int SourceWidth { get; set; }
        }
    }
}