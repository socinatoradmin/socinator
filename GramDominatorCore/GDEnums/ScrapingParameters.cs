using System.ComponentModel;

namespace GramDominatorCore.GDEnums
{
    public enum ScrapingParameters
    {
        [Description("Hashtag")]
        Hashtag,
        //[Description("Users Followers")]
        //UsersFollowers,
        //[Description("Users Followings")]
        //UsersFollowing,
        [Description("Follow Back Followers")]
        FollowBack,
        //[Description("Single AccountUsername")]
        //SingleUser,
        //[Description("Users who commented on Media")]
        //UsersCommented,
        //[Description("Users who liked a Media")]
        //UsersLiked,
        //[Description("Recent Activity")]
        //RecentActivity,
        //[Description("Similar Users")]
        //SuggestedUsers,
        [Description("Location")]
        Location,
        //[Description("My Feed")]
        //Timeline,
    }
}
