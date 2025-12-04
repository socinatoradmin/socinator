using System.Collections.Generic;

namespace GramDominatorCore.PowerAdSpy
{
    public class InstaFeedData
    {
        public List<InstaFeedTimeLineData> listUserFeedInfo { get; set; } = new List<InstaFeedTimeLineData>();
        public List<string> listid { get; set; } = new List<string>();
        public List<string> InsertedAdsId { get; set; } = new List<string>();
        public List<string> listTimestamp { get; set; } = new List<string>();
        public string maxId = string.Empty;
        public List<string> ImageList { get; set; } = new List<string>();
    }

    public class InstaFeedTimeLineData
    {
        public string label = null;
        public string ad_id = null;
        public string post_date = null;
        public string MediaType = null;
        public string code = null;
        public string image_video_url = null;
        public string post_owner_image = null;
        public string likes = null;
        public string comment = null;
        public string destination_url = null;
        public string call_to_action = null;
        public string news_feed_description = null;
        public string ad_url = null;
        public string post_owner = null;
        public string mediaId = null;
        public string ImageHeight = null;
        public string other_multimedia = null;
    }

    public class ResultFeedTimeLineData
    {
        public InstaFeedTimeLineData objFeedTimeLineData;
    }
}
