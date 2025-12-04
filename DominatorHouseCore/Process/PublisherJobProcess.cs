#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using CommonServiceLocator;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Patterns;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using DominatorHouseCore.Enums.SocioPublisher;
using System.IO;

#endregion

namespace DominatorHouseCore.Process
{
    public abstract class PublisherJobProcess
    {
        protected readonly IGenericFileManager GenericFileManager;
        private readonly IAccountsFileManager _accountsFileManager;


        #region Constructor

        public PublisherJobProcess()
        {
            _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            GenericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();

            var softwareSettings = InstanceProvider.GetInstance<ISoftwareSettings>();
            if (!(softwareSettings.Settings?.IsThreadLimitChecked ?? false)) return;
            DominatorScheduler.islogged = false;
            DominatorScheduler.maxThreadCount = softwareSettings.Settings.MaxThreadCount;
            DominatorScheduler._lockWithThreadLimit = new SemaphoreSlim(DominatorScheduler.maxThreadCount,
                DominatorScheduler.maxThreadCount);

            SetThreadCountAsPerConfig();
        }

        protected PublisherJobProcess(string campaignId,
            string accountId,
            SocialNetworks network,
            List<string> groupDestinationLists,
            List<string> pageDestinationList,
            List<PublisherCustomDestinationModel> customDestinationModels,
            bool isPublishOnOwnWall,
            CancellationTokenSource campaignCancellationToken) : this()
        {
            // assign campaign Id
            CampaignId = campaignId;

            // assign network 
            Network = network;

            //Get the general settings from bin files
            GeneralSettingsModel = GenericFileManager.GetModuleDetails<GeneralModel>
                                           (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                                       .FirstOrDefault(x => x.CampaignId == campaignId) ?? new GeneralModel();

            // Get the full account details from account Id
            AccountModel = _accountsFileManager.GetAccountById(accountId);

            PageDestinationList = pageDestinationList;

            GroupDestinationList = groupDestinationLists;

            CustomDestinationList = customDestinationModels;

            IsPublishOnOwnWall = isPublishOnOwnWall;

            // Get the campaigns full model
            var publisherCampaign =
                GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable
                    .GetPublisherCampaignFile()).FirstOrDefault(x => x.CampaignId == CampaignId);

            JobConfigurations = publisherCampaign?.JobConfigurations;

            OtherConfiguration = publisherCampaign?.OtherConfiguration;

            CampaignCancellationToken = campaignCancellationToken;

            CurrentJobCancellationToken = new CancellationTokenSource();

            //Linked the job configuration's cancellation token source with campaign's cancellation token
            CombinedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(CampaignCancellationToken.Token,
                CurrentJobCancellationToken.Token);

            // Get the fetcher details, its useful for getting campaigns Name
            var campaign = GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == CampaignId);

            CampaignName = campaign?.CampaignName;
        }


