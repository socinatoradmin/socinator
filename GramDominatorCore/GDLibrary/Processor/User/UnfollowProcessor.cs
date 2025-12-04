using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class UnfollowProcessor : BaseInstagramUserProcessor
    {
        private readonly UnfollowerModel _unfollowerModel;
        public UnfollowProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService _delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, _delayService,gdBrowserManager)
        {
            _unfollowerModel = JsonConvert.DeserializeObject<UnfollowerModel>(TemplateModel.ActivitySettings);
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            if (_unfollowerModel.IsUnfollowFollowings)
            {
                try
                {
                    Token.ThrowIfCancellationRequested();
                    List<InstagramUser> followingsUsers;
                    int noOfUserCheked = 0;
                    //Get Following of Respective account
                    followingsUsers = GetFollowers();

                    DateTime lastTime = DateTime.Now;
                    bool gotActivityToPerform = false;

                    var usersList = FilterWhitelistBlacklistUsers(followingsUsers);
                    if (usersList == null)
                        usersList = followingsUsers;

                    if (_unfollowerModel.IsUseFilterBySourceType)
                        usersList = SourceTypeUser(usersList);

                    if (_unfollowerModel.IsChkPeopleFollowedBySoftwareChecked && _unfollowerModel.IsChkCancelPrivateRequest &&
                        !_unfollowerModel.IsUseFilterBySourceType)
                    {
                        var privateRequest = DbAccountService.GetInteractedUserFriends().Where(req =>
                            req.Username == DominatorAccountModel.UserName && req.Status == "Requested").ToList();
                        var userUnfollowedBySoftware = DbAccountService.GetUnfollowedUser()
                            .Where(req => req.AccountUsername == DominatorAccountModel.UserName).ToList();
                        if (userUnfollowedBySoftware.Count > 0)
                            privateRequest.RemoveAll(x => userUnfollowedBySoftware.Any(y => y.UnfollowedUsername == x.InteractedUsername));


                        var privateFollowings = GetFollowingUser(privateRequest);
                        UnfollowUserProcess(jobProcessResult, privateFollowings);
                    }
                    if (_unfollowerModel.IsChkPeopleFollowedBySoftwareChecked && !_unfollowerModel.IsChkPeopleFollowedOutsideSoftwareChecked && _unfollowerModel.IsChkCancelPrivateRequest &&

                        _unfollowerModel.IsUseFilterBySourceType && _unfollowerModel.IsWhoDoNotFollowBackChecked)
                    {
                        var privateRequest = DbAccountService.GetInteractedUserFriends().Where(req =>
                                req.Username == DominatorAccountModel.UserName && req.Status == "Requested").Distinct()
                            .ToList();
                        var userUnfollowedBySoftware = DbAccountService.GetUnfollowedUser()
                            .Where(req => req.AccountUsername == DominatorAccountModel.UserName).ToList();
                        if (userUnfollowedBySoftware.Count > 0)
                            privateRequest.RemoveAll(x => userUnfollowedBySoftware.Any(y => y.UnfollowedUsername == x.InteractedUsername));


                        var privateFollowings = GetFollowingUser(privateRequest);
                        UnfollowUserProcess(jobProcessResult, privateFollowings);
                    }
                    else
                    {
                        foreach (var eachUser in usersList)
                        {
                            Token.ThrowIfCancellationRequested();
                            try
                            {
                                if (!gotActivityToPerform && lastTime.AddMinutes(1) < DateTime.Now)
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                        "Finding users as per the query and filters set is taking time. Please wait..");
                                gotActivityToPerform = true;
                                Token.ThrowIfCancellationRequested();
                                var instaUser = GetEachUser(eachUser);
                                UserFriendshipResponse userFriendshipResponse = null;
                                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                                {
                                    userFriendshipResponse = InstaFunction.UserFriendship(DominatorAccountModel, AccountModel, instaUser.Pk);
                                }
                                else
                                {
                                    //var userInfo = InstaFunction.GdBrowserManager.GetUserInfo(DominatorAccountModel, instaUser.Username, Token);
                                    var userInfo = InstaFunction.SearchUsername(DominatorAccountModel, instaUser.Username, Token);
                                    var res = userInfo.ToString();
                                    userFriendshipResponse = new UserFriendshipResponse(new ResponseParameter { Response = res });
                                    instaUser = MapUserDetails(instaUser, userFriendshipResponse);
                                }
                                DelayService.ThreadSleep(TimeSpan.FromSeconds(2));// System.Threading.Thread.Sleep(2000);
                                if (userFriendshipResponse.Success)
                                {
                                    if (!CheckingLoginRequiredResponse(userFriendshipResponse.ToString(), "", queryInfo))
                                        return;
                                    instaUser.OutgoingRequest = userFriendshipResponse.OutgoingRequest;
                                    if (!(userFriendshipResponse.Following || userFriendshipResponse.OutgoingRequest))
                                    {
                                        if (_unfollowerModel.IsChkCustomUsersListChecked)
                                        {
                                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                                DominatorAccountModel.UserName, ActivityType,
                                                $"{instaUser.Username} is not followed by {DominatorAccountModel.UserName}, hence skipped");
                                        }

                                        continue;
                                    }

                                    if (_unfollowerModel.IsChkCancelPrivateRequest && _unfollowerModel.IsUseFilterBySourceType &&
                                        _unfollowerModel.IsWhoDoNotFollowBackChecked && userFriendshipResponse.OutgoingRequest)
                                    {
                                        GlobusLogHelper.log.Info(Log.CustomMessage,
                                           DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                           DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                           $"This user {instaUser.Username} has not accepted your request");
                                        continue;
                                    }

                                    if (_unfollowerModel.IsChkCancelPrivateRequest && !_unfollowerModel.IsUseFilterBySourceType &&
                                        _unfollowerModel.IsWhoDoNotFollowBackChecked && !userFriendshipResponse.OutgoingRequest)
                                    {
                                        GlobusLogHelper.log.Info(Log.CustomMessage,
                                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                            $"This user {instaUser.Username} has  accepted your request");
                                        continue;
                                    }

                                    if (_unfollowerModel.IsUseFilterBySourceType &&
                                        _unfollowerModel.IsWhoDoNotFollowBackChecked && userFriendshipResponse.FollowedBack)
                                    {
                                        noOfUserCheked++;
                                        if (noOfUserCheked == 100)
                                        {
                                            noOfUserCheked = 0;
                                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                                $"Please wait we are trying to find user who do not follow back you,it may take a while");
                                        }
                                        continue;
                                    }
                                    else if (_unfollowerModel.IsUseFilterBySourceType &&
                                             _unfollowerModel.IsWhoFollowBackChecked && !userFriendshipResponse.FollowedBack)
                                        continue;
                                }
                                Token.ThrowIfCancellationRequested();
                                FilterAndStartFinalProcessForOneUser(QueryInfo.NoQuery, ref jobProcessResult, instaUser);
                                noOfUserCheked = 0;
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }
                        if(usersList.Count == 0)
                        {
                            jobProcessResult.HasNoResult = true;
                            jobProcessResult.IsProcessCompleted = true;
                        }

                    }
                    // CheckAndStopProcessForNoMoreData(jobProcessResult);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (AggregateException ae)
                {
                    ae.HandleOperationCancellation();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                finally
                {
                    if (_unfollowerModel != null && _unfollowerModel.IsChkCustomUsersListChecked)
                        jobProcessResult.IsProcessCompleted = true;
                }
            }
            if(_unfollowerModel.IsUnfollowFollowers)
            {
                Token.ThrowIfCancellationRequested();
                try
                {                                      
                    if (_unfollowerModel.RemoveAllFollowUsers)
                    {
                        List<InstagramUser> lstFollowers = GetAccountFollowers();
                        foreach (var Follower in lstFollowers)
                        {
                            FilterAndStartFinalProcessForOneUser(QueryInfo.NoQuery, ref jobProcessResult, Follower);
                        }
                    }
                    else
                    {
                        if (_unfollowerModel.IsChkCustomFollowUsersListChecked && !string.IsNullOrEmpty(_unfollowerModel.CustomFollowUsersList))
                        {
                            _unfollowerModel.LstFollowCustomUser = Regex.Split(_unfollowerModel.CustomFollowUsersList, "\r\n").ToList();
                            foreach(string username in _unfollowerModel.LstFollowCustomUser)
                            {
                                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                                {
                                    var FollowerInfo = InstaFunction.SearchUsername(DominatorAccountModel, username, Token);
                                    FilterAndStartFinalProcessForOneUser(QueryInfo.NoQuery, ref jobProcessResult, FollowerInfo);
                                }
                                else
                                {
                                    UsernameInfoIgResponseHandler FollowerInfo = new UsernameInfoIgResponseHandler(new ResponseParameter { Response = "<!DOCTYPE html>" });
                                    FollowerInfo.Username = username;
                                    FilterAndStartFinalProcessForOneUser(QueryInfo.NoQuery, ref jobProcessResult, FollowerInfo);
                                }
                            }
                        }                   
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (AggregateException ae)
                {
                    ae.HandleOperationCancellation();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                finally
                {
                    if(_unfollowerModel!=null && _unfollowerModel.IsChkCustomUsersListChecked)
                        jobProcessResult.IsProcessCompleted= true;
                }
            }
        }

        private InstagramUser MapUserDetails(InstagramUser instaUser, UserFriendshipResponse userFriendshipResponse)
        {
            try
            {
                instaUser.IsFollowing = userFriendshipResponse.Following;
                instaUser.IsPrivate = userFriendshipResponse.IsPrivate;
                instaUser.IsBlocking = userFriendshipResponse.Blocking;
                instaUser.OutgoingRequest = userFriendshipResponse.OutgoingRequest;
            }
            catch { }
            return instaUser;
        }

        private void CheckAndStopProcessForNoMoreData(JobProcessResult jobProcessResult)
        {
            if (!IsProcessCompleted(jobProcessResult))
            {
                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.SocialNetworks,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, JobProcess.ActivityType);
                JobProcess.Stop();
                Token.ThrowIfCancellationRequested();
            }
        }

        private bool IsProcessCompleted(JobProcessResult jobProcessResult)
        {
            return jobProcessResult.IsProcessCompleted;
        }

        private int GetFinalHours()
        {
            var lastAccountUpdate = DbAccountService.Get<DailyStatitics>().Last();
            DateTime todaydateTime = DateTime.Now;
            int lastUpdatehours = lastAccountUpdate.Date.Hour;
            int nowUpdateHour = todaydateTime.Hour;
            int finalHour = nowUpdateHour - lastUpdatehours;
            return finalHour;
        }

        private List<InstagramUser> GetFollowers()
        {
            List<InstagramUser> followingsUsers = new List<InstagramUser>();
            try
            {
                List<Friendships> allFollowings;
                List<InteractedUsers> allInteractedUser;


                //here we are checking users who followed this{1-2 etc} days before 
                if ((_unfollowerModel.IsChkPeopleFollowedBySoftwareChecked ||
                    _unfollowerModel.IsChkPeopleFollowedOutsideSoftwareChecked) && _unfollowerModel.IsUserFollowedBeforeChecked)
                {
                    #region Operation for users followed before particular time
                    var dboperation = new DbOperations(DominatorAccountModel.AccountBaseModel.AccountId,
                        SocialNetworks.Instagram, ConstantVariable.GetAccountDb);
                    int userFollowedTimeCriteriaInEpoch = GetTime();

                    allFollowings = dboperation.Get<Friendships>(x =>x.Followings == 1 && x.FollowType == 0 && x.Time < userFollowedTimeCriteriaInEpoch);
                    allFollowings = DbAccountService.GetFollowings().Where(x => x.FollowType == 0 && x.Time < userFollowedTimeCriteriaInEpoch).ToList();
                    followingsUsers = GetFollowingUser(null, allFollowings);
                    #endregion
                }

                if (_unfollowerModel.IsChkCustomUsersListChecked)
                {
                    #region Get Followings from Custom list of users

                    if (_unfollowerModel.LstCustomUser == null)
                    {
                        _unfollowerModel.LstCustomUser = Regex.Split(_unfollowerModel.CustomUsersList, "\r\n").ToList();
                    }
                    if (_unfollowerModel.IsUserFollowedBeforeChecked)
                    {
                        var dboperation = new DbOperations(DominatorAccountModel.AccountBaseModel.AccountId,
                       SocialNetworks.Instagram, ConstantVariable.GetAccountDb);
                        int userFollowedTimeCriteriaInEpoch = GetTime();
                        allInteractedUser = dboperation.Get<InteractedUsers>(x => x.Time > userFollowedTimeCriteriaInEpoch).ToList();
                        _unfollowerModel.LstCustomUser = _unfollowerModel.LstCustomUser.Distinct().ToList();
                        foreach (InteractedUsers user in allInteractedUser)
                        {
                            if (_unfollowerModel.LstCustomUser.Contains(user.InteractedUsername))
                                _unfollowerModel.LstCustomUser.Remove(user.InteractedUsername);
                        }
                        if (_unfollowerModel.LstCustomUser.Count != 0)
                        {
                            _unfollowerModel.LstCustomUser.ForEach(eachUser =>
                            {
                                if (followingsUsers.All(x => x.Username != eachUser))
                                    followingsUsers.Add(new InstagramUser() { Username = eachUser });
                            });
                        }

                    }
                    else
                    {
                        _unfollowerModel.LstCustomUser = _unfollowerModel.LstCustomUser.Distinct().ToList();

                        _unfollowerModel.LstCustomUser.ForEach(eachUser =>
                        {
                            if (followingsUsers.All(x => x.Username != eachUser))
                                followingsUsers.Add(new InstagramUser() { Username = eachUser });
                        });
                    }
                    #endregion
                }

                #region  by software and outsidesoftware

                //if both software and outsidesoftware is check then we are getting following using request hit
                //but if followedBefore options is check then we are getting following from Database
                if (_unfollowerModel.IsChkPeopleFollowedBySoftwareChecked &&
                            _unfollowerModel.IsChkPeopleFollowedOutsideSoftwareChecked)
                {

                    //if (unfollowerModel.IsUserFollowedBeforeChecked)
                    //{
                    //    var dboperation = new DbOperations(DominatorAccountModel.AccountBaseModel.AccountId,
                    //        SocialNetworks.Instagram, ConstantVariable.GetAccountDb);
                    //    int userFollowedTimeCriteriaInEpoch = GetTime();
                    //    allFollowings = dboperation.Get<Friendships>(x =>
                    //        x.Followings == 1 && x.FollowType == 0 && x.Time < userFollowedTimeCriteriaInEpoch);
                    //    allFollowings = DbAccountService.GetFollowings()
                    //        .Where(x => x.FollowType == 0 && x.Time < userFollowedTimeCriteriaInEpoch).ToList();
                    //    followingsUsers = GetFollowingUser(null, allFollowings);
                    //}
                    //else
                    //{
                        AccountModel.LstFollowings = GetFollowingUsers();
                        followingsUsers = AccountModel.LstFollowings;
                   // }


                
                    if (ModuleSetting.IsChkCancelPrivateRequest)
                    {
                        var privateRequest = DbAccountService.GetInteractedUserFriends().Where(req =>
                            req.Username == DominatorAccountModel.UserName && req.Status == "Requested").ToList();
                        List<InstagramUser> requestedPrivateUserLists = getAllRequstedUserForBoth(privateRequest);
                        followingsUsers.AddRange(requestedPrivateUserLists);
                    }
                    var unfollowedUser = DbAccountService.GetUnfollowedUser()
                        .Where(req => req.AccountUsername == DominatorAccountModel.UserName).ToList();

                    if (unfollowedUser.Count > 0)
                    {
                        followingsUsers.RemoveAll(x =>
                            unfollowedUser.Any(y => y.UnfollowedUsername == x.Username));
                    }
                }
                #endregion

                #region outsidesoftware
                //here we did two thing first is that we are getting following from Database then remove user which is already unfollowed by software
                //second is if we dont get user form Database then for getting following we are hitting request based on last friendship update 
                else if (_unfollowerModel.IsChkPeopleFollowedOutsideSoftwareChecked)
                {
                    List<Friendships> usersFollowedOutsideOfSoftware =
                        DbAccountService.GetFollowings().Where(user => !user.IsFollowBySoftware).ToList();

                    if (_unfollowerModel.IsUserFollowedBeforeChecked)
                    {
                        int userFollowedTimeCriteriaInEpoch = GetTime();
                        usersFollowedOutsideOfSoftware = usersFollowedOutsideOfSoftware
                            .Where(c => c.Time < userFollowedTimeCriteriaInEpoch).ToList();
                    }

                    var userUnfollowedByOutsideOfSoftware = DbAccountService.GetUnfollowedUser()
                                    .Where(req => req.AccountUsername == DominatorAccountModel.UserName).ToList();

                    if (ModuleSetting.IsChkCancelPrivateRequest)
                    {
                        var privateRequest = DbAccountService.GetInteractedUserFriends().Where(req =>
                            req.Username == DominatorAccountModel.UserName && req.Status == "Requested").ToList();
                        List<Friendships> privateRequested = GetAllRequestedUser(privateRequest);
                        usersFollowedOutsideOfSoftware.AddRange(privateRequested);
                    }
                    if (userUnfollowedByOutsideOfSoftware.Count > 0)
                    {
                        usersFollowedOutsideOfSoftware.RemoveAll(x =>
                            userUnfollowedByOutsideOfSoftware.Any(y => y.UnfollowedUsername == x.Username));
                    }
                    followingsUsers = GetFollowingUser(null, usersFollowedOutsideOfSoftware);
                    if (usersFollowedOutsideOfSoftware.Count == 0)
                    {
                        int lastUpdateTime = GetFinalHours();
                        if (lastUpdateTime > 12)
                        {
                            AccountModel.LstFollowings = GetFollowingUsers();
                            followingsUsers = AccountModel.LstFollowings;

                        }
                    }
                }
                #endregion

                #region Software
                else if (_unfollowerModel.IsChkPeopleFollowedBySoftwareChecked)
                {

                    //here we are getting users from database
                    List<InteractedUsers> usersFollowedBySoftware;
                    var users = DbAccountService.GetInteractedUserFriends();
                    usersFollowedBySoftware = DbAccountService.GetInteractedUserFriends().Where(req =>
                       req.Username == DominatorAccountModel.UserName && req.Status == "Followed" ||
                       req.Status == "Requested").ToList();
                    if (_unfollowerModel.IsUserFollowedBeforeChecked)
                    {
                        int userFollowedTimeCriteriaInEpoch = GetTime();
                        usersFollowedBySoftware = usersFollowedBySoftware
                            .Where(c => c.Time < userFollowedTimeCriteriaInEpoch).ToList();
                    }

                    followingsUsers = GetFollowingUser(usersFollowedBySoftware);
                }
                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return followingsUsers;
        }

        private List<InstagramUser> SourceTypeUser(List<InstagramUser> usersList)
        {
            // Filter users according to Source Type Filteration like : 
            // i) Who did not follow back 
            // ii) Who followed back


            //if do not follow back is check then firstly we are removing user from following list which is already followed 
            if (_unfollowerModel.IsUseFilterBySourceType &&
                _unfollowerModel.IsWhoDoNotFollowBackChecked)
            {
                List<Friendships> lstFollowers =
                    DbAccountService.GetFollowers().Where(x => x.Followers == 1).ToList();
                usersList.RemoveAll(x => lstFollowers.Any(y => y.Username == x.Username));
                if (usersList.Count > 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.UserName, ActivityType,
                        $"Please wait we are searching users who do not follow back");
                }
            }
            else if (_unfollowerModel.IsUseFilterBySourceType &&
                _unfollowerModel.IsWhoFollowBackChecked && !_unfollowerModel.IsChkCustomUsersListChecked)
            {
                List<Friendships> lstFollowers =
                DbAccountService.GetFollowers().Where(x => x.Followers == 1).ToList();
                usersList = GetAllUser(lstFollowers);
                if (usersList.Count > 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,DominatorAccountModel.AccountBaseModel.AccountNetwork,DominatorAccountModel.UserName, ActivityType,
                        $"Please wait we are searching users follow back");
                }

            }
            return usersList;
        }

        private List<Friendships> GetAllRequestedUser(List<InteractedUsers> privateUser)
        {
            List<Friendships> privateUserList = new List<Friendships>();
            Friendships friendships = new Friendships();
            foreach (var user in privateUser)
            {
                friendships.Username = user.InteractedUsername;
                friendships.UserId = user.InteractedUserId;
                friendships.HasAnonymousProfilePicture = user.IsProfilePicAvailable;
                friendships.IsPrivate = user.IsPrivate;
                friendships.IsVerified = user.IsVerified;
                friendships.ProfilePicUrl = user.ProfilePicUrl;
                privateUserList.Add(friendships);
            }
            return privateUserList;
        }

        private List<InstagramUser> getAllRequstedUserForBoth(List<InteractedUsers> privateUser)
        {
            List<InstagramUser> privateUserList = new List<InstagramUser>();
            InstagramUser friendships = new InstagramUser();
            foreach (var user in privateUser)
            {
                friendships.Username = user.InteractedUsername;
                friendships.UserId = user.InteractedUserId;
                friendships.HasAnonymousProfilePicture = user.IsProfilePicAvailable;
                friendships.IsPrivate = user.IsPrivate;
                friendships.IsVerified = user.IsVerified;
                friendships.ProfilePicUrl = user.ProfilePicUrl;
                privateUserList.Add(friendships);
            }
            return privateUserList;
        }

        private List<InstagramUser> GetFollowingUser(List<InteractedUsers> followingUser, List<Friendships> followingUsers = null)
        {
            List<InstagramUser> followingsUsers = new List<InstagramUser>();
            if (followingUser != null)
            {
                followingUser.ForEach(user =>
                {
                    followingsUsers.Add(new InstagramUser()
                    {
                        Username = user.InteractedUsername,
                        UserId = user.InteractedUserId,
                        Pk = user.InteractedUserId,
                        IsPrivate = user.IsPrivate,
                        IsVerified = user.IsVerified,
                        IsBusiness = user.IsBusiness,
                        ProfilePicUrl = user.ProfilePicUrl
                    });
                });
            }
            else
            {
                if (followingUsers != null)
                    followingUsers.ForEach(user =>
                    {
                        followingsUsers.Add(new InstagramUser
                        {
                            Username = user.Username,
                            UserId = user.UserId,
                            IsPrivate = user.IsPrivate,
                            Pk = user.UserId,
                            IsVerified = user.IsVerified,
                            IsBusiness = user.IsBusiness,
                            FullName = user.FullName,
                            HasAnonymousProfilePicture = user.HasAnonymousProfilePicture,
                            ProfilePicUrl = user.ProfilePicUrl
                        });
                    });
            }
            return followingsUsers;
        }

        private void UnfollowUserProcess(JobProcessResult jobProcessResult, List<InstagramUser> followingUsers)
        {
            var usersList = FilterWhitelistBlacklistUsers(followingUsers);
            int noOfUserCheked = 0;
            if (usersList == null)
                usersList = followingUsers;

            foreach (var eachUser in usersList)
            {
                try
                {
                    var instaUser = GetEachUser(eachUser);
                    if (_unfollowerModel.IsUseFilterBySourceType && _unfollowerModel.IsWhoDoNotFollowBackChecked)
                    {
                        UserFriendshipResponse userFriendshipResponse =
                            InstaFunction.UserFriendship(DominatorAccountModel, AccountModel, instaUser.Pk);
                        DelayService.ThreadSleep(TimeSpan.FromSeconds(2)); //System.Threading.Thread.Sleep(2000);
                        instaUser.OutgoingRequest = userFriendshipResponse.OutgoingRequest;

                        if (_unfollowerModel.IsChkCancelPrivateRequest &&
                            userFriendshipResponse.OutgoingRequest)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"This user {instaUser.Username} has not accepted your request");
                            continue;
                        }


                        if (_unfollowerModel.IsUseFilterBySourceType &&
                            _unfollowerModel.IsWhoDoNotFollowBackChecked && userFriendshipResponse.FollowedBack)
                        {

                            noOfUserCheked++;
                            if (noOfUserCheked == 10)
                            {
                                noOfUserCheked = 0;
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    $"Please wait we are trying to find user who do not follow back you");
                            }
                            continue;
                        }


                        else if (_unfollowerModel.IsUseFilterBySourceType &&
                                 _unfollowerModel.IsWhoFollowBackChecked && !userFriendshipResponse.FollowedBack)
                            continue;
                    }
                    else
                    {
                        UserFriendshipResponse userFriendshipResponse =
                            InstaFunction.UserFriendship(DominatorAccountModel, AccountModel, instaUser.Pk);
                        DelayService.ThreadSleep(TimeSpan.FromSeconds(2));//System.Threading.Thread.Sleep(2000);
                        instaUser.OutgoingRequest = userFriendshipResponse.OutgoingRequest;
                        if (_unfollowerModel.IsChkCancelPrivateRequest &&
                            !userFriendshipResponse.OutgoingRequest)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"This user {instaUser.Username} has accepted your request");
                            continue;
                        }
                    }
                    Token.ThrowIfCancellationRequested();
                    FilterAndStartFinalProcessForOneUser(QueryInfo.NoQuery, ref jobProcessResult, instaUser);
                    noOfUserCheked = 0;
                }
                catch (OperationCanceledException)
                {

                    throw;
                }
                catch (AggregateException e)
                {
                    foreach (Exception ex in e.InnerExceptions)
                        Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            CheckAndStopProcessForNoMoreData(jobProcessResult);
        }

        private InstagramUser GetEachUser(InstagramUser eachUser)
        {
            InstagramUser instaUser = new InstagramUser();
            if (string.IsNullOrEmpty(eachUser.Pk))
            {
                if (string.IsNullOrEmpty(eachUser.UserId))
                {
                    if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        instaUser = InstaFunction.SearchUsername(DominatorAccountModel, eachUser.Username, Token);
                        DelayService.ThreadSleep(TimeSpan.FromSeconds(1));//System.Threading.Thread.Sleep(1000); 
                    }
                }
                else
                    instaUser.Pk = eachUser.UserId;
                instaUser.Username = eachUser.Username;
            }
            else
            {
                instaUser = eachUser;
            }
            return instaUser;
        }

        private int GetTime()
        {
            TimeSpan userFollowedTimeSpan = new TimeSpan(_unfollowerModel.FollowedBeforeDay,
                _unfollowerModel.FollowedBeforeHour, 0, 0);

            int userFollowedTimeCriteriaInEpoch =
                DateTime.Now.Subtract(userFollowedTimeSpan).ConvertToEpoch();
            return userFollowedTimeCriteriaInEpoch;
        }
    }
}

