using System.ComponentModel;

namespace GramDominatorCore.GDEnums
{
   public class Enums
    {
        public enum GdMainModule
        {
            GrowFollower = 1,
            Poster = 2,
            Chat = 3,
            LikeComment = 4,
            Scraper = 5,
            Campaign = 6,
            InstaChat = 7,
            StoryViewer=8
        }


        //public enum LoginStatus
        //{
        //    [Description("Success")]
        //    Success,
        //    [Description("Failed")]
        //    Failed,
        //    [Description("Invalid Credentials")]
        //    InvalidCredentials,
        //    [Description("Verify")]
        //    Verify,
        //    [Description("Verify via Phone Number")]
        //    VerifyviaPhoneNumber,
        //    [Description("Verify via Email")]
        //    VerifyviaEmail,
        //    [Description("Verify via Phone Number or Email")]
        //    VerifyviaPhoneNumberorEmail,
        //    [Description("Not Checked")]
        //    NotChecked,
        //    [Description("Proxy is Not Working")]
        //    ProxyisNotWorking,
        //    [Description("Verify Your Account")]
        //    VerifyYourAccount
        //}




        //public enum PostLocation
        //{
        //    Timeline,
        //    Story,
        //    [Description("Direct Story")]
        //    DirectStory
        //}




        //public enum InstagramError
        //{
        //    CheckPoint,
        //    FailedRequest,
        //    ToManyActions,
        //    SettingsCritical,
        //    FollowSettingsMinor,
        //    RateLimit,
        //    IncorrectPassword,
        //    NotAuthorized,
        //    UsernameNotExist,
        //    NotFound,
        //    SentryBlocked,
        //    LoginRequired,
        //    PasswordReset,
        //    AccountDisabled,
        //    Challenge,
        //    ScrapingMinor,
        //    CanNotLike,
        //    InputError,
        //    Feedback,
        //    TwoFactor
        //}

       

        public enum UserQueryParameters
        {
            [Description("LangKeyKeywords")]
            Keywords,
            //[Description("LangKeyOwnFollowers")]
            //OwnFollowers,
            //[Description("LangKeyOwnFollowersPosts")]
            //OwnFollowersPost,
            //[Description("LangKeyOwnFollowings")]
            //OwnFollowing,
            //[Description("LangKeyOwnFollowingsPosts")]
            //OwnFollowingsPost,
            //[Description("LangKeySuggestedUsers")]
            //SuggestedUsers,
            //[Description("LangKeySuggestedUsersPosts")]
            //SuggestedUsersPosts,
            //[Description("LangKeyHashtagUsers")]
            //HashtagUsers,
            //[Description("LangKeyHashtagPostS")]
            //HashtagPost,
            //[Description("LangKeyHashtagUsersPostS")]
            //HashtagUsersPost,
            //[Description("LangKeySomeonesFollowers")]
            //SomeonesFollowers,
            //[Description("LangKeySomeonesFollowersPostS")]
            //SomeonesFollowersPost,
            //[Description("LangKeySomeonesFollowings")]
            //SomeonesFollowings,
            //[Description("LangKeySomeonesFollowingsPostS")]
            //SomeonesFollowingsPost,
            //[Description("LangKeyFollowersOfSomeonesFollowers")]
            //FollowersOfFollowers,
            //[Description("LangKeyFollowersOfSomeonesFollowersPostS")]
            //FollowersOfFollowersPost,
            //[Description("LangKeyFollowersOfSomeonesFollowings")]
            //FollowersOfFollowings,
            //[Description("LangKeyFollowersOfSomeonesFollowingsPostS")]
            //FollowersOfFollowingsPost,
            //[Description("LangKeyLocationUsers")]
            //LocationUsers,
            //[Description("LangKeyLocationPosts")]
            //LocationPosts,
            //[Description("LangKeyLocationUsersPosts")]
            //LocationUsersPost,
            //[Description("LangKeyCustomUsers")]
            //CustomUsers,
            //[Description("LangKeyCustomPhotos")]
            //CustomPhotos,
            ////[Description("LangKeyEngagedUsers")]
            ////EngagedUsers,
            //[Description("LangKeyUsersWhoLikedPosts")]
            //UsersWhoLikedPost,
            //[Description("LangKeyPostsOfUsersWhoLikedPost")]
            //PostOfUsersWhoLikedPost,
            //[Description("LangKeyUsersWhoCommentedOnPosts")]
            //UsersWhoCommentedOnPost,
            //[Description("LangKeyPostsOfUsersWhoCommentedOnPost")]
            //PostOfUsersWhoCommentedOnPost
        }

        public enum WhitelistblacklistType
        {
            Private,
            Group
        }
    }
}
