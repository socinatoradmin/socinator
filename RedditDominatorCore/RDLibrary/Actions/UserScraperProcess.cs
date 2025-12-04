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

namespace RedditDominatorCore.RDLibrary.Actions
{
    internal class UserScraperProcess : RdJobProcessInteracted<InteractedUsers>
    {
        private readonly IDbCampaignService _campaignService;

        private readonly IDbAccountServiceScoped _dbAccountServiceScoped;

        public UserScraperProcess(IProcessScopeModel processScopeModel, IRdQueryScraperFactory queryScraperFactor,
            IRedditLogInProcess redditLogInProcess, IRdHttpHelper rdHttpHelper,
            IDbAccountServiceScoped dbAccountServiceScoped, IDbCampaignService campaignService)
            : base(processScopeModel, queryScraperFactor, redditLogInProcess, rdHttpHelper)
        {
            _dbAccountServiceScoped = dbAccountServiceScoped;
            _campaignService = campaignService;
            UserScraperModel = processScopeModel.GetActivitySettingsAs<UserScraperModel>();
        }

        public UserScraperModel UserScraperModel { get; set; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                IncrementCounters();
                AddScrapedUsersToDataBase(scrapeResult);
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

        public void AddScrapedUsersToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var user = (RedditUser)scrapeResult.ResultUser;
                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration =
                    jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];


                if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
                    _campaignService.Add(new DominatorHouseCore.DatabaseHandler.RdTables.Campaigns.InteractedUsers
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        InteractedUsername = user.Username,
                        Date = DateTimeUtilities.GetEpochTime(),
                        InteractedUserId = user.Id,
                        UpdatedTime = DateTimeUtilities.GetEpochTime(),
                        AccountIcon = user.AccountIcon,
                        CommentKarma = user.CommentKarma,
                        Created = user.Created,
                        DisplayName = user.DisplayName,
                        DisplayNamePrefixed = user.DisplayNamePrefixed,
                        DisplayText = user.DisplayText,
                        HasUserProfile = user.HasUserProfile,
                        IsEmployee = user.IsEmployee,
                        IsFollowing = user.IsFollowing,
                        IsGold = user.IsGold,
                        IsMod = user.IsMod,
                        IsNsfw = user.IsNsfw,
                        PrefShowSnoovatar = user.PrefShowSnoovatar,
                        PostKarma = user.PostKarma,
                        Url = user.Url,
                        SinAccId = AccountId,
                        SinAccUsername = AccountName,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now
                    });

                _dbAccountServiceScoped.Add(new InteractedUsers
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    InteractedUsername = user.Username,
                    Date = DateTimeUtilities.GetEpochTime(),
                    InteractedUserId = user.Id,
                    UpdatedTime = DateTimeUtilities.GetEpochTime(),
                    AccountIcon = user.AccountIcon,
                    CommentKarma = user.CommentKarma,
                    Created = user.Created,
                    DisplayName = user.DisplayName,
                    DisplayNamePrefixed = user.DisplayNamePrefixed,
                    DisplayText = user.DisplayText,
                    HasUserProfile = user.HasUserProfile,
                    IsEmployee = user.IsEmployee,
                    IsFollowing = user.IsFollowing,
                    IsGold = user.IsGold,
                    IsMod = user.IsMod,
                    IsNsfw = user.IsNsfw,
                    PrefShowSnoovatar = user.PrefShowSnoovatar,
                    PostKarma = user.PostKarma,
                    Url = user.Url,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

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
    }
}