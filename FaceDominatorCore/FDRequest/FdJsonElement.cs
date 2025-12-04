using Newtonsoft.Json;

namespace FaceDominatorCore.FDRequest
{
    public class FdJsonElement
    {
        [JsonProperty(PropertyName = "lsd")]
        public string Lsd { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "pass")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "timezone")]
        public string Timezone { get; set; }

        [JsonProperty(PropertyName = "lgndim")]
        public string Lgndim { get; set; }

        [JsonProperty(PropertyName = "lgnrnd")]
        public string Lgnrnd { get; set; }

        [JsonProperty(PropertyName = "lgnjs")]
        public string Lgnjs { get; set; }

        [JsonProperty(PropertyName = "ab_test_data")]
        public string AbTestData { get; set; }

        [JsonProperty(PropertyName = "locale")]
        public string Locale { get; set; }

        [JsonProperty(PropertyName = "login_source")]
        public string LoginSource { get; set; }

        [JsonProperty(PropertyName = "prefill_contact_point")]
        public string PrefillContactPoint { get; set; }

        [JsonProperty(PropertyName = "prefill_source")]
        public string PrefillSource { get; set; }

        [JsonProperty(PropertyName = "prefill_type")]
        public string PrefillType { get; set; }

        [JsonProperty(PropertyName = "skstamp")]
        public string Skstamp { get; set; }

        [JsonProperty(PropertyName = "field_type")]
        public string FieldType { get; set; }

        [JsonProperty(PropertyName = "nctr[_mod]")]
        public string NctrMod { get; set; }

        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }


        [JsonProperty(PropertyName = "gid")]
        public string GroupId { get; set; }

        [JsonProperty(PropertyName = "order")]
        public string Order { get; set; }

        [JsonProperty(PropertyName = "view")]
        public string View { get; set; }

        [JsonProperty(PropertyName = "limit")]
        public string Limit { get; set; }

        [JsonProperty(PropertyName = "sectiontype")]
        public string Sectiontype { get; set; }

        [JsonProperty(PropertyName = "cursor")]
        public string Cursor { get; set; }

        [JsonProperty(PropertyName = "start")]
        public string Start { get; set; }

        [JsonProperty(PropertyName = "dpr")]
        public string Dpr { get; set; }



        [JsonProperty(PropertyName = "__user")]
        public string User { get; set; }

        [JsonProperty(PropertyName = "__a")]
        public string ElementA { get; set; }

        [JsonProperty(PropertyName = "__dyn")]
        public string ElementDyn { get; set; }

        [JsonProperty(PropertyName = "__req")]
        public string ElementReq { get; set; }

        [JsonProperty(PropertyName = "__be")]
        public string ElementBe { get; set; }

        [JsonProperty(PropertyName = "fb_dtsg")]
        public string ElementFbDtsg { get; set; }

        [JsonProperty(PropertyName = "jazoest")]
        public string ElementJazoest { get; set; }


        [JsonProperty(PropertyName = "collection_token")]
        public string Collectiontoken { get; set; }



        [JsonProperty(PropertyName = "disablepager")]
        public string Disablepager { get; set; }

        [JsonProperty(PropertyName = "overview")]
        public string Overview { get; set; }

        [JsonProperty(PropertyName = "profile_id")]
        public string Profileid { get; set; }

        [JsonProperty(PropertyName = "pagelet_token")]
        public string Pagelettoken { get; set; }

        [JsonProperty(PropertyName = "tab_key")]
        public string Tabkey { get; set; }

        [JsonProperty(PropertyName = "lst")]
        public string Lst { get; set; }

        [JsonProperty(PropertyName = "ftid")]
        public string Ftid { get; set; }

        //[JsonProperty(PropertyName = "order")]
        //public string Order { get; set; }

        [JsonProperty(PropertyName = "sk")]
        public string Sk { get; set; }

        [JsonProperty(PropertyName = "importer_state")]
        public string Importerstate { get; set; }



        [JsonProperty(PropertyName = "FriendId")]
        public string FriendId { get; set; }

        [JsonProperty(PropertyName = "FriendName")]
        public string FriendName { get; set; }

        [JsonProperty(PropertyName = "friendUrl")]
        public string FriendUrl { get; set; }

        [JsonProperty(PropertyName = "FriendsWithAccount")]
        public string FriendsWithAccount { get; set; }


        [JsonProperty(PropertyName = "ajaxpipe_token")]

        public string Ajaxtoken { get; set; }


        [JsonProperty(PropertyName = "action_history")]
        public string ActionHistory { get; set; }

        [JsonProperty(PropertyName = "HostName")]
        public string HostName { get; set; }

        [JsonProperty(PropertyName = "Referrer")]
        public string Referrer { get; set; }

        [JsonProperty(PropertyName = "LIp")]
        public string LIp { get; set; }

    }

    public class FdPublisherJsonElement
    {
        [JsonProperty(PropertyName = "client_mutation_id")]
        public string ClientMutationId { get; set; }

        [JsonProperty(PropertyName = "actor_id")]
        public string ActorId { get; set; }

        [JsonProperty(PropertyName = "input")]
        public FdPublisherJsonElement Input { get; set; }

        [JsonProperty(PropertyName = "message")]
        public FdPublisherJsonElement Message { get; set; }


        [JsonProperty(PropertyName = "logging")]
        public FdPublisherJsonElement Logging { get; set; }

        [JsonProperty(PropertyName = "with_tags_ids")]
        public string[] WithTagsId { get; set; }

        [JsonProperty(PropertyName = "multilingual_translations")]
        public string[] MultilingualTranslations { get; set; }

        [JsonProperty(PropertyName = "camera_post_context")]
        public FdPublisherJsonElement CameraPostContext { get; set; }


        [JsonProperty(PropertyName = "composer_source_surface")]
        public string ComposerSourceSurface { get; set; }


        [JsonProperty(PropertyName = "composer_entry_point")]
        public string ComposerEntryPoint { get; set; }


        [JsonProperty(PropertyName = "composer_entry_time")]
        public int? ComposerEntryTime { get; set; }


        [JsonProperty(PropertyName = "composer_session_events_log")]
        public FdPublisherJsonElement ComposerSessionEventsLog { get; set; }

        [JsonProperty(PropertyName = "direct_share_status")]
        public string DirectShareStatus { get; set; }

        [JsonProperty(PropertyName = "sponsor_relationship")]
        public string SponsorRelationship { get; set; }


        [JsonProperty(PropertyName = "web_graphml_migration_params")]
        public FdPublisherJsonElement WebGraphmlMigrationParams { get; set; }

        [JsonProperty(PropertyName = "place_attachment_setting")]
        public string PlaceAttachmentSetting { get; set; }


        [JsonProperty(PropertyName = "attachments")]
        public FdPublisherJsonElement[] Attachments { get; set; }


        [JsonProperty(PropertyName = "link")]
        public FdPublisherJsonElement Link { get; set; }

        [JsonProperty(PropertyName = "share_scrape_data")]
        public string ShareScrapedData { get; set; }


        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }


        [JsonProperty(PropertyName = "tags")]
        public string[] Tags { get; set; }


        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }


        [JsonProperty(PropertyName = "audience")]
        public FdPublisherJsonElement Audience { get; set; }


        [JsonProperty(PropertyName = "audiences")]
        public FdPublisherJsonElement[] Audiences { get; set; }



        [JsonProperty(PropertyName = "stories")]
        public FdPublisherJsonElement Stories { get; set; }

        [JsonProperty(PropertyName = "self")]
        public FdPublisherJsonElement Self { get; set; }


        [JsonProperty(PropertyName = "to_id")]
        public string ToId { get; set; }



        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }


        [JsonProperty(PropertyName = "ranges")]
        public FdPublisherJsonElement[] Ranges { get; set; }


        [JsonProperty(PropertyName = "deduplication_id")]
        public string DeduplicationId { get; set; }

        [JsonProperty(PropertyName = "composition_duration")]
        public string CompositionDuration { get; set; }

        [JsonProperty(PropertyName = "number_of_keystrokes")]
        public string NumberOfkeystrokes { get; set; }


        [JsonProperty(PropertyName = "target_type")]
        public string TargetType { get; set; }


        [JsonProperty(PropertyName = "xhpc_composerid")]
        public string XhpcComposerid { get; set; }



        [JsonProperty(PropertyName = "xhpc_context")]
        public string XhpcContext { get; set; }



        [JsonProperty(PropertyName = "xhpc_finch")]
        public bool? XhpcFinch { get; set; }



        [JsonProperty(PropertyName = "xhpc_publish_type")]
        public string XhpcPublishType { get; set; }

        [JsonProperty(PropertyName = "xhpc_timeline")]
        public bool? XhpcTimeline { get; set; }


        [JsonProperty(PropertyName = "waterfall_id")]
        public string WaterfallId { get; set; }

        [JsonProperty(PropertyName = "xpost_target_ids")]
        public string XpostTargetIds { get; set; }

        [JsonProperty(PropertyName = "product_item")]
        public FdPublisherJsonElement ProductItem { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "item_price")]
        public FdPublisherJsonElement ItemPrice { get; set; }

        [JsonProperty(PropertyName = "price")]
        public string Price { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "location_page_id")]
        public string LocationId { get; set; }

        [JsonProperty(PropertyName = "condition")]
        public string Condition { get; set; }

        [JsonProperty(PropertyName = "pickup_note")]
        public FdPublisherJsonElement PickupNote { get; set; }

        [JsonProperty(PropertyName = "description")]
        public FdPublisherJsonElement Description { get; set; }

        [JsonProperty(PropertyName = "composer_session_id")]
        public string ComposerSessionId { get; set; }

        [JsonProperty(PropertyName = "ref")]
        public string Ref { get; set; }

        [JsonProperty(PropertyName = "photo")]
        public FdPublisherJsonElement Photo { get; set; }

        [JsonProperty(PropertyName = "video")]
        public FdPublisherJsonElement Video { get; set; }

        [JsonProperty(PropertyName = "notify_when_processed")]
        public bool? NotifyWhenProcessed { get; set; }

        [JsonProperty(PropertyName = "web_privacyx")]
        public string WebPrivacyX { get; set; }

        [JsonProperty(PropertyName = "custom_name")]
        public string CustomName { get; set; }

        [JsonProperty(PropertyName = "video_ids")]
        public string[] VideoIds { get; set; }

        [JsonProperty(PropertyName = "creation_source")]
        public string CreationSource { get; set; }

        [JsonProperty(PropertyName = "composer_type")]
        public string ComposerType { get; set; }

        [JsonProperty(PropertyName = "is_also_posting_video_to_feed")]
        public bool? IsAlsoPostingVideoToFeed { get; set; }

        [JsonProperty(PropertyName = "extensible_sprouts_ranker_request")]
        public FdPublisherJsonElement ExtensibleSproutsRankerRequest { get; set; }

        [JsonProperty(PropertyName = "RequestID")]
        public string RequestId { get; set; }

        [JsonProperty(PropertyName = "external_movie_data")]
        public string[] ExternalMovieData { get; set; }

        [JsonProperty(PropertyName = "living_room_attachment")]
        public FdPublisherJsonElement LivingRoomAttachment { get; set; }

        [JsonProperty(PropertyName = "group_id")]
        public string GrourpId { get; set; }

        [JsonProperty(PropertyName = "living_room_id")]
        public string LivingRoomId { get; set; }

        [JsonProperty(PropertyName = "target_id")]
        public string TargetId { get; set; }

        [JsonProperty(PropertyName = "query_params")]
        public FdPublisherJsonElement QueryParams { get; set; }

        [JsonProperty(PropertyName = "query")]
        public string Query { get; set; }

        [JsonProperty(PropertyName = "viewer_coordinates")]
        public string[] ViewerCoordinates { get; set; }

        [JsonProperty(PropertyName = "provider")]
        public string Provider { get; set; }

        [JsonProperty(PropertyName = "search_type")]
        public string SearchType { get; set; }

        [JsonProperty(PropertyName = "integration_strategy")]
        public string IntegrationStrategy { get; set; }

        [JsonProperty(PropertyName = "result_ordering")]
        public string ResultOrdering { get; set; }

        [JsonProperty(PropertyName = "caller")]
        public string Caller { get; set; }

        [JsonProperty(PropertyName = "geocode_fallback")]
        public bool? GeocodeFallback { get; set; }

        [JsonProperty(PropertyName = "max_results")]
        public int? MaxResults { get; set; }

        [JsonProperty(PropertyName = "photo_width")]
        public int? PhotoWidth { get; set; }

        [JsonProperty(PropertyName = "photo_height")]
        public int? PhotoHeight { get; set; }

        [JsonProperty(PropertyName = "sid_create")]
        public string SidCreate { get; set; }

        [JsonProperty(PropertyName = "action_history")]
        public string ActionHistory { get; set; }

        [JsonProperty(PropertyName = "has_source")]
        public bool? HasSource { get; set; }

        [JsonProperty(PropertyName = "offset")]
        public string Offset { get; set; }

        [JsonProperty(PropertyName = "length")]
        public string Length { get; set; }

        [JsonProperty(PropertyName = "entity")]
        public FdPublisherJsonElement Entity { get; set; }

        [JsonProperty(PropertyName = "o0")]
        public FdPublisherJsonElement o0 { get; set; }

        [JsonProperty(PropertyName = "doc_id")]
        public string DocId { get; set; }

        [JsonProperty(PropertyName = "limit")]
        public int? Limit { get; set; }

        [JsonProperty(PropertyName = "before")]
        public string Before { get; set; }

        [JsonProperty(PropertyName = "0")]
        public string Zero { get; set; }

        [JsonProperty(PropertyName = "isWorkUser")]
        public bool? IsWorkUser { get; set; }

        [JsonProperty(PropertyName = "includeDeliveryReceipts")]
        public bool? IncludeDeliveryReceipts { get; set; }

        [JsonProperty(PropertyName = "includeSeqID")]
        public bool? IncludeSeqID { get; set; }

        [JsonProperty(PropertyName = "privacy")]
        public FdPublisherJsonElement Privacy { get; set; }

        [JsonProperty(PropertyName = "base_state")]
        public string BaseState { get; set; }

        [JsonProperty(PropertyName = "page_category")]
        public FdPublisherJsonElement PageCategory { get; set; }

        [JsonProperty(PropertyName = "1")]
        public string One { get; set; }

        [JsonProperty(PropertyName = "2")]
        public string Two { get; set; }

        [JsonProperty(PropertyName = "groupID")]
        public string GroupID { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int? Count { get; set; }

        [JsonProperty(PropertyName = "cursor")]
        public string Cursor { get; set; }


        // Lcs Update for Ads

        [JsonProperty(PropertyName = "ad_id")]
        public string AdId { get; set; }

        [JsonProperty(PropertyName = "likes_counts")]
        public string LikeCount { get; set; }

        [JsonProperty(PropertyName = "comments_count")]
        public string CommentCount { get; set; }

        [JsonProperty(PropertyName = "shares_count")]
        public string ShareCount { get; set; }

        [JsonProperty(PropertyName = "destination_url")]
        public string DestinationUrl { get; set; }

        [JsonProperty(PropertyName = "comments_data")]
        public string[] CommentData { get; set; }

        [JsonProperty(PropertyName = "analytics")]
        public FdPublisherJsonElement[] Analytics { get; set; }

        [JsonProperty(PropertyName = "change_type")]
        public string ChangeType { get; set; }

    }

    public class FdAdsScraperJsonElement
    {


        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "call_to_action")]
        public string CallToAction { get; set; }

        [JsonProperty(PropertyName = "image_video_url")]
        public string MediaUrl { get; set; }

        [JsonProperty(PropertyName = "other_multimedia")]
        public string OtherMediaUrl { get; set; }

        [JsonProperty(PropertyName = "destination_url")]
        public string DestinationUrl { get; set; }

        [JsonProperty(PropertyName = "ad_title")]
        public string AdTitle { get; set; }


        [JsonProperty(PropertyName = "news_feed_description")]
        public string NewsFeedDescription { get; set; }

        [JsonProperty(PropertyName = "ad_id")]
        public string AdId { get; set; }


        [JsonProperty(PropertyName = "post_date")]
        public string PostDate { get; set; }


        [JsonProperty(PropertyName = "first_seen")]
        public string FirstSeen { get; set; }


        [JsonProperty(PropertyName = "last_seen")]
        public string LastSeen { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }


        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }


        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "lower_age")]
        public string LowerAge { get; set; }

        [JsonProperty(PropertyName = "upper_age")]
        public string UpperAge { get; set; }


        [JsonProperty(PropertyName = "post_owner")]
        public string PostOwner { get; set; }

        [JsonProperty(PropertyName = "post_owner_image")]
        public string PostOwnerImage { get; set; }

        [JsonProperty(PropertyName = "ad_position")]
        public string AdPosition { get; set; }

        [JsonProperty(PropertyName = "likes")]
        public string LikeCount { get; set; }

        [JsonProperty(PropertyName = "comment")]
        public string CommentCount { get; set; }

        [JsonProperty(PropertyName = "share")]
        public string ShareCount { get; set; }

        [JsonProperty(PropertyName = "ad_text")]
        public string AdText { get; set; }

        [JsonProperty(PropertyName = "ad_url")]
        public string AdUrl { get; set; }

        [JsonProperty(PropertyName = "facebook_id")]
        public string FacebookId { get; set; }

        [JsonProperty(PropertyName = "side_url")]
        public string SideUrl { get; set; }

        [JsonProperty(PropertyName = "platform")]
        public string Platform { get; set; }

        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "comments_data")]
        public string[] CommentData { get; set; }

        [JsonProperty(PropertyName = "lsd")]

        public string Lsd { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "pass")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "timezone")]
        public string Timezone { get; set; }

        [JsonProperty(PropertyName = "lgndim")]
        public string Lgndim { get; set; }

        [JsonProperty(PropertyName = "lgnrnd")]
        public string Lgnrnd { get; set; }

        [JsonProperty(PropertyName = "lgnjs")]
        public string Lgnjs { get; set; }

        [JsonProperty(PropertyName = "ab_test_data")]
        public string AbTestData { get; set; }

        [JsonProperty(PropertyName = "locale")]
        public string Locale { get; set; }

        [JsonProperty(PropertyName = "login_source")]
        public string LoginSource { get; set; }

        [JsonProperty(PropertyName = "prefill_contact_point")]
        public string PrefillContactPoint { get; set; }

        [JsonProperty(PropertyName = "prefill_source")]
        public string PrefillSource { get; set; }

        [JsonProperty(PropertyName = "prefill_type")]
        public string PrefillType { get; set; }

        [JsonProperty(PropertyName = "skstamp")]
        public string Skstamp { get; set; }

        [JsonProperty(PropertyName = "field_type")]
        public string FieldType { get; set; }

        [JsonProperty(PropertyName = "nctr[_mod]")]
        public string NctrMod { get; set; }

        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }


        [JsonProperty(PropertyName = "gid")]
        public string GroupId { get; set; }

        [JsonProperty(PropertyName = "order")]
        public string Order { get; set; }

        [JsonProperty(PropertyName = "view")]
        public string View { get; set; }

        [JsonProperty(PropertyName = "limit")]
        public string Limit { get; set; }

        [JsonProperty(PropertyName = "sectiontype")]
        public string Sectiontype { get; set; }

        [JsonProperty(PropertyName = "cursor")]
        public string Cursor { get; set; }

        [JsonProperty(PropertyName = "start")]
        public string Start { get; set; }

        [JsonProperty(PropertyName = "dpr")]
        public string Dpr { get; set; }



        [JsonProperty(PropertyName = "__user")]
        public string User { get; set; }

        [JsonProperty(PropertyName = "__a")]
        public string ElementA { get; set; }

        [JsonProperty(PropertyName = "__dyn")]
        public string ElementDyn { get; set; }

        [JsonProperty(PropertyName = "__req")]
        public string ElementReq { get; set; }

        [JsonProperty(PropertyName = "__be")]
        public string ElementBe { get; set; }

        [JsonProperty(PropertyName = "fb_dtsg")]
        public string ElementFbDtsg { get; set; }

        [JsonProperty(PropertyName = "jazoest")]
        public string ElementJazoest { get; set; }


        [JsonProperty(PropertyName = "collection_token")]
        public string Collectiontoken { get; set; }



        [JsonProperty(PropertyName = "disablepager")]
        public string Disablepager { get; set; }

        [JsonProperty(PropertyName = "overview")]
        public string Overview { get; set; }

        [JsonProperty(PropertyName = "profile_id")]
        public string Profileid { get; set; }

        [JsonProperty(PropertyName = "pagelet_token")]
        public string Pagelettoken { get; set; }

        [JsonProperty(PropertyName = "tab_key")]
        public string Tabkey { get; set; }

        [JsonProperty(PropertyName = "lst")]
        public string Lst { get; set; }

        [JsonProperty(PropertyName = "ftid")]
        public string Ftid { get; set; }

        //[JsonProperty(PropertyName = "order")]
        //public string Order { get; set; }

        [JsonProperty(PropertyName = "sk")]
        public string Sk { get; set; }

        [JsonProperty(PropertyName = "importer_state")]
        public string Importerstate { get; set; }



        [JsonProperty(PropertyName = "FriendId")]
        public string FriendId { get; set; }

        [JsonProperty(PropertyName = "FriendName")]
        public string FriendName { get; set; }

        [JsonProperty(PropertyName = "friendUrl")]
        public string FriendUrl { get; set; }

        [JsonProperty(PropertyName = "FriendsWithAccount")]
        public string FriendsWithAccount { get; set; }


        [JsonProperty(PropertyName = "ajaxpipe_token")]

        public string Ajaxtoken { get; set; }



        [JsonProperty(PropertyName = "socionator")]
        public string Socionator { get; set; }

        [JsonProperty(PropertyName = "verified")]
        public string Verified { get; set; }
    }


    public class FdMarketplaceJsonElements
    {
        [JsonProperty(PropertyName = "bqf")]
        public FdMarketplaceJsonElements BqfElement { get; set; }

        [JsonProperty(PropertyName = "browse_request_params")]
        public FdMarketplaceJsonElements BrowseRequestParam { get; set; }

        [JsonProperty(PropertyName = "callsite")]
        public string CallSite { get; set; }

        [JsonProperty(PropertyName = "commerce_search_sort_by")]
        public string CommerceSearchSortBy { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int? Count { get; set; }

        [JsonProperty(PropertyName = "cursor")]
        public string Cursor { get; set; }

        [JsonProperty(PropertyName = "custom_request_params")]
        public FdMarketplaceJsonElements CustomRequestParams { get; set; }

        [JsonProperty(PropertyName = "MARKETPLACE_FEED_ITEM_IMAGE_WIDTH")]
        public int? FeedItemWidth { get; set; }

        [JsonProperty(PropertyName = "filter_location_id")]
        public string FilterLocationId { get; set; }

        [JsonProperty(PropertyName = "filter_radius_km")]
        public string FilterRadiusKm { get; set; }

        [JsonProperty(PropertyName = "params")]
        public FdMarketplaceJsonElements MarketplaceParameters { get; set; }

        [JsonProperty(PropertyName = "MERCHANT_LOGO_SCALE")]
        public string MerchantLogoScale { get; set; }

        [JsonProperty(PropertyName = "filter_price_lower_bound")]
        public string PriceLowerBound { get; set; }

        [JsonProperty(PropertyName = "filter_price_upper_bound")]
        public string PriceUpperBound { get; set; }

        [JsonProperty(PropertyName = "query")]
        public string Query { get; set; }

        [JsonProperty(PropertyName = "commerce_search_and_rp_category_id")]
        public long[] SearchRpCategoryId { get; set; }

        [JsonProperty(PropertyName = "search_vertical")]
        public string SearchVertical { get; set; }

        [JsonProperty(PropertyName = "surface")]
        public string Surface { get; set; }

        [JsonProperty(PropertyName = "VERTICALS_LEAD_GEN_PHOTO_HEIGHT_WIDTH")]
        public string VerticalPhotoWidth { get; set; }
    }


}