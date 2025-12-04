using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.DatabaseHandler.LdTables.Campaign;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.LdQuery;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.DetailedInfo;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Response;

namespace LinkedDominatorCore.LDLibrary.Processor.Posts
{
    public abstract class BaseLinkedinPostProcessor : BaseLinkedinProcessor, IQueryProcessor
    {
        private static readonly ConcurrentDictionary<string, object> LockObjects =
            new ConcurrentDictionary<string, object>();

        public static readonly object LockReachedMaxPostActionPerUser = new object();
        private LdDataHelper ldDataHelper = LdDataHelper.GetInstance;
        protected BaseLinkedinPostProcessor(ILdJobProcess ldJobProcess, IDbCampaignService campaignService,
            ILdFunctionFactory ldFunctionFactory, IDelayService delayService, IProcessScopeModel ProcessScopeModel)
            : base(ldJobProcess, campaignService, ldFunctionFactory, delayService, ProcessScopeModel)
        {
            ldDataHelper.ldFunctions = ldFunctionFactory.LdFunctions;
        }

        public JobProcessResult ScrapeUserPostsAndMoveToFinalProcess(JobProcessResult jobProcessResult,
            QueryInfo queryInfo, bool isCheckedFilterProfileImageCheckbox, string id,
            string publicIdentifier, List<string> lstCommentInDom, int maxActionPerPost = 0, int maxActionPerGroup = 0)
        {
            try
            {
                var automationExtension = new BrowserAutomationExtension(LdFunctions.BrowserWindow);
                var start = 0;
                var lastResultCount = 0;
                var paginationToken = string.Empty;
                if (queryInfo.QueryType == EnumUtility.GetQueryFromEnum(LDEngageQueryParameters.MyGroupsPosts))
                    return jobProcessResult;

                jobProcessResult.HasNoResult = false;
                var count = 0;
                var logCount = 0;


                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    count = ++count;

                    // post searching user profile id

                    #region MyRegion

                    var actionUrl = queryInfo.QueryType == "GroupUrl Post(S)"
                        ? $"https://www.linkedin.com/voyager/api/groups/updatesV2?count=20&groupId={id}&q=groupsFeed"
                        : $"https://www.linkedin.com/in/{publicIdentifier}/detail/recent-activity/shares/";
                    if (queryInfo.QueryType == "HashtagUrl Post(S)")
                        actionUrl = publicIdentifier;
                    if (IsBrowser)
                    {
                        if (count == 1 && !string.IsNullOrWhiteSpace(publicIdentifier))
                            automationExtension.LoadAndScroll(actionUrl,10,true,4000,true,string.Empty,true);
                    }
                    else
                    {
                        id = string.IsNullOrEmpty(id) ? ldDataHelper.GetProfileId(ldDataHelper.GetDecodedResponse(publicIdentifier,true)):id;
                        if (queryInfo.QueryType == "GroupUrl Post(S)")
                            actionUrl = count == 1
                                ? actionUrl
                                : $"https://www.linkedin.com/voyager/api/groups/updatesV2?count=20&groupId={id}&q=groupsFeed&paginationToken={paginationToken}&start={start}";
                        else if (queryInfo.QueryType == "HashtagUrl Post(S)")
                            actionUrl =$"https://www.linkedin.com/voyager/api/feed/interestUpdatesV2?count=20&q=interestFeedByUrn&sortOrder=RELEVANCE&start={start}&urn=urn%3Ali%3Ahashtag%3A{id}";
                        else
                            actionUrl = count == 1
                                ? $"{LdConstants.UserPostsApiConstantPagination(id)}"
                                : $"{LdConstants.UserPostsApiConstantPagination(id)}&paginationToken={paginationToken}&start={start}";
                    }

                    var objPostsResponseHandler = LdFunctions.SearchForLinkedinPosts(actionUrl, ActivityType,
                        DominatorAccountModel.AccountBaseModel.ProfilePictureUrl, publicIdentifier, lstCommentInDom);
                    if (objPostsResponseHandler != null && objPostsResponseHandler.PostsList.Count > 0)
                        objPostsResponseHandler.PostsList.RemoveAll(x => string.IsNullOrEmpty(x.FullName) || x.FullName.Contains("Linkedin Member"));
                    if (IsBrowser && lastResultCount == objPostsResponseHandler.PostsList.Count)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"no unique post found for {publicIdentifier}.");
                        jobProcessResult.HasNoResult = true;
                        return jobProcessResult;
                    }

