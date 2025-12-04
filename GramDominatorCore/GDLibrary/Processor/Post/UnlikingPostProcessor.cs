using ThreadUtils;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.Post
{
    public class UnlikingPostProcessor : BaseInstagramPostProcessor
    {
        public UnlikingPostProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService _delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, _delayService, gdBrowserManager)
        {
        }
        private List<InteractedPosts> LstInteractedPosts { get; set; } = new List<InteractedPosts>();
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            Token.ThrowIfCancellationRequested();
            try
            {
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    Token.ThrowIfCancellationRequested();
                    var likedMediaResopnse = !GramStatic.IsBrowser ?
                        InstaFunction.GdBrowserManager.GetLikedMedia(DominatorAccountModel, Token)
                        : InstaFunction.GetLikedMedia(DominatorAccountModel, jobProcessResult.maxId).Result;
                    if (!CheckingLoginRequiredResponse(likedMediaResopnse.ToString(), "", queryInfo))
                        return;
                    if (likedMediaResopnse.Success)
                    {
                        List<InstagramPost> filteredFeeds = FilterWhitelistBlacklistUsersFromFeeds(likedMediaResopnse.Items);
                        filteredFeeds.Shuffle();
                        LstInteractedPosts = DbAccountService.GetInteractedPosts(DominatorAccountModel.UserName, ActivityType).ToList();
                        filteredFeeds.RemoveAll(x => LstInteractedPosts.Any(y => y.PkOwner == x.Code));
                        foreach (var instaPost in filteredFeeds)
                        {
                            FilterAndStartFinalProcessForOnePost(QueryInfo.NoQuery, ref jobProcessResult, instaPost);
                            Token.ThrowIfCancellationRequested();
                        }
                        jobProcessResult.maxId = likedMediaResopnse.MaxId;
                        if (string.IsNullOrEmpty(likedMediaResopnse.MaxId))
                            jobProcessResult.HasNoResult = true;
                    }
                    else
                        jobProcessResult.maxId = null;
                    jobProcessResult.IsProcessCompleted = true;
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
                // ex.DebugLog();
            }
        }
    }
}
