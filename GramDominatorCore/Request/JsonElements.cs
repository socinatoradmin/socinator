using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace GramDominatorCore.Request
{
    public class JsonElements
    {
        [JsonProperty(PropertyName = "multi_sharing")]
        public string MultiSharing { get; set; }

        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        [JsonProperty(PropertyName = "activity_module")]
        public string ActivityModule { get; set; }

        [JsonProperty(PropertyName = "adid")]
        public string Adid { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "av_latitude")]
        public double AvLatitude { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "av_longitude")]
        public double AvLongitude { get; set; }

        [JsonProperty(PropertyName = "battery_level")]
        public int? BatteryLevel { get; set; }

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

        [JsonProperty(PropertyName = "supported_capabilities_new")]
        public string SupportedCapabilitiesNew { get; set; }

        [JsonProperty(PropertyName = "_csrftoken")]
        public string Csrftoken { get; set; }

        [JsonProperty(PropertyName = "device")]
        public DeviceJson Device { get; set; }

        [JsonProperty(PropertyName = "device_id")]
        public string DeviceId { get; set; }

        [JsonProperty(PropertyName = "edits")]
        public EditsJson Edits { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "experiments")]
        public string Experiments { get; set; }

        [JsonProperty(PropertyName = "external_url")]
        public string ExternalUrl { get; set; }

        [JsonProperty(PropertyName = "extra")]
        public ExtraJson Extra { get; set; }

        [JsonProperty(PropertyName = "feed_view_info")]
        public string FeedViewInfo { get; set; }

        [JsonProperty(PropertyName = "full_name")]
        public string FullName { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "gender")]
        public int Gender { get; set; }

        //[JsonProperty(PropertyName = "geotag_enabled")]
        //public string GeotagEnableStatus { get; set; }

        [JsonProperty(PropertyName = "guid")]
        public string Guid { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "idempotence_token")]
        public string IdempotenceToken { get; set; }

        [JsonProperty(PropertyName = "image_compression")]
        public string ImageCompression { get; set; }


        [JsonProperty(PropertyName = "retry_context")]
        public string RetryContext { get; set; }

        [JsonProperty(PropertyName = "photo")]
        public string Photo { get; set; }

        [JsonProperty(PropertyName = "is_charging")]
        public int? IsCharging { get; set; }

        [JsonProperty(PropertyName = "is_prefetch")]
        public string IsPrefetch { get; set; }

        [JsonProperty(PropertyName = "is_pull_to_refresh")]
        public int? IsPullToRefresh { get; set; }

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

        [JsonProperty(PropertyName = "container_module")]
        public string ContainerModuleForLike { get; set; }


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

        [JsonProperty(PropertyName = "query")]
        public string Query { get; set; }

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

        [JsonProperty(PropertyName = "seen_steps")]
        public string SeenSteps { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "show_threads")]
        public bool ShowThreads { get; set; }

        [JsonProperty(PropertyName = "source_type")]
        public string SourceType { get; set; }
        [JsonProperty(PropertyName = "tap_source")]
        public string TapSource { get; set; }

        [JsonProperty(PropertyName = "camera_session_id")]
        public string CameraSessionid { get; set; }

        [JsonProperty(PropertyName = "clips_segments_metadata")]
        public ClipsSegmentsMetadataJson ClipsSegmentMetadata { get; set; }

        //[JsonProperty(PropertyName = "clips_segments")]
        //public ClipsSegmentsJson[] ClipsSegments { get;  set; }

        [JsonProperty(PropertyName = "story_hashtags")]
        public object StoryHashtags { get; set; }

        [JsonProperty(PropertyName = "surface_param")]
        public string SurfaceParam { get; set; }

        [JsonProperty(PropertyName = "tray_session_id")]
        public string traySessionId { get; set; }

        [JsonProperty(PropertyName = "viewer_session_id")]
        public string ViewerSessionId { get; set; }

        [JsonProperty(PropertyName = "entry_point_index")]
        public string EntryPointIndex { get; set; }


        [JsonProperty(PropertyName = "tray_user_ids")]
        public string[] TrayUserids { get; set; }

        [JsonProperty(PropertyName = "thread_ids")]
        public object ThreadIds { get; set; }

        [JsonProperty(PropertyName = "thread_title")]
        public string ThreadTitle { get; set; }

        [JsonProperty(PropertyName = "timezone_offset")]
        public string TimezoneOffset { get; set; }

        [JsonProperty(PropertyName = "two_factor_identifier")]
        public string TwoFactorIdentifier { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "_uid")]
        public string Uid { get; set; }

        [JsonProperty(PropertyName = "unseen_posts")]
        public string UnseenPosts { get; set; }

        [JsonProperty(PropertyName = "upload_id")]
        public string UploadId { get; set; }

        [JsonProperty(PropertyName = "user_breadcrumb")]
        public string UserBreadcrumb { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "user_ids")]
        public string[] UserIds { get; set; }

        //[JsonProperty(PropertyName = "user_ids")]
        //public string[] LstUserIds { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "usertags")]
        public string Usertags { get; set; }

        [JsonProperty(PropertyName = "reel_mentions")]
        public string ReelMentions { get; set; }

        [JsonProperty(PropertyName = "story_locations")]
        public string StoryLocations { get; set; }

        [JsonProperty(PropertyName = "story_sticker_ids")]
        public string StoryStickerIds { get; set; }

        [JsonProperty(PropertyName = "_uuid")]
        public string Uuid { get; set; }

        [JsonProperty(PropertyName = "uuid")]
        public string UuidS { get; set; }

        [JsonProperty(PropertyName = "vc_policy")]
        public string VcPolicy { get; set; }

        [JsonProperty(PropertyName = "verification_code")]
        public string VerificationCode { get; set; }

        [JsonProperty(PropertyName = "verification_method")]
        public string VerificationMethod { get; set; }

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

        [JsonProperty(PropertyName = "usage")]
        public string Usage { get; set; }

        [JsonProperty(PropertyName = "security_code")]
        public string SecurityCode { get; set; }
        [JsonProperty(PropertyName = "should_promote_account_status")]
        public string ShouldPromoteAccountStatus { get; set; }
        [JsonProperty(PropertyName = "choice")]
        public string Choice { get; set; }
        [JsonProperty(PropertyName = "nav_chain")]
        public string NavChain { get; set; }
        [JsonProperty(PropertyName = "logging_info_token")]
        public string LoggingInfoToken { get; set; }

        [JsonProperty(PropertyName = "nest_data_manifest")]
        public bool? NestDataManifest { get; set; }

        [JsonProperty(PropertyName = "media_type")]
        public string MediaType { get; set; }

        [JsonProperty(PropertyName = "filter_type")]
        public string FilterType { get; set; }
     
        [JsonProperty(PropertyName = "video_result")]
        public string VideoResult { get; set; }

        [JsonProperty(PropertyName = "clips")]
        public ClipJson[] Clips { get; set; }

        [JsonProperty(PropertyName = "media_info")]
        public MediaInfo mediaInfos { get; set; }
        [JsonProperty(PropertyName = "rich_text_format_types")]
        public string RichTextFormatTypes { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore,PropertyName = "audio_muted")]

        public bool AudioMuted { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "is_suggested_venue")]
        public bool IsSuggestedVenue { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "poster_frame_index")]
        public int PosterFrameIndex { get; set; }

        [JsonProperty(PropertyName = "upload_media_duration_ms")]
        public string UploadMediaDurationMS { get; set; }

        [JsonProperty(PropertyName = "additional_audio_info")]
        public AdditionalAudioInfo AdditionalInfo { get; set; }

        [JsonProperty(PropertyName = "upload_media_height")]
        public string UploadMediaHeight { get; set; }

        [JsonProperty(PropertyName = "upload_media_width")]
        public string UploadMediaWidth { get; set; }

        [JsonProperty(PropertyName = "client_context")]
        public string ClientContext { get; set; }

        [JsonProperty(PropertyName = "is_shh_mode")]
        public string ShhMode { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "raw_text")]
        public string RawText { get; set; }

        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "link_text")]
        public string LinkText { get; set; }

        [JsonProperty(PropertyName = "link_urls")]
        public string LinkUrls { get; set; }
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "disable_comments")]
        public bool DisableComments { get; set; }

        [JsonProperty(PropertyName = "camera_position")]
        public string CameraPosition { get; set; }

        [JsonProperty(PropertyName = "is_sidecar")]
        public string IsSidecar { get; set; }

        [JsonProperty(PropertyName = "client_sidecar_id")]
        public string ClientSidecarId { get; set; }

        [JsonProperty(DefaultValueHandling= DefaultValueHandling.Ignore, PropertyName = "geotag_enabled")]
        public bool GeotagEnabled { get; set; }

        [JsonProperty(PropertyName = "children_metadata")]
        public JArray ChildrenMetadata { get; set; }

        [JsonProperty(PropertyName = "module")]
        public string Module { get; set; }

        [JsonProperty(PropertyName = "paginate")]
        public string paginate { get; set; }     
        
        [JsonProperty(PropertyName = "reel_auto_archive")]  
        public string reel_auto_archive { get; set; }

        [JsonProperty(PropertyName = "capture_type")]
        public string capture_type { get; set; }

        [JsonProperty(PropertyName = "audience")]
        public string audience { get; set; }

       // [JsonProperty(PropertyName = "length")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "length")]
        public double Length { get; set; }

        [JsonProperty(PropertyName = "country_codes")]
        public string CountryCodes { get; set; }

        [JsonProperty(PropertyName = "allow_multi_configures")]
        public string AllowMultiConfigures { get; set; }

        [JsonProperty(PropertyName = "target_posts_author_id")]
        public string Target_posts_author_id { get; set; }

        [JsonProperty(PropertyName = "target_reel_author_id")]
        public string Target_reel_author_id { get; set; }

        [JsonProperty(PropertyName = "google_tokens")]
        public string GoogleTokens { get; set; }

        [JsonProperty(PropertyName = "alt")]
        public string alt { get; set; }

        [JsonProperty(PropertyName = "lat")]
        public string lat { get; set; }

        [JsonProperty(PropertyName = "lng")]
        public string lng { get; set; }

        [JsonProperty(PropertyName = "speed")]
        public string speed { get; set; }

        [JsonProperty(PropertyName = "horizontalAccuracy")]
        public string horizontalAccuracy { get; set; }

        [JsonProperty(PropertyName = "imported_taken_at")]
        public string ImportedTakenAt { get; set; }

        [JsonProperty(PropertyName = "implicit_location")]
        public string ImplicitLocation { get; set; }
        
        [JsonProperty(PropertyName = "xsharing_user_ids")]
        public string XSharingUserids { get; set; }

        [JsonProperty(PropertyName = "segmentation_model_version")]
        public string SegmentationModelVersion { get; set; }

        [JsonProperty(PropertyName = "aml_facetracker_model_version")]
        public string AmlFacetrackerModelVersion { get; set; }

        [JsonProperty(PropertyName = "model_request_blobs")]
        public string ModelRequestBlobs { get; set; }

        [JsonProperty(PropertyName = "is_multi_upload")]
        public string IsMultiUpload { get; set; }
        [JsonProperty(PropertyName = "multi_upload_session_id")]
        public string MultiUploadSessionId { get; set; }

        [JsonProperty(PropertyName = "tos_version")]
        public string TosVersion { get; set; }

        [JsonProperty(PropertyName = "allow_contacts_sync")]
        public string AllowContactsSync { get; set; }

        [JsonProperty(PropertyName = "sn_result")]
        public string SnResult { get; set; }
     
        [JsonProperty(PropertyName = "sn_nonce")]
        public string SnNonce { get; set; }

        [JsonProperty(PropertyName = "login_nonces")]
        public string LoginNonces { get; set; }

        [JsonProperty(PropertyName = "qe_id")]
        public string QeId { get; set; }

        [JsonProperty(PropertyName = "force_sign_up_code")]
       
        public string ForceSignUpCode { get; set; }

        [JsonProperty(PropertyName = "waterfall_id")]
        public string WaterfallId { get; set; }
        
        [JsonProperty(PropertyName = "qs_stamp")]
        public string QsStamp { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "fb_connected")]
        public string FbConnected { get; set; }

        [JsonProperty(PropertyName = "progress_state")]
        public string ProgressState { get; set; }
    
        [JsonProperty(PropertyName = "fb_installed")]
        public string FbInstalled { get; set; }
       
        [JsonProperty(PropertyName = "locale")]
        public string Locale { get; set; }
       
        [JsonProperty(PropertyName = "network_type")]
        public string NetworkType { get; set; }
    
        [JsonProperty(PropertyName = "is_ci")]
        public string IsCi { get; set; }

        [JsonProperty(PropertyName = "android_id")]
        public string AndroidId { get; set; }

        [JsonProperty(PropertyName = "reg_flow_taken")]
        public string RegFlowTaken { get; set; }
      
        [JsonProperty(PropertyName = "tos_accepted")]
        public string TosAccepted { get; set; }
       
        [JsonProperty(PropertyName = "tab")]
        public string Tab { get; set; }
     
        [JsonProperty(PropertyName = "session_id")]
        public string SessionId { get; set; }

        [JsonProperty(PropertyName = "page")]
        public string Page { get; set; }
       
        [JsonProperty(PropertyName = "next_media_ids")]
        public string NextMediaIds { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore ,PropertyName = "include_persistent")]
        public bool IncludePersistent { get; set; }

        [JsonProperty(PropertyName = "supported_tabs")]
        public string SupportedTabs { get; set; }

        [JsonProperty(PropertyName = "date_time_original")]
        public string DateTimeOriginal { get; set; }

        [JsonProperty(PropertyName = "client_session_id")]
        public string clientSessionId { get; set; }

        [JsonProperty(PropertyName = "will_sound_on")]
        public int? WillSoundOn { get; set; }

        [JsonProperty(PropertyName = "is_async_ads_in_headload_enabled")]
        public string isAsyncAdsInHeadloadEnabled { get; set; }

        [JsonProperty(PropertyName = "rti_delivery_backend")]
        public string RtiDeliveryBackend { get; set; }

        [JsonProperty(PropertyName = "is_async_ads_double_request")]
        public string IsAsyncAdsDoubleRequest { get; set; }

        [JsonProperty(PropertyName = "is_async_ads_rti")]
        public string isAsyncAdsRti { get; set; }

        [JsonProperty(PropertyName = "mobile_subno_usage")]
        public string MobileSubnoUsage { get; set; }

        [JsonProperty(PropertyName = "configs")]
        public string Configs { get; set; }

        [JsonProperty(PropertyName = "media_id_attribution")]
        public string MediaIdAttribution { get; set; }

        [JsonProperty(PropertyName = "max_number_to_display")]
        public string MaxNumberToDisplay { get; set; }
        //is_carousel_bumped_post
        [JsonProperty(PropertyName = "is_carousel_bumped_post")]
        public string IsCarouselBumpedPost { get; set; }

        [JsonProperty(PropertyName = "feed_position")]
        public string FeedPosition { get; set; }

        [JsonProperty(PropertyName = "users")]
        public string Users { get; set; }

        [JsonProperty(PropertyName = "hashtag_name")]
        public string HashtagName { get; set; }

        [JsonProperty(PropertyName = "location_id")]
        public string LocationId { get; set; }
        [JsonProperty(PropertyName = "family_device_id")]
        public string FamilyDeviceId { get; set; }

        //[JsonProperty(PropertyName = "device_type")]
        //public string DeviceType { get; set; }

        //[JsonProperty(PropertyName = "is_main_push_channel")]
        //public bool IsMainPushChannel { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "device_sub_type")]
        public int DeviceSubType { get; set; }

        [JsonProperty(PropertyName = "device_token")]
        public string DeviceToken { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "is_on_screen")]
        public bool IsOnScreen { get; set; }

        [JsonProperty(PropertyName = "bloks_versioning_id")]
        public string BlockVersioningId { get; set; }
        [JsonProperty(PropertyName = "bloks_version")]
        public string BlockVersion { get; set; }

        [JsonProperty(PropertyName = "scene_capture_type")]
        public string SceneCaptureType { get; set; }

        [JsonProperty(PropertyName = "date_time_digitalized")]
        public string DateTimeDigitalized { get; set; }

        [JsonProperty(PropertyName = "camera_model")]
        public string CameraModel { get; set; }

        [JsonProperty(PropertyName = "camera_make")]
        public string CameraMake { get; set; }

        [JsonProperty(PropertyName = "creation_logger_session_id")]
        public string CreationLoggerSessionId { get; set; }

        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "reel_media_skipped")]
        public Dictionary<string,string> ReelMediaSkipped { get; set; }
        [JsonProperty(PropertyName = "live_vods_skipped")]
        public Dictionary<string, string> LiveVodsSkipped { get; set; }

        [JsonProperty(PropertyName = "nuxes")]
        public Dictionary<string, string> Nuxes { get; set; }

        [JsonProperty(PropertyName = "nuxes_skipped")]
        public Dictionary<string, string> NuxesSkipped { get; set; }

        [JsonProperty(PropertyName = "live_vods")]
        public Dictionary<string, string> LiveVods { get; set; }

        [JsonProperty(PropertyName = "reels")]
        public Dictionary<string,string[]> Reels { get; set; }

        [JsonProperty(PropertyName = "mutation_token")]
        public string MutationToken { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "allow_full_aspect_ratio")]
        public bool AllowFullAspectRatio { get; set; }

        //[JsonProperty(PropertyName = "one_tap_app_login")]
        //public string OneTapAppLogin { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "use_fbuploader")]
        public bool UseFbuploader { get; set; }

        [JsonProperty(PropertyName = "enc_password")]
        public string Enc_Password { get; set; }

        [JsonProperty(PropertyName = "jazoest")]
        public string Jazoest { get; set; }

        [JsonProperty(PropertyName = "offline_threading_id")]
        public string OfflineThreadingId { get; set; }

        [JsonProperty(PropertyName = "trust_this_device")]
        public string TrustThisDevice { get; set; }

        [JsonProperty(PropertyName = "sampled")]
        public string Sampled { get; set; }

        [JsonProperty(PropertyName = "server_config_retrieval")]
        public string ServerConfigRetrieval { get; set; }

        [JsonProperty(PropertyName = "android_device_id")]
        public string AndroidDeviceId { get; set; }

        [JsonProperty(PropertyName = "client_contact_points")]
        public string ClientContactPoints { get; set; }

        [JsonProperty(PropertyName = "last_unseen_ad_id")]
        public string Last_Unseen_Ad_Id { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "is_dark_mode")]
        //[JsonProperty(PropertyName = "is_dark_mode")]
        public int? Is_Dark_Mode { get; set; }

        [JsonProperty(PropertyName = "request_id")]
        public string RequestId { get; set; }

        [JsonProperty(PropertyName = "delivery_class")]
        public string Delivery_Class { get; set; }

        [JsonProperty(PropertyName = "inventory_source")]
        public string InventorySource { get; set; }

        [JsonProperty(PropertyName = "entity_page_id")]
        public string EntityPageId { get; set; }

        [JsonProperty(PropertyName = "entity_page_name")]
        public string EntityPageName { get; set; }

        [JsonProperty(PropertyName = "surface")]
        public string Surface { get; set; }

        [JsonProperty(PropertyName = "nonce_code")]
        public string Nonce_Code { get; set; }

        //challenge_context
        [JsonProperty(PropertyName = "challenge_context")]
        public string Challenge_Context { get; set; }
        [JsonProperty(PropertyName = "perf_logging_id")]
        public string PerfLogging_id { get; set; }
        [JsonProperty(PropertyName = "usages")]
        public string Usages { get; set; }
        [JsonProperty(PropertyName = "normal_token_hash")]
        public object TokenHash { get; internal set; }
        [JsonProperty(PropertyName = "custom_device_id")]
        public string CustomDeviceId { get; set; }
        [JsonProperty(PropertyName = "fetch_reason")]
        public string FetchReason { get; set; }
        [JsonProperty(PropertyName = "params")]
        public JsonElements Params { get; set; }
        [JsonProperty(PropertyName = "client_input_params")]
        public JsonElements ClientInputParams { get; set; }
         [JsonProperty(PropertyName = "server_params")]
        public JsonElements ServerParams { get; set; }

        [JsonProperty(PropertyName = "bk_client_context")]

        public JsonElements BkClientContext { get; set; }
        [JsonProperty(PropertyName = "logged_out_user")]
        public string LoggedOutUser { get; set; }
        [JsonProperty(PropertyName = "offline_experiment_group")]
        public string OfflineExperimentGroup { get; set; }
        [JsonProperty(PropertyName = "show_internal_settings")]
        public bool? ShowInternalSettings { get; set; }
        [JsonProperty(PropertyName = "styles_id")]
        public string InstaStyIeId { get; set; }
        [JsonProperty(PropertyName = "ttrc_join_id")]
        public string TtrJoinId { get; set; }
        [JsonProperty(PropertyName = "bool_opt_policy")]
        public int? BoolOptPolicy { get; set; }
        [JsonProperty(PropertyName = "mobileconfigsessionless")]
        public string MobileConfigSessionless { get; set; }
        [JsonProperty(PropertyName = "api_version")]
        public int? ApiVersion { get; set; }
        [JsonProperty(PropertyName = "unit_type")]
        public int? UnitType { get; set; }
        [JsonProperty(PropertyName = "query_hash")]
        public string QueryHash { get; set; }
        [JsonProperty(PropertyName = "ts")]
        public long? Ts { get; set; } 
        [JsonProperty(PropertyName = "fetch_type")]
        public string FetchType { get; set; }
        [JsonProperty(PropertyName = "panavision_mode")]
        public string PanavisionMode { get; set; }
        [JsonProperty(PropertyName = "has_camera_permission")]
        public int? HasCameraPermission { get; set; }
        [JsonProperty(PropertyName = "reel_tray_impressions")]
        public object ReelTrayImpression { get; set; }
        [JsonProperty(PropertyName = "qpl_join_id")]
        public string QplJoinId { get; set; }
        [JsonProperty(PropertyName = "qe_device_id")]
        public string QeDeviceId { get; set; }
        [JsonProperty(PropertyName = "INTERNAL_INFRA_THEME")]
        public string INTERNAL_INFRA_THEME { get; set; }
        [JsonProperty(PropertyName = "account_list")]
        public IList<object> AccountList { get; set; }
        [JsonProperty(PropertyName = "accounts_list")]
        public IList<object> AccountsList { get; set; }
        [JsonProperty(PropertyName = "blocked_uid")]
        public IList<object> BlockedUid { get; set; }
        [JsonProperty(PropertyName = "secure_family_device_id")]
        public object SecureFamilyDeviceId { get; set; }
        [JsonProperty(PropertyName = "machine_id")]
        public object MachineId { get; set; }
        [JsonProperty(PropertyName = "auth_secure_device_id")]
        public object AuthASecureDeviceId { get; set; }
        [JsonProperty(PropertyName = "fb_ig_device_id")]
        public IList<object> FbIgDeviceId { get; set; }
        [JsonProperty(PropertyName = "device_emails")]
        public IList<object> DeviceEmails { get; set; }
        [JsonProperty(PropertyName = "try_num")]
        public string TryNum { get; set; }
        [JsonProperty(PropertyName = "event_flow")]
        public string EventFlow { get; set; }
        [JsonProperty(PropertyName = "event_step")]
        public string EventStep { get; set; }
        [JsonProperty(PropertyName = "openid_tokens")]
        public IList<object> OpenIdTokens { get; set; }
        [JsonProperty(PropertyName = "openid_token")]
        public object OpenIdToken { get; set; }
        [JsonProperty(PropertyName = "contact_points")]
        public string ContactPoints { get; set; }
        [JsonProperty(PropertyName = "contact_point")]
        public string ContactPoint { get; set; }
        [JsonProperty(PropertyName = "username_text_input_id")]
        public string UsernameTextInputId { get; set; }
        [JsonProperty(PropertyName = "server_login_source")]
        public string ServerLoginSource { get; set; }
        [JsonProperty(PropertyName = "login_source")]
        public string LoginSource { get; set; }
        [JsonProperty(PropertyName = "password_text_input_id")]
        public string PasswordTextInputId { get; set; }
        [JsonProperty(PropertyName = "credential_type")]
        public string CredentialType { get; set; }

        [JsonProperty(PropertyName = "INTERNAL__latency_qpl_marker_id")]
        public string INTERNALLatencyMarkerId { get; set; }
        [JsonProperty(PropertyName = "INTERNAL__latency_qpl_instance_id")]
        public string INTERNALLatencyInstanceId { get; set; }
        [JsonProperty(PropertyName = "is_platform_login")]
        public int? IsPLatformLogin { get; set; }
        [JsonProperty(PropertyName = "ar_event_source")]
        public string ArEventSource { get; set; }
        [JsonProperty(PropertyName = "account_centers")]
        public string[] AccountCenters { get; set; }
        [JsonProperty(PropertyName = "text_input_id")]
        public string TextInputId { get; set; }
        [JsonProperty(PropertyName = "typeahead_id")]
        public string TypeaheadId { get; set; }
        [JsonProperty(PropertyName = "text_component_id")]
        public string TextComponentId { get; set; }
        [JsonProperty(PropertyName = "screen_id")]
        public string ScreenId { get; set; }
        [JsonProperty(PropertyName = "fdid")]
        public string Fdid { get; set; }

        public class ClipJson
        {
            [JsonProperty(PropertyName = "length")]
            public double Length { get; set; }

            [JsonProperty(PropertyName = "source_type")]
            public string SourceType { get; set; }
        }

        public class DeviceJson
        {
            [JsonProperty(PropertyName = "android_release")]
            public string AndroidRelease { get; set; }

            [JsonProperty(PropertyName = "android_version")]
            public int AndroidVersion { get; set; }

            [JsonProperty(PropertyName = "manufacturer")]
            public string Manufacturer { get; set; }

            [JsonProperty(PropertyName = "model")]
            public string Model { get; set; }
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

        public class AdditionalAudioInfo
        {
            [JsonProperty(PropertyName= "has_voiceover_attribution")]
            public string hasVoiceoverAttribution { get; set; }
        }

        public class ClipsSegmentsMetadataJson
        {
            [JsonProperty(PropertyName = "num_segments")]
            public int Numsegment { get; set; }

            [JsonProperty(PropertyName = "clips_segments")]
            public ClipsSegmentsJson[] ClipsSegments { get; set; }


        }

        public class ClipsSegmentsJson
        {
            [JsonProperty(PropertyName = "index")]
            public int Index { get; set; }

            [JsonProperty(PropertyName = "face_effect_id")]
            public string FaceEffectedid { get; set; }

            [JsonProperty(PropertyName = "speed")]
            public int Speed { get; set; }

            [JsonProperty(PropertyName = "source")]
            public string Source { get; set; }

            [JsonProperty(PropertyName = "duration_ms")]
            public string  DurationInMs { get; set; }

            [JsonProperty(PropertyName = "audio_type")]
            public string AudioType { get; set; }

            [JsonProperty(PropertyName = "from_draft")]
            public string FromDraft { get; set; }

            [JsonProperty(PropertyName = "camera_position")]
            public int CameraPosition { get; set; }

            [JsonProperty(PropertyName = "media_folder")]
            public string MediaFolder { get; set; }

            [JsonProperty(PropertyName = "media_type")]
            public string Media_type { get; set; }

            [JsonProperty(PropertyName = "original_media_type")]
            public string OriginalMediaType { get; set; }

        }

        public class MediaInfo
        {
            [JsonProperty(PropertyName = "capture_mode")]
            public string capture_mode { get; set; }

            [JsonProperty(PropertyName = "media_type")]
            public string media_type { get; set; }

            [JsonProperty(PropertyName = "mentions")]
            public string[] mentions { get; set; }

            [JsonProperty(PropertyName = "hashtags")]
            public string[] hashtags { get; set; }

            [JsonProperty(PropertyName = "locations")]
            public string[] locations { get; set; }

            [JsonProperty(PropertyName = "stickers")]
            public string[] stickers { get; set; }

        }
    }
}