        protected PublisherJobProcess(string campaignId, string campaignName, string accountId, SocialNetworks network,
            IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken) : this()
        {
            // assign campaign Id
            CampaignId = campaignId;

            // assign network 
            Network = network;

            //Get the general settings from bin files
            GeneralSettingsModel = GenericFileManager.GetModuleDetails<GeneralModel>
                                           (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                                       .FirstOrDefault(x => x.CampaignId == campaignId) ?? new GeneralModel();

            // Get the full account details from account Id
            AccountModel = _accountsFileManager.GetAccountById(accountId);

            PublisherDestinationDetailsModels = destinationDetails.ToList();

            // Get the campaigns full model
            var publisherCampaign =
                GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable
                    .GetPublisherCampaignFile()).FirstOrDefault(x => x.CampaignId == CampaignId);

            JobConfigurations = publisherCampaign?.JobConfigurations;

            OtherConfiguration = publisherCampaign?.OtherConfiguration;

            CampaignCancellationToken = campaignCancellationToken;

            CurrentJobCancellationToken = new CancellationTokenSource();

            //Linked the job configuration's cancellation token source with campaign's cancellation token
            CombinedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(CampaignCancellationToken.Token,
                CurrentJobCancellationToken.Token);

            CampaignName = campaignName;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     To specify the campaign Id
        /// </summary>
        public string CampaignId { get; set; }

        /// <summary>
        ///     To Specify the campaign Name
        /// </summary>
        public string CampaignName { get; set; }

        /// <summary>
        ///     To Specify the social network
        /// </summary>
        public SocialNetworks Network { get; set; }

        /// <summary>
        ///     To Hold Job's configuration settings
        /// </summary>
        public JobConfigurationModel JobConfigurations { get; set; }

        /// <summary>
        ///     To holds other configurations settings
        /// </summary>
        public OtherConfigurationModel OtherConfiguration { get; set; }

        /// <summary>
        ///     To holds advanced general settings of the campaign
        /// </summary>
        public GeneralModel GeneralSettingsModel { get; set; }

        /// <summary>
        ///     lock for job process
        /// </summary>
        private static readonly object SyncJobProcess = new object();


        /// <summary>
        ///     Current account details
        /// </summary>
        public DominatorAccountModel AccountModel { get; set; }

        /// <summary>
        ///     Groups destinations Collection
        /// </summary>
        public List<string> GroupDestinationList { get; set; }

        /// <summary>
        ///     Pages destination collections
        /// </summary>
        public List<string> PageDestinationList { get; set; }


        public List<PublisherDestinationDetailsModel> PublisherDestinationDetailsModels { get; set; }

        /// <summary>
        ///     Custom destination collections
        /// </summary>
        public List<PublisherCustomDestinationModel> CustomDestinationList { get; set; }

        /// <summary>
        ///     Is need to publish on own wall
        /// </summary>
        public bool IsPublishOnOwnWall { get; set; }

        /// <summary>
        ///     Campaign's cancellation token
        /// </summary>
        public CancellationTokenSource CampaignCancellationToken { get; set; }

        /// <summary>
        ///     Job's cancellation token
        /// </summary>
        public CancellationTokenSource CurrentJobCancellationToken { get; set; }

        /// <summary>
        ///     Combined cancellation token source for campaigns and jobs
        /// </summary>
        public CancellationTokenSource CombinedCancellationToken { get; set; }

        /// <summary>
        ///     The method for override to publishing to group for an accounts
        /// </summary>
        /// <param name="accountId">Account Id</param>
        /// <param name="groupUrl">Group Url</param>
        /// <param name="postDetails">Publishing post details<see cref="PublisherPostlistModel" /></param>
        /// <param name="isDelayNeed">Specify is delay needed after success/failed post</param>
        /// <returns></returns>
        public virtual bool PublishOnGroups(string accountId, string groupUrl, PublisherPostlistModel postDetails,
            bool isDelayNeed = true)
        {
            return false;
        }


        /// <summary>
        ///     The method for override to publishing to page for an accounts
        /// </summary>
        /// <param name="accountId">Account Id</param>
        /// <param name="pageUrl">Page Url</param>
        /// <param name="postDetails">Publishing post details<see cref="PublisherPostlistModel" /></param>
        /// <param name="isDelayNeed">Specify is delay needed after success/failed post</param>
        public virtual bool PublishOnPages(string accountId, string pageUrl, PublisherPostlistModel postDetails,
            bool isDelayNeed = true)
        {
            return false;
        }


        /// <summary>
        ///     The method for override to publishing to own profile for an accounts
        /// </summary>
        /// <param name="accountId">Account Id</param>
        /// <param name="postDetails">Publishing post details<see cref="PublisherPostlistModel" /></param>
        /// <param name="isDelayNeed">Specify is delay needed after success/failed post</param>
        public virtual bool PublishOnOwnWall(string accountId, PublisherPostlistModel postDetails,
            bool isDelayNeed = true)
        {
            return false;
        }

        /// <summary>
        ///     The method for override to publishing to custom destination for an accounts
        /// </summary>
        /// <param name="accountId">Account Id</param>
        /// <param name="customDestinationModel">
        ///     custom destination tyoe and Url <see cref="PublisherCustomDestinationModel" />
        /// </param>
        /// <param name="postDetails">Publishing post details<see cref="PublisherPostlistModel" /></param>
        /// <param name="isDelayNeed">Specify is delay needed after success/failed post</param>
        public virtual bool PublishOnCustomDestination(string accountId,
            PublisherCustomDestinationModel customDestinationModel, PublisherPostlistModel postDetails,
            bool isDelayNeed = true)
        {
            return false;
        }
        /// <summary>
        ///     To specify already published count
        /// </summary>
        public int PublishedCount { get; set; }

        #endregion

        #region Methods
        public virtual CustomPostDetail GetCustomPostDetails(DominatorAccountModel dominatorAccount,string PostUrl)
        {
            return new CustomPostDetail();
        }
        /// <summary>
        ///     To delete a published post after x hours
        /// </summary>
        /// <param name="postId">Published Post Id/Post Url</param>
        /// <returns></returns>
        public virtual bool DeletePost(string postId)
        {
            return true;
        }

        public void ThreadLimit()
        {
            if (DominatorScheduler._lockWithThreadLimit == null) return;
            if (DominatorScheduler._lockWithThreadLimit?.CurrentCount == 0 && !DominatorScheduler.islogged)
            {
                DominatorScheduler.islogged = true;
                GlobusLogHelper.log.Info(
                    $"{"LangKeyThreadLimitReachedTo".FromResourceDictionary()} {DominatorScheduler.maxThreadCount} {"LangKeyPendingStartsWhenRunnningStops".FromResourceDictionary()}");
            }

            DominatorScheduler._lockWithThreadLimit?.Wait();
        }

        public void ThreadLimitAsPerConfig()
        {
            if (AccountModel != null && AccountModel.IsRunProcessThroughBrowser)
            {
                if (DominatorScheduler._lockWithThreadLimitAsPerConfig == null) return;
                DominatorScheduler._lockWithThreadLimitAsPerConfig?.Wait();
            }
        }

        private void SetThreadCountAsPerConfig()
        {
            try
            {
                if (DominatorScheduler._lockWithThreadLimitAsPerConfig == null)
                {
                    var perfCounterService = InstanceProvider.GetInstance<IPerfCounterService>();
                    var ramSize = Convert.ToInt32(perfCounterService.LoadedMemoryDescrption.Replace(" MB", ""));
                    if (ramSize < 5000)
                    {
                        DominatorScheduler.maxBrowserInstances = 6;
                        DominatorScheduler._lockWithThreadLimitAsPerConfig = new SemaphoreSlim(DominatorScheduler.maxBrowserInstances, DominatorScheduler.maxBrowserInstances);
                    }
                    else if (ramSize < 9000)
                    {
                        DominatorScheduler.maxBrowserInstances = 10;
                        DominatorScheduler._lockWithThreadLimitAsPerConfig = new SemaphoreSlim(DominatorScheduler.maxBrowserInstances, DominatorScheduler.maxBrowserInstances);
                    }
                    else
                    {
                        DominatorScheduler.maxBrowserInstances = 15;
                        DominatorScheduler._lockWithThreadLimitAsPerConfig = new SemaphoreSlim(DominatorScheduler.maxBrowserInstances, DominatorScheduler.maxBrowserInstances);
                    }

                }

            }
            catch (Exception)
            {
                if (DominatorScheduler._lockWithThreadLimitAsPerConfig == null)
                {
                    DominatorScheduler.maxBrowserInstances = 10;
                    DominatorScheduler._lockWithThreadLimitAsPerConfig = new SemaphoreSlim(DominatorScheduler.maxBrowserInstances, DominatorScheduler.maxBrowserInstances);
                }
            }
        }

        /// <summary>
        ///     To Start publishing a post for an account
        /// </summary>
        /// <param name="isRunParallel">Specify whether need to run on parallely or not</param>
        public void StartPublishingPosts(bool isRunParallel)
        {
            lock (SyncJobProcess)
            {
                // check whether need to run parallel
                if (isRunParallel)
                {
                    if (AccountModel == null)
                        return;

                    ThreadLimit();

                    ThreadLimitAsPerConfig();

                    // Call with task
                    ThreadFactory.Instance.Start(() =>
                    {
                        // start publishing with max post count
                        StartPublish();

                        // check any action waiting to perform
                        PublishScheduler.RunAndRemovePublisherAction(
                            $"{CampaignId}-{AccountModel.AccountBaseModel.AccountId}");

                        // after completion of publishing, decrease a publishing count
                        PublishScheduler.DecreasePublishingCount(CampaignId);

                        GlobusLogHelper.log.Info(Log.PublishingProcessCompleted,
                            AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName,
                            CampaignName);
                    }, CampaignCancellationToken.Token);
                }
                else
                {
                    ThreadLimit();

                    ThreadLimitAsPerConfig();

                    // start publishing with max post count
                    StartPublish();

                    // check any action waiting to perform
                    PublishScheduler.RunAndRemovePublisherAction(
                        $"{CampaignId}-{AccountModel.AccountBaseModel.AccountId}");

                    // after completion of publishing, decrease a publishing count
                    PublishScheduler.DecreasePublishingCount(CampaignId);

                    GlobusLogHelper.log.Info(Log.PublishingProcessCompleted,
                        AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName,
                        CampaignName);

                    var publishedPostList = PostlistFileManager.GetAll(CampaignId)
                        .Where(x => x.PostQueuedStatus == PostQueuedStatus.Published).ToList();
                    var accountIds = new List<string>();
                    bool flag = false;
                    foreach (var post in publishedPostList)
                    {
                        var pst = post.LstPublishedPostDetailsModels.Where(x => x.AccountId == AccountModel.AccountId).ToList();
                        var success = pst.Any(x => x.Successful != "Yes");
                        if (success && JobConfigurations.IsDelayPostChecked)
                        {
                            var delay = RandomUtilties.GetRandomNumber(JobConfigurations.DelayBetweenEachPost.EndValue, JobConfigurations.DelayBetweenEachPost.StartValue);
                            //       GlobusLogHelper.log.Info(Log.DelayBetweenMultiPost, AccountModel.AccountBaseModel.AccountNetwork,
                            //              AccountModel.AccountBaseModel.UserName, delay);
                            //  Thread.Sleep(TimeSpan.FromMinutes(delay));
                            flag = true;
                        }
                        else
                            flag = false;
                    }

                    if (!flag && JobConfigurations.IsAccountDelayChecked)
                    {
                        DelayBeforeNextAccountPublish();
                    }

                }
            }
        }

        /// <summary>
        ///     Starting a post for a current accounts with selected destinations
        /// </summary>
        private void StartPublish()
        {
            PublishedCount = 0;
            try
            {
                // Getting the delay while running after a x posts completions
                var multipostDelayCount = RandomUtilties.GetRandomNumber(JobConfigurations.PostRange.EndValue,
                    JobConfigurations.PostRange.StartValue);

                //var totalPostByCampaign = PostlistFileManager.GetAll(CampaignId);

                foreach (var destination in PublisherDestinationDetailsModels)
                {
                    // check whether cancellation token source already arised or not 
                    CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                    var destinationUrls = destination.DestinationType == ConstantVariable.OwnWall
                        ? AccountModel.AccountBaseModel.UserName
                        : destination.DestinationUrl;
                    if (!string.IsNullOrEmpty(destinationUrls) && destinationUrls != AccountModel.AccountBaseModel.UserFullName && !string.IsNullOrEmpty(destination.ListOfSection.Key) && destination.ListOfSection.Value != null)
                    {
                        if (destinationUrls == destination.ListOfSection.Key && destination.ListOfSection.Value?.Count > 0)
                            destination.PublisherPostlistModel.ListOfSections = destination.ListOfSection.Value;
                    }
                    GlobusLogHelper.log.Info(Log.StartPublishing,
                        AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, $"{destination.DestinationType} [{destinationUrls}]");
                    var IsDelayNeeded = CheckDelayNeeded(destination);
                    if (destination.DestinationType == ConstantVariable.Group)
                    {
                        // call networks to publishing on groups 
                        //PublishOnGroups(AccountModel.AccountId, destination.DestinationUrl,
                        //    destination.PublisherPostlistModel,
                        //    destination != PublisherDestinationDetailsModels.Last());
                        PublishOnGroups(AccountModel.AccountId, destination.DestinationUrl,
                            destination.PublisherPostlistModel,
                            IsDelayNeeded);
                    }
                    else if (destination.DestinationType == ConstantVariable.PageOrBoard)
                    {
                        // call networks to publishing on pages
                        //PublishOnPages(AccountModel.AccountId, destination.DestinationUrl,
                        //    destination.PublisherPostlistModel,
                        //    destination != PublisherDestinationDetailsModels.Last());
                        PublishOnPages(AccountModel.AccountId, destination.DestinationUrl,
                            destination.PublisherPostlistModel,
                            IsDelayNeeded);
                    }
                    else if (destination.DestinationType == ConstantVariable.OwnWall)
                    {
                        // call networks to publishing on own wall of an account
                        //PublishOnOwnWall(AccountModel.AccountId, destination.PublisherPostlistModel,
                        //    destination != PublisherDestinationDetailsModels.Last());
                        //PublishOnOwnWall(AccountModel.AccountId, destination.PublisherPostlistModel,
                        //    totalPostByCampaign.Count() > 1 && destination.PublisherPostlistModel != totalPostByCampaign.Last() ||
                        //    destination != PublisherDestinationDetailsModels.Last());
                        PublishOnOwnWall(AccountModel.AccountId, destination.PublisherPostlistModel,
                            IsDelayNeeded);
                    }
                    else
                    {
                        var customList = new PublisherCustomDestinationModel
                        {
                            DestinationType = destination.DestinationType,
                            DestinationValue = destination.DestinationUrl
                        };

                        // call networks to publishing on custom destinations 
                        //PublishOnCustomDestination(AccountModel.AccountId, customList,
                        //    destination.PublisherPostlistModel,
                        //    destination != PublisherDestinationDetailsModels.Last());
                        PublishOnCustomDestination(AccountModel.AccountId, customList,
                           destination.PublisherPostlistModel,
                           IsDelayNeeded);
                    }

                    PublishedCount++;

                    if (!JobConfigurations.IsAddDelayBetweenPublishingPost)
                        continue;

                    // check whether multiple post delay reached or not
                    if (PublishedCount % multipostDelayCount == 0)
                        DelayBetweenMultiPublish();
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
            finally
            {
                if (DominatorScheduler._lockWithThreadLimit?.CurrentCount != DominatorScheduler.maxThreadCount)
                    DominatorScheduler._lockWithThreadLimit?.Release();

                if (AccountModel != null && AccountModel.IsRunProcessThroughBrowser)
                    DominatorScheduler._lockWithThreadLimitAsPerConfig?.Release();
            }
        }

        private bool CheckDelayNeeded(PublisherDestinationDetailsModel destination)
        {
            try
            {
                var publishedcount = PostlistFileManager.GetAll(CampaignId).Where(z => z.PostQueuedStatus == PostQueuedStatus.Published).ToList();
                var totalPostByCampaign = PostlistFileManager.GetAll(CampaignId);
                if(totalPostByCampaign!= null && totalPostByCampaign.Count > 0)
                {
                    return publishedcount.Count(t => t.LstPublishedPostDetailsModels.Any(c => c.Successful == ConstantVariable.Yes && c.IsPublished == ConstantVariable.Yes))+1
                        < JobConfigurations.MaxPost;
                }
                return totalPostByCampaign.Count() > 1 && destination.PublisherPostlistModel != totalPostByCampaign.Last() ||
                            destination != PublisherDestinationDetailsModels.Last();
            }
            catch { return true; }
        }


        /// <summary>
        ///     Update Post with specified success status
        /// </summary>
        /// <param name="destinationUrl">>Destination Url</param>
        /// <param name="posts">Post model <see cref="PublisherPostlistModel" /></param>
        /// <param name="publishedUrl">Published Post Id/ Url</param>
        public void UpdatePostWithSuccessful(string destinationUrl, PublisherPostlistModel posts, string publishedUrl)
        {
            Thread.Sleep(1000);

            //Get the locking objects
            var updatelock = PublishScheduler.UpdatingLock.GetOrAdd(posts.PostId, _lock => new object());

            lock (updatelock)
            {
                // get the post details
                var post = PostlistFileManager.GetByPostId(CampaignId, posts.PostId);

                // get the post index where current destination present
                var postIndex = post.LstPublishedPostDetailsModels.IndexOf(
                    post.LstPublishedPostDetailsModels.FirstOrDefault(y =>
                        y.DestinationUrl == destinationUrl && y.AccountId == AccountModel.AccountId));

                // if post index is not present,then return
                if (postIndex == -1)
                    return;

                // Pass the information about success
                post.LstPublishedPostDetailsModels[postIndex].Successful = ConstantVariable.Yes;
                post.LstPublishedPostDetailsModels[postIndex].Link = publishedUrl;
                post.LstPublishedPostDetailsModels[postIndex].PublishedDate = DateTime.Now;
                post.LstPublishedPostDetailsModels[postIndex].ErrorDetails = ConstantVariable.NoError;
                post.LstPublishedPostDetailsModels[postIndex].Title = posts.PublisherInstagramTitle;
                post.LstPublishedPostDetailsModels[postIndex].Description = posts.PostDescription;
                // Update the post details to bin file
                PostlistFileManager.UpdatePost(CampaignId, post);

                // Update the used post status
                PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);

                // Add into success details
                GenericFileManager.AddModule(post.LstPublishedPostDetailsModels[postIndex],
                    ConstantVariable.GetPublishedSuccessDetails);
            }
        }


        /// <summary>
        ///     Update Post with specified failed status
        /// </summary>
        /// <param name="destinationUrl">Destination Url</param>
        /// <param name="posts">Post model <see cref="PublisherPostlistModel" /></param>
        /// <param name="errorMessage">Pass the error message</param>
        public void UpdatePostWithFailed(string destinationUrl, PublisherPostlistModel posts, string errorMessage)
        {
            //Get the locking objects
            var updatelock = PublishScheduler.UpdatingLock.GetOrAdd(posts.PostId, _lock => new object());

            lock (updatelock)
            {
                // get the post details
                var post = PostlistFileManager.GetByPostId(CampaignId, posts.PostId);

                // get the post index where current destination present
                var postIndex = post.LstPublishedPostDetailsModels.IndexOf(
                    post.LstPublishedPostDetailsModels.FirstOrDefault(y =>
                        y.DestinationUrl == destinationUrl && y.AccountId == AccountModel.AccountId));

                // if post index is not present,then return
                if (postIndex == -1)
                    return;

                // Pass error message with current date time
                post.LstPublishedPostDetailsModels[postIndex].Successful = ConstantVariable.No;
                post.LstPublishedPostDetailsModels[postIndex].ErrorDetails = errorMessage;
                post.LstPublishedPostDetailsModels[postIndex].PublishedDate = DateTime.Now;

                // Update the post details to bin file
                PostlistFileManager.UpdatePost(CampaignId, post);

                // Update the used post status
                PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);
            }
        }

        /// <summary>
        ///     Update Post for successful deletion after specified time
        /// </summary>
        /// <param name="destinationUrl">Destination url</param>
        /// <param name="postId">Post Id</param>
        public void UpdatePostWithDeletion(string destinationUrl, string postId)
        {
            Thread.Sleep(1000);

            // Get the locking objects
            var updatelock = PublishScheduler.UpdatingLock.GetOrAdd(postId, _lock => new object());

            lock (updatelock)
            {
                // get the post details
                var post = PostlistFileManager.GetByPostId(CampaignId, postId);

                // get the post index where current destination present
                var postIndex = post.LstPublishedPostDetailsModels.IndexOf(
                    post.LstPublishedPostDetailsModels.FirstOrDefault(y => y.DestinationUrl == destinationUrl));

                // if post index is not present,then return
                if (postIndex == -1)
                    return;

                // Pass the proper delete post text
                post.LstPublishedPostDetailsModels[postIndex].ErrorDetails = ConstantVariable.DeletedDateText();

                // Update the post details to bin file
                PostlistFileManager.UpdatePost(CampaignId, post);

                // Update the used post status
                PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);

                // Add into success details
                GenericFileManager.AddModule(post.LstPublishedPostDetailsModels[postIndex],
                    ConstantVariable.GetPublishedSuccessDetails);
            }
        }

