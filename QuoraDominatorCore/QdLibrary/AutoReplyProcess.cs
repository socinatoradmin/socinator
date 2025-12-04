using System;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.QdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Factories;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.Request;

namespace QuoraDominatorCore.QdLibrary
{
    internal class AutoReplyProcess : BroadCastMessageProcess
    {
        private readonly AutoReplyToNewMessageModel _autoreplymodel;

        private readonly IQuoraFunctions quoraFunct;

        public AutoReplyProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IExecutionLimitsManager executionLimitsManager,
            IQuoraFunctions qdFunc,
            IQdQueryScraperFactory queryScraperFactory, IQdHttpHelper qdHttpHelper, IQdLogInProcess qdLogInProcess)
            : base(processScopeModel, accountServiceScoped, executionLimitsManager, qdFunc,
                queryScraperFactory, qdHttpHelper, qdLogInProcess)
        {
            quoraFunct = qdFunc;
            _autoreplymodel = processScopeModel.GetActivitySettingsAs<AutoReplyToNewMessageModel>();
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            try
            {
                var quoraUser = (QuoraUser) scrapeResult.ResultUser;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var sentTime = _users.LastOrDefault(x => x.Username == quoraUser.Username)?.InteractionDate;
                if (sentTime != null && _users.Any(x => x.Username == quoraUser.Username) &&
                    DateTime.Now.Subtract((DateTime) sentTime).Hours != 24)
                {
                    var jobProcessResult = new JobProcessResult();
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $" Message has been already sent to {scrapeResult.ResultUser.Username}");
                    jobProcessResult.IsProcessSuceessfull = true;
                    return jobProcessResult;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return base.PostScrapeProcess(scrapeResult);
        }

        public override void StartSendingMessages(ScrapeResultNew scrapeResult, BasePostData postcontent,
            string orgjsInit,
            QuoraUser quoraUser, string checkCanMessage = null)
        {
            var result =
                quoraFunct.SendMessageToNew(DominatorAccountModel, postcontent, orgjsInit, _autoreplymodel.Message);
            ShowSucessLogAndSaveToDb(scrapeResult, _autoreplymodel.Message, quoraUser, result);
        }

        public override void AddToPrivateBlacklistDB(QuoraUser quorauser, string message)
        {
            #region Add to PrivateBlacklistDB 

            if (_autoreplymodel.IsChkAutoReplyPrivateBlacklist)
                DbAccountService.Add(
                    new PrivateBlacklist
                    {
                        UserName = quorauser.Username,
                        UserId = quorauser.UserId,
                        InteractionTimeStamp = DateTime.Now.GetCurrentEpochTime()
                    });

            #endregion
        }

        public override void AddToGroupBlacklistDB(QuoraUser quorauser, string message)
        {
            #region Add to GroupBlacklist DB

            if (_autoreplymodel != null && _autoreplymodel.IsChkAutoReplyGroupBlacklist)
            {
                IDbGlobalService dbGlobalService = new DbGlobalService();
                dbGlobalService.Add(new BlackListUser
                {
                    UserName = quorauser.Username,
                    UserId = quorauser.UserId,
                    AddedDateTime = DateTime.Now
                });
            }

            #endregion
        }
    }
}