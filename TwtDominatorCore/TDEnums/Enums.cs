using System.ComponentModel;

namespace TwtDominatorCore.TDEnums
{
    public class Enums
    {
        public enum LoginStatus
        {
            [Description("Failed")] Failed,
            [Description("Success")] Success,
            [Description("Invalid Credentials")] InvalidCredentials,
            [Description("Verify Phone Number")] VerifyPhoneNumber,
            [Description("Verify Email")] VerifyEmail,
            [Description("Retype Phone Number")] RetypePhoneNumber,
            [Description("Retype Email")] RetypeEmail,
            [Description("Retype UserName")] RetypeUserName,
            [Description("Not Checked")] NotChecked,
            [Description("Proxy is Not Working")] ProxyisNotWorking,
            [Description("Verify Your Account")] VerifyYourAccount,
            [Description("Account Suspended")] AccountSuspended,
            [Description("Reset Password")] ResetPassword,
            [Description("Captcha")] Captcha,
            [Description("Add Phone Number")] Add_Phonenumber
        }

        public enum ModuleExtraDetails
        {
            ModulePrivateDetails = 1,
            UserProfileDetails
        }

        public enum Month
        {
            NotSet = 0,
            Jan = 01,
            Feb = 02,
            Mar = 03,
            Apr = 04,
            May = 05,
            Jun = 06,
            Jul = 07,
            Aug = 08,
            Sep = 09,
            Oct = 10,
            Nov = 11,
            Dec = 12
        }

        public enum NewUIFollowType
        {
            friends = 1,
            followers = 2
        }

        public enum ProcessType
        {
            AfterFollow = 1,
            AfterUnfollow = 2,
            AfterTweet = 3,
            AfterReposter = 4,
            AfterRetweet = 5,
            AfterDelete = 6,
            AfterLike = 7,
            AfterComment = 8,
            AfterMessage = 9,
            AfterFollowBack = 10
        }

        public enum TdMainModule
        {
            GrowFollower = 1,
            TwtBlaster = 2,
            TwtEngage = 3,
            TwtMessenger = 4,
            Scraper = 5,
            Campaign = 6
        }

        public enum TwitterElements
        {
            TweetFunction,
            UsersTweetFunction,
            None
        }
    }
}