using Newtonsoft.Json;
using System;

namespace PinDominatorCore.Request
{
    public class PdJsonElement
    {
        [JsonProperty(PropertyName = "source_url")]
        public string SourceUrl { get; set; }
        [JsonProperty(PropertyName = "carousel_slot_index")]
        public int? CarouselSlotIntex { get; set; }
        [JsonProperty(PropertyName = "commerce_data")]
        public string CommerceData { get; set; }
        [JsonProperty(PropertyName = "data")] public PdJsonElement Data { get; set; }

        [JsonProperty(PropertyName = "options")]
        public PdJsonElement Options { get; set; }

        [JsonProperty(PropertyName = "context")]
        public PdJsonElement Context { get; set; }

        [JsonProperty(PropertyName = "username_or_email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "recaptchaToken")]
        public string RecaptchaToken { get; set; }

        [JsonProperty(PropertyName = "seamless")]
        public bool? Seamless { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "is_stl_pin")]
        public bool? IsStlPin { get; set; }

        [JsonProperty(PropertyName = "board_id")]
        public string BoardId { get; set; }

        [JsonProperty(PropertyName = "board_section_id")]
        public string BoardSectionId { get; set; }

        [JsonProperty(PropertyName = "objectId")]
        public string ObjectId { get; set; }

        [JsonProperty(PropertyName = "pinId")]
        public string PinId { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }

        [JsonProperty(PropertyName = "clientTrackingParams")]
        public string TrackingParam { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "disable_comments")]
        public bool? DisableComments { get; set; }

        [JsonProperty(PropertyName = "disable_did_it")]
        public bool? DisableDidIt { get; set; }

        [JsonProperty(PropertyName = "guid")] public string Guid { get; set; }

        [JsonProperty(PropertyName = "link")] public string Link { get; set; }

        [JsonProperty(PropertyName = "image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty(PropertyName = "is_buyable_pin")]
        public bool? IsBuyablePin { get; set; }

        [JsonProperty(PropertyName = "is_removable")]
        public bool? IsRemovable { get; set; }

        [JsonProperty(PropertyName = "method")]
        public string Method { get; set; }

        [JsonProperty(PropertyName = "pin_id")]
        public string PinUnderscoreId { get; set; }

        [JsonProperty(PropertyName = "title")] public string Title { get; set; }

        [JsonProperty(PropertyName = "share_facebook")]
        public bool? ShareFacebook { get; set; }

        [JsonProperty(PropertyName = "share_twitter")]
        public bool? ShareTwitter { get; set; }

        [JsonProperty(PropertyName = "details")]
        public string Details { get; set; }

        [JsonProperty(PropertyName = "image_signatures")]
        public string ImageSignatures { get; set; }

        [JsonProperty(PropertyName = "publish_facebook")]
        public bool? PublishFacebook { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "privacy")]
        public string Privacy { get; set; }

        [JsonProperty(PropertyName = "collab_board_email")]
        public bool? CollabBoardEmail { get; set; }

        [JsonProperty(PropertyName = "collaborator_invites_enabled")]
        public bool? CollaboratorInvitesEnabled { get; set; }

        [JsonProperty(PropertyName = "aggregatedCommentId")]
        public string AggregatedCommentId { get; set; }

        [JsonProperty(PropertyName = "user_ids")]
        public string UserIds { get; set; }

        [JsonProperty(PropertyName = "filter")]
        public string Filter { get; set; }

        [JsonProperty(PropertyName = "limit")] public int? Limit { get; set; }

        [JsonProperty(PropertyName = "sort")] public string Sort { get; set; }

        [JsonProperty(PropertyName = "field_set_key")]
        public string FieldSetKey { get; set; }
        [JsonProperty(PropertyName = "currentFilter")]
        public int? CurrentFilter { get; set; }
        [JsonProperty(PropertyName = "skip_board_create_rep")]
        public bool? SkipBoardCreateRep { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "hide_find_friends_rep")]
        public bool? HideFindFriendsRep { get; set; }

        [JsonProperty(PropertyName = "bookmarks")]
        public string[] Bookmarks { get; set; }

        [JsonProperty(PropertyName = "auto_correction_disabled")]
        public bool? AutoCorrectionDisabled { get; set; }

        [JsonProperty(PropertyName = "corpus")]
        public string Corpus { get; set; }

