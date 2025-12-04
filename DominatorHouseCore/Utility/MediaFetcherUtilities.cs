#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using CommonServiceLocator;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;

#endregion

namespace DominatorHouseCore.Utility
{
    public class MediaFetcherUtilities
    {
        private ISocialBrowserManager BrowserManager { get; }

        public MediaFetcherUtilities ()
        {
            BrowserManager = InstanceProvider.GetInstance<ISocialBrowserManager>();
        }

        public void FetchDetailsFromLink ( ScrapePostModel postDetailsModel, string campaignId,
            CancellationTokenSource cancellationTokenSource, int maximumPostLimitToStore, string campaignName )
        {
            try
            {
                var imageUrlList = Regex.Split(postDetailsModel.AddGooglePostSource, ",").ToList();

                imageUrlList.Shuffle();

                foreach (var imageUrl in imageUrlList)
                {
                    if (imageUrl.Contains("[G]"))
                        FetchMediaFromGoogle(imageUrl, postDetailsModel, campaignId, cancellationTokenSource,
                            maximumPostLimitToStore, campaignName);
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (imageUrl.Contains("[U]"))
                        FetchMediaFromLink(imageUrl, postDetailsModel, campaignId, cancellationTokenSource,
                            maximumPostLimitToStore, campaignName);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void FetchMediaFromGoogle ( string googleImageLink, ScrapePostModel postDetailsModel, string campaignId,
            CancellationTokenSource cancellationTokenSource, int maximumPostLimitToStore, string campaignName )
        {
            try
            {
                int totalCount;
                var descriptionCount = 1;
                var pageTitle = string.Empty;
                DateTime? expireDate = DateTime.Now.AddYears(2);
                var postCollection = new List<PublisherPostlistModel>();
                var imageUrl = Regex.Split(googleImageLink, "\\[G\\]").LastOrDefault();

                var campaignDetails = PostlistFileManager.GetAll(campaignId);

                if (campaignDetails.Count > 0 && campaignDetails.Any(x => x.PostSource == PostSource.ScrapeImages))
                    int.TryParse(campaignDetails.LastOrDefault(x => x.PostSource == PostSource.ScrapeImages
                                                                    && x.PostDescription.Contains("_")).PostDescription
                        .Split('_').LastOrDefault(), out descriptionCount);

                totalCount = campaignDetails.Count;

                var postCount = maximumPostLimitToStore - campaignDetails.Count;

                try
                {
                    var account = new DominatorAccountModel
                    {
                        AccountBaseModel = new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Social,
                            AccountProxy = new Proxy()
                        }
                    };

                    BrowserManager.BrowserLogin(account, cancellationTokenSource.Token);

                    BrowserManager.ExpandGoogleImagesFromLink(imageUrl, ref pageTitle);

                    var publisherInitialize = PublisherInitialize.GetInstance;

                    while (postCollection.Count < postDetailsModel.ScrapeCountPerUrl
                           && totalCount < postCount)
                        try
                        {
                            Thread.Sleep(2000);

                            var image = BrowserManager.GetGoogleImages();

                            cancellationTokenSource.Token.ThrowIfCancellationRequested();

                            //if (!BrowserManager.HasMoreResults())
                            //    break;

                            //if (!image.Contains("http") || !image.ToLower().Contains(".jpg") &&
                            //                            !image.ToLower().Contains(".png") &&
                            //                            !image.ToLower().Contains(".jpeg")
                            //                            && !image.ToLower().Contains("format=jpeg") &&
                            //                            !image.ToLower().Contains("googleusercontent.com")
                            //                            || campaignDetails.FirstOrDefault(x =>
                            //                                x.MediaList.Contains(image)) != null)
                            //    continue;
                            if (string.IsNullOrEmpty(image) || campaignDetails.FirstOrDefault(x => x.MediaList.Contains(image)) != null)
                                continue;
                            var postFetcherList = new PublisherPostlistModel
                            {
                                MediaList = new ObservableCollection<string> { image },
                                CampaignId = campaignId,
                                CreatedTime = DateTime.Now,
                                ExpiredTime = expireDate,
                                PostId = Utilities.GetGuid(),
                                PostCategory = PostCategory.OrdinaryPost,
                                PostQueuedStatus = PostQueuedStatus.Pending,
                                PostRunningStatus = PostRunningStatus.Active,
                                PostSource = PostSource.ScrapeImages,
                                PostDescription = postDetailsModel.IsUseFileNameAsDescription ||
                                                  postDetailsModel.LstUploadPostDescription.Count < descriptionCount
                                    ? $"{pageTitle}_{descriptionCount}"
                                    : postDetailsModel.LstUploadPostDescription[descriptionCount - 1]
                                //PdSourceUrl = postDetailsModel.PdSourceUrl.Replace("[FeedUrl]", newLink),
                                //PublisherInstagramTitle = 
                            };



                            postCollection.Add(postFetcherList);
                            PostlistFileManager.Add(campaignId, postFetcherList);
                            publisherInitialize.UpdatePostCounts(campaignId);


                            totalCount++;
                            descriptionCount++;
                        }
                        catch (OperationCanceledException)
                        {
                            throw new OperationCanceledException();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                    BrowserManager.CloseBrowser(account);

                    Thread.Sleep(1000);

                    if (postCount > 0)
                    {
                        publisherInitialize.UpdatePostCounts(campaignId);
                    }
                    else
                    {
                        // Inform the maximum post has reached via Toaster notification
                        ToasterNotification.ShowInfomation(string.Format(
                            "LangKeyPostlistReachedToMax".FromResourceDictionary(), campaignName,
                            maximumPostLimitToStore));
                        return;
                    }

                    postCollection.Clear();

                    if (totalCount >= postDetailsModel.ScrapeCount)
                        return;
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void FetchMediaFromLink ( string imageLink, ScrapePostModel postDetailsModel, string campaignId,
            CancellationTokenSource cancellationTokenSource, int maximumPostLimitToStore, string campaignName )
        {
            try
            {
                var totalCount = 1;
                var descriptionCount = 1;
                var pageTitle = string.Empty;
                var description = string.Empty;
                DateTime? expireDate = DateTime.Now.AddYears(2);
                var postCollection = new List<PublisherPostlistModel>();
                var imageUrlList = Regex.Split(imageLink, "\\[U\\]").ToList();

                var campaignDetails = PostlistFileManager.GetAll(campaignId);

                if (campaignDetails.Count > 0 && campaignDetails.Any(x => x.PostSource == PostSource.ScrapeImages))
                    int.TryParse(campaignDetails.LastOrDefault(x => x.PostSource == PostSource.ScrapeImages
                                                                    && x.PostDescription.Contains("_"))?.PostDescription
                        ?.Split('_').LastOrDefault(), out descriptionCount);

                var postCount = maximumPostLimitToStore - campaignDetails.Count;

                imageUrlList.RemoveAll(x => string.IsNullOrEmpty(x) || x.Contains("\r\n"));

                foreach (var imageUrl in imageUrlList)
                    try
                    {
                        var failedCount = 0;

                        cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        var imagelist = ImageExtracter.ExtractLinkDetails(imageUrl, ref pageTitle, ref description)
                            .ToList();

                        while (imagelist.Any(x => !x.Contains("https://") && !x.Contains("http://")) &&
                               failedCount++ < 4)
                        {
                            Thread.Sleep(1000);
                            imagelist = ImageExtracter.ExtractLinkDetails(imageUrl, ref pageTitle, ref description)
                                .ToList();
                        }

                        imagelist.RemoveAll(x => !x.Contains("https://") && !x.Contains("http://"));

                        imagelist.RemoveAll(x => campaignDetails.Any(y => y.MediaList.Contains(x)));

                        foreach (var image in imagelist)
                        {
                            var postFetcherList = new PublisherPostlistModel
                            {
                                MediaList = new ObservableCollection<string> { image },
                                CampaignId = campaignId,
                                CreatedTime = DateTime.Now,
                                ExpiredTime = expireDate,
                                PostId = Utilities.GetGuid(),
                                PostCategory = PostCategory.OrdinaryPost,
                                PostQueuedStatus = PostQueuedStatus.Pending,
                                PostRunningStatus = PostRunningStatus.Active,
                                PostSource = PostSource.ScrapeImages,
                                PostDescription = postDetailsModel.IsUseFileNameAsDescription ||
                                                  postDetailsModel.LstUploadPostDescription.Count < descriptionCount
                                    ? description
                                    : postDetailsModel.LstUploadPostDescription[descriptionCount],
                                PdSourceUrl = imageUrl,
                                PublisherInstagramTitle = pageTitle
                            };

                            if (postCollection.Count < postDetailsModel.ScrapeCountPerUrl)
                                postCollection.Add(postFetcherList);
                            else
                                break;

                            totalCount++;
                            descriptionCount++;
                        }

                        Thread.Sleep(1000);

                        if (postCount > 0)
                        {
                            postCollection = postCollection.Take(postCount).ToList();
                            PostlistFileManager.AddRange(campaignId, postCollection);
                            var publisherInitialize = PublisherInitialize.GetInstance;
                            publisherInitialize.UpdatePostCounts(campaignId);
                        }
                        else
                        {
                            // Inform the maximum post has reached via Toaster notification
                            ToasterNotification.ShowInfomation(string.Format(
                                "LangKeyPostlistReachedToMax".FromResourceDictionary(), campaignName,
                                maximumPostLimitToStore));
                            break;
                        }

                        postCount -= postCollection.Count;

                        postCollection.Clear();

                        if (totalCount >= postDetailsModel.ScrapeCount)
                            break;
                    }
                    catch (OperationCanceledException)
                    {
                        throw new OperationCanceledException();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}