using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;

namespace Tumblr
{
    public class PostDataElement
    {


        [JsonProperty("determine_email")]
        public string DetermineEmail { get; set; }


        [JsonProperty("user%5Bemail%5D")]
        public string UserEmail { get; set; }


        [JsonProperty("user%5Bpassword%5D")]
        public string UserPassword { get; set; }


        [JsonProperty("tumblelog%5Bname%5D")]
        public string TumblelogName { get; set; }


        [JsonProperty("user%5Bage%5D")]
        public string UserAge { get; set; }


        [JsonProperty("context")]
        public string Context { get; set; }


        [JsonProperty("version")]
        public string Version { get; set; }


        [JsonProperty("follow")]
        public string Follow { get; set; }


        [JsonProperty("http_referer")]
        public string HttpReferer { get; set; }



        [JsonProperty("form_key")]
        public string FormKey { get; set; }



        [JsonProperty("seen_suggestion")]
        public string SeenSuggestion { get; set; }


        [JsonProperty("used_suggestion")]
        public string UsedSuggestion { get; set; }


        [JsonProperty("used_auto_suggestion")]
        public string UsedAutoSuggestion { get; set; }


        [JsonProperty("about_tumblr_slide")]
        public string AboutTumblrSlide { get; set; }



        [JsonProperty("random_username_suggestions")]
        public string RandomUsernameSuggestions { get; set; }


        [JsonProperty("action")]
        public string Action { get; set; }

    }
}
