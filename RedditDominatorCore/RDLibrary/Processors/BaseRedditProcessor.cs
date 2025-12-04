using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.RdTables.Campaigns;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.BrowserManager.BrowserUtility;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDModel;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using ThreadUtils;

namespace RedditDominatorCore.RDLibrary.Processors
{
    internal abstract class BaseRedditProcessor : IQueryProcessor
    {
        private static readonly ConcurrentDictionary<string, object> LockObjects =
            new ConcurrentDictionary<string, object>();

        public readonly IRdBrowserManager _browserManager;
        private readonly ICampaignInteractionDetails _campaignInteractionDetails;
        private readonly IDbCampaignService _campaignService;
        private readonly IDbAccountService _dbAccountService;
        private readonly IDelayService _delayService;
        private readonly IGlobalInteractionDetails _globalInteractionDetails;
        protected readonly IRdJobProcess JobProcess;
        protected readonly IRedditFunction RedditFunction;

        protected BaseRedditProcessor(IRdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignService campaignService, IRedditFunction redditFunction, IRdBrowserManager browserManager)
        {
            JobProcess = jobProcess;
            _dbAccountService = dbAccountService;
            _campaignService = campaignService;
            RedditFunction = redditFunction;
            _browserManager = browserManager;
            _delayService = InstanceProvider.GetInstance<IDelayService>();
            _campaignInteractionDetails = InstanceProvider.GetInstance<ICampaignInteractionDetails>();
            _globalInteractionDetails = InstanceProvider.GetInstance<IGlobalInteractionDetails>();
        }

        protected ActivityType ActivityType => JobProcess.ActivityType;

        public void Start(QueryInfo queryInfo)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var jobProcessResult = new JobProcessResult();
            GlobusLogHelper.log.Info(Log.CustomMessage,
               JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
               JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
               $"Searching for {queryInfo.QueryType} {queryInfo.QueryValue}");
            while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                Process(queryInfo, ref jobProcessResult);
        }

