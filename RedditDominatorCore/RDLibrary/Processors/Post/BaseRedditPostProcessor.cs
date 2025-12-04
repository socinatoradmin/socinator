using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
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

namespace RedditDominatorCore.RDLibrary.Processors.Post
{
    internal abstract class BaseRedditPostProcessor : BaseRedditProcessor
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly IDbAccountService _dbAccountService;
        private readonly DownvoteModel _downvoteModel;
        private readonly PostAutoActivityModel autoActivityModel;
        private readonly UpvoteModel _upvoteModel;
        IRdBrowserManager _browserManager;
        public readonly IProcessScopeModel processScope;
        protected BaseRedditPostProcessor(IProcessScopeModel processScopeModel, IRdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IRedditFunction redditFunction, IRdBrowserManager browserManager)
            : base(jobProcess, dbAccountService, campaignService, redditFunction, browserManager)
        {
            processScope = processScopeModel;
            _dbAccountService = dbAccountService;
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _upvoteModel = processScopeModel.GetActivitySettingsAs<UpvoteModel>();
            _downvoteModel = processScopeModel.GetActivitySettingsAs<DownvoteModel>();
            autoActivityModel = processScopeModel.GetActivitySettingsAs<PostAutoActivityModel>();
            _browserManager = browserManager;
        }
        public void StartAutoActivity(ref JobProcessResult jobProcessResult)
        {
            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew
            {
                QueryInfo = new QueryInfo { QueryType = "Home Feed", QueryValue = RdConstants.NewRedditHomePageAPI }
            });
        }
        public void StartKeywordPostProcess(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<RedditPost> lstRedditPost)
        {
            try
            {
                var lstOfRedditPostAfterFilterCheckOrUncheck = new List<RedditPost>();
                CampaignDetails campaignDetails = null;
                if (JobProcess.ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts ||
                    JobProcess.ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost)
                    campaignDetails = JobProcess.CampaignDetails;
                var objPost = new ScrapeFilter.Post(JobProcess.ModuleSetting);
                var IsFilterApplied = objPost.IsFilterApplied();
                if (IsFilterApplied && lstRedditPost?.Count > 0)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    lstRedditPost.ForEach(redditPost =>
                    {
                        if (objPost.ApplyFilters(redditPost))
                            lstOfRedditPostAfterFilterCheckOrUncheck.Add(redditPost);
                    });
                }
                else
                    lstOfRedditPostAfterFilterCheckOrUncheck.AddRange(lstRedditPost);
                var SkippedCount = 0;
                if ((SkippedCount = lstOfRedditPostAfterFilterCheckOrUncheck.RemoveAll(post => post.IsArchived)) > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Skipped {{ {SkippedCount} }} Archived Post");
                if (JobProcess.ActivityType == ActivityType.RemoveVote && (SkippedCount = lstOfRedditPostAfterFilterCheckOrUncheck.RemoveAll(post => post.VoteState == 0)) > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"Skipped {{ {SkippedCount} }} Un Upvoted Post");
                if (lstOfRedditPostAfterFilterCheckOrUncheck.Count == 0 && IsFilterApplied)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType, "Filter Not Matched");
                    jobProcessResult.IsProcessCompleted = true;
                }
                else
                {
                    if (IsFilterApplied)
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                            $"{lstOfRedditPostAfterFilterCheckOrUncheck.Count} Result Matched With Filter");

                    //To check with number of upvote per account by Keyword
                    if (ActivityType == ActivityType.Upvote && _upvoteModel.UpvotePostOnQueryWithLimit)
                    {
                        var upvoteCountPerAccount = _upvoteModel.NumberOfUpvotePostOnQuery.GetRandom();
                        var upvoteCount = 1;
                        foreach (var redditPost in lstOfRedditPostAfterFilterCheckOrUncheck)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            if (redditPost.VoteState == 1) continue;
                            if (!CheckPostUniqueNess(jobProcessResult, redditPost, ActivityType)) continue;
                            if (AlreadyInteractedPost(redditPost.Permalink)) continue;
                            if (!ApplyCampaignLevelSettings(queryInfo, redditPost.Permalink, campaignDetails)) continue;
                            if (upvoteCount++ >= upvoteCountPerAccount)
                            {
                                jobProcessResult.IsProcessCompleted = true;
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    $"Upvote Post Per Account Limit Reached on Query {queryInfo.QueryValue} " +
                                    "\nSearching For Next Query If Any...");
                                return;
                            }

                            if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser ||
                                ActivityType.UrlScraper == ActivityType)
                            {
                                StartFinalPostProcess(ref jobProcessResult, redditPost, queryInfo);
                            }

                            //For browser automation
                            else
                            {
                                if (_browserManager == null || _browserManager.BrowserWindow == null)
                                    _browserManager = _accountScopeFactory[$"{JobProcess.AccountId}"].Resolve<IRdBrowserManager>();
                                var response = _browserManager.TryAndGetResponse(JobProcess.DominatorAccountModel, redditPost.Permalink, 3, string.Empty, false);
                                var _redditPostResponseHandler = new RedditPostResponseHandler(response, false, null);
                                var Post = _redditPostResponseHandler.LstRedditPost.Count > 0 ? _redditPostResponseHandler.LstRedditPost.FirstOrDefault() : redditPost;
                                StartFinalPostProcess(ref jobProcessResult, Post,
                                    queryInfo);
                            }
                        }
                    }

                    //To check with number of downvote per account by Keyword
                    else if (ActivityType == ActivityType.Downvote && _downvoteModel.DownvotePostOnQueryWithLimit)
                    {
                        var downvoteCountPerAccount = _downvoteModel.NumberOfDownvotePostOnQuery.GetRandom();
                        var downvoteCount = 1;
                        foreach (var redditPost in lstOfRedditPostAfterFilterCheckOrUncheck)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            if (redditPost.VoteState == -1) continue;
                            if (!CheckPostUniqueNess(jobProcessResult, redditPost, ActivityType)) continue;
                            if (AlreadyInteractedPost(redditPost.Permalink)) continue;
                            if (!ApplyCampaignLevelSettings(queryInfo, redditPost.Permalink, campaignDetails)) continue;

                            if (downvoteCount++ > downvoteCountPerAccount)
                            {
                                jobProcessResult.IsProcessCompleted = true;
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    $"Downvote Post Per Account Limit Reached on Query {queryInfo.QueryValue} " +
                                    "\nSearching For Next Query If Any...");
                                return;
                            }

                            if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser ||
                                ActivityType.UrlScraper == ActivityType)
                            {
                                StartFinalPostProcess(ref jobProcessResult, redditPost, queryInfo);
                            }

                            //For browser automation
                            else
                            {
                                if (_browserManager == null || _browserManager.BrowserWindow == null)
                                    _browserManager = _accountScopeFactory[$"{JobProcess.AccountId}"].Resolve<IRdBrowserManager>();
                                var response = _browserManager.TryAndGetResponse(JobProcess.DominatorAccountModel, redditPost.Permalink, 3, string.Empty, false);
                                var _redditPostResponseHandler = new RedditPostResponseHandler(response, false, null);
                                if (_redditPostResponseHandler.LstRedditPost.FirstOrDefault().IsArchived)
                                    continue;
                                StartFinalPostProcess(ref jobProcessResult, _redditPostResponseHandler?.LstRedditPost?.FirstOrDefault(),
                                    queryInfo);
                            }
                        }
                    }
                    else
                    {
                        foreach (var redditPost in lstOfRedditPostAfterFilterCheckOrUncheck)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            if (!CheckPostUniqueNess(jobProcessResult, redditPost, ActivityType)) continue;
                            if (AlreadyInteractedPost(redditPost.Permalink)) continue;
                            if (!ApplyCampaignLevelSettings(queryInfo, redditPost.Permalink, campaignDetails)) continue;
                            if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser ||
                                ActivityType.UrlScraper == ActivityType)
                            {
                                StartFinalPostProcess(ref jobProcessResult, redditPost, queryInfo);
                            }
                            else
                            {
                                var response = _browserManager.TryAndGetResponse(JobProcess.DominatorAccountModel, redditPost.Permalink, 3, string.Empty);
                                var _redditPostResponseHandler = new RedditPostResponseHandler(response, false, null);
                                var Post = _redditPostResponseHandler.LstRedditPost.Count > 0 ? _redditPostResponseHandler.LstRedditPost.FirstOrDefault() : redditPost;
                                StartFinalPostProcess(ref jobProcessResult, Post, queryInfo);
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _browserManager.CloseBrowser();
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void StartCustomPostProcess(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            RedditPost newRedditPost)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                CampaignDetails campaignDetails = null;
                if (JobProcess.ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts ||
                    JobProcess.ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost)
                    campaignDetails = JobProcess.CampaignDetails;
                var objPost = new ScrapeFilter.Post(JobProcess.ModuleSetting);
                if (objPost.IsFilterApplied() && !objPost.ApplyFilters(newRedditPost))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                        $"Filter Not Matched For {newRedditPost.Permalink}");
                    return;
                }

                if ((ActivityType == ActivityType.Upvote || ActivityType == ActivityType.UpvoteComment) && newRedditPost.Upvoted)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType, "Already Upvoted");
                    jobProcessResult.HasNoResult = true;
                    jobProcessResult.IsProcessCompleted = true;
                    return;
                }
                if ((ActivityType == ActivityType.RemoveVote ||ActivityType==ActivityType.RemoveVoteComment) && !(newRedditPost.Upvoted || newRedditPost.Downvoted))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType, "Its not voted by the user");
                    jobProcessResult.HasNoResult = true;
                    jobProcessResult.IsProcessCompleted = true;
                    return;
                }
                if (!CheckPostUniqueNess(jobProcessResult, newRedditPost, ActivityType)) return;
                if (!ApplyCampaignLevelSettings(queryInfo, queryInfo.QueryValue, campaignDetails)) return;

                StartFinalPostProcess(ref jobProcessResult, newRedditPost, queryInfo);
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

        protected bool AlreadyInteractedPostForEditComment(string permalink, string comment)
        {
            try
            {
                return _dbAccountService.GetInteractedPostPermLinkforComment(ActivityType, permalink, comment).Count >
                       0;
            }
            catch (Exception)
            {
                // ignored
            }

            return false;
        }

        protected void StartFinalPostProcess(ref JobProcessResult jobProcessResult, RedditPost newRedditPost,
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