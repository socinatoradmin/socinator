namespace PinDominatorCore.Report
{
    public class InteractedUsersReportDetails
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Query { get; set; }
        public string QueryType { get; set; }
        public int FollowedBack { get; set; }
        public int FollowedBackDate { get; set; }
        public string InteractionTime { get; set; }
        public string ActivityType { get; set; }
        public string Username { get; set; }
        public string InteractedUsername { get; set; }
        public string InteractedUserId { get; set; }
        public int UpdatedTime { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingsCount { get; set; }
        public int PinsCount { get; set; }
        public int TriesCount { get; set; }
        public string FullName { get; set; }
        public bool? HasAnonymousProfilePicture { get; set; }
        public string ProfilePicUrl { get; set; }
        public string Website { get; set; }
        public string Bio { get; set; }
        public string DirectMessage { get; set; }
        public string BoardDescription { get; set; }
        public string BoardUrl { get; set; }
        public string BoardName { get; set; }
        public string SinAccUsername { get; set; }
        public string PinId { get; set; }
        public string Status { get; set; }
    }
}