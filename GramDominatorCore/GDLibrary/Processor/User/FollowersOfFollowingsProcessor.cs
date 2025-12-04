using ThreadUtils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDModel;
using GramDominatorCore.Response;
using System;
using System.Collections.Generic;
using GramDominatorCore.GDFactories;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using Newtonsoft.Json;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class FollowersOfFollowingsProcessor : BaseInstagramUserProcessor
    {
        public BroadcastMessagesModel BroadcastMessagesModel { get; set; }
        public FollowersOfFollowingsProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService _delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, _delayService, gdBrowserManager)
        {
            BroadcastMessagesModel = JsonConvert.DeserializeObject<BroadcastMessagesModel>(TemplateModel.ActivitySettings);
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                if (CheckQueryValueOnMessageList(BroadcastMessagesModel, queryInfo)) return;
                if (string.IsNullOrEmpty(queryInfo.QueryValue))
                    CheckForNoMoreDataAndStopProcess(jobProcessResult);

                QueryType = queryInfo.QueryType;
                var instaUserList = new List<InstagramUser>();
                JobProcessResult result = new JobProcessResult();
                string maxId = null;
                bool hasnoResult = false;
                var browser = GramStatic.IsBrowser;
                var userInfo = 
                    browser ?
                    GdBrowserManager.GetUserInfo(DominatorAccountModel, queryInfo.QueryValue, Token)
                    :InstaFunction.SearchUsername(DominatorAccountModel, queryInfo.QueryValue, Token);
                if (!CheckingLoginRequiredResponse(userInfo.ToString(), "", queryInfo))
                    return;
                if (userInfo.Success)
                {
                    if (userInfo.IsPrivate && (userInfo?.instaUserDetails != null && !userInfo.instaUserDetails.IsFollowing))
                        return;
                    // Outer loop for getting followings Opertaion
                    #region Getting Followers operation
                    while (!jobProcessResult.IsProcessCompleted && !hasnoResult)
                    {
                        Token.ThrowIfCancellationRequested();

                        // Get Follower users from User provided username
                        var followingUsers = 
                            browser ?
                            GdBrowserManager.GetUserFollowings(DominatorAccountModel, userInfo.Username, Token)
                            : InstaFunction.GetUserFollowings(DominatorAccountModel, AccountModel, userInfo.Pk, Token, queryInfo.QueryType, maxId,userInfo.Username, IsWeb: true).Result;
                        if (!CheckingLoginRequiredResponse(followingUsers.ToString(), "", queryInfo))
                            return;
                        if (followingUsers.Success)
                        {
                            foreach (InstagramUser instagramUser in followingUsers.UsersList)
                            {
                                Token.ThrowIfCancellationRequested();
                                jobProcessResult.HasNoResult = false;
                                // Inner loop mentioning Followers of Followings Opertaion
                                #region Inner loop mentioning Followers of Followers Opertaion

                                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                                {
                                    FollowerAndFollowingIgResponseHandler followerOfFollowingUsers = null;
                                    Token.ThrowIfCancellationRequested();
                                    if (DominatorAccountModel.IsRunProcessThroughBrowser)
                                        followerOfFollowingUsers =InstaFunction.GetUserFollowersBrowser(DominatorAccountModel, instagramUser.Username, Token, QueryType, maxid: jobProcessResult.maxId);
                                    else
                                        followerOfFollowingUsers =InstaFunction.GetUserFollowers(DominatorAccountModel, instagramUser.Pk, Token, maxid: jobProcessResult.maxId, instagramUser.Username, IsWeb: true).Result;
                                    // Get Followers of Followers Users from intermediate users
                                    if (!CheckingLoginRequiredResponse(followerOfFollowingUsers.ToString(), "", queryInfo))
                                        continue;
                                    if (followerOfFollowingUsers.Success)
                                    {
                                        #region Process for "FollowersOfFollowings" query parameter
                                        var usersList = FilterWhitelistBlacklistUsers(followerOfFollowingUsers.UsersList);

                                        GetInteractedUserAccrossAllFor(usersList ?? followerOfFollowingUsers.UsersList);
                                        GetInteractedCampaignUser(usersList ?? followerOfFollowingUsers.UsersList);
                                        CheckUserInDatabase(usersList ?? followerOfFollowingUsers.UsersList);
                                        if (ActivityType == ActivityType.Follow)
                                        {
                                            var userList = usersList ?? followerOfFollowingUsers.UsersList;
                                            int count = 0;
                                            for (int i = 0; i < userList.Count; i++)
                                            {
                                                Token.ThrowIfCancellationRequested();
                                                count++;
                                                instaUserList.Add(userList[i]);

                                                if (count == 30 || userList.Count - 1 == i)
                                                {
                                                    count = 0;
                                                    instaUserList = GetUserInfoDetails(instaUserList);
                                                    jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult, instaUserList);
                                                    instaUserList = new List<InstagramUser>();
                                                }
                                            }
                                        }
                                        else
                                            jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult, usersList ?? followerOfFollowingUsers.UsersList);

                                        #endregion
                                        jobProcessResult.maxId = followerOfFollowingUsers.MaxId;
                                        if (string.IsNullOrEmpty(jobProcessResult.maxId))
                                            jobProcessResult.HasNoResult = true;
                                    }
                                    else
                                        jobProcessResult.maxId = null;
                                }

                                #endregion
                            }

                            result.maxId = followingUsers.MaxId;
                        }
                        else
                        {
                            maxId = null;
                            result.maxId = maxId;
                        }

                        result.IsProcessCompleted = jobProcessResult.IsProcessCompleted;
                        CheckNoMoreDataForWithQuery(ref result);
                        hasnoResult = result.HasNoResult;
                    }
                    #endregion
                }
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
            catch (Exception)
            {
                // ex.DebugLog();
            }
        }
    }
}
