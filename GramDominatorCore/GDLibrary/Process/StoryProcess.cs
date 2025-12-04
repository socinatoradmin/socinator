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
using System.Linq;

namespace GramDominatorCore.GDLibrary.Process
{
    public class StoryProcess : GdJobProcessInteracted<InteractedUsers>
    {
        public StoryModel StoryModel { get; set; }
        private readonly BlackListWhitelistHandler _blackListWhitelistHandler;
        public StoryProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IGdQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, IGdLogInProcess logInProcess, IGdBrowserManager gdBrowser, IDelayService _delayService)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser, _delayService)
        {
            StoryModel = JsonConvert.DeserializeObject<StoryModel>(templateModel.ActivitySettings);
            _blackListWhitelistHandler = new BlackListWhitelistHandler(ModuleSetting, DominatorAccountModel, ActivityType);
            loginProcess = logInProcess;
        }
        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            instaFunct = loginProcess.InstagramFunctFactory.InstaFunctions;
            int ActionBlockedCount = 0;
            JobProcessResult jobProcessResult = new JobProcessResult();
            try
            {
                InstagramUser instagramUser = (InstagramUser)scrapeResult.ResultUser;
                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,instagramUser?.Username);
                CommonIgResponseHandler response = null;
                if (!DominatorAccountModel.IsRunProcessThroughBrowser)
                    response = instaFunct.SeenUserStory(DominatorAccountModel, AccountModel, instagramUser.UserStories);
                else
                {
                    //var userInfo = instaFunct.GdBrowserManager.GetUserInfo(DominatorAccountModel, instagramUser.Username, JobCancellationTokenSource.Token);
                    var userInfo = instaFunct.SearchUsername(DominatorAccountModel, instagramUser.Username, JobCancellationTokenSource.Token);
                    instagramUser = userInfo;
                    var storyResponse = instaFunct.GdBrowserManager.GetStoriesUser(DominatorAccountModel, instagramUser, JobCancellationTokenSource.Token);
                    if (storyResponse.LstUsers.Count > 0)
                        instagramUser.UserStories = storyResponse.LstUsers.Where(x => x.UserId == instagramUser.Pk).FirstOrDefault().LstMedia;
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Story Not Found for {scrapeResult.ResultUser.Username}");
                        return jobProcessResult;
                    }
                    response = instaFunct.GdBrowserManager.SeenUserStory(DominatorAccountModel, instagramUser, JobCancellationTokenSource.Token);
                }
                if ((response != null && response.Success))
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultUser.Username);
                    IncrementCounters();
                    AddFollowedDataToDataBase(scrapeResult, "Story Seen");
                    if (StoryModel.ManageBlackWhiteListModel.IsAddToBlackListOnceUnfollowed)
                    {
                        _blackListWhitelistHandler.AddToBlackList(instagramUser.UserId, instagramUser.Username);
                    }
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    if (!CheckResponse.CheckProcessResponse(response, DominatorAccountModel, ActivityType, scrapeResult, ref ActionBlockedCount, 1))
                    {
                        Stop();
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    }
                    jobProcessResult.IsProcessSuceessfull = false;

                }
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                // Delay between each activity
                DelayBeforeNextActivity();

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
            return jobProcessResult;

        }

        private void AddFollowedDataToDataBase(ScrapeResultNew scrapeResult, string status)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            try
            {
                // Add data to respected campaign InteractedUsers table
                if (!string.IsNullOrEmpty(CampaignId))
                {
                    // CampaignDbOperation

                    // Thread.Sleep(15000);
                    CampaignDbOperation?.Add(
                            new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedUsers()
                            {
                                ActivityType = ActivityType.ToString(),
                                Date = DateTimeUtilities.GetEpochTime(),
                                QueryType = scrapeResult.QueryInfo.QueryType,
                                Query = scrapeResult.QueryInfo.QueryValue,
                                Username = DominatorAccountModel.AccountBaseModel.UserName,
                                InteractedUsername = scrapeResult.ResultUser.Username,
                                InteractedUserId = ((InstagramUser)scrapeResult.ResultUser).Pk,
                                Time = DateTimeUtilities.GetEpochTime(),
                                Status = status
                            });
                }

                // Add data to respected Account InteractedUsers table;
                AccountDbOperation.Add(new InteractedUsers()
                {
                    ActivityType = ActivityType.ToString(),
                    Date = DateTimeUtilities.GetEpochTime(),
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    Query = scrapeResult.QueryInfo.QueryValue,
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    InteractedUsername = scrapeResult.ResultUser.Username,
                    InteractedUserId = ((InstagramUser)scrapeResult.ResultUser).Pk,
                    Time = DateTimeUtilities.GetEpochTime(),
                    Status = status

                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

    }
}
