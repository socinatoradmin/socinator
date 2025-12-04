using ThreadUtils;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.Response;
using System;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.Post
{
    public class SomeonesFollowingsPostProcessor : BaseInstagramPostProcessor
    {
        public SomeonesFollowingsPostProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService _delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, _delayService, gdBrowserManager)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                QueryType = queryInfo.QueryType;
                var browser = GramStatic.IsBrowser;
                if (queryInfo.QueryValue.ToLower() == "[own]")
                {
                    StartProcessForOwnFollowings(queryInfo);
                    return;
                }
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

                        var followingUsers = 
                            browser ?
                            GdBrowserManager.GetUserFollowings(DominatorAccountModel, userInfo.Username, Token)
                            : InstaFunction.GetUserFollowings(DominatorAccountModel,AccountModel,userInfo.Pk,Token,string.Empty, jobProcessResult.maxId,userInfo.Username, IsWeb: true).Result;
                        if (!CheckingLoginRequiredResponse(followingUsers.ToString(), "", queryInfo))
                            return;
                        if (followingUsers.Success && followingUsers.UsersList.Count > 0)
                        {                         
                            jobProcessResult = StartProcessWithUsersFeeds(queryInfo, followingUsers.UsersList);
                            jobProcessResult.maxId = followingUsers.MaxId;
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
              //  ex.DebugLog();
            }
        }
    }
}
