using ThreadUtils;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using System;
using System.Collections.Generic;

namespace GramDominatorCore.GDLibrary.Processor.Post
{
    public class SpecificUsersPostsProcessor : BaseInstagramPostProcessor
    {
        public SpecificUsersPostsProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, delayService, gdBrowserManager)
        {
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                //UsernameInfoIgResponseHandler infuser = null;
                //if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                //    infuser = InstaFunction.SearchUsername(DominatorAccountModel, queryInfo.QueryValue, Token);
                //else
                //    infuser = InstaFunction.GdBrowserManager.GetUserInfo(DominatorAccountModel, queryInfo.QueryValue, Token);
                var infuser = InstaFunction.SearchUsername(DominatorAccountModel, queryInfo.QueryValue, Token);
                if (!CheckingLoginRequiredResponse(infuser.ToString(), "", queryInfo))
                    return;
                if (infuser.Success)
                    jobProcessResult = StartProcessWithUsersFeeds(queryInfo, new List<InstagramUser>() { infuser });
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
                //ignore
            }
            finally
            {
                jobProcessResult.IsProcessCompleted = true;
            }
        }
    }
}
