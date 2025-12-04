#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommonServiceLocator;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using FluentScheduler;

#endregion

namespace DominatorHouseCore.Process
{
    public static class PublishScheduler
    {
        #region Properties

        /// <summary>
        ///     To specify the Campaign Cancellation Token with campaigns
        /// </summary>
        public static Dictionary<string, CancellationTokenSource> CampaignsCancellationTokens { get; set; }
            = new Dictionary<string, CancellationTokenSource>();

        /// <summary>
        ///     To specify the campaign with their running count
        /// </summary>
        public static ConcurrentDictionary<string, int> AttachedActionCounts { get; set; } =
            new ConcurrentDictionary<string, int>();

        /// <summary>
        ///     To block more running campaign for an actions
        /// </summary>
        public static ConcurrentDictionary<string, LinkedList<Action>> PublisherActionList { get; set; }
            = new ConcurrentDictionary<string, LinkedList<Action>>();

        /// <summary>
        ///     To Specify the scheduled list id of campaigns
        /// </summary>
        public static List<string> PublisherScheduledList { get; set; } = new List<string>();

        /// <summary>
        ///     To used in <see cref="PublisherJobProcess" /> for updating success or failed post details
        /// </summary>
        public static ConcurrentDictionary<string, object> UpdatingLock { get; set; } =
            new ConcurrentDictionary<string, object>();

        public static ConcurrentDictionary<string, object> DownloadLock { get; set; } =
            new ConcurrentDictionary<string, object>();

        /// <summary>
        ///     To used in <see cref="PublisherJobProcess" /> getting post model
        /// </summary>
        public static ConcurrentDictionary<string, object> GetPostsForPublishing { get; set; } =
            new ConcurrentDictionary<string, object>();

        #endregion

