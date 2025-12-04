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
using QuoraDominatorCore.Request;
using System;

namespace QuoraDominatorCore.QdLibrary
{
    internal class DownvoteQuestionProcess : QdJobProcessInteracted<InteractedQuestion>
    {
        private IQuoraBrowserManager _browser;
        private readonly DownvoteQuestionsModel _downvoteQuestionsModel;
        private readonly SocialNetworks _networks;
        private readonly IQuoraFunctions quoraFunct;
        private IQDBrowserManagerFactory managerFactory;
        public DownvoteQuestionProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IExecutionLimitsManager executionLimitsManager, IQuoraFunctions qdFunc,
            IQdQueryScraperFactory queryScraperFactory, IQdHttpHelper qdHttpHelper, IQdLogInProcess qdLogInProcess)
            : base(processScopeModel, accountServiceScoped, executionLimitsManager, queryScraperFactory, qdHttpHelper,
                qdLogInProcess)
        {
            quoraFunct = qdFunc;
            _downvoteQuestionsModel = processScopeModel.GetActivitySettingsAs<DownvoteQuestionsModel>();
            _networks = SocialNetworks.Quora;
            managerFactory = qdLogInProcess.managerFactory;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            _browser = managerFactory.QdBrowserManager();
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var quoraUser = (QuoraUser)scrapeResult.ResultUser;
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, quoraUser.Url);
            var jobProcessResult = new JobProcessResult();
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var IsSuccess = false;
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    var url = quoraUser.Url;
                    var linkResp = quoraFunct.QuestionDetails(DominatorAccountModel, url).Response.Response;
                    var downvoteQuestionResponse = quoraFunct.DownvoteQuestion(DominatorAccountModel, quoraUser, linkResp).Result;
                    IsSuccess = downvoteQuestionResponse.Success;
                }
                else
                {
                    var linkresp = _browser.SearchByCustomUrl(DominatorAccountModel, quoraUser.Url).Response;
                    IsSuccess = _browser.DownVoteQuestion(quoraUser.Url, DominatorAccountModel);
                }
                if (IsSuccess)
                {
                    AddUpvotedQuestionDataToDataBase(scrapeResult);
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, quoraUser.Url);
                    //PostDownvoteProcess(ObjQuoraUser);
                    IncrementCounters();
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

            DelayBeforeNextActivity();

            return jobProcessResult;
        }

        private void AddUpvotedQuestionDataToDataBase(ScrapeResultNew scrapeResult)
        {
            var quoraUser = (QuoraUser)scrapeResult.ResultUser;

            #region Add to CampaignDb

            if (!string.IsNullOrEmpty(CampaignId))
            {
                IDbCampaignService dbCampaignService = new DbCampaignService(CampaignId);
                dbCampaignService.Add(new DominatorHouseCore.DatabaseHandler.QdTables.Campaigns.InteractedQuestion
                {
                    ActivityType = ActivityType.ToString(),
                    InteractionDateTime = DateTime.Now,
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    QuestionUrl = quoraUser.Url,
                    Accountusername = DominatorAccountModel.UserName
                });
            }

            #endregion

            #region Add to AccountDB

            var dbAccountService =
                InstanceProvider.ResolveAccountDbOperations(DominatorAccountModel.AccountId, _networks);
            dbAccountService.Add(new InteractedQuestion
            {
                ActivityType = ActivityType.ToString(),
                InteractionDateTime = DateTime.Now,
                QueryType = scrapeResult.QueryInfo.QueryType,
                QueryValue = scrapeResult.QueryInfo.QueryValue,
                QuestionUrl = quoraUser.Url,
                Accountusername = DominatorAccountModel.UserName
            });

            #endregion

            #region Add to PrivateBlacklist DB

            if (_downvoteQuestionsModel.IsChkDownvoteQuestionPrivateBlacklist)
                dbAccountService.Add(
                    new PrivateBlacklist
                    {
                        UserName = quoraUser.Username,
                        UserId = quoraUser.UserId,
                        InteractionTimeStamp = GetEpochTime()
                    });

            #endregion

            #region Add to GroupBlacklist DB

            if (_downvoteQuestionsModel.IsChkDownvoteQuestionGroupBlacklist)
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
    }
}