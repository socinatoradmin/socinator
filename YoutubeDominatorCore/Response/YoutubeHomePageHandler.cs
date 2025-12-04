using DominatorHouseCore;
using DominatorHouseCore.Utility;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using YoutubeDominatorCore.YDUtility;

namespace YoutubeDominatorCore.Response
{
    public class YoutubeHomePageHandler
    {
        public YoutubeHomePageHandler(string response)
        {
            try
            {
                if (string.IsNullOrEmpty(response))
                    return;

                var jSonString = Utilities.GetBetween(response, "var ytInitialGuideData =", "if (window['").Trim()
                    .TrimEnd(';');
                if (string.IsNullOrEmpty(jSonString))
                    jSonString = Utilities.GetBetween(response, "var ytInitialData = ", ";</script>").Trim()
                        .TrimEnd(';');
                if (string.IsNullOrEmpty(jSonString))
                    jSonString = "{" + Utilities.GetBetween(response, "window[\"ytInitialData\"] = {", "};").Trim() + "}";

                YdStatic.ExtractCommonJsonData(ref jSonString);
                if (!string.IsNullOrEmpty(jSonString.Trim()))
                {
                    var jsonHand = new JsonHandler(jSonString);

                    var isLoggedInJToken = jsonHand.GetJToken("responseContext", "serviceTrackingParams", 0, "params");

                    if (jsonHand.GetJTokenValue(isLoggedInJToken, 0, "key") == "creator_channel_id")
                        ChannelId = jsonHand.GetJTokenValue(isLoggedInJToken, 0, "value");

                    if (jsonHand.GetJTokenValue(isLoggedInJToken, 1, "key") == "logged_in")
                        IsLoggedIn = jsonHand.GetJTokenValue(isLoggedInJToken, 1, "value") == "1";
                    else
                        IsLoggedIn = jsonHand.GetJTokenValue(isLoggedInJToken, 0, "value") != "0";

                    ChannelUsername = jsonHand.GetElementValue("responseContext", "webResponseContextExtensionData",
                        "ytConfigData", "delegatedSessionId");
                }
                else
                {
                    var splitResponse = Regex.Split(response, "yt.setConfig[(]");
                    jSonString = splitResponse.FirstOrDefault(x => x.StartsWith("{'PAGE_NAME': \"index\",'LOGGED_IN':"))
                        ?.Trim().TrimEnd(';').Trim(')');

                    var jsonHand = new JsonHandler(jSonString);

                    ChannelId = jsonHand.GetElementValue("GUIDED_HELP_PARAMS", "creator_channel_id");

                    IsLoggedIn = jsonHand.GetElementValue("GUIDED_HELP_PARAMS", "logged_in") == "1";

                    ChannelUsername =
                        jsonHand.GetElementValue(
                            "delegatedSessionId"); // still finding the element to get ChannelPageID from this response// Pending
                }
                if (string.IsNullOrEmpty(ChannelId)) ChannelId = Utilities.GetBetween(response, "href=\"/channel/", "\"");
                IsCurrentChannelDefault = string.IsNullOrEmpty(ChannelUsername);
                if (IsCurrentChannelDefault)
                    ChannelUsername = "Default Channel";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public bool IsLoggedIn { get; set; }
        public string ChannelId { get; set; }
        public string ChannelUsername { get; set; }
        public bool IsCurrentChannelDefault { get; set; }
    }
}