        /// <summary>
        ///     Increasing running count of the campaign
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        public static void IncreasePublishingCount(string campaignId)
        {
            try
            {
                // Check whether campaign Id already present or not, If its not present add with zero 
                if (!AttachedActionCounts.ContainsKey(campaignId)) AttachedActionCounts.GetOrAdd(campaignId, 0);

                // Get the already saved running count
                var runningCount = AttachedActionCounts[campaignId];
                ++runningCount;

                // Update the action count with new value
                AttachedActionCounts.AddOrUpdate(campaignId, runningCount, (id, count) =>
                {
                    if (count < 0)
                        // ReSharper disable once RedundantAssignment
                        count = 0;
                    count = runningCount;
                    return count;
                });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     Decrease the running count for a campaign
        /// </summary>
        /// <param name="campaignId"></param>
        public static void DecreasePublishingCount(string campaignId)
        {
            try
            {
                // Check whether campaign Id already present or not
                if (!AttachedActionCounts.ContainsKey(campaignId))
                    return;

                // If its present reduce by 1 
                var runningCount = AttachedActionCounts[campaignId];
                --runningCount;

                // And Update the recent value
                AttachedActionCounts.AddOrUpdate(campaignId, runningCount, (id, count) =>
                {
                    try
                    {
                        if (count <= 0)
                            count = 0;
                        count = runningCount;
                        return count;
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        ex.DebugLog();
                    }

                    return count;
                });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     To added actions after specific no of actions are already running with a particular account
        /// </summary>
        /// <param name="campaignAndAccountId">Camapign Id and Account Id</param>
        /// <param name="action">Action which is going to take place</param>
        public static void AddPublisherAction(string campaignAndAccountId, Action action)
        {
            try
            {
                // Check given campaign Id or account Id combination present or not
                if (PublisherActionList.ContainsKey(campaignAndAccountId))
                {
                    // If its present and get action list
                    var actionCollection = PublisherActionList[campaignAndAccountId];

                    // And Append into last
                    actionCollection.AddLast(action);

                    // Update the action list
                    PublisherActionList.AddOrUpdate(campaignAndAccountId, actionCollection, (id, actions) =>
                    {
                        try
                        {
                            if (actions == null)
                                throw new ArgumentNullException(nameof(actions));
                            actions = actionCollection;
                            return actions;
                        }
                        catch (ArgumentNullException ex)
                        {
                            ex.DebugLog();
                        }

                        return actions;
                    });
                }
                else
                {
                    // If its not present , create a new action 
                    var list = new LinkedList<Action>();

                    // and as Head of Linked list
                    list.AddFirst(action);

                    // Add to concurrent list
                    PublisherActionList.GetOrAdd(campaignAndAccountId, list);
                }
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
        ///     Execute the action for a campaign with specific account
        /// </summary>
        /// <param name="campaignAndAccountId">Campaign Id with account Id</param>
        public static void RunAndRemovePublisherAction(string campaignAndAccountId)
        {
            try
            {
                // Check whether action list contails campaign Id or account Id
                if (!PublisherActionList.ContainsKey(campaignAndAccountId))
                    return;

                // Get the actions collection for a given Index
                var actionCollection = PublisherActionList[campaignAndAccountId];

                // Fetching a first element from an action lsit
                var action = actionCollection.First();

                // After fetching an action remove from action collection
                actionCollection.RemoveFirst();

                // After remove update the action list
                PublisherActionList.AddOrUpdate(campaignAndAccountId, actionCollection, (id, actions) =>
                {
                    if (actions == null)
                        throw new ArgumentNullException(nameof(actions));
                    actions = actionCollection;
                    return actions;
                });

                // Invoking the actions
                action.Invoke();
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
        ///     Start publishing posts
        /// </summary>
        /// <param name="campaignStatusModel">Campaign full status model</param>
        public static void StartPublishingPosts(PublisherCampaignStatusModel campaignStatusModel)
        {
            try
            {
                // create a new cancellation token source
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var currentCampaignsCancallationToken = new CancellationTokenSource();

                // If CampaignsCancellationTokens dictionary doesnt contains for current campaign, add to with proper campaign Id
                if (!CampaignsCancellationTokens.ContainsKey(campaignStatusModel.CampaignId))
                    CampaignsCancellationTokens.Add(campaignStatusModel.CampaignId, currentCampaignsCancallationToken);
                else
                    // If its already present fetch that cancellation token
                    currentCampaignsCancallationToken = CampaignsCancellationTokens[campaignStatusModel.CampaignId];

                // added delay so that source file path could be added into the bin before this publishing execution
                Task.Delay(TimeSpan.FromSeconds(20)).Wait(currentCampaignsCancallationToken.Token);

                // Get the post fetcher details
                var publisherPostFetchModel =
                    genericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == campaignStatusModel.CampaignId);

                // Get the success published details
                var publishedDetails = genericFileManager
                    .GetModuleDetails<PublishedPostDetailsModel>(ConstantVariable.GetPublishedSuccessDetails)
                    .Where(x => x.CampaignId == campaignStatusModel.CampaignId).ToList();

                // Filter the success published details with destination url
                var usedDestination = publishedDetails.Select(x => x.DestinationUrl);

                // Get the advanced settings for current campaign Id
                var advancedSettings =
                    genericFileManager
                        .GetModuleDetails<GeneralModel>(
                            ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                        .FirstOrDefault(x => x.CampaignId == campaignStatusModel.CampaignId) ??
                    new GeneralModel();

                var runningCount = 0;

                // Attached accounts contains campaign Id, then get the already running count
                if (AttachedActionCounts.ContainsKey(campaignStatusModel.CampaignId))
                    runningCount = AttachedActionCounts[campaignStatusModel.CampaignId];

                // Increase the running count
                IncreasePublishingCount(campaignStatusModel.CampaignId);

                #region Assigning Destinations with posts

                #region Initializations

                var accountsWithDestinations =
                    new ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>>();

                //Get the general settings from bin files
                var generalSettingsModel = genericFileManager.GetModuleDetails<GeneralModel>
                                                   (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                                               .FirstOrDefault(x => x.CampaignId == campaignStatusModel.CampaignId) ??
                                           new GeneralModel();

                var accountIds = new SortedSet<string>();

                var allDestination = new Queue<PublisherDestinationDetailsModel>();

                var accountsWithNetworks = new Dictionary<string, SocialNetworks>();

                var totalDestinationCount = 0;

                // To specify deleted destination, like suppose while making campaign with 10 destination then after some time 5 destination
                var deletedDestinationCount = 0;

                var postsMaximumDestinationCount = campaignStatusModel.TotalRandomDestination;

                var postsAccountDestinationLimits = campaignStatusModel.MinRandomDestinationPerAccount;

                #endregion

                #region Gathering details about all selected destinations

                // Iterate all selected destinations
                publisherPostFetchModel?.SelectedDestinations.ToList().ForEach(destinationId =>
                {
                    // Get destination details
                    var binFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();
                    var destinationDetails = binFileHelper.GetSingleDestination(destinationId);

                    // If destination is aleady deleted, process will give null from above statement, if its null increase destination count
                    if (destinationDetails == null)
                        deletedDestinationCount++;

                    if (!generalSettingsModel.IsStopRandomisingDestinationsOrder)
                        destinationDetails?.DestinationDetailsModels.Shuffle();

                    #region Adding destinations

                    destinationDetails?.DestinationDetailsModels.ForEach(x =>
                    {
                        // If campaign saved remove already used destination, then remove used destinations
                        if (advancedSettings.IsDeselectUsedDestination && usedDestination.Contains(x.DestinationUrl))
                            return;

                        ++totalDestinationCount;

                        accountIds.Add(x.AccountId);

                        if (!accountsWithNetworks.ContainsKey(x.AccountId))
                            accountsWithNetworks.Add(x.AccountId, x.SocialNetworks);

                        if (!campaignStatusModel.SendOnePostForEachDestination &&
                            campaignStatusModel.IsTakeRandomDestination && postsAccountDestinationLimits > 0)
                        {
                            var currentAccountQueue = accountsWithDestinations.GetOrAdd(x.AccountId,
                                queue => new Queue<PublisherDestinationDetailsModel>());
                            currentAccountQueue.Enqueue(x);
                        }
                        else
                        {
                            allDestination.Enqueue(x);
                        }
                    });

                    #endregion
                });

                #endregion

                // Check any destinations has been deleted
                if (deletedDestinationCount > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, campaignStatusModel.CampaignName,
                        "LangKeyPublisher".FromResourceDictionary(),
                        string.Format("LangKeyNDestinationsDeletedFromCampaign".FromResourceDictionary(),
                            deletedDestinationCount, publisherPostFetchModel?.SelectedDestinations.Count,
                            campaignStatusModel.CampaignName));

                ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>> destinations;

                #region Random Destinations

                if (campaignStatusModel.IsTakeRandomDestination)
                {
                    // Check whether total destination is zero 
                    if (campaignStatusModel.TotalRandomDestination == 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social,
                            campaignStatusModel.CampaignName, "LangKeyPublisher".FromResourceDictionary(),
                            string.Format("LangKeyCampaignHasZeroAsMaxPublishingCount".FromResourceDictionary(),
                                campaignStatusModel.CampaignName));
                        return;
                    }

                    destinations = postsAccountDestinationLimits > 0
                        ? AssignPostsToDestinationWithAccountLimit(accountsWithDestinations, accountIds,
                            totalDestinationCount, campaignStatusModel.CampaignId, campaignStatusModel.CampaignName,
                            postsMaximumDestinationCount, postsAccountDestinationLimits)
                        : AssignPostsToDestinationWithNoAccountLimit(allDestination, accountIds, totalDestinationCount,
                            campaignStatusModel.CampaignId, campaignStatusModel.CampaignName,
                            postsMaximumDestinationCount);
                }

                #endregion

                #region All Destinations

                else
                {
                    destinations = AssignPostToSelectAllDestination(allDestination, accountIds, totalDestinationCount,
                        campaignStatusModel.CampaignId, campaignStatusModel.CampaignName, postsMaximumDestinationCount,
                        campaignStatusModel.SendOnePostForEachDestination);
                }

                #endregion

                if (destinations == null)
                    return;

                #region Assign and start publishing


                foreach (var destination in destinations)
                {
                    var accountsNetwork = accountsWithNetworks[destination.Key];

                    // Check whether current accounts network present or not
                    if (!SocinatorInitialize.IsNetworkAvailable(accountsNetwork))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social,
                            campaignStatusModel.CampaignName, "LangKeyPublisher".FromResourceDictionary(),
                            string.Format("LangKeyNoPermissionToRunNetworkPurchaseIt".FromResourceDictionary(),
                                accountsNetwork));
                        continue;
                    }

                    // Get the publisher Job process
                    var publisherJobProcess = PublisherInitialize
                        .GetPublisherLibrary(accountsNetwork)
                        .GetPublisherCoreFactory()
                        .PublisherJobFactory.Create(campaignStatusModel.CampaignId, campaignStatusModel.CampaignName,
                            destination.Key, accountsNetwork, destination.Value,
                            currentCampaignsCancallationToken);

                    #region Wait to start actions

                    // Check whether wait to start action after x actions are running parallely
                    if (advancedSettings.IsWaitToStartAction)
                    {
                        if (runningCount >= advancedSettings.JobProcessRunningCount)
                            AddPublisherAction(
                                $"{campaignStatusModel.CampaignId}-{destination.Key}", () =>
                                    publisherJobProcess.StartPublishingPosts(!advancedSettings
                                        .IsRunSingleAccountPerCampaign));
                        else
                            // Otherwise start calling
                            publisherJobProcess.StartPublishingPosts(!advancedSettings.IsRunSingleAccountPerCampaign);
                    }

                    #endregion

                    #region Without waiting to next action count

                    else if (publisherJobProcess.JobConfigurations.IsAccountDelayChecked)
                    {
                        // If there is no settings for wait to start, then call directly publishing methods
                        publisherJobProcess.StartPublishingPosts(advancedSettings.IsRunSingleAccountPerCampaign);
                    }

                    else if (publisherJobProcess.JobConfigurations.IsDelayPostChecked)
                    {
                        publisherJobProcess.StartPublishingPosts(advancedSettings.IsRunSingleAccountPerCampaign);
                    }

                    else
                    {
                        publisherJobProcess.StartPublishingPosts(!advancedSettings.IsRunSingleAccountPerCampaign);

                    }

                    #endregion
                }

                #endregion

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
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void StartPublishingPosts(PublisherPostlistModel post)
        {
            try
            {
                // Get the campaign Details
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var campaignDetails =
                    PublisherInitialize.GetInstance.GetSavedCampaigns().ToList();

                // Get the specific campaign Details
                var campaignStatusModel = campaignDetails.FirstOrDefault(x => x.CampaignId == post.CampaignId);

                if (campaignStatusModel == null)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, campaignStatusModel?.CampaignName ?? "null",
                        "LangKeyPublisher".FromResourceDictionary(),
                        "LangKeyPostNotRegisterWithAnyCampaign".FromResourceDictionary());
                    return;
                }

                // Validate the campaign Time
                var isStart = ValidateCampaignsTime(campaignStatusModel);

                if (!isStart)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, campaignStatusModel.CampaignName,
                        "LangKeyPublisher".FromResourceDictionary(),
                        "LangKeyPostCampaignExpired".FromResourceDictionary());
                    return;
                }

                // create a new cancellation token source
                var currentCampaignsCancallationToken = new CancellationTokenSource();

                // If CampaignsCancellationTokens dictionary doesnt contains for current campaign, add to with proper campaign Id
                if (!CampaignsCancellationTokens.ContainsKey(campaignStatusModel.CampaignId))
                    CampaignsCancellationTokens.Add(campaignStatusModel.CampaignId, currentCampaignsCancallationToken);
                else
                    // If its already present fetch that cancellation token
                    currentCampaignsCancallationToken = CampaignsCancellationTokens[campaignStatusModel.CampaignId];

                // Get he post fetcher details
                var publisherPostFetchModel =
                    genericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == campaignStatusModel.CampaignId);


                // Get the advanced settings for current campaign Id
                var advancedSettings =
                    genericFileManager
                        .GetModuleDetails<GeneralModel>(
                            ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                        .FirstOrDefault(x => x.CampaignId == campaignStatusModel.CampaignId) ??
                    new GeneralModel();

                #region Assigning Destinations with posts

                #region Initializations

                //Get the general settings from bin files
                var generalSettingsModel = genericFileManager.GetModuleDetails<GeneralModel>
                                                   (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                                               .FirstOrDefault(x => x.CampaignId == campaignStatusModel.CampaignId) ??
                                           new GeneralModel();

                var accountIds = new SortedSet<string>();

                var allDestination = new Queue<PublisherDestinationDetailsModel>();

                var accountsWithNetworks = new Dictionary<string, SocialNetworks>();


                // To specify deleted destination, like suppose while making campaign with 10 destination then after some time 5 destination
                var deletedDestinationCount = 0;

                var allDestinaionGuid = new List<string>();

                #endregion

                #region Gathering details about all selected destinations

                // Iterate all selected destinations
                publisherPostFetchModel?.SelectedDestinations.ToList().ForEach(destinationId =>
                {
                    // Get destination details
                    var binFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();
                    var destinationDetails = binFileHelper.GetSingleDestination(destinationId);

                    // If destination is aleady deleted, process will give null from above statement, if its null increase destination count
                    if (destinationDetails == null)
                        deletedDestinationCount++;

                    if (!generalSettingsModel.IsStopRandomisingDestinationsOrder)
                        destinationDetails?.DestinationDetailsModels.Shuffle();

                    #region Adding destinations

                    destinationDetails?.DestinationDetailsModels.ForEach(x =>
                    {
                        accountIds.Add(x.AccountId);

                        if (!accountsWithNetworks.ContainsKey(x.AccountId))
                            accountsWithNetworks.Add(x.AccountId, x.SocialNetworks);

                        allDestinaionGuid.Add(x.DestinationGuid);

                        allDestination.Enqueue(x);
                    });

                    #endregion
                });

                #endregion

                // Check any destinations has been deleted
                if (deletedDestinationCount > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, campaignStatusModel.CampaignName,
                        "LangKeyPublisher".FromResourceDictionary(),
                        string.Format("LangKeyNDestinationsDeletedFromCampaign".FromResourceDictionary(),
                            deletedDestinationCount, publisherPostFetchModel?.SelectedDestinations.Count,
                            campaignStatusModel.CampaignName));

                var destinations = UpdatePostDetails(campaignStatusModel.CampaignId, campaignStatusModel.CampaignName,
                    allDestination, post, allDestinaionGuid);

                if (destinations == null)
                    return;

                #region Assign and start publishing

                foreach (var destination in destinations)
                {
                    var accountsNetwork = accountsWithNetworks[destination.Key];

                    // Check whether current accounts network present or not
                    if (!SocinatorInitialize.IsNetworkAvailable(accountsNetwork))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social,
                            campaignStatusModel.CampaignName, "LangKeyPublisher".FromResourceDictionary(),
                            string.Format("LangKeyNoPermissionToRunNetworkPurchaseIt".FromResourceDictionary(),
                                accountsNetwork));
                        continue;
                    }

                    // Get the publisher Job process
                    var publisherJobProcess = PublisherInitialize
                        .GetPublisherLibrary(accountsNetwork)
                        .GetPublisherCoreFactory()
                        .PublisherJobFactory.Create(campaignStatusModel.CampaignId, campaignStatusModel.CampaignName,
                            destination.Key, accountsNetwork, destination.Value,
                            currentCampaignsCancallationToken);

                    // If there is no settings for wait to start, then call directly publishing methods
                    publisherJobProcess.StartPublishingPosts(!advancedSettings.IsRunSingleAccountPerCampaign);
                }

                #endregion

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
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region Helper functionality : To Assign Posts to Destinations

        public static ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>>
            AssignPostsToDestinationWithNoAccountLimit(
                Queue<PublisherDestinationDetailsModel> totalDestinations,
                SortedSet<string> accountId,
                int totalDestinationCount,
                string campaignId,
                string campaignName,
                int postsMaximumDestinationCount)
        {
            ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>> destinationWithPosts;


            var updatelock = GetPostsForPublishing.GetOrAdd(campaignId, _lock => new object());

            lock (updatelock)
            {
                var skipCount = 0;

                var givenDestinations = totalDestinations.ToList();

                var postsDestinations = new List<List<string>>();

                #region Split the destinations for a post

                while (true)
                {
                    #region Commented

                    // Split the destination with maximum destinations
                    //var currentPostsDestination = new List<string>();

                    //for (var initial = 0; initial < postsMaximumDestinationCount; initial++)
                    //{
                    //    if (totalDestinations.Count <= 0)
                    //        break;
                    //    // Get the destinations
                    //    var destination = totalDestinations.Dequeue();
                    //    currentPostsDestination.Add(destination.DestinationGuid);
                    //}
                    //if (currentPostsDestination.Count > 0)
                    //    // Add the splitted destinations
                    //    postsDestinations.Add(currentPostsDestination);
                    //else
                    //    break; 

                    #endregion

                    var currentPostDestination = givenDestinations.Skip(skipCount).Take(postsMaximumDestinationCount)
                        .ToList();

                    if (currentPostDestination.Count == 0) break;

                    if (currentPostDestination.Count < postsMaximumDestinationCount)
                    {
                        givenDestinations.Shuffle();

                        currentPostDestination.AddRange(givenDestinations.Except(currentPostDestination).Take
                            (postsMaximumDestinationCount - currentPostDestination.Count));
                    }


                    postsDestinations.Add(currentPostDestination.Select(x => x.DestinationGuid).ToList());

                    skipCount += postsMaximumDestinationCount;
                }

                #endregion

                destinationWithPosts =
                    SubstitudePoststoDestinations(campaignId, campaignName, givenDestinations, postsDestinations);
            }

            return destinationWithPosts;
        }

        public static ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>>
            AssignPostsToDestinationWithAccountLimit
            (ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>> totalDestinations,
                SortedSet<string> accountId,
                int totalDestinationCount,
                string campaignId,
                string campaignName,
                int postsMaximumDestinationCount,
                int postsAccountDestinationLimits)
        {
            ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>> destinationWithPosts;

            var updatelock = GetPostsForPublishing.GetOrAdd(campaignId, _lock => new object());

            lock (updatelock)
            {
                var accountsDestinations = new List<PublisherDestinationDetailsModel>();

                var accounts = accountId.ToList();

                accounts.Shuffle();

                var postsDestinations = new List<List<string>>();

                #region Split the destinations for a post

                while (true)
                {
                    // Split the destination with maximum destinations
                    var currentPostsDestination = new List<KeyValuePair<string, string>>();

                    foreach (var account in accounts)
                    {
                        // Check all destination already partcipate for splitting
                        if (accountsDestinations.Count >= totalDestinationCount)
                            break;

                        // current split already assigned reached accounts limits
                        if (currentPostsDestination.Count >= postsMaximumDestinationCount)
                            break;

                        // Get the destinations queue   
                        var currentAccountQueue = totalDestinations[account];

                        for (var accountLimit = 0; accountLimit < postsAccountDestinationLimits; accountLimit++)
                        {
                            // current split already assigned reached accounts limits
                            if (currentPostsDestination.Count >= postsMaximumDestinationCount)
                                break;

                            if (currentAccountQueue.Count == 0)
                                break;
                            var destination = currentAccountQueue.Dequeue();
                            accountsDestinations.Add(destination);
                            currentPostsDestination.Add(
                                new KeyValuePair<string, string>(account, destination.DestinationGuid));
                        }
                    }

                    if (currentPostsDestination.Count > 0)
                        postsDestinations.Add(currentPostsDestination.Select(x => x.Value).ToList());
                    // Add the splitted destinations
                    else
                        break;
                }

                if (postsDestinations.Any(x => x.Count == postsMaximumDestinationCount))
                    postsDestinations.RemoveAll(x => x.Count < postsMaximumDestinationCount);

                #endregion

                destinationWithPosts =
                    SubstitudePoststoDestinations(campaignId, campaignName, accountsDestinations, postsDestinations);
            }

            return destinationWithPosts;
        }

        private static ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>>
            SubstitudePoststoDestinations(
                string campaignId,
                string campaignName,
                IReadOnlyCollection<PublisherDestinationDetailsModel> givenDestinations,
                List<List<string>> postsDestinations)
        {
            if (givenDestinations.Count == 0)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, campaignName,
                    "LangKeyPublisher".FromResourceDictionary(),
                    "LangKeyNoUniqueDestinationsPresent".FromResourceDictionary());
                return new ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>>();
            }

            var destinationWithPosts = new ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>>();

            try
            {
                // Getting all pending post lists
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var pendingPostList = PostlistFileManager.GetAll(campaignId)
                    .Where(x => x.PostQueuedStatus == PostQueuedStatus.Pending).ToList();

                // Checking, If no more post available
                if (!pendingPostList.Any())
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, campaignName,
                        "LangKeyPublisher".FromResourceDictionary(),
                        string.Format("LangKeyNoUniquePostsAvailableForCampaign".FromResourceDictionary(),
                            campaignName));
                    return null;
                }

                //Get the general settings from bin files
                var generalSettingsModel = genericFileManager.GetModuleDetails<GeneralModel>
                                                   (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                                               .FirstOrDefault(x => x.CampaignId == campaignId) ?? new GeneralModel();

                // Validate the toaster notifications is needed
                if (ConstantVariable.IsToasterNotificationNeed)
                    // If user needs to notify when postlists going lesser than specified post, then trigger a notifications
                    if (pendingPostList.Count < generalSettingsModel.TriggerNotificationCount &&
                        generalSettingsModel.TriggerNotificationCount > 0)
                        ToasterNotification.ShowInfomation(string.Format(
                            "LangKeyCampaignHasNPendingPosts".FromResourceDictionary(), campaignName,
                            pendingPostList.Count));

                // Check whether needs to shuffle postlist order
                if (generalSettingsModel.IsChooseRandomPostsChecked)
                    pendingPostList.Shuffle();


                // Validate whether all destinations contains posts or not
                if (pendingPostList.Count < postsDestinations.Count)
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, campaignName,
                        "LangKeyPublisher".FromResourceDictionary(),
                        "LangKeyPendingPostsLesserThanRequiredRandDestinationCount".FromResourceDictionary());

                #region Assigning the Posts to Destinations

                #region Commented

                //// Get the posts
                //var post = pendingPostList[count];

                //// Check whether count exceeds destinations
                //if (count >= postsDestinations.Count)
                //    break;

                //// Get the destination
                //var destinations = postsDestinations[count]; 

                #endregion

                var post = pendingPostList.First();

                var destinations = GetOrAddProcessedDestination(campaignId,
                    givenDestinations.Select(x => x.DestinationGuid).ToList(), postsDestinations.FirstOrDefault().Count,
                    postsDestinations);

                UpdatePostDetails(campaignId, campaignName, givenDestinations, destinationWithPosts, post,
                    destinations);

                #endregion

                // update stats to publisher default view
                PublisherInitialize.GetInstance.UpdatePostStatus(campaignId);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return destinationWithPosts;
        }

        public static ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>>
            AssignPostToSelectAllDestination(
                Queue<PublisherDestinationDetailsModel> totalDestinations,
                SortedSet<string> accountId,
                int totalDestinationCount,
                string campaignId,
                string campaignName,
                int postsMaximumDestinationCount,
                bool isWhenPublishingSendOnePostChecked)
        {
            var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            var destinationWithPosts = new ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>>();

            var updatelock = GetPostsForPublishing.GetOrAdd(campaignId, _lock => new object());

            lock (updatelock)
            {
                var givenDestinations = totalDestinations.ToList();

                if (givenDestinations.Count == 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, campaignName,
                        "LangKeyPublisher".FromResourceDictionary(),
                        "LangKeyNoUniqueDestinationsPresent".FromResourceDictionary());
                    return new ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>>();
                }

                var alldestinations = givenDestinations.Select(x => x.DestinationGuid).ToList();

                var accounts = accountId.ToList();

                accounts.Shuffle();

                var postsDestinations = new List<List<string>> { alldestinations };

                #region Split the destinations for a post

                try
                {
                    // Getting all pending post lists
                    var pendingPostList = PostlistFileManager.GetAll(campaignId)
                        .Where(x => x.PostQueuedStatus == PostQueuedStatus.Pending).ToList();

                    var publishedPost = PostlistFileManager.GetAll(campaignId)
                        .Where(x => x.PostQueuedStatus == PostQueuedStatus.Published).ToList();

                    var errorPosts = publishedPost.Where(x => x.LstPublishedPostDetailsModels.Any(y => y.Successful == "No")).ToList();

                    pendingPostList.AddRange(errorPosts);

                    // Get the expire post counts
                    var expiredPosts =
                        pendingPostList.Where(x =>
                            x.PublisherPostSettings.GeneralPostSettings.IsExpireDate &&
                            x.PublisherPostSettings.GeneralPostSettings.ExpireDate < DateTime.Today).ToList();

                    var expiredPostCount = expiredPosts.Count;

                    pendingPostList = pendingPostList.Except(expiredPosts).ToList();

                    // Checking, If no more post available
                    if (!pendingPostList.Any())
                    {
                        if (expiredPostCount > 0)
                            ToasterNotification.ShowInfomation(string.Format(
                                "LangKeyCampaignHasNExpiredPosts".FromResourceDictionary(), campaignName,
                                expiredPostCount));
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, campaignName,
                            "LangKeyPublisher".FromResourceDictionary(),
                            string.Format("LangKeyNoUniquePostsAvailableForCampaign".FromResourceDictionary(),
                                campaignName));
                        return null;
                    }

                    //Get the general settings from bin files
                    var generalSettingsModel = genericFileManager.GetModuleDetails<GeneralModel>
                                                   (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks
                                                       .Social))
                                                   .FirstOrDefault(x => x.CampaignId == campaignId) ??
                                               new GeneralModel();

                    // Validate the toaster notifications is needed
                    if (ConstantVariable.IsToasterNotificationNeed)
                        // If user needs to notify when postlists going lesser than specified post, then trigger a notifications
                        if (pendingPostList.Count < generalSettingsModel.TriggerNotificationCount &&
                            generalSettingsModel.TriggerNotificationCount > 0)
                            ToasterNotification.ShowInfomation(string.Format(
                                "LangKeyCampaignHasNPendingPosts".FromResourceDictionary(), campaignName,
                                pendingPostList.Count));

                    // Check whether needs to shuffle postlist order
                    if (generalSettingsModel.IsChooseRandomPostsChecked)
                        pendingPostList.Shuffle();

                    // Validate whether all destinations contains posts or not
                    if (pendingPostList.Count < postsDestinations.Count)
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, campaignName,
                            "LangKeyPublisher".FromResourceDictionary(),
                            "LangKeyPendingPostsLesserThanRequiredRandDestinationCount".FromResourceDictionary());

                    #region Assigning the Posts to Destinations

                    if (isWhenPublishingSendOnePostChecked)
                    {
                        //for (var count = 0; count < pendingPostList.Count; count++)
                        //{
                        //    // Get the posts
                        //    var post = pendingPostList[count];

                        //    // Check whether count exceeds destinations
                        //    if (count >= alldestinations.Count)
                        //        break;

                        //    // Get the destination
                        //    var destination = alldestinations[count];

                        //    var destinations = new List<string> { destination };

                        //    UpdatePostDetails(campaignId, campaignName, givenDestinations, destinationWithPosts, post, destinations);

                        //}

                        var post = pendingPostList.First();

                        var destinations = GetOrAddProcessedDestination(campaignId, alldestinations, 1, null);

                        UpdatePostDetails(campaignId, campaignName, givenDestinations, destinationWithPosts, post,
                            destinations);
                    }
                    else
                    {
                        // Get the posts
                        var post = pendingPostList.First();

                        // Update a post to all destinations
                        destinationWithPosts = UpdatePostDetails(campaignId, campaignName, givenDestinations, post,
                            alldestinations);
                    }

                    #endregion

                    // update stats to publisher default view
                    PublisherInitialize.GetInstance.UpdatePostStatus(campaignId);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion
            }

