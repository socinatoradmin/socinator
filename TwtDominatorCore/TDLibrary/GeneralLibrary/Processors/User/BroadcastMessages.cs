using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.User
{
    internal class BroadcastMessages : BaseTwitterUserProcessor, IQueryProcessor
    {
        private readonly ITDAccountUpdateFactory _accountUpdateFactory;
        private readonly MessageModel messageModel;

        public BroadcastMessages(ITdJobProcess jobProcess,
            IBlackWhiteListHandler blackWhiteListHandler, IDbCampaignService campaignService,
            ITwitterFunctionFactory twitterFunctionFactory, ITDAccountUpdateFactory accountUpdateFactory,
            IProcessScopeModel processScopeModel, IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                dbInsertionHelper)
        {
            _accountUpdateFactory = accountUpdateFactory;
            messageModel = processScopeModel.GetActivitySettingsAs<MessageModel>();
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            if (_jobProcess.checkJobCompleted()) return;

            jobProcessResult = new JobProcessResult();
            if (_jobProcess.ModuleSetting.MessageSetting.IsChkCustomFollowers)
            {
                queryInfo = new QueryInfo {QueryType = "Custom Followers"};
                MessageCustomFollowers(queryInfo, out jobProcessResult);
            }

            if (_jobProcess.ModuleSetting.MessageSetting.IsChkRandomFollowers)
            {
                queryInfo = new QueryInfo {QueryType = "Random Followers"};
                MessageRandomFollowers(queryInfo, out jobProcessResult);
            }


            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
        }

        private void MessageRandomFollowers(QueryInfo query, out JobProcessResult jobProcessResult)
        {
            jobProcessResult = new JobProcessResult();
            _accountUpdateFactory.UpdateFollowers(_jobProcess.DominatorAccountModel, TwitterFunction);

            var followersList = GetFriendshipsFromDb(FollowType.Followers);
            if (followersList != null && followersList.Count <= 0)
                return;

            followersList = FilterAlreadyMessagedUserById(followersList);

            followersList.Shuffle();
            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            jobProcessResult = StartFinalProcess(query, jobProcessResult, followersList);
        }


        private void MessageCustomFollowers(QueryInfo query, out JobProcessResult jobProcessResult)
        {
            jobProcessResult = new JobProcessResult();
            List<string> listOfCustomUser;

            if (_jobProcess.ModuleSetting.MessageSetting.CustomFollowersList == null &&
                !string.IsNullOrEmpty(_jobProcess.ModuleSetting.MessageSetting.CustomFollowers.Trim()))
            {
                if (_jobProcess.ModuleSetting.MessageSetting.CustomFollowers.Trim().Contains(","))
                    listOfCustomUser = _jobProcess.ModuleSetting.MessageSetting.CustomFollowers.Split(',')
                        .Select(x => x.Trim())
                        .ToList();

                else
                    listOfCustomUser = _jobProcess.ModuleSetting.MessageSetting.CustomFollowers.Split('\n')
                        .Select(x => x.Trim()).ToList();
            }

            else
            {
                listOfCustomUser = _jobProcess.ModuleSetting.MessageSetting.CustomFollowersList;
            }

            // filter alreadyMessagedUsers
            listOfCustomUser = FilterAlreadyMessagedCustomUser(listOfCustomUser);

            // SkipBlackListOrWhiteList
            listOfCustomUser = SkipBlackListOrWhiteList(listOfCustomUser);

            if(listOfCustomUser.Count == 0)
                GlobusLogHelper.log.Info(Log.CustomMessage,
                            _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            _jobProcess.DominatorAccountModel.UserName,
                            ActivityType,
                            $"Skipped User {_jobProcess.ModuleSetting.MessageSetting.CustomFollowers}");

            foreach (var eachUser in listOfCustomUser)
            {
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                try
                {
                    var response = TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel, eachUser.Trim(),
                        query.QueryType);

                    if (_jobProcess.ModuleSetting.IsCampaignWiseUniqueChecked &&
                        _dbAccountService.IsActivityDoneWithThisUserId(response.UserDetail.UserId, ActivityType))
                        continue;
                    if (response.Success)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            _jobProcess.DominatorAccountModel.UserName,
                            ActivityType,
                            string.Format("LangKeySendingMessageUsers".FromResourceDictionary(), eachUser));
                        FinalProcessForEachUser(query, out jobProcessResult, response.UserDetail);
                    }
                    //else if (!response.UserDetail.FollowBackStatus)
                    //{
                    //    GlobusLogHelper.log.Info(Log.CustomMessage,
                    //        _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    //        _jobProcess.DominatorAccountModel.UserName,
                    //        ActivityType, string.Format("LangKeyUserNotFollowing".FromResourceDictionary(), eachUser));
                    //}

                    if (jobProcessResult.IsProcessCompleted)
                        break;
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

        private List<TwitterUser> FilterAlreadyMessagedUserById(List<TwitterUser> followersList)
        {
            if (followersList?.Count == 0 ||
                !messageModel.UserFilterModel.IsSkipUsersWhoWereAlreadySentAMessageFromSoftware)
                return followersList;

            // here we are picking/selecting the userId since it is unique and never changes
            var interactedUsersId = _dbAccountService.GetInteractedUsers(
                ActivityType.BroadcastMessages.ToString(), ActivityType.AutoReplyToNewMessage.ToString(),
                ActivityType.SendMessageToFollower.ToString()).Select(x => x.InteractedUserId).ToList();

            //selecting only those users who are not get interacted before
            followersList = followersList.Where(x => !interactedUsersId.Contains(x.UserId)).ToList();
            return followersList;
        }

        private List<string> FilterAlreadyMessagedCustomUser(List<string> listOfCustomUser)
        {
            if (listOfCustomUser?.Count == 0 ||
                !messageModel.UserFilterModel.IsSkipUsersWhoWereAlreadySentAMessageFromSoftware)
                return listOfCustomUser;

            try
            {
                // here we are picking/selecting the username, since client give username in custom userList
                var interactedUsers = _dbAccountService.GetInteractedUsers(
                    ActivityType.BroadcastMessages.ToString(), ActivityType.AutoReplyToNewMessage.ToString(),
                    ActivityType.SendMessageToFollower.ToString())?.Select(x => x.InteractedUsername)?.ToList();

                //selecting only those users who are not get interacted from custom user list
                listOfCustomUser = listOfCustomUser.Where(x => !interactedUsers.Contains(x)).ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return listOfCustomUser;
        }
    }
}