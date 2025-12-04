using ThreadUtils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Response;
using System;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.Post
{
    public class FollowersOfFollowingsPostProcessor : BaseInstagramPostProcessor
    {
        public FollowersOfFollowingsPostProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel,  IDelayService _delayService, IGdBrowserManager gdBrowserManager)
            : base(jobProcess, dbAccountService, campaignService, processScopeModel, _delayService, gdBrowserManager)
        {
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();

                if (string.IsNullOrEmpty(queryInfo.QueryValue))
                    CheckForNoMoreDataAndStopProcess(jobProcessResult);
                
                QueryType = queryInfo.QueryType;

                JobProcessResult result = new JobProcessResult();
                string maxId = null;
                bool hasnoResult = false;
                var browser = GramStatic.IsBrowser;
                UsernameInfoIgResponseHandler userInfo = 
                    browser ?
                    GdBrowserManager.GetUserInfo(DominatorAccountModel, queryInfo.QueryValue, Token)
                    :InstaFunction.SearchUsername(DominatorAccountModel, queryInfo.QueryValue, Token);

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
                        FollowerAndFollowingIgResponseHandler followingUsers =
                            browser ?
                            GdBrowserManager.GetUserFollowings(DominatorAccountModel, userInfo.Username, Token)
                            : InstaFunction.GetUserFollowings(DominatorAccountModel, AccountModel, userInfo.Pk, Token,string.Empty, maxId, userInfo.Username, IsWeb: true).Result;
                        if (!CheckingLoginRequiredResponse(followingUsers.ToString(), "", queryInfo))
                            return;
                        if (followingUsers.Success)
                        {
                            foreach (InstagramUser instagramUser in followingUsers.UsersList)
                            {
                              
                                Token.ThrowIfCancellationRequested();
                                if (instagramUser.IsPrivate && (ActivityType == ActivityType.Like||ActivityType==ActivityType.Comment||ActivityType==ActivityType.Reposter))
                                {
                                    continue;
                                }
                                // Inner loop mentioning Followers of Followings operation
                                #region Inner loop mentioning Followers of Followers Opertaion
                                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                                {
                                    Token.ThrowIfCancellationRequested();
                                    FollowerAndFollowingIgResponseHandler followerOfFollowingUsers = null;
                                    // Get Followers of Followers Users from intermediate users
                                    if (browser)
                                        followerOfFollowingUsers =
                                       InstaFunction.GetUserFollowersBrowser(DominatorAccountModel, instagramUser.Username, Token, QueryType, jobProcessResult.maxId);
                                    else
                                        followerOfFollowingUsers = InstaFunction.GetUserFollowers(DominatorAccountModel, instagramUser.Pk, Token, jobProcessResult.maxId,instagramUser.Username, IsWeb: true).Result;
                                    Token.ThrowIfCancellationRequested();

                                    if (followerOfFollowingUsers.Success)
                                    {
                                        jobProcessResult = StartProcessWithUsersFeeds(queryInfo, followerOfFollowingUsers.UsersList);
                                        jobProcessResult.maxId = followerOfFollowingUsers.MaxId;
                                    }
                                    else
                                        jobProcessResult.maxId = null;                                    
                                }
                                #endregion
                            }

                            result.maxId = followingUsers.MaxId;
                            if (string.IsNullOrEmpty(result.maxId))
                                hasnoResult = true;
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
                throw new OperationCanceledException();
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
