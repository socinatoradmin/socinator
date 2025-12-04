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

namespace GramDominatorCore.GDLibrary.Processor.Post
{
    public class LocationUserPostProcessor : BaseInstagramPostProcessor
    {
        public LocationUserPostProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel,  IDelayService _delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, _delayService,gdBrowserManager)
        {
        }
        private List<InteractedPosts> LstInteractedPosts { get; set; } = new List<InteractedPosts>();
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                List<InstagramPost> allFeedDetails = new List<InstagramPost>();
                FeedIgResponseHandlerAlternate locationFeedDetails = null;
                QueryType = queryInfo.QueryType;
                string nextPage = string.Empty;

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    Token.ThrowIfCancellationRequested();
                    locationFeedDetails = InstaFunction.GetLocationFeedAlternate(DominatorAccountModel, AccountModel, queryInfo.QueryValue, Token, jobProcessResult.maxId, nextPage).Result;
                    if (!string.IsNullOrEmpty(locationFeedDetails.NextPage))
                        nextPage = locationFeedDetails.NextPage;
                    if (!CheckingLoginRequiredResponse(locationFeedDetails.ToString(), "", queryInfo))
                        return;
                    if (locationFeedDetails.Success)
                    {
                        if (locationFeedDetails.Sections.Count != 0)
                            allFeedDetails = locationFeedDetails.Sections;

                        CheckInteractedPostsData(LstInteractedPosts, allFeedDetails);
                        List<InstagramPost> filteredFeeds = FilterWhitelistBlacklistUsersFromFeeds(allFeedDetails);
                        filteredFeeds.Shuffle();
                        filteredFeeds = FilterAllImages(filteredFeeds);
                        if (ModuleSetting.PostFilterModel.FilterPostAge && !ModuleSetting.PostFilterModel.FilterBeforePostAge && filteredFeeds.Count == 0)
                            break;
                        foreach (var eachPost in filteredFeeds)
                        {
                            Token.ThrowIfCancellationRequested();
                            StartProcessWithUsersFeeds(queryInfo, new List<InstagramUser>() { eachPost.User });
                        }
                        if (!string.IsNullOrEmpty(locationFeedDetails.MaxId))
                            jobProcessResult.maxId = locationFeedDetails.MaxId;
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
