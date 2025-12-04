using System;
using System.Collections.Generic;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Response;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDFactories;
using ThreadUtils;

namespace GramDominatorCore.GDLibrary.Processor.Post
{
    public class HashTagPostProcessor : BaseInstagramPostProcessor
    {
        private List<InteractedPosts> LstInteractedPosts { get; set; } = new List<InteractedPosts>();
        public HashTagPostProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, delayService, gdBrowserManager)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                List<InstagramPost> lstInstagramPost = new List<InstagramPost>();
                int nextPageCount = 0;
                QueryType = queryInfo.QueryType;
                string topMaxId = null; string topNextMediaId = null;
                string recentMaxid = null; string recentNextMediaId = null;
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    var feedDetails = GetResponseFromHashTagPost(queryInfo, jobProcessResult, queryInfo.QueryValue, nextPageCount, ref lstInstagramPost, ref topMaxId, ref topNextMediaId, ref recentMaxid, ref recentNextMediaId);
                    if (!CheckingLoginRequiredResponse(feedDetails.ToString(), "", queryInfo) && !DominatorAccountModel.IsRunProcessThroughBrowser)
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
                        //filteredFeeds = FilterPostAge(filteredFeeds);

                        filteredFeeds = FilterAllImages(filteredFeeds);
                        if (ModuleSetting.PostFilterModel.FilterPostAge && !ModuleSetting.PostFilterModel.FilterBeforePostAge && filteredFeeds.Count == 0)
                            break;
                        foreach (var eachPost in filteredFeeds)
                        {
                            Token.ThrowIfCancellationRequested();
                            if (ActivityType != ActivityType.CommentScraper && ActivityType != ActivityType.LikeComment && ActivityType!=ActivityType.ReplyToComment)
                                FilterAndStartFinalProcessForOnePost(queryInfo, ref jobProcessResult, eachPost);
                            else
                                FilterAndStartFinalProcessForOneComment(queryInfo, ref jobProcessResult, eachPost);
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
