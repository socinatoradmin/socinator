using ThreadUtils;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using GramDominatorCore.GDLibrary.InstagramBrowser;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class LocationUsersProcessor : BaseInstagramUserProcessor
    {
        public BroadcastMessagesModel BroadcastMessagesModel { get; set; }
        
           
        private List<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers> LstCampaignIntractedUsersForUserScraper { get; set; } = new List<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers>();
        public LocationUsersProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService _delayService, IGdBrowserManager gdBrowserManager) :
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
                List<InstagramPost> allFeedDetails = new List<InstagramPost>();
                QueryType = queryInfo.QueryType;
                string nextPage = string.Empty;

                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    Token.ThrowIfCancellationRequested();
                    var locationFeedDetailss = InstaFunction.GetLocationFeedAlternate(DominatorAccountModel, AccountModel, queryInfo.QueryValue, Token, jobProcessResult.maxId, nextPage).Result;
                    if (!string.IsNullOrEmpty(locationFeedDetailss.NextPage))
                        nextPage = locationFeedDetailss.NextPage;

                    if (!CheckingLoginRequiredResponse(locationFeedDetailss.ToString(), "", queryInfo))
                        return;
                    if (locationFeedDetailss.Success)//locationFeedDetails.Success ||
                    {
                        if (locationFeedDetailss != null && locationFeedDetailss.Sections.Count != 0)//&& locationFeedDetails.Items.Count == 0
                            allFeedDetails = locationFeedDetailss.Sections;

                        List<InstagramPost> filteredFeeds = FilterWhitelistBlacklistUsersFromFeeds(allFeedDetails);
                        filteredFeeds.Shuffle();
                        if (ModuleSetting.IsSkipUserWhoReceivedMessage)
                        {
                            LstInteractedUsers = DbAccountService.Get<InteractedUsers>(x => x.DirectMessage != null).ToList();
                            filteredFeeds.RemoveAll(x => LstInteractedUsers.Any(y => y.InteractedUsername == x.User.Username));
                        }
                        if (ActivityType == ActivityType.UserScraper && ModuleSetting.IsScrpeUniqueUserForThisCampaign)
                        {
                            LstCampaignIntractedUsersForUserScraper = CampaignService.GetAllInteractedUsers().ToList();
                            allFeedDetails.RemoveAll(x => LstCampaignIntractedUsersForUserScraper.Any(y => y.InteractedUsername == x.User.Username));
                        }
                        if (ActivityType==ActivityType.Follow)
                        {
                            GetPostOfUserInfoDetails(filteredFeeds);
                            FilterAllPostUserResults(filteredFeeds, queryInfo);
                            FilterOnlyBusinessAccounts(filteredFeeds);
                        }
                        foreach (var eachPost in filteredFeeds)
                        {
                            Token.ThrowIfCancellationRequested();
                            if (ActivityType == ActivityType.UserScraper && ModuleSetting.IsTaggedPostUser)
                            {
                                foreach (var taggedUser in eachPost.UserTags)
                                    FilterAndStartFinalProcessForOneUser(queryInfo, ref jobProcessResult, taggedUser);
                            }
                            else
                                FilterAndStartFinalProcessForOneUser(queryInfo, ref jobProcessResult, eachPost.User, eachPost);
                        }
                        if (locationFeedDetailss != null)
                        {
                            jobProcessResult.maxId = locationFeedDetailss.MaxId;
                            if (!string.IsNullOrEmpty(jobProcessResult.maxId))
                                jobProcessResult.maxId = locationFeedDetailss.MaxId;
                            else
                                CheckNoMoreDataForWithQuery(ref jobProcessResult);
                            Token.ThrowIfCancellationRequested();
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
