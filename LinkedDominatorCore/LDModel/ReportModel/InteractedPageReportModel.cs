using System;

namespace LinkedDominatorCore.LDModel.ReportModel
{
    public class InteractedPageReportModel
    {
        public int Id { get; set; }
        public string AccountEmail { get; set; }
        public string QueryType { get; set; }
        public string QueryValue { get; set; }
        public string ActivityType { get; set; }
        public string PageName { get; set; }
        public string PageUrl { get; set; }
        public string FollowerCount { get; set; }
        public string TotalEmployees { get; set; }
        public DateTime InteractionDateTime { get; set; }
    }
}