using ThreadUtils;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using System;
using System.Collections.Generic;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.Post
{
    public class ScrapAllLikedPostProcessor : BaseInstagramPostProcessor
    {
        public ScrapAllLikedPostProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService _delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, _delayService, gdBrowserManager)
        {
        }
        private List<InteractedPosts> LstInteractedPosts { get; set; } = new List<InteractedPosts>();
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                QueryType = queryInfo.QueryType;

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    var likedMediaResopnse = /*GramStatic.IsBrowser*/ true ?
                        InstaFunction.GdBrowserManager.GetLikedMedia(DominatorAccountModel, Token)
                        : InstaFunction.GetLikedMedia(DominatorAccountModel, jobProcessResult.maxId).Result;
                    if (!CheckingLoginRequiredResponse(likedMediaResopnse.ToString(), "", queryInfo))
                        return;
                    if (likedMediaResopnse.Success)
                    {
                        List<InstagramPost> filteredFeeds = FilterWhitelistBlacklistUsersFromFeeds(likedMediaResopnse.Items);
                        CheckInteractedPostsData(LstInteractedPosts, filteredFeeds);
                        filteredFeeds = FilterAllImagesApply(filteredFeeds);
                        if (filteredFeeds.Count == 0)
                            break;
                        foreach (var eachPost in filteredFeeds)
                        {
                            Token.ThrowIfCancellationRequested();
                            if (ActivityType == ActivityType.LikeComment || ActivityType == ActivityType.CommentScraper || ActivityType == ActivityType.ReplyToComment)
                                FilterAndStartFinalProcessForOneComment(queryInfo, ref jobProcessResult, eachPost);
                            else
                                FilterAndStartFinalProcessForOnePost(queryInfo, ref jobProcessResult, eachPost);
                        }
                        jobProcessResult.maxId = likedMediaResopnse.MaxId;
                    }
                    else
                        jobProcessResult.maxId = null;

                    CheckNoMoreDataForWithQuery(ref jobProcessResult);
                    DelayForScraperActivity();
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
                //ex.DebugLog();
            }
        }
    }
}
