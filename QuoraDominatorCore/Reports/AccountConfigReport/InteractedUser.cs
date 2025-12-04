using System;

namespace QuoraDominatorCore.Reports.AccountConfigReport
{
    public class InteractedUser
    {
        public int Id { get; set; }

        public string Query { get; set; }

        public string QueryType { get; set; }

        public int FollowedBack { get; set; }

        public DateTime Date { get; set; }

        public string Username { get; set; }

        public string InteractedUsername { get; set; }
    }
}