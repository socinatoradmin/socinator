using DominatorHouseCore.Interfaces;
using RedditDominatorCore.RDLibrary;

namespace RedditDominatorCore.RDModel
{
    public class SubRedditModel : IPage
    {
        public string WhitelistStatus { get; set; }
        public bool IsNsfw { get; set; }
        public int Subscribers { get; set; }
        public string PrimaryColor { get; set; }
        public string Id { get; set; }
        public bool IsQuarantined { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public int Wls { get; set; }
        public string DisplayText { get; set; }
        public string Type { get; set; }
        public string CommunityIcon { get; set; }
        public string PublicDescription { get; set; }
        public Allowedposttypes AllowedPostTypes { get; set; }
        public bool UserIsSubscriber { get; set; }
        public string AccountsActive { get; set; }
        public string AdvertiserCategory { get; set; }
        public bool ShowMedia { get; set; }
        public string UsingNewModmail { get; set; }
        public bool EmojisEnabled { get; set; }
        public bool OriginalContentTagEnabled { get; set; }
        public bool AllOriginalContent { get; set; }
        public PaginationParameter PaginationParameter { get; set; }
        public string FanPageID { get; set; }
        public string FanPageName { get; set; }
        public string FanPageUrl { get; set; }
    }

    public class Allowedposttypes
    {
        public bool Images { get; set; }
        public bool Text { get; set; }
        public bool Videos { get; set; }
        public bool Links { get; set; }
        public bool Spoilers { get; set; }
    }
}