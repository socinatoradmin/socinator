using Newtonsoft.Json;

namespace RedditDominatorCore.RDRequest
{
    public class RdAdJsonElement
    {
        [JsonProperty(PropertyName = "id")] public string ID { get; set; }

        [JsonProperty(PropertyName = "variables")]
        public AdsVariables AdsVariables { get; set; }
    }

    public class AdsVariables
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "recentPostIds")]
        public string[] RecentPostIds { get; set; }
        [JsonProperty(PropertyName = "subredditNames")]
        public string[] SubredditNames { get; set; }

        [JsonProperty(PropertyName = "includeIdentity")]
        public bool IncludeIdentity { get; set; }

        [JsonProperty(PropertyName = "includeFeatured")]
        public bool IncludeFeatured { get; set; }

        [JsonProperty(PropertyName = "adContext")]
        public AdContext AdContext { get; set; }

        [JsonProperty(PropertyName = "sort")] public string Sort { get; set; }

        [JsonProperty(PropertyName = "range")] public string Range { get; set; }

        [JsonProperty(PropertyName = "pageSize")]
        public int PageSize { get; set; }

        [JsonProperty(PropertyName = "after")] public string After { get; set; }
        [JsonProperty(PropertyName = "includeRecents")]
        public bool IncludeRecents { get; set; }
        [JsonProperty(PropertyName = "includeTrending")]
        public bool IncludeTrending { get; set; }
        [JsonProperty(PropertyName = "includeSubredditRankings")]
        public bool IncludeSubRedditRanking { get; set; }
        [JsonProperty(PropertyName = "includeSubredditChannels")]
        public bool IncludeSubredditChannels { get; set; }
        [JsonProperty(PropertyName = "isAdHocMulti")]
        public bool IsAdHocMulti { get; set; }
        [JsonProperty(PropertyName = "isAll")]
        public bool IsAll { get; set; }
        [JsonProperty(PropertyName = "isLoggedOutGatedOptedin")]
        public bool IsLoggedOutGatedOptedin { get; set; }
        [JsonProperty(PropertyName = "isLoggedOutQuarantineOptedin")]
        public bool IsLoggedOutQuarantineOptedin { get; set; }
        [JsonProperty(PropertyName = "isPopular")]
        public bool IsPopular { get; set; }
        [JsonProperty(PropertyName = "isFake")]
        public bool IsFake { get; set; }
        [JsonProperty(PropertyName = "includeDevPlatformMetadata")]
        public bool IncludeDevPlatformMetadata { get; set; }
    }

    public class AdContext
    {
        [JsonProperty(PropertyName = "layout")]
        public string layout { get; set; }

        [JsonProperty(PropertyName = "reddaid")]
        public string Reddaid { get; set; }
    }
}