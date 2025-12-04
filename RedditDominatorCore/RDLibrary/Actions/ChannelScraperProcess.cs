using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDRequest;
using System;
using CampaignTables = DominatorHouseCore.DatabaseHandler.RdTables.Campaigns;

namespace RedditDominatorCore.RDLibrary.Actions
{
    internal class ChannelScraperProcess : RdJobProcessInteracted<InteractedSubreddit>
    {
        private readonly IDbCampaignService _campaignService;
        private readonly IDbAccountServiceScoped _dbAccountServiceScoped;

        /// <summary>
        ///     Intializes the dominator account model, per week, per day, per hour
        /// </summary>
        /// <param name="processScopeModel"></param>
        /// <param name="queryScraperFactor"></param>
        public ChannelScraperProcess(IProcessScopeModel processScopeModel, IRdQueryScraperFactory queryScraperFactor,
            IRedditLogInProcess redditLogInProcess, IRdHttpHelper rdHttpHelper,
            IDbAccountServiceScoped dbAccountServiceScoped, IDbCampaignService campaignService)
            : base(processScopeModel, queryScraperFactor, redditLogInProcess, rdHttpHelper)
        {
            _dbAccountServiceScoped = dbAccountServiceScoped;
            _campaignService = campaignService;
            ChannelScraperModel = processScopeModel.GetActivitySettingsAs<ChannelScraperModel>();
        }

        /// <summary>
        ///     Intializes the channel scraper model
        /// </summary>

        public ChannelScraperModel ChannelScraperModel { get; set; }

        /// <summary>
        ///     To perform other configuration
        /// </summary>
        /// <param name="scrapeResult"></param>

        // ReSharper disable once InheritdocConsiderUsage
        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
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

        /// <summary>
        ///     It will scrape the information with the help of keywords and custom url's.
        /// </summary>
        /// <param name="scrapeResult"></param>
        /// <returns></returns>

        // ReSharper disable once InheritdocConsiderUsage
        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    ((SubRedditModel)scrapeResult.ResultPage).DisplayText);

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                IncrementCounters();
                AddChannelDataToDataBase(scrapeResult);
                jobProcessResult.IsProcessSuceessfull = true;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            DelayBeforeNextActivity();

            return jobProcessResult;
        }

        /// <summary>
        ///     It will add the scraped data into database
        /// </summary>
        /// <param name="scrapeResult"></param>
        public void AddChannelDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var subReddit = (SubRedditModel)scrapeResult.ResultPage;
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration =
                    jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
                    _campaignService.Add(new CampaignTables.InteractedSubreddit
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        whitelistStatus = subReddit.WhitelistStatus,
                        isNSFW = subReddit.IsNsfw,
                        subscribers = subReddit.Subscribers,
                        primaryColor = subReddit.PrimaryColor,
                        SubscribeId = subReddit.Id,
                        isQuarantined = subReddit.IsQuarantined,
                        name = subReddit.Name,
                        title = subReddit.Title,
                        url = subReddit.Url,
                        wls = subReddit.Wls,
                        displayText = subReddit.DisplayText,
                        type = subReddit.Type,
                        communityIcon = subReddit.CommunityIcon,
                        publicDescription = subReddit.PublicDescription,
                        userIsSubscriber = subReddit.UserIsSubscriber,
                        accountsActive = subReddit.AccountsActive,
                        advertiserCategory = subReddit.AdvertiserCategory,
                        showMedia = subReddit.ShowMedia,
                        usingNewModmail = subReddit.UsingNewModmail,
                        emojisEnabled = subReddit.EmojisEnabled,
                        originalContentTagEnabled = subReddit.OriginalContentTagEnabled,
                        allOriginalContent = subReddit.AllOriginalContent,
                        SinAccId = AccountId,
                        SinAccUsername = AccountName,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now
                    });

                _dbAccountServiceScoped.Add(new InteractedSubreddit
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    whitelistStatus = subReddit.WhitelistStatus,
                    isNSFW = subReddit.IsNsfw,
                    subscribers = subReddit.Subscribers,
                    primaryColor = subReddit.PrimaryColor,
                    SubscribeId = subReddit.Id,
                    isQuarantined = subReddit.IsQuarantined,
                    name = subReddit.Name,
                    title = subReddit.Title,
                    url = subReddit.Url,
                    wls = subReddit.Wls,
                    displayText = subReddit.DisplayText,
                    type = subReddit.Type,
                    communityIcon = subReddit.CommunityIcon,
                    publicDescription = subReddit.PublicDescription,
                    userIsSubscriber = subReddit.UserIsSubscriber,
                    accountsActive = subReddit.AccountsActive,
                    advertiserCategory = subReddit.AdvertiserCategory,
                    showMedia = subReddit.ShowMedia,
                    usingNewModmail = subReddit.UsingNewModmail,
                    emojisEnabled = subReddit.EmojisEnabled,
                    originalContentTagEnabled = subReddit.OriginalContentTagEnabled,
                    allOriginalContent = subReddit.AllOriginalContent,
                    SinAccId = AccountId,
                    SinAccUsername = AccountName,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}