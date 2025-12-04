#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Diagnostics
{
    public class PublisherInitialize
    {
        private readonly IGenericFileManager _genericFileManager;

        private PublisherInitialize()
        {
            _genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
        }

        #region Properties

        // Networkwise Publisher objects
        private static Dictionary<SocialNetworks, IPublisherCollectionFactory> NetworkWisePublishers { get; } =
            new Dictionary<SocialNetworks, IPublisherCollectionFactory>();

        // Single ton objects
        private static PublisherInitialize _publisherInitialize;

        // Get intanse of Publisher Initialize
        public static PublisherInitialize GetInstance =>
            _publisherInitialize ?? (_publisherInitialize = new PublisherInitialize());

        // Collection of campaigns Status
        public ObservableCollection<PublisherCampaignStatusModel> ListPublisherCampaignStatusModels { get; set; } =
            new ObservableCollection<PublisherCampaignStatusModel>();

        #endregion

        /// <summary>
        ///     Register publisher Collection factory
        /// </summary>
        /// <param name="publisherCollectionFactory">
        ///     Publisher Objects
        ///     <see cref="DominatorHouseCore.Interfaces.IPublisherCollectionFactory" />
        /// </param>
        /// <param name="networks">social networks</param>
        public static void SaveNetworkPublisher(IPublisherCollectionFactory publisherCollectionFactory,
            SocialNetworks networks)
        {
            // If publisher network already present return 
            if (NetworkWisePublishers.ContainsKey(networks))
                return;

            // Add publisher collection factory with network
            NetworkWisePublishers.Add(networks, publisherCollectionFactory);
        }

        /// <summary>
        ///     Get publisher library for a specified network
        /// </summary>
        /// <param name="networks">
        ///     <see cref="DominatorHouseCore.Enums.SocialNetworks" />
        /// </param>
        /// <returns></returns>
        public static IPublisherCollectionFactory GetPublisherLibrary(SocialNetworks networks)
        {
            // return the collection factory for a network, if specified network is not present return simply null
            return NetworkWisePublishers.ContainsKey(networks) ? NetworkWisePublishers[networks] : null;
        }

        /// <summary>
        ///     Get all saved campaigns Status
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<PublisherCampaignStatusModel> GetSavedCampaigns()
        {
            return ListPublisherCampaignStatusModels;
        }

        /// <summary>
        ///     Initialize all saved campaign for display in default page
        /// </summary>
        public void PublishCampaignInitializer()
        {
            // Get all saved campaign Model
            var allCampaign =
                _genericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable
                    .GetPublisherCampaignFile());

            InitilizePublisher(allCampaign);
        }

        private void InitilizePublisher(List<PublisherCreateCampaignModel> allCampaign)
        {
            Task.Factory.StartNew(() =>
            {
                // Iterate campaigns
                allCampaign.ForEach(campaigns =>
                {
                    // Get the campaign status 
                    var publisherCampaignStatus = campaigns.CampaignStatus;

                    // Check end has reached
                    if (campaigns.JobConfigurations.CampaignEndDate != null &&
                        DateTime.Now < campaigns.JobConfigurations.CampaignEndDate)
                    {
                        // Commented for Fixing bug EW-I563
                        //publisherCampaignStatus = DateTime.Now < campaigns.JobConfigurations.CampaignEndDate
                        //    ? campaigns.CampaignStatus
                        //    : PublisherCampaignStatus.Completed;
                    }

                    List<TimeSpan> specificRunningTime = null;
                    if (campaigns.JobConfigurations.IsDelayPostChecked)
                    {
                        specificRunningTime = new List<TimeSpan>();

                        if (DateTime.Now.Date.Add(campaigns.JobConfigurations.TimeRange.StartTime) < DateTime.Now
                            && DateTime.Now.TimeOfDay < campaigns.JobConfigurations.TimeRange.EndTime)
                            specificRunningTime.Add(DateTime.Now.TimeOfDay);
                        else
                            specificRunningTime.Add(campaigns.JobConfigurations.TimeRange.StartTime);

                        for (var i = 0; i < campaigns.JobConfigurations.MaxPost - 1; i++)
                            specificRunningTime.Add(specificRunningTime[i]
                                .Add(TimeSpan.FromMinutes(RandomUtilties.GetRandomNumber(
                                    campaigns.JobConfigurations.DelayBetweenEachPost.EndValue,
                                    campaigns.JobConfigurations.DelayBetweenEachPost.StartValue))));
                    }

                    //Assign the Campaign Detatils
                    var publisherCampaignStatusModel = new PublisherCampaignStatusModel
                    {
                        CampaignName = campaigns.CampaignName,
                        CampaignId = campaigns.CampaignId,
                        StartDate = campaigns.JobConfigurations.CampaignStartDate,
                        EndDate = campaigns.JobConfigurations.CampaignEndDate,
                        CreatedDate = campaigns.CreatedDate,
                        UpdatedTime = campaigns.UpdatedDate,
                        Status = publisherCampaignStatus,
                        DestinationCount = campaigns.LstDestinationId.Count,
                        IsRotateDayChecked = campaigns.JobConfigurations.IsRotateDayChecked,
                        TimeRange = campaigns.JobConfigurations.TimeRange,
                        SpecificRunningTime = campaigns.JobConfigurations.IsDelayPostChecked
                            ? specificRunningTime
                            : campaigns.JobConfigurations.LstTimer.Select(x => x.MidTime).ToList(),
                        ScheduledWeekday = campaigns.JobConfigurations.Weekday.Where(x => x.IsContentSelected).ToList(),
                        IsTakeRandomDestination = campaigns.JobConfigurations.IsPublishPostOnRandomNDestinationsChecked,
                        SendOnePostForEachDestination = campaigns.JobConfigurations.IsWhenPublishingSendOnePostChecked,
                        TotalRandomDestination = campaigns.JobConfigurations.RandomDestinationCount,
                        MinRandomDestinationPerAccount = campaigns.JobConfigurations.PostBetween.EndValue,
                        IsRandomRunningTime = campaigns.JobConfigurations.IsRandomizePublishingTimerChecked,
                        MaximumTime = campaigns.JobConfigurations.MaxPost,
                        PendingCount =
                            campaigns.PostCollection.Count(x => x.PostQueuedStatus == PostQueuedStatus.Pending),
                        DraftCount = campaigns.PostCollection.Count(x => x.PostQueuedStatus == PostQueuedStatus.Draft)
                    };

                    if (!ListPublisherCampaignStatusModels.Any(x =>
                        x.CampaignName == publisherCampaignStatusModel.CampaignName
                        || x.CampaignId == publisherCampaignStatusModel.CampaignId))
                    {
                        if (!Application.Current.CheckAccess())
                            Application.Current.Dispatcher.Invoke(() =>
                                ListPublisherCampaignStatusModels.Add(publisherCampaignStatusModel));

                        else
                            // Add to lists
                            ListPublisherCampaignStatusModels.Add(publisherCampaignStatusModel);

                        // Update post counts
                        GetPostStatus(publisherCampaignStatusModel);

                        // Update campaign status to complete   // Commented for Fixing bug EW-I563
                        //if (DateTime.Now > campaigns.JobConfigurations.CampaignEndDate)
                        //    UpdateCampaignStatus(campaigns.CampaignId, PublisherCampaignStatus.Completed);
                    }
                });
                Thread.Sleep(2);
                PublishScheduler.ScheduleTodaysPublisher();
            });
        }


        /// <summary>
        ///     Update campaign Status
        /// </summary>
        /// <param name="campaignId">campaign Id</param>
        /// <param name="status">Campaign Current status</param>
        public void UpdateCampaignStatus(string campaignId, PublisherCampaignStatus status)
        {
            // Get campaign Details
            var campaignItem = ListPublisherCampaignStatusModels.FirstOrDefault(x => x.CampaignId == campaignId);

            if (campaignItem == null)
                return;

            // get the index of current item
            var currentCampaignIndex = ListPublisherCampaignStatusModels.IndexOf(campaignItem);

            // Update the status
            ListPublisherCampaignStatusModels[currentCampaignIndex].Status = status;

            // Get campaign model
            var allCampaign = _genericFileManager
                .GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile());

            // Get the particular campaign
            var currentCampaign = allCampaign.FirstOrDefault(x => x.CampaignId == campaignId);

            if (currentCampaign == null)
                return;
            // Finding index
            var campaignIndex = allCampaign.IndexOf(currentCampaign);

            // Update status 
            currentCampaign.CampaignStatus = status;
            allCampaign[campaignIndex] = currentCampaign;

            //Save into bin file 
            _genericFileManager.UpdateModuleDetails(allCampaign, ConstantVariable.GetPublisherCampaignFile());
        }

        /// <summary>
        ///     Update post status
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        public void UpdatePostStatus(string campaignId)
        {
            //get specific campaign
            var campaignItem = ListPublisherCampaignStatusModels.FirstOrDefault(x => x.CampaignId == campaignId);

            if (campaignItem == null)
                return;

            // Finding Index for campaign
            var currentCampaignIndex = ListPublisherCampaignStatusModels.IndexOf(campaignItem);

            // Update the post details
            GetPostStatus(ListPublisherCampaignStatusModels[currentCampaignIndex]);
        }

        /// <summary>
        ///     Update post details counts
        /// </summary>
        /// <param name="publisherCampaignStatusModel">Campaigns Statu model</param>
        public void GetPostStatus(PublisherCampaignStatusModel publisherCampaignStatusModel)
        {
            // Get all post list for a campaign
            var postdetails = PostlistFileManager.GetAll(publisherCampaignStatusModel.CampaignId);


            // Pending count
            publisherCampaignStatusModel.PendingCount =
                postdetails.Count(x => x.PostQueuedStatus == PostQueuedStatus.Pending);

            // Published count
            publisherCampaignStatusModel.PublishedCount =
                postdetails.Count(x => x.PostQueuedStatus == PostQueuedStatus.Published);

            // Draft count
            publisherCampaignStatusModel.DraftCount =
                postdetails.Count(x => x.PostQueuedStatus == PostQueuedStatus.Draft);
        }

        /// <summary>
        ///     Update post counts
        /// </summary>
        /// <param name="campaignId"></param>
        public void UpdatePostCounts(string campaignId)
        {
            //get specific campaign
            var campaignItem = ListPublisherCampaignStatusModels.FirstOrDefault(x => x.CampaignId == campaignId);

            if (campaignItem == null)
                return;
            // Finding Index for campaign
            var currentCampaignIndex = ListPublisherCampaignStatusModels.IndexOf(campaignItem);

            // Update the post details
            GetPostStatus(ListPublisherCampaignStatusModels[currentCampaignIndex]);
        }

        /// <summary>
        ///     Add new campaign status, Its used whilesaving campaigns
        /// </summary>
        /// <param name="publisherCampaignStatusModel">Campaign status model</param>
        /// <returns></returns>
        public bool AddCampaignDetails(PublisherCampaignStatusModel publisherCampaignStatusModel)
        {
            // Check whether campaign is already present or not
            if (ListPublisherCampaignStatusModels.Any(x => x.CampaignId == publisherCampaignStatusModel.CampaignId))
            {
                // Finding current items
                var currentItem =
                    ListPublisherCampaignStatusModels.FirstOrDefault(x =>
                        x.CampaignId == publisherCampaignStatusModel.CampaignId);

                // Get the index of the current campaign
                var index = ListPublisherCampaignStatusModels.IndexOf(currentItem);

                // Update post count
                // GetPostStatus(publisherCampaignStatusModel);

                // Substutite with proper index
                ListPublisherCampaignStatusModels[index] = publisherCampaignStatusModel;

                return true;
            }

            // Check campaigns start and end time
            if (publisherCampaignStatusModel.ValidDateTime())
            {
                try
                {
                    // access with dispatcher
                    if (!Application.Current.Dispatcher.CheckAccess())
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            // Update the post status
                            // GetPostStatus(publisherCampaignStatusModel);
                            // Add into collections
                            ListPublisherCampaignStatusModels.Add(publisherCampaignStatusModel);
                        });
                    else
                        // Update the post status
                        // GetPostStatus(publisherCampaignStatusModel);
                        // Add into collections
                        ListPublisherCampaignStatusModels.Add(publisherCampaignStatusModel);
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Update new groups
        /// </summary>
        /// <param name="destinationId">Destination ID</param>
        public static void UpdateNewGroups(string destinationId)
        {
            // Get create destination objects
            var objPublisherCreateDestinationModel = new PublisherCreateDestinationModel();
            // Call to update new groups
            objPublisherCreateDestinationModel.UpdateNewGroup(destinationId);
        }

        /// <summary>
        ///     Remove the destination which requires Admin Verification
        /// </summary>
        /// <param name="destinationId">Destination Id</param>
        /// <param name="accountId">Account ID</param>
        /// <param name="network">Social Networks</param>
        /// <param name="groupUrl">groups</param>
        // ReSharper disable once UnusedMember.Global
        public static void RemoveGroupsFromDestination(string destinationId, string accountId, SocialNetworks network,
            string groupUrl)
        {
            // Get create destination objects
            var objPublisherCreateDestinationModel = new PublisherCreateDestinationModel();
            // Deselect group url from destinations
            objPublisherCreateDestinationModel.RemoveGroupsFromDestination(destinationId, accountId, network, groupUrl);
        }

        /// <summary>
        ///     Get networks published post count
        /// </summary>
        /// <param name="campaignId">campaign ID</param>
        /// <param name="network">social network</param>
        /// <returns></returns>
        public static List<PublishedPostDetailsModel> GetNetworksPublishedPost(string campaignId,
            SocialNetworks network)
        {
            try
            {
                // Check whether campaign Id is not or not
                if (string.IsNullOrEmpty(campaignId) || network == SocialNetworks.Social)
                    return new List<PublishedPostDetailsModel>();

                // Return published posts details
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                return genericFileManager
                    .GetModuleDetails<PublishedPostDetailsModel>(ConstantVariable.GetPublishedSuccessDetails)
                    .Where(x => x.CampaignId == campaignId && x.SocialNetworks == network).ToList();
            }
            catch (Exception)
            {
                return new List<PublishedPostDetailsModel>();
            }
        }
    }

    public class PublisherCoreLibraryBuilder
    {
        #region Constructors

        public PublisherCoreLibraryBuilder(IPublisherCoreFactory publisherCoreFactory)
        {
            PublisherCoreFactory = publisherCoreFactory;
        }

        #endregion


        #region Properties

        public IPublisherCoreFactory PublisherCoreFactory { get; set; }

        #endregion

        /// <summary>
        ///     Add network for publisher
        /// </summary>
        /// <param name="networks">Social Network</param>
        /// <returns></returns>
        public PublisherCoreLibraryBuilder AddNetwork(SocialNetworks networks)
        {
            // Assign Social Network
            PublisherCoreFactory.Network = networks;
            return this;
        }

        /// <summary>
        ///     Add Publisher Job Factory object for a network
        /// </summary>
        /// <param name="jobFactory">Base class which inherits IPublisherJobProcessFactory</param>
        /// <returns></returns>
        public PublisherCoreLibraryBuilder AddPublisherJobFactory(IPublisherJobProcessFactory jobFactory)
        {
            PublisherCoreFactory.PublisherJobFactory = jobFactory;
            return this;
        }

        /// <summary>
        ///     Added post scarper objects for a networks
        /// </summary>
        /// <param name="postScraper">post scraper base class which inherits IPublisherPostScraper</param>
        /// <returns></returns>
        public PublisherCoreLibraryBuilder AddPostScraper(IPublisherPostScraper postScraper)
        {
            PublisherCoreFactory.PostScraper = postScraper;
            return this;
        }
    }
}