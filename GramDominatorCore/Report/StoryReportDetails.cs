using DominatorHouseCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramDominatorCore.Report
{
    public class StoryReportDetails
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public ActivityType ActivityType { get; set; } = ActivityType.StoryViewer;

        public string Query { get; set; }

        public string QueryType { get; set; }

        public DateTime Date { get; set; }

        public string InteractedUsername { get; set; }

        public string InteractedUserId { get; set; }

        public string Status { get; set; }

    }
}
