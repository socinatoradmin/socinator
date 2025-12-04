using Newtonsoft.Json;

namespace RedditDominatorCore.RDRequest
{
    public class RdJsonElement
    {
        [JsonProperty(PropertyName = "id")] public string Id { get; set; }

        [JsonProperty(PropertyName = "dir")] public string Dir { get; set; }

        [JsonProperty(PropertyName = "api_type")]
        public string ApiType { get; set; }

        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        [JsonProperty(PropertyName = "sr_name")]
        public string SrName { get; set; }

        [JsonProperty(PropertyName = "richtext_json")]
        public RdJsonElement RichtextJson { get; set; }

        [JsonProperty(PropertyName = "document")]
        public RdJsonElement[] Document { get; set; }

        [JsonProperty(PropertyName = "e")] public string E { get; set; }

        [JsonProperty(PropertyName = "c")] public RdJsonElement[] C { get; set; }

        [JsonProperty(PropertyName = "t")] public string T { get; set; }

        [JsonProperty(PropertyName = "text")] public string Text { get; set; }

        [JsonProperty(PropertyName = "thing_id")]
        public string ThingId { get; set; }

        [JsonProperty(PropertyName = "return_rtjson", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool ReturnRtjson { get; set; }

        [JsonProperty(PropertyName = "url")] public string FetchTitleUrl { get; set; }

        [JsonProperty(PropertyName = "accept")]
        public string Accept { get; set; }
    }

    public class RdPostJsonElements
    {
        [JsonProperty(PropertyName = "post_category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "nsfw")] public bool Nsfw { get; set; }

        [JsonProperty(PropertyName = "original_content")]
        public bool OriginalCcontent { get; set; }

        [JsonProperty(PropertyName = "spoiler")]
        public bool Spoiler { get; set; }

        [JsonProperty(PropertyName = "url")] public string Url { get; set; }

        [JsonProperty(PropertyName = "api_type")]
        public string ApiType { get; set; }

        [JsonProperty(PropertyName = "title")] public string Title { get; set; }

        [JsonProperty(PropertyName = "kind")] public string Kind { get; set; }

        [JsonProperty(PropertyName = "submit_type")]
        public string SubmitType { get; set; }

        [JsonProperty(PropertyName = "sendreplies")]
        public bool Sendreplies { get; set; }

        [JsonProperty(PropertyName = "validate_on_submit")]
        public bool ValidateOnSubmit { get; set; }

        [JsonProperty(PropertyName = "sr")] public string Sr { get; set; }

        [JsonProperty(PropertyName = "text")] public string Text { get; set; }

        [JsonProperty(PropertyName = "crosspost_fullname")]
        public string CrossPostId { get; set; }

        [JsonProperty(PropertyName = "video_poster_url")]
        public string VideoPosterURL { get; set; }

        [JsonProperty(PropertyName = "richtext_json")]
        public string RichTextJson { get; set; }

        [JsonProperty(PropertyName = "post_to_twitter")]
        public string PostToTwitter { get; set; }

        [JsonProperty(PropertyName = "show_error_list")]
        public string ShowErrorList { get; set; }
    }

    public class RdPostMediaUploadJsonElements
    {
        [JsonProperty(PropertyName = "filepath")]
        public string FilePath { get; set; }

        [JsonProperty(PropertyName = "mimetype")]
        public string FileType { get; set; }
    }

    public class RdLoginJsonElements
    {
        [JsonProperty(PropertyName = "csrf_token")]
        public string CsrfToken { get; set; }

        [JsonProperty(PropertyName = "otp")] public string OTP { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "dest")] public string Destination { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }
    }
}