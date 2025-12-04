using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.User
{
    internal class SendMessageORWelcomeTweet : BaseTwitterUserProcessor, IQueryProcessor
    {
        private readonly ITDAccountUpdateFactory _accountUpdateFactory;
        private readonly MessageModel messageModel;

        public SendMessageORWelcomeTweet(ITdJobProcess jobProcess,
            IBlackWhiteListHandler blackWhiteListHandler, IDbCampaignService campaignService,
            ITwitterFunctionFactory twitterFunctionFactory, IProcessScopeModel processScopeModel,
            ITDAccountUpdateFactory accountUpdateFactory, IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                dbInsertionHelper)
        {
            messageModel = processScopeModel.GetActivitySettingsAs<MessageModel>();
            _accountUpdateFactory = accountUpdateFactory;
            _dbInsertionHelper = dbInsertionHelper;
        }

        private IDbInsertionHelper _dbInsertionHelper { get; }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            if (_jobProcess.checkJobCompleted()) return;

            _accountUpdateFactory.UpdateFollowers(_jobProcess.DominatorAccountModel, TwitterFunction);

            var followNotification =
                TwitterFunction.GetNewFollowedUserFromNotification(_jobProcess.DominatorAccountModel,
                    _dbInsertionHelper);
            var listOfNewFollowers = GetNewFollowers();

            #region adding notification users

            try
            {
                var users = listOfNewFollowers.Select(x => x.Username).ToList();

                foreach (var user in followNotification)
                    if (!users.Contains(user.Username))
                        listOfNewFollowers.Add(user);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            try
            {
                if (ActivityType == ActivityType.WelcomeTweet)
                {
                    var allPosts = _dbAccountService.GetInteractedPosts(ActivityType)
                        .Select(y => y.Username.ToLower()).ToList();

                    listOfNewFollowers.RemoveAll(user => allPosts.Contains(user.Username.ToLower()));
                }
                else
                {
                    var allUsersList = _dbAccountService.GetInteractedUsers(ActivityType)
                        .Select(y => y.InteractedUsername.ToLower()).ToList();

                    listOfNewFollowers.RemoveAll(user => allUsersList.Contains(user.Username.ToLower()));
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            listOfNewFollowers = FilterAlreadyMessagedUserById(listOfNewFollowers);

            #region Skip BlackList or WhiteList

            listOfNewFollowers = SkipBlackListOrWhiteList(listOfNewFollowers);

            #endregion

            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            jobProcessResult = new JobProcessResult();
            jobProcessResult = StartFinalProcess(QueryInfo.NoQuery, jobProcessResult, listOfNewFollowers);

            if (jobProcessResult.IsProcessCompleted) return;
        }

        private List<TwitterUser> FilterAlreadyMessagedUserById(List<TwitterUser> NewfollowersList)
        {
            if (NewfollowersList?.Count == 0 ||
                !messageModel.UserFilterModel.IsSkipUsersWhoWereAlreadySentAMessageFromSoftware)
                return NewfollowersList;

            //here we are picking/ selecting the userId since it is unique and never changes
            var interactedUsersId = _dbAccountService.GetInteractedUsers(
                ActivityType.BroadcastMessages.ToString(), ActivityType.AutoReplyToNewMessage.ToString(),
                ActivityType.SendMessageToFollower.ToString()).Select(x => x.InteractedUserId).ToList();

            // selecting only those users who are not get interacted before
            NewfollowersList = NewfollowersList.Where(x => !interactedUsersId.Contains(x.UserId)).ToList();
            return NewfollowersList;
        }

        private List<TwitterUser> GetNewFollowers()
        {
            var friendships = _dbAccountService.GetFriendships(FollowType.Followers, FollowType.Mutual);

            // Check if ALL are older than 1 day
            bool allAreOld = friendships.All(f =>
                (DateTime.Now - f.Time.EpochToTimeSpan()).Day > 1);

            if (allAreOld)
                return new List<TwitterUser>();

            return friendships
                .Where(f =>
                    f.FirstMessageStatus == 0 &&
                    (DateTime.Now - f.Time.EpochToTimeSpan()).Day <= 1)
                .Select(f => new TwitterUser
                {
                    UserId = f.UserId,
                    Username = f.Username,
                    FullName = f.FullName,
                    IsVerified = f.IsVerified == 1,
                    IsPrivate = f.IsPrivate == 1,
                    FollowBackStatus = true,
                    HasProfilePic = f.HasAnonymousProfilePicture == 0
                })
                .ToList();
        }

    }
}