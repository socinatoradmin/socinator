using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramDominatorCore.Report
{
    public class FollowReportDto
    {
        public string AccountName { get; set; }

        public string FollowedUserName { get; set; }

        public DateTime ActionDateTime { get; set; } = DateTime.Now;
    }
}
