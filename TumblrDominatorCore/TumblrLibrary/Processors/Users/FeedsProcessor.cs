using DominatorHouseCore.Enums.TumblrQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction.TumblrBrowserManager;
using TumblrDominatorCore.TumblrLibrary.TumblrProcesses;
using TumblrDominatorCore.TumblrResponseHandler;

namespace TumblrDominatorCore.TumblrLibrary.Processors.Users
{
    public class FeedsProcessor : BaseTumblrUserProcessor, IQueryProcessor
    {
        private readonly ITumblrBrowserManager _browser;

        public FeedsProcessor(ITumblrBrowserManager browser, IProcessScopeModel processScopeModel,
            ITumblrJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, ITumblrFunct tumblrFunct) : base(
            processScopeModel, jobProcess, dbAccountService, globalService, campaignService, tumblrFunct)
        {
            _browser = browser;
        }

        public string BeforeStamp { get; set; }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var lstTumblrCommenters = new List<TumblrUser>();
            FeedInteractionResponseHandler feedInteractionResponseHandler;
            if (queryInfo.QueryValue.Contains(".tumblr.com"))
            {
                var tumblrPost = TumblrUtility.getTumblrPostFromPostUrl(queryInfo.QueryValue);

                if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                {

                    feedInteractionResponseHandler = TumblrFunct.GetFeedInteractions(JobProcess.DominatorAccountModel, tumblrPost.Id, tumblrPost.OwnerUsername, queryInfo,
                                BeforeStamp);
                    if (!feedInteractionResponseHandler.Success)
                    {
                        GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                            JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.UserName, ((BaseTumblrProcessor)this).ActivityType);
                        jobProcessResult.HasNoResult = true;
                        jobProcessResult.maxId = null;
                    }
                    else
                    {
                        lstTumblrCommenters = GetListUsersFromFeeds(queryInfo, feedInteractionResponseHandler);
                    }

                    if (feedInteractionResponseHandler.FeedInteraction.Response.Links != null)
                        BeforeStamp = feedInteractionResponseHandler.FeedInteraction.Response.Links.Next.QueryParams
                            .BeforeTimestamp;
                    else
                        jobProcessResult.HasNoResult = true;
                }
                else
                {
                    var status = _browser.SearchPostDetails(JobProcess.DominatorAccountModel, ref tumblrPost);
                    var QueryActivity = TumblrUtility.GetActivityByQuery(queryInfo.QueryType);
                    var FeedResponse = _browser.GetFeedDetails(JobProcess.DominatorAccountModel, tumblrPost, QueryActivity.Item1, QueryActivity.Item2, QueryActivity.Item3);
                    var CommentersList = QueryActivity.Item1 ? FeedResponse.Item1 : QueryActivity.Item2 ? FeedResponse.Item2 : FeedResponse.Item3;
                    if (CommentersList.Count > 0)
                        lstTumblrCommenters = CommentersList.Select(x => x.commenter).ToList();
                }
                if (lstTumblrCommenters.Count == 0)
                {
                    GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, ((BaseTumblrProcessor)this).ActivityType);
                    jobProcessResult.HasNoResult = true;
                    jobProcessResult.maxId = null;
                    return;
                }

                GlobusLogHelper.log.Info(Log.FoundXResults,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, lstTumblrCommenters.Count,
                    queryInfo.QueryType, queryInfo.QueryValue, ((BaseTumblrProcessor)this).ActivityType);
                FilterAndStartFinalProcess(queryInfo, ref jobProcessResult, lstTumblrCommenters);
                if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    jobProcessResult.IsProcessCompleted = true;
            }
            else
            {
                GlobusLogHelper.log.Info(" QueryType => " + queryInfo.QueryType + " QueryValue => " +
                                         queryInfo.QueryValue +
                                         " is not in correct format , please go through help section or ask the support team to add correct format :) ");
                jobProcessResult.HasNoResult = true;
            }
        }


        public List<TumblrUser> GetListUsersFromFeeds(QueryInfo queryInfo,
            FeedInteractionResponseHandler feedsResponseHandler)
        {
            var lstTumblrUsers = new List<TumblrUser>();
            var xTumblrFormKey = string.Empty;
            try
            {
                {
                    if (queryInfo.QueryType == "LangKeyUsersWhoLikedPosts".FromResourceDictionary())
                    {
                        lstTumblrUsers = feedsResponseHandler.FeedInteraction.Response.notes.Where(x => x.type == "like")
                            .Select(x => new TumblrUser
                            {
                                Username = x.blog_name,
                                PageUrl = x.blog_url,
                                Uuid = x.blog_uuid,
                                IsFollowed = x.followed,
                                TumblrsFormKey = xTumblrFormKey
                            }).ToList();
                    }
                    else if (queryInfo.QueryType == EnumUtility.GetQueryFromEnum(TumblrQuery.UserReblogedThePost))
                    {
                        lstTumblrUsers = feedsResponseHandler.FeedInteraction.Response.notes.Where(x => x.type == "reblog")
                             .Select(x => new TumblrUser
                             {
                                 Username = x.blog_name,
                                 PageUrl = x.blog_url,
                                 Uuid = x.blog_uuid,
                                 IsFollowed = x.followed,
                                 TumblrsFormKey = xTumblrFormKey
                             }).ToList();
                    }
                    else if (queryInfo.QueryType == EnumUtility.GetQueryFromEnum(TumblrQuery.UserCommentedOnPost))
                    {
                        lstTumblrUsers = feedsResponseHandler.FeedInteraction.Response.notes.Where(x => x.type == "reply")
                            .Select(x => new TumblrUser
                            {
                                Username = x.blog_name,
                                PageUrl = x.blog_url,
                                IsFollowed = x.followed,
                                Uuid = x.blog_uuid,
                                TumblrsFormKey = xTumblrFormKey
                            }).ToList();
                    }
                    else if (queryInfo.QueryType ==
                             EnumUtility.GetQueryFromEnum(TumblrQuery.UserLikedCommentedReblogedThePost))
                        lstTumblrUsers = feedsResponseHandler.FeedInteraction.Response.notes.Select(x => new TumblrUser
                        {
                            Username = x.blog_name,
                            Uuid = x.blog_uuid,
                            PageUrl = x.blog_url,
                            IsFollowed = x.followed,
                            TumblrsFormKey = xTumblrFormKey
                        }).ToList();
                }
            }
            catch (Exception)
            {
            }

            return lstTumblrUsers;
        }
    }
}