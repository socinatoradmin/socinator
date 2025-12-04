using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using GramDominatorCore.Factories;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using Newtonsoft.Json;
using System;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary
{
    public class BlockFollowerProcess : GdJobProcessInteracted<InteractedUsers>
    {
        private int _actionBlockedCount;
        public BlockFollowerModel BlockFollowerModel { get; set; }
        private BlackListWhitelistHandler BlackListWhitelistHandler { get; set; }
        public BlockFollowerProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IGdQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, IGdBrowserManager gdBrowser, IDelayService _delayService) :
            base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser, _delayService)
        {
            BlockFollowerModel = JsonConvert.DeserializeObject<BlockFollowerModel>(templateModel.ActivitySettings);
            BlackListWhitelistHandler = new BlackListWhitelistHandler(ModuleSetting, DominatorAccountModel, ActivityType);
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, GramStatic.GetUrl(scrapeResult.ResultUser.Username));
           JobProcessResult jobProcessResult = new JobProcessResult();
            try
            {
                InstagramUser instagramUser = (InstagramUser)scrapeResult.ResultUser;
                var response =
                    GramStatic.IsBrowser ?
                    instaFunct.GdBrowserManager.Block(DominatorAccountModel, JobCancellationTokenSource.Token, instagramUser)
                    : instaFunct.Block(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instagramUser.UserId ?? instagramUser.Pk).Result;
                if (response!=null && response.Success && response.Blocking)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, GramStatic.GetUrl(scrapeResult.ResultUser.Username));
                    IncrementCounters();
                    AddBlockUserDataToDataBase(scrapeResult, response);

                    // Add to Blacklist
                    if (BlockFollowerModel.ManageBlackWhiteListModel.IsAddToBlackListOnceUnfollowed)
                    {
                       // BlackListWhitelistHandler = new BlackListWhitelistHandler(ModuleSetting,DominatorAccountModel,ActivityType.BlockFollower);
                        BlackListWhitelistHandler.AddToBlackList(instagramUser.UserId, instagramUser.Username);
                    }
                    jobProcessResult.IsProcessSuceessfull = true;
                }                
                else
                {
                    if (!CheckResponse.CheckProcessResponse(response, DominatorAccountModel, ActivityType, scrapeResult,ref _actionBlockedCount))
                    {
                        Stop();
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    }
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                // Delay between each activity
                DelayBeforeNextActivity();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private void AddBlockUserDataToDataBase(ScrapeResultNew scrapeResult, FriendshipsResponse response)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            try
            {
                // Add data to respected campaign InteractedUsers table
                if (!string.IsNullOrEmpty(CampaignId))
                {
                    CampaignDbOperation?.Add(
                            new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers()
                            {
                                ActivityType = ActivityType.ToString(),
                                Date = DateTimeUtilities.GetEpochTime(),
                                Username = DominatorAccountModel.AccountBaseModel.UserName,
                                InteractedUsername = scrapeResult.ResultUser.Username,
                                InteractedUserId = ((InstagramUser) scrapeResult.ResultUser).UserId,
                                FollowedBack = response.FollowedBack ? 1 : 0,
                                Time = DateTimeUtilities.GetEpochTime()
                            });
                }
                // Add data to respected Account InteractedUsers table
                AccountDbOperation.Add(new InteractedUsers()
                {
                    ActivityType = ActivityType.ToString(),
                    Date = DateTimeUtilities.GetEpochTime(),
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    InteractedUsername = scrapeResult.ResultUser.Username,
                    InteractedUserId = ((InstagramUser)scrapeResult.ResultUser).UserId,
                    FollowedBack = response.FollowedBack ? 1 : 0,
                    Time = DateTimeUtilities.GetEpochTime()
                });
                // Remove data from respected Account Friendships table
                Friendships friendships = AccountDbOperation.GetSingle<Friendships>(x =>
                    (x.Username == scrapeResult.ResultUser.Username) &&
                    (x.UserId == ((InstagramUser)scrapeResult.ResultUser).UserId));
                AccountDbOperation.Remove(friendships);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }

    }
}
