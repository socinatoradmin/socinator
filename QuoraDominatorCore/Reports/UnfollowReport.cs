using System;

namespace QuoraDominatorCore.Reports
{
    public class UnfollowReport
    {
        public int Id { get; set; }

        public string AccountName { get; set; }

        public int FollowedBack { get; set; }
        public DateTime Date { get; set; }

        public string Username { get; set; }
    }
}