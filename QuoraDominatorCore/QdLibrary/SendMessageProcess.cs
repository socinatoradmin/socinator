using System;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.QdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.Factories;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdLibrary.DAL;
using QuoraDominatorCore.Request;
using QuoraDominatorCore.Response;

namespace QuoraDominatorCore.QdLibrary
{
    internal class SendMessageProcess : BroadCastMessageProcess
    {
        private readonly SendMessageToFollowerModel _sendmessagemodel;

        public SendMessageProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IExecutionLimitsManager executionLimitsManager,
            IQuoraFunctions qdFunc,
            IQdQueryScraperFactory queryScraperFactory, IQdHttpHelper qdHttpHelper, IQdLogInProcess qdLogInProcess)
            : base(processScopeModel, accountServiceScoped, executionLimitsManager, qdFunc,
                queryScraperFactory, qdHttpHelper, qdLogInProcess)
        {
            _sendmessagemodel = processScopeModel.GetActivitySettingsAs<SendMessageToFollowerModel>();
        }

        public override void StartSendingMessages(ScrapeResultNew scrapeResult, BasePostData postcontent,
            string orgjsInit,
            QuoraUser quoraUser, string checkCanMessage = null)
        {
            SendMessageResponseHandler result;
            result = quoraFunct.SendMessageToNew(DominatorAccountModel, postcontent, orgjsInit,
                _sendmessagemodel.Message);
            ShowSucessLogAndSaveToDb(scrapeResult, _sendmessagemodel.Message, quoraUser, result);
        }

        public override void AddToPrivateBlacklistDB(QuoraUser quorauser, string message)
        {
            #region Add to PrivateBlacklistDB 

            if (ActivityType == ActivityType.SendMessageToFollower &&
                _sendmessagemodel.IschkSendMessagePrivateBlacklist)
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

            if (_sendmessagemodel != null && _sendmessagemodel.IschkSendMessageGroupBlacklist)
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