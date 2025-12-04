using DominatorHouseCore;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Net;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.Request
{
    public class YdRequestParameters : RequestParameters
    {
        public YdRequestParameters()
        {
            SetupDefaultHeaders();
        }

        public JsonElements Body { private get; set; }

        public JsonElements JsonElements { get; set; }

        private void SetupDefaultHeaders(CookieCollection cookie = null)
        {
            try
            {
                Headers.Clear();
                Accept =
                    "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
                AddHeader("Origin", "https://www.youtube.com");
                AddHeader("Accept-Language", "en-US,en;q=0.9");
                KeepAlive = true;
                UserAgent =
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.135 Safari/537.36";

                ContentType = Constants.ContentTypeDefault;
                if (cookie != null)
                    Cookies = cookie;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     To generate the post based on already stored elements from
        ///     <see cref="PinDominatorCore.Request.PtRequestParameters.PdPostElements" />
        /// </summary>
        /// <returns>post data in bytes of sequence</returns>
        public byte[] GetPostDataFromJson()
        {
            var jsonString = GetJsonString();

            if (string.IsNullOrEmpty(jsonString)) return null;
            return !IsMultiPart ? GeneratePostDataFromJson(jsonString) : CreateMultipartBodyFromJson(jsonString);
        }

        /// <summary>
        ///     To convert the <see cref="PinDominatorCore.Request.PtRequestParameters.PdPostElements" /> elements to string
        /// </summary>
        /// <returns>json string</returns>
        internal string GetJsonString()
        {
            return JsonElements == null
                ? null
                : JsonConvert.SerializeObject(
                    JsonElements,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
        }
    }
}