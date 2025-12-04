using Newtonsoft.Json;

namespace TwtDominatorCore.Requests
{
    public class JsonElementsForPostReq
    {
        [JsonProperty(PropertyName = "challenges_passed")]
        public string ChallengesPassed { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "handles_challenges")]
        public int HandlesChallenges { get; set; }

        [JsonProperty(PropertyName = "include_profile_interstitial_type")]
        public string IncludeProfileInterstitialType { get; set; }

        [JsonProperty(PropertyName = "include_want_retweets")]
        public string IncludeWantRetweets { get; set; }

        [JsonProperty(PropertyName = "include_can_media_tag")]
        public string IncludeCanMediaTtag { get; set; }

        [JsonProperty(PropertyName = "cursor")]
        public string Cursor { get; set; }

        [JsonProperty(PropertyName = "device")]
        public string Device { get; set; }

        [JsonProperty(PropertyName = "include_blocked_by")]
        public string IncludeBlockedBy { get; set; }

        [JsonProperty(PropertyName = "include_blocking")]
        public string IncludeBlocking { get; set; }

        [JsonProperty(PropertyName = "include_can_dm")]
        public string IncludeCanDm { get; set; }

        [JsonProperty(PropertyName = "include_followed_by")]
        public string IncludeFollowedBy { get; set; }

        [JsonProperty(PropertyName = "include_mute_edge")]
        public string IncludeMuteEdge { get; set; }

        [JsonProperty(PropertyName = "skip_status")]
        public string SkipStatus { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "impression_id")]
        public string ImpressionId { get; set; }

        [JsonProperty(PropertyName = "id")] public string Id { get; set; }

        [JsonProperty(PropertyName = "auto_populate_reply_metadata")]
        public string AutoPopulateReplyMetaData { get; set; }

        [JsonProperty(PropertyName = "batch_mode")]
        public string BatchMode { get; set; }

        [JsonProperty(PropertyName = "in_reply_to_status_id")]
        public string InReplyToStatusId { get; set; }

        [JsonProperty(PropertyName = "is_permalink_page")]
        public string IsPermalinkPage { get; set; }

        [JsonProperty(PropertyName = "place_id")]
        public string PlaceId { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "tagged_users")]
        public string TaggedUsers { get; set; }

        [JsonProperty(PropertyName = "weighted_character_count")]
        public string WeightedCharacterCount { get; set; }

        [JsonProperty(PropertyName = "_method")]
        public string Method { get; set; }

        [JsonProperty(PropertyName = "conversation_id")]
        public string ConversationId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "resend_id")]
        public int ResendId { get; set; }

        [JsonProperty(PropertyName = "scribeContext%5Bcomponent%5D")]
        public string ScribeContextComponent { get; set; }

        [JsonProperty(PropertyName = "text")] public string Text { get; set; }

        [JsonProperty(PropertyName = "tweetboxId")]
        public string TweetBoxId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "media_data%5BfileId%5D")]
        public int MediaData_FileId { get; set; }

        [JsonProperty(PropertyName = "media_data%5BfileType%5D")]
        public string MediaData_FileType { get; set; }

        [JsonProperty(PropertyName = "media_data%5BmediaCategory%5D")]
        public string MediaData_MediaCategory { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "media_data%5BuploadId%5D")]
        public int MediaData_UploadId { get; set; }

        [JsonProperty(PropertyName = "media_data%5BmediaType%5D")]
        public string MediaData_MediaType { get; set; }

        [JsonProperty(PropertyName = "media_id")]
        public string MediaId { get; set; }

        [JsonProperty(PropertyName = "media_tags")]
        public string MediaTags { get; set; }

        [JsonProperty(PropertyName = "iframe_callback")]
        public string IFrameCallback { get; set; }

        [JsonProperty(PropertyName = "origin")]
        public string Origin { get; set; }

        [JsonProperty(PropertyName = "upload_id")]
        public string UploadId { get; set; }

        [JsonProperty(PropertyName = "media_ids")]
        public string MediaIds { get; set; }

        [JsonProperty(PropertyName = "session%5Busername_or_email%5D")]
        public string UserNameOrEmail { get; set; }

        [JsonProperty(PropertyName = "session%5Bpassword%5D")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "authenticity_token")]
        public string AuthenticityToken { get; set; }

        [JsonProperty(PropertyName = "ui_metrics")]
        public string UiMetrics { get; set; }

        [JsonProperty(PropertyName = "scribe_log")]
        public string ScribeLog { get; set; }

        [JsonProperty(PropertyName = "return_to_ssl")]
        public string ReturnToSSL { get; set; }

        [JsonProperty(PropertyName = "redirect_after_login")]
        public string RedirectAfterLogin { get; set; }

        [JsonProperty(PropertyName = "card_data")]
        public string CardData { get; set; }

        [JsonProperty(PropertyName = "card_uri")]
        public string CardUri { get; set; }
    }
}