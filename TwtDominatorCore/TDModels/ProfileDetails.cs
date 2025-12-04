using System;

namespace TwtDominatorCore.TDModels
{
    public class ProfileDetails
    {
        public string UserId {  get; set; }
        public string Id { get; set; }
        public bool HasGraduated {  get; set; }
        public bool IsVerified {  get; set; }
        public string ProfileImageShape {  get; set; }
        public bool SmartBlockedBy {  get; set; }
        public bool SmartBlocking {  get; set; }
        public bool IsProfileTranslatable {  get; set; }
        public bool VerifiedPhoneStatus {  get; set; }
        public bool HasHiddenLikesOnProfile {  get; set; }
        public bool HasHiddenSubscriptionOnProfile {  get; set; }
        public BirthDayDetails birthDayDetails { get; set; }
        public bool IsIdentityVerified {  get; set; }
        public bool CanHighlightTweet {  get; set; }
        public int HighlightTweetCount {  get; set; }
        public int UserSeedTweetCount {  get; set; }
        public bool CanDm {  get; set; }
        public bool CanMediaTag {  get; set; }
        public DateTime Created { get; set; }
        public bool DefaultProfile {  get; set; }
        public bool DefaultProfileImage {  get; set; }
        public string ProfileDescription {  get; set; }
        public int FastFollowerCount {  get; set; }
        public int LikedCount { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public bool HasCustomTimeLines {  get; set; }
        public bool IsTranslator {  get; set; }
        public int ListedCount { get; set; }
        public string Location {  get; set; }
        public int MediaCount {  get; set; }
        public string Name {  get; set; }
        public string UserName {  get; set; }
        public bool NeedPhoneVerification {  get; set; }
        public string ProfilePicUrl {  get; set; }
        public bool PossiblySensitive {  get; set; }
        public string WebSiteUrl {  get; set; }
        public bool Mute { get; set; }
    }
    public class BirthDayDetails
    {
        public string visibility { get; set; }
        public string YearVisibility {  get; set; }
        public int Day {  get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
