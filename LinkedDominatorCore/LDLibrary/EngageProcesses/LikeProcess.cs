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
        LikeProcess : LDJobProcessInteracted<InteractedPosts>
    {
        private readonly IDelayService _delayService;
        private readonly ILdFunctions _ldFunctions;
        private readonly IDbInsertionHelper _dbInsertionHelper;

        public LikeProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper,
            ILdLogInProcess logInProcess, ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper,
            IDelayService delayService)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            LikeModel = processScopeModel.GetActivitySettingsAs<LikeModel>();
            _dbInsertionHelper = dbInsertionHelper;
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _delayService = delayService;
        }

        private LikeModel LikeModel { get; }


        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = null;
            #region Like Process.
            try
            {
                jobProcessResult = new JobProcessResult();

                var objLinkedinPost = (LinkedinPost) scrapeResult.ResultPost;
                DetailsFetcher.GetUserScraperDetailedInfo(DominatorAccountModel);

                #region Filters After Visiting Profile

                try
                {
                    //var LdUserFilterProcess = new LdUserFilterProcess();
                    if (LdUserFilterProcess.IsUserFilterActive(LikeModel.LDUserFilterModel))
                    {
                        var isValidUser = LdUserFilterProcess.GetFilterStatus(objLinkedinPost.ProfileUrl,
                            LikeModel.LDUserFilterModel, _ldFunctions);
                        if (!isValidUser)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,DominatorAccountModel.AccountBaseModel.AccountNetwork,DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                string.Format("LangKeyNotAValidUserAccordingToTheFilter".FromResourceDictionary(),objLinkedinPost.FullName));
                            jobProcessResult.IsProcessSuceessfull = false;
                            return jobProcessResult;
                        }else
                            GlobusLogHelper.log.Info(Log.CustomMessage,DominatorAccountModel.AccountBaseModel.AccountNetwork,DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"All Filter Matched Successfully,Proceeding For {ActivityType} Of {objLinkedinPost.FullName}");
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                if (!string.IsNullOrEmpty(objLinkedinPost.IsLiked) && objLinkedinPost.IsLiked == "True")
                {
                    var user = (string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.UserFullName) || DominatorAccountModel.AccountBaseModel.UserFullName == " ") ? DominatorAccountModel.UserName : DominatorAccountModel.AccountBaseModel.UserFullName;
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"{LdConstants.AlreadyLikedFeed} by {user}");
                    jobProcessResult.IsProcessSuceessfull = false;
                    return jobProcessResult;
                }
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,$"Trying to {ActivityType} {{{objLinkedinPost.PostLink}}}");
                _delayService.ThreadSleep(RandomUtilties.GetRandomNumber(5000,3000));
                var response = IsBrowser
                    ? _ldFunctions.Like(objLinkedinPost.PostLink, objLinkedinPost.NodeId)
                    : NormalProcess(objLinkedinPost, jobProcessResult);

                if (response == string.Empty)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "[ " + objLinkedinPost.PostLink + " ]");
                    IncrementCounters();
                    _dbInsertionHelper.DatabaseInsertionPost(scrapeResult, objLinkedinPost);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "[ " + objLinkedinPost.PostLink + " ]", "");
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                if(LikeModel!=null && LikeModel.IsEnableAdvancedUserMode)
                {
                    if (LikeModel.EnableDelayBetweenPerformingActionOnSamePost)
                        DelayBeforeNextActivity(LikeModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
                    else
                        DelayBeforeNextActivity();
                }else
                    DelayBeforeNextActivity();
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

        private string NormalProcess(LinkedinPost objLinkedinPost, JobProcessResult jobProcessResult)
        {
            string response = null;
            string feedPageResponse;
            if (new GetDetailedUserInfo(_delayService).IsValidLinkJobProcessResult(objLinkedinPost.PostLink,
                jobProcessResult, _ldFunctions,
                DominatorAccountModel, out feedPageResponse))
                return response;

            #region scraping Requirements to Like Feed

            var fileIdentifyingUrlPath = DetailsFetcher.GetFileIdentifyPath(feedPageResponse);
            fileIdentifyingUrlPath.ActionUrl = "https://www.linkedin.com/voyager/api/feed/reactions";

            string dynamicJsonPostData;

            #endregion
            if (!string.IsNullOrWhiteSpace(fileIdentifyingUrlPath.GroupPostId) &&
                objLinkedinPost.Id.Contains("groupPost"))
            {
                dynamicJsonPostData = "{\"threadUrn\":\"" + objLinkedinPost.Id + "\",\"reactionType\":\"LIKE\"}";
            }
            else
            {
                var ugcPostId = GetUgcPostId(feedPageResponse);
                if (!ugcPostId.Contains(objLinkedinPost.Id))
                    ugcPostId = Utilities.GetBetween(feedPageResponse, "urn:li:fs_updateV2:(urn:li:", ",");
                dynamicJsonPostData = "{\"threadUrn\":\"urn:li:" + ugcPostId + "\",\"reactionType\":\"LIKE\"}";
            }


            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            string ActivityId;
            if (objLinkedinPost.PostLink.Contains("urn:li:activity"))
            {
                ActivityId = Utils.GetBetween(objLinkedinPost.PostLink, "/update/", "?");
                ActivityId = string.IsNullOrEmpty(ActivityId) ? Utils.GetBetween(objLinkedinPost.PostLink + "**", "/update/", "**") : ActivityId;
            }else if (objLinkedinPost.PostLink.Contains("urn:li:ugcPost"))
            {
                ActivityId = Utils.GetBetween(objLinkedinPost.PostLink + "**", "/update/", "**");
            }else if (objLinkedinPost.PostLink.Contains("urn:li:groupPost"))
            {
                ActivityId = Utils.GetBetween(objLinkedinPost.PostLink + "**", "/update/", "**");
            }
            else
            {
                ActivityId = $"urn:li:ugcPost:{objLinkedinPost.Id}";
            }
            dynamicJsonPostData = "{\"reactionType\":\"LIKE\"}";
            ActivityId = Uri.EscapeDataString(ActivityId);
            response = _ldFunctions.Like(LdConstants.GetUserFeedLikeAPI(ActivityId), dynamicJsonPostData);
            if (response == null)
            {
                ActivityId =Uri.EscapeDataString($"urn:li:activity:{objLinkedinPost.Id}");
                response= _ldFunctions.Like(LdConstants.GetUserFeedLikeAPI(ActivityId), dynamicJsonPostData);
                if (response == null)
                    response = _ldFunctions.Like(LdConstants.GetUserFeedLikeAPI(Uri.EscapeDataString(objLinkedinPost.ShareUrn)),dynamicJsonPostData);
            }
            return response;
        }

        private string GetUgcPostId(string feedPageResponse)
        {
            string ugcId;
            if (string.IsNullOrWhiteSpace(ugcId = Regex
                .Match(feedPageResponse, @"urn:li:ugcPost:(\d*?)\)")?.Groups[1]?.ToString()))
                ugcId = "activity:" + Regex
                            .Match(feedPageResponse, @"urn:li:activity:(\d*?)\)")?.Groups[1];

            else
                ugcId = "ugcPost:" + ugcId;
            return ugcId;
        }
    }
}