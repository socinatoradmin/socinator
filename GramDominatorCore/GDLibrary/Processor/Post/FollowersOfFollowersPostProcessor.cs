using ThreadUtils;
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
    public class FollowersOfFollowersPostProcessor : BaseInstagramPostProcessor
    {
        public FollowersOfFollowersPostProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, delayService, gdBrowserManager)
        {
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                QueryType = queryInfo.QueryType;

                JobProcessResult result = jobProcessResult;
                string maxId = null;
                bool hasnoResult = false;
                var browser = GramStatic.IsBrowser;
                UsernameInfoIgResponseHandler userInfo = 
                    browser ?
                    GdBrowserManager.GetUserInfo(DominatorAccountModel, queryInfo.QueryValue, Token)
                    :InstaFunction.SearchUsername(DominatorAccountModel, queryInfo.QueryValue, Token);

                if (!CheckingLoginRequiredResponse(userInfo.ToString(), "", queryInfo))
                    return;
                if (userInfo.Success)
                {
                    if (userInfo.IsPrivate && (userInfo?.instaUserDetails != null && !userInfo.instaUserDetails.IsFollowing))
                        return;

                    // Outer loop operation for getting followers
                    #region Getting Followers operation
                    while (!jobProcessResult.IsProcessCompleted && !hasnoResult)
                    {
                        Token.ThrowIfCancellationRequested();

                        // Get Follower users from User provided username
                        FollowerAndFollowingIgResponseHandler followerUsers =
                            browser ?
                            GdBrowserManager.GetUserFollowers(DominatorAccountModel, userInfo.Username, Token)
                               : InstaFunction.GetUserFollowers(DominatorAccountModel, userInfo.Pk, Token, maxId, userInfo.Username, IsWeb: true).Result;
                        if (!CheckingLoginRequiredResponse(followerUsers.ToString(), "", queryInfo))
                            break;
                        if (followerUsers.Success)
                        {
                            foreach (InstagramUser instaUser in followerUsers.UsersList)
                            {
                                Token.ThrowIfCancellationRequested();

                                // Inner loop operation mentioning Followers of Followers
                                #region Inner loop mentioning Followers of Followers Opertaion

                                if (instaUser.IsPrivate)// && SkipNotFollowedPrivateUser(instaUser.Pk, showMessage: false)
                                    continue;

                                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                                {
                                    Token.ThrowIfCancellationRequested();
                                    FollowerAndFollowingIgResponseHandler followerOfFollowersUsers = null;
                                    // Get Followers of Followers Users from intermediate users
                                    if (browser)
                                        followerOfFollowersUsers =
                                       GdBrowserManager.GetUserFollowers(DominatorAccountModel, instaUser.Username, Token);
                                    else
                                        followerOfFollowersUsers =
                                       InstaFunction.GetUserFollowers(DominatorAccountModel, instaUser.Pk, Token, jobProcessResult.maxId, instaUser.Username, IsWeb: true).Result;

                                    if (followerOfFollowersUsers.Success && followerOfFollowersUsers.UsersList.Count > 0)
                                    {
                                        jobProcessResult = StartProcessWithUsersFeeds(queryInfo, followerOfFollowersUsers.UsersList);
                                        jobProcessResult.maxId = followerOfFollowersUsers.MaxId;
                                    }
                                    else
                                        jobProcessResult.maxId = null;
                                    
                                    CheckNoMoreDataForWithQuery(ref jobProcessResult);
                                }
                            }
                            #endregion
                        }
                        result.maxId = followerUsers.MaxId;
                        if (string.IsNullOrEmpty(result.maxId))
                            hasnoResult = true;
                    }
                }
                else
                {
                    maxId = null;
                    result.maxId = maxId;
                    result.IsProcessCompleted = jobProcessResult.IsProcessCompleted;
                    CheckNoMoreDataForWithQuery(ref result);
                    hasnoResult = jobProcessResult.HasNoResult;
                }

                #endregion
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
