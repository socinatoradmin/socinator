using DominatorHouseCore.Enums;

namespace PinDominatorCore.Report
{
    public class UnfollowedUsersReportDetails
    {
        public int Id { get; set; }
        public int FollowedBack { get; set; }
        public string InteractionDate { get; set; }
        public ActivityType OperationType { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string SinAccId { get; set; }
        public string SinAccUsername { get; set; }
    }
}