                    //for browser if after scrolling still getting same result means find no unique post
                    lastResultCount = objPostsResponseHandler.PostsList.Count;

                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (objPostsResponseHandler.Success)
                    {
                        if (objPostsResponseHandler.PostsList.Count > 0)
                        {
                            if (isCheckedFilterProfileImageCheckbox && queryInfo.QueryType !=
                                EnumUtility.GetQueryFromEnum(LDEngageQueryParameters.MyConnectionsPosts))
                            {
                                if(objPostsResponseHandler.PostsList.RemoveAll(x => x.ProfilePicUrl == null) > 0)
                                    GlobusLogHelper.log.Info(Log.CustomMessage,DominatorAccountModel.AccountBaseModel.AccountNetwork,DominatorAccountModel.AccountBaseModel.UserName, ActivityType,"successfully filtered posts created by users having no profile picture.");
                            }
                            RemoveAlreadyPerformedPosts(objPostsResponseHandler);

                            // here we checking is reached max action per post
                            if (objPostsResponseHandler.PostsList.Count > 0 && ProcessLinkedinPostsFromPost(queryInfo,
                                    ref jobProcessResult, objPostsResponseHandler.PostsList, maxActionPerPost,
                                    maxActionPerGroup))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "Reached maximum actions perform per user/Groups posts.");
                                return jobProcessResult;
                            }

