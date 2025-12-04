using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDRequest;
using System;

namespace RedditDominatorCore.RDLibrary.Actions
{
    internal class UnSubscribeProcess : RdJobProcessInteracted<InteractedSubreddit>
    {
        private readonly IRdBrowserManager _browserManager;
        private readonly IDbCampaignService _campaignService;
        private readonly IDbAccountServiceScoped _dbAccountServiceScoped;
        private readonly IRedditFunction _redditFunction;
        private int _activityFailedCount = 1;

        public UnSubscribeProcess(IProcessScopeModel processScopeModel, IRdQueryScraperFactory queryScraperFactor,
            IRedditLogInProcess redditLogInProcess, IRdHttpHelper rdHttpHelper,
            IDbAccountServiceScoped dbAccountServiceScoped,
            IDbCampaignService campaignService, IRedditFunction redditFunction)
            : base(processScopeModel, queryScraperFactor, redditLogInProcess, rdHttpHelper)
        {
            _dbAccountServiceScoped = dbAccountServiceScoped;
            _campaignService = campaignService;
            _redditFunction = redditFunction;
            UnSubscribeModel = processScopeModel.GetActivitySettingsAs<UnSubscribeModel>();
            _browserManager = redditLogInProcess._browserManager;
        }

        public UnSubscribeModel UnSubscribeModel { get; set; }

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

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            var subReddit = (SubRedditModel)scrapeResult.ResultPage;
            if (subReddit == null || string.IsNullOrEmpty(subReddit.Name))
            {
                _browserManager.CloseBrowser();
                return jobProcessResult;
            }

            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            try
            {
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    var response = _redditFunction.NewUnSubscribe(DominatorAccountModel, subReddit);
                    if (response.Success)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, subReddit.Url);
                        IncrementCounters();
                        AddUnSubscribeedDataToDataBase(scrapeResult);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, subReddit.Url,
                            "Reason: Blocked");
                        //Reschedule if action block
                        StopActivityAfterFailedAttemptAndReschedule();
                    }
                }
                //For browser automation
                else
                {
                    if (_browserManager.UnSubscribe())
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, subReddit.Url);
                        IncrementCounters();
                        AddUnSubscribeedDataToDataBase(scrapeResult);
                        jobProcessResult.IsProcessSuceessfull = true;
                        _browserManager.CloseBrowser();
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, subReddit.Url,
                            "Reason: Blocked");
                        _browserManager.CloseBrowser();

                        //Reschedule if action block
                        StopActivityAfterFailedAttemptAndReschedule();
                    }
                }

                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                _browserManager.CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        public void AddUnSubscribeedDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var subReddit = (SubRedditModel)scrapeResult.ResultPage;
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration =
                    jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
                    _campaignService.Add(new DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedSubreddit
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

        private void StopActivityAfterFailedAttemptAndReschedule()
        {
            if (UnSubscribeModel.IsChkStopActivityAfterXXFailed &&
                _activityFailedCount++ == UnSubscribeModel.ActivityFailedCount)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_activityFailedCount - 1} " +
                    $"{"LangKeyFailedAttemptFor".FromResourceDictionary()} {UnSubscribeModel.FailedActivityReschedule} " +
                    $"{"LangKeyHour".FromResourceDictionary()}");

                StopAndRescheduleJob(UnSubscribeModel.FailedActivityReschedule);
            }
        }
    }
}