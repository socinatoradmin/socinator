#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using CommonServiceLocator;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using FluentScheduler;
using Newtonsoft.Json;

#endregion

namespace DominatorHouseCore.Process
{
    public class PublisherPostFetcher
    {
        /// <summary>
        ///     To keep the cancellation token for the campaign to avoid scraping after deleted the campaign
        /// </summary>
        public static ConcurrentDictionary<string, CancellationTokenSource> FetchingCampaignsCancellationToken
        {
            get;
            set;
        } = new ConcurrentDictionary<string, CancellationTokenSource>();

        /// <summary>
        ///     Campaign Id which are running under RSS or Monitor Folder
        /// </summary>
        public static SortedSet<string> JobFetcherId { get; set; } = new SortedSet<string>();

        /// <summary>
        ///     To Start fetching the post from Scrape Post, Share Post(only for Facebook), Rss and Monitor folder
        /// </summary>
        public void StartFetchingPostData()
        {
            var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            // Get the post fetch details from bin file <see cref="ConstantVariable.GetPublisherPostFetchFile" /> other than normal post
            var allCampaign =
                genericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable
                    .GetPublisherCampaignFile());

            var getFetchDetails = genericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                .GetPublisherPostFetchFile);

            var deletedCampaignFetcherList = getFetchDetails.Where(x =>
                allCampaign.Any(y => y.CampaignId == x.CampaignId)).ToList();

            // Commented to fix bug 53 (Social bug sheet) - commented for not letting it to delete the postdestination information from the bin file, because we need it to publish existing active publisher campaign
            //deletedCampaignFetcherList.ForEach(x =>
            //{
            //    genericFileManager.Delete<PublisherPostFetchModel>(y => y.CampaignId == x.CampaignId, ConstantVariable
            //        .GetPublisherPostFetchFile);
            //});

            getFetchDetails.RemoveAll(x => deletedCampaignFetcherList.Any(y => y.CampaignId == x.CampaignId));

            getFetchDetails = getFetchDetails.Where(x => x.PostSource != PostSource.NormalPost).ToList();

