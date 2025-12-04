using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.Response
{
    public class ScrapeAdsResponseHandler : QuoraResponseHandler
    {
        public List<AdScraperModel> LstOfAds = new List<AdScraperModel>();
        private readonly JsonJArrayHandler handler = JsonJArrayHandler.GetInstance;
        public ScrapeAdsResponseHandler(IResponseParameter response,PaginationParameter paginationParameter ,bool IsPagination=false) : base(response)
        {
            try
            {
                if (IsPagination)
                {
                    var jsonObject = handler.ParseJsonToJObject(response.Response);
                    var pageInfo = handler.GetJTokenOfJToken(jsonObject, "data", "multifeedObject", "multifeedConnection", "pageInfo");
                    bool.TryParse(handler.GetJTokenValue(pageInfo, "hasNextPage"), out bool hasNextPage);
                    var quoraId = handler.GetJTokenValue(jsonObject, "data", "viewer", "user", "uid");
                    paginationParameter.HasNextPage = hasNextPage;
                    paginationParameter.PaginationID = handler.GetJTokenValue(pageInfo, "endCursor");
                    var DataNodes = handler.GetJArrayElement(handler.GetJTokenValue(jsonObject, "data", "multifeedObject", "multifeedConnection", "edges"));
                    if(DataNodes!=null && DataNodes.HasValues)
                    {
                        int.TryParse(paginationParameter.EndCursorPosition, out int previousPosition);
                        paginationParameter.EndCursorPosition = (previousPosition + DataNodes.Count).ToString();
                        //Getting Details of Each Ads.
                        foreach(var Node in DataNodes)
                        {
                            try
                            {
                                var TypeName = handler.GetJTokenValue(Node, "node", "__typename");
                                if (string.IsNullOrEmpty(TypeName) || !TypeName.Contains("AdBundle"))
                                    continue;
                                AdScraperModel model = new AdScraperModel();
                                var AdData = handler.GetJTokenOfJToken(Node, "node", "stories",0,"ad");
                                var type = handler.GetJTokenValue(AdData, "__typename");
                                model.AdTitle = handler.GetJTokenValue(AdData, "title");
                                model.PostOwnerImage = handler.GetJTokenValue(AdData, "logoImageUrl");
                                model.PostOwnerImage = string.IsNullOrEmpty(model.PostOwnerImage) ?handler.GetJTokenValue(AdData, "logo", "imageUrl") : model.PostOwnerImage;
                                model.PostOwner = handler.GetJTokenValue(AdData, "businessName");
                                model.CallToAction = handler.GetJTokenValue(AdData, "ctaString");
                                model.NewsFeedDescription = handler.GetJTokenValue(AdData, "contentText");
                                model.AdId = handler.GetJTokenValue(AdData, "adId");
                                model.TimeStamp = QdConstants.GetTimeStamp;
                                model.DestinationUrl = handler.GetJTokenValue(AdData, "urlTemplate");
                                model.ImageOrVideoUrl = handler.GetJTokenValue(AdData, "imageUrl");
                                model.Upvote = handler.GetJTokenValue(AdData, "numUpvotes");
                                model.quoraId = quoraId;
                                LstOfAds.Add(model);
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }
                    }
                }
                else
                {
                    var decodedResponse = QdUtilities.GetDecodedResponse(response.Response);
                    var endCursorString = Utilities.GetBetween(decodedResponse, "\"MultifeedConnection\",\"pageInfo\"", "edges\"");
                    paginationParameter.PaginationID = Utilities.GetBetween(endCursorString, "\"endCursor\":\"", "\"");
                    paginationParameter.EndCursorPosition = "6";
                }
                #region Old Code to scrape Ads From Browser Response.
                //if (response.Response.Contains("upper_content_group"))
                //{
                //    //FeedStory HyperLinkFeedStory feed_item
                //    var document = new HtmlDocument();
                //    document.LoadHtml(decodedResponse);
                //    var adsNodes = (document.DocumentNode.SelectNodes("//div[@class='upper_content_group']") ??
                //                    document.DocumentNode.SelectNodes("//div[@class='Bundle HyperLinkBundle']"))
                //        .ToArray();
                //    var adScrapeModel = new AdScraperModel();
                //    var count = 1;

                //    foreach (var singleAd in adsNodes)
                //        try
                //        {
                //            adScrapeModel.AdTitle =
                //                Utilities.GetBetween(singleAd.InnerHtml, "content_title'>", "</div>");
                //            adScrapeModel.DestinationUrl =
                //                Utilities.GetBetween(singleAd.InnerHtml, "upper_content_link' href='", "'");
                //            adScrapeModel.PostOwner =
                //                Utilities.GetBetween(singleAd.InnerHtml, "advertiser_endpoint'>", "<");

                //            if (string.IsNullOrEmpty(adScrapeModel.PostOwner))
                //                try
                //                {
                //                    var test = document.DocumentNode
                //                        .SelectNodes("//p[@class='advertiser_endpoint']").ToArray();
                //                    var cout = test.Length;
                //                    if (test.Length == cout)
                //                        foreach (var name in test.Skip(cout - 1))
                //                            adScrapeModel.PostOwner = name.InnerText;
                //                    else
                //                        adScrapeModel.PostOwner = test[0].InnerText;
                //                }
                //                catch (Exception ec)
                //                {
                //                    ec.DebugLog();
                //                }

                //            adScrapeModel.PostOwnerImage = Utilities.GetBetween(singleAd.OuterHtml,
                //                "account_logo_img' style='background-image:url(&quot;", "&quot");


                //            if (string.IsNullOrEmpty(adScrapeModel.PostOwnerImage))
                //            {
                //                var postOwnerImage = document.DocumentNode
                //                    .SelectSingleNode("//div[@class='account_logo_img']").OuterHtml;
                //                adScrapeModel.PostOwnerImage =
                //                    Utilities.GetBetween(postOwnerImage, "url(\"", "\")'></div>");
                //            }

                //            if (string.IsNullOrEmpty(adScrapeModel.PostOwnerImage))
                //            {
                //            }

                //            adScrapeModel.NewsFeedDescription =
                //                Utilities.GetBetween(singleAd.InnerHtml, "content_summary'>", "</div>");

                //            adScrapeModel.ImageOrVideoUrl = string.IsNullOrEmpty(
                //                Utilities.GetBetween(singleAd.InnerHtml,
                //                    "u-bg--16-9' style='background-image:url(&quot;", "&quot"))
                //                ? Utilities.GetBetween(singleAd.OuterHtml,
                //                    "featured_image' style='background-image:url(\"", ")")
                //                : Utilities.GetBetween(singleAd.InnerHtml,
                //                    "u-bg--16-9' style='background-image:url(&quot;", "&quot");

                //            adScrapeModel.TimeStamp = DateTime.Now.GetCurrentEpochTimeSeconds().ToString();

                //            //var imageUrl = document.DocumentNode.SelectSingleNode("//div[@class='featured_image full_width_image_ad u-margin-bottom--sm u-bg--16-9']").OuterHtml;

                //            if (string.IsNullOrEmpty(adScrapeModel.ImageOrVideoUrl)) //
                //            {
                //                var imageUrl = document.DocumentNode
                //                    .SelectSingleNode(
                //                        "//div[@class='featured_image full_width_image_ad u-margin-bottom--sm u-bg--16-9']")
                //                    .OuterHtml;
                //                adScrapeModel.ImageOrVideoUrl =
                //                    Utilities.GetBetween(imageUrl, "url(\"", "\");'></div>");
                //            }

                //            adScrapeModel.CallToAction = document.DocumentNode
                //                .SelectSingleNode("//a[@class='content_button']").InnerText;

                //            if (string.IsNullOrEmpty(adScrapeModel.ImageOrVideoUrl))
                //            {
                //            }

                //            LstOfAds.Add(adScrapeModel);
                //        }
                //        catch (Exception ex)
                //        {
                //            ex.DebugLog();
                //        }
                //}
                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}