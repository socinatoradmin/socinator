using System;

namespace LinkedDominatorCore.LDModel.ReportModel.Account
{
    public class ConnectionRequestReportModel
    {
        public int Id { get; set; }

        public string QueryType { get; set; }

        public string QueryValue { get; set; }

        public string ActivityType { get; set; }

        public string UserFullName { get; set; }

        public string UserProfileUrl { get; set; }

        public string PersonalNote { get; set; }

        public DateTime RequestedDate { get; set; }
    }
}