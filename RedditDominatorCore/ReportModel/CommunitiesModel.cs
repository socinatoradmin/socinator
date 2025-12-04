namespace RedditDominatorCore.ReportModel
{
    public class CommunitiesModel
    {
        public string WhitelistStatus { get; set; }
        public bool IsNsfw { get; set; }
        public int Subscribers { get; set; }
        public string PrimaryColor { get; set; }
        public string CommunityId { get; set; }
        public bool IsQuarantined { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string DisplayText { get; set; }
        public string Type { get; set; }
        public string CommunityIcon { get; set; }
        public bool IsOwn { get; set; }
    }
}