using DominatorHouseCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramDominatorCore.Report
{
    public class LikeCommentReportDetails
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public ActivityType ActivityType { get; set; } = ActivityType.LikeComment;

        public string LikedMediaCode { get; set; }

        public string LikedMediaOwner { get; set; }

        public string MediaType { get; set; }

        public string Comments { get; set; }

        public DateTime Date { get; set; }

        public string status { get; set; }
    }
}
