using ThreadUtils;
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
using GramDominatorCore.Request;
using Newtonsoft.Json.Linq;
using System;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary
{
    public class DeletePostProcess : GdJobProcessInteracted<InteractedPosts>
    {
        public DeletePostModel DeletePostModel { get;set;}

        public DeletePostProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IGdQueryScraperFactory queryScraperFactory,
            IGdHttpHelper httpHelper, IGdBrowserManager gdBrowser, IDelayService _delayService)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser, _delayService)
        {
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultPost.Code);

            JobProcessResult jobProcessResult = new JobProcessResult();
            bool deleteStatus = false;

            try
            {

                var instagramPost = (InstagramPost)scrapeResult.ResultPost;
                var deleteMediaResponse = 
                    GramStatic.IsBrowser ?
                    instaFunct.GdBrowserManager.DeleteMedia(DominatorAccountModel, instagramPost, JobCancellationTokenSource.Token)
                    : instaFunct.DeleteMedia(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token, instagramPost?.Code).Result;
                
                if (deleteMediaResponse!=null && deleteMediaResponse.Success && deleteMediaResponse.DidDelete)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, GramStatic.GetUrl(scrapeResult.ResultPost.Code,false,true));

                    IncrementCounters();

                    AddPostDetailsIntoDataBase(scrapeResult);

                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else if(deleteMediaResponse != null && !deleteMediaResponse.Success && !deleteStatus &&deleteMediaResponse.ToString().Contains("could not delete"))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                         $"{scrapeResult.ResultPost.Code} Post could not delete");

                }
                else if (deleteMediaResponse.ToString().Contains("login_required") && deleteMediaResponse != null && deleteMediaResponse.Issue != null && deleteMediaResponse.Issue.Status == "Response Response Login session expired")
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                           $"Log in required please update your account");
                    Stop();
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultPost.Code, !deleteStatus ? "May be the media has been deleted already" : "Unknown");

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

        private void AddPostDetailsIntoDataBase(ScrapeResultNew scrapeResult)
        {
            InstagramPost instagramPost = (InstagramPost)scrapeResult.ResultPost;
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            // Add data to respected campaign InteractedPosts table
            if (!string.IsNullOrEmpty(CampaignId))
            {

                CampaignDbOperation?.Add(
                    new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts()
                    {
                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                        MediaType = instagramPost.MediaType,
                        ActivityType = ActivityType,
                        PkOwner = instagramPost.Code,
                        UsernameOwner = DominatorAccountModel.AccountBaseModel.UserName,
                        Username = DominatorAccountModel.AccountBaseModel.UserName
                    });
            }

            // Add data to respected Account InteractedPosts table
            AccountDbOperation.Add(
                new InteractedPosts
                {
                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                    MediaType = instagramPost.MediaType,
                    ActivityType = ActivityType,
                    PkOwner = instagramPost.Code,
                    UsernameOwner = DominatorAccountModel.AccountBaseModel.UserName,
                    Username = DominatorAccountModel.AccountBaseModel.UserName
                });
        }

        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }

    }
}