        [JsonProperty(PropertyName = "customized_rerank_type")]
        public string CustomizedRerankType { get; set; }

        [JsonProperty(PropertyName = "filters")]
        public string Filters { get; set; }

        [JsonProperty(PropertyName = "page_size")]
        public int? PageSize { get; set; }

        [JsonProperty(PropertyName = "query")] public string Query { get; set; }

        [JsonProperty(PropertyName = "query_pin_sigs")]
        public string QueryPinSigs { get; set; }

        [JsonProperty(PropertyName = "redux_normalize_feed")]
        public bool? ReduxNormalizeFeed { get; set; }

        [JsonProperty(PropertyName = "rs")] public string Rs { get; set; }

        [JsonProperty(PropertyName = "scope")] public string Scope { get; set; }

        [JsonProperty(PropertyName = "article")]
        public string Article { get; set; }

        [JsonProperty(PropertyName = "board_url")]
        public string BoardUrl { get; set; }

        [JsonProperty(PropertyName = "filter_section_pins")]
        public bool? FilterSectionPins { get; set; }

        [JsonProperty(PropertyName = "layout")]
        public string Layout { get; set; }

        [JsonProperty(PropertyName = "aggregated_pin_data_id")]
        public string AggregatedPinDataId { get; set; }

        [JsonProperty(PropertyName = "show_did_it_feed")]
        public bool? ShowDidItFeed { get; set; }

        [JsonProperty(PropertyName = "is_own_profile_pins")]
        public bool? IsOwnProfilePins { get; set; }

        [JsonProperty(PropertyName = "pin_filter")]
        public string PinFilter { get; set; }

        [JsonProperty(PropertyName = "id")] public string Id { get; set; }

        [JsonProperty(PropertyName = "upload_metric")]
        public PdJsonElement UploadMetric { get; set; }

