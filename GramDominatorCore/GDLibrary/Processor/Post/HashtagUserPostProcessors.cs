using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using DominatorHouseCore;
using GramDominatorCore.GDModel;
using DominatorHouseCore.Utility;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using System.Threading;
using GramDominatorCore.Response;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDFactories;
using ThreadUtils;

namespace GramDominatorCore.GDLibrary.Processor.Post
{
    public class HashtagUserPostProcessors : BaseInstagramPostProcessor
    {
        private List<InteractedPosts> LstInteractedPosts { get; set; } = new List<InteractedPosts>();
        public HashtagUserPostProcessors(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService _delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, _delayService, gdBrowserManager)
        { }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                List<InstagramPost> lstInstagramPost = new List<InstagramPost>();
                QueryType = queryInfo.QueryType;
                int nextPageCount = 0;
                string topMaxId = null; string topNextMediaId = null;
                string recentMaxid = null; string recentNextMediaId = null;

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    HashTagFeedIgResponseHandler feedDetails = GetResponseFromHashTagPost(queryInfo, jobProcessResult, queryInfo.QueryValue, nextPageCount, ref lstInstagramPost, ref topMaxId, ref topNextMediaId, ref recentMaxid, ref recentNextMediaId);
                    if (!CheckingLoginRequiredResponse(feedDetails.ToString(), "", queryInfo))
                        return;
                    if (feedDetails.Success)
                    {
                        var allFeedDetails = lstInstagramPost;
                        if (allFeedDetails.Count == 0)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram,
                                DominatorAccountModel.UserName, ActivityType.Like,
                                $"Hashtag [{queryInfo.QueryValue}] not exist or no posts available");
                            return;
                        }
                        List<InstagramPost> filteredFeeds = FilterWhitelistBlacklistUsersFromFeeds(allFeedDetails);
                        CheckInteractedPostsData(LstInteractedPosts, filteredFeeds);
                        // filteredFeeds = FilterPostAge(filteredFeeds);
                        filteredFeeds=FilterAllImages(filteredFeeds);
                        if (ModuleSetting.PostFilterModel.FilterPostAge && !ModuleSetting.PostFilterModel.FilterBeforePostAge && filteredFeeds.Count == 0)
                            break;
                        foreach (var eachPost in filteredFeeds)
                        {
                            Token.ThrowIfCancellationRequested();
                            jobProcessResult = StartProcessWithUsersFeeds(queryInfo, new List<InstagramUser>() { eachPost.User }, eachPost.Code);
                        }
                        if (!string.IsNullOrEmpty(feedDetails.MaxId))
                        {
                            jobProcessResult.maxId = feedDetails.MaxId;
                            nextPageCount++;
                        }
                    }
                    else
                        jobProcessResult.maxId = null;


                    CheckNoMoreDataForWithQuery(ref jobProcessResult);
                    DelayForScraperActivity();
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
