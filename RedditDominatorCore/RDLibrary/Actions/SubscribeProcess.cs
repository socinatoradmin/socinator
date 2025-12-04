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
using Unity;

namespace RedditDominatorCore.RDLibrary.Actions
{
    internal class SubscribeProcess : RdJobProcessInteracted<InteractedSubreddit>
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly IRdBrowserManager _browserManager;
        private readonly IDbCampaignService _campaignService;
        private readonly IDbAccountServiceScoped _dbAccountServiceScoped;
        private readonly IRedditFunction _redditFunction;
        private int _activityFailedCount = 1;
        private IRdBrowserManager _newBrowserWindow;

        public SubscribeProcess(IProcessScopeModel processScopeModel, IRdQueryScraperFactory queryScraperFactor,
            IRedditLogInProcess redditLogInProcess, IRdHttpHelper rdHttpHelper,
            IDbAccountServiceScoped dbAccountServiceScoped, IDbCampaignService campaignService,
            IRedditFunction redditFunction)
            : base(processScopeModel, queryScraperFactor, redditLogInProcess, rdHttpHelper)
        {
            _dbAccountServiceScoped = dbAccountServiceScoped;
            _campaignService = campaignService;
            _redditFunction = redditFunction;
            SubscribeModel = processScopeModel.GetActivitySettingsAs<SubscribeModel>();
            _browserManager = redditLogInProcess._browserManager;
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
        }

        public SubscribeModel SubscribeModel { get; set; }

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
            if (subReddit == null || subReddit.UserIsSubscriber) return jobProcessResult;
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            try
            {
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    var response = _redditFunction.NewSubscribe(DominatorAccountModel, subReddit);
                    if (response.Success)
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, subReddit.Url);
                        IncrementCounters();
                        AddSubscribeedDataToDataBase(scrapeResult);
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
                    if (scrapeResult.QueryInfo.QueryType == "Keywords")
                    {
                        _newBrowserWindow = _accountScopeFactory[$"{DominatorAccountModel.AccountId}"]
                            .Resolve<IRdBrowserManager>();
                        if (_browserManager.Subscribe(subReddit.Url, DominatorAccountModel).Item1)
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, subReddit.Url);
                            IncrementCounters();
                            AddSubscribeedDataToDataBase(scrapeResult);
                            jobProcessResult.IsProcessSuceessfull = true;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.ActivityFailed,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, subReddit.Url,
                                "Reason: Already Upvoted");

                            //Reschedule if action block
                            StopActivityAfterFailedAttemptAndReschedule();
                        }
                    }
                    else
                    {
                        if (_browserManager.Subscribe(subReddit.Url,DominatorAccountModel).Item1)
                        {
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, subReddit.Url);
                            IncrementCounters();
                            AddSubscribeedDataToDataBase(scrapeResult);
                            jobProcessResult.IsProcessSuceessfull = true;
                            _browserManager.CloseBrowser();
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.ActivityFailed,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, subReddit.Url,"Reason: Already upvoted");
                            _browserManager.CloseBrowser();

                            //Reschedule if action block
                            StopActivityAfterFailedAttemptAndReschedule();
                        }
                    }
                }
                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                if (_browserManager != null && _browserManager.BrowserWindow != null) _browserManager.CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return jobProcessResult;
        }

        public void AddSubscribeedDataToDataBase(ScrapeResultNew scrapeResult)
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
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        ActivityType = ActivityType.ToString(),
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        title = subReddit.Title,
                        name = subReddit.Name,
                        InteractionDateTime = DateTime.Now,
                        url = subReddit.Url,
                        subscribers = subReddit.Subscribers,
                        publicDescription = subReddit.PublicDescription,
                        isNSFW = subReddit.IsNsfw,
                        SubscribeId = subReddit.Id,
                        isQuarantined = subReddit.IsQuarantined,
                        whitelistStatus = subReddit.WhitelistStatus,
                        displayText = subReddit.DisplayText,
                        primaryColor = subReddit.PrimaryColor,
                        wls = subReddit.Wls,
                        type = subReddit.Type,
                        communityIcon = subReddit.CommunityIcon,
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

                    });


                _dbAccountServiceScoped.Add(new InteractedSubreddit
                {
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    ActivityType = ActivityType.ToString(),
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now,
                    title = subReddit.Title,
                    name = subReddit.Name,
                    subscribers = subReddit.Subscribers,
                    whitelistStatus = subReddit.WhitelistStatus,
                    primaryColor = subReddit.PrimaryColor,
                    SubscribeId = subReddit.Id,
                    userIsSubscriber = subReddit.UserIsSubscriber,
                    publicDescription = subReddit.PublicDescription,
                    url = subReddit.Url,
                    isNSFW = subReddit.IsNsfw,
                    isQuarantined = subReddit.IsQuarantined,
                    wls = subReddit.Wls,
                    displayText = subReddit.DisplayText,
                    type = subReddit.Type,
                    communityIcon = subReddit.CommunityIcon,
                    accountsActive = subReddit.AccountsActive,
                    advertiserCategory = subReddit.AdvertiserCategory,
                    showMedia = subReddit.ShowMedia,
                    usingNewModmail = subReddit.UsingNewModmail,
                    emojisEnabled = subReddit.EmojisEnabled,
                    originalContentTagEnabled = subReddit.OriginalContentTagEnabled,
                    allOriginalContent = subReddit.AllOriginalContent,
                    SinAccId = AccountId,
                    SinAccUsername = AccountName,


                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void StopActivityAfterFailedAttemptAndReschedule()
        {
            if (SubscribeModel.IsChkStopActivityAfterXXFailed &&
                _activityFailedCount++ == SubscribeModel.ActivityFailedCount)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_activityFailedCount - 1} " +
                    $"{"LangKeyFailedAttemptFor".FromResourceDictionary()} {SubscribeModel.FailedActivityReschedule} " +
                    $"{"LangKeyHour".FromResourceDictionary()}");

                StopAndRescheduleJob(SubscribeModel.FailedActivityReschedule);
            }
        }
    }
}