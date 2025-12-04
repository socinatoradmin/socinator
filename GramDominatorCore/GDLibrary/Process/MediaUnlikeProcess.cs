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
using GramDominatorCore.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static GramDominatorCore.GDEnums.Enums;
using Newtonsoft.Json.Linq;
using GramDominatorCore.GDUtility;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary
{
    public class MediaUnlikeProcess : GdJobProcessInteracted<InteractedPosts>
    {
        public MediaUnlikerModel MediaUnlikerModel { get; set; }
        private BlackListWhitelistHandler BlackListWhitelistHandler { get;  }
        private int ActionBlockedCount;
        public MediaUnlikeProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IGdQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, IGdBrowserManager gdBrowser, IDelayService _delayService) : 
            base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser,_delayService)
        {
           // var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            MediaUnlikerModel = JsonConvert.DeserializeObject<MediaUnlikerModel>(templateModel.ActivitySettings);
            BlackListWhitelistHandler = new BlackListWhitelistHandler(ModuleSetting, DominatorAccountModel, ActivityType);
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            
            var jobProcessResult = new JobProcessResult();
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var instagramPost = scrapeResult.ResultPost as InstagramPost;
                var Post = $"https://www.instagram.com/p/{instagramPost?.Code}/";
                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, Post);
                var mediaInfo = 
                    GramStatic.IsBrowser ?
                    instaFunct.GdBrowserManager.MediaInfo(DominatorAccountModel, instagramPost.Code, JobCancellationTokenSource.Token)
                    : instaFunct.MediaInfo(DominatorAccountModel, AccountModel, instagramPost.Code, JobCancellationTokenSource.Token).Result;
                instagramPost = mediaInfo.InstagramPost;
                if (CheckAndSkipAsWhiteListed(instagramPost.User?.Username))
                    return jobProcessResult;
                var unlikeMediaResponse = 
                    GramStatic.IsBrowser ?
                    instaFunct.GdBrowserManager.UnlikeMedia(DominatorAccountModel, instagramPost.Code, JobCancellationTokenSource.Token)
                    : instaFunct.UnlikeMedia(DominatorAccountModel, AccountModel, JobCancellationTokenSource.Token,instagramPost?.Code).Result;

                if (unlikeMediaResponse!=null && unlikeMediaResponse.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,Post);

                    IncrementCounters();
                    scrapeResult.ResultPost = instagramPost;
                    AddUnlikedMediaDetailsIntoDataBase(scrapeResult);

                    // Add to Blacklist
                    if (MediaUnlikerModel.ManageBlackWhiteListModel.IsAddToBlackListOnceUnfollowed)
                        BlackListWhitelistHandler.AddToBlackList(instagramPost.User.Pk, instagramPost.User.Username);
                    
                    jobProcessResult.IsProcessSuceessfull = true;
                }              
                else
                {
                    if (!CheckResponse.CheckProcessResponse(unlikeMediaResponse, DominatorAccountModel, ActivityType, scrapeResult,ref ActionBlockedCount))
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

        private bool CheckAndSkipAsWhiteListed(string username)
        {
            var IsSkipped = false;
            try
            {
                var moduleSetting = ModuleSetting.ManageBlackWhiteListModel;
                var Users = new List<string>();
                if (moduleSetting.IsSkipWhiteListUsers)
                {
                    if(moduleSetting.IsSkipPrivateWhiteList)
                        Users = BlackListWhitelistHandler.GetWhiteListUsers(WhitelistblacklistType.Private);
                    else if(moduleSetting.IsSkipGroupWhiteList)
                        Users = BlackListWhitelistHandler.GetWhiteListUsers(WhitelistblacklistType.Group);
                    if (Users.Count > 0)
                        if(IsSkipped = Users.Any(x => x == username))
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "Skipped Post As WhiteListed User.");

                }
            }
            catch (Exception) { }
            return IsSkipped;
        }

        private void AddUnlikedMediaDetailsIntoDataBase(ScrapeResultNew scrapeResult)
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
                        UsernameOwner = instagramPost.User.Username,
                        Username = DominatorAccountModel.AccountBaseModel.UserName
                    });
            }

            // Add data to respected Account InteractedPosts table
            AccountDbOperation.Add(
                new InteractedPosts()
                {
                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                    MediaType = instagramPost.MediaType,
                    ActivityType = ActivityType,
                    PkOwner = instagramPost.Code,
                    UsernameOwner = instagramPost.User.Username,
                    Username = DominatorAccountModel.AccountBaseModel.UserName
                });
        }
    }
}
