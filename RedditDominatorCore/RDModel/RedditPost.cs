using DominatorHouseCore.Interfaces;
using RedditDominatorCore.Interface;
using RedditDominatorCore.RDLibrary;

namespace RedditDominatorCore.RDModel
{
    public class RedditPost : IPost, IPostScraper
    {
        public bool IsCrosspostable { get; set; }
        public bool IsStickied { get; set; }
        public SubRedditModel SubReddit { get; set; } = new SubRedditModel();
        public RedditUser User { get; set; } = new RedditUser();
        public string DomainOverride { get; set; }
        public string CallToAction { get; set; }
        public object[] EventsOnRender { get; set; }
        public bool Saved { get; set; }
        public int NumComments { get; set; }
        public string UpvoteRatio { get; set; }
        public string Author { get; set; }
        public Media Media { get; set; }
        public int NumCrossposts { get; set; }
        public bool IsSponsored { get; set; }
        public string ContentCategories { get; set; }
        public string Source { get; set; }
        public bool IsLocked { get; set; }
        public int Score { get; set; }
        public bool IsArchived { get; set; }
        public bool Hidden { get; set; }
        public string Preview { get; set; }
        public Thumbnail Thumbnail { get; set; }
        public Belongsto BelongsTo { get; set; }
        public bool IsRoadblock { get; set; }
        public string CrosspostRootId { get; set; }
        public string CrosspostParentId { get; set; }
        public bool SendReplies { get; set; }
        public int GoldCount { get; set; }
        public bool IsSpoiler { get; set; }
        public bool IsNsfw { get; set; }
        public bool IsMediaOnly { get; set; }
        public string PostId { get; set; }
        public string SuggestedSort { get; set; }
        public bool IsBlank { get; set; }
        public int ViewCount { get; set; }
        public string Permalink { get; set; }
        public long Created { get; set; }
        public string Title { get; set; }
        public object[] Events { get; set; }
        public bool IsOriginalContent { get; set; }
        public string DistinguishType { get; set; }
        public int VoteState { get; set; }
        public object[] Flair { get; set; }
        public string Commenttext { get; set; }
        public string OldComment { get; set; }
        public PaginationParameter PaginationParameter { get; set; }
        public string TypeName { get; set; }
        public string MediaType { get; set; }
        public string MediaResolution { get; set; }
        public string DominForAd { get; set; }
        public string PostVideoUrl { get; set; }
        public string Id { get; set; }
        public string Caption { get; set; }
        public string Code { get; set; }
        public string VideoUrl { get; set; }
        public bool Upvoted { get; set; }
        public bool Downvoted { get; set; }
        public string FlairText { get; set; }
    }

    public class Media
    {
        public object Obfuscated { get; set; }
        public string Content { get; set; }
        public int Width { get; set; }
        public Resolution[] Resolutions { get; set; }
        public string Type { get; set; }
        public int Height { get; set; }
    }

    public class Resolution
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Thumbnail
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Belongsto
    {
        public string Type { get; set; }
        public string Id { get; set; }
    }
}