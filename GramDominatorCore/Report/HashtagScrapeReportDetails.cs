using System;
using DominatorHouseCore.Enums;

namespace GramDominatorCore.Report
{
    public class HashtagScrapeReportDetails
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public ActivityType ActivityType { get; set; } = ActivityType.HashtagsScraper;

        public string Keyword { get; set; }

        public string HashtagName { get; set; }

        public string HashtagId { get; set; }

        public string MediaCount { get; set; }

        public DateTime Date { get; set; }
    }
}
