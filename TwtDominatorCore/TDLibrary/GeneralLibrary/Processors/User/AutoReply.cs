using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.User
{
    internal class AutoReply : BaseTwitterUserProcessor, IQueryProcessor
    {
        private readonly MessageModel messageModel;

        public AutoReply(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, ITwitterFunctionFactory twitterFunctionFactory,
            IProcessScopeModel processScopeModel, IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                dbInsertionHelper)
        {
            messageModel = processScopeModel.GetActivitySettingsAs<MessageModel>();
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            if (_jobProcess.checkJobCompleted()) return;
            jobProcessResult = new JobProcessResult();
            var countForPagination = 0;
            GlobusLogHelper.log.Info(Log.CustomMessage,
                _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                _jobProcess.DominatorAccountModel.UserName, ActivityType, "Searching for new messages");

            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
            {
                countForPagination++;

                var responseHandler =
                    TwitterFunction.getNewMessages(_jobProcess.DominatorAccountModel, jobProcessResult.maxId);
                if (messageModel.IsReplyToMessagesThatContainSpecificWord)
                {
                    var message = new List<TwitterUser>();
                    var SpecificWordList = messageModel.SpecificWord.Split(',').Select(x => x.Trim()).ToList();
                    SpecificWordList.ForEach(word =>
                    {
                        message.AddRange(responseHandler.ListNewMessage
                            .Where(x => x.Message.ToLower().Trim().Contains(word.ToLower().Trim())).ToList());
                    });

                    responseHandler.ListNewMessage = message;
                }

                // using filter Alreadysendmessage
                responseHandler.ListNewMessage = FilterAlreadyMessagedUserById(responseHandler.ListNewMessage);

                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (responseHandler.Success)
                {
                    jobProcessResult = StartFinalProcess(QueryInfo.NoQuery, jobProcessResult,
                        responseHandler.ListNewMessage);
                    jobProcessResult.maxId = responseHandler.MinPosition;

                    if (!responseHandler.HasMore || countForPagination > TdConstants.MaxPaginationCount)
                        jobProcessResult.HasNoResult = true;
                }
                else
                {
                    jobProcessResult.maxId = null;
                    jobProcessResult.HasNoResult = true;
                }
            }
        }


        private List<TwitterUser> FilterAlreadyMessagedUserById(List<TwitterUser> MessageList)
        {
            if (MessageList?.Count == 0 ||
                !messageModel.UserFilterModel.IsSkipUsersWhoWereAlreadySentAMessageFromSoftware)
                return MessageList;

            //here we are picking/ selecting the userId since it is unique and never changes
            var interactedUsersId = _dbAccountService.GetInteractedUsers(
                ActivityType.BroadcastMessages.ToString(), ActivityType.AutoReplyToNewMessage.ToString(),
                ActivityType.SendMessageToFollower.ToString()).Select(x => x.InteractedUserId).ToList();

            // selecting only those users who are not get interacted before
            MessageList = MessageList.Where(x => !interactedUsersId.Contains(x.UserId)).ToList();
            return MessageList;
        }
    }
}