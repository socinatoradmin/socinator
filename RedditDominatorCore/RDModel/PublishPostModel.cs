namespace RedditDominatorCore.RDModel
{
    public class PublishPostModel
    {
        public string Category { get; set; }
        public bool Nsfw { get; set; }
        public bool OriginalContent { get; set; }
        public bool Spoiler { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string ThumbnailKind { get; set; } = string.Empty;
        public string ApiType { get; set; }
        public string Title { get; set; }
        public string Kind { get; set; }
        public string SubmitType { get; set; }
        public bool Sendreplies { get; set; }
        public bool ValidateOnSubmit { get; set; }
        public string Sr { get; set; }
        public string Text { get; set; }
        public string CrosspostFullname { get; set; }
        public string PostToTwitter { get; set; }
    }
}