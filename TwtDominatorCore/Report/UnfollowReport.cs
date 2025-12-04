using System;

namespace TwtDominatorCore.Report
{
    public class UnfollowReport
    {
        public int SlNo { get; set; }

        public string SinAccUsername { get; set; }


        public string UnfollowSource { get; set; }


        public string SourceType { get; set; }


        public string UserName { get; set; }

        public string UserId { get; set; }

        public string FollowBackStatus { get; set; }

        public DateTime UnfollowedDate { get; set; }
    }
}