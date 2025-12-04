using DominatorHouseCore.Diagnostics;
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

namespace RedditDominatorCore.RDLibrary.Processors.Post
{
    internal class SocianatorPublisherCampaignProcessor : BaseRedditPostProcessor
    {
        private RedditPostResponseHandler _redditPostResponseHandler;

        public SocianatorPublisherCampaignProcessor(IProcessScopeModel processScopeModel, IRdJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, IRedditFunction redditFunction, IRdBrowserManager browserManager)
            : base(processScopeModel, jobProcess, dbAccountService, globalService, campaignService, redditFunction,
                browserManager)
        {
            _redditPostResponseHandler = null;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            var redditPost = new RedditPost();
            var publishedPostDetails =
                PublisherInitialize.GetNetworksPublishedPost(queryInfo.QueryValue, SocialNetworks.Reddit);

            GlobusLogHelper.log.Info(Log.FoundXResults,
                JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                JobProcess.DominatorAccountModel.AccountBaseModel.UserName, publishedPostDetails.Count,
                queryInfo.QueryType, queryInfo.QueryValue, ActivityType);

            foreach (var publishedPost in publishedPostDetails)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (publishedPost.Link == null) continue;
                try
                {
                    if (!publishedPost.Link.ToLower()
                            .Contains($"https://www.reddit.com/user/{publishedPost.AccountName?.ToLower()}/comments/")
                        && !publishedPost.Link.ToLower()
                            .Contains(publishedPost.DestinationUrl?.ToLower().TrimEnd('/') + "/comments") || queryInfo.QueryType == "Socinator Publisher Campaign")
                    {
                        _redditPostResponseHandler = RedditFunction.ScrapePostsByCampaignIdUrl(
                            JobProcess.DominatorAccountModel,
                            $"https://www.reddit.com/user/{publishedPost.AccountName}", _redditPostResponseHandler);
                        foreach (var postid in _redditPostResponseHandler.LstRedditPost)
                        {
                            if (postid.Title != publishedPost.Title || postid.Author != publishedPost.AccountName.Trim() ||
                                !postid.Permalink.Contains(publishedPost.DestinationUrl.Trim())) continue;
                            publishedPost.Link = postid.Permalink;
                            redditPost = postid;
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Reddit,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName,
                        ActivityType, "The Page You Requested Does Not Exist");
                    return;
                }

                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                _redditPostResponseHandler = null;
                _redditPostResponseHandler = RedditFunction.ScrapePostsByUrl(JobProcess.DominatorAccountModel,
                    publishedPost.Link, queryInfo, _redditPostResponseHandler);
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (_redditPostResponseHandler != null && _redditPostResponseHandler.Success &&
                    _redditPostResponseHandler.LstRedditPost.Count == 0 && !string.IsNullOrEmpty(redditPost.Title))
                {
                    _redditPostResponseHandler.LstRedditPost.Add(redditPost);
                    _redditPostResponseHandler.HasMoreResults = true;
                }
                if (_redditPostResponseHandler == null || !_redditPostResponseHandler.Success ||
                    _redditPostResponseHandler.LstRedditPost.Count == 0)
                {
                    if (!string.IsNullOrEmpty(_redditPostResponseHandler.Response))
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"Not found {publishedPost.Link}");
                    continue;
                }

                if (_redditPostResponseHandler.HasMoreResults == false)
                {
                    GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType);
                    jobProcessResult.HasNoResult = true;
                }

                StartCustomPostProcess(queryInfo, ref jobProcessResult, _redditPostResponseHandler.LstRedditPost[0]);
                jobProcessResult.maxId = _redditPostResponseHandler.PaginationParameter.LastPaginationId;
            }

            jobProcessResult.HasNoResult = true;
        }
    }
}