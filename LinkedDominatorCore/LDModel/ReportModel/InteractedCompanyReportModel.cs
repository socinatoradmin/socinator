using System;

namespace LinkedDominatorCore.LDModel.ReportModel
{
    public class InteractedCompanyReportModel
    {
        public int Id { get; set; }

        public string AccountEmail { get; set; }

        public string QueryType { get; set; }

        public string QueryValue { get; set; }

        public string ActivityType { get; set; }

        public string CompanyName { get; set; }

        public string CompanyUrl { get; set; }
        public string CompanyLogoUrl { get; set; }

        public string TotalEmployees { get; set; }

        public string Industry { get; set; }
        public string IsFollowed { get; set; }

        public string DetailedCompanyScraperInfo { get; set; }

        public DateTime InteractionDateTime { get; set; }
    }
}