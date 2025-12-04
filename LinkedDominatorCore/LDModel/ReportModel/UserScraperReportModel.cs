using System;

namespace LinkedDominatorCore.LDModel.ReportModel
{
    public class UserScraperReportModel
    {
        public int Id { get; set; }

        public string AccountEmail { get; set; }

        public string QueryType { get; set; }

        public string QueryValue { get; set; }

        public string ActivityType { get; set; }

        public string UserFullName { get; set; }

        public string UserProfileUrl { get; set; }

        public string DetailedUserInfo { get; set; }

        public DateTime ScrapedDate { get; set; }
    }
}