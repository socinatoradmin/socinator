using DominatorHouseCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramDominatorCore.Report
{
   public  class ReplyCommentReportDetails
    {
        public int Id { get; set; }

        public string AccountUsername { get; set; }

        public ActivityType ActivityType { get; set; } = ActivityType.ReplyToComment;

        public string MediaType { get; set; }

        public string Comments { get; set; }

        public DateTime Date { get; set; }

        public string CommentOwnerName { get; set; }

        public string MediaCode { get; set; }

        public string status { get; set; }
        
    }
}
