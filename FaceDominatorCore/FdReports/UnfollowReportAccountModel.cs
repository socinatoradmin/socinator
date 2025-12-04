using System;

namespace FaceDominatorCore.FdReports
{
    public class UnfollowReportAccountModel
    {
        public int Id { get; set; }
        public string ActivityType { get; set; }
        public string UserId { get; set; }
        public string UserProfileUrl { get; set; }
        public string UserName { get; set; }
        public DateTime InteractionTimeStamp { get; set; }
        public DateTime ConnectedDate { get; set; }
        public DateTime RequestedDate { get; set; }
    }
}
