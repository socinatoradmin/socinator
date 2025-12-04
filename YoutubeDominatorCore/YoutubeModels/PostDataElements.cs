namespace YoutubeDominatorCore.YoutubeModels
{
    /// <summary>
    ///     Necessary elements scraped from a web page for creating post data
    /// </summary>
    public class PostDataElements
    {
        public string TrackingParams { get; set; }
        public string ClickTrackingParams { get; set; }
        public string Params { get; set; }
        public string UploadButtonClickTracking { get; set; }
        public string XsrfToken { get; set; }
        public string CreateCommentParams { get; set; }
        public string CreateReplyParams { get; set; }
        public string Csn { get; set; }
        public string ContinuationToken { get; set; }
        public string Itct { get; set; }
        public string AddInUrl { get; set; }
        public string LikeParams { get; set; }
        public string DislikeParams { get; set; }
        public string ReportFromEndPointParams { get; set; }
        public string FlagAction { get; set; }
        public string FlagRequestType { get; set; }
        public string InnertubeApiKey { get; set; }

        public string InnertubeContext { get; set; }
    }
}