            return destinationWithPosts;
        }

        private static ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>> UpdatePostDetails(
            string campaignId,
            string campaignName,
            IReadOnlyCollection<PublisherDestinationDetailsModel> givenDestinations,
            PublisherPostlistModel post,
            List<string> destinations)
        {
            var destinationWithPosts = new ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>>();

            var locationIds = post.LstPublishedPostDetailsModels.Where(x => x.Successful == "No").Select(x => x.DestinationUrl).ToList();

            var postDestinations = new List<PublisherDestinationDetailsModel>();

            if (locationIds.Count != 0)
                postDestinations = givenDestinations.Where(x => locationIds.Any(y => y == x.DestinationUrl)).ToList();
            else
                postDestinations = givenDestinations.ToList();



            try
            {
                // Iterate and assign -> given post to all given destinations
                destinations.ForEach(destinationId =>
                {
                    // get the destination full details
                    var destinationDetails = postDestinations.FirstOrDefault(x => x.DestinationGuid == destinationId);

                    // check null destinations
                    if (destinationDetails == null)
                        return;

                    // Assign the current destination url in case of Own wall 
                    var currentDestinationUrl = destinationDetails.DestinationType == ConstantVariable.OwnWall
                        ? destinationDetails.AccountName
                        : destinationDetails.DestinationUrl;

                    if (post.LstPublishedPostDetailsModels.Any(x =>
                        x.DestinationUrl == currentDestinationUrl && x.Successful == "Yes" && destinationDetails.AccountId == x.AccountId))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, campaignName,
                            "LangKeyPublisher".FromResourceDictionary(),
                            string.Format("LangKeyPostAlreadyPublishedOnTheAccount".FromResourceDictionary(),
                                destinationDetails.AccountName, destinationDetails.DestinationType,
                                destinationDetails.DestinationUrl));
                    }
                    else
                    {
                        // Assign the posts
                        destinationDetails.PublisherPostlistModel = post;

                        // get the accounts queue
                        var accountsQueue = destinationWithPosts.GetOrAdd(destinationDetails.AccountId,
                            queue => new Queue<PublisherDestinationDetailsModel>());

                        // Add to queue
                        accountsQueue.Enqueue(destinationDetails);

                        if (!post.LstPublishedPostDetailsModels.Any(x => x.AccountId == destinationDetails.AccountId && x.DestinationUrl == destinationDetails.DestinationUrl))
                        {
                            // Append the post list details 
                            post.LstPublishedPostDetailsModels.Add(new PublishedPostDetailsModel
                            {
                                AccountName = destinationDetails.AccountName,
                                Destination = destinationDetails.DestinationType,
                                DestinationUrl = currentDestinationUrl,
                                Description = post.PostDescription,
                                IsPublished = ConstantVariable.Yes,
                                Successful = ConstantVariable.No,
                                PublishedDate = DateTime.Now,
                                Link = ConstantVariable.NotPublished,
                                CampaignId = campaignId,
                                CampaignName = campaignName,
                                SocialNetworks = destinationDetails.SocialNetworks,
                                AccountId = destinationDetails.AccountId,
                                ErrorDetails = ConstantVariable.NotPublished
                            });
                        }

                    }
                });

