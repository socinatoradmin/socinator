#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Models.SocioPublisher.Settings;
using DominatorHouseCore.Patterns;
using DominatorHouseCore.Request;
using HtmlAgilityPack;

#endregion

namespace DominatorHouseCore.Utility
{
    public class RssFeedUtilities
    {
        public async Task RssFeedFetchMethod(string feedUrl, string feedTemplate, PostDetailsModel postDetailsModel,
            string campaignId, CancellationTokenSource cancellationTokenSource, int maximumPostLimitToStore,
            string campaignName)
        {
            try
            {
                // Get all Campaign Details
                var campaignDetails = PostlistFileManager.GetAll(campaignId);

                // Get Rss Campaign Details
                var postdetails = campaignDetails.Where(x => x.PostSource == PostSource.RssFeedPost)
                    .Select(x => x.ShareUrl).ToList();

                //// Requesting Rss feed urls


                var httpHelper = new SocialHttpHelper();
                var requestParameter = new RequestParameters
                {
                    Headers =
                    {
                        ["User-Agent"] =
                            "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36"
                    },
                    Cookies = new CookieCollection()
                };

                httpHelper.SetRequestParameter(requestParameter);

                var pageSource = await httpHelper.GetRequestAsync(feedUrl, cancellationTokenSource.Token);

                // ReSharper disable once RedundantAssignment
                var postlists = new List<PublisherPostlistModel>();

                var htmlDoc = new HtmlDocument();

                var htmlResponse = WebUtility.HtmlDecode(pageSource.Response);

                htmlDoc.LoadHtml(htmlResponse);

                DateTime? expireDate = null;
                // Calculate Expire date of the post
                if (postDetailsModel.PublisherPostSettings.GeneralPostSettings.IsExpireDate)
                    expireDate = postDetailsModel.PublisherPostSettings.GeneralPostSettings.ExpireDate;


                var postItems = htmlDoc.DocumentNode.Descendants("item");

                if (!postItems.Any())
                {
                    postItems = htmlDoc.DocumentNode.Descendants("entry");

                    // Scrape the posts from Http Response

                    #region Http Response

                    postlists = (from node in postItems
                                 let InnerHtml = node.InnerHtml
                                 let title = RemoveCdata(node.Element("title").InnerHtml)
                                 let content = WebUtility.HtmlDecode(RemoveCdata(node.Element("content").InnerHtml))
                                 let descriptionDetails =
                                     Regex.Matches(content, "(<p|<p>)(.*?)(<h2|<div)", RegexOptions.Singleline)
                                 let descriptionDetailsNew = descriptionDetails == null || descriptionDetails.Count == 0
                                     ? null
                                     : Regex.Matches(content, "(<p|<p>)(.*)(</p>)", RegexOptions.Singleline)
                                 let description = descriptionDetailsNew == null || descriptionDetailsNew.Count == 0
                                     ? string.Empty
                                     : descriptionDetailsNew[0].Groups[0].ToString()
                                 let link = RemoveCdata(node.Element("link").InnerText)
                                 let newLink = string.IsNullOrEmpty(link)
                                     ? Utilities.GetBetween(node.Element("link").OuterHtml, "href=\"", "\"")
                                     : link
                                 let pubDate = RemoveCdata(node.Element("published").InnerHtml)
                                 let descriptionUrl = RemoveCdata(node.Element("description").InnerHtml)
                                 let Imageurl = !descriptionUrl.Contains("src")
                                     ? ""
                                     : Utilities.GetBetween(descriptionUrl, "src=\"", "\"")
                                 where !postdetails.Contains(link)
                                 select new PublisherPostlistModel
                                 {
                                     MediaList = new ObservableCollection<string>(postDetailsModel.MediaViewer.MediaList),
                                     CampaignId = campaignId,
                                     CreatedTime = DateTime.Now,
                                     ExpiredTime = expireDate,
                                     PostId = Utilities.GetGuid(),
                                     PostCategory = PostCategory.OrdinaryPost,
                                     PostQueuedStatus = PostQueuedStatus.Pending,
                                     PostRunningStatus = PostRunningStatus.Active,
                                     PostSource = PostSource.RssFeedPost,
                                     PostDescription = WebUtility.HtmlDecode(feedTemplate.Replace("[FeedTitle]", title)
                                         .Replace("[FeedDescription]", HtmlParseUtility.GetAllInnerTextFromTags(description))
                                         .Replace("[FeedUrl]", newLink)
                                         .Replace("[FeedPublishedDate]", pubDate)),
                                     ShareUrl = newLink,
                                     PdSourceUrl = string.IsNullOrWhiteSpace(postDetailsModel.PdSourceUrl) ? link : postDetailsModel.PdSourceUrl.Replace("[FeedUrl]", link),
                                     PublisherInstagramTitle =
                                         string.IsNullOrWhiteSpace(postDetailsModel.PublisherInstagramTitle) ? title : postDetailsModel.PublisherInstagramTitle.Replace("[FeedTitle]", title),
                                     GeneralPostSettings = postDetailsModel.PublisherPostSettings.GeneralPostSettings,
                                     FdPostSettings = postDetailsModel.PublisherPostSettings.FdPostSettings,
                                     GdPostSettings = postDetailsModel.PublisherPostSettings.GdPostSettings,
                                     TdPostSettings = new TdPostSettings
                                     {
                                         IsDeletePostAfterHours = postDetailsModel.PublisherPostSettings.TdPostSettings
                                             .IsDeletePostAfterHours,
                                         IsMentionUser = postDetailsModel.PublisherPostSettings.TdPostSettings.IsMentionUser,
                                         DeletePostAfterHours = postDetailsModel.PublisherPostSettings.TdPostSettings
                                             .DeletePostAfterHours,
                                         MentionUserList = postDetailsModel.PublisherPostSettings.TdPostSettings.MentionUserList,
                                         RssImageList = new List<string> { Imageurl }
                                     },
                                     LdPostSettings = postDetailsModel.PublisherPostSettings.LdPostSettings,
                                     TumblrPostSettings = postDetailsModel.PublisherPostSettings.TumblrPostSettings,
                                     YTPostSettings = postDetailsModel.PublisherPostSettings.YTPostSettings,
                                     RedditPostSetting = postDetailsModel.PublisherPostSettings.RedditPostSetting,
                                     FdSellLocation = postDetailsModel.FdSellLocation,
                                     FdSellPrice = postDetailsModel.FdSellPrice,
                                     FdSellProductTitle = postDetailsModel.FdSellProductTitle,
                                     IsFdSellPost = postDetailsModel.IsFdSellPost,
                                     PublisherPostSettings = postDetailsModel.PublisherPostSettings,
                                     FdCondition = postDetailsModel.FdCondition
                                 }).ToList();

                    #endregion
                }
                else
                {
                    // Scrape the posts from Http Response

                    #region Http Response

                    postlists = (from node in postItems
                                 let InnerHtml = node.InnerHtml
                                 let title = RemoveCdata(node.Element("title").InnerHtml)
                                 let description = RemoveCdata(node?.Element("description")?.InnerText ?? "")
                                     .Replace("(notitle)", "")
                                 let link = RemoveCdata(node?.Element("link")?.NextSibling?.InnerText ?? "")
                                 let pubDate = RemoveCdata(node?.Element("pubdate")?.InnerHtml ?? "")
                                 let descriptionUrl = RemoveCdata(node?.Element("description")?.InnerHtml ?? "")
                                 let Imageurl = !descriptionUrl?.Contains("src") ?? false
                                     ? !node.InnerHtml?.Contains("src") ?? false ? "" :
                                     Utilities.GetBetween(node.InnerHtml, "src=\"", "\"")
                                     : Utilities.GetBetween(descriptionUrl, "src=\"", "\"")
                                 let url = RemoveCdata(node?.Element("url")?.InnerHtml ?? "")
                                 where !postdetails.Contains(link)
                                 select new PublisherPostlistModel
                                 {
                                     MediaList = new ObservableCollection<string>(postDetailsModel.MediaViewer.MediaList)
                                {string.IsNullOrEmpty(Imageurl)?Utilities.GetBetween(node.InnerHtml,"url=\"","\""):Imageurl},
                                     CampaignId = campaignId,
                                     CreatedTime = DateTime.Now,
                                     ExpiredTime = expireDate,
                                     PostId = Utilities.GetGuid(),
                                     PostCategory = PostCategory.OrdinaryPost,
                                     PostQueuedStatus = PostQueuedStatus.Pending,
                                     PostRunningStatus = PostRunningStatus.Active,
                                     PostSource = PostSource.RssFeedPost,
                                     PostDescription = WebUtility.HtmlDecode(feedTemplate.Replace("[FeedTitle]", title)
                                         .Replace("[FeedDescription]", description)
                                         .Replace("[FeedUrl]", link)
                                         .Replace("[FeedPublishedDate]", pubDate)),
                                     ShareUrl = link,
                                     PdSourceUrl = string.IsNullOrWhiteSpace(postDetailsModel.PdSourceUrl) ? link : postDetailsModel.PdSourceUrl.Replace("[FeedUrl]", link),
                                     PublisherInstagramTitle =
                                         string.IsNullOrWhiteSpace(postDetailsModel.PublisherInstagramTitle) ? title : postDetailsModel.PublisherInstagramTitle.Replace("[FeedTitle]", title),
                                     GeneralPostSettings = postDetailsModel.PublisherPostSettings.GeneralPostSettings,
                                     FdPostSettings = postDetailsModel.PublisherPostSettings.FdPostSettings,
                                     GdPostSettings = postDetailsModel.PublisherPostSettings.GdPostSettings,
                                     TdPostSettings = new TdPostSettings
                                     {
                                         IsDeletePostAfterHours = postDetailsModel.PublisherPostSettings.TdPostSettings
                                             .IsDeletePostAfterHours,
                                         IsMentionUser = postDetailsModel.PublisherPostSettings.TdPostSettings.IsMentionUser,
                                         DeletePostAfterHours = postDetailsModel.PublisherPostSettings.TdPostSettings
                                             .DeletePostAfterHours,
                                         MentionUserList = postDetailsModel.PublisherPostSettings.TdPostSettings.MentionUserList,
                                         RssImageList = new List<string> { Imageurl }
                                     },
                                     LdPostSettings = postDetailsModel.PublisherPostSettings.LdPostSettings,
                                     TumblrPostSettings = postDetailsModel.PublisherPostSettings.TumblrPostSettings,
                                     YTPostSettings = postDetailsModel.PublisherPostSettings.YTPostSettings,
                                     RedditPostSetting = postDetailsModel.PublisherPostSettings.RedditPostSetting,
                                     FdSellLocation = postDetailsModel.FdSellLocation,
                                     FdSellPrice = postDetailsModel.FdSellPrice,
                                     FdSellProductTitle = postDetailsModel.FdSellProductTitle,
                                     IsFdSellPost = postDetailsModel.IsFdSellPost,
                                     PublisherPostSettings = postDetailsModel.PublisherPostSettings,
                                     FdCondition = postDetailsModel.FdCondition
                                 }).ToList();

                    #endregion
                }


                // If Readd Checked, then readd the same posts in selected/given times

                #region Readd

                if (postDetailsModel.PublisherPostSettings.GeneralPostSettings.IsReaddCount)
                {
                    var duplicatedPostlist = new List<PublisherPostlistModel>();

                    foreach (var post in postlists)
                        try
                        {
                            for (var readdIndex = 1;
                                readdIndex < postDetailsModel.PublisherPostSettings.GeneralPostSettings.ReaddCount;
                                readdIndex++)
                            {
                                var newPost = post.DeepClone();
                                newPost.PostId = Utilities.GetGuid();
                                duplicatedPostlist.Add(newPost);
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                    postlists.AddRange(duplicatedPostlist);
                }

                #endregion

                // Check whether cancellation token arised or not
                cancellationTokenSource.Token.ThrowIfCancellationRequested();

                campaignDetails = PostlistFileManager.GetAll(campaignId);

                var postCount = maximumPostLimitToStore - campaignDetails.Count;

                // Checking whether maximum count reached or not

                #region Add to bin Files

                if (postCount > 0)
                {
                    var neededPostLists = postlists.Take(postCount).ToList();
                    PostlistFileManager.AddRange(campaignId, neededPostLists);
                    var publisherInitialize = PublisherInitialize.GetInstance;
                    publisherInitialize.UpdatePostCounts(campaignId);
                }
                else
                {
                    // Inform the maximum post has reached via Toaster notification
                    ToasterNotification.ShowInfomation(string.Format(
                        "LangKeyPostlistReachedToMax".FromResourceDictionary(), campaignName, maximumPostLimitToStore));
                }

                #endregion
            }
            catch (OperationCanceledException ex)
            {
                ex.DebugLog("Cancellation Requested!");
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (ArgumentNullException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     Remove Cdata Details from Rss Feed
        /// </summary>
        /// <param name="node">Rss node Details</param>
        /// <returns></returns>
        private static string RemoveCdata(string node)
        {
            return node?.Replace("<![CDATA[", "").Replace("]]>", "") ?? string.Empty;
        }
    }

    public class SocialHttpHelper : HttpHelper
    {
    }
}