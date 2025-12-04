using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.Response;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.User
{
    internal class UnfollowUser : BaseTwitterUserProcessor, IQueryProcessor
    {
        private readonly IDbInsertionHelper _dbInsertionHelper;
        private readonly ModuleSetting _moduleSetting;
        private readonly UnFollower _unFollowSetting;
        private FollowerFollowingResponseHandler _followingsResponseHandler;

        public UnfollowUser(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, ITwitterFunctionFactory twitterFunctionFactory,
            IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                dbInsertionHelper)
        {
            _dbInsertionHelper = dbInsertionHelper;
            _moduleSetting = _jobProcess.ModuleSetting;
            _unFollowSetting = _jobProcess.ModuleSetting.Unfollower;
        }


        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                if (_jobProcess.checkJobCompleted())
                    return;

                jobProcessResult = new JobProcessResult();
                if (_unFollowSetting.IsChkCustomUsersListChecked && !jobProcessResult.IsProcessCompleted)
                {
                    queryInfo = new QueryInfo {QueryType = "Custom users"};
                    UnFollowCustomUserList(queryInfo, out jobProcessResult);
                }
                if(_unFollowSetting.IsChkPeopleFollowedBySoftwareCheecked && _unFollowSetting.IsChkPeopleFollowedOutsideSoftwareChecked && !jobProcessResult.IsProcessCompleted)
                {
                    queryInfo = new QueryInfo { QueryType = "Followed by software & Followed outside software" };
                    UnFollowPplFollowedBySoftwareAndOutsideSoftware(queryInfo, out jobProcessResult);
                }
                else
                {
                    if (_unFollowSetting.IsChkPeopleFollowedBySoftwareCheecked)
                    {
                        queryInfo = new QueryInfo { QueryType = "Followed by software" };
                        UnFollowPplFollowedBySoftware(queryInfo, out jobProcessResult);
                    }

                    if (_unFollowSetting.IsChkPeopleFollowedOutsideSoftwareChecked && !jobProcessResult.IsProcessCompleted)
                    {
                        queryInfo = new QueryInfo { QueryType = "Followed outside software" };
                        UnFollowPplFollowedOutsideSoftware(queryInfo, out jobProcessResult);
                    }
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
        }

        private void UpdateStatusForFollowBackUsers(TwitterUser user)
        {
            // adding user to db if user following back adding it to friendship db 
            // so that next time again this user not come for unFollow 
            // it will useful for account having so many followers
            if (!user.FollowBackStatus)
                return;


            // var dbInsertionHelper = new Database.DbInsertionHelper(_jobProcess.DominatorAccountModel);

            //var dbInsertionHelper = new Database.DbInsertionHelper(_jobProcess.DominatorAccountModel);
            try
            {
                _dbInsertionHelper.AddFriendshipData(user, FollowType.Followers, 0);
            }
            catch (Exception)
            {
            }
        }

        #region Private methods

        private void UnFollowPplFollowedBySoftwareAndOutsideSoftware(QueryInfo queryInfo, out JobProcessResult jobProcessResult)
        {
            jobProcessResult = new JobProcessResult();
            GlobusLogHelper.log.Info(Log.CustomMessage,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,
                    string.Format("LangKeySearchingFor".FromResourceDictionary(), queryInfo.QueryType,string.Empty));
            string minPosition = null;
            var isSavePagination = _moduleSetting.IsSavePagination &&
                                   _moduleSetting.Unfollower.IsChkPeopleFollowedOutsideSoftwareChecked &&
                                   _moduleSetting.Unfollower.IsChkPeopleFollowedBySoftwareCheecked;
            if (isSavePagination)
            {
                // In user filter we are assigning query value and in db insertion we are also saving query value
                // on saving time and getting time pagination id might become different therefore we are changing query value
                if (_unFollowSetting.IsWhoDoNotFollowBackChecked)
                    queryInfo.QueryValue = TdConstants.WhoAreNotFollowingBack;
                if (_unFollowSetting.IsWhoFollowBackChecked)
                    queryInfo.QueryValue = TdConstants.WhoAreFollowingBack;
                // var demo = GetPaginationId(queryInfo);
                minPosition = GetPaginationId(queryInfo);
            }

            var isNotFirst = false;

            try
            {
                _followingsResponseHandler = TwitterFunction.GetUserFollowingsAsync(_jobProcess.DominatorAccountModel,
                        _jobProcess.DominatorAccountModel.AccountBaseModel.ProfileId,
                        _jobProcess.DominatorAccountModel.CancellationSource.Token, minPosition)
                    .Result;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            try
            {
                while (!jobProcessResult.IsProcessCompleted && _followingsResponseHandler.HasMoreResults || _followingsResponseHandler?.ListOfTwitterUser?.Count > 0)
                {
                    if (isNotFirst)
                        _followingsResponseHandler = TwitterFunction
                            .GetUserFollowingsAsync(_jobProcess.DominatorAccountModel,
                                _jobProcess.DominatorAccountModel.AccountBaseModel.ProfileId,
                                _jobProcess.DominatorAccountModel.CancellationSource.Token, minPosition).Result;
                    if (isSavePagination)
                        AddOrUpdatePaginationId(queryInfo, minPosition, ref isNotFirst);
                    isNotFirst = true;

                    minPosition = _followingsResponseHandler.MinPosition;
                    foreach (var user in _followingsResponseHandler.ListOfTwitterUser)
                    {
                        _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            _jobProcess.DominatorAccountModel.UserName, ActivityType,
                            $"Scraped user '{user.Username}'");

                        var twtUser = user;

                        if (!UnFollowFilterApply(twtUser, ref queryInfo))
                        {
                            FinalProcessForEachUser(queryInfo, out jobProcessResult, twtUser);
                            if (jobProcessResult.IsProcessSuceessfull &&
                                _jobProcess.ModuleSetting.ManageBlackWhiteListModel.IsAddToBlackListOnceUnfollowed)
                                BlackWhiteListHandler.AddToBlackList(twtUser.UserId, twtUser.Username);
                        }

                        if (jobProcessResult.IsProcessCompleted)
                            break;
                    }
                    if (_followingsResponseHandler?.ListOfTwitterUser?.Count == 0 ||
                        _followingsResponseHandler.ListOfTwitterUser == null || !_followingsResponseHandler.HasMoreResults)
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException(" OperationCanceledException in UnFollow by software.");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        private void UnFollowPplFollowedBySoftware(QueryInfo queryInfo, out JobProcessResult jobProcessResult)
        {
            jobProcessResult = new JobProcessResult();
            GlobusLogHelper.log.Info(Log.CustomMessage,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,
                    string.Format("LangKeySearchingFor".FromResourceDictionary(), queryInfo.QueryType,string.Empty));
            var followedUserFromSoftwareDict = GetFollowedUserFromDb(true);
            // Removing whitelist or blackList users
            followedUserFromSoftwareDict = SkipBlackWhiteDict(followedUserFromSoftwareDict);
            RemovedAlreadyUnFollowedUser(followedUserFromSoftwareDict);
            //removing suspended accounts
            RemoveSuspendedUser(followedUserFromSoftwareDict);

            // randomize dictionary
            if (followedUserFromSoftwareDict.Count > 500)
                followedUserFromSoftwareDict = RandomizeDictionary(followedUserFromSoftwareDict);

            if (followedUserFromSoftwareDict.Count <= 0)
                return;

            foreach (var user in followedUserFromSoftwareDict)
            {
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                GlobusLogHelper.log.Info(Log.CustomMessage,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName,
                    ActivityType, $"Scraped user '{user.InteractedUsername}'");

                var twtUser = new TwitterUser
                {
                    UserId = user.InteractedUserId,
                    Username = user.InteractedUsername,
                    UserBio = user.Bio,
                    UserLocation = user.Location,
                    JoiningDate = user.JoinedDate,
                    ProfilePicUrl = user.ProfilePicUrl,
                    FollowersCount = user.FollowersCount,
                    FollowingsCount = user.FollowingsCount,
                    LikesCount = user.LikesCount,
                    TweetsCount = user.TweetsCount
                };

                var isScrapUser = _unFollowSetting.IsWhoDoNotFollowBackChecked ||
                                  _unFollowSetting.IsWhoFollowBackChecked;

                if (!UnFollowFilterApply(twtUser, ref queryInfo, isScrapUser))
                {
                    FinalProcessForEachUser(queryInfo, out jobProcessResult, twtUser);

                    if (jobProcessResult.IsProcessSuceessfull && _jobProcess.ModuleSetting.ManageBlackWhiteListModel
                            .IsAddToBlackListOnceUnfollowed)
                        BlackWhiteListHandler.AddToBlackList(twtUser.UserId, twtUser.Username);
                }

                if (jobProcessResult.IsProcessCompleted) break;
            }
        }


        private void UnFollowPplFollowedOutsideSoftware(QueryInfo queryInfo, out JobProcessResult jobProcessResult)
        {
            jobProcessResult = new JobProcessResult();
            GlobusLogHelper.log.Info(Log.CustomMessage,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,
                    string.Format("LangKeySearchingFor".FromResourceDictionary(), queryInfo.QueryType,string.Empty));
            // getting all the users followed from db 
            var followedUserFromSoftwareDict =
                RemoveAlreadyExistedUsersDictionary(_dbAccountService.GetInteractedUserFriends().ToList());

            string minPosition = null;
            var isSavePagination = _moduleSetting.IsSavePagination &&
                                   _moduleSetting.Unfollower.IsChkPeopleFollowedOutsideSoftwareChecked;
            if (isSavePagination)
            {
                // In user filter we are assigning query value and in db insertion we are also saving query value
                // on saving time and getting time pagination id might become different therefore we are changing query value
                if (_unFollowSetting.IsWhoDoNotFollowBackChecked)
                    queryInfo.QueryValue = TdConstants.WhoAreNotFollowingBack;
                if (_unFollowSetting.IsWhoFollowBackChecked)
                    queryInfo.QueryValue = TdConstants.WhoAreFollowingBack;
                // var demo = GetPaginationId(queryInfo);
                minPosition = GetPaginationId(queryInfo);
            }

            var isNotFirst = false;

            try
            {
                _followingsResponseHandler = TwitterFunction.GetUserFollowingsAsync(_jobProcess.DominatorAccountModel,
                        _jobProcess.DominatorAccountModel.AccountBaseModel.ProfileId,
                        _jobProcess.DominatorAccountModel.CancellationSource.Token, minPosition)
                    .Result;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            try
            {
                while (!jobProcessResult.IsProcessCompleted && _followingsResponseHandler.HasMoreResults)
                {
                    if (isNotFirst)
                        _followingsResponseHandler = TwitterFunction
                            .GetUserFollowingsAsync(_jobProcess.DominatorAccountModel,
                                _jobProcess.DominatorAccountModel.AccountBaseModel.ProfileId,
                                _jobProcess.DominatorAccountModel.CancellationSource.Token, minPosition).Result;
                    if (isSavePagination)
                        AddOrUpdatePaginationId(queryInfo, minPosition, ref isNotFirst);
                    isNotFirst = true;

                    minPosition = _followingsResponseHandler.MinPosition;

                    if (_followingsResponseHandler?.ListOfTwitterUser?.Count == 0 ||
                        _followingsResponseHandler.ListOfTwitterUser == null)
                        break;

                    _followingsResponseHandler.ListOfTwitterUser.RemoveAll(x =>
                        followedUserFromSoftwareDict.Any(y => y.InteractedUserId == x.UserId) ||
                        followedUserFromSoftwareDict.Any(y => y.InteractedUsername == x.Username));


                    foreach (var user in _followingsResponseHandler.ListOfTwitterUser)
                    {
                        _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            _jobProcess.DominatorAccountModel.UserName, ActivityType,
                            $"Scraped user '{user.Username}'");

                        var twtUser = user;

                        if (!UnFollowFilterApply(twtUser, ref queryInfo))
                        {
                            FinalProcessForEachUser(queryInfo, out jobProcessResult, twtUser);
                            if (jobProcessResult.IsProcessSuceessfull &&
                                _jobProcess.ModuleSetting.ManageBlackWhiteListModel.IsAddToBlackListOnceUnfollowed)
                                BlackWhiteListHandler.AddToBlackList(twtUser.UserId, twtUser.Username);
                        }

                        if (jobProcessResult.IsProcessCompleted)
                            break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException(" OperationCanceledException in UnFollow by software.");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void UnFollowCustomUserList(QueryInfo queryInfo, out JobProcessResult jobProcessResult)
        {
            var customUsers = _unFollowSetting.ListCustomUsers;
            customUsers = SkipBlackListOrWhiteList(customUsers);

            jobProcessResult = new JobProcessResult();

            if (customUsers == null || customUsers.Count == 0)
                return;

            // remove already followed users from list
            RemovedAlreadyUnFollowedOrSuspendedUser(customUsers);

            foreach (var user in customUsers)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName, _jobProcess.ActivityType,
                    string.Format("LangKeySearchingFor".FromResourceDictionary(), queryInfo.QueryType,
                        TdUtility.GetProfileUrl(user)));
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var userDetail =
                    TwitterFunction.GetUserDetails(_jobProcess.DominatorAccountModel, user, queryInfo.QueryType);

                if (!userDetail.Success && userDetail.UserDetail?.UserStatus != null
                                        && userDetail.UserDetail.UserStatus.Equals(TdConstants.AccountSuspended))
                {
                    // adding in to interacted user so that we can filter
                    UpdateSuspendedUser(userDetail.UserDetail.UserId, userDetail.UserDetail.Username);
                    continue;
                }

                if (userDetail.Success && userDetail.UserDetail.FollowStatus &&
                    !UnFollowFilterApply(userDetail.UserDetail, ref queryInfo))
                {
                    FinalProcessForEachUser(queryInfo, out jobProcessResult, userDetail.UserDetail);

                    if (jobProcessResult.IsProcessSuceessfull &&
                        _jobProcess.ModuleSetting.ManageBlackWhiteListModel.IsAddToBlackListOnceUnfollowed)
                        BlackWhiteListHandler.AddToBlackList(userDetail.UserDetail.UserId,
                            userDetail.UserDetail.Username);
                }

                if (jobProcessResult.IsProcessCompleted) break;
            }
        }

        /// <summary>
        ///     we will check from our friendship that user is following back we will filter them
        /// </summary>
        /// <param name="isFilterForFollowBack"></param>
        /// <returns></returns>
        protected List<InteractedUsers> GetFollowedUserFromDb(bool isFilterForFollowBack = false)
        {
            try
            {
                List<InteractedUsers> allUsers;

                if (_unFollowSetting.IsUserFollowedBeforeChecked)
                {
                    var filterDate = DateTime.UtcNow.AddDays(-_unFollowSetting.FollowedBeforeDay)
                        .AddHours(-_unFollowSetting.FollowedBeforeHour);
                    var filterTimeStamp = filterDate.GetCurrentEpochTime();

                    allUsers = _dbAccountService.GetInteractedUserFriends()
                        .Where(x => x.InteractionTimeStamp <= filterTimeStamp &&
                                    x.ProfilePicUrl != TdConstants.AccountSuspended).ToList();
                }
                else
                {
                    allUsers = _dbAccountService.GetInteractedUserFriends().ToList();
                }

                if (allUsers.Count > 100000)
                    // _delayService.ThreadSleep(15000);
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        _jobProcess.DominatorAccountModel.UserName,
                        ActivityType,
                        "Number of following Count is more than '100000'.So,it will take some time to process the data,Please Wait...");


                if (isFilterForFollowBack)
                    FilterFollowBackUsers(allUsers);

                return RemoveAlreadyExistedUsersDictionary(allUsers);
            }
            catch (Exception ex)
            {
                // handle exception
                ex.DebugLog();
                return new List<InteractedUsers>();
            }
        }

        private void FilterFollowBackUsers(List<InteractedUsers> interactedUsers)
        {
            Dictionary<string, string> dbFollowers;
            if (interactedUsers == null || interactedUsers.Count <= 0)
                return;
            dbFollowers = _dbAccountService.GetFriendships(FollowType.Followers, FollowType.Mutual)
                ?.ToDictionary(x => x.UserId, y => y.Username);

            if (dbFollowers == null)
                return;

            interactedUsers.RemoveAll(x => x.InteractedUserId == null || dbFollowers.ContainsKey(x.InteractedUserId));
        }


        /// <summary>
        ///     Handle if user db contains already existing users
        /// </summary>
        /// <param name="interactedUserFriends"></param>
        /// <returns></returns>
        protected List<InteractedUsers> RemoveAlreadyExistedUsersDictionary(
            List<InteractedUsers> interactedUserFriends)
        {
            var userIdNamePair = interactedUserFriends.GroupBy(x => x.InteractedUserId).Where(group => group.Any())
                .Select(group => group.First()).ToList();
            return userIdNamePair;
        }

        protected Dictionary<string, string> GetFollowingsFromDb(bool excludeMutual = false,
            bool excludeWhoAreNotFollowingBack = false)
        {
            var filterTimeStamp = DateTime.Now.GetCurrentEpochTime();

            if (_unFollowSetting.IsUserFollowedBeforeChecked)
            {
                var filterDate = DateTime.UtcNow.AddDays(-_unFollowSetting.FollowedBeforeDay)
                    .AddHours(-_unFollowSetting.FollowedBeforeHour);
                filterTimeStamp = filterDate.GetCurrentEpochTime();
            }

            if (excludeMutual && !excludeWhoAreNotFollowingBack)
                return _dbAccountService.GetFriendships(FollowType.Following).Where(x => x.Time <= filterTimeStamp)
                    .ToDictionary(y => y.UserId, y => y.Username);

            if (excludeWhoAreNotFollowingBack && !excludeMutual)
                return _dbAccountService.GetFriendships(FollowType.Mutual).Where(x => x.Time <= filterTimeStamp)
                    .ToDictionary(y => y.UserId, y => y.Username);

            return _dbAccountService.GetFriendships(FollowType.Mutual, FollowType.Following)
                .Where(x => x.Time <= filterTimeStamp)
                .ToDictionary(y => y.UserId, y => y.Username);
        }

        private List<InteractedUsers> SkipBlackWhiteDict(List<InteractedUsers> userIdWithName)
        {
            var skipBlackWhiteDict = new List<InteractedUsers>();
            var skippedBlackListWhiteListUsers = new List<InteractedUsers>();

            try
            {
                if (userIdWithName != null) skipBlackWhiteDict = userIdWithName;
                skippedBlackListWhiteListUsers = SkipBlackListOrWhiteList(skipBlackWhiteDict.ToList());

                // In large data list loop takes much time
                // therefore count of skip blacklist and UserIdName are same we skip it
                if (skippedBlackListWhiteListUsers.Count != 0 &&
                    skippedBlackListWhiteListUsers.Count != skipBlackWhiteDict.Count)
                    skipBlackWhiteDict = skipBlackWhiteDict
                        .Where(x => skippedBlackListWhiteListUsers.Contains(x)).ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return skipBlackWhiteDict;
        }

        private void RemovedAlreadyUnFollowedUser(List<InteractedUsers> userList)
        {
            try
            {
                var alreadyUnFollowedUserList = _dbAccountService.GetUnfollowedUsers().Select(x => x.UserId).ToList();


                foreach (var key in alreadyUnFollowedUserList)
                {
                    var removeIt = userList.FirstOrDefault(x => x.InteractedUserId == key);
                    if (removeIt != null)
                        userList.Remove(removeIt);
                }
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        private void RemoveSuspendedUser(List<InteractedUsers> userList)
        {
            try
            {
                // removing suspended or deleted accounts
                var suspendedAccounts = GetSuspendedUserIdList();

                foreach (var key in suspendedAccounts)
                {
                    var removeIt = userList.FirstOrDefault(x => x.InteractedUserId == key);
                    if (removeIt != null)
                        userList.Remove(removeIt);
                }
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        /// <summary>
        ///     Here we returning list of non existing user either it is suspended or page not found
        /// </summary>
        /// <returns></returns>
        private List<string> GetSuspendedUserIdList()
        {
            var suspendedAccounts = _dbAccountService.GetInteractedUserFriends()
                .Where(x => x.ProfilePicUrl == TdConstants.AccountSuspended).Select(x => x.InteractedUserId).ToList();
            var interactedSuspendedAccounts = _dbAccountService
                .GetList<InteractedUsers>(x => x.ProfilePicUrl == TdConstants.AccountSuspended)
                .Select(x => x.InteractedUserId).ToList();
            suspendedAccounts.AddRange(interactedSuspendedAccounts);
            suspendedAccounts = suspendedAccounts.Distinct().ToList();
            return suspendedAccounts;
        }

        public void UpdateSuspendedUser(string userId, string userName)
        {
            try
            {
                if (GetSuspendedUserNameList().Contains(userName))
                    return;
                // adding in to interacted user so that we can filter
                _dbAccountService.Add(new InteractedUsers
                {
                    ProfilePicUrl = TdConstants.AccountSuspended,
                    InteractedUserId = userId,
                    InteractedUsername = userName,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime()
                });
            }
            catch (Exception)
            {
            }
        }

        private void RemovedAlreadyUnFollowedOrSuspendedUser(List<string> userList)
        {
            try
            {
                var alreadyUnFollowedUserList =
                    _dbAccountService.GetUnfollowedUsers().Select(x => x.Username).Distinct().ToList();
                alreadyUnFollowedUserList.AddRange(GetSuspendedUserNameList());
                for (var i = 0; i < alreadyUnFollowedUserList.Count; i++)
                    userList.Remove(alreadyUnFollowedUserList[i]);
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        private List<string> GetSuspendedUserNameList()
        {
            var suspendedAccounts = _dbAccountService.GetInteractedUserFriends()
                .Where(x => x.ProfilePicUrl == TdConstants.AccountSuspended).Select(x => x.InteractedUsername).ToList();
            var interactedSuspendedAccounts = _dbAccountService
                .GetList<InteractedUsers>(x => x.ProfilePicUrl == TdConstants.AccountSuspended)
                .Select(x => x.InteractedUsername).ToList();
            suspendedAccounts.AddRange(interactedSuspendedAccounts);
            suspendedAccounts = suspendedAccounts.Distinct().ToList();
            return suspendedAccounts;
        }

        private bool UnFollowFilterApply(TwitterUser user, ref QueryInfo queryInfo, bool isScrapeDetail = false)
        {
            // after getting user details of account which is suspended we userId will get null 
            // this we need to update the user details in db so, that it will not again go for unFollow
            var userId = user.UserId;
            if (_unFollowSetting.IsWhoDoNotFollowBackChecked && _unFollowSetting.IsWhoFollowBackChecked)
            {
                queryInfo.QueryValue = "All";
                return false;
            }

            if (isScrapeDetail)
                user = TwitterFunction
                    .GetUserDetails(_jobProcess.DominatorAccountModel, user.Username, queryInfo.QueryType)?.UserDetail;

            if (user == null)
                return true;

            if (TwitterFunction.GetType().Name == "BrowserTwitterFunctions" && isScrapeDetail && !user.FollowStatus)
                return !user.FollowStatus;

            if (user.UserStatus != null && user.UserStatus.Equals(TdConstants.AccountSuspended))
            {
                var updateUser = _dbAccountService.GetInteractedUserFriends()
                    .FirstOrDefault(x => x.InteractedUserId == userId);
                if (updateUser == null)
                    return true;

                updateUser.ProfilePicUrl = TdConstants.AccountSuspended;
                _dbAccountService.Update(updateUser);
                return true;
            }

            if (_unFollowSetting.IsWhoFollowBackChecked)
            {
                queryInfo.QueryValue = TdConstants.WhoAreFollowingBack;
                return !user.FollowBackStatus;
            }

            if (_unFollowSetting.IsWhoDoNotFollowBackChecked)
            {
                UpdateStatusForFollowBackUsers(user);
                queryInfo.QueryValue = TdConstants.WhoAreNotFollowingBack;
                return user.FollowBackStatus;
            }


            if (_jobProcess.ModuleSetting.ManageBlackWhiteListModel.IsSkipWhiteListUsers)
                return BlackWhiteListHandler.GetWhiteListUsers().Contains(user.Username.ToLower());

            return false;
        }

        #endregion

        //private void ResolvingDbInsertionHelper(DominatorAccountModel dominatorAccountModel)
        //{
        //    try
        //    {
        //        var commonJobConfiguration = new DominatorHouseCore.Process.JobConfigurations.CommonJobConfiguration(new JobConfiguration(), null, false);
        //        _accountScopeFactory[dominatorAccountModel.AccountId].RegisterInstance<IProcessScopeModel>
        //            (new ProcessScopeModel(dominatorAccountModel, ActivityType.Follow, null,
        //        null, SocialNetworks.Twitter, null, null, commonJobConfiguration, null));
        //        _accountScopeFactory[dominatorAccountModel.AccountId].RegisterInstance<TDLibrary.GeneralLibrary.DAL.IDbCampaignService>(new TDLibrary.GeneralLibrary.DAL.DbCampaignService(null));
        //        // InstanceProvider.ResolveWithDominatorAccount<TDLibrary.GeneralLibrary.DAL.IDbAccountService>(dominatorAccountModel);
        //        _accountScopeFactory[dominatorAccountModel.AccountId].RegisterInstance<IDbInsertionHelper>(_accountScopeFactory[dominatorAccountModel.AccountId].Resolve<DbInsertionHelper>());
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}
    }
}