        /// <summary>
        ///     To apply user selected general settings for campaigns
        /// </summary>
        /// <param name="givenPostModel">post list model <see cref="PublisherPostlistModel" /></param>
        /// <returns></returns>
        public PublisherPostlistModel PerformGeneralSettings(PublisherPostlistModel givenPostModel)
        {
            // Get the deep clone of the post list
            var postModelWithGeneralSettings = givenPostModel.DeepClone();
            if (postModelWithGeneralSettings.ListOfSections == null && givenPostModel.ListOfSections.Count > 0)
                postModelWithGeneralSettings.ListOfSections = givenPostModel.ListOfSections;
            #region Fetch Post media list

            // is user need to get a random single post or not
            if (GeneralSettingsModel.IsChooseSingleRandomImageChecked)
            {
                // Validate media list atleast contains a single post
                if (givenPostModel.MediaList.Count > 0)
                {
                    // get a random number
                    var randomNumber = RandomUtilties.GetRandomNumber(postModelWithGeneralSettings.MediaList.Count - 1);

                    // Fetch the media
                    postModelWithGeneralSettings.MediaList = new ObservableCollection<string>
                        {givenPostModel.MediaList[randomNumber]};
                }
            }
            // If user need first image 
            else if (GeneralSettingsModel.IsChooseOnlyFirstImageChecked)
            {
                // Validate media list atleast contains a single post
                if (givenPostModel.MediaList.Count > 0)
                    //Get the first image
                    postModelWithGeneralSettings.MediaList = new ObservableCollection<string>
                        {givenPostModel.MediaList[0]};
            }
            // If user needs select between random no of images
            else if (GeneralSettingsModel.IsChooseBetweenChecked)
            {
                // Validate media list atleast contains a single post
                if (givenPostModel.MediaList.Count > 0)
                {
                    // get the random no of counts for fetching medias
                    var randomNumber = RandomUtilties.GetRandomNumber(GeneralSettingsModel.ChooseBetween.EndValue,
                        GeneralSettingsModel.ChooseBetween.StartValue);
                    if (randomNumber < givenPostModel.MediaList.Count)
                    {
                        // shuffle the image media lists
                        givenPostModel.MediaList.Shuffle();
                        postModelWithGeneralSettings.MediaList =
                            new ObservableCollection<string>(givenPostModel.MediaList.Take(randomNumber));
                    }
                }
            }

            // In socinator we are appending "_SOCINATORIMAGE.jpg" text for video url, before publishing we need to remove those constant text
            var removeVideoExtension = new ObservableCollection<string>();

            // Removing extra added media lists
            foreach (var media in postModelWithGeneralSettings.MediaList)
                removeVideoExtension.Add(media.Replace(ConstantVariable.VideoToImageConvertFileName, string.Empty));

            postModelWithGeneralSettings.MediaList = removeVideoExtension;

            DownloadMediaFiles(postModelWithGeneralSettings);

            #endregion

            #region Macro Substitution

            // Substitute the macros for a post descriptions
            postModelWithGeneralSettings.PostDescription =
                MacrosHelper.SubstituteMacroValues(postModelWithGeneralSettings.PostDescription);

            #endregion

            #region Spin Text

            // check whether post description is null or not
            if (string.IsNullOrEmpty(postModelWithGeneralSettings.PostDescription))
                postModelWithGeneralSettings.PostDescription = string.Empty;
            if (string.IsNullOrEmpty(postModelWithGeneralSettings.PostDescription) 
                && (postModelWithGeneralSettings.scrapePostModel.IsScrapeGoogleImgaes 
                && postModelWithGeneralSettings.scrapePostModel.IsUseFileNameAsDescription
                || postModelWithGeneralSettings.PostDetailModel.IsMultipleImagePost 
                && postModelWithGeneralSettings.PostDetailModel.IsUseFileNameAsDescription) 
                && postModelWithGeneralSettings.MediaList.Count > 0)
            {
                postModelWithGeneralSettings.PostDescription = new FileInfo(postModelWithGeneralSettings.MediaList.FirstOrDefault()).Name;
            }
            // Substitute the spin text for post descriptions
            if(!string.IsNullOrEmpty(postModelWithGeneralSettings.PostDescription) && postModelWithGeneralSettings.IsSpinTax)
                postModelWithGeneralSettings.PostDescription = SpinTexHelper.GetSpinText(postModelWithGeneralSettings.PostDescription);

            //spintax for title
            if (!string.IsNullOrEmpty(postModelWithGeneralSettings.PublisherInstagramTitle) && postModelWithGeneralSettings.IsSpinTax)
                postModelWithGeneralSettings.PublisherInstagramTitle = SpinTexHelper.GetSpinText(postModelWithGeneralSettings.PublisherInstagramTitle);
            #endregion

            #region Remove link

            // Check user need to remove the links from post descriptions
            if (GeneralSettingsModel.IsRemoveLinkFromPostsChecked)
                // Call to remove urls from post descriptions
                postModelWithGeneralSettings.PostDescription = Utilities.RemoveUrls(givenPostModel.PostDescription);

            #endregion

            #region Shorten Url

            // Check user needs to make shorten url for long url
            if (OtherConfiguration.IsShortenURLsChecked)
                // call to replace the long url to shorten rul by using Bitly 
                postModelWithGeneralSettings.PostDescription =
                    Utilities.ReplaceWithShortenUrl(postModelWithGeneralSettings.PostDescription);

            #endregion

            #region Adding Signature

            // Check user needs to append signature to post descriptions 
            if (OtherConfiguration.IsEnableSignatureChecked)
                // Append the post signatures
                postModelWithGeneralSettings.PostDescription = postModelWithGeneralSettings.PostDescription + "\r\n" +
                                                               OtherConfiguration.SignatureText;

            #endregion


            return postModelWithGeneralSettings;
        }

