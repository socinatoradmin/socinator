using RedditDominatorCore.RDLibrary;
using System;
using System.Net;

namespace RedditDominatorCore.RDModel
{
    public class RedditUser : BaseUser
    {
        public string WhitelistStatus { get; set; }
        public int Subscribers { get; set; }
        public bool IsQuarantined { get; set; }
        public bool UserIsSubscriber { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int Wls { get; set; }
        public string Type { get; set; }
        public string CommunityIcon { get; set; }
        public string PrimaryColor { get; set; }
        public string PublicDescription { get; set; }
        public string AccountsActive { get; set; }
        public string AdvertiserCategory { get; set; }
        public bool ShowMedia { get; set; }
        public string UsingNewModmail { get; set; }
        public bool EmojisEnabled { get; set; }
        public bool OriginalContentTagEnabled { get; set; }
        public bool AllOriginalContent { get; set; }

        public string AccountIcon { get; set; }
        public int CommentKarma { get; set; }
        public DateTime Created { get; set; }
        public string DisplayName { get; set; }
        public string DisplayNamePrefixed { get; set; }
        public string DisplayText { get; set; }
        public bool HasUserProfile { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsFollowing { get; set; }
        public bool IsSubscribing { get; set; }
        public bool IsGold { get; set; }
        public bool IsMod { get; set; }
        public bool IsNsfw { get; set; }
        public bool PrefShowSnoovatar { get; set; }
        public int PostKarma { get; set; }
        public string Url { get; set; }

        public string Text { get; set; }
        public bool IsPending { get; set; }
        public string ThreadID { get; set; }
        public PaginationParameter PaginationParameter { get; set; }
        public CookieCollection CookieCollection { get; set; }
    }
}