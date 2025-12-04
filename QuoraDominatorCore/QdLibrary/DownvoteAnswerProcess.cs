using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.QdTables.Campaigns;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Factories;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Request;
using System;
using System.Linq;
using Unity;
using InteractedAnswerAccount = DominatorHouseCore.DatabaseHandler.QdTables.Accounts;

namespace QuoraDominatorCore.QdLibrary
{
    internal class DownvoteAnswerProcess : QdJobProcessInteracted<InteractedAnswerAccount.InteractedAnswers>
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private IQuoraBrowserManager _browser;
        private readonly DownvoteAnswersModel _downvoteAnswerModel;

        private readonly IQdHttpHelper _httpHelper;
        private readonly SocialNetworks _networks;

        private readonly IQuoraFunctions quoraFunct;
        private int _followedToday;
        private IQDBrowserManagerFactory managerFactory;
        public DownvoteAnswerProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IExecutionLimitsManager executionLimitsManager, IQuoraFunctions qdFunc,
            IQdQueryScraperFactory queryScraperFactory, IQdHttpHelper qdHttpHelper, IQdLogInProcess qdLogInProcess)
            : base(processScopeModel, accountServiceScoped, executionLimitsManager, queryScraperFactory, qdHttpHelper,
                qdLogInProcess)
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _httpHelper = _accountScopeFactory[DominatorAccountModel.AccountId].Resolve<IQdHttpHelper>();
            quoraFunct = qdFunc;
            _downvoteAnswerModel = processScopeModel.GetActivitySettingsAs<DownvoteAnswersModel>();
            _networks = SocialNetworks.Quora;
            managerFactory = qdLogInProcess.managerFactory;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            _browser = managerFactory.QdBrowserManager();
            var quoraUser = (QuoraUser) scrapeResult.ResultUser;
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, quoraUser.Url);

            var jobProcessResult = new JobProcessResult();
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var IsBrowser = DominatorAccountModel.IsRunProcessThroughBrowser;
                var IsSuccess = false;
                if (!IsBrowser)
                {
                    var linkresp = quoraFunct.AnswerDetails(DominatorAccountModel, quoraUser.Url).Response.Response;
                    var objDownvoteResponseHandler = quoraFunct.DownvoteAnswer(DominatorAccountModel, linkresp, quoraUser.Url).Result;
                    IsSuccess = objDownvoteResponseHandler.Success;
                }
                else
                {
                    _browser.SearchByCustomUrl(DominatorAccountModel, quoraUser.Url);
                    IsSuccess = _browser.DownVoteAnswer(quoraUser.Url);
                }
                if (IsSuccess)
                {
                    AddDownvotingAnswerDataToDataBase(scrapeResult);
                    IncrementCounters();
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, quoraUser.Url);
                    PostDownvoteProcess(quoraUser);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, quoraUser.Url,"");
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
            catch (Exception e)
            {
                e.DebugLog();
            }
            if(_downvoteAnswerModel != null && _downvoteAnswerModel.IsEnableAdvancedUserMode && _downvoteAnswerModel.EnableDelayBetweenPerformingActionOnSamePost)
                DelayBeforeNextActivity(_downvoteAnswerModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
            else
                DelayBeforeNextActivity();

            return jobProcessResult;
        }

        private void AddDownvotingAnswerDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var quoraUser = (QuoraUser) scrapeResult.ResultUser;

                #region Save to CampaignDB

                if (!string.IsNullOrEmpty(CampaignId))
                {
                    IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                    dbCampaignService.Add(new InteractedAnswers
                    {
                        ActivityType = ActivityType.ToString(),
                        InteractionDateTime = DateTime.Now,
                        QueryType = scrapeResult.QueryInfo.QueryType,
                        QueryValue = scrapeResult.QueryInfo.QueryValue,
                        AnswersUrl = quoraUser.Url,
                        AnsweredUserName = quoraUser.Username,
                        Accountusername = DominatorAccountModel.UserName
                    });
                }

                #endregion

                #region Save to AccountDB

                var dbAccountService =
                    InstanceProvider.ResolveAccountDbOperations(DominatorAccountModel.AccountId, _networks);

                dbAccountService.Add(new InteractedAnswerAccount.InteractedAnswers
                {
                    ActivityType = ActivityType.ToString(),
                    InteractionDateTime = DateTime.Now,
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    AnswersUrl = quoraUser.Url,
                    AnsweredUserName = quoraUser.Username,
                    Accountusername = DominatorAccountModel.UserName
                });

                #endregion

                #region Save to PrivateBlacklistDB

                if (_downvoteAnswerModel.IsChkDownvoteAnswerPrivateBlacklist)
                    dbAccountService.Add(
                        new InteractedAnswerAccount.PrivateBlacklist
                        {
                            UserName = quoraUser.Username,
                            UserId = quoraUser.UserId,
                            InteractionTimeStamp = DateTime.Now.GetCurrentEpochTime()
                        });

                #endregion

                #region Add to GroupBlacklist DB

                if (_downvoteAnswerModel.IsChkDownvoteAnswerGroupBlacklist)
                {
                    IDbGlobalService dbGlobalService = new DbGlobalService();
                    dbGlobalService.Add(new BlackListUser
                    {
                        UserName = quoraUser.Username,
                        UserId = quoraUser.UserId,
                        AddedDateTime = DateTime.Now
                    });
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void PostDownvoteProcess(QuoraUser quorauser)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            try
            {
                #region Follow

                if (_downvoteAnswerModel.EnableFollowUserer)
                {
                    IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                    _followedToday = dbCampaignService.GetAllInteractedUsers().Count(x =>
                        x.SinAccUsername == DominatorAccountModel.UserName && x.InteractionDateTime >= DateTime.Today);


                    if (_followedToday >= _downvoteAnswerModel.FollowMaxBetween.GetRandom() &&
                        _downvoteAnswerModel.IsChkFollowMax)
                    {
                        GlobusLogHelper.log.Info(
                            $"{SocialNetworks.Quora}\t {DominatorAccountModel.UserName}\t has reached per day limit to Follow");
                        return;
                    }

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var profilepagesource = _httpHelper
                        .GetRequest($"{QdConstants.HomePageUrl}/profile/" + quorauser.Username).Response;

                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var response = quoraFunct.FollowAfterCompleteProcess(DominatorAccountModel, profilepagesource);
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (response.Success)
                    {
                        GlobusLogHelper.log.Info("{0}\t {1}\t Followed " + quorauser.Username,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName);
                        AddFollowedDataToDataBase(quorauser);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddFollowedDataToDataBase(QuoraUser quorauser)
        {
            try
            {
                var dbAccountService =
                    InstanceProvider.ResolveAccountDbOperations(DominatorAccountModel.AccountId, _networks);
                dbAccountService.Add(new InteractedAnswerAccount.InteractedUsers
                {
                    ActivityType = ActivityType.ToString(),
                    Date = DateTimeUtilities.GetEpochTime(),
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    InteractedUsername = quorauser.Username
                });

                IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                dbCampaignService.Add(new InteractedUsers
                {
                    InteractedUsername = quorauser.Username,
                    InteractionDateTime = DateTime.Now,
                    SinAccUsername = DominatorAccountModel.UserName
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}