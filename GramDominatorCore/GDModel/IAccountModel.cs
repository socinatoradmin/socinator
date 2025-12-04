using GramDominatorCore.Settings;

namespace GramDominatorCore.GDModel
{
    public interface IAccountModel
    {
        AccountGroup AccountGroup { get; set; }

        //string AccountHasAnonymousProfile { get; set; }
        //SocialNetworks AccountNetwork { get; set; }
        //Proxy AccountProxy { get; set; }
        //ActivityManager ActivityManager { get; set; }
        //DeviceGenerator DeviceDetails { get; set; }
        int FollowersCount { get; set; }
        //int FollowingCount { get; set; }
        //HttpHelper HttpHelper { get; set; }
        //bool IsAccountSelected { get; set; }
        //bool IsPrivateUser { get; set; }
        //bool IsUserLoggedIn { get; set; }
        //bool IsVerifiedUser { get; set; }
        //int LastAnalyticsUpdate { get; set; }
        //int LastLogin { get; set; }
        string Password { get; set; }
        int PostsCount { get; set; }
        //string ProfilePictureUrl { get; set; }
        //bool SelectedGroup { get; set; }
        int RowNo { get; set; }
       // string SessionId { get; set; }
        string Status { get; set; }
      //  string UserFullName { get; set; }
        string UserId { get; set; }
        string UserName { get; set; }
    }
}