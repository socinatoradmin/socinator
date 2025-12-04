using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDEnums;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDRequest;
using System;
using System.Linq;

namespace RedditDominatorCore.RDLibrary.Actions
{
    public class UnFollowProcess : RdJobProcessInteracted<UnfollowedUsers>
    {
        private readonly IRdBrowserManager _browserManager;
        private readonly IDbCampaignService _campaignService;
        private readonly IDbAccountServiceScoped _dbAccountServiceScoped;
        private readonly IRedditFunction _redditFunction;
        private int _activityFailedCount = 1;

        public UnFollowProcess(IProcessScopeModel processScopeModel, IRdQueryScraperFactory queryScraperFactor,
            IRedditLogInProcess redditLogInProcess, IRdHttpHelper rdHttpHelper,
            IDbAccountServiceScoped dbAccountServiceScoped, IDbCampaignService campaignService,
            IRedditFunction redditFunction)
            : base(processScopeModel, queryScraperFactor, redditLogInProcess, rdHttpHelper)
        {
            _dbAccountServiceScoped = dbAccountServiceScoped;
            _campaignService = campaignService;
            _redditFunction = redditFunction;
            UnFollowModel = processScopeModel.GetActivitySettingsAs<UnfollowerModel>();
            blackListWhitelistHandler =
                new BlackListWhitelistHandler(ModuleSetting, DominatorAccountModel, ActivityType);
            _browserManager = redditLogInProcess._browserManager;
        }

        public UnfollowerModel UnFollowModel { get; set; }
        private BlackListWhitelistHandler blackListWhitelistHandler { get; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            var newRedditUser = (RedditUser)scrapeResult.ResultUser;
            if (newRedditUser == null || !newRedditUser.IsFollowing) return jobProcessResult;
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            try
            {
                // Skip Whitelist User
                if (UnFollowModel.ManageBlackWhiteListModel.IsSkipGroupWhiteList)
                {
                    var whiteListUsers = blackListWhitelistHandler.GetWhiteListUsers(Enums.WhitelistblacklistType.Group);
                    if (whiteListUsers != null && whiteListUsers.Count > 0 && whiteListUsers.Any(user => string.Equals(newRedditUser.DisplayText, user, StringComparison.OrdinalIgnoreCase)))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                                ActivityType, $"{newRedditUser.DisplayText} Skiped, Present in Group Whitelist");
                        return jobProcessResult;
                    }
                }

                if (UnFollowModel.ManageBlackWhiteListModel.IsSkipPrivateWhiteList)
                {
                    var whiteListUsers = blackListWhitelistHandler.GetWhiteListUsers(Enums.WhitelistblacklistType.Private);
                    if (whiteListUsers != null && whiteListUsers.Count > 0 && whiteListUsers.Any(user => string.Equals(newRedditUser.DisplayText, user, StringComparison.OrdinalIgnoreCase)))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                                ActivityType, $"{newRedditUser.DisplayText} Skiped, Present in Private Whitelist");
                        return jobProcessResult;
                    }
                }

                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    var response = _redditFunction.NewUnfollow(DominatorAccountModel, newRedditUser);
                    if (response.Success)
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditUser.Username);
                        IncrementCounters();
                        AddUnFollowedDataToDataBase(scrapeResult);

                        // Add to Blacklist 
                        if (UnFollowModel.ManageBlackWhiteListModel.IsAddToBlackListOnceUnfollowed)
                            blackListWhitelistHandler.AddToBlackList(newRedditUser.Id, newRedditUser.Username);

                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditUser.Username,
                            "Reason: Blocked");
                        //Reschedule if action block
                        StopActivityAfterFailedAttemptAndReschedule();
                    }
                }
                //For browser automation
                else
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    _browserManager.UnFollow().TryGetValue("UnFollow", out string RedditUserUnfollowResponse);
                    if (string.IsNullOrEmpty(RedditUserUnfollowResponse))
                    {
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditUser.Username);
                        IncrementCounters();

                        AddUnFollowedDataToDataBase(scrapeResult);

                        // Add to Blacklist 
                        if (UnFollowModel.ManageBlackWhiteListModel.IsAddToBlackListOnceUnfollowed)
                            blackListWhitelistHandler.AddToBlackList(newRedditUser.Id, newRedditUser.Username);

                        jobProcessResult.IsProcessSuceessfull = true;
                        if(scrapeResult.QueryInfo.QueryType =="Custom User Lists" || scrapeResult.QueryInfo.QueryType == "Custom Url")
                        _browserManager.CloseBrowser();
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, newRedditUser.Username,
                            RedditUserUnfollowResponse);
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

        public void AddUnFollowedDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var user = (RedditUser)scrapeResult.ResultUser;
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration =
                    jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
                    _campaignService.Add(new DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.UnfollowedUsers
                    {
                        SinAccUsername = DominatorAccountModel.AccountBaseModel.UserName,
                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                        Username = user.Username,
                        UserId = user.Id,
                        FullName = user.DisplayName
                    });


                _dbAccountServiceScoped.Add(new UnfollowedUsers
                {
                    InteractionDateTime = DateTime.Now,
                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                    Username = user.Username,
                    UserId = user.Id,
                    FullName = user.DisplayName
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     Overrides abstract method of JobProcess. Will be called when JobProcess completes.
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

        private void StopActivityAfterFailedAttemptAndReschedule()
        {
            if (UnFollowModel.IsChkStopActivityAfterXXFailed &&
                _activityFailedCount++ == UnFollowModel.ActivityFailedCount)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"{"LangKeyStopActivityAfter".FromResourceDictionary()} {_activityFailedCount - 1} " +
                    $"{"LangKeyFailedAttemptFor".FromResourceDictionary()} {UnFollowModel.FailedActivityReschedule} " +
                    $"{"LangKeyHour".FromResourceDictionary()}");

                StopAndRescheduleJob(UnFollowModel.FailedActivityReschedule);
            }
        }
    }
}