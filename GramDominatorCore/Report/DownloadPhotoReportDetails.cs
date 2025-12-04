using System;
using DominatorHouseCore.Enums;

namespace GramDominatorCore.Report
{
    public class DownloadPhotoReportDetails
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public ActivityType ActivityType { get; set; } = ActivityType.PostScraper;

        public string ScrapedMediaOwnerUsername { get; set; }

        public string ScrapedMediaCode { get; set; }

        public string MediaType { get; set; }

        public DateTime Date { get; set; }

        public string CommentCount { get; set; }
        public string LikeCount { get; set; }

        public string Location { get; set; }

        public DateTime PostDate { get; set; }

        public string PostUrl { get; set; }

    }
}
