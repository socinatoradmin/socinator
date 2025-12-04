using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.QdTables.Accounts;
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
using QuoraDominatorCore.Response;
using System;
using System.Linq;
using Unity;

namespace QuoraDominatorCore.QdLibrary
{
    internal class UpvoteAnswerProcess : QdJobProcessInteracted<InteractedAnswers>
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private IQuoraBrowserManager _browser;
        private readonly IQdHttpHelper _httpHelper;
        private readonly SocialNetworks _networks;
        private readonly UpvoteAnswersModel _upvoteAnswerModel;
        private readonly IQuoraFunctions quoraFunct;
        private int _followedToday;
        private readonly IDbGlobalService dbGlobalService;
        private IQDBrowserManagerFactory managerFactory;
        public UpvoteAnswerProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, IQuoraFunctions qdFunc,
            IQdQueryScraperFactory queryScraperFactory, IQdHttpHelper qdHttpHelper, IQdLogInProcess qdLogInProcess)
            : base(processScopeModel, accountServiceScoped, executionLimitsManager, queryScraperFactory, qdHttpHelper,
                qdLogInProcess)
        {
            dbGlobalService = globalService;
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _httpHelper = _accountScopeFactory[DominatorAccountModel.AccountId].Resolve<IQdHttpHelper>();
            quoraFunct = qdFunc;
            _upvoteAnswerModel = processScopeModel.GetActivitySettingsAs<UpvoteAnswersModel>();
            _networks = SocialNetworks.Quora;
            managerFactory = qdLogInProcess.managerFactory;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            _browser = managerFactory.QdBrowserManager();
            var linkresp = "";
            UpvoteResponseHandler response = null;
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var quoraUser = (QuoraUser) scrapeResult.ResultUser;
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, quoraUser.Url);


            var jobProcessResult = new JobProcessResult();
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var IsBrowser = DominatorAccountModel.IsRunProcessThroughBrowser;
                if (!IsBrowser)
                {
                    linkresp = quoraFunct.AnswerDetails(DominatorAccountModel, quoraUser.Url).Response.Response;
                    response = quoraFunct.UpvoteAnswer(DominatorAccountModel, linkresp, quoraUser).Result;
                    if (response.Success)
                    {
                        AddUpvotedAnswerDataToDataBase(scrapeResult);
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, quoraUser.Url);
                        IncrementCounters();
                        PostUpvoteProcess(quoraUser);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        response.ErrorMessage=string.IsNullOrEmpty(response.ErrorMessage)?string.Empty:$"==> {response.ErrorMessage} {ActivityType}";
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"{quoraUser.Url}{response.ErrorMessage}",
                            $"Failed to {ActivityType}");
                        jobProcessResult.IsProcessSuceessfull = false;
                    }
                }
                else
                {
                    linkresp = _browser.SearchByCustomUrl(DominatorAccountModel, quoraUser.Url).Response;
                    if (_browser.UpvoteAnswer(DominatorAccountModel, quoraUser.Url))
                    {
                        AddUpvotedAnswerDataToDataBase(scrapeResult);

                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, quoraUser.Url);
                        IncrementCounters();
                        PostUpvoteProcess(quoraUser);
                        jobProcessResult.IsProcessSuceessfull = true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, quoraUser.Url,
                            $"Failed to {ActivityType}");
                        jobProcessResult.IsProcessSuceessfull = false;
                    }
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
            }
            catch (Exception e)
            {
                e.DebugLog();
            }
            if(_upvoteAnswerModel != null && _upvoteAnswerModel.IsEnableAdvancedUserMode && _upvoteAnswerModel.EnableDelayBetweenPerformingActionOnSamePost)
                DelayBeforeNextActivity(_upvoteAnswerModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
            else
                DelayBeforeNextActivity();
            return jobProcessResult;
        }

        private void AddUpvotedAnswerDataToDataBase(ScrapeResultNew scrapeResult)
        {
            try
            {
                var quoraUser = (QuoraUser) scrapeResult.ResultUser;

                #region Save To CampaignDb

                if (!string.IsNullOrEmpty(CampaignId))
                {
                    IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                    dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.QdTables.Campaigns.InteractedAnswers
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

                #region Save to AccountDb

                var dbAccountService =
                    InstanceProvider.ResolveAccountDbOperations(DominatorAccountModel.AccountId, _networks);

                dbAccountService.Add(new InteractedAnswers
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

                #region Save to PrivateBlacklist DB

                if (_upvoteAnswerModel.IsChkUpvoteAnswerPrivateBlacklist)
                    dbAccountService.Add(
                        new PrivateBlacklist
                        {
                            UserName = quoraUser.Username,
                            UserId = quoraUser.UserId,
                            InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                        });

                #endregion

                #region Save to GroupBlacklist DB

                if (_upvoteAnswerModel.IsChkUpvoteAnswerGroupBlackList)
                    dbGlobalService.Add(new BlackListUser
                    {
                        UserName = quoraUser.Username,
                        UserId = quoraUser.UserId,
                        AddedDateTime = DateTime.Now
                    });

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void PostUpvoteProcess(QuoraUser quorauser)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            #region Follow

            if (_upvoteAnswerModel.EnableFollowUserer)
                try
                {
                    var dbAccountService = InstanceProvider.GetInstance<IDbAccountService>();

                    if (dbAccountService.GetInteractedUsers(ActivityType)
                            .Count(x => x.InteractedUsername == quorauser.Username) != 0)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Quora,
                            DominatorAccountModel.UserName, ActivityType,
                            $"Already Followed {quorauser.Username} from this account");
                        return;
                    }

                    var dbCampaignService = new DbCampaignService(CampaignId);
                    _followedToday = dbCampaignService.GetAllInteractedUsers().Count(x =>
                        x.SinAccUsername == DominatorAccountModel.UserName && x.InteractionDateTime >= DateTime.Today);


                    if (_followedToday >= _upvoteAnswerModel.FollowMaxBetween.GetRandom() &&
                        _upvoteAnswerModel.IsChkFollowMax)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Quora,
                            DominatorAccountModel.UserName, ActivityType, "has reached per day limit to follow");
                        return;
                    }

                    var profilepagesource = _httpHelper
                        .GetRequest($"{QdConstants.HomePageUrl}/profile/" + quorauser.Username).Response;

                    var response = quoraFunct.FollowAfterCompleteProcess(DominatorAccountModel, profilepagesource);
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (response.Success)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            "Followed " + quorauser.Username);
                        AddFollowedDataToDataBase(quorauser);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            #endregion
        }

        private void AddFollowedDataToDataBase(QuoraUser quorauser)
        {
            try
            {
                #region Save to AccountDb

                var dbAccountService =
                    InstanceProvider.ResolveAccountDbOperations(DominatorAccountModel.AccountId, _networks);
                dbAccountService.Add(new InteractedUsers
                {
                    ActivityType = ActivityType.ToString(),
                    Date = GetEpochTime(),
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    InteractedUsername = quorauser.Username
                });

                #endregion

                #region Save To CampaignDb

                IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.QdTables.Campaigns.InteractedUsers
                {
                    InteractedUsername = quorauser.Username,
                    InteractionDateTime = DateTime.Now,
                    SinAccUsername = DominatorAccountModel.UserName
                });

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}