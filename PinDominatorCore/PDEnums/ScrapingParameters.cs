using System.ComponentModel;

namespace PinDominatorCore.PDEnums
{
    public enum ScrapingParameters
    {
        [Description("Users Followers")] UsersFollowers,
        [Description("Users Followings")] UsersFollowing,
        [Description("Users Followers")] Keywords,
        [Description("Follow Back")] FollowBack,
        [Description("Single User")] SingleUser,
        [Description("Users Commented")] UsersCommented,
        [Description("Users Liked")] UsersLiked
    }
}