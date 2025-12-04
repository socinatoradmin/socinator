using System;
using System.Linq;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.QdTables.Accounts;
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
    internal class BroadCastMessagesProcess : BroadCastMessageProcess
    {
        private readonly BroadcastMessagesModel _broadcastMessagesModel;

        public BroadCastMessagesProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IExecutionLimitsManager executionLimitsManager,
            IQuoraFunctions qdFunc,
            IQdQueryScraperFactory queryScraperFactory, IQdHttpHelper qdHttpHelper, IQdLogInProcess qdLogInProcess)
            : base(processScopeModel, accountServiceScoped, executionLimitsManager, qdFunc,
                queryScraperFactory, qdHttpHelper, qdLogInProcess)
        {
            _broadcastMessagesModel = processScopeModel.GetActivitySettingsAs<BroadcastMessagesModel>();
        }

        public override void StartSendingMessages(ScrapeResultNew scrapeResult, BasePostData postcontent,
            string orgjsInit,
            QuoraUser quoraUser, string checkCanMessage = null)
        {
            SendMessageResponseHandler result = null;

            var messages = _broadcastMessagesModel.LstDisplayManageMessageModel.Where(x =>
                x.LstQueries.Any(y => y.Content.QueryValue == scrapeResult.QueryInfo.QueryValue));

            messages.ForEach(message =>
            {
                postcontent.ThreadId = Utilities.GetBetween(checkCanMessage, "thread_id\\\": ", "}");

                postcontent.TargetUid = Utilities.GetBetween(checkCanMessage, "target_uid\\\": ", ",");

                if (checkCanMessage.Contains("thread_id"))
                {
                    result = quoraFunct.SendMessage(DominatorAccountModel, postcontent, orgjsInit,
                        message.MessagesText,"");
                }
                else
                {
                    postcontent.TargetUid = Utilities.GetBetween(checkCanMessage, "target_uid\\\": ", "},");
                    result = quoraFunct.SendMessageToNew(DominatorAccountModel, postcontent, orgjsInit,
                        message.MessagesText);
                }

                ShowSucessLogAndSaveToDb(scrapeResult, message.MessagesText, quoraUser, result);
            });
        }

        public override void AddToPrivateBlacklistDB(QuoraUser quorauser, string message)
        {
            #region Add to PrivateBlacklistDB 

            if (_broadcastMessagesModel.IsChkBroadCastPrivateBlacklist)
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

            if (_broadcastMessagesModel != null && _broadcastMessagesModel.IsChkBroadCastGroupBlacklist)
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