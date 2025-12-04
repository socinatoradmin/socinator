using Newtonsoft.Json;

namespace YoutubeDominatorCore.YoutubeModels
{
    public class JsonElements
    {
        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        [JsonProperty(PropertyName = "clickTrackingParams")]
        public string ClickTrackingParams { get; set; }

        [JsonProperty(PropertyName = "commandMetadata")]
        public JsonElements CommandMetadata { get; set; }

        [JsonProperty(PropertyName = "webCommandMetadata")]
        public JsonElements WebCommandMetadata { get; set; }

        [JsonProperty(PropertyName = "url")] public string Url { get; set; }

        [JsonProperty(PropertyName = "sendPost")]
        public bool? SendPost { get; set; }

        [JsonProperty(PropertyName = "performCommentActionEndpoint")]
        public JsonElements PerformCommentActionEndpoint { get; set; }

        [JsonProperty(PropertyName = "clientActions")]
        public JsonElements[] ClientActions { get; set; }

        [JsonProperty(PropertyName = "voteCount")]
        public JsonElements VoteCount { get; set; }

        [JsonProperty(PropertyName = "accessibility")]
        public JsonElements Accessibility { get; set; }

        [JsonProperty(PropertyName = "accessibilityData")]
        public JsonElements AccessibilityData { get; set; }

        [JsonProperty(PropertyName = "label")] public string Label { get; set; }

        [JsonProperty(PropertyName = "simpleText")]
        public string SimpleText { get; set; }

        [JsonProperty(PropertyName = "voteStatus")]
        public string VoteStatus { get; set; }

        [JsonProperty(PropertyName = "sej")] public JsonElements Sej { get; set; }

        [JsonProperty(PropertyName = "csn")] public string Csn { get; set; }

        [JsonProperty(PropertyName = "session_token")]
        public string SessionToken { get; set; }
    }
}