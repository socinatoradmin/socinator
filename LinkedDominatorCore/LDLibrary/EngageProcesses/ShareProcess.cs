using System;
using System.Text.RegularExpressions;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.Engage;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;

namespace LinkedDominatorCore.LDLibrary.EngageProcesses
{
    public class
        ShareProcess : LDJobProcessInteracted<InteractedPosts>
    {
        private readonly IDelayService _delayService;
        private readonly ILdFunctions _ldFunctions;

        public ShareProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess, ILdFunctionFactory ldFunctionFactory,
            IDbInsertionHelper dbInsertionHelper, IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            ShareModel = processScopeModel.GetActivitySettingsAs<ShareModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _delayService = delayService;
        }

        private ShareModel ShareModel { get; }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = null;
            #region Share Process.
            try
            {
                jobProcessResult = new JobProcessResult();
                var objLinkedinPost = (LinkedinPost) scrapeResult.ResultPost;
                var postLinkResp = "";
                if (!IsBrowser)
                {
                    if (new GetDetailedUserInfo(_delayService).IsValidLinkJobProcessResult(objLinkedinPost.PostLink,
                        jobProcessResult, _ldFunctions, DominatorAccountModel, out postLinkResp))
                        return jobProcessResult;
                    Utils.RandomDelay();
                }


                #region Filters After Visiting Profile

                try
                {
                    if (LdUserFilterProcess.IsUserFilterActive(ShareModel.LDUserFilterModel))
                    {
                        var isValidUser = LdUserFilterProcess.GetFilterStatus(objLinkedinPost.ProfileUrl,
                            ShareModel.LDUserFilterModel, _ldFunctions);
                        if (!isValidUser)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                string.Format("LangKeyNotAValidUserAccordingToTheFilter".FromResourceDictionary(),
                                    objLinkedinPost.FullName));
                            jobProcessResult.IsProcessSuceessfull = false;
                            return jobProcessResult;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.AccountName, ActivityType, $"Trying to Share post {{{objLinkedinPost.PostLink}}}");
                var shareResponse = IsBrowser
                    ? _ldFunctions.Share(objLinkedinPost.PostLink, objLinkedinPost.NodeId, objLinkedinPost.ShareNodeId)
                    : ShareResponse(objLinkedinPost, postLinkResp);

                if (shareResponse == null || shareResponse.Contains("This post has already been shared."))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.AccountName, ActivityType, "This Post Is Already Has Been Shared.");
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                else if (shareResponse != null && shareResponse.Contains("urn:li:share:") ||
                   shareResponse.Contains("urn:li:ugcPost:") || shareResponse.Contains("Repost successful."))
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "Share post",
                        "[ " + objLinkedinPost.PostLink + " ]");
                    IncrementCounters();
                    DbInsertionHelper.DatabaseInsertionPost(scrapeResult, objLinkedinPost);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "share post",
                        "[ " + objLinkedinPost.PostLink + " ]", "");
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                if(ShareModel != null && ShareModel.IsEnableAdvancedUserMode && ShareModel.EnableDelayBetweenPerformingActionOnSamePost)
                    DelayBeforeNextActivity(ShareModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
                else
                    DelayBeforeNextActivity();
                // string apiToGetPendingGroupMembers = "https://www.linkedin.com/communities-api/v1/memberships/community/" + objPosts.GroupId + "?membershipStatus=PENDING";
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            #endregion
            return jobProcessResult;
        }

        private string ShareResponse(LinkedinPost objLinkedinPost, string postLinkResp)
        {
            var actionUrl = LdConstants.GetRepostAPI;
            

            #region Postdata
            string parentUrn;
            string postType;
            var rootUrn = RootAndParentUrn(objLinkedinPost, postLinkResp, out parentUrn, out postType);
            var ShareActivityID = string.IsNullOrEmpty(objLinkedinPost.ShareUrn) ?$"urn:li:{postType}:{rootUrn}": objLinkedinPost.ShareUrn;
            //var postData = $"{{\"visibleToConnectionsOnly\":false,\"externalAudienceProviders\":[],\"commentaryV2\":{{\"text\":\"\",\"attributes\":[]}},\"origin\":\"RESHARE\",\"allowedCommentersScope\":\"ALL\",\"parentUrn\":\"{ShareActivityID}\"}}";
            var postData = $"{{\"rootContentUrn\":\"{ShareActivityID}\"}}";
            #endregion

            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var oldContentType = SetHeaders(objLinkedinPost.PublicIdentifier);
            var shareResponse = _ldFunctions.Share(actionUrl, postData);
            RemoveHeaders(oldContentType);
            return shareResponse;
        }

        private string SetHeaders(string publicidentifier)
        {
            var oldContentType = HttpHelper.GetRequestParameter().ContentType;
            HttpHelper.GetRequestParameter().Accept = "application/vnd.linkedin.normalized+json+2.1";
            HttpHelper.GetRequestParameter().ContentType = "application/json; charset=UTF-8";
            if (!string.IsNullOrWhiteSpace(HttpHelper.GetRequestParameter()?.Headers?.ToString()) &&
                HttpHelper.GetRequestParameter().Headers.ToString().Contains("X-li-page-instance"))
                HttpHelper.GetRequestParameter().Headers.Remove("X-li-page-instance");
            HttpHelper.GetRequestParameter()?.Headers?.Add("X-li-page-instance",
                "urn:li:page:p_flagship3_feed_reshare_share;{postTrackingId}");
            HttpHelper.GetRequestParameter().Referer = $"https://www.linkedin.com/in/{publicidentifier}/detail/recent-activity/shares/";
            return oldContentType;
        }

        private void RemoveHeaders(string oldContentType)
        {
            HttpHelper.GetRequestParameter().Accept = null;
            HttpHelper.GetRequestParameter().ContentType = oldContentType;
            HttpHelper.GetRequestParameter().Headers.Remove("X-li-page-instance");
            HttpHelper.GetRequestParameter().Headers
                .Add("X-li-page-instance", "urn:li:page:d_flagship3_profile_view_base");
        }

        private static string RootAndParentUrn(LinkedinPost objLinkedinPost, string postPageResponse,
            out string parentUrn, out string postType)
        {
            var rootUrn = parentUrn = objLinkedinPost.Id;
            postType = "ugcPost";
            var threadSplit = Regex.Split(postPageResponse, "threadId");
            // if it contains multiple threadId means it is shared post therefore it require two urnId for share post
            // first urn is 'rootUrn' and second is 'parentUrn'

            if (threadSplit.Length == 2 &&
                string.IsNullOrEmpty(rootUrn = Utils.GetBetween(threadSplit[1], "ugcPost:", "\"")))
            {
                parentUrn = rootUrn = Utils.GetBetween(threadSplit[1], "ugcPost:", "\"");
            }
            else if (threadSplit.Length > 2)
            {
                if (string.IsNullOrEmpty(rootUrn = Utils.GetBetween(threadSplit[1], "ugcPost:", "\"")))
                {
                    parentUrn = rootUrn = Utils.GetBetween(postPageResponse, "urn:li:share:", "\"");
                    postType = "share";
                }
                else
                {
                    parentUrn = Utils.GetBetween(threadSplit[2], "ugcPost:", "\"");
                }
            }

            return rootUrn;
        }
    }
}