using System;

namespace FaceDominatorCore.FdReports
{
    public class UnfollowReportModel
    {
        public int Id { get; set; }
        public string AccountEmail { get; set; }
        public string ActivityType { get; set; }
        public string UserId { get; set; }
        public string UserProfileUrl { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public string DetailedUserInfo { get; set; }
        public DateTime InteractionTimeStamp { get; set; }
        public DateTime RequestedDate { get; set; }
    }
}