        [JsonProperty(PropertyName = "scrape_metric")]
        public PdJsonElement ScrapeMetric { get; set; }

        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "app_type_from_client")]
        public string AppTypeFromClient { get; set; }

        [JsonProperty(PropertyName = "isPrefetch")]
        public bool? IsPrefetch { get; set; }

        [JsonProperty(PropertyName = "source_id")]
        public string SourceId { get; set; }

        [JsonProperty(PropertyName = "contact_request")]
        public PdJsonElement ContactRequest { get; set; }

        [JsonProperty(PropertyName = "sender")]
        public PdJsonElement Sender { get; set; }

        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "gender")]
        public string Gender { get; set; }

        [JsonProperty(PropertyName = "image_medium_url")]
        public string ImageMediumUrl { get; set; }

        [JsonProperty(PropertyName = "image_xlarge_url")]
        public string ImageXlargeUrl { get; set; }

        [JsonProperty(PropertyName = "full_name")]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "image_small_url")]
        public string ImageSmallUrl { get; set; }

        [JsonProperty(PropertyName = "type")] public string Type { get; set; }

        [JsonProperty(PropertyName = "image_large_url")]
        public string ImageLargeUrl { get; set; }

        [JsonProperty(PropertyName = "read")] public bool? Read { get; set; }

        [JsonProperty(PropertyName = "recipient")]
        public PdJsonElement Recipient { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty(PropertyName = "conversation")]
        public string Conversation { get; set; }

        [JsonProperty(PropertyName = "board")] public PdJsonElement Board { get; set; }

        [JsonProperty(PropertyName = "board_order_modified_at")]
        public string BoardOrderModifiedAt { get; set; }

        [JsonProperty(PropertyName = "collaborated_by_me")]
        public string CollaboratedByMe { get; set; }

        [JsonProperty(PropertyName = "is_collaborative")]
        public bool? IsCollaborative { get; set; }

        [JsonProperty(PropertyName = "url")] public string Url { get; set; }

        [JsonProperty(PropertyName = "followed_by_me")]
        public string FollowedByMe { get; set; }

        [JsonProperty(PropertyName = "image_thumbnail_url")]
        public string ImageThumbnailUrl { get; set; }

        [JsonProperty(PropertyName = "invited_user_ids")]
        public string[] invitedUserId { get; set; }

        [JsonProperty(PropertyName = "invited_user_id")]
        public string InvitedUserId { get; set; }

        [JsonProperty(PropertyName = "isInboxRedesign")]
        public bool? IsInboxRedesign { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "isRegistration")]
        public bool? IsRegistration { get; set; }

        [JsonProperty(PropertyName = "disable_auth_failure_redirect")]
        public string DisableAuth { get; set; }

        [JsonProperty(PropertyName = "save_password")]
        public string SavePassword { get; set; }

        [JsonProperty(PropertyName = "locale")]
        public string Locale { get; set; }

        [JsonProperty(PropertyName = "privacy_filter")]
        public string PrivacyFilter { get; set; }

        [JsonProperty(PropertyName = "group_by")]
        public string GroupBy { get; set; }

        [JsonProperty(PropertyName = "include_archived")]
        public bool? IncludeArchived { get; set; }

        [JsonProperty(PropertyName = "business_id")]
        public string BusinessId { get; set; }

        [JsonProperty(PropertyName = "get_user")]
        public bool? GetUser { get; set; }

        [JsonProperty(PropertyName = "visited_pages_before_login")]
        public string VisitedPagesBeforeLogin { get; set; }

        [JsonProperty(PropertyName = "skip_pin_create_log")]
        public bool? SkipPinCreateLog { get; set; }

        [JsonProperty(PropertyName = "actions")]
        public PdJsonElement[] Action { get; set; }

        [JsonProperty(PropertyName = "aux_data")]
        public PdJsonElement AuxData { get; set; }

        [JsonProperty(PropertyName = "page")]
        public string Page { get; set; }

        [JsonProperty(PropertyName = "referrer")]
        public string Referrer { get; set; }

        [JsonProperty(PropertyName = "container")]
        public string Container { get; set; }

        [JsonProperty(PropertyName = "hybridTier")]
        public string HybridTier { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public PdJsonElement TagUser { get; set; }

        [JsonProperty(PropertyName = "media_upload_id")]
        public string MediaUploadId { get; set; }

        [JsonProperty(PropertyName = "user_mention_tags")]
        public Array[] UserMentionTag { get; set; }

        [JsonProperty(PropertyName = "age")]
        public string Age { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "signupSource")]
        public string SignupSource { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "interestid")]
        public string Interestid { get; set; }

        [JsonProperty(PropertyName = "logdata")]
        public PdJsonElement LogData { get; set; }

        [JsonProperty(PropertyName = "pos")]
        public string Pos { get; set; }

        [JsonProperty(PropertyName = "user_behavior_data")]
        public PdJsonElement UserBehaviorData { get; set; }

        [JsonProperty(PropertyName = "signupInterestsPickerTimeSpent")]
        public string SignupInterestsPickerTimeSpent { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email1 { get; set; }

        [JsonProperty(PropertyName = "no_fetch_context_on_resource")]
        public bool? nofetchcontext { get; set; }

        [JsonProperty(PropertyName = "name_source")]
        public string name_source { get; set; }

        [JsonProperty(PropertyName = "initial_pins")]
        public Array[] initial_pin { get; set; }
        [JsonProperty(PropertyName = "width")]
        public string Width { get; set; }
        [JsonProperty(PropertyName = "height")]
        public string Height { get; set; }
        [JsonProperty(PropertyName = "downlink")]
        public string Downlink { get; set; }
        [JsonProperty(PropertyName = "effectiveType")]
        public string EffectiveType { get; set; }
        [JsonProperty(PropertyName = "rtt")]
        public string Rtt { get; set; }
        [JsonProperty(PropertyName = "saveData")]
        public bool? SaveData { get; set; }
        [JsonProperty(PropertyName = "isAuth")]
        public bool? IsAuth { get; set; }
        [JsonProperty(PropertyName = "docReferrer")]
        public string DocReferer { get; set; }
        [JsonProperty(PropertyName = "tz")]
        public string TimeZone { get; set; }
        [JsonProperty(PropertyName = "invalid_action")]
        public string InvalidAction { get; set; }
        [JsonProperty(PropertyName = "format")]
        public string Format { get; set; }
        [JsonProperty(PropertyName = "item")]
        public string Item { get; set; }
        [JsonProperty(PropertyName = "within")]
        public string WithIn { get; set; }
        [JsonProperty(PropertyName = "action")]
        public string action { get; set; }
        [JsonProperty(PropertyName = "event")]
        public string Event { get; set; }
        [JsonProperty(PropertyName = "trigger")]
        public string Trigger { get; set; }
    }
}