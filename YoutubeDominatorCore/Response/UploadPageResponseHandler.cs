using DominatorHouseCore.Utility;
using System.Linq;
using System.Text.RegularExpressions;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.Response
{
    public class UploadPageResponseHandler
    {
        public UploadPageResponseHandler()
        {
        }

        public UploadPageResponseHandler(string response)
        {
            var splitResponse = Regex.Split(response, "yt.setConfig");

            var jsonData1 = Utilities
                .GetBetween(
                    splitResponse.FirstOrDefault(x => x.Contains(" yt.www.upload.Uploader.initSingleInstance(")),
                    " yt.www.upload.Uploader.initSingleInstance(", ");").Trim();
            var jsonHand = new JsonHandler(jsonData1);

            ExperimentIds = jsonHand.GetElementValue("experimentIds");
            UserToken = jsonHand.GetElementValue("userToken");

            var jsonData2 = Utilities
                .GetBetween(splitResponse.FirstOrDefault(x => x.Contains("youtube_web_uploads:")), "(", ");").Trim();
            jsonHand = new JsonHandler(jsonData2);

            UploadId = jsonHand.GetElementValue("apiaryFeedbackClientIds", "web_uploads")
                .Replace("youtube_web_uploads:", "").Trim();
            FrontEndUploadIdBase = jsonHand.GetElementValue("frontendUploadIdBase");

            var jsonData3 = Utilities
                .GetBetween(splitResponse.FirstOrDefault(x => x.Contains("INNERTUBE_CONTEXT_CLIENT_VERSION:")), "(",
                    ");").Trim();
            jsonHand = new JsonHandler(jsonData3);

            HeadersElements = new HeadersElements
            {
                ClientName = jsonHand.GetElementValue("INNERTUBE_CONTEXT_CLIENT_NAME"),
                ClientVersion = jsonHand.GetElementValue("INNERTUBE_CONTEXT_CLIENT_VERSION")
            };

            var jsonData4 = Utilities
                .GetBetween(splitResponse.FirstOrDefault(x => x.Contains("'PAGE_BUILD_LABEL'")), "(", ");").Trim();
            jsonHand = new JsonHandler(jsonData4);

            HeadersElements.PageBuildLabel = jsonHand.GetElementValue("PAGE_BUILD_LABEL");
            HeadersElements.VariantsChecksum = jsonHand.GetElementValue("VARIANTS_CHECKSUM");
            HeadersElements.PageCl = jsonHand.GetElementValue("PAGE_CL");
            HeadersElements.IdToken = Utilities
                .GetBetween(splitResponse.FirstOrDefault(x => x.Contains("('ID_TOKEN',")), "\"", "\"").Trim();

            if (string.IsNullOrEmpty(HeadersElements.IdToken))
                HeadersElements.IdToken = Utilities.GetBetween(response, "{'X-YouTube-Identity-Token': \"", "\"");

            SessionToken = Utilities.GetBetween(response, "var session_token = \"", "\"");
        }

        public string UploadId { get; set; }
        public string FrontEndUploadIdBase { get; set; }
        public string UserToken { get; set; }
        public string ExperimentIds { get; set; }
        public string SessionToken { get; set; }
        public HeadersElements HeadersElements { get; set; }
    }
}