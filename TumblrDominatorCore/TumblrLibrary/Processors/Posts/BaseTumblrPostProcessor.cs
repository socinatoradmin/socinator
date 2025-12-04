using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrLibrary.TumblrProcesses;
namespace TumblrDominatorCore.TumblrLibrary.Processors.Posts
{
    public abstract class BaseTumblrPostProcessor : BaseTumblrProcessor
    {


        protected BaseTumblrPostProcessor(IProcessScopeModel processScopeModel,
            ITumblrJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService,
            ITumblrFunct tumblrFunct, IDbGlobalService globalService) : base(processScopeModel, jobProcess, dbAccountService, campaignService,
            tumblrFunct, globalService)
        {
        }

        public void ProcessOnUserPosts(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<TumblrPost> lstTumblrPosts, string tumblrFormKey)
        {
            try
            {
                var postLists = FilterData(queryInfo, ref jobProcessResult, lstTumblrPosts, tumblrFormKey);
                if (lstTumblrPosts.Count - postLists.Count > 0)
                    GlobusLogHelper.log.Info(Log.FilterApplied, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType, lstTumblrPosts.Count - postLists.Count);
                var RemovedLiked = postLists.RemoveAll(x => x.IsLiked && ActivityType == ActivityType.Like);
                if (RemovedLiked > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                                $"SuccessFully Skipped {RemovedLiked} Already Liked Posts.");
                var RemovedZeroNotes = postLists.RemoveAll(x => string.IsNullOrEmpty(x.NotesCount) && x.NotesCount == "0" && ActivityType == ActivityType.CommentScraper);
                if (RemovedZeroNotes > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                                $"SuccessFully Skipped {RemovedZeroNotes} Posts As Zero CommentNotes.");
                foreach (var post in postLists)
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (!CheckPostUniqueNess(jobProcessResult, post, ActivityType)) continue;
                    //     if (!ApplyCampaignLevelSettings(queryInfo, post.PostUrl)) continue;
                    SendToPerformActivity(ref jobProcessResult, post, queryInfo, tumblrFormKey);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public List<TumblrPost> FilterData(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<TumblrPost> lstTumblrPosts, string tumblrFormKey)
        {
            var postLists = lstTumblrPosts.DeepCloneObject();
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var InteractedPost = _dbAccountService.GetInteractedPosts(ActivityType);
                postLists.RemoveAll(x => InteractedPost.Any(y => y.ContentId == x.Id));
                var SkippedBlackorWhiteListedPostOwnerNames = FilterBlacklistedOrWhiteListedUsers(postLists.Select(y => y.BlogName).ToList());
                postLists.RemoveAll(z => SkippedBlackorWhiteListedPostOwnerNames.Count == 0 || !SkippedBlackorWhiteListedPostOwnerNames.Any(t => t == z.BlogName));
                //     postLists.RemoveAll(x => !x.CanReply && ActivityType == ActivityType.Comment);
                postLists = FilterdPost(postLists);
                return postLists;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return postLists;
        }
        public void SendToPerformActivity([NotNull] ref JobProcessResult jobProcessResult, TumblrPost tumblrpost,
            QueryInfo queryInfo, string tumblrFormKey)
        {
            if (jobProcessResult == null) throw new ArgumentNullException(nameof(jobProcessResult));
            jobProcessResult = JobProcess.FinalProcess(new TumblrScrapeResult
            {
                ResultPost = tumblrpost,
                TumblrFormKey = tumblrFormKey,
                QueryInfo = queryInfo
            });
        }
    }
}