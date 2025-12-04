using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;
using YoutubeDominatorCore.YDEnums;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.Request
{
    public class SetHeaders
    {
        private const string Accept1 = "*/*";

        private const string Accept2 =
            "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";

        public static void SetWebHeadersBeforeClick(IYdHttpHelper httpHelper, YoutubeAct actType,
            HeadersElements elements = null)
        {
            try
            {
                httpHelper.GetRequestParameter().Headers.Clear();
                var ydRp = httpHelper.GetRequestParameter();

                ydRp.Headers["Origin"] = "https://www.youtube.com";
                ydRp.Headers["Accept-Language"] = "en-US,en;q=0.9";

                switch (actType)
                {
                    case YoutubeAct.CreateChannel1:
                    case YoutubeAct.SearchAnything:
                    case YoutubeAct.HitHomePage:
                    case YoutubeAct.UploadStep1:
                    case YoutubeAct.GetSubscribedChannels:
                    case YoutubeAct.ScrapePostsFromAChannel:
                        SetHeaders1(ref ydRp);
                        break;

                    case YoutubeAct.CreateChannel2:
                        SetHeaders2(ref ydRp, elements, false);
                        break;

                    case YoutubeAct.Comment:
                    case YoutubeAct.CommentAsReply:
                        SetHeaders5(ref ydRp, elements);
                        break;
                    case YoutubeAct.GetSubscribedChannelsAtPagination:
                        SetHeaders2(ref ydRp, elements, needHeaderSet3: true);
                        break;
                    case YoutubeAct.Subscribe:
                    case YoutubeAct.ScrapePostsFromAChannelInPagination:
                        SetHeaders2(ref ydRp, elements, needHeaderSet3: true, needHeaderSet4: true);
                        break;

                    case YoutubeAct.Like:
                    case YoutubeAct.Dislike:
                    case YoutubeAct.LikeComment:
                    case YoutubeAct.ScrapeComments:
                    case YoutubeAct.SearchAnythingInPagination:
                    case YoutubeAct.Report:
                        SetHeaders2(ref ydRp, elements, needHeaderSet4: true);
                        break;

                    case YoutubeAct.UploadStep2:
                        SetHeaders3(ref ydRp, elements);
                        break;

                    case YoutubeAct.UploadStep4:
                        SetHeaders4(ref ydRp);
                        break;

                    case YoutubeAct.UploadStep5:
                        ydRp.ContentType = Constants.ContentTypeDefault;
                        SetHeaders2(ref ydRp, elements, false, true);
                        break;
                }

                httpHelper.SetRequestParameter(ydRp);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private static void SetHeaders1(ref IRequestParameters ydRp)
        {
            ydRp.Referer = "https://www.youtube.com/";
            ydRp.Accept = Accept2;
            ydRp.AddHeader("Upgrade-Insecure-Requests", "1");
            //ydRp.AddHeader("dpr", "1");
            ydRp.AddHeader("X-Client-Data",
                "CIW2yQEIprbJAQjBtskBCKmdygEIlqzKAQiZtcoBCKvHygEI9cfKAQjnyMoBCOnIygEItMvKAQi8y8oBCNvVygEI2dfKAQ==");
            ydRp.AddHeader("Sec-Fetch-Site", "none");
            ydRp.AddHeader("Sec-Fetch-Mode", "navigate");
            ydRp.AddHeader("Sec-Fetch-User", "?1");
            ydRp.AddHeader("Sec-Fetch-Dest", "document");
        }

        private static void SetHeaders2(ref IRequestParameters ydRp, HeadersElements elements,
            bool needHeaderSet2 = true, bool needHeaderSet3 = false, bool needHeaderSet4 = false)
        {
            ydRp.Accept = Accept1;
            ydRp.AddHeader("X-Client-Data",
                "CIW2yQEIprbJAQjBtskBCKmdygEIlqzKAQiZtcoBCKvHygEI9cfKAQjnyMoBCOnIygEItMvKAQi8y8oBCNvVygEI2dfKAQ==");

            if (elements == null)
                return;

            ydRp.Referer = string.IsNullOrEmpty(elements.RefererUrl) ? "https://www.youtube.com/" : elements.RefererUrl;

            HeadersSet1(ref ydRp, elements);
            if (needHeaderSet2)
                HeadersSet2(ref ydRp, elements);
            if (needHeaderSet3)
                HeadersSet3(ref ydRp);
            if (needHeaderSet4)
                HeadersSet4(ref ydRp, elements);

            HeadersSet5(ref ydRp, elements);
        }

        private static void SetHeaders4(ref IRequestParameters ydRp)
        {
            try
            {
                ydRp.Referer = "https://www.youtube.com/";
                ydRp.ContentType = "application/json";
                ydRp.Accept = Accept1;
                ydRp.AddHeader("X-Client-Data", "CIW2yQEIprbJAQjBtskBCKmdygEIlqzKAQiZtcoBCKvHygEI9cfKAQjnyMoBCOnIygEItMvKAQi8y8oBCNvVygEI2dfKAQ==");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private static void SetHeaders3(ref IRequestParameters ydRp, HeadersElements elements)
        {
            try
            {
                ydRp.Referer = "https://www.youtube.com/";
                ydRp.Accept = Accept1;
                ydRp.AddHeader("X-GUploader-Client-Info", "mechanism=scotty xhr resumable; clientVersion=190833997");
                ydRp.AddHeader("X-Goog-AuthUser", "0");
                ydRp.AddHeader("X-Client-Data", "CIW2yQEIprbJAQjBtskBCKmdygEIlqzKAQiZtcoBCKvHygEI9cfKAQjnyMoBCOnIygEItMvKAQi8y8oBCNvVygEI2dfKAQ==");
                if (!string.IsNullOrEmpty(elements?.ChannelPageId ?? ""))
                    ydRp.AddHeader("X-Goog-PageId", elements?.ChannelPageId);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private static void SetHeaders5(ref IRequestParameters ydRp, HeadersElements elements)
        {
            try
            {
                ydRp.Referer = string.IsNullOrEmpty(elements.RefererUrl) ? "https://www.youtube.com/" : elements.RefererUrl;
                ydRp.Accept = Accept1;
                ydRp.AddHeader("X-Origin", "https://www.youtube.com");
                ydRp.AddHeader("X-Goog-Visitor-Id", elements.XGoogVisitorId);
                ydRp.AddHeader("X-Goog-AuthUser", "0");
                ydRp.AddHeader("X-Client-Data", "CIW2yQEIprbJAQjBtskBCKmdygEIlqzKAQiZtcoBCKvHygEI9cfKAQjnyMoBCOnIygEItMvKAQi8y8oBCNvVygEI2dfKAQ==");
                ydRp.AddHeader("Sec-Fetch-Site", "same-origin");
                ydRp.AddHeader("Sec-Fetch-Mode", "same-origin");
                ydRp.AddHeader("Sec-Fetch-Dest", "empty");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private static void HeadersSet1(ref IRequestParameters ydRp, HeadersElements elements)
        {
            ydRp.AddHeader("X-YouTube-Variants-Checksum", elements.VariantsChecksum);
            ydRp.AddHeader("X-YouTube-Page-CL", elements.PageCl);
            ydRp.AddHeader("X-YouTube-Utc-Offset", "330");
        }

        private static void HeadersSet2(ref IRequestParameters ydRp, HeadersElements elements)
        {
            ydRp.AddHeader("X-YouTube-Client-Name", elements.ClientName);
            ydRp.AddHeader("X-YouTube-Client-Version", elements.ClientVersion);
            ydRp.AddHeader("X-Youtube-Identity-Token", elements.IdToken);
        }

        private static void HeadersSet3(ref IRequestParameters ydRp)
        {
            ydRp.AddHeader("X-SPF-Referer", ydRp.Referer);
            ydRp.AddHeader("X-SPF-Previous", ydRp.Referer);
        }

        private static void HeadersSet4(ref IRequestParameters ydRp, HeadersElements elements)
        {
            ydRp.AddHeader("X-YouTube-Page-Label", elements.PageBuildLabel);
        }

        private static void HeadersSet5(ref IRequestParameters ydRp, HeadersElements elements)
        {
            ydRp.ContentType = "application/x-www-form-urlencoded";
            ydRp.AddHeader("X-YouTube-Device", elements.XYouTubeDevice);
            ydRp.AddHeader("Sec-Fetch-Site", "same-origin");
            ydRp.AddHeader("Sec-Fetch-Mode", "cors");
            ydRp.AddHeader("Sec-Fetch-Dest", "empty");
        }
    }
}