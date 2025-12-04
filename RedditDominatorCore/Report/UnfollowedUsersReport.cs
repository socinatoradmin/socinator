namespace RedditDominatorCore.Report
{
    public class UnfollowedUsersReport
    {
        public int Id { get; set; }


        public int FollowedBack { get; set; }


        public int InteractionDate { get; set; }


        public int OperationType { get; set; }


        public string Username { get; set; }

        public string UserId { get; set; }

        public string FullName { get; set; }
    }
}