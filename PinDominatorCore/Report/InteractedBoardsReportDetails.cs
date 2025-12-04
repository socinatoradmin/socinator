using DominatorHouseCore.Enums;

namespace PinDominatorCore.Report
{
    public class InteractedBoardsReportDetails
    {
        public int Id { get; set; }
        public string BoardId { get; set; }
        public string BoardName { get; set; }
        public string BoardUrl { get; set; }
        public string BoardDescription { get; set; }
        public int PinCount { get; set; }
        public int FollowerCount { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public string Query { get; set; }
        public string QueryType { get; set; }
        public string InteractionDate { get; set; }
        public ActivityType OperationType { get; set; }
        public string SinAccId { get; set; }
        public string SinAccUsername { get; set; }
        public string BoardSection { get; set; }
    }
}