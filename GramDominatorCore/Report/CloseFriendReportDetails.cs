using System;

namespace GramDominatorCore.Report
{
    public class CloseFriendReportDetails
    {
        public int Id {  get; set; }
        public string AccountUserName {  get; set; }
        public string ActivityType {  get; set; }
        public string UserName {  get; set; }
        public bool IsCloseFriend {  get; set; }
        public DateTime InteractedDate { get; set; }
    }
}
