using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using System;
using System.Linq;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response
{
    public class ProfileDetailsResponseHandler:TdResponseHandler
    {
        public ProfileDetails UserProfileDetails { get; set; }
        public ProfileDetailsResponseHandler(IResponseParameter responseParameter):base(responseParameter)
        {
            try
            {
                var jObject = handler.ParseJsonToJObject(responseParameter.Response);
                var Details = handler.GetJTokenOfJToken(jObject, "data", "user","result");
                var Profile = handler.GetJTokenOfJToken(Details, "legacy");
                bool.TryParse(handler.GetJTokenValue(Details, "has_graduated_access"), out bool hasGraduated);
                bool.TryParse(handler.GetJTokenValue(Details, "is_blue_verified"), out bool isVerified);
                bool.TryParse(handler.GetJTokenValue(Details, "smart_blocked_by"), out bool smart_blocked_by);
                bool.TryParse(handler.GetJTokenValue(Details, "smart_blocking"), out bool smart_blocking);
                bool.TryParse(handler.GetJTokenValue(Details, "is_profile_translatable"), out bool is_profile_translatable);
                bool.TryParse(handler.GetJTokenValue(Details, "verified_phone_status"), out bool verified_phone_status);
                bool.TryParse(handler.GetJTokenValue(Details, "has_hidden_likes_on_profile"), out bool has_hidden_likes_on_profile);
                bool.TryParse(handler.GetJTokenValue(Details, "has_hidden_subscriptions_on_profile"), out bool has_hidden_subscriptions);
                int.TryParse(handler.GetJTokenValue(Details, "legacy_extended_profile", "birthdate", "day"), out int day);
                int.TryParse(handler.GetJTokenValue(Details, "legacy_extended_profile", "birthdate", "month"), out int month);
                int.TryParse(handler.GetJTokenValue(Details, "legacy_extended_profile", "birthdate", "year"), out int year);
                bool.TryParse(handler.GetJTokenValue(Details, "verification_info", "is_identity_verified"), out bool is_identity_verified);
                bool.TryParse(handler.GetJTokenValue(Details, "highlights_info", "can_highlight_tweets"), out bool can_highlight_tweets);
                int.TryParse(handler.GetJTokenValue(Details, "highlights_info", "highlighted_tweets"), out int highlighted_tweets);
                int.TryParse(handler.GetJTokenValue(Details, "user_seed_tweet_count"), out int user_seed_tweet_count);
                bool.TryParse(handler.GetJTokenValue(Profile, "can_dm"), out bool can_dm);
                bool.TryParse(handler.GetJTokenValue(Profile, "can_media_tag"), out bool can_media_tag);
                bool.TryParse(handler.GetJTokenValue(Profile, "default_profile"), out bool default_profile);
                bool.TryParse(handler.GetJTokenValue(Profile, "default_profile_image"), out bool default_profile_image);
                int.TryParse(handler.GetJTokenValue(Profile, "fast_followers_count"), out int fast_followers_count);
                int.TryParse(handler.GetJTokenValue(Profile, "favourites_count"), out int favourites_count);
                int.TryParse(handler.GetJTokenValue(Profile, "followers_count"), out int followers_count);
                int.TryParse(handler.GetJTokenValue(Profile, "friends_count"), out int friends_count);
                bool.TryParse(handler.GetJTokenValue(Profile, "has_custom_timelines"), out bool has_custom_timelines);
                bool.TryParse(handler.GetJTokenValue(Profile, "is_translator"), out bool is_translator);
                int.TryParse(handler.GetJTokenValue(Profile, "listed_count"), out int listed_count);
                bool.TryParse(handler.GetJTokenValue(Profile, "needs_phone_verification"), out bool needs_phone_verification);
                bool.TryParse(handler.GetJTokenValue(Profile, "possibly_sensitive"), out bool possibly_sensitive);
                int.TryParse(handler.GetJTokenValue(Profile, "media_count"), out int media_count);
                bool.TryParse(handler.GetJTokenValue(Profile, "muting"), out bool IsMuting);
                var website = string.Empty;
                var urls = handler.GetJArrayElement(handler.GetJTokenValue(Profile, "entities", "url", "urls"));
                if (urls != null && urls.HasValues)
                    website = handler.GetJTokenValue(urls?.FirstOrDefault(), "expanded_url");
                UserProfileDetails = new ProfileDetails
                {
                    WebSiteUrl = website,
                    Mute = IsMuting,
                    UserId = handler.GetJTokenValue(Details, "rest_id"),
                    Id = handler.GetJTokenValue(Details, "id"),
                    HasGraduated = hasGraduated,
                    IsVerified = isVerified,
                    ProfileImageShape = handler.GetJTokenValue(Details, "profile_image_shape"),
                    SmartBlockedBy = smart_blocked_by,
                    SmartBlocking = smart_blocking,
                    IsProfileTranslatable = is_profile_translatable,
                    VerifiedPhoneStatus = verified_phone_status,
                    HasHiddenLikesOnProfile = has_hidden_likes_on_profile,
                    HasHiddenSubscriptionOnProfile = has_hidden_subscriptions,
                    birthDayDetails = new BirthDayDetails
                    {
                        visibility = handler.GetJTokenValue(Details, "legacy_extended_profile", "birthdate", "visibility"),
                        YearVisibility = handler.GetJTokenValue(Details, "legacy_extended_profile", "birthdate", "year_visibility"),
                        Day = day,
                        Month = month,
                        Year = year
                    },
                    IsIdentityVerified = is_identity_verified,
                    CanHighlightTweet = can_highlight_tweets,
                    HighlightTweetCount = highlighted_tweets,
                    UserSeedTweetCount = user_seed_tweet_count,
                    CanDm = can_dm,
                    CanMediaTag = can_media_tag,
                    Created = TdUtility.GetDateTime(handler.GetJTokenValue(Profile, "created_at")),
                    DefaultProfile = default_profile,
                    DefaultProfileImage = default_profile_image,
                    ProfileDescription = handler.GetJTokenValue(Profile, "description"),
                    FastFollowerCount = fast_followers_count,
                    LikedCount = favourites_count,
                    FollowerCount = followers_count,
                    FollowingCount = friends_count,
                    HasCustomTimeLines = has_custom_timelines,
                    IsTranslator = is_translator,
                    ListedCount = listed_count,
                    Location = handler.GetJTokenValue(Profile, "location"),
                    MediaCount = media_count,
                    Name = handler.GetJTokenValue(Profile,"name"),
                    NeedPhoneVerification = needs_phone_verification,
                    UserName = handler.GetJTokenValue(Profile, "screen_name"),
                    ProfilePicUrl = handler.GetJTokenValue(Profile, "profile_image_url_https"),
                    PossiblySensitive = possibly_sensitive
                };
            }catch (Exception ex) { ex.DebugLog(); }
        }
    }
}
