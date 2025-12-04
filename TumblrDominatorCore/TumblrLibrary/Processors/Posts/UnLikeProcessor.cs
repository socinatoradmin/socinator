using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrLibrary.TumblrProcesses;
using TumblrDominatorCore.TumblrResponseHandler;

namespace TumblrDominatorCore.TumblrLibrary.Processors.Posts
{
    public class UnLikeProcessor : BaseTumblrPostProcessor, IQueryProcessor
    {
        private string MainUrl = "";
        SearchLikedPostResponse _searchLikedPostResponse;
        public UnLikeProcessor(IProcessScopeModel processScopeModel, ITumblrJobProcess jobProcess,
            IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, ITumblrFunct tumblrFunct, IDbGlobalService globalService) :
            base(processScopeModel, jobProcess, dbAccountService, campaignService, tumblrFunct, globalService)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var lstPostList = new List<TumblrPost>();
                if (UnLikeModel.IsChkPostLikedOutsideSoftware)
                {
                    if (!string.IsNullOrEmpty(MainUrl))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                        $"Searching for More... of Liked Post");
                    }
                    if (string.IsNullOrEmpty(MainUrl))
                        MainUrl = ConstantHelpDetails.GetLikedPostDetailsofAnyUserAPI();
                    _searchLikedPostResponse =
                        TumblrFunct.SearchLikedPost(JobProcess.DominatorAccountModel, MainUrl);
                    if (!string.IsNullOrEmpty(_searchLikedPostResponse.NextPageUrl)) MainUrl = _searchLikedPostResponse.NextPageUrl;
                    if (_searchLikedPostResponse != null && _searchLikedPostResponse.Success)
                    {
                        if (_searchLikedPostResponse.TotalLikeCount == 0 && _searchLikedPostResponse.TumblrPosts.Count == 0)
                            GlobusLogHelper.log.Info(JobProcess.DominatorAccountModel.UserName + " have " +
                                                      _searchLikedPostResponse.TotalLikeCount + " Likes for Post ");
                        if (_searchLikedPostResponse.TumblrPosts.Count > 0)
                            _searchLikedPostResponse.TumblrPosts.RemoveAll(x => _dbAccountService.GetInteractedPosts(ActivityType.Like).Any(y => y.PostUrl.Equals(x.PostUrl)));
                        if (_searchLikedPostResponse.TotalLikeCount > 0 && _searchLikedPostResponse.TumblrPosts.Count == 0)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                            JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                            "No post Found for LikedOutsideSoftwere in this Page");
                        }
                        else
                            lstPostList.AddRange(_searchLikedPostResponse.TumblrPosts);
                    }
                }
                if (UnLikeModel.IsChkCustomPostsListChecked)
                {
                    var customList = new List<TumblrPost>();
                    Regex.Split(UnLikeModel.CustomPostsList, "\r\n").ToList().ForEach(x =>
                    {
                        customList.Add(TumblrUtility.getTumblrPostFromPostUrl(x));
                    });
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                            JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                            "Getting Custom Post Details Please Wait A While...");
                    foreach (var item in customList)
                    {
                        var post = item;
                        if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                        {
                            _browser.SearchPostDetails(JobProcess.DominatorAccountModel, ref post);
                        }
                        else
                        {
                            post = TumblrFunct.PostScraper(JobProcess.DominatorAccountModel, post, string.Empty).tumblrPost;
                        }
                        lstPostList.Add(post);
                    }
                }
                if (UnLikeModel.IsChkPostLikedBySoftwareChecked)
                    _dbAccountService.GetInteractedPosts(ActivityType.Like).ForEach(
                        x =>
                        {
                            lstPostList.Add(new TumblrPost() { PostUrl = x.PostUrl, Id = x.ContentId, RebloggedRootId = x.DataRootId, OwnerUsername = x.InteractedUserName });
                        });
                #region database checking
                var skippedPostCount = lstPostList.RemoveAll(x => _dbAccountService.GetUnLikedUsers(ActivityType).Any(y => y.PostUrl.Contains(x.PostUrl)));
                #endregion
                if (skippedPostCount > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                                "Successfully Skipped " + skippedPostCount + " No of Posts as already Unliked from this Software");

                if (skippedPostCount > 0 && lstPostList.Count == 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                                "No Post Found to Process for Unlike");

                if (lstPostList.Count > 0)
                    ProcessOnUserPosts(queryInfo, ref jobProcessResult, lstPostList, "");
                else
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                                    "No Post Found To Unlike");
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(Log.CustomMessage,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    " method=>> StartUnLikeProcess",
                    JobProcess.DominatorAccountModel.UserName + " " + ex.Message);
            }
            if (string.IsNullOrEmpty(MainUrl) ||
                (!string.IsNullOrEmpty(MainUrl) && string.IsNullOrEmpty(_searchLikedPostResponse.NextPageUrl)))
            {
                GlobusLogHelper.log.Info(Log.ProcessCompleted,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, ActivityType);
                jobProcessResult.IsProcessCompleted = true;
            }
        }

    }
}