using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace FaceDominatorCore.FDLibrary.FdProcessors.CommentProcessor
{

    public class BaseFbCommentLikerProcessor : BaseFbProcessor
    {
        private readonly IDbAccountService _dbAccountService;

        private ICommentLikerModule _activitySettingsAs;

        //        private readonly object _lockReachedMaxTweetActionPerUser = new object();

        private Dictionary<string, string> DictOwnPage { get; set; }

        private readonly IAccountScopeFactory _accountScopeFactory;

        protected BaseFbCommentLikerProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _dbAccountService = dbAccountService;
            SetActivity(processScopeModel);
        }

        public void SetActivity(IProcessScopeModel processScopeModel)
        {

            if (_ActivityType != ActivityType.CommentScraper)
            {
                _activitySettingsAs = ProcessScopeModel.GetActivitySettingsAs<CommentLikerModule>();

                if (_ActivityType == ActivityType.ReplyToComment)
                    _activitySettingsAs = ProcessScopeModel.GetActivitySettingsAs<ReplyToCommentModel>();


            }
        }

        public void ProcessDataOfComments(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<FdPostCommentDetails> lstPostCommentDetails, FbEntityTypes fbEntityTypes)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            List<FdPostCommentDetails> filteredData;

            bool foundNewComment = false;

            if (IsCommentFilterActive())
            {
                filteredData = FilterCommentApply(lstPostCommentDetails.ToList<IComments>(), JobProcess.ActivityType).Select(c => (FdPostCommentDetails)c).ToList();
                GlobusLogHelper.log.Info(Log.FilterApplied, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, _ActivityType, filteredData.Count);
            }
            else
                filteredData = lstPostCommentDetails;

            foreach (var commentDetails in filteredData)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if ((!commentDetails.CanCommentByUser || (_activitySettingsAs != null && _activitySettingsAs.IsSkipCommentsOfSameUser &&
                        commentDetails.CommenterID == AccountModel.AccountBaseModel.UserId)) && _ActivityType == ActivityType.ReplyToComment)
                        continue;
                    if (AlreadyInteractedComments(commentDetails))
                        continue;

                    if ((fbEntityTypes == FbEntityTypes.Page || fbEntityTypes == FbEntityTypes.Friend) && _ActivityType != ActivityType.CommentScraper)
                        FilterAndStartFinalProcessForEachPageCommentLike(queryInfo, ref jobProcessResult,
                                   commentDetails);
                    else
                    {
                        FilterAndStartFinalProcessForEachComment(queryInfo, out jobProcessResult,
                                   commentDetails);
                        foundNewComment = jobProcessResult.IsProcessSuceessfull;

                    }

                    if (JobProcess.IsStopped())
                        break;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            if (foundNewComment && _ActivityType == ActivityType.CommentScraper)
                JobProcess.DelayBeforeNextActivity();
        }

        public bool CheckPagePostConditions(ref JobProcessResult jobProcessResult, FacebookPostDetails objFacebookPostDetails)
        {

            if (_ActivityType != ActivityType.CommentScraper)
            {
                //ICommentLikerModule activitySettingsAs = ProcessScopeModel.GetActivitySettingsAs<CommentLikerModule>();
                ////var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                ////ICommentLikerModule postCommentorModel = JsonConvert.DeserializeObject<CommentLikerModule>(
                ////    templatesFileManager.Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);


                //if (ActivityType == ActivityType.ReplyToComment)
                //    activitySettingsAs = ProcessScopeModel.GetActivitySettingsAs<ReplyToCommentModel>();

                ////JsonConvert.DeserializeObject<ReplyToCommentModel>(templatesFileManager.Get()
                ////.FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);

                if (!_activitySettingsAs.IsActionasOwnAccountChecked && _activitySettingsAs.IsActionasPageChecked && objFacebookPostDetails.EntityType != FbEntityTypes.Page && objFacebookPostDetails.EntityType != FbEntityTypes.Friend)
                {
                    jobProcessResult = new JobProcessResult { IsProcessSuceessfull = false, HasNoResult = true };
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,

                        AccountModel.AccountBaseModel.UserName, _ActivityType,
                        string.Format("LangKeyCannotReplyToComment".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{objFacebookPostDetails.Id}"));

                    return false;
                }
            }

            return true;
        }

        private void FilterAndStartFinalProcessForEachPageCommentLike(QueryInfo queryInfo, ref JobProcessResult jobProcessResult, FdPostCommentDetails objFdPostCommentDetails)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            try
            {
                //var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                //ICommentLikerModule postCommentorModel = JsonConvert.DeserializeObject<CommentLikerModule>(templatesFileManager.Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);

                //if (ActivityType == ActivityType.ReplyToComment)
                //    postCommentorModel = JsonConvert.DeserializeObject<ReplyToCommentModel>(templatesFileManager.Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);

                if (_activitySettingsAs.IsActionasOwnAccountChecked)
                {
                    FilterAndStartFinalProcessForEachComment(queryInfo, out jobProcessResult,
                                   objFdPostCommentDetails);

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
                if (_activitySettingsAs.IsActionasPageChecked)
                {
                    DictOwnPage = DictOwnPage ?? new Dictionary<string, string>();
                    foreach (var pageUrl in _activitySettingsAs.ListOwnPageUrl)
                    {
                        var objFanpageDetails = new FanpageDetails();

                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        var pageId = DictOwnPage.ContainsKey(pageUrl) ? DictOwnPage[pageUrl] : string.Empty;

                        if (string.IsNullOrEmpty(pageId))
                        {
                            objFanpageDetails = ObjFdRequestLibrary.GetPageDetailsFromUrl(AccountModel, pageUrl);

                            pageId = objFanpageDetails.FanPageID;

                            if (!string.IsNullOrEmpty(pageId))
                                DictOwnPage.Add(pageUrl, pageId);
                        }
                        else
                            objFanpageDetails.FanPageID = pageId;
                        if (_dbAccountService.Any<OwnPages>(y => y.PageId == pageId))
                        {
                            objFanpageDetails.FanPageName = _dbAccountService.GetSingleColumn<OwnPages>(x => x.PageName, y => y.PageId == pageId).FirstOrDefault();

                            StartFinalProcessForEachPageCommentLike(queryInfo, out jobProcessResult,
                                   objFdPostCommentDetails, objFanpageDetails);

                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }

                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Requested Cancelled !");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                jobProcessResult = new JobProcessResult();
            }
        }

        private void StartFinalProcessForEachPageCommentLike(QueryInfo queryInfo, out JobProcessResult jobProcessResult,
            FdPostCommentDetails objFdPostCommentDetails, FanpageDetails pageDetails)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew
            {
                ResultPage = pageDetails,
                ResultComment = objFdPostCommentDetails,
                QueryInfo = queryInfo
            });
        }

        public void FilterAndStartFinalProcessForEachPost(QueryInfo queryInfo, JobProcessResult jobProcessResult
            , List<FacebookPostDetails> lstPostId, FbEntityType entityType = FbEntityType.Fanpage)
        {
            try
            {
                lstPostId = FilterPostWithXDaysOld(lstPostId);
                foreach (var post in lstPostId)
                {
                    var listComments = new List<FdPostCommentDetails>();

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    try
                    {
                        if (post.Id.Contains("about"))
                            continue;
                        IResponseHandler PostCommentorResponseHandler = null;

                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, _ActivityType,
                            FdFunctions.FdFunctions.IsIntegerOnly(post.Id)
                                ? string.Format("LangKeyGettingCommentsForPostUrl".FromResourceDictionary(), post.Id.Contains($"{FdConstants.FbHomeUrl}") ? post.Id : $"{FdConstants.FbHomeUrl}{post.Id}")
                                : string.Format("LangKeyGettingCommentsForPostUrl".FromResourceDictionary(), $"{post.Id}"));

                        jobProcessResult.HasNoResult = false;

                        if (queryInfo.QueryTypeEnum == "PostUrl" && AccountModel.IsRunProcessThroughBrowser)
                            Browsermanager.GetCommentersFromPost(AccountModel, BrowserReactionType.Comment, post.PostUrl);

                        FacebookPostDetails postDetails = post;

                        while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            try
                            {

                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                                if (AccountModel.IsRunProcessThroughBrowser && (postDetails == null ||
                                    !FdFunctions.FdFunctions.IsIntegerOnly(postDetails?.Id)))
                                {
                                    var userSpecificWindow = _accountScopeFactory[$"{AccountModel.AccountId}{post.Id}"]
                                    .Resolve<IFdBrowserManager>();
                                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                    var postDetailsResponseHandler = userSpecificWindow.GetFullPostDetails(AccountModel, post);
                                    userSpecificWindow.CloseBrowser(AccountModel);
                                    postDetails = postDetailsResponseHandler.ObjFdScraperResponseParameters.PostDetails;
                                    if (postDetails.Id.Contains("about"))
                                        break;
                                }

                                //In browser for custom Url diffrent and for Other Diffrent
                                if (AccountModel.IsRunProcessThroughBrowser)
                                {
                                    PostCommentorResponseHandler = //queryInfo.QueryTypeEnum == "PostUrl" || queryInfo.QueryTypeEnum == "PagePostComments" ?
                                          Browsermanager.ScrollWindowAndGetDataForCommentsForSinglePost(AccountModel, postDetails, entityType, 2, listComments, 0);
                                    //: Browsermanager.ScrollWindowAndGetDataForComments(AccountModel, post, entityType, 8, listComments, 0);
                                    //      PostCommentorResponseHandler.ObjFdScraperResponseParameters.CommentList.RemoveAll(x => x.CommenterID == JobProcess.DominatorAccountModel.AccountBaseModel.UserId);
                                    listComments.AddRange(PostCommentorResponseHandler.ObjFdScraperResponseParameters.CommentList);
                                }
                                else
                                    PostCommentorResponseHandler = ObjFdRequestLibrary.GetPostCommentor(
                                        JobProcess.DominatorAccountModel, FdFunctions.FdFunctions.IsIntegerOnly(post.Id)
                                            ? $"{FdConstants.FbHomeUrl}{post.Id}" : $"{post.Id}", PostCommentorResponseHandler, JobProcess.JobCancellationTokenSource.Token);

                                postDetails = postDetails == null ?
                                   postDetails : PostCommentorResponseHandler.ObjFdScraperResponseParameters.PostDetails;

                                // ObjFdRequestLibrary.GetPostDetails(AccountModel, postDetails);

                                if (!CheckPagePostConditions(ref jobProcessResult, post))
                                    break;

                                if (PostCommentorResponseHandler.Status)
                                {
                                    List<FdPostCommentDetails> lstFdCommentDetails = PostCommentorResponseHandler.ObjFdScraperResponseParameters.CommentList;

                                    if (_ActivityType == ActivityType.ReplyToComment && !lstFdCommentDetails.Any(x => x.CanCommentByUser))
                                    {
                                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                        AccountModel.AccountBaseModel.UserName, _ActivityType,
                                        string.Format("LangKeyCannotReplyToCommentPrivateUser".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{postDetails.Id}"));
                                        break;
                                    }

                                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                                    GlobusLogHelper.log.Info(
                                        Log.CustomMessage,
                                        AccountModel.AccountBaseModel.AccountNetwork,
                                        AccountModel.AccountBaseModel.UserName,
                                        _ActivityType,
                                        string.Format("LangKeyNoCommentsFound".FromResourceDictionary(),
                                            lstFdCommentDetails.Count, postDetails.PostUrl, ""));

                                    if (_ActivityType == ActivityType.CommentRepliesScraper)
                                    {
                                       
                                        //var postURL = post.PostUrl.Replace("www", "m");
                                        //var postUrl = FdConstants.FbHomeUrlMobile + post.PostUrl;
                                        Browsermanager.LoadPageSource(AccountModel, post.PostUrl);
                                        ProcessDataOfCommentReplies(queryInfo, ref jobProcessResult, lstFdCommentDetails);
                                    }
                                    else
                                        ProcessDataOfComments(queryInfo, ref jobProcessResult, lstFdCommentDetails, PostCommentorResponseHandler.ObjFdScraperResponseParameters.PostDetails.EntityType);

                                    //ProcessDataOfComments(queryInfo, ref jobProcessResult, lstFdCommentDetails, PostCommentorResponseHandler.ObjFdScraperResponseParameters.PostDetails.EntityType);
                                    jobProcessResult.maxId = PostCommentorResponseHandler.ObjFdScraperResponseParameters.Offset.ToString();
                                    if (!PostCommentorResponseHandler.HasMoreResults)
                                    {
                                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                        jobProcessResult.HasNoResult = true;
                                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyNoMoreCommentsForPostUrl".FromResourceDictionary(), postDetails.PostUrl));
                                    }
                                }
                                else
                                {
                                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                    jobProcessResult.HasNoResult = true;
                                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyNoMoreCommentsForPostUrl".FromResourceDictionary(), postDetails.PostUrl));
                                    jobProcessResult.maxId = null;
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                throw;
                            }
                            catch (Exception ex)
                            {
                                jobProcessResult.HasNoResult = true;
                                ex.DebugLog();
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private List<FacebookPostDetails> FilterPostWithXDaysOld(List<FacebookPostDetails> lstPostId)
        {
            var posts = lstPostId?.ToList();
            try
            {
                if (IsCommentFilterActive() && JobProcess.ModuleSetting.CommentFilterModel.IsCommentDateBetweenChecked)
                {
                    lstPostId.ForEach(p =>
                    {
                        if (p != null && p.PostedDateTime != null && p.PostedDateTime != new DateTime())
                        {
                            var days = (DateTime.Now - p.PostedDateTime).Days;
                            if (!JobProcess.ModuleSetting.CommentFilterModel.CommentedBeforeDays.InRange(days))
                                posts.RemoveAll(y => y.PostedDateTime == p.PostedDateTime);
                        }
                    });
                    var filteredCount = lstPostId.Count - posts.Count;
                    if (filteredCount > 0)
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName,
                            _ActivityType, $"Filtered {filteredCount} Posts Which Are Not In Range Of Comment Filter.");
                }
            }
            catch { }
            return posts;
        }

        public void ProcessDataOfCommentReplies(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
            List<FdPostCommentDetails> lstPostCommentDetails)
        {
            foreach (var commentDetails in lstPostCommentDetails)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (AlreadyInteractedComments(commentDetails)|| string.IsNullOrEmpty(commentDetails.CommentId)||commentDetails.ReplyCount==0)
                        continue;

                    var responseHandler = AccountModel.IsRunProcessThroughBrowser
                        ? Browsermanager.ScrollWindowGetRepliesForComment(AccountModel, commentDetails, FbEntityType.Comments, 3)
                        : ObjFdRequestLibrary.GetCommentReplies(AccountModel, commentDetails);

                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyNoCommentRepliesFound".FromResourceDictionary(),
                        responseHandler.ObjFdScraperResponseParameters?.CommentRepliesList?.Count, FdConstants.FbHomeUrl, commentDetails.CommentId));
                    if (responseHandler.ObjFdScraperResponseParameters.CommentRepliesList.Count > 0)
                    {
                        List<FdPostCommentRepliesDetails> filteredData;

                        if (IsCommentFilterActive())
                        {
                            filteredData = FilterCommentApply(responseHandler.ObjFdScraperResponseParameters.CommentRepliesList.ToList<IComments>(), JobProcess.ActivityType).Select(x => (FdPostCommentRepliesDetails)x).ToList();
                            GlobusLogHelper.log.Info(Log.FilterApplied, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork, JobProcess.DominatorAccountModel.AccountBaseModel.UserName, _ActivityType, filteredData.Count);
                        }
                        else
                            filteredData = responseHandler.ObjFdScraperResponseParameters.CommentRepliesList;


                        foreach (var commentReply in filteredData)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew
                            {
                                ResultComment = commentReply,
                                QueryInfo = queryInfo
                            });
                        }
                    }
                    if (JobProcess.IsStopped())
                        break;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        //public void FilterAndStartFinalProcessForCommentRepliesEachPost(QueryInfo queryInfo, JobProcessResult jobProcessResult, List<FacebookPostDetails> lstPostId)
        //{
        //    try
        //    {

        //        foreach (var post in lstPostId)
        //        {
        //            var listComments = new List<FdPostCommentDetails>();

        //            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

        //            //System.Threading.Thread.Sleep(60000);

        //            try
        //            {
        //                IResponseHandler PostCommentorResponseHandler = null;

        //                GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
        //                    AccountModel.AccountBaseModel.UserName, _ActivityType,
        //                    FdFunctions.FdFunctions.IsIntegerOnly(post.Id)
        //                        ? string.Format("LangKeyGettingCommentsForPostUrl".FromResourceDictionary(), post.Id.Contains($"{FdConstants.FbHomeUrl}") ? post.Id : $"{FdConstants.FbHomeUrl}{post.Id}")
        //                        : string.Format("LangKeyGettingCommentsForPostUrl".FromResourceDictionary(), $"{post.Id}"));

        //                jobProcessResult.HasNoResult = false;

        //                if (queryInfo.QueryType == "Post Url" && AccountModel.IsRunProcessThroughBrowser)
        //                    Browsermanager.GetCommentersFromPost(AccountModel, BrowserReactionType.Comment, post.Id);

        //                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
        //                {
        //                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

        //                    try
        //                    {
        //                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

        //                        //In browser for custom Url diffrent and for Other Diffrent
        //                        if (AccountModel.IsRunProcessThroughBrowser)
        //                        {
        //                            PostCommentorResponseHandler = queryInfo.QueryType == "Post Url"
        //                                ? Browsermanager.ScrollWindowAndGetDataForCommentsForSinglePost(AccountModel, post, FDEnums.FbEntityType.PostCommentor, 50, listComments, 0)
        //                                : Browsermanager.ScrollWindowAndGetDataForComments(AccountModel, post, FDEnums.FbEntityType.PostCommentor, 50, 0);

        //                            listComments.AddRange(PostCommentorResponseHandler.ObjFdScraperResponseParameters.CommentList);
        //                        }
        //                        else
        //                            PostCommentorResponseHandler = ObjFdRequestLibrary.GetPostCommentor(
        //                                JobProcess.DominatorAccountModel, FdFunctions.FdFunctions.IsIntegerOnly(post.Id)
        //                                    ? $"{FdConstants.FbHomeUrl}{post.Id}" : $"{post.Id}", PostCommentorResponseHandler);

        //                        var postDetails = PostCommentorResponseHandler.ObjFdScraperResponseParameters.PostDetails;

        //                        // ObjFdRequestLibrary.GetPostDetails(AccountModel, postDetails);

        //                        if (!CheckPagePostConditions(ref jobProcessResult, post))
        //                            break;

        //                        if (PostCommentorResponseHandler.Status)
        //                        {
        //                            List<FdPostCommentDetails> lstFdCommentDetails = PostCommentorResponseHandler.ObjFdScraperResponseParameters.CommentList;

        //                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

        //                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyNoCommentsFound".FromResourceDictionary(), lstFdCommentDetails.Count, FdConstants.FbHomeUrl, postDetails.Id));

        //                            if (_ActivityType == ActivityType.CommentRepliesScraper)
        //                                ProcessDataOfCommentReplies(queryInfo, ref jobProcessResult, lstFdCommentDetails);
        //                            else
        //                                ProcessDataOfComments(queryInfo, ref jobProcessResult, lstFdCommentDetails, PostCommentorResponseHandler.ObjFdScraperResponseParameters.PostDetails.EntityType);

        //                            jobProcessResult.maxId = PostCommentorResponseHandler.ObjFdScraperResponseParameters.Offset.ToString();
        //                            if (!PostCommentorResponseHandler.HasMoreResults)
        //                            {
        //                                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
        //                                jobProcessResult.HasNoResult = true;
        //                                GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyNoCommentsForPostUrl".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{postDetails.Id}")); jobProcessResult.HasNoResult = true;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
        //                            jobProcessResult.HasNoResult = true;
        //                            GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyNoMoreCommentsForPostUrl".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{postDetails.Id}")); jobProcessResult.HasNoResult = true;
        //                            jobProcessResult.maxId = null;
        //                        }
        //                    }
        //                    catch (OperationCanceledException)
        //                    {
        //                        throw;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        jobProcessResult.HasNoResult = true;
        //                        ex.DebugLog();
        //                    }
        //                }

        //            }
        //            catch (OperationCanceledException)
        //            {
        //                throw;
        //            }
        //            catch (Exception ex)
        //            {
        //                ex.DebugLog();
        //            }

        //        }
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }

        //}

        public List<IComments> FilterCommentApply(List<IComments> lstPostCommentDetails, ActivityType activityType)
        {
            List<IComments> filteredIds = lstPostCommentDetails.ToList();
            if (JobProcess.ModuleSetting.CommentFilterModel.IsCommentTextFilter && activityType == ActivityType.CommentRepliesScraper)
            {
                List<FdPostCommentRepliesDetails> filteredIdsx = lstPostCommentDetails.Select(x => (FdPostCommentRepliesDetails)x).ToList();
                filteredIdsx.RemoveAll(x => JobProcess.ModuleSetting.CommentFilterModel.ListFilterText.Any
                        (y => x.ReplyCommentText.ToLower().Contains(y.ToLower())));
                filteredIds = filteredIdsx.ToList<IComments>();
            }
            else
            {
                filteredIds.RemoveAll(x => JobProcess.ModuleSetting.CommentFilterModel.ListFilterText.Any
                        (y => x.CommentText.ToLower().Contains(y.ToLower())));
            }

            if (JobProcess.ModuleSetting.CommentFilterModel.IsCommentDateBetweenChecked)
            {
                lstPostCommentDetails.ForEach((x) =>
                {
                    DateTime interactionDate;

                    if (DateTime.TryParse(x.CommentTimeWithDate, out interactionDate))
                    {
                        var days = (DateTime.Now - interactionDate).Days;
                        if (!JobProcess.ModuleSetting.CommentFilterModel.CommentedBeforeDays.InRange(days))
                            filteredIds.RemoveAll(y => y.CommentTimeWithDate == x.CommentTimeWithDate);
                    }
                });
            }

            return filteredIds;
        }


        public bool IsCommentFilterActive()
            => JobProcess.ModuleSetting.CommentFilterModel.IsCommentDateBetweenChecked ||
                   JobProcess.ModuleSetting.CommentFilterModel.IsCommentTextFilter;

        public void FilterAndStartFinalProcessForEachComment
           (QueryInfo queryInfo, out JobProcessResult jobProcessResult, FdPostCommentDetails objFdPostCommentDetails)
        {
            //if (_ActivityType != ActivityType.CommentScraper)
            //{

            //    ////var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            //    ////ICommentLikerModule postCommentorModel = JsonConvert.DeserializeObject<CommentLikerModule>(
            //    ////    templatesFileManager.Get().FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);

            //    //ICommentLikerModule activitySettingsAs = ProcessScopeModel.GetActivitySettingsAs<CommentLikerModule>();

            //    //if (ActivityType == ActivityType.ReplyToComment)
            //    //    activitySettingsAs = ProcessScopeModel.GetActivitySettingsAs<ReplyToCommentModel>();

            //    ////postCommentorModel = JsonConvert.DeserializeObject<ReplyToCommentModel>(templatesFileManager.Get()
            //    ////    .FirstOrDefault(x => x.Id == JobProcess.TemplateId)?.ActivitySettings);

            //    if (!_activitySettingsAs.IsActionasOwnAccountChecked && _activitySettingsAs.IsActionasPageChecked)
            //    {
            //        jobProcessResult = new JobProcessResult { IsProcessSuceessfull = false };
            //        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"Cannot Reply To comment of post url {FdConstants.FbHomeUrl}{objFdPostCommentDetails.PostId} because its not a page post");
            //        return;
            //    }
            //}

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew
            {
                ResultComment = objFdPostCommentDetails,
                QueryInfo = queryInfo
            });
        }


        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {

        }
    }
}
