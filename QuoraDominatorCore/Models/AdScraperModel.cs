namespace QuoraDominatorCore.Models
{
    public class AdScraperModel
    {
        public string AdTitle { set; get; }
        public string PostOwner { set; get; }
        public string ImageOrVideoUrl { set; get; }
        public string PostOwnerImage { set; get; }
        public string NewsFeedDescription { set; get; }
        public string DestinationUrl { set; get; }
        public string CallToAction { set; get; }
        public string AdId { set; get; }
        public string TimeStamp { set; get; }
        public string Upvote { get; set; }
        public string quoraId { get; set; }
    }
}