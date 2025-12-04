using ThreadUtils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Response;
using System;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.Post
{
    public class PostOfUsersWhoLikedPostProcessor : BaseInstagramPostProcessor
    {
        public PostOfUsersWhoLikedPostProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService _delayService, IGdBrowserManager gdBrowserManager)
            : base(jobProcess, dbAccountService, campaignService, processScopeModel, _delayService, gdBrowserManager)
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

                    var mediaLikers = 
                        GramStatic.IsBrowser ?
                        InstaFunction.GdBrowserManager.GetMediaLikers(DominatorAccountModel, queryInfo.QueryValue, Token)
                        : InstaFunction.GetMediaLikers(DominatorAccountModel, idFromCode, Token, jobProcessResult.maxId);
                    if (!CheckingLoginRequiredResponse(mediaLikers.ToString(), "", queryInfo))
                        return;
                    if (mediaLikers.Success)
                    {
                        jobProcessResult = StartProcessWithUsersFeeds(queryInfo, mediaLikers.UserList);
                        jobProcessResult.maxId = mediaLikers.MaxId;
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
                //ex.DebugLog();
            }
        }
    }
}
