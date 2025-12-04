using Newtonsoft.Json;
using System.Collections.Generic;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    public class ScrapedLinkDetails
    {
        [JsonProperty(PropertyName = "canonical")]
        public string CanonocalLink { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "final")]
        public string FinalLink { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "user")]
        public string UserLink { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "hmac")]
        public string Hmac { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "iframe")]
        public string ImageLink { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "title")]
        public string[] ImageArray { get; set; }

        [JsonProperty(PropertyName = "images_sorted_by_dom")]
        public string Url { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "ranked_images")]
        public string ExternalImage { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "lsd")]
        public string Summary { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "lsd")]
        public string Title { get; set; } = string.Empty;



        [JsonProperty(PropertyName = "lsd")]
        public string GlobalShareId { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "lsd")]
        public string UrlScrapeId { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "lsd")]
        public string RankingModelVersion { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "log")]
        public Dictionary<string, string> LogDict { get; set; } = new Dictionary<string, string>();


        [JsonProperty(PropertyName = "Favicon")]
        public string FaviconLink { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "specified_og")]
        public string SpecifiedOg { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "medium")]
        public string Medium { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "amp_url")]
        public string AmpUrl { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "locale")]
        public string Locale { get; set; } = string.Empty;
    }
}
