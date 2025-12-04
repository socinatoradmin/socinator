using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.User
{
    internal class FollowBack : BaseTwitterUserProcessor, IQueryProcessor
    {
        private readonly ITDAccountUpdateFactory _accountUpdateFactory;
        private readonly IDbInsertionHelper _dbInsertionHelper;

        public FollowBack(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, ITwitterFunctionFactory twitterFunctionFactory,
            ITDAccountUpdateFactory accountUpdateFactory, IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                dbInsertionHelper)
        {
            _accountUpdateFactory = accountUpdateFactory;
            _dbInsertionHelper = dbInsertionHelper;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            if (_jobProcess.checkJobCompleted())
                return;
            try
            {
                _accountUpdateFactory.UpdateFollowers(_jobProcess.DominatorAccountModel, TwitterFunction);

                var followersList = GetFriendshipsFromDb(FollowType.Followers, true);
                var alreadyFollowed = _dbAccountService.GetInteractedUsers(ActivityType).Select(x => x.InteractedUserId)
                    .ToList();

                var followNotification =
                    TwitterFunction.GetNewFollowedUserFromNotification(_jobProcess.DominatorAccountModel,
                        _dbInsertionHelper);

                if (FilterAlreadyFollowedOrSuspendedAccounts(followersList, followNotification, alreadyFollowed))

                {
                    TdAccountsBrowserDetails.CloseAllBrowser(_jobProcess.DominatorAccountModel);
                    return;
                }

                jobProcessResult = new JobProcessResult();

                jobProcessResult = StartFinalProcess(QueryInfo.NoQuery, jobProcessResult, followersList);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        private bool FilterAlreadyFollowedOrSuspendedAccounts(List<TwitterUser> followersList,
            List<TwitterUser> followNotification, List<string> alreadyFollowed)
        {
            try
            {
                var users = followersList.Select(x => x.Username).ToList();
                foreach (var user in followNotification)
                    if (!users.Contains(user.Username))
                        followersList.Add(user);

                followersList.RemoveAll(x => alreadyFollowed.Contains(x.UserId));
                // avoid calling again and again same suspended, blocked account by checking in db

                var suspendedAccountsByUserIdAndName =
                    _dbAccountService.GetList<InteractedUsers>(x =>
                            x.ProfilePicUrl == TdConstants.AccountSuspended ||
                            x.ProfilePicUrl == TdConstants.AccountBlockedToYou)
                        .Select(x => new KeyValuePair<string, string>(x.InteractedUserId, x.InteractedUsername))
                        .Distinct().ToList();
                followersList.RemoveAll(x =>
                    suspendedAccountsByUserIdAndName.Any(y => y.Key.Equals(x.UserId) || y.Value.Equals(x.Username)));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }
    }
}