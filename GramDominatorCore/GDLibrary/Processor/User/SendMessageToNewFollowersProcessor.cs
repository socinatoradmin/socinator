using ThreadUtils;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using System;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class SendMessageToNewFollowersProcessor : BaseInstagramUserProcessor
    {
        public SendMessageToNewFollowersProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, delayService, gdBrowserManager)
        {
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();

                GramDominatorCore.Response.FollowerAndFollowingIgResponseHandler newFollowersResponse = null;
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                    newFollowersResponse = InstaFunction.GetRecentFollowers(DominatorAccountModel);
                else
                {
                    newFollowersResponse = InstaFunction.GdBrowserManager.GetUserFollowers(DominatorAccountModel, DominatorAccountModel.UserName, Token);
                    
                }
                if (!CheckingLoginRequiredResponse(newFollowersResponse.ToString(), "", queryInfo))
                    return;
                if (newFollowersResponse.Success)
                {
                    var usersList = FilterWhitelistBlacklistUsers(newFollowersResponse.UsersList);
                    FilterAndStartFinalProcess(QueryInfo.NoQuery, jobProcessResult, usersList ?? newFollowersResponse.UsersList);
                    Token.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception)
            {
                // ex.DebugLog();
            }
        }
    }
}
