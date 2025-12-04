using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDUtility;
using RedditDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace RedditDominatorCore.RDLibrary.Processors.CommentScraper
{
    internal abstract class BaseRedditCommentScraperProcessor : BaseRedditProcessor
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private IRdBrowserManager _newBrowserWindow;

        protected BaseRedditCommentScraperProcessor(IRdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbGlobalService globalService,
            IDbCampaignService campaignService, IRedditFunction redditFunction, IRdBrowserManager browserManager)
            : base(jobProcess, dbAccountService, campaignService, redditFunction, browserManager)
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
        }

        public void StartKeywordCommentScraperProcess(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<RedditPost> newredditPostList)
        {
            var lstOfRedditPostAfterFilterCheckOrUncheck = new List<RedditPost>();
            CampaignDetails campaignDetails = null;
            if (JobProcess.ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts ||
                JobProcess.ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost)
            {
                var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
                campaignDetails = campaignFileManager.FirstOrDefault(x => x.TemplateId == JobProcess.TemplateId);
            }

            try
            {
                var objPost = new ScrapeFilter.Post(JobProcess.ModuleSetting);
                var IsFilterApplied = objPost.IsFilterApplied();
                if (objPost != null && IsFilterApplied)
                {
                    newredditPostList.ForEach(post =>
                    {
                        if (objPost.ApplyFilters(post))
                            lstOfRedditPostAfterFilterCheckOrUncheck.Add(post);
                    });
                }
                else
                    lstOfRedditPostAfterFilterCheckOrUncheck.AddRange(newredditPostList);
                if (lstOfRedditPostAfterFilterCheckOrUncheck.Count == 0 && IsFilterApplied)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType, "Filter Not Matched");
                }
                else
                {
                    if (IsFilterApplied)
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                            $"{lstOfRedditPostAfterFilterCheckOrUncheck.Count} Result Matched With Filter");

                    foreach (var post in lstOfRedditPostAfterFilterCheckOrUncheck)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (!CheckPostUniqueNess(jobProcessResult, post, ActivityType)) continue;
                        if (AlreadyInteractedPost(post.Permalink)) continue;
                        if (!ApplyCampaignLevelSettings(queryInfo, post.Permalink, campaignDetails)) continue;

                        RedditCommentRespondHandler redditCommentRespondHandler = null;
                        if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                        {
                            redditCommentRespondHandler = RedditFunction.ScrapeCommentByUrl(
                                JobProcess.DominatorAccountModel, post.Permalink, queryInfo,
                                redditCommentRespondHandler, null);
                            redditCommentRespondHandler.LstCommentOnRedditPost.ForEach(comment => { comment.NumComments = post.NumComments; });
                        }

                        //For browser automation
                        else
                        {
                            _newBrowserWindow = _accountScopeFactory[$"{JobProcess.AccountId}{post.Id}"]
                                .Resolve<IRdBrowserManager>();
                            var response = _newBrowserWindow.TryAndGetResponse(JobProcess.DominatorAccountModel, post.Permalink, 3, string.Empty, true);
                            redditCommentRespondHandler = new RedditCommentRespondHandler(response, false, null);
                            _newBrowserWindow.CloseBrowser();
                        }

                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                            "Found " + post.NumComments + " Comments in this " +
                            post.Permalink);

                        if (redditCommentRespondHandler == null || !redditCommentRespondHandler.Success ||
                            redditCommentRespondHandler.LstCommentOnRedditPost.Count == 0)
                        {
                            jobProcessResult.HasNoResult = true;
                            jobProcessResult.maxId = null;
                            return;
                        }

                        foreach (var Items in redditCommentRespondHandler.LstCommentOnRedditPost)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            Items.NumComments = post.NumComments;
                            StartFinalCommentScraperProcess(ref jobProcessResult, Items, queryInfo);
                        }

                        JobProcess.DelayBeforeNextActivity();

                        if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                            _newBrowserWindow.CloseBrowser();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _newBrowserWindow.CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void StartCustomCommentScraperProcess(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            RedditPost newRedditPost, IResponseParameter response)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                CampaignDetails campaignDetails = null;
                if (JobProcess.ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts ||
                    JobProcess.ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost)
                {
                    var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
                    campaignDetails = campaignFileManager.FirstOrDefault(x => x.TemplateId == JobProcess.TemplateId);
                }

                var objPost = new ScrapeFilter.Post(JobProcess.ModuleSetting);
                if (objPost.IsFilterApplied() && !objPost.ApplyFilters(newRedditPost))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType, "Filter Not Matched");
                    return;
                }
                if (!CheckPostUniqueNess(jobProcessResult, newRedditPost, ActivityType)) return;
                if (!ApplyCampaignLevelSettings(queryInfo, queryInfo.QueryValue, campaignDetails)) return;

                var userId = Utils.GetLastWordFromUrl(queryInfo.QueryValue);

                //If customUrl for comment scraper don't have postid then we are scraping all comment present on that url
                if (!userId.Length.Equals(7))
                    userId = "";
                RedditCommentRespondHandler redditCommentRespondHandler = null;

                if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    redditCommentRespondHandler = RedditFunction.ScrapeCommentByUrl(JobProcess.DominatorAccountModel,
                        newRedditPost.Permalink, queryInfo, redditCommentRespondHandler, userId);

                //For browser automation
                else
                    redditCommentRespondHandler = new RedditCommentRespondHandler(response, false, null, userId);

                GlobusLogHelper.log.Info(Log.CustomMessage,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                    "Found " + redditCommentRespondHandler.LstCommentOnRedditPost.Count + " Comments in this " +
                    newRedditPost.Permalink);

                foreach (var comments in redditCommentRespondHandler.LstCommentOnRedditPost)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    StartFinalCommentScraperProcess(ref jobProcessResult, comments, queryInfo);
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void StartFinalCommentScraperProcess(ref JobProcessResult jobProcessResult, RedditPost newRedditPost,
            QueryInfo queryInfo)
        {
            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew
            {
                ResultPost = newRedditPost,
                QueryInfo = queryInfo
            });
        }
    }
}