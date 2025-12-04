using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace RedditDominatorCore.RDLibrary.Processors.Channel
{
    internal abstract class BaseRedditChannelProcessor : BaseRedditProcessor
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private IRdBrowserManager _newBrowserWindow;

        protected BaseRedditChannelProcessor(IRdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignService campaignService, IRedditFunction redditFunction, IRdBrowserManager browserManager)
            : base(jobProcess, dbAccountService, campaignService, redditFunction, browserManager)
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
        }

        public void StartKeywordSubscribeProcess(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<SubRedditModel> subReddits)
        {
            CampaignDetails campaignDetails = null;
            if (JobProcess.ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts || JobProcess.ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost)
                campaignDetails = JobProcess.CampaignDetails;
            try
            {
                var objCommunity = new ScrapeFilter.Community(JobProcess.ModuleSetting);
                var FilteredCount = 0;
                if (objCommunity.IsFilterApplied())
                    FilteredCount = subReddits.RemoveAll(subReddit => !objCommunity.AppplyFilters(subReddit));
                if (subReddits.Count == 0 && FilteredCount > 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType, $"Filter Not Matched For Any Channel.");
                    return;
                }
                else if (FilteredCount > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType, $"{subReddits.Count} Results Matched With Filter.");
                var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                var templateModel = templatesFileManager.GetTemplateById(JobProcess.TemplateId);
                _newBrowserWindow = _accountScopeFactory[$"{JobProcess.AccountId}"].Resolve<IRdBrowserManager>();
                foreach (var subReddit in subReddits)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (!CheckSubRedditUniqueness(jobProcessResult, subReddit, ActivityType)) continue;
                    if (AlreadyInteractedSubReddit(subReddit.Url)) continue;
                    if (!ApplyCampaignLevelSettings(queryInfo, subReddit.Url, campaignDetails)) continue;
                    if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser || ActivityType == ActivityType.ChannelScraper)
                        StartFinalChannelProcess(ref jobProcessResult, subReddit, queryInfo);
                    else
                        StartFinalChannelProcess(ref jobProcessResult, subReddit, queryInfo);
                }
            }
            catch (OperationCanceledException)
            {
                if (_browserManager != null && _browserManager.BrowserWindow != null) _browserManager.CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                if (_browserManager != null && _browserManager.BrowserWindow != null) _browserManager.CloseBrowser();
                jobProcessResult.IsProcessCompleted = true;
            }
        }

        public void StartCustomSubscribeProcess(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            SubRedditModel subReddit)
        {
            CampaignDetails campaignDetails = null;
            if (JobProcess.ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts ||
                JobProcess.ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost)
                campaignDetails = JobProcess.CampaignDetails;
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var objCommunity = new ScrapeFilter.Community(JobProcess.ModuleSetting);
                if (objCommunity.IsFilterApplied() && !objCommunity.AppplyFilters(subReddit))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                        $"Filter Not Matched For {subReddit.DisplayText}");
                    return;
                }

                if (!CheckSubRedditUniqueness(jobProcessResult, subReddit, ActivityType)) return;
                if (AlreadyInteractedSubReddit(subReddit.DisplayText.Replace("/r/", string.Empty))) return;
                if (!ApplyCampaignLevelSettings(queryInfo, subReddit.Url, campaignDetails)) return;

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                StartFinalChannelProcess(ref jobProcessResult, subReddit, queryInfo);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void StartFinalChannelProcess(ref JobProcessResult jobProcessResult, SubRedditModel subReddits,
            QueryInfo queryInfo)
        {
            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew
            {
                ResultPage = subReddits,
                QueryInfo = queryInfo
            });
        }
    }
}