        /// <summary>
        ///     To stop the current running jobs
        /// </summary>
        public void Stop()
        {
            try
            {
                //Cancell the cancellation token sources
                CurrentJobCancellationToken.Cancel();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     Apply the delay for next publish
        /// </summary>
        public void DelayBeforeNextPublish()
        {
            // Fetching delay minutes count
            var delay = RandomUtilties.GetRandomNumber(60, 30);

            if (JobConfigurations.IsDelayPostChecked)
            {
                delay = RandomUtilties.GetRandomNumber(JobConfigurations.DelayBetweenEachPost.EndValue, JobConfigurations.DelayBetweenEachPost.StartValue);
                GlobusLogHelper.log.Info(Log.DelayBetweenMultiPost, AccountModel.AccountBaseModel.AccountNetwork,
                AccountModel.AccountBaseModel.UserName, delay);
                Thread.Sleep(TimeSpan.FromMinutes(delay));
            }
            else
            {
                GlobusLogHelper.log.Info(Log.DelayBetweenPublishing, AccountModel.AccountBaseModel.AccountNetwork,
                AccountModel.AccountBaseModel.UserName, delay);

                // Apply the delay to current campaign specifically for current account to next post in seconds
                Thread.Sleep(TimeSpan.FromSeconds(delay));
            }



        }

        /// <summary>
        ///     Apply the delay for next publish
        /// </summary>
        public void DelayBeforeNextAccountPublish()
        {
            // Fetching delay minutes count
            var delay = RandomUtilties.GetRandomNumber(JobConfigurations.DelayBetweenPost.EndValue,
                JobConfigurations.DelayBetweenPost.StartValue);

            GlobusLogHelper.log.Info(Log.DelayBetweenAccountPublishing, AccountModel.AccountBaseModel.AccountNetwork,
                AccountModel.AccountBaseModel.UserName, delay);

            // Apply the delay to current campaign specifically for current account to next post in seconds
            Thread.Sleep(TimeSpan.FromMinutes(delay));
        }

        /// <summary>
        ///     Make a delay between every x to y posts for specified minutes
        /// </summary>
        public void DelayBetweenMultiPublish()
        {
            // Fetching delay minute count
            var delay = RandomUtilties.GetRandomNumber(JobConfigurations.DelayBetween.EndValue,
                JobConfigurations.DelayBetween.StartValue);

            GlobusLogHelper.log.Info(Log.DelayBetweenMultiPost, AccountModel.AccountBaseModel.AccountNetwork,
                AccountModel.AccountBaseModel.UserName, delay);

            // Apply the delay to current campaign specifically for current account to next post in minutes
            Thread.Sleep(delay * 1000 * 60);
        }


        protected void DownloadMediaFiles(PublisherPostlistModel postDetails)
        {
            try
            {
                //if (postDetails.MediaList.Any(x => x.Contains("https://") || x.Contains("http://")))
                //    return;
                var downloadLock = PublishScheduler.DownloadLock.GetOrAdd(postDetails.PostId, _lock => new object());
                RemovePreviousDirectory();

                var folderPath = $@"{ConstantVariable.MediaTempFolder}\[{DateTime.Now:MM-dd-yyyy}]";

                DirectoryUtilities.CreateDirectory(folderPath);

                var mediaCount = postDetails.MediaList.Count;

                foreach (var media in postDetails.MediaList.DeepCloneObject())
                    lock (downloadLock)
                    {
                        var fileName = $"{folderPath}\\{postDetails.CampaignId}_{postDetails.PostId}_{mediaCount--}" +
                                       $"{MediaUtilites.GetFileExtension(media)}";

                        if (!media.StartsWith("https://") && !media.StartsWith("http://") && !string.IsNullOrWhiteSpace(media))
                        {
                            FileInfo fi = new FileInfo(fileName);
                            var extension = fi.Extension;
                            FileInfo mediaExtension = new FileInfo(media);
                            var mediaExtnsn = mediaExtension.Extension;
                            if (string.IsNullOrEmpty(extension) && !string.IsNullOrEmpty(mediaExtnsn))
                                fileName = fileName + mediaExtnsn;
                        }
                        else
                        {
                            var MediaExtension = Utilities.GetFileExtensonFromRemoteUrl(media);
                            if (!MediaExtension.Contains("Error"))
                                fileName = $"{fileName}.{MediaExtension}";
                        }

                        //if (media.Contains("https://") && media.Contains("http://"))
                        //    continue;

                        if (DirectoryUtilities.CheckExistingFie(fileName))
                        {
                            postDetails.MediaList[postDetails.MediaList.IndexOf(media)] = fileName;
                            continue;
                        }

                        if (MediaUtilites.DownloadMediaFromUrl(media, fileName))
                        {
                            postDetails.MediaList[postDetails.MediaList.IndexOf(media)] = fileName;
                        }
                        else
                        {
                            postDetails.CanPostForNetwork = false;
                            return;
                        }
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void RemovePreviousDirectory()
        {
            try
            {
                var lstTemprorayFolderList = DirectoryUtilities.GetSubDirectories(ConstantVariable.MediaTempFolder);

                lstTemprorayFolderList.RemoveAll(x => x.Contains($@"[{DateTime.Now:MM-dd-yyyy}]") ||
                                                      x.Contains(
                                                          $@"[{DateTime.Now.AddDays(-1):MM-dd-yyyy}]"));

                DirectoryUtilities.DeleteFolder(lstTemprorayFolderList, true);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        protected bool ValidateInstagramPosts(PublisherPostlistModel postDetails)
        {
            try
            {
                if (postDetails.MediaList.Count == 0 && string.IsNullOrEmpty(postDetails.ShareUrl))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName
                        , "", $"Post with Id {postDetails.PostId} dosen't contain any media! Failed to publish");
                    return false;
                }

                if (postDetails.MediaList.Any(x => x.StartsWith("https://") ||
                                                   postDetails.MediaList.Any(y => y.StartsWith("http://"))))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName
                        , "",
                        $"Post with Id {postDetails.PostId} has media in \"Incorrect Format\"! Failed to publish");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return true;
        }

        #endregion
    }
}