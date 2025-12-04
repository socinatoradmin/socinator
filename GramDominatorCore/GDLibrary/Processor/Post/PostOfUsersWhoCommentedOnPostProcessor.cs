using ThreadUtils;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using System;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.Post
{
    public class PostOfUsersWhoCommentedOnPostProcessor : BaseInstagramPostProcessor
    {
        public PostOfUsersWhoCommentedOnPostProcessor(IGdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService,
            IProcessScopeModel processScopeModel, IDelayService delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, delayService, gdBrowserManager)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                QueryType = queryInfo.QueryType;
                string idFromCode = CheckPostId(queryInfo);

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    Token.ThrowIfCancellationRequested();
                    GramDominatorCore.Response.MediaCommentsIgResponseHandler mediaComments = null;
                    mediaComments = 
                        GramStatic.IsBrowser ?
                        InstaFunction.GdBrowserManager.GetMediaComments(DominatorAccountModel, queryInfo.QueryValue, Token)
                        : InstaFunction.GetMediaComments(DominatorAccountModel, DominatorAccountModel.IsRunProcessThroughBrowser ? queryInfo.QueryValue.Trim() : idFromCode, Token, jobProcessResult.maxId);
                    if (!CheckingLoginRequiredResponse(mediaComments.ToString(), "", queryInfo))
                        return;
                    if (mediaComments.Success)
                    {
                        jobProcessResult = StartProcessWithUsersFeeds(queryInfo, mediaComments.UserList);
                        jobProcessResult.maxId = mediaComments.MaxId;
                    }
                    else
                        jobProcessResult.maxId = null;

                    CheckNoMoreDataForWithQuery(ref jobProcessResult);
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
