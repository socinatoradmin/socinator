using ThreadUtils;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.GDModel;
using System;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.Post
{
    public class SuggestedUsersPostsProcessor : BaseInstagramPostProcessor
    {
        public SuggestedUsersPostsProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService _delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, _delayService, gdBrowserManager)
        {
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                QueryType = queryInfo.QueryType;
                var suggestedUsers = 
                    GramStatic.IsBrowser ?
                    InstaFunction.GdBrowserManager.GetSuggestedUsers(DominatorAccountModel, Token)
                    : InstaFunction.GetSuggestedUsers(DominatorAccountModel, AccountModel, string.Empty, Token, jobProcessResult.maxId).Result;
                if (!CheckingLoginRequiredResponse(suggestedUsers.ToString(), "", queryInfo))
                    return;
                if (suggestedUsers.Success)
                    StartProcessWithUsersFeeds(queryInfo, suggestedUsers.UsersList);
                Token.ThrowIfCancellationRequested();
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
                //  ex.DebugLog();
            }
        }
    }
}