        protected abstract void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult);

        protected bool AlreadyInteractedUser(string username)
        {
            return _dbAccountService.GetInteractedUserName(ActivityType, username).Count > 0;
        }

        protected bool CheckUserUniqueNess(JobProcessResult jobProcessResult, string username,
            ActivityType activityType)
        {
            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleConfiguration =
                jobActivityConfigurationManager[JobProcess.DominatorAccountModel.AccountId, ActivityType];
            if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
                if (JobProcess.ModuleSetting.IschkUniqueUserForCampaign)
                    try
                    {
                        _globalInteractionDetails.AddInteractedData(SocialNetworks.Reddit, activityType, username);
                    }
                    catch (Exception)
                    {
                        jobProcessResult.IsProcessSuceessfull = false;
                        return false;
                    }

            if (!JobProcess.ModuleSetting.IschkUniqueUserForAccount) return true;
            try
            {
                if (_dbAccountService.GetInteractedUserName(ActivityType, username).Count > 0) return false;
            }
            catch (Exception)
            {
                return true;
            }

            return true;
        }

        protected bool AlreadyInteractedSubReddit(string url)
        {
            return _dbAccountService.GetInteractedSubredditUrl(ActivityType, url).Count > 0;
        }

        protected bool CheckSubRedditUniqueness(JobProcessResult jobProcessResult, SubRedditModel subReddit,
            ActivityType activityType)
        {
            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleConfiguration =
                jobActivityConfigurationManager[JobProcess.DominatorAccountModel.AccountId, ActivityType];
            if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
            {
                if (JobProcess.ModuleSetting.IschkUniquePostForCampaign)
                    try
                    {
                        _globalInteractionDetails.AddInteractedData(SocialNetworks.Reddit, activityType, subReddit.Url);
                    }
                    catch (Exception)
                    {
                        jobProcessResult.IsProcessSuceessfull = false;
                        return false;
                    }

                if (JobProcess.ModuleSetting.IschkUniqueUserForCampaign)
                    try
                    {
                        _globalInteractionDetails.AddInteractedData(SocialNetworks.Reddit, activityType, subReddit.Id);
                    }
                    catch (Exception)
                    {
                        jobProcessResult.IsProcessSuceessfull = false;
                        return false;
                    }
            }

            if (!JobProcess.ModuleSetting.IschkUniqueUserForAccount) return true;
            try
            {
                if (_dbAccountService.GetInteractedSubredditUrl(ActivityType, subReddit.Id).Count > 0) return false;
            }
            catch (Exception)
            {
                return true;
            }

            return true;
        }

        protected bool AlreadyInteractedPost(string permalink)
        {
            try
            {
                return _dbAccountService.GetInteractedPostPermLink(ActivityType, permalink).Count > 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        protected bool CheckPostUniqueNess(JobProcessResult jobProcessResult, RedditPost post,
            ActivityType activityType)
        {
            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleConfiguration =
                jobActivityConfigurationManager[JobProcess.DominatorAccountModel.AccountId, ActivityType];
            if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
            {
                if (JobProcess.ModuleSetting.IschkUniquePostForCampaign)
                    try
                    {
                        _campaignInteractionDetails.AddInteractedData(SocialNetworks.Reddit,
                            $"{JobProcess.CampaignId}.post", post.PostId);
                    }
                    catch (Exception)
                    {
                        return false;
                    }

                if (JobProcess.ModuleSetting.IschkUniqueUserForCampaign)
                    try
                    {
                        _campaignInteractionDetails.AddInteractedData(SocialNetworks.Reddit, JobProcess.CampaignId,
                            post.Author);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
            }

            if (!JobProcess.ModuleSetting.IschkUniqueUserForAccount) return true;
            try
            {
                if (_dbAccountService.GetInteractedPostPermLink(ActivityType, post.Author).Count > 0) return false;
            }
            catch (Exception)
            {
                return true;
            }

            return true;
        }

        protected bool ApplyCampaignLevelSettings(QueryInfo queryInfo, string postPermalink,
            CampaignDetails campaignDetails)
        {
            if (campaignDetails == null) return true;
            try
            {
                JobProcess.AddedToDb = false;

                #region Upvote From Random Percentage Of Accounts

                if (JobProcess.ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts)
                {
                    var lockObject = LockObjects.GetOrAdd("Lock1" + postPermalink, new object());
                    lock (lockObject)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        try
                        {
                            decimal count = campaignDetails.SelectedAccountList.Count;
                            var randomMaxAccountToPerform = (int)Math.Round(
                                count * JobProcess.ModuleSetting.PerformActionFromRandomPercentage.GetRandom() / 100);
                            var numberOfAccountsAlreadyPerformedAction = _campaignService
                                .GetInteractedPostPermaLink(postPermalink, ActivityType).Count;

                            if (randomMaxAccountToPerform <= numberOfAccountsAlreadyPerformedAction) return false;

                            JobProcess.AddedToDb =
                                _campaignService.AddToInteractedPost(
                                    SetPendingActivityValueToInsert(queryInfo, postPermalink, "Pending"));
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                }

                #endregion

                #region Delay Between Upvoting On SamePost

                if (JobProcess.ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost)
                {
                    var lockObject = LockObjects.GetOrAdd("Lock2" + postPermalink, new object());
                    lock (lockObject)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        var dbOperation = new DbOperations(campaignDetails.CampaignId, SocialNetworks.Reddit,
                            ConstantVariable.GetCampaignDb);

                        try
                        {
                            var activityType = ActivityType.ToString();
                            var recentlyPerformedActions = dbOperation
                                .Get<InteractedPost>(x =>
                                    x.ActivityType == activityType && x.Permalink == postPermalink &&
                                    (x.Status == "Success" || x.Status == "Working"))
                                .OrderByDescending(x => x.InteractionTimeStamp).Select(x => x.InteractionTimeStamp)
                                .Take(1).ToList();

                            if (recentlyPerformedActions.Count > 0)
                            {
                                var recentlyPerformedTime = recentlyPerformedActions[0];
                                var delay = JobProcess.ModuleSetting.DelayBetweenPerformingActionOnSamePost.GetRandom();
                                var time = DateTimeUtilities.GetEpochTime();
                                var time2 = recentlyPerformedTime + delay;
                                if (time < time2) _delayService.ThreadSleep((time2 - time) * 1000);
                            }

                            if (!JobProcess.AddedToDb)
                            {
                                JobProcess.AddedToDb =
                                    _campaignService.AddToInteractedPost(
                                        SetPendingActivityValueToInsert(queryInfo, postPermalink, "Working"));
                            }
                            else
                            {
                                var interactedPost = _campaignService.GetSingleInteractedPost(postPermalink,
                                    ActivityType, JobProcess.DominatorAccountModel);
                                interactedPost.InteractionDateTime = DateTime.Now;
                                interactedPost.InteractionTimeStamp = DateTimeUtilities.GetEpochTime();
                                interactedPost.Status = "Working";
                                dbOperation.Update(interactedPost);
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return true;
        }

        private InteractedPost SetPendingActivityValueToInsert(QueryInfo queryInfo, string postPermalink, string status)
        {
            return new InteractedPost
            {
                ActivityType = ActivityType.ToString(),
                QueryType = queryInfo.QueryType,
                Query = queryInfo.QueryValue,
                SinAccUsername = JobProcess.DominatorAccountModel.AccountBaseModel.UserName,
                SinAccId = JobProcess.DominatorAccountModel.AccountBaseModel.AccountId,
                InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                InteractionDateTime = DateTime.Now,
                Permalink = postPermalink,
                Status = status
            };
        }

        protected void AllowPermissionForAdultPageInBrowser(IResponseParameter response)
        {
            var attrPermissionToAdultCommunity = BrowserUtilities.GetPath(response.Response, "a", "Yes");
            _browserManager.BrowserWindow.BrowserAct(ActType.ClickByClass, attrPermissionToAdultCommunity, 2, 2);
            Thread.Sleep(TimeSpan.FromSeconds(5));
            var adultPermissionResp = _browserManager.BrowserWindow.GetPageSource();
            var i = 1;
            while (i++ < 3 && (!string.IsNullOrEmpty(adultPermissionResp) &&
                               adultPermissionResp.Contains("You must be 18+ to view this community")
                               || adultPermissionResp.Contains(
                                   "You must be at least eighteen years old to view this content. Are you over eighteen and willing to see adult content?")
                   ))
            {
                attrPermissionToAdultCommunity = BrowserUtilities.GetPath(adultPermissionResp, "a", "Yes");
                _browserManager.BrowserWindow.BrowserAct(ActType.ClickByClass, attrPermissionToAdultCommunity, 2, 2);
                Thread.Sleep(TimeSpan.FromSeconds(5));
                adultPermissionResp = _browserManager.BrowserWindow.GetPageSource();
            }
        }
    }
}