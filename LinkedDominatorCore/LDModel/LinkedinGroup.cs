using DominatorHouseCore.Interfaces;

namespace LinkedDominatorCore.LDModel
{
    public class LinkedinGroup : IGroup
    {
        public LinkedinGroup()
        {
        }

        public LinkedinGroup(string groupId)
        {
            GroupId = groupId;
            GroupUrl = $"https://www.linkedin.com/groups/{groupId}";
        }

        public string TotalMembers { get; set; }
        public string CommunityType { get; set; }
        public string MembershipStatus { get; set; }

        public string GroupName { get; set; }
        public string GroupId { get; set; }
        public string GroupUrl { get; set; }
    }
}