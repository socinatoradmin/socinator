using System.ComponentModel;

namespace FaceDominatorCore.FDEnums
{
    public enum GroupMemberCategory
    {
        [Description("All Members")]
        AllMembers = 0,

        [Description("Local Members")]
        LocalMembers = 1,

        [Description("Admins And Moderators")]
        AdminsAndModerators = 2,

        [Description("Members With Things In Common")]
        MembersWithThingsInCommon = 3,

        [Description("Friends")]
        Friends = 4

    }
}