            // Iterate the post fetch detail
            getFetchDetails.ForEach(postFetchModel =>
            {
                try
                {
                    if (allCampaign.FirstOrDefault(x => x.CampaignId == postFetchModel.CampaignId)?.CampaignStatus !=
                        PublisherCampaignStatus.Active) return;
                    // Register the campaign its running from current fetcher with respective post source and get the cancellation token
                    var cancellationTokenSource =
                        RegisterPostFetcher(postFetchModel.CampaignId, postFetchModel.PostSource);

                    // Call the fetch methods with passing respective cancellation source
                    FetchPosts(postFetchModel, cancellationTokenSource);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
        }

        /// <summary>
        ///     To receive the campaign Id for give post source and campaign Id
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="postSource">
        ///     <see cref=" DominatorHouseCore.Enums.SocioPublisher.PostSource" />
        /// </param>
        /// <returns></returns>
        public static string GetCampaignFetcherId(string campaignId, PostSource postSource)
        {
            return $"{campaignId}-{postSource.ToString()}-PostFetcher";
        }

        /// <summary>
        ///     Start fetching the post by using campaing Id, Its used in while saving the campaign and cloned campaigns
        /// </summary>
        /// <param name="campaignId">campaign Id</param>
        public void FetchPostsForCampaign(string campaignId)
        {
            try
            {
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                // Collect the respective Fetcher model
                var postFetchModels = genericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile)
                    .Where(x => x.CampaignId == campaignId && x.PostSource != PostSource.NormalPost);

                // Call with Task factory
                ThreadFactory.Instance.Start(() =>
                {
                    // Iterate the post fetcher, because for a same campaign we may have to run RSS , Monitor Folder
                    postFetchModels.ForEach(postFetchModel =>
                    {
                        // Register the campaign its running from current fetcher with respective post source and get the cancellation token
                        var cancellationTokenSource = RegisterPostFetcher(campaignId, postFetchModel.PostSource);

                        // Call the fetch methods with passing respective cancellation source
                        FetchPosts(postFetchModel, cancellationTokenSource);
                    });
                });
            }
            catch (ArgumentNullException ex)
            {
                ex.DebugLog();
            }
            catch (OperationCanceledException ex)
            {
                ex.DebugLog("Cancellation Requested!");
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     Register the campaign with their running cancellation token per every post source
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="postSource">
        ///     <see cref=" DominatorHouseCore.Enums.SocioPublisher.PostSource" />
        /// </param>
        /// <returns></returns>
        private CancellationTokenSource RegisterPostFetcher(string campaignId, PostSource postSource)
        {
            // Get new cancellation token
            var cancellationTokenSource = new CancellationTokenSource();

            // Get the unique fetcher name, its generated from campaign Id and Post source
            var fetcherName = GetCampaignFetcherId(campaignId, postSource);

            // Its already present, update the new Cancellation token source. Otherwise add new cancellation along with fetcher name
            if (!FetchingCampaignsCancellationToken.ContainsKey(fetcherName))
            {
                // Register fercher name with cancellation token
                cancellationTokenSource =
                    FetchingCampaignsCancellationToken.GetOrAdd(fetcherName, cancellationTokenSource);
            }
            else
            {
                // If, fetcher already prensent, stop all running instance
                StopFetchingPosts(campaignId, postSource);

                // Update cancellation token source along with fetcher name
                FetchingCampaignsCancellationToken.AddOrUpdate(fetcherName, cancellationTokenSource,
                    (fetcher, oldvalue) =>
                    {
                        if (oldvalue == null)
                            throw new ArgumentNullException(nameof(oldvalue));

                        oldvalue = cancellationTokenSource;
                        return oldvalue;
                    });
            }

            return cancellationTokenSource;
        }

        /// <summary>
        ///     Stop post fetcher by using campaign Id
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="isDeleteRelatedModel"></param>
        public static void StopFetchingPostsByCampaignId(string campaignId, bool isDeleteRelatedModel = true)
        {
            try
            {
                // Get the fetcher details from the campaign ID
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var getFetchDetails =
                    genericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile).Where(x =>
                        x.PostSource != PostSource.NormalPost && x.CampaignId == campaignId);

                // Iterate all fetcher from list
                getFetchDetails.ForEach(fetchModel =>
                {
                    // Call stop fetching 
                    StopFetchingPosts(campaignId, fetchModel.PostSource);
                });

                // Delete all fetcher

                if (isDeleteRelatedModel)
                    genericFileManager.Delete<PublisherPostFetchModel>(x => x.CampaignId == campaignId, ConstantVariable
                        .GetPublisherPostFetchFile);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     Stop running post fetcher details
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="postSource">
        ///     <see cref=" DominatorHouseCore.Enums.SocioPublisher.PostSource" />
        /// </param>
        public static void StopFetchingPosts(string campaignId, PostSource postSource)
        {
            try
            {
                // Get the unique fetcher name, its generated from campaign Id and Post source
                var fetcherCampaignId = GetCampaignFetcherId(campaignId, postSource);

                // Its not present in running fetcher list, simply return
                if (!FetchingCampaignsCancellationToken.ContainsKey(fetcherCampaignId))
                    return;

                // Gather all Rss and monitor folder fetcher Id
                var relatedPostFectcher = JobFetcherId.Where(x => x.Contains(campaignId));

                // Remove all post fetcher details
                relatedPostFectcher.ToList().ForEach(JobManager.RemoveJob);

                // Remove Post fetcher Id from Sorted set
                JobFetcherId.RemoveWhere(x => x.Contains(campaignId));

                // Get the cancellation of fetcher
                var cancellationToken = FetchingCampaignsCancellationToken[fetcherCampaignId];

                //Cancel the operation which is running
                cancellationToken.Cancel();
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
        ///     Start fetching post for all Scarpe post(Facebook,Twitter,Pinterest),Share post(Facebook), Rss and Monitor Folder
        /// </summary>
        /// <param name="publisherPostFetchModel">
        /// </param>
        /// <param name="cancellationTokenSource"></param>
        public void FetchPosts(PublisherPostFetchModel publisherPostFetchModel,
            CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                if (publisherPostFetchModel.PostSource == PostSource.NormalPost)
                    return;

                //var generaldata = GenericFileManager.GetModuleDetails<GeneralModel>
                //    (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                //    .FirstOrDefault(x => x.CampaignId == publisherPostFetchModel.CampaignId) ?? new GeneralModel();


                // Check whether campaign expired or not
                if (publisherPostFetchModel.ExpireDate != null && publisherPostFetchModel.ExpireDate < DateTime.Now)
                {
                    GlobusLogHelper.log.Info(
                        $"{publisherPostFetchModel.CampaignName} expired on {publisherPostFetchModel.ExpireDate}");
                    return;
                }

                dynamic postFetchDetails = null;

                // Collect neccessary details for fetching and assign to dynamic type variable
                switch (publisherPostFetchModel.PostSource)
                {
                    case PostSource.SharePost:
                        postFetchDetails =
                            JsonConvert.DeserializeObject<SharePostModel>(
                                publisherPostFetchModel.PostDetailsWithFilters);
                        break;
                    case PostSource.ScrapeImages:
                    case PostSource.ScrapedPost:
                        postFetchDetails =
                            JsonConvert.DeserializeObject<ScrapePostModel>(publisherPostFetchModel
                                .PostDetailsWithFilters);
                        break;
                    case PostSource.RssFeedPost:
                        postFetchDetails =
                            JsonConvert.DeserializeObject<ObservableCollection<PublisherRssFeedModel>>(
                                publisherPostFetchModel.PostDetailsWithFilters);
                        break;
                    case PostSource.MonitorFolderPost:
                        postFetchDetails =
                            JsonConvert.DeserializeObject<ObservableCollection<PublisherMonitorFolderModel>>(
                                publisherPostFetchModel.PostDetailsWithFilters);
                        break;
                    case PostSource.NormalPost:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // get the post scraper object for Rss feed and monitor folder
                var postScraper = PublisherInitialize.GetPublisherLibrary(SocialNetworks.Social)
                    .GetPublisherCoreFactory()
                    .PostScraper.GetPostScraperLibrary(publisherPostFetchModel.CampaignId, cancellationTokenSource,
                        publisherPostFetchModel);

                // Call the respective post scraper methods
                switch (publisherPostFetchModel.PostSource)
                {
                    case PostSource.RssFeedPost:
                        // Get the proper name for monitor job process
                        var jobName = $"{publisherPostFetchModel.CampaignId}-{PostSource.RssFeedPost.ToString()}";

                        var currentCampaignstatus = PublisherInitialize.GetInstance.GetSavedCampaigns()
                            .FirstOrDefault(x => x.CampaignId == publisherPostFetchModel.CampaignId);
                        if (currentCampaignstatus?.Status != PublisherCampaignStatus.Active)
                            return;
                        // Register to sorted set
                        JobFetcherId.Add(jobName);
                        // Add the Job for Rss feed 
                        JobManager.AddJob(() =>
                        {
                            // Call the Rss feed fetcher
                            postScraper.ScrapeRssPosts(publisherPostFetchModel.CampaignId, postFetchDetails,
                                cancellationTokenSource, publisherPostFetchModel.MaximumPostLimitToStore,
                                publisherPostFetchModel.CampaignName);
                        },
                            s => s.WithName(jobName).ToRunOnceAt(DateTime.Now.AddSeconds(2))
                                .AndEvery(publisherPostFetchModel.DelayForNext).Minutes());
                        break;
                    case PostSource.MonitorFolderPost:
                        // Get the proper name for monitor job process
                        var monitorJobName =
                            $"{publisherPostFetchModel.CampaignId}-{PostSource.MonitorFolderPost}";

                        // Register to sorted set
                        JobFetcherId.Add(monitorJobName);

                        // Add the Job for Monitor Folder
                        JobManager.AddJob(() =>
                        {
                            // Call the Monitor folder fetcher
                            postScraper.FetchMonitorFoldersPosts(publisherPostFetchModel.CampaignId,
                                postFetchDetails,
                                cancellationTokenSource, publisherPostFetchModel.MaximumPostLimitToStore,
                                publisherPostFetchModel.CampaignName);
                        },
                            s => s.WithName(monitorJobName).ToRunOnceAt(DateTime.Now.AddSeconds(2))
                                .AndEvery(publisherPostFetchModel.DelayForNext).Minutes());
                        break;
                    case PostSource.ScrapeImages:
                        var scrapeImagesJobName =
                            $"{publisherPostFetchModel.CampaignId}-{PostSource.RssFeedPost.ToString()}";

                        var scrapeImagesCampaignstatus = PublisherInitialize.GetInstance.GetSavedCampaigns()
                            .FirstOrDefault(x => x.CampaignId == publisherPostFetchModel.CampaignId);
                        if (scrapeImagesCampaignstatus?.Status != PublisherCampaignStatus.Active)
                            return;
                        // Register to sorted set
                        JobFetcherId.Add(scrapeImagesJobName);
                        // Add the Job for Rss feed 
                        JobManager.AddJob(() =>
                        {
                            // Call the Rss feed fetcher
                            postScraper.ScrapeImagePosts(publisherPostFetchModel.CampaignId, postFetchDetails,
                                cancellationTokenSource, publisherPostFetchModel.MaximumPostLimitToStore,
                                publisherPostFetchModel.CampaignName);
                        },
                            s => s.WithName(scrapeImagesJobName).ToRunOnceAt(DateTime.Now.AddSeconds(2))
                                .AndEvery(publisherPostFetchModel.DelayForNext).Minutes());
                        break;
                    case PostSource.NormalPost:
                        break;
                    default:
                        // Iterate the selected destination Id
                        //if(typeof(postFetchDetails)== ScrapePostModel)

                        publisherPostFetchModel.SelectedDestinations.ToList().ForEach(destinationId =>
                        {
                            // Get the details of Destination Id
                            var binFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();
                            var destinationDetails = binFileHelper.GetSingleDestination(destinationId);

                            // Itereate the destination
                            try
                            {
                                destinationDetails.AccountsWithNetwork.ForEach(networkWithAccount =>
                                {
                                    var destinationSelectModel = destinationDetails.ListSelectDestination
                                        .FirstOrDefault(x => x.AccountId == networkWithAccount.Value);
                                    if (!SocinatorInitialize.IsNetworkAvailable(networkWithAccount.Key) ||
                                        destinationSelectModel == null || !destinationSelectModel.IsScrapeFromAccount) return;
                                    try
                                    {
                                        var networkPostScraper = PublisherInitialize
                                            .GetPublisherLibrary(networkWithAccount.Key).GetPublisherCoreFactory()
                                            .PostScraper.GetPostScraperLibrary(publisherPostFetchModel.CampaignId,
                                                cancellationTokenSource, publisherPostFetchModel);

                                        if (publisherPostFetchModel.PostSource == PostSource.SharePost)
                                        {
                                            var scrapeJobName =
                                                    $"{publisherPostFetchModel.CampaignId}-{PostSource.SharePost.ToString()}";
                                            var postDetails = (SharePostModel)postFetchDetails;
                                            // Call Share post from facebook
                                            if (networkWithAccount.Key == SocialNetworks.Facebook)
                                            {
                                                // Register to sorted set
                                                JobFetcherId.Add(scrapeJobName);
                                                if (postDetails.IsShareFdPagePost)
                                                {
                                                    // Add the Job for scrape 
                                                    JobManager.AddJob(
                                                        () =>
                                                        {
                                                            networkPostScraper.ScrapeFdPagePostUrl(
                                                                networkWithAccount.Value,
                                                                publisherPostFetchModel.CampaignId, postFetchDetails,
                                                                cancellationTokenSource,
                                                                publisherPostFetchModel.ScrapeCount);
                                                        },
                                                        s => s.WithName(scrapeJobName)
                                                            .ToRunOnceAt(DateTime.Now.AddSeconds(2))
                                                            .AndEvery(publisherPostFetchModel.DelayForNext).Minutes());
                                                }
                                                else
                                                {
                                                    // Add the Job for scrape 
                                                    JobManager.AddJob(
                                                        () =>
                                                        {
                                                            networkPostScraper.ScrapeFdKeywords(
                                                                networkWithAccount.Value,
                                                                publisherPostFetchModel.CampaignId, postFetchDetails,
                                                                cancellationTokenSource,
                                                                publisherPostFetchModel.ScrapeCount);
                                                        },
                                                        s => s.WithName(scrapeJobName)
                                                            .ToRunOnceAt(DateTime.Now.AddSeconds(2))
                                                            .AndEvery(publisherPostFetchModel.DelayForNext).Minutes());
                                                }
                                            }
                                            else
                                            {
                                                JobFetcherId.Add(scrapeJobName);
                                                // Add the Job for scrape 
                                                JobManager.AddJob(
                                                    () =>
                                                    {
                                                        networkPostScraper.ScrapeFdKeywords(
                                                            networkWithAccount.Value,
                                                            publisherPostFetchModel.CampaignId, postFetchDetails,
                                                            cancellationTokenSource,
                                                            publisherPostFetchModel.ScrapeCount);
                                                    },
                                                    s => s.WithName(scrapeJobName)
                                                        .ToRunOnceAt(DateTime.Now.AddSeconds(2))
                                                        .AndEvery(publisherPostFetchModel.DelayForNext).Minutes());
                                            }
                                        }
                                        // Scarpe the posts from Facebook, Twitter, Pinterest
                                        else if (publisherPostFetchModel.PostSource == PostSource.ScrapedPost)
                                        {
                                            // Get the proper name for scrape job process
                                            var scrapeJobName =
                                                $"{publisherPostFetchModel.CampaignId}-{PostSource.ScrapedPost.ToString()}";

                                            // Register to sorted set
                                            JobFetcherId.Add(scrapeJobName);

                                            // Add the Job for scrape 
                                            JobManager.AddJob(() =>
                                            {
                                                networkPostScraper.ScrapePosts(networkWithAccount.Value,
                                                    publisherPostFetchModel.CampaignId, postFetchDetails,
                                                    cancellationTokenSource,
                                                    publisherPostFetchModel.ScrapeCount);
                                            },
                                                s => s.WithName(scrapeJobName)
                                                    .ToRunOnceAt(DateTime.Now.AddSeconds(2))
                                                    .AndEvery(publisherPostFetchModel.DelayForNext).Minutes());
                                        }
                                    }
                                    catch (OperationCanceledException ex)
                                    {
                                        ex.DebugLog("Cancellation Requested!");
                                    }
                                    catch (AggregateException ae)
                                    {
                                        ae.HandleOperationCancellation();
                                    }
                                    catch (Exception ex)
                                    {
                                        ex.DebugLog();
                                    }
                                });
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        });
                        break;
                }
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
    }
}