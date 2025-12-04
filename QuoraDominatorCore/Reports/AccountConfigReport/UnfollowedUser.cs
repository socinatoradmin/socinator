using System;

namespace QuoraDominatorCore.Reports.AccountConfigReport
{
    public class UnfollowedUser
    {
        public int Id { get; set; }   
        public string Username { get; set; }
        public DateTime InteractionDate { get; set; }

        public int FollowedBack { get; set; }
    }
}