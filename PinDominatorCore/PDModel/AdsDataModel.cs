using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinDominatorCore.PDModel
{
    public class AdsDataModel
    {
        public string ad_id { get; set; } = string.Empty;
        public string ad_title { get; set; } = string.Empty;
        public string destination_url { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public string[] ad_video { get; set; }
        public string[] ad_image { get; set; }
        public string ad_text { get; set; } = string.Empty;
        public string[] post_owner_image { get; set; }
        public string post_owner { get; set; } = string.Empty;
        public int adcrawled { get; set; } = 3;
        public string network { get; set; } = "Pinterest";
        public int platform { get; set; } = 12;
        public string version { get; set; } = "1.0.1";
        public string ad_position { get; set; } = "FEED";
        public string target_keyword { get; set; } = string.Empty;
        public string ad_sub_position { get; set; } = string.Empty;
        public string newsfeed_description { get; set; } = string.Empty;
        public int target_keyword_id { get; set; }
        public string source { get; set; } = "desktop";
        public string city { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public string country { get; set; } = string.Empty;
        public string ip_address { get; set; } = string.Empty;

    }
}