                // Mark as published one
                post.PostQueuedStatus = PostQueuedStatus.Published;

                // Calculate already tried count
                var triedCount =
                    post.LstPublishedPostDetailsModels.Count(x => x.IsPublished == ConstantVariable.Yes);

                // Calculate already success count
                var successCount =
                    post.LstPublishedPostDetailsModels.Count(x => x.Successful == ConstantVariable.Yes);

                // Update the stats
                post.PublishedTriedAndSuccessStatus = $"{triedCount}/{successCount}";

                // Checking post expire date time
                if (post.ExpiredTime == null)
                    post.PostRunningStatus = PostRunningStatus.Active;
                else
                    post.PostRunningStatus = DateTime.Now > post.ExpiredTime
                        ? PostRunningStatus.Completed
                        : PostRunningStatus.Active;

                // Update to bin file
                PostlistFileManager.UpdatePost(campaignId, post);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return destinationWithPosts;
        }

        private static List<string> GetOrAddProcessedDestination(string campaignId, List<string> totalDestination,
            int destinationCount, List<List<string>> postDestinationList)
        {
            //Get the locking objects
            var updatelock = GetPostsForPublishing.GetOrAdd(campaignId, _lock => new object());

            lock (updatelock)
            {
                if (destinationCount > 1)
                    totalDestination.Shuffle();

                List<string> unProcessedDestinations;

                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();

                var processedDestinationModel = genericFileManager
                    .GetModel<PublisherProcessedDestinationModel>(
                        $"{ConstantVariable.GetProcessedDestinationDir()}//{campaignId}.bin");

                if (!(processedDestinationModel.ListTotalDestination.Intersect(totalDestination).ToList().Count ==
                      totalDestination.Count
                      && processedDestinationModel.ListTotalDestination.Count == totalDestination.Count) ||
                    processedDestinationModel.DestinationCount != destinationCount)
                {
                    processedDestinationModel.CampaignId = campaignId;
                    processedDestinationModel.ListTotalDestination = totalDestination;
                    processedDestinationModel.ListProcessedDestination.Clear();
                    processedDestinationModel.ListSkippedDestination.Clear();
                    processedDestinationModel.DestinationCount = destinationCount;
                }

                var processdDestinaionList = processedDestinationModel.ListProcessedDestination;

                if (postDestinationList == null || postDestinationList.Count == 0)
                {
                    unProcessedDestinations = processedDestinationModel.ListTotalDestination
                        .Except(processdDestinaionList).ToList();

                    if (unProcessedDestinations.Count < destinationCount && processdDestinaionList.Count > 0)
                    {
                        if (destinationCount > 1)
                            processdDestinaionList.Shuffle();

                        unProcessedDestinations.AddRange(
                            processdDestinaionList.Take(destinationCount - unProcessedDestinations.Count));

                        processedDestinationModel.ListProcessedDestination.Clear();
                    }

                    processedDestinationModel.ListProcessedDestination.AddRange(
                        unProcessedDestinations.Take(destinationCount));
                }
                else
                {
                    if (processedDestinationModel.ListSkippedDestination.Count > 0)
                    {
                        unProcessedDestinations = processedDestinationModel.ListSkippedDestination.FirstOrDefault()
                            .DestinationGuidList;
                        processedDestinationModel.ListSkippedDestination.Remove(processedDestinationModel
                            .ListSkippedDestination.FirstOrDefault());
                    }
                    else
                    {
                        unProcessedDestinations = postDestinationList.FirstOrDefault();
                        postDestinationList.Remove(unProcessedDestinations);
                        postDestinationList.ForEach(x =>
                        {
                            processedDestinationModel.ListSkippedDestination.Add(new PostDestinationModel
                            {
                                DestinationGuidList = x
                            });
                        });
                    }
                }

                genericFileManager
                    .Save(processedDestinationModel,
                        $"{ConstantVariable.GetProcessedDestinationDir()}//{campaignId}.bin");

                return unProcessedDestinations.Take(destinationCount).ToList();
            }
        }

        private static void UpdatePostDetails(string campaignId,
            string campaignName,
            IReadOnlyCollection<PublisherDestinationDetailsModel> givenDestinations,
            ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>> destinationWithPosts,
            PublisherPostlistModel post,
            List<string> destinations)
        {
            try
            {
                // Iterate and assign -> given post to all given destinations
                destinations.ForEach(destinationId =>
                {
                    // get the destination full details
                    var destinationDetails = givenDestinations.FirstOrDefault(x => x.DestinationGuid == destinationId);

                    // check null destinations
                    if (destinationDetails == null)
                        return;

                    // Assign the current destination url in case of Own wall 
                    var currentDestinationUrl = destinationDetails.DestinationType == ConstantVariable.OwnWall
                        ? destinationDetails.AccountName
                        : destinationDetails.DestinationUrl;

                    if (post.LstPublishedPostDetailsModels.Any(x =>
                        x.DestinationUrl == currentDestinationUrl && destinationDetails.AccountId == x.AccountId))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, campaignName,
                            "LangKeyPublisher".FromResourceDictionary(),
                            string.Format("LangKeyPostAlreadyPublishedOnTheAccount".FromResourceDictionary(),
                                destinationDetails.AccountName, destinationDetails.DestinationType,
                                destinationDetails.DestinationUrl));
                        return;
                    }

                    // Assign the posts
                    destinationDetails.PublisherPostlistModel = post;

                    // get the accounts queue
                    var accountsQueue = destinationWithPosts.GetOrAdd(destinationDetails.AccountId,
                        queue => new Queue<PublisherDestinationDetailsModel>());

                    // Add to queue
                    accountsQueue.Enqueue(destinationDetails);

                    // Append the post list details 
                    post.LstPublishedPostDetailsModels.Add(new PublishedPostDetailsModel
                    {
                        AccountName = destinationDetails.AccountName,
                        Destination = destinationDetails.DestinationType,
                        DestinationUrl = currentDestinationUrl,
                        Description = post.PostDescription,
                        IsPublished = ConstantVariable.Yes,
                        Successful = ConstantVariable.No,
                        PublishedDate = DateTime.Now,
                        Link = ConstantVariable.NotPublished,
                        CampaignId = campaignId,
                        CampaignName = campaignName,
                        SocialNetworks = destinationDetails.SocialNetworks,
                        AccountId = destinationDetails.AccountId,
                        ErrorDetails = ConstantVariable.NotPublished
                    });
                });

                // Mark as published one
                post.PostQueuedStatus = PostQueuedStatus.Published;

                // Calculate already tried count
                var triedCount =
                    post.LstPublishedPostDetailsModels.Count(x => x.IsPublished == ConstantVariable.Yes);

                // Calculate already success count
                var successCount =
                    post.LstPublishedPostDetailsModels.Count(x => x.Successful == ConstantVariable.Yes);

                // Update the stats
                post.PublishedTriedAndSuccessStatus = $"{triedCount}/{successCount}";

                // Checking post expire date time
                if (post.ExpiredTime == null)
                    post.PostRunningStatus = PostRunningStatus.Active;
                else
                    post.PostRunningStatus = DateTime.Now > post.ExpiredTime
                        ? PostRunningStatus.Completed
                        : PostRunningStatus.Active;

                // Update to bin file
                PostlistFileManager.UpdatePost(campaignId, post);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        /// <summary>
        ///     Check whether start date is greater than or equal to today and check end date is already expire or not
        /// </summary>
        /// <param name="specificCampaign">Campaign Status model</param>
        /// <returns></returns>
        public static bool ValidateCampaignsTime(PublisherCampaignStatusModel specificCampaign)
        {
            var isStart = true;

            var pendingPostDetails = PostlistFileManager.GetAll(specificCampaign.CampaignId)
                        .Where(x => x.PostQueuedStatus == PostQueuedStatus.Pending).ToList();

            var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();

            var allCampaign = genericFileManager
                .GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile());

            // Get the particular campaign
            var currentCampaign = allCampaign.FirstOrDefault(x => x.CampaignId == specificCampaign.CampaignId);

            try
            {
                if (pendingPostDetails.Count == 0 && currentCampaign?.JobConfigurations?.MaxPost < 2)
                    isStart = false;
            }
            catch (Exception)
            {
                
            }

            if (pendingPostDetails.Count == 0 && !string.IsNullOrEmpty(currentCampaign?.SharePostModel?.AddFdPageUrl))
                isStart = false;

            // Check start time is equal to null or not
            if (specificCampaign.StartDate != null)
                // Compare with today
                if (!(DateTime.Now >= specificCampaign.StartDate))
                    isStart = false;
            // Check end time is equal to null or not
            if (specificCampaign.EndDate == null) return isStart;
            if (!(DateTime.Now <= specificCampaign.EndDate))
                isStart = false;

            return isStart;
        }

        /// <summary>
        ///     Stop publishing campaigns
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        public static void StopPublishingPosts(string campaignId)
        {
            try
            {
                // Call to stop already scheduled Jobs
                StopScheduledPublisher(campaignId);

                // Get the cancellation token from campaigns
                var cancellationToken = CampaignsCancellationTokens.FirstOrDefault(x => x.Key == campaignId);

                // Cancel the token
                if(cancellationToken.Value != null)
                    cancellationToken.Value.Cancel();

                // After cancelling remove the token sources from collections
                if (!CampaignsCancellationTokens.ContainsKey(campaignId)) return;
                CampaignsCancellationTokens.Remove(campaignId);
                // ReSharper disable once NotAccessedVariable
                var deletedList = new LinkedList<Action>();
                PublisherActionList.TryRemove(campaignId, out deletedList);
                DecreasePublishingCount(campaignId);
            }
            catch (Exception ex)
            {
                // Check whether campaign already started or nor
                var specificCampaign = PublisherInitialize.GetInstance.GetSavedCampaigns().ToList()
                    .FirstOrDefault(x => x.CampaignId == campaignId);
                if (specificCampaign != null)
                    ex.DebugLog($"Campaign : {specificCampaign.CampaignName} not started before!");
            }
        }

        /// <summary>
        ///     Enable the delete option for published post
        /// </summary>
        /// <param name="postDeletionModel">Deletion post models</param>
        public static void EnableDeletePost(PostDeletionModel postDeletionModel)
        {
            // Add into bin files
            var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            genericFileManager.AddModule(postDeletionModel,
                ConstantVariable.GetDeletePublisherPostModel);

            // Schedule delete post itmes
            DeletePublishedPost(postDeletionModel);
        }

        /// <summary>
        ///     Delete published post at specific time
        /// </summary>
        /// <param name="postDeletionModel"></param>
        public static void DeletePublishedPost(PostDeletionModel postDeletionModel)
        {
            // Check whether network present or not
            if (FeatureFlags.IsNetworkAvailable(postDeletionModel.Networks))
                // Add into job process
                JobManager.AddJob(() =>
                {
                    // Get the publisher Job Process factory
                    var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                    var publisherJobProcess = PublisherInitialize.GetPublisherLibrary(postDeletionModel.Networks)
                        ?.GetPublisherCoreFactory()?
                        .PublisherJobFactory.Create(postDeletionModel.CampaignId, postDeletionModel.AccountId, null,
                            null, null, false, new CancellationTokenSource());

                    // Call is delete options
                    if (!(publisherJobProcess?.DeletePost(postDeletionModel.PublishedIdOrUrl) ?? false)) return;
                    // If successfully deleted , update the details
                    publisherJobProcess.UpdatePostWithDeletion(postDeletionModel.DestinationUrl,
                        postDeletionModel.PostId);

                    // Make already deleted true
                    postDeletionModel.IsDeletedAlready = true;

                    // Get deletion model
                    var allDeletionList =
                        genericFileManager.GetModuleDetails<PostDeletionModel>(ConstantVariable
                            .GetDeletePublisherPostModel);

                    // Find the index of particular published Id
                    var index = allDeletionList.FindIndex(x =>
                        x.PublishedIdOrUrl == postDeletionModel.PublishedIdOrUrl);

                    // Update bin file objects
                    allDeletionList[index].IsDeletedAlready = true;

                    // save the updated details into bin files
                    genericFileManager.UpdateModuleDetails(allDeletionList,
                        ConstantVariable.GetDeletePublisherPostModel);
                },
                    s => s.WithName($"{postDeletionModel.CampaignId}- Delete Posts -{ConstantVariable.GetDate()}")
                        .ToRunOnceAt(postDeletionModel.DeletionTime));
        }

        /// <summary>
        ///     Publish now by campaign Id
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        public static void SchedulePublishNowByCampaign(string campaignId)
        {
            // get the all campaigns which should be present in between 
            //var campaignDetails =
            //    PublisherInitialize.GetInstance.GetSavedCampaigns().Where(x => DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate).ToList();

            // get the all campaigns
            var campaignDetails =
                PublisherInitialize.GetInstance.GetSavedCampaigns().ToList();
            // Filter with current campaign
            var specificCampaign = campaignDetails.FirstOrDefault(x => x.CampaignId == campaignId);

            // Validate the campaigns Times
            if (specificCampaign == null || !ValidateCampaignsTime(specificCampaign)) return;

            // Is Rotate day has been selected
            if (specificCampaign.IsRotateDayChecked)
            // Call to start publishing
            // SchedulePublisher(specificCampaign);
            {
                StartPublishingPosts(specificCampaign);
            }
            else
            {
                // Check whether today is selected or not
                var isCampaignSelected = specificCampaign.ScheduledWeekday.FirstOrDefault(x =>
                    x.Content == DateTime.Now.DayOfWeek.ToString() && x.IsContentSelected);
                if (isCampaignSelected == null)
                    return;
                // Call to start publishing
                // SchedulePublisher(specificCampaign);
                StartPublishingPosts(specificCampaign);
            }
        }

        /// <summary>
        ///     Todays publishing scheduler
        /// </summary>
        public static void ScheduleTodaysPublisher()
        {
            // get the all campaigns which should active 
            var campaignDetails =
                PublisherInitialize.GetInstance.GetSavedCampaigns()
                    .Where(x => x.Status == PublisherCampaignStatus.Active).ToList();
            if (campaignDetails.Count > 0)
                Task.Factory.StartNew(() =>
                {
                    // Iterate campaigns 
                    campaignDetails.ForEach(campaign =>
                    {
                        // Validate the start and end time of the campaign
                        //if (!ValidateCampaignsTime(campaign))
                        //    return;

                        // Is Rotate day has been selected
                        if (campaign.IsRotateDayChecked)
                        // Call to start publishing
                        {
                            SchedulePublisher(campaign);
                        }
                        else
                        {
                            // Check whether today is selected or not
                            var isCampaignSelected = campaign.ScheduledWeekday.FirstOrDefault(x =>
                                x.Content == DateTime.Now.DayOfWeek.ToString() && x.IsContentSelected);
                            if (isCampaignSelected == null)
                                return;
                            // Call to start publishing
                            SchedulePublisher(campaign);
                        }

                        Thread.Sleep(2);
                    });
                });
        }


        /// <summary>
        ///     Today Publishing scheduler by Campaign Id
        /// </summary>
        /// <param name="campaignId"> Campaign Id</param>
        public static void ScheduleTodaysPublisherByCampaign(string campaignId)
        {
            //var campaignDetails =
            //    PublisherInitialize.GetInstance.GetSavedCampaigns().Where(x => DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate).ToList();

            // get the all campaigns 
            var campaignDetails =
                PublisherInitialize.GetInstance.GetSavedCampaigns().ToList();

            var specificCampaign = campaignDetails.FirstOrDefault(x => x.CampaignId == campaignId);

            // Validate the start and end time of the campaign
            if (specificCampaign == null) return;
            {
                if (specificCampaign.IsRotateDayChecked)
                // Call to start publishing
                {
                    SchedulePublisher(specificCampaign);
                }
                else
                {
                    // Check whether today is selected or not
                    var isCampaignSelected = specificCampaign.ScheduledWeekday.FirstOrDefault(x =>
                        x.Content == DateTime.Now.DayOfWeek.ToString() && x.IsContentSelected);
                    if (isCampaignSelected == null)
                        return;
                    // Call to start publishing
                    SchedulePublisher(specificCampaign);
                }
            }
        }

        /// <summary>
        ///     Schedule publisher
        /// </summary>
        /// <param name="campaign"></param>
        private static void SchedulePublisher(PublisherCampaignStatusModel campaign)
        {
            #region Schedule
            var campaignItems = PublisherScheduledList.Where(x => x.Contains(campaign.CampaignId)).ToList();
            
            // stop already running campaigns
            if(campaignItems.Count != 0)
                StopScheduledPublisher(campaign.CampaignId);

            var postDetailsList = PostlistFileManager.GetAll(campaign.CampaignId);

            postDetailsList = postDetailsList.Where(x => x.PostQueuedStatus == PostQueuedStatus.Published
                                                         && x.LstPublishedPostDetailsModels.FirstOrDefault(y =>
                                                             y.PublishedDate.Date == DateTime.Now.Date
                                                             && y.Successful == ConstantVariable.Yes) != null).ToList();
            

            // Get the specific running time of a campaign
            var timeRange = campaign.SpecificRunningTime;

            if (timeRange.Count > postDetailsList.Count)
            {
                // reverse timings so the last one be remained in the list
                var reverseTime = timeRange.DeepCloneObject();
                reverseTime.Reverse();
                // taking only those timings which was not taken yet
                timeRange = reverseTime.Take(timeRange.Count - postDetailsList.Count).ToList();
            }

            // Check whether random time for every day has selected
            if (campaign.IsRandomRunningTime)
                if (campaign.UpdatedTime.Date != DateTime.Today)
                    // Otherwise fetch random intervals
                    campaign.SpecificRunningTime = GenerateRandomIntervals(campaign.MaximumTime, campaign.TimeRange);
            //timeRange = GenerateRandomIntervals(campaign.MaximumTime, campaign.TimeRange);

            // Iterate running times 
            var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();


            // Get campaign model
            var allCampaign = genericFileManager
                .GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile());

            // Get the particular campaign
            var currentCampaign = allCampaign.FirstOrDefault(x => x.CampaignId == campaign.CampaignId);

            if (currentCampaign == null)
                return;
            // Finding index
            var campaignIndex = allCampaign.IndexOf(currentCampaign);

            allCampaign[campaignIndex] = currentCampaign;

            //Save into bin file 
            genericFileManager.UpdateModuleDetails(allCampaign, ConstantVariable.GetPublisherCampaignFile());
            timeRange.ForEach(runningTime =>
            {

                // Make start time
                var startTime =
                    DateTime.Today.Add(new TimeSpan(runningTime.Hours, runningTime.Minutes, runningTime.Seconds));

                // If start time is greater than current time
                // if (startTime > DateTime.Now) // Commented for Fixing bug EW-I563
                {
                    // Generate job name
                    var addJobName = $"{campaign.CampaignId}-{ConstantVariable.GetDateTime()}";

                    // Add into scheduled lsit
                    PublisherScheduledList.Add(addJobName);

                    //if (startTime.AddSeconds(10) < DateTime.Now)
                    //    startTime = startTime.AddDays(1);

                    if (DateTime.Now.Date.Add(campaign.TimeRange.EndTime) < DateTime.Now)
                        startTime = startTime.AddDays(1);
                    else if (postDetailsList.Count > campaign.SpecificRunningTime.Count)
                        startTime = startTime.AddDays(1);

                    // Add job manager
                    JobManager.AddJob(() =>
                    {
                        var scheduleTime = startTime;
                        if (ValidateCampaignsTime(campaign))
                        {
                            if (!(startTime > DateTime.Now.AddMinutes(1)))
                            {
                                // Call the start publishing
                                StartPublishingPosts(campaign);
                                return;
                            }

                            scheduleTime = DateTime.Now.AddSeconds((startTime - DateTime.Now).TotalSeconds);
                        }
                        else
                        {
                            if (campaign.EndDate != null && campaign.EndDate < DateTime.Now )
                                return;
                            if (!string.IsNullOrEmpty(currentCampaign.SharePostModel.AddFdPageUrl))
                                scheduleTime =
                                        DateTime.Now.AddMinutes(1);
                            else
                                scheduleTime =
                                            DateTime.Now.AddSeconds(((campaign.StartDate ?? DateTime.Now) - DateTime.Now)
                                                    .TotalSeconds);
                        }

                        JobManager.AddJob(() =>
                        {
                            // Call the start publishing
                            SchedulePublisher(campaign);
                        }, x => x.WithName(addJobName).ToRunOnceAt(scheduleTime));
                    }, s => s.WithName(addJobName).ToRunOnceAt(startTime));

                    // Get the advanced settings details of an campaigns
                    var advancedSettings = genericFileManager
                        .GetModuleDetails<GeneralModel>(
                            ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                        .FirstOrDefault(x => x.CampaignId == campaign.CampaignId);

                    // Check whether campaign destination time out options
                    if (!(advancedSettings?.DestinationTimeout > 0)) return;
                    {
                        // Generate the job name for stopping campaigns
                        var stopJobName = $"{campaign.CampaignId}-StopRunningDueToTimeOut";

                        // Add into schedule list
                        PublisherScheduledList.Add(stopJobName);

                        // Calculate stopping time
                        var stopTime = DateTime.Now.AddMinutes(advancedSettings.DestinationTimeout);

                        // Add job process for stop publishing after some x minutes
                        JobManager.AddJob(() =>
                        {
                            // Call stop publishing
                            StopPublishingPosts(campaign.CampaignId);
                        }, s => s.WithName(stopJobName).ToRunOnceAt(stopTime));
                    }
                }
            });

            #endregion
        }

        /// <summary>
        ///     Generate random running times
        /// </summary>
        /// <param name="maxCount">max running time count</param>
        /// <param name="timeRange">Time range</param>
        /// <returns></returns>
        public static List<TimeSpan> GenerateRandomIntervals(int maxCount, TimeRange timeRange)
        {
            // Initialize time span
            var timer = new List<TimeSpan>();

            // Random objects
            var random = new Random();

            // start time
            var startTime = timeRange.StartTime;

            // End time
            var endTime = timeRange.EndTime;

            // Iterate untill getting required amount of time range
            for (var countIndex = 0; countIndex < maxCount; countIndex++)
                timer.Add(DateTimeUtilities.GetRandomTime(startTime, endTime, random));
            return timer;
        }

        /// <summary>
        ///     Stop Publishing scheduler
        /// </summary>
        /// <param name="campaignId"></param>
        private static void StopScheduledPublisher(string campaignId)
        {
            // Get the current campaings schedule lists
            var currentCampaignsItems = PublisherScheduledList.Where(x => x.Contains(campaignId)) != null ?
                PublisherScheduledList.Where(x => x.Contains(campaignId)).ToList() : new List<string>();

            if (currentCampaignsItems == null)
                return;
            // Iterate one by one to remove 
            currentCampaignsItems.ForEach(name =>
            {
                // Remove schedule list
                JobManager.RemoveJob(name);
                PublisherScheduledList.RemoveAll(x => x.Contains(name));
            });
        }

        /// <summary>
        ///     Update new groups for a destinations
        /// </summary>
        public static void UpdateNewGroupList()
        {
            // Get all destinations details
            var destinations = ManageDestinationFileManager.GetAll();

            // Iterate destinations
            destinations.ForEach(x =>
            {
                if (x.IsAddNewGroups)
                    // Update groups
                    PublisherInitialize.UpdateNewGroups(x.DestinationId);
            });
        }
    }
}