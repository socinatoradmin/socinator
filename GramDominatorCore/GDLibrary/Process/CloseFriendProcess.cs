using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.Factories;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDViewModel.GrowFollower;
using GramDominatorCore.Request;
using System;
using ThreadUtils;

namespace GramDominatorCore.GDLibrary.Process
{
    public class CloseFriendProcess : GdJobProcessInteracted<MakeCloseFriendAccount>
    {
        private readonly CloseFriendViewModel closeFriend;
        public CloseFriendProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped accountServiceScoped,
            IGdQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, 
            IGdBrowserManager gdBrowser, IDelayService _delayService) 
            : base(processScopeModel, accountServiceScoped, queryScraperFactory,
                  httpHelper, gdBrowser, _delayService)
        {
            closeFriend = processScopeModel.GetActivitySettingsAs<CloseFriendViewModel>();
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);
                var instagramUser = (InstagramUser)scrapeResult.ResultUser;
                var ClosedFriendsResponse = instaFunct.GdBrowserManager.MakeCloseFriends(DominatorAccountModel,instagramUser).Result;
                if(ClosedFriendsResponse != null && ClosedFriendsResponse.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            scrapeResult.ResultUser.Username);
                    instagramUser.IsBestie = true;
                    IncrementCounters();
                    AddToCloseFriendDB(instagramUser);
                    jobProcessResult.IsProcessSuceessfull = true;
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    DelayBeforeNextActivity();
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"{ClosedFriendsResponse?.FailedMessage}");
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                
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
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return jobProcessResult;
        }

        private void AddToCloseFriendDB(InstagramUser instagramUser)
        {
            try
            {
                CampaignDbOperation?.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.MakeCloseFriendCampaign
                {
                    AccountUserName = DominatorAccountModel.AccountBaseModel.UserName,
                    ActivityType = ActivityType.ToString(),
                    UserName = instagramUser?.Username,
                    IsCloseFriend = instagramUser.IsBestie,
                    InteractedDate = DateTime.Now,
                });
                AccountDbOperation?.Add(new MakeCloseFriendAccount
                {
                    AccountUserName = DominatorAccountModel.AccountBaseModel.UserName,
                    ActivityType = ActivityType.ToString(),
                    UserName = instagramUser?.Username,
                    IsCloseFriend = instagramUser.IsBestie,
                    InteractedDate = DateTime.Now.GetCurrentEpochTime(),
                });
            }
            catch { }
        }
    }
}
