namespace RedditDominatorCore.RDModel
{
    public class RedditPostFlairInfo
    {
        public string Id { get; set; } = string.Empty;
        public string FlairTitle { get; set; } = string.Empty;
        public bool IsTextEditable { get; set; } = false;
        public string Type { get; set; } = string.Empty;
        public bool IsModOnly { get; set; } = false;
        public int MaxEmojis { get; set; } = 0;
        public string AllowableContent { get; set; } = string.Empty;
        public bool IsFlairEnabled { get; set; } = false;
        public bool CanAssignOwn { get; set; } = false;
    }
}
