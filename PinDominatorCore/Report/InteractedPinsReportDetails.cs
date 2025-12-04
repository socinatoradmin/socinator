using DominatorHouseCore.Enums;

namespace PinDominatorCore.Report
{
    public class InteractedPinsReportDetails
    {
        public int Id { get; set; }
        public string OriginalPinId { get; set; }
        public string GeneratedPinId { get; set; }
        public string MediaString { get; set; }
        public string PinDescription { get; set; }
        public int TryCount { get; set; }
        public int CommentCount { get; set; }
        public double PinnedTimeStamp { get; set; }
        public double VideoDuration { get; set; }
        public string SourceBoard { get; set; }
        public string PinWebUrl { get; set; }
        public string SourceBoardName { get; set; }
        public string DestinationBoard { get; set; }
        public string InteractionDate { get; set; }
        public MediaType MediaType { get; set; }
        public string OperationType { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Query { get; set; }
        public string QueryType { get; set; }
        public string SinAccId { get; set; }
        public string SinAccUsername { get; set; }
        public string BoardLabel { get; set; }
        public string Comment { get; set; }
        public string CommentId { get; set; }
        public string Status { get; set; }
        public string PinTitle { get; set; }
    }
}