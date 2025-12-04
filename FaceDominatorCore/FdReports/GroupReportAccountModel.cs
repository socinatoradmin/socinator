using System;

namespace FaceDominatorCore.FdReports
{
    public class GroupReportAccountModel
    {
        public int Id { get; set; }


        public string QueryType
        { get; set; }


        public string QueryValue { get; set; }


        public string ActivityType { get; set; }


        public string GroupName { get; set; }


        public string GroupUrl { get; set; }


        public string TotalMembers { get; set; }


        public string GroupType { get; set; }

        public string MembershipStatus { get; set; }


        public DateTime InteractionTimeStamp { get; set; }
    }
}
