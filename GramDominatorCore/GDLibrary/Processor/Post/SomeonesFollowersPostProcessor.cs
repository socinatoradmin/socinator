using System;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.Response;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDFactories;
using ThreadUtils;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.Post
{
    public class SomeonesFollowersPostProcessor : BaseInstagramPostProcessor
    {
        public SomeonesFollowersPostProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService delayService, IGdBrowserManager gdBrowserManager) : 
            base(jobProcess, dbAccountService, campaignService, processScopeModel, delayService, gdBrowserManager)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                QueryType = queryInfo.QueryType;
                if (queryInfo.QueryValue.ToLower() == "[own]")
                {
                    StartProcessForOwnFollowers(queryInfo);
                    return;
                }
                var browser = GramStatic.IsBrowser;
                var userInfo = 
                    browser ?
                    GdBrowserManager.GetUserInfo(DominatorAccountModel, queryInfo.QueryValue, Token)
                    :InstaFunction.SearchUsername(DominatorAccountModel,queryInfo.QueryValue, Token);
                if (!CheckingLoginRequiredResponse(userInfo.ToString(), "", queryInfo))
                    return;
                if (userInfo.Success)
                {
                    if (userInfo.IsPrivate && (userInfo?.instaUserDetails != null && !userInfo.instaUserDetails.IsFollowing))
                        return;

                    while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                    {
                        Token.ThrowIfCancellationRequested();

                        FollowerAndFollowingIgResponseHandler followerUsers = 
                            browser ?
                            GdBrowserManager.GetUserFollowers(DominatorAccountModel, userInfo.Username, Token)
                            :InstaFunction.GetUserFollowers(DominatorAccountModel,userInfo.Pk,Token, jobProcessResult.maxId,userInfo.Username, IsWeb: true).Result;
                        if (!CheckingLoginRequiredResponse(followerUsers.ToString(), "", queryInfo))
                            return;
                        if (followerUsers.Success && followerUsers.UsersList.Count > 0)
                        {                                                    
                            jobProcessResult = StartProcessWithUsersFeeds(queryInfo, followerUsers.UsersList);
                            jobProcessResult.maxId = followerUsers.MaxId;
                            Token.ThrowIfCancellationRequested();
                        }
                        else
                            jobProcessResult.maxId = null;
                        
                        CheckNoMoreDataForWithQuery(ref jobProcessResult);
                    }
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
            catch (Exception )
            {
             //   ex.DebugLog();
            }
        }
    }
}
