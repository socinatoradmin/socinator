using ThreadUtils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDModel;
using GramDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using GramDominatorCore.GDLibrary.InstagramBrowser;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class HashTagUsersProcessors : BaseInstagramUserProcessor
    {
        public BroadcastMessagesModel BroadcastMessagesModel { get; set; }
        private List<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers> LstCampaignIntractedUsersForUserScraper { get; set; } = new List<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers>();
        public HashTagUsersProcessors(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService _delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, _delayService, gdBrowserManager)
        {
            BroadcastMessagesModel = JsonConvert.DeserializeObject<BroadcastMessagesModel>(TemplateModel.ActivitySettings);
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                if (CheckQueryValueOnMessageList(BroadcastMessagesModel, queryInfo)) return;
                List<InstagramPost> lstInstagramPost = new List<InstagramPost>();
                QueryType = queryInfo.QueryType;
                int nextPageCount = 0;
                string topMaxId = null; string topNextMediaId = null;
                string recentMaxid = null; string recentNextMediaId = null;
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    HashTagFeedIgResponseHandler feedDetails = GetResponseFromHashTagPost(queryInfo, jobProcessResult, queryInfo.QueryValue, nextPageCount, ref lstInstagramPost, ref topMaxId, ref topNextMediaId, ref recentMaxid, ref recentNextMediaId);
                    if (feedDetails.RankedItems.Count == 0)
                        return;
                    if (!CheckingLoginRequiredResponse(feedDetails.ToString(), feedDetails.message ?? "", queryInfo))
                        return;
                    if (feedDetails.Success)
                    {
                        var allFeedDetails = lstInstagramPost;

                        if (ModuleSetting.IsScrpeUniqueUserForThisCampaign && ActivityType == ActivityType.UserScraper)
                        {
                            LstCampaignIntractedUsersForUserScraper = CampaignService.GetAllInteractedUsers().ToList();
                            allFeedDetails.RemoveAll(x =>
                                LstCampaignIntractedUsersForUserScraper.Any(y => y.InteractedUsername == x.User.Username));
                        }
                        else
                        {
                            LstInteractedUsers = DbAccountService.GetInteractedUsers(ActivityType).Where(x => x.Username == DominatorAccountModel.UserName).ToList();
                            allFeedDetails.RemoveAll(x => LstInteractedUsers.Any(y => y.InteractedUsername == x.User.Username));
                        }

                        List<InstagramPost> filteredFeeds = FilterWhitelistBlacklistUsersFromFeeds(allFeedDetails);
                        if (ModuleSetting.IsSkipUserWhoReceivedMessage)
                        {
                            LstInteractedUsers = DbAccountService.GetInteractedUsersMessageData().ToList();
                            filteredFeeds.RemoveAll(x => LstInteractedUsers.Any(y => y.InteractedUsername == x.User.Username));
                        }
                        if (ActivityType == ActivityType.Follow)
                        {
                            GetPostOfUserInfoDetails(filteredFeeds);
                            FilterAllPostUserResults(filteredFeeds, queryInfo);
                            FilterOnlyBusinessAccounts(filteredFeeds);
                        }       
                        foreach (var eachPost in filteredFeeds)
                        {
                            Token.ThrowIfCancellationRequested();

                            if (feedDetails.RankedItems != null && ActivityType == ActivityType.UserScraper && ModuleSetting.IsTaggedPostUser)
                            {
                                foreach (var taggedUser in eachPost.UserTags)
                                    FilterAndStartFinalProcessForOneUser(queryInfo, ref jobProcessResult, taggedUser);
                            }
                            else
                                FilterAndStartFinalProcessForOneUser(queryInfo, ref jobProcessResult, eachPost.User, eachPost);
                        }
                        if (!string.IsNullOrEmpty(feedDetails.MaxId))
                        {
                            Token.ThrowIfCancellationRequested();
                            jobProcessResult.maxId = feedDetails.MaxId;
                            //recentMaxid = feedDetails.MaxId;
                            nextPageCount++;
                        }
                        else
                        {
                            jobProcessResult.maxId = feedDetails.MaxId;
                            CheckNoMoreDataForWithQuery(ref jobProcessResult);
                        }
                    }
                    else
                        jobProcessResult.maxId = null;
                    CheckNoMoreDataForWithQuery(ref jobProcessResult);
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