                            if (logCount == 0)
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "no result after filtering users having no profile picture : navigating to next page.");
                                logCount++;
                            }
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "no unique post found in this page to perform this activity : navigating to next page.");
                        }

                        #region Paginate ActionUrl
                        start += 20;
                        if (!string.IsNullOrEmpty(objPostsResponseHandler.PaginationToken))
                            paginationToken = Uri.EscapeDataString(objPostsResponseHandler.PaginationToken);

                        #endregion
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"no unique post found for {publicIdentifier}.");
                        jobProcessResult.HasNoResult = true;
                    }

                    #endregion
                }


                return jobProcessResult;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                jobProcessResult.HasNoResult = true;
                return jobProcessResult;
            }
        }


        /// <summary>
        ///     it returns is reached max action per post
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <param name="jobProcessResult"></param>
        /// <param name="lstLinkedinPost"></param>
        /// <param name="maxActionPerPost"></param>
        /// <param name="maxActionperGroup"></param>
        /// <returns></returns>
        public bool ProcessLinkedinPostsFromPost(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<LinkedinPost> lstLinkedinPost, int maxActionPerPost = 0, int maxActionperGroup = 0)
        {
            try
            {
                foreach (var post in lstLinkedinPost)
                {
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (IsReachedMaxPostActionPerUser(post.ProfileUrl, maxActionPerPost))
                        return true;
                    if (IsReachedMaxPostActionPerGroup(post.Id, maxActionperGroup))
                        return true;
                    SendToPerformActivity(ref jobProcessResult, post, queryInfo);
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        protected bool IsReachedMaxPostActionPerUser(string user, int maxCount)
        {
            if (maxCount == 0)
                return false;

            var count = 0;
            lock (LockReachedMaxPostActionPerUser)
            {
                if (DbCampaignService == null)
                    count = DbCampaignService.GetInteractedPosts(ActivityTypeString, user).ToList().Count;
                else
                    count = DbCampaignService.GetInteractedPosts(ActivityTypeString, user).ToList().Count;
                return maxCount <= count;
            }
        }

        protected bool IsReachedMaxPostActionPerGroup(string user, int maxCount)
        {
            try
            {
                var GroupId = string.Empty;
                if (user.Contains("groupPost"))
                    GroupId = Utils.GetBetween(user, "groupPost:", "-");
                else
                    GroupId = user;
                var groupUrl = $"https://www.linkedin.com/groups/{GroupId}/";
                if (maxCount == 0)
                    return false;

                var count = 0;
                lock (LockReachedMaxPostActionPerUser)
                {
                    if (DbCampaignService == null)
                        count = DbCampaignService.GetInteractedGroups(ActivityTypeString, groupUrl).ToList().Count;
                    else
                        count = DbCampaignService.GetInteractedGroups(ActivityTypeString, groupUrl).ToList().Count;
                    return maxCount <= count;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SendToPerformActivity(ref JobProcessResult jobProcessResult, LinkedinPost linkedinPost,
            QueryInfo queryInfo)
        {
            try
            {
                if (!CheckPostUniqueNess(jobProcessResult, linkedinPost))
                    return;

                if (!ApplyCampaignLevelSettings(queryInfo, linkedinPost.PostLink, _campaignDetails))
                    return;

                jobProcessResult = LdJobProcess.FinalProcess(new ScrapeResultNew
                {
                    ResultPost = linkedinPost,
                    QueryInfo = queryInfo
                });
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void RemoveAlreadyPerformedPosts(PostsResponseHandler objPostsResponseHandler)
        {
            try
            {
                var listPost = DbAccountService.GetInteractedPosts(ActivityTypeString).Select(x => x.PostLink).ToList();
                objPostsResponseHandler.PostsList.RemoveAll(x => listPost.Contains(x.PostLink));
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        public JobProcessResult ScrapeCompanyPostsAndMoveToFinalProcess(JobProcessResult jobProcessResult,
            QueryInfo queryInfo, CompanyScraperDetailedInfo linkedinCompany, List<string> lstCommentInDom)
        {
            try
            {
                var automationExtension = new BrowserAutomationExtension(LdFunctions.BrowserWindow);
                var start = 0;
                var paginationToken = string.Empty;
                if (queryInfo.QueryType == EnumUtility.GetQueryFromEnum(LDEngageQueryParameters.MyGroupsPosts))
                    return jobProcessResult;

                jobProcessResult.HasNoResult = false;
                var count = 0;
                int loadcount = 6000;
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    count = ++count;                    
                    //$"https://www.linkedin.com/voyager/api/feed/updates?companyIdOrUniversalName={linkedinCompany.CompanyId}&count=40&moduleKey=ORGANIZATION_MEMBER_FEED_DESKTOP&numComments=0&numLikes=0&q=companyRelevanceFeed&start=0";
                    var actionUrl = IsBrowser ? queryInfo.QueryValue.EndsWith("/")?$"{queryInfo.QueryValue}posts": $"{queryInfo.QueryValue}/posts" : $"https://www.linkedin.com/voyager/api/organization/updatesV2?companyIdOrUniversalName={linkedinCompany.CompanyId}&count=40&moduleKey=ORGANIZATION_MEMBER_FEED_DESKTOP&numComments=0&numLikes=0&q=companyRelevanceFeed&start={start}"; 
                        

                    if (IsBrowser)
                        automationExtension.LoadAndScroll(actionUrl, 10, true, loadcount,true,null,true);
                    else
                        actionUrl = _apiAssist.CompanyPostsApiConstantPagination(linkedinCompany.CompanyId,
                            paginationToken, start);

                    var objPostsResponseHandler = LdFunctions.SearchForLinkedinPosts(actionUrl, ActivityType,
                        DominatorAccountModel.AccountBaseModel.ProfilePictureUrl, "", lstCommentInDom);
                    Utils.RandomDelay();
                    LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (objPostsResponseHandler.Success)
                    {
                        if (objPostsResponseHandler.PostsList.Count > 0)
                        {
                            if (ActivityType == ActivityType.Comment || ActivityType == ActivityType.Share ||
                                ActivityType == ActivityType.Like)
                                RemoveAlreadyPerformedPosts(objPostsResponseHandler);

                            foreach (var linkedinPost in objPostsResponseHandler.PostsList)
                            {
                                linkedinPost.FullName = linkedinCompany.CompanyName;
                                linkedinPost.ProfileUrl = linkedinCompany.CompanyUrl;
                            }

                            if (objPostsResponseHandler.PostsList.Count > 0)
                                ProcessLinkedinPostsFromPost(queryInfo, ref jobProcessResult,
                                    objPostsResponseHandler.PostsList);
                            else
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    "no result after filtering users having no profile picture : navigating to next page.");
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "no unique post found in this page to perform this activity : navigating to next page.");
                        }

                       start += 40;
                       loadcount += 6000;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            $"no unique post found for {linkedinCompany.CompanyName}.");
                        jobProcessResult.HasNoResult = true;
                    }
                }

                return jobProcessResult;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                jobProcessResult.HasNoResult = true;
                ex.DebugLog();
                return new JobProcessResult();
            }
        }


        protected bool CheckPostUniqueNess(JobProcessResult jobProcessResult, LinkedinPost post)
        {
            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleConfiguration = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
            if (moduleConfiguration != null && moduleConfiguration.IsTemplateMadeByCampaignMode)
            {
                var instance = InstanceProvider.GetInstance<ICampaignInteractionDetails>();

                if (LdJobProcess.ModuleSetting.IschkUniquePostForCampaign)
                    try
                    {
                        instance.AddInteractedData(SocialNetworks.LinkedIn, $"{_campaignDetails.CampaignId}.post",
                            post.PostLink);
                    }
                    catch (Exception)
                    {
                        jobProcessResult.IsProcessSuceessfull = false;
                        return false;
                    }

                if (LdJobProcess.ModuleSetting.IschkUniqueUserForCampaign)
                    try
                    {
                        instance.AddInteractedData(SocialNetworks.LinkedIn, _campaignDetails.CampaignId,
                            post.PublicIdentifier);
                    }
                    catch (Exception)
                    {
                        jobProcessResult.IsProcessSuceessfull = false;
                        return false;
                    }
            }

            if (LdJobProcess.ModuleSetting.IschkUniqueUserForAccount)
                try
                {
                    if (DbAccountService.GetInteractedPosts(ActivityType.ToString())
                        .Where(x => x.PostOwnerProfileUrl == post.ProfileUrl).Any())
                        return false;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            return true;
        }

        protected bool ApplyCampaignLevelSettings(QueryInfo queryInfo, string postPermalink,
            [NotNull] CampaignDetails campaignDetails)
        {
            if (campaignDetails == null)
                return true;

            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            campaignDetails = campaignFileManager.GetCampaignById(_campaignDetails.CampaignId);

            if (campaignDetails != null)
                try
                {
                    LdJobProcess.AddedToDb = false;

                    #region Action From Random Percentage Of Accounts

                    if (LdJobProcess.ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts)
                    {
                        var lockObject = LockObjects.GetOrAdd("Lock1" + postPermalink, new object());
                        lock (lockObject)
                        {
                            LdJobProcess.LdCancellationToken.ThrowIfCancellationRequested();
                            var dbOperation = new DbOperations(campaignDetails.CampaignId, SocialNetworks.LinkedIn,
                                ConstantVariable.GetCampaignDb);
                            try
                            {
                                decimal count = campaignDetails.SelectedAccountList.Count;
                                var randomMaxAccountToPerform = (int) Math.Round(
                                    count * LdJobProcess.ModuleSetting.PerformActionFromRandomPercentage.GetRandom() /
                                    100);

                                var numberOfAccountsAlreadyPerformedAction = DbCampaignService
                                    .GetInteractedPosts(ActivityType.ToString()).Where(x => x.PostLink == postPermalink)
                                    .ToList();

                                if (randomMaxAccountToPerform <= numberOfAccountsAlreadyPerformedAction.Count)
                                    return false;
                                LdJobProcess.AddedToDb = true;
                            }
                            catch (OperationCanceledException)
                            {
                                throw new OperationCanceledException(@"Cancellation Requested !");
                            }
                            catch (AggregateException ae)
                            {
                                ae.HandleOperationCancellation();
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }
                    }

                    #endregion

                    #region Delay Between action On SamePost

                    if (LdJobProcess.ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost)
                    {
                        var activityType = ActivityType.ToString();
                        var lockObject = LockObjects.GetOrAdd("Lock2" + postPermalink, new object());
                        lock (lockObject)
                        {
                            LdJobProcess.LdCancellationToken.ThrowIfCancellationRequested();
                            var dbOperation = new DbOperations(campaignDetails.CampaignId, SocialNetworks.LinkedIn,
                                ConstantVariable.GetCampaignDb);
                            try
                            {
                                var recentlyPerformedActions = DbCampaignService.GetInteractedPosts(ActivityType.ToString())
                                    .Where(x => x.PostLink == postPermalink && x.Status == "Success" ||
                                                x.Status == "Working").OrderByDescending(x => x.InteractionTimeStamp)
                                    .Select(x => x.InteractionTimeStamp).Take(1).ToList();
                                if (recentlyPerformedActions.Count > 0)
                                {
                                    var recentlyPerformedTime = recentlyPerformedActions[0];
                                    var delay = LdJobProcess.ModuleSetting.DelayBetweenPerformingActionOnSamePost
                                        .GetRandom();
                                    var time = DateTimeUtilities.GetEpochTime();
                                    var time2 = recentlyPerformedTime + delay;
                                    if (time < time2)
                                        Thread.Sleep(
                                            (time2 - time) * 1000); // Thread.Sleep((time2 - time) * 1000);
                                }

                                if (!LdJobProcess.AddedToDb)
                                {
                                    //AddWorkingActivityValueToDb(queryInfo, postPermalink, dbOperation);
                                }
                                else
                                {
                                    var interactedPost =
                                        dbOperation
                                            .GetSingle<InteractedPosts>(
                                                x => x.PostLink == postPermalink && x.ActivityType == activityType &&
                                                     x.AccountEmail ==
                                                     DominatorAccountModel.AccountBaseModel.UserName &&
                                                     (x.Status == "Pending" || x.Status == "Working"));
                                    interactedPost.InteractionTimeStamp = DateTimeUtilities.GetEpochTime();
                                    interactedPost.Status = "Working";
                                    dbOperation.Update(interactedPost);
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                throw new OperationCanceledException(@"Cancellation Requested !");
                            }
                            catch (AggregateException ae)
                            {
                                ae.HandleOperationCancellation();
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }
                    }

                    #endregion
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException(@"Cancellation Requested !");
                }
                catch (AggregateException ae)
                {
                    ae.HandleOperationCancellation();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            return true;
        }

        protected void AddPendingActivityValueToDb(QueryInfo queryInfo, string postPermalink, DbOperations dbOperation)
        {
            var activityType = ActivityType.ToString();
            dbOperation.Add(new InteractedPosts
            {
                ActivityType = activityType,
                QueryType = queryInfo.QueryType,
                QueryValue = queryInfo.QueryValue,
                AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                PostLink = postPermalink,
                Status = "Pending"
            });
        }

        protected void AddWorkingActivityValueToDb(QueryInfo queryInfo, string postPermalink, DbOperations dbOperation)
        {
            var activitytype = ActivityType.ToString();
            dbOperation.Add(new InteractedPosts
            {
                ActivityType = activitytype,
                QueryType = queryInfo.QueryType,
                QueryValue = queryInfo.QueryValue,
                AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                PostLink = postPermalink,
                Status = "Working"
            });
        }


        public string ConstractedApiToGetKeywordPosts(string keyword, ActivityType activityType)
        {
            try
            {
                var api = "";
                if (string.IsNullOrEmpty(keyword))
                    return api;

                if (keyword.Contains("”") || keyword.Contains("“"))
                    keyword = keyword.Replace("”", "\"").Replace("“", "\"");

                keyword = Uri.EscapeDataString(keyword);
                api = $"https://www.linkedin.com/voyager/api/search/dash/clusters?decorationId=com.linkedin.voyager.dash.deco.search.SearchClusterCollection-124&origin=GLOBAL_SEARCH_HEADER&q=all&query=(keywords:{keyword},flagshipSearchIntent:SEARCH_SRP,queryParameters:(resultType:List(CONTENT)),includeFiltersInResponse:false)";

                return api;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
            
        }
    }
}