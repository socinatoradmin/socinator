using System;

namespace LinkedDominatorCore.LDModel.ReportModel
{
    public class InteractedJobsReportModel
    {
        public int Id { get; set; }
        public string AccountEmail { get; set; }
        public string QueryType { get; set; }
        public string QueryValue { get; set; }
        public string ActivityType { get; set; }
        public string JobTitle { get; set; }
        public string JobPostUrl { get; set; }
        public string DetailedInfo { get; set; }
        public DateTime InteractedDateTime { get; set; }
    }
}