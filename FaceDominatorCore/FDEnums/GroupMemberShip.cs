using System.ComponentModel;

namespace FaceDominatorCore.FDEnums
{
    public enum GroupMemberShip
    {
        [Description("Any group")]
        AnyGroup = 1,

        [Description("Friends groups")]
        FriendsGroups = 2,

        [Description("My groups")]
        MyGroups = 3,

        /*
                [Description("Skip Joined Groups")]
                SkipJoinedGroups = 4
        */
    }
}
