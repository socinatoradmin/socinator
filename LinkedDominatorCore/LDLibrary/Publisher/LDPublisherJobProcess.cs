using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Response;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ThreadUtils;
using Unity;
using PublisherDestinationDetailsModel = DominatorHouseCore.Models.SocioPublisher.PublisherDestinationDetailsModel;

namespace LinkedDominatorCore.LDLibrary.Publisher
{
    // ReSharper disable once InconsistentNaming
    public class LDPublisherJobProcess : PublisherJobProcess
    {
        private readonly IDelayService _delayService;
        private IAccountsFileManager _accountsFileManager;
        private bool _isBrowser;
        private ILdFunctions _ldFunction;
        private ILdFunctionFactory _ldFunctionFactory;
        private ILdLogInProcess _logInProcess;
        public LDModel.LdJsonElements Body;
        private LdDataHelper _ldDataHelper;
        private readonly JsonJArrayHandler handler = JsonJArrayHandler.GetInstance;
        public LDPublisherJobProcess(string campaignId, string accountId, SocialNetworks network,
            List<string> groupDestinationLists, List<string> pageDestinationList,
            List<PublisherCustomDestinationModel> customDestinationModels, bool isPublishOnOwnWall,
            CancellationTokenSource campaignCancellationToken, IDelayService delayService)
            : base(campaignId, accountId, network, groupDestinationLists, pageDestinationList, customDestinationModels,
                isPublishOnOwnWall, campaignCancellationToken)
        {
            InitializeValues(accountId);
            _delayService = delayService;
        }

        public LDPublisherJobProcess(string campaignId, string campaignName, string accountId, SocialNetworks network,
            IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken, IDelayService delayService)
            : base(campaignId, campaignName, accountId, network, destinationDetails, campaignCancellationToken)
        {
            InitializeValues(accountId);
            _delayService = delayService;
        }

        private DominatorAccountModel DominatorAccountModel { get; set; }
        private CancellationToken CancellationToken { get; set; }
        private IAccountScopeFactory AccountScopeFactory { get; set; }


        private void InitializeValues(string accountId)
        {
            try
            {
                var uniquePublishId = $"{Guid.NewGuid().ToString()} {accountId}";
                _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                DominatorAccountModel = _accountsFileManager.GetAccountById(accountId);
                // using this scope factory account post only first time next time is not assigning browser window
                AccountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
                _isBrowser = DominatorAccountModel.IsRunProcessThroughBrowser;
                _ldFunctionFactory = AccountScopeFactory[uniquePublishId].Resolve<ILdFunctionFactory>();

                if (_isBrowser && _ldFunctionFactory.LdFunctions.GetType().Name.Equals("LdFunctions"))
                    _ldFunctionFactory.AssignFunction(AccountModel);

                _logInProcess = AccountScopeFactory[uniquePublishId].Resolve<ILdLogInProcess>();
                _ldFunction = _ldFunctionFactory.LdFunctions;
                CancellationToken = DominatorAccountModel.Token;
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }


        public override bool PublishOnGroups(string accountId, string groupUrl, PublisherPostlistModel postDetails,
            bool isDelay)
        {
            var isSuccess = false;
            try
            {
                var UploadParameter = new MediaUploadResponseHandler();
                if (string.IsNullOrEmpty(groupUrl) || postDetails == null || !LoginProcess())
                    return false;

                #region variables

                var activityType = string.Empty;
                var singleUploadUrl = string.Empty;
                var encryptionId = string.Empty;
                var encryption = string.Empty;
                var isMediaVideoType = false;
                var finalResponse = string.Empty;
                var errorMessage = string.Empty;
                var postDataToPostOnGroupWall = string.Empty;

                #endregion

                var postDetailsUpdated = PerformGeneralSettings(postDetails);


                CancellationToken.ThrowIfCancellationRequested();
                var groupId =
                    LdDataHelper.GetInstance
                        .GetGroupIdFromGroupUrl(groupUrl); // Utils.GetBetween(groupUrl + "**", "groups/", "**");

                if (_isBrowser)
                {
                    finalResponse = BrowserPosting(postDetailsUpdated, groupUrl);
                    GetPostDescriptionOfCustomPost(postDetailsUpdated);
                }
                else
                {
                    #region non browser

                    #region CurrentActivityType

                    if (postDetailsUpdated.LdPostSettings.IsGeneralPost)
                        activityType = "DISCUSSION";
                    else if (postDetailsUpdated.LdPostSettings.IsJobPost)
                        activityType = "JOB";
                    else if (postDetailsUpdated.LdPostSettings.IsAnnouncementPost)
                        activityType = "ANNOUNCEMENT";

                    #endregion

                    var actionUrl = LdConstants.GetMediaShareAPI;
                    var postDescription = postDetailsUpdated.PostDescription.Replace("\"", "\\\"").Replace("\r", "\\r")
                        .Replace("\n", "\\n").Replace("'", "").Replace("’", "");
                    if (string.IsNullOrEmpty(postDescription))
                        postDescription = postDetails.ShareUrl;
                    if (string.IsNullOrEmpty(postDescription))
                    {
                        postDescription = string.Empty;
                        //UpdatePostWithFailed(groupUrl, postDetailsUpdated,
                        //    "post description can't be empty if you want to post on group wall.");
                        //GlobusLogHelper.log.Info(Log.ShareFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        //    DominatorAccountModel.AccountBaseModel.UserName, "Groups",
                        //    "post description can't be empty if you want to post on group wall.");
                        //return false;
                    }

                    if (postDetailsUpdated.MediaList.Count > 0)
                    {
                        var fileInfo = new FileInfo(postDetailsUpdated.MediaList[0]);

                        // video uploading part here
                        if (ConstantVariable.SupportedVideoFormat.Contains(fileInfo.Extension.Replace(".", "")))
                        {
                            isMediaVideoType = true;
                            var mediaUploadVideoAndPostDataToPostOnAccountWallTuple =
                                GetPostDataToPostVideo(DominatorAccountModel,_ldFunction, postDetailsUpdated, groupId,ref UploadParameter);
                            if (mediaUploadVideoAndPostDataToPostOnAccountWallTuple != null)
                            {
                                postDataToPostOnGroupWall = mediaUploadVideoAndPostDataToPostOnAccountWallTuple.Item1;
                                try
                                {
                                    singleUploadUrl = mediaUploadVideoAndPostDataToPostOnAccountWallTuple.Item2;
                                    encryptionId = mediaUploadVideoAndPostDataToPostOnAccountWallTuple.Item3;
                                    encryption = mediaUploadVideoAndPostDataToPostOnAccountWallTuple.Item4;
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }

                                if (!mediaUploadVideoAndPostDataToPostOnAccountWallTuple.Item5)
                                    return false;
                            }
                        }
                        // image uploading goes here
                        else
                        {
                            #region UploadImageAndGetContentId

                            try
                            {
                                // trying again sometimes not able to post in group one time
                                for (var i = 0; i < 2; i++)
                                {
                                    if (postDetailsUpdated.MediaList.Count > 1)
                                        finalResponse = UploadMultipleImageGroup(_ldFunction, postDetailsUpdated, groupUrl);
                                    else
                                        finalResponse = UploadSingleImageGroup(_ldFunction, postDetailsUpdated, groupUrl);
                                    if (!string.IsNullOrEmpty(finalResponse))
                                        break;
                                    _delayService.ThreadSleep(25000);
                                }

                                if (finalResponse != null && finalResponse.Contains("(409) Conflict"))
                                    GlobusLogHelper.log.Info(Log.ShareFailed,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName, "Groups",
                                        " may be same post already posted in this group.");
                                //PublishIssues(imageUploadResponse);
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }

                            #endregion
                        }
                    }

                    //post with no media and only description goes here
                    else
                    {
                        var previewResponse = GetPostDescriptionOfCustomPost(postDetailsUpdated, postDescription);
                        var shareActivityId = string.Empty;
                        if (!string.IsNullOrEmpty(previewResponse))
                        {
                            var jsonHandler = new JsonHandler(previewResponse);
                            shareActivityId = jsonHandler.GetJTokenValue(jsonHandler.GetJToken("update", "updateMetadata"), "shareUrn");
                        }
                        postDataToPostOnGroupWall = LdConstants.GetCustomPostListPostData(postDescription, shareActivityId, $",\"containerEntity\":\"urn:li:group:{groupId}\"",string.IsNullOrEmpty(postDetails.PublisherInstagramTitle) ? postDescription: postDetails.PublisherInstagramTitle);
                        finalResponse = _ldFunction.FinalPostRequest(IsPublishOnOwnWall, actionUrl,
                            postDataToPostOnGroupWall, "","");
                    }

                    if (_isBrowser)
                    {
                        finalResponse = BrowserPosting(postDetailsUpdated, groupUrl);
                    }
                    else if (isMediaVideoType && UploadParameter != null && UploadParameter.Type == UploadType.MULTIPART)
                    {
                        var finalResponseTuple = UploadMultiPartVideo(DominatorAccountModel, _ldFunction, postDetailsUpdated,
                            singleUploadUrl, postDataToPostOnGroupWall, UploadParameter);
                        finalResponse = finalResponseTuple.Item1;
                        errorMessage = finalResponseTuple.Item2;
                    }
                    else if (isMediaVideoType && !string.IsNullOrEmpty(singleUploadUrl))
                    {
                        var finalResponseTuple = GetFinalResponseForVideoPost(_ldFunction, postDetailsUpdated,
                            actionUrl, postDataToPostOnGroupWall, singleUploadUrl, encryptionId, encryption);
                        finalResponse = finalResponseTuple.Item1;
                        errorMessage = finalResponseTuple.Item2;
                    }
                    #endregion
                }

                IsPublishOnOwnWall = false;

                isSuccess = IsSuccess(postDetailsUpdated, finalResponse, errorMessage, groupUrl);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
                return isSuccess;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return isSuccess;
            }
            finally
            {
                if (_isBrowser)
                    LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
                if (isDelay)
                    DelayBeforeNextPublish();
            }
            return isSuccess;
        }

        private Tuple<string,string> UploadMultiPartVideo(DominatorAccountModel dominatorAccount, ILdFunctions ldFunction, PublisherPostlistModel postDetailsUpdated, string actionUrl, string postDataToPostOnGroupWall, MediaUploadResponseHandler uploadParameter)
        {
            var finalResponse = string.Empty;
            var errorMessage =string.Empty;
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, "Publish", "please wait while uploading media.");
                //finalResponse =
                //    ldFunction.FinalPostRequest_VideoUploading(actionUrl, postDataToPostOnGroupWall);
                SetRequestParameterForVideoUpload(dominatorAccount, _ldFunction, referer: "https://www.linkedin.com/feed/", PageInstance: $"urn:li:page:d_flagship3_feed;{dominatorAccount.CrmUuid}", PemMeta: "Voyager - Sharing - ShareStatus=sharing-share-status-polling", IsGet: true,UserAgent: "Mozilla/5.0 (X11; Linux i686) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.6599.234 Safari/537.36");
                var uploadActionUrl = $"https://www.linkedin.com/voyager/api/graphql?variables=(shareUrn:{WebUtility.UrlEncode(actionUrl)})&queryId=voyagerContentcreationDashShares.b802f9f485da9e03597508d5fc3d502b";
                finalResponse = ldFunction.GetInnerLdHttpHelper().GetRequest(uploadActionUrl).Response;
                var stopWatch = new Stopwatch();
                //var shareID = Utilities.GetBetween(finalResponse, "\"urn\":\"", "\"");
                //var uploadActionUrl = LdConstants.GetMultipartMediaAPI(shareID);
                stopWatch.Start();
                var isFirst = true;
                while (finalResponse.Contains("PROCESSING"))
                {
                    if (stopWatch.Elapsed.TotalSeconds > 90 && isFirst)
                    {
                        isFirst = false;
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, "Publish",
                            "it seems processing media after uploading taking time.");
                    }
                    _delayService.ThreadSleep(8000);
                    if (stopWatch.Elapsed.Minutes > 10)
                        break;
                    finalResponse = ldFunction.GetVideoUploadResponseStatus(uploadActionUrl);
                }
                stopWatch.Stop();
            }
            catch { }
            return new Tuple<string, string>(finalResponse, errorMessage);
        }


        /// <summary>
        ///     Todo: Implement functionality for publishing on own page
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="pageUrl"></param>
        /// <param name="postDetails"></param>
        /// <param name="isDelay"></param>
        /// <returns></returns>
        public override bool PublishOnPages(string accountId, string pageUrl, PublisherPostlistModel postDetails,
            bool isDelay)
        {
            var isSuccess = false;
            try
            {
                var UploadParameter = new MediaUploadResponseHandler();
                if (string.IsNullOrEmpty(pageUrl) || postDetails == null || !LoginProcess())
                    return false;

                #region variables

                var activityType = string.Empty;
                var singleUploadUrl = string.Empty;
                var encryptionId = string.Empty;
                var encryption = string.Empty;
                var isMediaVideoType = false;
                var finalResponse = string.Empty;
                var errorMessage = string.Empty;
                var isMediaDocType = false;
                var postDataToPostOnPagesWall = string.Empty;
                var postDataToPostOnAccountPage = string.Empty;
                #endregion

                var postDetailsUpdated = PerformGeneralSettings(postDetails);

                //Implement functionality for posting on page

                #region Implement functionality

                CancellationToken.ThrowIfCancellationRequested();

                var pageId = LdDataHelper.GetInstance.GetPageIdFromPageUrl(pageUrl);

                if (_isBrowser)
                {
                    finalResponse = BrowserPosting(postDetailsUpdated, pageUrl);
                    //Get Post Details for Custom Post
                    GetPostDescriptionOfCustomPost(postDetailsUpdated);
                }
                else
                {
                    #region CurrentActivityType

                    if (postDetailsUpdated.LdPostSettings.IsGeneralPost)
                        activityType = "DISCUSSION";
                    else if (postDetailsUpdated.LdPostSettings.IsJobPost)
                        activityType = "JOB";
                    else if (postDetailsUpdated.LdPostSettings.IsAnnouncementPost)
                        activityType = "ANNOUNCEMENT";

                    #endregion

                    var actionUrl = LdConstants.GetMediaShareAPI;
                    var postDescription = postDetailsUpdated.PostDescription.Replace("\"", "\\\"").Replace("\r", "\\r")
                        .Replace("\n", "\\n").Replace("'", "").Replace("’", "");
                    if (string.IsNullOrEmpty(postDescription))
                        postDescription = postDetails.ShareUrl;
                    if (string.IsNullOrEmpty(postDescription))
                    {
                        postDescription = string.Empty;
                        //UpdatePostWithFailed(pageUrl, postDetailsUpdated,
                        //    "post description can't be empty if you want to post on group wall.");
                        //GlobusLogHelper.log.Info(Log.ShareFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        //    DominatorAccountModel.AccountBaseModel.UserName, "Pages",
                        //    "post description can't be empty if you want to post on Company wall.");
                        //return false;
                    }

                    if (postDetailsUpdated.MediaList.Count > 0)
                    {
                        var fileInfo = new FileInfo(postDetailsUpdated.MediaList.FirstOrDefault());
                        // video uploading part here
                        if (ConstantVariable.SupportedVideoFormat.Contains(fileInfo.Extension.Replace(".", "")))
                        {
                            isMediaVideoType = true;
                            var mediaUploadVideoAndPostDataToPostOnAccountWallTuple =
                                GetPostDataToPostVideo(DominatorAccountModel,_ldFunction, postDetailsUpdated, pageId,ref UploadParameter, true);
                            if (mediaUploadVideoAndPostDataToPostOnAccountWallTuple != null)
                            {
                                postDataToPostOnPagesWall = mediaUploadVideoAndPostDataToPostOnAccountWallTuple.Item1;
                                try
                                {
                                    singleUploadUrl = mediaUploadVideoAndPostDataToPostOnAccountWallTuple.Item2;
                                    encryptionId = mediaUploadVideoAndPostDataToPostOnAccountWallTuple.Item3;
                                    encryption = mediaUploadVideoAndPostDataToPostOnAccountWallTuple.Item4;
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }

                                if (!mediaUploadVideoAndPostDataToPostOnAccountWallTuple.Item5)
                                    return false;
                            }
                        }
                        //if uploaded file having no extension then it will post only description
                        else if(string.IsNullOrEmpty(fileInfo.Extension))
                        {
                            postDataToPostOnPagesWall =
                                                            "{\"externalAudienceProviders\":[],\"visibleToConnectionsOnly\":false,\"commentary\":{\"text\":\"" +
                                                            postDescription +
                                                            "\",\"attributes\":[]},\"organizationActor\":\"urn:li:fs_normalized_company:" + pageId +
                                                            "\",\"origin\":\"ORGANIZATION\",\"media\":[]}";


                            finalResponse = _ldFunction.FinalPostRequest(IsPublishOnOwnWall, actionUrl,
                                postDataToPostOnPagesWall, "","");
                        }
                        // image uploading goes here
                        else
                        {
                            #region UploadImageAndGetContentId

                            try
                            {
                                if (fileInfo.Extension.Contains(".docx") || fileInfo.Extension.Contains(".pdf"))
                                {
                                    if (InitializeDocFilesUploading(postDetailsUpdated, out isMediaDocType,
                                        ref postDataToPostOnAccountPage, ref singleUploadUrl, ref isSuccess, out var publishOnOwnWall,true,pageId))
                                        return publishOnOwnWall;
                                }
                                else
                                {
                                    // trying again sometimes not able to post in group one time
                                    for (var i = 0; i < 2; i++)
                                    {
                                        if (postDetailsUpdated.MediaList.Count > 1)
                                            finalResponse = UploadMultipleImagePage(_ldFunction, postDetailsUpdated, pageUrl);
                                        else
                                            finalResponse = UploadSingleImagePage(_ldFunction, postDetailsUpdated, pageUrl);
                                        if (!string.IsNullOrEmpty(finalResponse))
                                            break;
                                        _delayService.ThreadSleep(25000);
                                    }
                                }
                                if (finalResponse != null && finalResponse.Contains("(409) Conflict"))
                                    GlobusLogHelper.log.Info(Log.ShareFailed,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.AccountBaseModel.UserName, "Pages",
                                        " may be same post already posted in this Page.");
                                //PublishIssues(imageUploadResponse);
                            }
                            catch (Exception ex)
                            {
                                
                                ex.DebugLog();
                            }

                            #endregion
                        }
                    }
                    // post with no media and only description goes here
                    else
                    {
                        if(!string.IsNullOrEmpty(postDetails.ShareUrl))
                        {
                            var previewResponse = GetPostDescriptionOfCustomPost(postDetailsUpdated);
                            var jsonHandler = new JsonHandler(previewResponse);
                            var ActivityId = jsonHandler.GetJTokenValue(jsonHandler.GetJToken("update", "updateMetadata"), "shareUrn");
                            postDataToPostOnPagesWall = $"{{\"visibleToConnectionsOnly\":false,\"externalAudienceProviders\":[],\"commentaryV2\":{{\"text\":\"\",\"attributes\":[]}},\"origin\":\"ORGANIZATION\",\"allowedCommentersScope\":\"ALL\",\"postState\":\"PUBLISHED\",\"parentUrn\":\"{ActivityId}\",\"dashNonMemberActor\":\"urn:li:fsd_company:{pageId}\"}}";
                        }
                        else
                        {
                            postDataToPostOnPagesWall = $"{{\"externalAudienceProviders\":[],\"visibleToConnectionsOnly\":false,\"commentary\":{{\"text\":\"\",\"attributes\":[]}},\"organizationActor\":\"urn:li:fs_normalized_company:{pageId}\",\"origin\":\"ORGANIZATION\",\"media\":[]}}";
                        }
                        finalResponse = _ldFunction.FinalPostRequest(IsPublishOnOwnWall, actionUrl,
                            postDataToPostOnPagesWall, "","");
                    }

                    if (_isBrowser)
                    {
                        finalResponse = BrowserPosting(postDetailsUpdated, pageUrl);
                    }
                    else if (isMediaVideoType && UploadParameter != null && UploadParameter.Type == UploadType.MULTIPART)
                    {
                        var finalResponseTuple = UploadMultiPartVideo(DominatorAccountModel, _ldFunction, postDetailsUpdated,
                            singleUploadUrl, postDataToPostOnPagesWall, UploadParameter);
                        finalResponse = finalResponseTuple.Item1;
                        errorMessage = finalResponseTuple.Item2;
                    }
                    else if (isMediaVideoType)
                    {
                        var finalResponseTuple = GetFinalResponseForVideoPost(_ldFunction, postDetailsUpdated,
                            actionUrl, postDataToPostOnPagesWall, singleUploadUrl, encryptionId, encryption);
                        finalResponse = finalResponseTuple.Item1;
                        errorMessage = finalResponseTuple.Item2;
                    }
                    else if(isMediaDocType)
                    {
                        #region DocType

                        try
                        {
                            Tuple<string, string> finalResponseTupple = null;
                            finalResponseTupple = GetFinalResponseForDocumentPost(_ldFunction, postDetailsUpdated,
                                actionUrl,
                                postDataToPostOnAccountPage, singleUploadUrl, encryptionId, encryption);
                            finalResponse = finalResponseTupple.Item1;
                            errorMessage = finalResponseTupple.Item2;
                        }
                        catch (Exception ex)
                        {
                            errorMessage = ex.Message;
                        }

                        #endregion
                    }
                }

                #endregion

                isSuccess = IsSuccess(postDetailsUpdated, finalResponse, errorMessage, pageurl: pageUrl);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
                return isSuccess;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return isSuccess;
            }
            finally
            {
                if (_isBrowser)
                    LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
                if (isDelay)
                    DelayBeforeNextPublish();
            }
            return isSuccess;
        }

        private string GetPostDescriptionOfCustomPost(PublisherPostlistModel postDetailsUpdated,string PostDescription="")
        {
            var previewResponse = string.Empty;
            _ldDataHelper = LdDataHelper.GetInstance;
            var FinalUrl = string.IsNullOrEmpty(postDetailsUpdated.ShareUrl) ? PostDescription : postDetailsUpdated.ShareUrl;
            if (!string.IsNullOrEmpty(FinalUrl) && (FinalUrl.StartsWith("https://") || FinalUrl.StartsWith("http://")))
            {
                previewResponse = _ldDataHelper.GetFeedPreviewResponse(_ldFunction, FinalUrl);
                int failedCount = 0;
                while (failedCount++ <= 2 && string.IsNullOrEmpty(previewResponse))
                    previewResponse = _ldDataHelper.GetFeedPreviewResponse(_ldFunction, FinalUrl);
                if (!string.IsNullOrEmpty(previewResponse))
                {
                    var jsonHandler = new JsonHandler(previewResponse);
                    postDetailsUpdated.PostDescription = jsonHandler.GetJTokenValue(jsonHandler.GetJToken("update", "commentary", "text"), "text");
                }
            }
            return previewResponse;
        }

        private bool LoginProcess()
        {
            if (_isBrowser)
            {
                _logInProcess.LoginWithBrowserMethod(DominatorAccountModel, DominatorAccountModel.Token);
                if (!DominatorAccountModel.IsUserLoggedIn)
                    return false;
            }
            else if (!_logInProcess.CheckLogin(DominatorAccountModel, DominatorAccountModel.Token))
            {
                return false;
            }

            return true;
        }

        private string BrowserPosting(PublisherPostlistModel postDetails, string groupUrl)
        {
            var response = string.Empty;
            var Postdestination = string.Empty;
            try
            {
                if (groupUrl.Contains("company/"))
                    Postdestination = "OnPage";
                if (!string.IsNullOrWhiteSpace(groupUrl))
                    _ldFunction.GetRequestUpdatedUserAgent(groupUrl);

                var media = postDetails.MediaList.Count == 0 ? new List<string> { "" } : postDetails.MediaList.ToList();
                var ListStringMedia = string.Join(",", media);
                if (string.IsNullOrEmpty(ListStringMedia))
                    ListStringMedia = postDetails.ShareUrl;
                response = _ldFunction.FinalPostRequest(IsPublishOnOwnWall, postDetails.PostDescription,
                    ListStringMedia, Postdestination, postDetails.PublisherInstagramTitle);
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return response;
        }

        public override bool PublishOnOwnWall(string accountId, PublisherPostlistModel postDetails, bool isDelay)
        {
            var isSuccess = false;
            try
            {
                IsPublishOnOwnWall = true;
                var UploadParameter = new MediaUploadResponseHandler();
                #region variables

                var postDataToPostOnAccountWall = string.Empty;
                var finalResponse = string.Empty;
                var errorMessage = string.Empty;
                var singleUploadUrl = string.Empty;

                #endregion

                CancellationToken.ThrowIfCancellationRequested();

                if (string.IsNullOrEmpty(accountId) || !LoginProcess())
                    return false;

                var postDetailsUpdated = PerformGeneralSettings(postDetails);
                ReplaceFileNameAsDescription(postDetails, postDetailsUpdated);
                if (_isBrowser)
                {
                    finalResponse = BrowserPosting(postDetailsUpdated, "");
                    GetPostDescriptionOfCustomPost(postDetailsUpdated);
                }
                else
                    try
                    {
                        var encryptionId = string.Empty;
                        var encryption = string.Empty;
                        var isMediaVideoType = false;
                        var isMediaDocType = false;
                        var postDescription = postDetailsUpdated.PostDescription?.Replace("\r", "").Replace("\n", "\\n")
                            .Replace("\"", "\\\"")
                            .Replace("'", "").Replace("’", "");
                        var actionUrl = LdConstants.GetMediaShareAPI;

                        #region media uploading

                        if (postDetailsUpdated.MediaList.Count > 0)
                        {
                            var downloadPath = Utils.GetDownloadedMediaPath();
                            _ldFunction.GetInnerLdHttpHelper()
                                .DownloadFile(postDetailsUpdated.MediaList?.FirstOrDefault(), downloadPath);
                            var fileInfo = new FileInfo(postDetailsUpdated.MediaList?.FirstOrDefault());

                            
                            // video uploading part here
                            if (ConstantVariable.SupportedVideoFormat.Contains(fileInfo.Extension.Replace(".", "")))
                            {
                                if (InitializeVideoUploading(DominatorAccountModel,postDetailsUpdated, out isMediaVideoType,
                                    ref postDataToPostOnAccountWall, ref singleUploadUrl, ref encryptionId,
                                    ref encryption, ref isSuccess,ref UploadParameter, out var publishOnOwnWall))
                                    return publishOnOwnWall;
                            }
                            //Doc and pdf file types uploading
                            if (fileInfo.Extension.Contains(".docx") || fileInfo.Extension.Contains(".pdf"))
                            {
                                if (InitializeDocFilesUploading(postDetailsUpdated, out isMediaDocType,
                                    ref postDataToPostOnAccountWall, ref singleUploadUrl, ref isSuccess, out var publishOnOwnWall))
                                    return publishOnOwnWall;
                            }
                            // image uploading part here
                            else
                            {
                                try
                                {
                                    if(string.IsNullOrEmpty(singleUploadUrl) || !singleUploadUrl.Contains("urn:li:"))
                                    {
                                        if (postDetailsUpdated.MediaList.Count > 0)
                                            finalResponse = UploadMultipleImage(_ldFunction, postDetailsUpdated);
                                        else
                                            finalResponse = UploadSingleImage(_ldFunction, postDetailsUpdated);
                                        var jObject = handler.ParseJsonToJObject(finalResponse);
                                        isSuccess = handler
                                            .GetJTokenValue(jObject, "status", "mediaStatus").Equals("READY");
                                        isSuccess = isSuccess ? isSuccess : handler
                                            .GetJTokenValue(jObject, "data", "status", "mediaStatus").Equals("READY");
                                    }
                                    
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }
                            }
                        }

                        #endregion

                        #region text posting

                        else
                        {
                            if (postDescription.Contains("https://") || postDescription.Contains("http://"))
                                postDataToPostOnAccountWall =
                                    GetPostDataToPost(string.Empty, string.Empty, postDescription);
                            else if (!string.IsNullOrEmpty(postDetailsUpdated.ShareUrl) && (postDetailsUpdated.ShareUrl.StartsWith("https://") || postDetailsUpdated.ShareUrl.StartsWith("http://")))
                            {
                                var previewResponse = GetPostDescriptionOfCustomPost(postDetailsUpdated);
                                var jobject = handler.ParseJsonToJObject(previewResponse);
                                var ActivityId = handler.GetJTokenValue(handler.GetJTokenOfJToken(jobject, "update", "updateMetadata"), "shareUrn");
                                postDataToPostOnAccountWall = LdConstants.GetCustomPostListPostData(postDetailsUpdated.ShareUrl, ActivityId,string.Empty, string.IsNullOrEmpty(postDetailsUpdated.PublisherInstagramTitle) ? postDescription : postDetailsUpdated.PublisherInstagramTitle);
                            }
                            else
                                postDataToPostOnAccountWall = LdConstants.GetCustomPostListPostData(string.IsNullOrEmpty(postDetailsUpdated.ShareUrl)?"Custom Post":postDetailsUpdated.ShareUrl, string.Empty,string.Empty, string.IsNullOrEmpty(postDetailsUpdated.PublisherInstagramTitle) ? postDescription : postDetailsUpdated.PublisherInstagramTitle);
                        }

                        #endregion

                        if (_isBrowser)
                        {
                            finalResponse = BrowserPosting(postDetailsUpdated, "");
                        }

                        //doc media
                        else if(isMediaDocType)
                        {
                            #region DocType

                            try
                            {
                                Tuple<string, string> finalResponseTupple = null;
                                finalResponseTupple = GetFinalResponseForDocumentPost(_ldFunction, postDetailsUpdated,
                                    actionUrl,
                                    postDataToPostOnAccountWall, singleUploadUrl, encryptionId, encryption);
                                finalResponse = finalResponseTupple.Item1;
                                errorMessage = finalResponseTupple.Item2;
                            }
                            catch (Exception ex)
                            {
                                errorMessage = ex.Message;
                            }

                            #endregion
                        }
                        else if (isMediaVideoType && UploadParameter != null && UploadParameter.Type == UploadType.MULTIPART)
                        {
                            var finalResponseTuple = UploadMultiPartVideo(DominatorAccountModel,_ldFunction, postDetailsUpdated,
                                singleUploadUrl, postDataToPostOnAccountWall, UploadParameter);
                            finalResponse = finalResponseTuple.Item1;
                            errorMessage = finalResponseTuple.Item2;
                        }
                        else if (isMediaVideoType)
                        {
                            #region VideoType

                            try
                            {
                                var finalResponseTupple = GetFinalResponseForVideoPost(_ldFunction, postDetailsUpdated,
                                    actionUrl,
                                    postDataToPostOnAccountWall, singleUploadUrl, encryptionId, encryption);
                                finalResponse = finalResponseTupple.Item1;
                                errorMessage = finalResponseTupple.Item2;
                            }
                            catch (Exception ex)
                            {
                                errorMessage = ex.Message;
                            }

                            #endregion
                        }
                        else
                        {
                            var dFlagship3Feed = string.Empty;

                            #region Other Media

                            // if not done final request with media 
                            if (!isSuccess)
                                finalResponse = _ldFunction.FinalPostRequest(IsPublishOnOwnWall, actionUrl,
                                    postDataToPostOnAccountWall, dFlagship3Feed, "");

                            #endregion
                        }
                    }

                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                isSuccess = IsSuccess(postDetailsUpdated, finalResponse, errorMessage,pageurl:singleUploadUrl);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);

                return isSuccess;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return isSuccess;
            }
            finally
            {
                if (_isBrowser)
                    LDAccountsBrowserDetails.CloseBrowser(DominatorAccountModel);
                if (isDelay)
                    DelayBeforeNextPublish();
            }
            return isSuccess;
        }

        private void ReplaceFileNameAsDescription(PublisherPostlistModel postDetails, PublisherPostlistModel updatedPost)
        {
            if (postDetails.MediaList.Count > 0 && ((postDetails.scrapePostModel.IsScrapeGoogleImgaes && postDetails.scrapePostModel.IsUseFileNameAsDescription) || (postDetails.PostDetailModel.IsUploadMultipleImage && postDetails.PostDetailModel.IsUseFileNameAsDescription)))
                updatedPost.PostDescription = new FileInfo(postDetails.MediaList.FirstOrDefault()).Name;
            else
                updatedPost.PostDescription = postDetails.PostDescription;
        }

        private bool InitializeVideoUploading(DominatorAccountModel dominatorAccount, PublisherPostlistModel postDetails, out bool isMediaVideoType,
            ref string postDataToPostOnAccountWall, ref string singleUploadUrl, ref string encryptionId,
            ref string encryption,
            ref bool isSuccess,ref MediaUploadResponseHandler mediaUploadResponse, out bool publishOnOwnWall)
        {
            publishOnOwnWall = false;
            isMediaVideoType = true;
            var mediaUploadVideoAndPostDataToPostOnAccountWallTuple =
                GetPostDataToPostVideo(dominatorAccount,_ldFunction, postDetails, string.Empty,ref mediaUploadResponse);

            if (mediaUploadVideoAndPostDataToPostOnAccountWallTuple == null)
                return false;

            postDataToPostOnAccountWall = mediaUploadVideoAndPostDataToPostOnAccountWallTuple.Item1;
            try
            {
                singleUploadUrl = mediaUploadVideoAndPostDataToPostOnAccountWallTuple.Item2;
                encryptionId = mediaUploadVideoAndPostDataToPostOnAccountWallTuple.Item3;
                encryption = mediaUploadVideoAndPostDataToPostOnAccountWallTuple.Item4;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            isSuccess = mediaUploadVideoAndPostDataToPostOnAccountWallTuple.Item5;
            if (isSuccess)
                return false;

            publishOnOwnWall = !isSuccess;
            return true;
        }

        /// <summary>
        /// This method is use to upload doc formated files
        /// </summary>
        /// <param name="postDetails"></param>
        /// <param name="isMediadocType"></param>
        /// <param name="postDataToPostOnAccountWall"></param>
        /// <param name="singleUploadUrl"></param>
        /// <param name="isSuccess"></param>
        /// <param name="publishOnOwnWall"></param>
        /// <returns></returns>
        private bool InitializeDocFilesUploading(PublisherPostlistModel postDetails, out bool isMediaDocType,
            ref string postDataToPostOnAccountWall, ref string singleUploadUrl,
            ref bool isSuccess, out bool publishOnOwnWall,bool IsPublishOnPage=false,string PageId="")
        {
            publishOnOwnWall = false;
            isMediaDocType = true;
            var mediaUploadDocAndPostDataToPostOnAccountWallTuple =
                GetPostDataToPostDocPdfFiles(_ldFunction, postDetails,IsPublishOnPage,PageId);

            if (mediaUploadDocAndPostDataToPostOnAccountWallTuple == null)
                return false;

            postDataToPostOnAccountWall = mediaUploadDocAndPostDataToPostOnAccountWallTuple.Item1;
            try
            {
                singleUploadUrl = mediaUploadDocAndPostDataToPostOnAccountWallTuple.Item2;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            isSuccess = mediaUploadDocAndPostDataToPostOnAccountWallTuple.Item5;
            if (isSuccess)
                return false;

            publishOnOwnWall = !isSuccess;
            return true;
        }

        private bool IsSuccess(PublisherPostlistModel postDetails, string finalResponse, string errorMessage,
            string groupUrl = "", string pageurl = "")
        {
            bool isSuccess;
            var PostSuccess = string.IsNullOrEmpty(finalResponse)||finalResponse.Contains("PROCESSING_FAILED") ? false :
                finalResponse.Contains("urn:li:activity") ? true :
                finalResponse.Contains("urn:li:ugcPost") ? true :
                finalResponse.Contains("urn:li:groupPost") ? true:
                finalResponse.Contains("toastCtaUrl\":\"")?true:
                !string.IsNullOrEmpty(pageurl) && pageurl.Contains("urn:li:");
            if (!PostSuccess)
            {
                isSuccess = false;
                UpdatePostWithFailed(DominatorAccountModel.AccountBaseModel.UserName, postDetails, errorMessage);
                errorMessage = Utils.GetBetween(finalResponse, "\"mediaStatus\":\"", "\"");
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = finalResponse;
                if (string.IsNullOrEmpty(pageurl))
                    GlobusLogHelper.log.Info(Log.ShareFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, IsPublishOnOwnWall ? "Own wall" : "Groups",
                        errorMessage);
                else
                    GlobusLogHelper.log.Info(Log.SharedSuccessfully,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "Pages",
                        errorMessage);
            }
            else
            {
                var postLink = _isBrowser ? finalResponse : GetPostLink(finalResponse,pageurl);
                isSuccess = true;
                var destination = IsPublishOnOwnWall ? DominatorAccountModel.AccountBaseModel.UserName : groupUrl;
                if ((destination == groupUrl || !string.IsNullOrEmpty(destination)) &&
                    (!string.IsNullOrEmpty(groupUrl) || string.IsNullOrEmpty(groupUrl)) &&
                    string.IsNullOrWhiteSpace(pageurl) ||(!string.IsNullOrEmpty(pageurl) && pageurl.Contains("urn:li:")))
                {
                    GlobusLogHelper.log.Info(Log.SharedSuccessfully,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, IsPublishOnOwnWall ? "Own wall" : "Groups",
                        destination);
                }
                else
                {
                    destination = pageurl;
                    GlobusLogHelper.log.Info(Log.SharedSuccessfully,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "Pages",
                        destination);
                }
                if(!string.IsNullOrEmpty(postLink))
                    UpdatePostWithSuccessful(destination, postDetails, postLink);
            }

            return isSuccess;
        }

        /// <summary>
        ///     post in description having link we get thumbnail for that link
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="groupId"></param>
        /// <param name="postDescription"></param>
        /// <returns> get generated postData with thumbnail link</returns>
        public string GetPostDataToPost(string activityType, string groupId, string postDescription)
        {
            try
            {
                #region Variable initialization required for this method

                var link = string.Empty;
                var postData = string.Empty;
                var linkResponse = string.Empty;
                var contructedLinkApi = string.Empty;
                var originalHeight = string.Empty;
                var originalWidth = string.Empty;
                var mediaURl = string.Empty;
                var article = string.Empty;
                var title = string.Empty;
                var description = string.Empty;

                #endregion

                #region link_Response

                try
                {
                    link = GetLink(postDescription);

                    contructedLinkApi = "https://www.linkedin.com/voyager/api/feed/urlpreview/" +
                                        Uri.EscapeDataString(link);
                    CancellationToken.ThrowIfCancellationRequested();
                    linkResponse = _ldFunction.GetInnerHttpHelper().GetRequest(contructedLinkApi).Response;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                try
                {
                    var objJObject = JObject.Parse(linkResponse);
                    var json = JsonJArrayHandler.GetInstance;
                    #region originalHeight

                    try
                    {
                        originalHeight = json.GetJTokenValue(objJObject, "value",
                            "com.linkedin.voyager.feed.urlpreview.PreviewCreationSuccessful",
                            "data", "previewImages",0, "mediaProxyImage", "com.linkedin.voyager.common.MediaProxyImage",
                            "originalHeight");
                    }
                    catch (Exception exx)
                    {
                        exx.DebugLog();
                        originalHeight = "";
                    }

                    #endregion

                    #region originalWidth

                    try
                    {
                        originalWidth = json.GetJTokenValue(objJObject, "value",
                            "com.linkedin.voyager.feed.urlpreview.PreviewCreationSuccessful",
                            "data", "previewImages",0, "mediaProxyImage", "com.linkedin.voyager.common.MediaProxyImage",
                            "originalWidth");
                    }
                    catch (Exception exx)
                    {
                        exx.DebugLog();
                        originalWidth = "";
                    }
                    #endregion

                    #region MediaURl

                    try
                    {
                        if (!objJObject.ToString().Contains("\"previewImages\": []"))
                        {
                            mediaURl = json.GetJTokenValue(objJObject, "value",
                            "com.linkedin.voyager.feed.urlpreview.PreviewCreationSuccessful",
                            "data", "previewImages", 0, "mediaProxyImage", "com.linkedin.voyager.common.MediaProxyImage", "url");
                        }
                        if (string.IsNullOrEmpty(mediaURl))
                            mediaURl = json.GetJTokenValue(objJObject, "value",
                            "com.linkedin.voyager.feed.urlpreview.PreviewCreationSuccessful",
                            "data", "url");
                    }
                    catch (Exception exx)
                    {
                        exx.DebugLog();
                        mediaURl = "";
                    }

                    #endregion

                    #region article

                    try
                    {
                        article = json.GetJTokenValue(objJObject, "value",
                            "com.linkedin.voyager.feed.urlpreview.PreviewCreationSuccessful",
                            "data","id");
                    }
                    catch (Exception exx)
                    {
                        exx.DebugLog();
                        article = "";
                    }

                    #endregion

                    #region Title

                    try
                    {
                        title = json.GetJTokenValue(objJObject, "value",
                            "com.linkedin.voyager.feed.urlpreview.PreviewCreationSuccessful",
                            "data","title");
                    }
                    catch (Exception exx)
                    {
                        exx.DebugLog();
                        title = "";
                    }

                    #endregion

                    #region Description

                    try
                    {
                        description = json.GetJTokenValue(objJObject, "value",
                            "com.linkedin.voyager.feed.urlpreview.PreviewCreationSuccessful",
                            "data", "description");
                    }
                    catch (Exception exx)
                    {
                        exx.DebugLog();
                        description = "";
                    }

                    #endregion
                }
                catch (Exception exx)
                {
                    exx.DebugLog();
                }

                if (IsPublishOnOwnWall)
                {
                    if (!string.IsNullOrEmpty(article))
                        postData = $"{{\"visibleToConnectionsOnly\":false,\"externalAudienceProviders\":[],\"commentaryV2\":{{\"text\":\"{postDescription}\",\"attributes\":[]}},\"origin\":\"FEED\",\"allowedCommentersScope\":\"ALL\",\"postState\":\"PUBLISHED\",\"media\":[{{\"mediaUrn\":\"{article}\",\"originalUrl\":\"{link}\"}}]}}";
                    else
                        postData = $"{{\"visibleToConnectionsOnly\":false,\"externalAudienceProviders\":[],\"commentaryV2\":{{\"text\":\"{postDescription}\",\"attributes\":[]}},\"origin\":\"FEED\",\"allowedCommentersScope\":\"ALL\",\"postState\":\"PUBLISHED\",\"media\":[]}}";
                }
                else
                {
                    if (!string.IsNullOrEmpty(article))
                        postData = $"{{\"visibleToConnectionsOnly\":false,\"externalAudienceProviders\":[],\"commentaryV2\":{{\"text\":\"{postDescription}\",\"attributes\":[]}},\"containerEntity\":\"urn:li:fsd_group:{groupId}\",\"origin\":\"CONTAINER_FEED\",\"allowedCommentersScope\":\"ALL\",\"postState\":\"PUBLISHED\",\"media\":[{{\"mediaUrn\":\"{article}\",\"originalUrl\":\"{link}\"}}]}}";
                    else
                        postData = $"{{\"visibleToConnectionsOnly\":false,\"externalAudienceProviders\":[],\"commentaryV2\":{{\"text\":\"{postDescription}\",\"attributes\":[]}},\"containerEntity\":\"urn:li:fsd_group:{groupId}\",\"origin\":\"CONTAINER_FEED\",\"allowedCommentersScope\":\"ALL\",\"postState\":\"PUBLISHED\",\"media\":[]}}";
                }
                return postData;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public string GetLink(string postDescription)
        {
            try
            {
                #region MyRegion

                var splittedPostDescription = new string[] { };
                var link = string.Empty;
                try
                {
                    splittedPostDescription = Regex.Split(postDescription, " ").ToArray();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                try
                {
                    link = splittedPostDescription.FirstOrDefault(x => x.Contains("https://") || x.Contains("http://"));
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                if (!link.StartsWith("https://") || !link.StartsWith("http://"))
                    try
                    {
                        var itemsTobeRmoved = Utils.GetBetween("**" + link, "**", "http");
                        link = link.Replace(itemsTobeRmoved, "");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                if (link.Contains("\\r\\n"))
                {
                    try
                    {
                        var extra = Utils.GetBetween(link + "**", "\\r\\n", "**");
                        link = link.Replace(extra, "");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    link = link.Replace("\\r\\n", "");
                }

                return link = link.TrimEnd('\n').TrimEnd('\r');

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return "";
            }
        }

        public string GetPostLink(string finalResponse,string urn="")
        {
            try
            {
                var postLink = string.Empty;
                try
                {
                    postLink = Utils.GetBetween(finalResponse, "toastCtaUrl\":\"", "\"");
                    if (!string.IsNullOrEmpty(urn))
                        postLink = urn.Contains("/company/") || urn.Contains("https://") ?!string.IsNullOrEmpty(postLink)?postLink:urn:$"https://www.linkedin.com/feed/update/{urn}";
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                if (string.IsNullOrEmpty(postLink))
                {
                    var activityId = Utils.GetBetween(finalResponse, "", "\"");
                    if (!string.IsNullOrEmpty(activityId))
                        postLink = "https://www.linkedin.com/feed/update/urn:li:activity:" + activityId;
                }

                return postLink;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public Tuple<string, string, string, string, bool> GetPostDataToPostVideo(DominatorAccountModel dominatorAccount, ILdFunctions objLdFunctions,
            PublisherPostlistModel postDetails, string groupId,ref MediaUploadResponseHandler mediaUploadResponse, bool IspublishedOnpage = false)
        {
            try
            {
                #region Initializations required for this method

                Tuple<string, string, string, string, bool> videoUploadAndPostDataTupple = null;
                var isSuccess = true;
                string postDataToPostVideo;
                string mediaUrn;
                string videoUploadPostData;
                string singleUploadUrl;
                string encryptionId;
                string encryption;
                var postDescription = postDetails.PostDescription?.Replace("\r", "").Replace("\n", "\\n")
                    .Replace("\"", "\\\"");
                var objDetailedFileInfo = new FileInfo(postDetails.MediaList[0]);

                #endregion
                SetparametersForUploadVideo(groupId);
                var videoUploadUrl = LdConstants.GetLDMediaUploadAPI;
                var imageByte = File.ReadAllBytes(objDetailedFileInfo.FullName.Replace("_SOCINATORIMAGE.jpg", ""));
                if (IspublishedOnpage)
                    videoUploadPostData = "{\"mediaUploadType\":\"VIDEO_SHARING\",\"fileSize\":" + imageByte.Length +
                                          ",\"organizationActor\":\"urn:li:fs_normalized_company:" + groupId +
                                          "\",\"filename\":\"" +
                                          objDetailedFileInfo.Name.Replace("_SOCINATORIMAGE.jpg", "") + "\"}";
                else
                    videoUploadPostData = "{\"mediaUploadType\":\"VIDEO_SHARING\",\"fileSize\":" + imageByte.Length +
                                          ",\"filename\":\"" +
                                          objDetailedFileInfo.Name.Replace("_SOCINATORIMAGE.jpg", "") + "\"}";
                CancellationToken.ThrowIfCancellationRequested();
                var refererString = IspublishedOnpage ? $"https://www.linkedin.com/company/{groupId}/admin/page-posts/published/?share=true" : "https://www.linkedin.com/feed/?trk=guest_homepage-basic_nav-header-signin";
                SetRequestParameterForVideoUpload(dominatorAccount, _ldFunction,refererString);
                var videoUploadResponse = _ldFunction.GetInnerHttpHelper()
                    .PostRequest(videoUploadUrl, videoUploadPostData).Response;

                if (videoUploadResponse.Contains("{\"value\":{\"urn\":\"") ||
                    videoUploadResponse.Contains("urn:li:digitalmediaAsset"))
                {
                    mediaUrn = Utils.GetBetween(videoUploadResponse, "urn\":\"", "\"");
                    var uploadUrlList = new List<string>();
                    mediaUploadResponse = GetUploadMetaData(videoUploadResponse);
                    if (string.IsNullOrWhiteSpace(singleUploadUrl =
                        Utils.GetBetween(videoUploadResponse, "singleUploadUrl\":\"", "\"")) && mediaUploadResponse.Type == UploadType.SINGLE)
                    {
                        var jArray = JObject.Parse(videoUploadResponse);
                        var jsonArrayHandler = JsonJArrayHandler.GetInstance;

                        var list = jsonArrayHandler.GetJTokenOfJToken(jArray, "value", "partUploadRequests");
                        foreach (var item in list)
                            uploadUrlList.Add(jsonArrayHandler.GetJTokenValue(item, "uploadUrl"));
                        _isBrowser = true;
                        LoginProcess();
                        _ldFunction = _ldFunctionFactory.LdFunctions;
                    }
                    else
                    {
                        if(!string.IsNullOrEmpty(singleUploadUrl))
                            uploadUrlList.Add(singleUploadUrl);
                    }
                    singleUploadUrl = uploadUrlList==null || uploadUrlList.Count ==0 ?string.Empty: JsonConvert.SerializeObject(uploadUrlList);
                    encryptionId = Utils.GetBetween(videoUploadResponse,
                        "x-amz-server-side-encryption-aws-kms-key-id\":\"", "\"");
                    encryption = Utils.GetBetween(videoUploadResponse, "x-amz-server-side-encryption\":\"", "\"");
                    if (mediaUploadResponse.Type == UploadType.MULTIPART)
                    {
                        dominatorAccount.CrmUuid = Utils.GenerateTrackingId();
                        var pageInstance =IspublishedOnpage ?$"urn:li:page:d_flagship3_company_admin;{dominatorAccount.CrmUuid}" : $"urn:li:page:d_flagship3_feed;{dominatorAccount.CrmUuid}";
                        var pemMeta =IspublishedOnpage ? "Voyager - Sharing - CreateShare=sharing-create-content,Voyager - Organization - Admin=organization-create-post-as-page" : "Voyager - Sharing - CreateShare=sharing-create-content";
                        SetRequestParameterForVideoUpload(dominatorAccount, _ldFunction,refererString,pageInstance,pemMeta);
                        var urn = Utils.GetBetween(videoUploadResponse, "\"urn\":\"", "\"");
                        var executeUploadBody =IspublishedOnpage ?$"{{\"mediaUploadType\":\"VIDEO_SHARING\",\"fileSize\":{imageByte.Length},\"nonMemberActorUrn\":\"urn:li:fsd_company:{groupId}\",\"filename\":\"{objDetailedFileInfo.Name?.Replace("-","_")}\"}}" : $"{{\"variables\":{{\"post\":{{\"allowedCommentersScope\":\"ALL\",\"intendedShareLifeCycleState\":\"PUBLISHED\",\"origin\":\"FEED\",\"visibilityDataUnion\":{{\"visibilityType\":\"ANYONE\"}},\"commentary\":{{\"text\":\"Nature\",\"attributesV2\":[]}},\"media\":{{\"category\":\"VIDEO\",\"mediaUrn\":\"{urn}\",\"recipes\":[\"urn:li:digitalmediaRecipe:feedshare-video-captions-thumbnails-ambry\",\"urn:li:digitalmediaRecipe:feedshare-video-auto-caption-public\"],\"nativeMediaSource\":\"PRE_RECORDED\"}}}}}},\"queryId\":\"voyagerContentcreationDashShares.b847d1c55e746148ce7cb64f4f46b2de\",\"includeWebMetadata\":true}}";
                        var guid1 = IspublishedOnpage ? "0015225a6e37d1399fbe6e159037f4e1" : "b847d1c55e746148ce7cb64f4f46b2de";
                        var ExecuteUploadResponse = _ldFunction.GetInnerHttpHelper()
                            .PostRequest($"https://www.linkedin.com/voyager/api/graphql?action=execute&queryId=voyagerContentcreationDashShares.{guid1}", executeUploadBody).Response;
                        var uploadCompleteHeaders = new List<HeaderCollection>();
                        foreach (var data in mediaUploadResponse.MultiUploadData)
                        {
                            SetRequestParameterForVideoUpload(dominatorAccount, _ldFunction, refererString,contentType:data.ContentType,accept:"*/*");
                            var byteToUpload = imageByte.Skip(data.firstByte-1).Take(data.lastByte-(data.firstByte-1)).ToArray();
                            var putResponse = _ldFunction.GetInnerLdHttpHelper().PutRequest(data.UploadUrl, byteToUpload);
                            var responseHeader = _ldFunction.GetInnerLdHttpHelper().Response.Headers;
                            uploadCompleteHeaders.Add(new HeaderCollection
                            {
                                uploadHeaders = new MediaUploadHeader
                                {
                                    AccessControlAllowOrigin = responseHeader["access-control-allow-origin"],
                                    CacheControl = responseHeader["Cache-Control"],
                                    ContentLength = responseHeader["Content-Length"],
                                    ContentSecurityPolicy = responseHeader["Content-Security-Policy"],
                                    Date = responseHeader["Date"],
                                    Expires = responseHeader["Expires"],
                                    Location = responseHeader["Location"],
                                    Pragma = responseHeader["Pragma"],
                                    StrictTransportSecurity = responseHeader["Strict-Transport-Security"],
                                    XAmbryCreationTime = responseHeader["x-ambry-creation-time"],
                                    XCache = responseHeader["X-Cache"],
                                    XContentTypeOptions = responseHeader["X-Content-Type-Options"],
                                    XFrameOptions = responseHeader["X-Frame-Options"],
                                    XLiFabric = responseHeader["X-Li-Fabric"],
                                    XLIpop = responseHeader["X-Li-Pop"],
                                    XLiProto = responseHeader["X-Li-Proto"],
                                    XLiUuid = responseHeader["X-LI-UUID"],
                                    XMsedgeRef = responseHeader["X-MSEdge-Ref"]
                                }
                            });
                        }
                        var counter = 0;
                        var jobject = handler.ParseJsonToJObject(ExecuteUploadResponse);
                        var urn1 = handler.GetJTokenValue(jobject, "included", 0, "entityUrn");
                        var statusUrl = $"https://www.linkedin.com/voyager/api/graphql?variables=(shareUrn:{WebUtility.UrlEncode(urn1)})&queryId=voyagerContentcreationDashShares.b802f9f485da9e03597508d5fc3d502b";
                        var guid = Utilities.GetGuid();
                    CheckStatus:
                        pageInstance = IspublishedOnpage ? $"urn:li:page:organization_admin_admin_page_posts_published;{guid}" : $"urn:li:page:d_flagship3_feed;{dominatorAccount.CrmUuid}";
                        SetRequestParameterForVideoUpload(dominatorAccount, _ldFunction, referer:refererString, PageInstance:pageInstance , PemMeta: "Voyager - Sharing - ShareStatus=sharing-share-status-polling", IsGet: true);
                        var statusResponse = _ldFunction.GetInnerLdHttpHelper().GetRequest(statusUrl);
                        while(counter++<=15 && (!string.IsNullOrEmpty(statusResponse?.Response) && statusResponse.Response.Contains("\"extensions\"")))
                        {
                            Thread.Sleep(10000);
                            goto CheckStatus;
                        }
                        pageInstance = IspublishedOnpage ? $"urn:li:page:organization_admin_admin_page_posts_published;{guid}" : $"urn:li:page:d_flagship3_feed;{dominatorAccount.CrmUuid}";
                        SetRequestParameterForVideoUpload(dominatorAccount, _ldFunction, referer: "https://www.linkedin.com/feed/", PageInstance: pageInstance,contentType: LdConstants.AcceptApplicationOrJson,UserAgent: "Mozilla/5.0 (X11; Linux i686) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.6599.234 Safari/537.36");
                        var partBody = JsonConvert.SerializeObject(uploadCompleteHeaders);
                        var uploadCompleteBody = $"{{\"completeUploadRequest\":{{\"partUploadResponses\":{partBody},\"mediaArtifactUrn\":\"urn:li:digitalmediaMediaArtifact:({urn},urn:li:digitalmediaMediaArtifactClass:uploadedVideo)\",\"multipartMetadata\":\"\"}}}}";
                        var uploadCompleteResponse = _ldFunction.GetInnerLdHttpHelper().PostRequest("https://www.linkedin.com/voyager/api/voyagerVideoDashMediaUploadMetadata?action=completeMultipartUpload", uploadCompleteBody);
                        if(uploadCompleteResponse?.Response == string.Empty)
                        {
                            jobject = handler.ParseJsonToJObject(ExecuteUploadResponse);
                            singleUploadUrl = handler.GetJTokenValue(jobject, "included",0, "entityUrn")?.Replace("urn:li:fsd_share:", "");
                        }
                    }
                    else
                    {
                        var postResponse = _ldFunction.GetInnerLdHttpHelper().PostRequest(singleUploadUrl,imageByte); ;
                    }
                }
                else
                {
                    UpdatePostWithFailed(DominatorAccountModel.AccountBaseModel.UserName, postDetails,
                        "failed to upload media");
                    GlobusLogHelper.log.Info(Log.ShareFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "own wall", "failed to upload media");
                    return new Tuple<string, string, string, string, bool>("", "", "", "", false);
                }
                if (!IspublishedOnpage)
                    postDataToPostVideo = IsPublishOnOwnWall
                        ? "{\"externalAudienceProviders\":[],\"visibleToConnectionsOnly\":false,\"commentaryV2\":{\"text\":\"" +
                          postDescription +
                          "\",\"attributes\":[]},\"origin\":\"FEED\",\"commentsDisabled\":false,\"media\":[{\"category\":\"VIDEO\",\"mediaUrn\":\"" +
                          mediaUrn +
                          "\",\"recipes\":[\"urn:li:digitalmediaRecipe:feedshare-video-captions-thumbnails-ambry\"]}],\"allowedCommentersScope\":\"ALL\"}"
                        : "{\"visibleToConnectionsOnly\":false,\"commentsDisabled\":false,\"externalAudienceProviders\":[],\"commentaryV2\":{\"text\":\"" + postDescription + "\",\"attributes\":[]},\"containerEntity\":\"urn:li:group:" + groupId + "\",\"origin\":\"CONTAINER_FEED\",\"media\":[{\"category\":\"VIDEO\",\"mediaUrn\":\"" + mediaUrn + "\",\"recipes\":[\"urn:li:digitalmediaRecipe:feedshare-video-captions-thumbnails-ambry\"]}],\"allowedCommentersScope\":\"ALL\"}";


                else
                    postDataToPostVideo =
                        "{\"visibleToConnectionsOnly\":false,\"commentsDisabled\":false,\"externalAudienceProviders\":[],\"commentaryV2\":{\"text\":\"" +
                        postDescription +
                        "\",\"attributes\":[]},\"organizationActor\":\"urn:li:fs_normalized_company:" + groupId +
                        "\",\"origin\":\"ORGANIZATION\",\"allowedCommentersScope\":\"ALL\",\"media\":[{\"category\":\"VIDEO\",\"mediaUrn\":\"" + mediaUrn +
                        "\",\"recipes\":[\"urn:li:digitalmediaRecipe:feedshare-video-captions-thumbnails-ambry\"]}]}";


                return new Tuple<string, string, string, string, bool>(postDataToPostVideo, singleUploadUrl,
                    encryptionId, encryption, isSuccess);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
                return null;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        private void SetRequestParameterForVideoUpload(DominatorAccountModel dominatorAccount, ILdFunctions ldFunction,string referer,string PageInstance="",string PemMeta="",string contentType="",string accept="",bool IsGet=false,string UserAgent="")
        {
            try
            {
                var httpHelper = _ldFunction.GetInnerLdHttpHelper();
                var reqParam = httpHelper.GetRequestParameter();
                reqParam.Headers.Clear();
                var csrf = dominatorAccount.Cookies.Cast<Cookie>().FirstOrDefault(x => x.Name == "JSESSIONID")?.Value;
                reqParam.Headers["x-li-track"] = "{\"clientVersion\":\"1.13.23611\",\"mpVersion\":\"1.13.23611\",\"osName\":\"web\",\"timezoneOffset\":5.5,\"timezone\":\"Asia/Calcutta\",\"deviceFormFactor\":\"DESKTOP\",\"mpName\":\"voyager-web\",\"displayDensity\":1,\"displayWidth\":1366,\"displayHeight\":768}";
                reqParam.Headers["sec-ch-ua"] = "\"(Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"127\", \"Chromium\";v=\"127\"";
                reqParam.Headers["csrf-token"] = csrf?.Replace("\"", "");
                reqParam.Headers["sec-ch-ua-mobile"] = "?0";
                if(!string.IsNullOrEmpty(PageInstance))
                    reqParam.Headers["x-li-page-instance"] = PageInstance;
                reqParam.Headers["x-restli-protocol-version"] = "2.0.0";
                reqParam.Headers["x-li-lang"] = "en_US";
                if(!string.IsNullOrEmpty(PemMeta))
                    reqParam.Headers["x-li-pem-metadata"] = PemMeta;
                reqParam.UserAgent = string.IsNullOrEmpty(UserAgent)? "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.6666.156 Safari/537.36":UserAgent;
                reqParam.Accept = string.IsNullOrEmpty(accept) ? "application/vnd.linkedin.normalized+json+2.1":accept;
                if(!IsGet)
                    reqParam.ContentType = string.IsNullOrEmpty(contentType)? "application/json; charset=UTF-8":contentType;
                reqParam.Headers["Sec-Fetch-Site"] = "same-origin";
                reqParam.Headers["Sec-Fetch-Mode"] = "cors";
                reqParam.Headers["Sec-Fetch-Dest"] = "empty";
                reqParam.Headers["Accept-Encoding"] = "gzip, deflate, br, zstd";
                reqParam.Headers["Accept-Language"] = "en-GB,en-US;q=0.9,en;q=0.8";
                reqParam.Headers["sec-ch-ua-full-version-list"] = "\"(Not(A:Brand\";v=\"99.0.0.0\", \"Google Chrome\";v=\"127\", \"Chromium\";v=\"127\"";
                reqParam.Referer = referer;
                reqParam.Cookies = dominatorAccount.Cookies;
                if (!string.IsNullOrEmpty(dominatorAccount.AccountBaseModel.AccountProxy.ProxyIp) && !string.IsNullOrEmpty(dominatorAccount.AccountBaseModel.AccountProxy.ProxyPort))
                {
                    reqParam.Proxy = new Proxy
                    {
                        ProxyIp = dominatorAccount?.AccountBaseModel?.AccountProxy?.ProxyIp,
                        ProxyPort = dominatorAccount?.AccountBaseModel?.AccountProxy?.ProxyPort,
                        ProxyUsername = dominatorAccount?.AccountBaseModel?.AccountProxy?.ProxyUsername,
                        ProxyPassword = dominatorAccount?.AccountBaseModel?.AccountProxy?.ProxyPassword,
                    };
                }
                httpHelper.SetRequestParameter(reqParam);
            }
            catch { }
        }

        public Tuple<string, string, string, string, bool> GetPostDataToPostDocPdfFiles(ILdFunctions objLdFunctions,
            PublisherPostlistModel postDetails, bool IspublishedOnpage = false,string PageId="")
        {
            try
            {
                #region Initializations required for this method
                Tuple<string, string, string, string, bool> DocUploadAndPostDataTupple = null;
                var isSuccess = true;
                string postDataToPostdoc;
                string mediaUrn;
                string docUploadPostData;
                string singleUploadUrl;
                string encryptionId;
                string encryption;
                var postDescription = postDetails.PostDescription?.Replace("\r", "").Replace("\n", "\\n")
                    .Replace("\"", "\\\"");
                var objDetailedFileInfo = new FileInfo(postDetails.MediaList[0]);
                if (string.IsNullOrEmpty(postDetails.PublisherInstagramTitle))
                    postDetails.PublisherInstagramTitle = objDetailedFileInfo.Name;
                if (postDetails.PublisherInstagramTitle.Length >= 58)
                    postDetails.PublisherInstagramTitle = postDetails.PublisherInstagramTitle.Substring(0, 50);
                #endregion
                var DocUploadUrl = LdConstants.GetLDMediaUploadAPI;
                var imageByte = File.ReadAllBytes(objDetailedFileInfo.FullName.Replace("_SOCINATORIMAGE.jpg", ""));
                if (IspublishedOnpage)
                {
                    docUploadPostData = $"{{\"mediaUploadType\":\"DOCUMENT_SHARING\",\"fileSize\":{imageByte.Length},\"nonMemberActorUrn\":\"urn:li:fsd_company:{PageId}\",\"filename\":\"{objDetailedFileInfo.Name.Replace("_SOCINATORIMAGE.jpg", "")}\"}}";
                }
                else
                    docUploadPostData = "{\"mediaUploadType\":\"DOCUMENT_SHARING\",\"fileSize\":" + imageByte.Length +
                                          ",\"filename\":\"" +
                                          objDetailedFileInfo.Name.Replace("_SOCINATORIMAGE.jpg", "") + "\"}";
                CancellationToken.ThrowIfCancellationRequested();
                var RequestParameter = _ldFunction.GetInnerHttpHelper().GetRequestParameter();
                RequestParameter.Accept = LdConstants.AcceptApplicationOrVndLinkedInMobileDedupedJson21;
                RequestParameter.ContentType = LdConstants.AcceptApplicationOrJson;
                if (RequestParameter.Headers.ToString().Contains("X-RestLi-Protocol-Version"))
                    RequestParameter.Headers.Remove("X-RestLi-Protocol-Version");
                var DocUploadResponse = _ldFunction.GetInnerHttpHelper()
                    .PostRequest(DocUploadUrl, docUploadPostData).Response;

                if (DocUploadResponse.Contains("{\"value\":{\"urn\":\"") ||
                    DocUploadResponse.Contains("urn:li:digitalmediaAsset"))
                {
                    mediaUrn = Utils.GetBetween(DocUploadResponse, "urn\":\"", "\"");
                    var uploadUrlList = new List<string>();
                    var UploadMetaData = GetUploadMetaData(DocUploadResponse);
                    if (string.IsNullOrWhiteSpace(singleUploadUrl =
                        Utils.GetBetween(DocUploadResponse, "singleUploadUrl\":\"", "\"")) && UploadMetaData.Type == UploadType.SINGLE)
                    {
                        var jArray = JObject.Parse(DocUploadResponse);
                        var jsonArrayHandler = JsonJArrayHandler.GetInstance;

                        var list = jsonArrayHandler.GetJTokenOfJToken(jArray, "value", "partUploadRequests");
                        foreach (var item in list)
                            uploadUrlList.Add(jsonArrayHandler.GetJTokenValue(item, "uploadUrl"));
                        _isBrowser = true;
                        LoginProcess();
                        _ldFunction = _ldFunctionFactory.LdFunctions;
                    }
                    else
                    {
                        if(!string.IsNullOrEmpty(singleUploadUrl))
                            uploadUrlList.Add(singleUploadUrl);
                    }

                    singleUploadUrl = JsonConvert.SerializeObject(uploadUrlList);
                    encryptionId = Utils.GetBetween(DocUploadResponse,
                        "x-amz-server-side-encryption-aws-kms-key-id\":\"", "\"");
                    encryption = Utils.GetBetween(DocUploadResponse, "x-amz-server-side-encryption\":\"", "\"");
                    
                }
                else
                {
                    UpdatePostWithFailed(DominatorAccountModel.AccountBaseModel.UserName, postDetails,
                        "failed to upload media");
                    GlobusLogHelper.log.Info(Log.ShareFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "own wall", "failed to upload media");
                    return new Tuple<string, string, string, string, bool>("", "", "", "", false);
                }

                if (!IspublishedOnpage)
                    postDataToPostdoc = $"{{\"visibleToConnectionsOnly\":false,\"externalAudienceProviders\":[],\"commentaryV2\":{{\"text\":\"{postDetails.PostDescription}\",\"attributes\":[]}},\"origin\":\"FEED\",\"allowedCommentersScope\":\"ALL\",\"postState\":\"PUBLISHED\",\"media\":[{{\"category\":\"NATIVE_DOCUMENT\",\"mediaUrn\":\"{mediaUrn}\",\"title\":{{\"text\":\"{postDetails.PublisherInstagramTitle?.Replace("\"", "\\\"")}\"}},\"recipes\":[\"urn:li:digitalmediaRecipe:feedshare-document-preview\",\"urn:li:digitalmediaRecipe:feedshare-document\"]}}]}}";
                else
                {
                    postDataToPostdoc = $"{{\"visibleToConnectionsOnly\":false,\"externalAudienceProviders\":[],\"commentaryV2\":{{\"text\":\"{postDetails.PostDescription}\",\"attributes\":[]}},\"origin\":\"ORGANIZATION\",\"allowedCommentersScope\":\"ALL\",\"postState\":\"PUBLISHED\",\"media\":[{{\"category\":\"NATIVE_DOCUMENT\",\"mediaUrn\":\"{mediaUrn}\",\"title\":{{\"text\":\"{postDetails.PublisherInstagramTitle?.Replace("\"","\\\"")}\"}},\"recipes\":[\"urn:li:digitalmediaRecipe:feedshare-document-preview\",\"urn:li:digitalmediaRecipe:feedshare-document\"]}}],\"dashNonMemberActor\":\"urn:li:fsd_company:{PageId}\"}}";
                }
                return new Tuple<string, string, string, string, bool>(postDataToPostdoc, singleUploadUrl,
                    encryptionId, encryption, isSuccess);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
                return null;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        private void SetparametersForUploadVideo(string groupId)
        {
            var responseParameter = _ldFunction.GetInnerHttpHelper().GetRequest($"https://www.linkedin.com/groups/{groupId}");
            var pageText = responseParameter.Response;
            pageText = HttpUtility.HtmlDecode(pageText);
            var dFlagship3SearchSrpPeople = Utils.GetBetween(pageText, "d_flagship3_groups_entity;", "<").TrimEnd();
            _ldFunction.SetRequestParameterForUploadVideo($"https://www.linkedin.com/groups/{groupId}", dFlagship3SearchSrpPeople);
        }

        /// <summary>
        ///     video uploading in bytes
        /// </summary>
        /// <param name="objLdFunctions"></param>
        /// <param name="postDetails"></param>
        /// <param name="actionUrl"></param>
        /// <param name="postDataToUploadVideo"></param>
        /// <param name="singleUploadUrl"></param>
        /// <param name="encryptionId"></param>
        /// <param name="encryption"></param>
        /// <returns> video response after successfully uploading</returns>
        public Tuple<string, string> GetFinalResponseForVideoPost(ILdFunctions objLdFunctions,
            PublisherPostlistModel postDetails, string actionUrl, string postDataToUploadVideo, string singleUploadUrl,
            string encryptionId, string encryption)
        {
            try
            {
                Tuple<string, string> finalResponseTuple = null;
                var objDetailedFileInfo = new FileInfo(postDetails.MediaList?.FirstOrDefault());
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, "Publish", "please wait while uploading media.");
                var finalResponse =
                    objLdFunctions.FinalPostRequest_VideoUploading(actionUrl, postDataToUploadVideo);
                var postUrn = Utils.GetBetween(finalResponse, "urn\":\"", "\"");
                VideoPutReponse(objLdFunctions, singleUploadUrl, objDetailedFileInfo);
                var uploadActionUrl = $"{LdConstants.GetMediaShareAPI}/{postUrn}/status";
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var isFirst = true;
                while (finalResponse.Contains("PROCESSING"))
                {
                    finalResponse = objLdFunctions.GetVideoUploadResponseStatus(uploadActionUrl);
                    if (stopWatch.Elapsed.TotalSeconds > 90 && isFirst)
                    {
                        isFirst = false;
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, "Publish",
                            "it seems processing media after uploading taking time.");
                    }

                    _delayService.ThreadSleep(15000);
                    if (stopWatch.Elapsed.Minutes > 10)
                        break;
                }

                stopWatch.Stop();
                finalResponseTuple = new Tuple<string, string>(finalResponse, string.Empty);
                return finalResponseTuple;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new Tuple<string, string>(string.Empty, "failed to upload video");
            }
        }

        /// <summary>
        /// This method is use to upload doc,pdf files
        /// </summary>
        /// <param name="objLdFunctions"></param>
        /// <param name="postDetails"></param>
        /// <param name="actionUrl"></param>
        /// <param name="postDataToUploadVideo"></param>
        /// <param name="singleUploadUrl"></param>
        /// <param name="encryptionId"></param>
        /// <param name="encryption"></param>
        /// <returns></returns>
        public Tuple<string, string> GetFinalResponseForDocumentPost(ILdFunctions objLdFunctions,
            PublisherPostlistModel postDetails, string actionUrl, string postDataToUploadVideo, string singleUploadUrl,
            string encryptionId, string encryption)
        {
            try
            {
                Tuple<string, string> finalResponseTuple = null;

                var dFlagship3Feed = string.Empty;

                if (IsPublishOnOwnWall)
                    try
                    {
                        var feedPageResponse = string.Empty;
                        var feedUrl = "https://www.linkedin.com/feed/";
                        feedPageResponse =
                            HttpUtility.HtmlDecode(_ldFunction.GetInnerHttpHelper().GetRequest(feedUrl).Response);
                        dFlagship3Feed = Utils.GetBetween(feedPageResponse, "urn:li:page:d_flagship3_feed;", "\n");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                var objDetailedFileInfo = new FileInfo(postDetails.MediaList[0]);
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, "Publish", "please wait while uploading media.");
                
                var mediaasset = Utils.GetBetween(postDataToUploadVideo, "urn:li:digitalmediaAsset:", "\",\"title\":");
                DocPdfPutReponse(objLdFunctions, singleUploadUrl, objDetailedFileInfo);
                var uploadActionUrl = $"https://www.linkedin.com/voyager/api/voyagerVideoDashMediaAssetStatus/urn%3Ali%3AdigitalmediaAsset%3A{mediaasset}?mediaStatusType=DOCUMENT_PREVIEW";
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var isFirst = true;
                var finalResponse = objLdFunctions.GetDocfileUploadResponseStatus(uploadActionUrl, dFlagship3Feed);
                while (finalResponse.Contains("PROCESSING") && !finalResponse.Contains("PROCESSING_FAILED"))
                {
                    //Response = objLDFunctions.GetUploadResponse(SingleUploadUrl, EncryptionId, Encryption, objDetailedFileInfo);
                    finalResponse = objLdFunctions.GetDocfileUploadResponseStatus(uploadActionUrl, dFlagship3Feed);
                    if (stopWatch.Elapsed.TotalSeconds > 90 && isFirst)
                    {
                        isFirst = false;
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, "Publish",
                            "it seems processing media after uploading taking time.");
                    }

                    _delayService.ThreadSleep(15000);
                    if (stopWatch.Elapsed.Minutes > 10)
                        break;
                }

                stopWatch.Stop();
                finalResponse =
                    objLdFunctions.FinalPostRequest_DocumentUploading(actionUrl, postDataToUploadVideo, dFlagship3Feed);
                finalResponseTuple = new Tuple<string, string>(finalResponse, string.Empty);
                return finalResponseTuple;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new Tuple<string, string>(string.Empty, "failed to upload File");
            }
        }

        private static void VideoPutReponse(ILdFunctions objLdFunctions, string singleUploadUrl, FileInfo objDetailedFileInfo)
        {
            var imageByte = File.ReadAllBytes(objDetailedFileInfo.FullName.Replace("_SOCINATORIMAGE.jpg", ""));
            var list = JsonConvert.DeserializeObject<List<string>>(singleUploadUrl);
            var putResponse = objLdFunctions.Getputresponse(list.FirstOrDefault(), imageByte);
        }
        /// <summary>
        /// this method is use to upload doc and pdf format
        /// </summary>
        /// <param name="objLdFunctions"></param>
        /// <param name="singleUploadUrl"></param>
        /// <param name="objDetailedFileInfo"></param>
        private static void DocPdfPutReponse(ILdFunctions objLdFunctions, string singleUploadUrl, FileInfo objDetailedFileInfo)
        {
            var fileContentType = MediaUtilites.GetMimeTypeByFilePath(objDetailedFileInfo.FullName);
            var imageByte = File.ReadAllBytes(objDetailedFileInfo.FullName.Replace("_SOCINATORIMAGE.jpg", ""));
            var list = JsonConvert.DeserializeObject<List<string>>(singleUploadUrl);
            var putResponse = objLdFunctions.GetputresponseforDocPdfFiles(list[0], imageByte,fileContentType);
        }

        public string UploadSingleImage(ILdFunctions objLdFunctions, PublisherPostlistModel postDetails)
        {
            var postResponse = string.Empty;
            try
            {
                var postDescription = postDetails.PostDescription.Replace("\"", "\\\"").Replace("\r", "\\r")
                    .Replace("\n", "\\n").Replace("'", "").Replace("’", "");
                var url = LdConstants.GetLDMediaUploadAPI;
                var file = new FileInfo(postDetails.MediaList.FirstOrDefault());
                var mediaType = MediaUtilites.GetMimeTypeByFilePath(postDetails.MediaList.FirstOrDefault());
                var IsVideo = string.IsNullOrEmpty(mediaType) ? false : ConstantVariable.SupportedVideoFormat.Contains(file.Extension?.Replace(".", ""));
                var uploadType = !IsVideo ? "IMAGE_SHARING" : "VIDEO_SHARING";
                var postData = $"{{\"mediaUploadType\":\"{uploadType}\",\"fileSize\":{file.Length},\"filename\":\"{file.Name}\"}}";
                var resp = objLdFunctions.GetInnerHttpHelper().PostRequest(url, postData).Response;
                var singleUploadUrl = Utilities.GetBetween(resp, "singleUploadUrl\":\"", "\"");
                var digitalmediaAsset = Utilities.GetBetween(resp, "urn:li:digitalmediaAsset:", "\"");
                var byteData = File.ReadAllBytes(file.FullName);
                var requestParameters =
                    objLdFunctions.GetInnerHttpHelper().GetRequestParameter();
                var extension = file.Extension.Equals(".jpg") ? "jpeg" : file.Extension.Replace(".", "");
                var lastContentType = requestParameters.ContentType;
                requestParameters.ContentType = !IsVideo ? $"image/{extension}":$"video/{extension}";
                var UploadMetaData = GetUploadMetaData(resp);
                if(UploadMetaData.Type == UploadType.SINGLE)
                {
                    objLdFunctions.GetInnerHttpHelper().PostRequest(singleUploadUrl, byteData);
                }
                else
                {
                    foreach(var data in UploadMetaData.MultiUploadData)
                    {
                        requestParameters.ContentType = data.ContentType;
                        var byteDataUpload = byteData.Skip(data.firstByte).Take(data.lastByte-data.firstByte).ToArray();
                        var response = objLdFunctions.GetInnerLdHttpHelper().PutRequest(data.UploadUrl, byteDataUpload);
                    }
                }
                
                var afterUploadUrl = LdConstants.GetMediaShareAPI;
                var category = IsVideo ? "VIDEO" : "IMAGE";
                var afterUploadPostData =
                    "{\"externalAudienceProviders\":[],\"visibleToConnectionsOnly\":false,\"commentary\":{\"text\":\"" +
                    postDescription +
                    "\",\"attributes\":[]},\"commentsDisabled\":false,\"media\":[{\"mediaUrn\":\"urn:li:digitalmediaAsset:" +
                    digitalmediaAsset + $"\",\"category\":\"{category}\"}}]}}";
                postResponse = objLdFunctions.GetInnerHttpHelper().PostRequest(afterUploadUrl, afterUploadPostData)
                    .Response;
                requestParameters.ContentType = lastContentType;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return postResponse;
        }

        public string UploadMultipleImage(ILdFunctions objLdFunctions, PublisherPostlistModel postDetails)
        {
            var postResponse = string.Empty;
            try
            {
                DominatorHouseCore.Interfaces.IRequestParameters requestParameters = null;
                var lastContentType = "";
                var postDescription = postDetails.PostDescription.Replace("\"", "\\\"").Replace("\r", "")
                    .Replace("\n", "\\n").Replace("'", "").Replace("’", "");
                postDescription = postDescription.Replace("\\n","\n");
                var url = LdConstants.GetLDMediaUploadAPI;
                var images = new JArray();
                foreach (var imagepath in postDetails.MediaList)
                {

                    var file = new FileInfo(imagepath);
                    var mediaType = MediaUtilites.GetMimeTypeByFilePath(imagepath);
                    var IsVideo = string.IsNullOrEmpty(mediaType) ? false : ConstantVariable.SupportedVideoFormat.Contains(file.Extension?.Replace(".", ""));
                    var uploadType = !IsVideo ? "IMAGE_SHARING" : "VIDEO_SHARING";
                    var postData =$"{{\"mediaUploadType\":\"{uploadType}\",\"fileSize\":{file.Length},\"filename\":\"{file.Name}\"}}";
                    var resp = objLdFunctions.GetInnerHttpHelper().PostRequest(url, postData).Response;
                    var singleUploadUrl = Utilities.GetBetween(resp, "singleUploadUrl\":\"", "\"");
                    var digitalmediaAsset = Utilities.GetBetween(resp, "urn:li:digitalmediaAsset:", "\"");
                    var byteData = File.ReadAllBytes(file.FullName);
                    dynamic media = new JObject();
                    media.category =!IsVideo? "IMAGE":"VIDEO";
                    media.mediaUrn = $"urn:li:digitalmediaAsset:{digitalmediaAsset}";
                    images.Add(media);
                    requestParameters =
                       objLdFunctions.GetInnerHttpHelper().GetRequestParameter();
                    var extension = file.Extension.Equals(".jpg") ? "jpeg" : file.Extension.Replace(".", "");

                    if (string.IsNullOrEmpty(extension))
                        extension =IsVideo?"mp4":"jpeg";
                    lastContentType = requestParameters.ContentType;
                    var UploadMetaData = GetUploadMetaData(resp);
                    if(UploadMetaData.Type == UploadType.SINGLE)
                    {
                        requestParameters.ContentType = string.IsNullOrEmpty(mediaType) ? IsVideo ? $"video/{extension}" : $"image/{extension}" : mediaType;
                        postResponse = objLdFunctions.GetInnerHttpHelper().PostRequest(UploadMetaData.SingleUploadUrl, byteData).Response;
                    }
                    else
                    {
                        foreach(var data in UploadMetaData.MultiUploadData)
                        {
                            requestParameters.ContentType = data.ContentType;
                            var byteDataToUpload = byteData.Skip(data.firstByte).Take(data.lastByte-data.firstByte).ToArray();
                            postResponse = objLdFunctions.GetInnerLdHttpHelper().PutRequest(data.UploadUrl, byteDataToUpload).Response;
                        }
                    }
                }
                var afterUploadUrl = LdConstants.GetMediaShareAPI;
                var afterUploadPostData = new LDModel.LdJsonElements
                {
                    VisibleToConnectionsOnly = false,
                    CommentsDisabled = false,
                    ExternalAudienceProviders = new List<LDModel.LdJsonElements>(),
                   
                    Commentary = new LDModel.LdJsonElements
                    {
                        Text = postDescription,
                        Attributes = new List<LDModel.LdJsonElements>(),
                    },
                    origin="FEED",
                };
                afterUploadPostData.Media = images;
                var postdata = GeneratePostBody(afterUploadPostData);
                postResponse = objLdFunctions.GetInnerHttpHelper().PostRequest(afterUploadUrl, postdata)
                    .Response;
                requestParameters.ContentType = lastContentType;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return postResponse;
        }

        private MediaUploadResponseHandler GetUploadMetaData(string resp)
        {
            var response = new MediaUploadResponseHandler();
            try
            {
                var jObject = handler.ParseJsonToJObject(resp);
                var type = handler.GetJTokenValue(jObject, "data", "value", "type");
                type = string.IsNullOrEmpty(type) ? handler.GetJTokenValue(jObject, "value", "type") : type;
                if (!string.IsNullOrEmpty(type))
                {
                    response.Type = type == "SINGLE"? UploadType.SINGLE:UploadType.MULTIPART;
                    if(response.Type == UploadType.SINGLE)
                    {
                        var uploadUrl = handler.GetJTokenValue(jObject, "data", "value", "singleUploadUrl");
                        uploadUrl = string.IsNullOrEmpty(uploadUrl) ? handler.GetJTokenValue(jObject, "value", "singleUploadUrl") : uploadUrl;
                        response.SingleUploadUrl = uploadUrl;
                        var statusUrl = handler.GetJTokenValue(jObject, "data", "value", "pollingUrl");
                        statusUrl = string.IsNullOrEmpty(statusUrl) ? handler.GetJTokenValue(jObject, "value", "pollingUrl") : statusUrl;
                        response.StatusUrl = statusUrl;
                    }
                    else
                    {
                        response.SingleUploadUrl = string.Empty;
                        response.StatusUrl = handler.GetJTokenValue(jObject, "data", "value", "pollingUrl");
                        var multipartNodes = handler.GetJArrayElement(handler.GetJTokenValue(jObject, "data", "value", "partUploadRequests"));
                        if(multipartNodes != null && multipartNodes.HasValues)
                        {
                            multipartNodes.ForEach(node =>
                            {
                                try
                                {
                                    int.TryParse(handler.GetJTokenValue(node, "lastByte"), out int LastByte);
                                    long.TryParse(handler.GetJTokenValue(node, "urlExpiresAt"), out long UrlExpiresAt);
                                    int.TryParse(handler.GetJTokenValue(node, "firstByte"), out int FirstByte);
                                    response.MultiUploadData.Add(new MultiPartData
                                    {
                                        ContentType = handler.GetJTokenValue(node, "headers", "Content-Type"),
                                        UploadUrl = handler.GetJTokenValue(node, "uploadUrl"),
                                        lastByte = LastByte,
                                        urlExpire = UrlExpiresAt,
                                        firstByte = FirstByte,
                                        type = handler.GetJTokenValue(node, "$type")
                                    });
                                }catch{ }
                            });
                        }
                    }
                }
            }
            catch { }
            return response;
        }

        public string UploadSingleImageGroup(ILdFunctions objLdFunctions, PublisherPostlistModel postDetails,
            string groupUrl)
        {
            var postResponse = string.Empty;
            try
            {
                var groupId = Utils.GetBetween(groupUrl + "$$", "groups/", "$$").Replace("/", "");
                var postDescription = postDetails.PostDescription.Replace("\"", "\\\"").Replace("\r", "\\r")
                    .Replace("\n", "\\n").Replace("'", "").Replace("’", "");
                var url = LdConstants.GetLDMediaUploadAPI;
                
                var file = new FileInfo(postDetails.MediaList[0]);
                var IsDoc = !string.IsNullOrEmpty(file.Extension) && file.Extension.Contains("pdf");
                var mediaUploadType = IsDoc ? "DOCUMENT_SHARING" : "IMAGE_SHARING";
                var postData =
                    "{\"mediaUploadType\":\""+mediaUploadType+"\",\"fileSize\":" + file.Length + ",\"filename\":\"" +
                    file.Name + "\"}";

                var resp = objLdFunctions.GetInnerHttpHelper().PostRequest(url, postData).Response;

                var singleUploadUrl = Utilities.GetBetween(resp, "singleUploadUrl\":\"", "\"");
                var digitalmediaAsset = Utilities.GetBetween(resp, "urn:li:digitalmediaAsset:", "\"");
                var byteData = File.ReadAllBytes(file.FullName);

                var requestParameters =
                    objLdFunctions.GetInnerHttpHelper().GetRequestParameter();
                var extension = file.Extension.Equals(".jpg") ? "jpeg" : file.Extension.Replace(".", "");
                var lastContentType = requestParameters.ContentType;
                requestParameters.ContentType = IsDoc ? $"application/{extension}": $"image/{extension}";
                objLdFunctions.GetInnerHttpHelper().PostRequest(singleUploadUrl, byteData);
                var afterUploadUrl = LdConstants.GetMediaShareAPI;
                var category = IsDoc ? "NATIVE_DOCUMENT" : "IMAGE";
                var title = string.IsNullOrEmpty(postDetails.PublisherInstagramTitle) ? file?.Name?.Replace(extension, "")?.Replace(".", ""): postDetails.PublisherInstagramTitle;
                var afterUploadPostData = IsDoc ?
                    $"{{\"visibleToConnectionsOnly\":false,\"externalAudienceProviders\":[],\"commentaryV2\":{{\"text\":\"{postDescription}\",\"attributes\":[]}},\"containerEntity\":\"urn:li:fsd_group:{groupId}\",\"origin\":\"CONTAINER_FEED\",\"allowedCommentersScope\":\"ALL\",\"postState\":\"PUBLISHED\",\"media\":[{{\"category\":\"{category}\",\"mediaUrn\":\"urn:li:digitalmediaAsset:{digitalmediaAsset}\",\"title\":{{\"text\":\"{title?.Replace("\"", "\\\"")}\"}},\"recipes\":[\"urn:li:digitalmediaRecipe:feedshare-document-preview\",\"urn:li:digitalmediaRecipe:feedshare-document\"]}}]}}"
                    :"{\"visibleToConnectionsOnly\":false,\"externalAudienceProviders\":[],\"commentaryV2\":{\"text\":\"" +
                    postDescription + "\",\"attributes\":[]},\"containerEntity\":\"urn:li:group:" + groupId +
                    "\",\"origin\":\"CONTAINER_FEED\",\"allowedCommentersScope\":\"ALL\",\"postState\":\"PUBLISHED\",\"media\":[{\"category\":\""+category+"\",\"mediaUrn\":\"urn:li:digitalmediaAsset:" + digitalmediaAsset +
                    "\",\"tapTargets\":[]}]}";
                requestParameters.Referer = groupUrl;
                if (!requestParameters.Headers.ToString().Contains("X-RestLi-Protocol-Version"))
                    requestParameters.Headers.Add("X-RestLi-Protocol-Version", "2.0.0");
                requestParameters.ContentType = "application/json; charset=UTF-8";
                requestParameters.Accept = "application/vnd.linkedin.normalized+json+2.1";
                postResponse = objLdFunctions.GetInnerHttpHelper().PostRequest(afterUploadUrl, afterUploadPostData)
                    .Response;
                if(string.IsNullOrEmpty(postResponse))
                {
                    if (requestParameters.Headers.ToString().Contains("X-LI-Track"))
                        requestParameters.Headers.Remove("X-LI-Track");
                    postResponse = objLdFunctions.GetInnerHttpHelper().PostRequest(afterUploadUrl, afterUploadPostData)
                    .Response;
                }
                requestParameters.ContentType = lastContentType;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return postResponse;
        }

        public string UploadSingleImagePage(ILdFunctions objLdFunctions, PublisherPostlistModel postDetails,
            string pageUrl)
        {
            var postResponse = string.Empty;
            try
            {
                var pageId = Utils.GetBetween(pageUrl + "$$", "company/", "$$").Replace("/", "");
                var postDescription = postDetails.PostDescription.Replace("\"", "\\\"").Replace("\r", "\\r")
                    .Replace("\n", "\\n").Replace("'", "").Replace("’", "");
                var url = LdConstants.GetLDMediaUploadAPI;
                var file = new FileInfo(postDetails.MediaList[0]); //postDetails.MediaList[0]
                //var postData ="{\"mediaUploadType\":\"IMAGE_SHARING\",\"fileSize\":" + file.Length +",\"organizationActor\":\"urn:li:fs_normalized_company:" + pageId + "\",\"filename\":\"" +file.Name + "\"}";
                var postData = $"{{\"mediaUploadType\":\"IMAGE_SHARING\",\"fileSize\":{file.Length},\"nonMemberActorUrn\":\"urn:li:fsd_company:{pageId}\",\"filename\":\"{file.Name}\"}}";
                var resp = objLdFunctions.GetInnerHttpHelper().PostRequest(url, postData).Response;
                var singleUploadUrl = Utilities.GetBetween(resp, "singleUploadUrl\":\"", "\"");
                var digitalmediaAsset = Utilities.GetBetween(resp, "urn:li:digitalmediaAsset:", "\"");
                var byteData = File.ReadAllBytes(file.FullName);
                var requestParameters =
                    objLdFunctions.GetInnerHttpHelper().GetRequestParameter();
                var extension = file.Extension.Equals(".jpg") ? "jpeg" : file.Extension.Replace(".", "");
                var lastContentType = requestParameters.ContentType;
                requestParameters.ContentType = $"image/{extension}";
                objLdFunctions.GetInnerHttpHelper().PostRequest(singleUploadUrl, byteData);
                var afterUploadUrl = LdConstants.GetMediaShareAPI;
                //var afterUploadPostData ="{\"visibleToConnectionsOnly\":false,\"commentsDisabled\":false,\"externalAudienceProviders\":[],\"commentary\":{\"text\":\"" +postDescription + "\",\"attributes\":[]},\"organizationActor\":\"urn:li:fs_normalized_company:" +pageId +"\",\"origin\":\"ORGANIZATION\",\"media\":[{\"category\":\"IMAGE\",\"mediaUrn\":\"urn:li:digitalmediaAsset:" +digitalmediaAsset + "\",\"tapTargets\":[]}]}";
                var afterUploadPostData = $"{{\"visibleToConnectionsOnly\":false,\"externalAudienceProviders\":[],\"commentaryV2\":{{\"text\":\"{postDescription}\",\"attributes\":[]}},\"origin\":\"ORGANIZATION\",\"allowedCommentersScope\":\"ALL\",\"postState\":\"PUBLISHED\",\"media\":[{{\"category\":\"IMAGE\",\"mediaUrn\":\"urn:li:digitalmediaAsset:{digitalmediaAsset}\",\"tapTargets\":[]}}],\"dashNonMemberActor\":\"urn:li:fsd_company:{pageId}\"}}";
                requestParameters.Referer = pageUrl;
                if (!requestParameters.Headers.ToString().Contains("X-RestLi-Protocol-Version"))
                    requestParameters.Headers.Add("X-RestLi-Protocol-Version", "2.0.0");
                requestParameters.ContentType = "application/json; charset=UTF-8";
                requestParameters.Accept = "application/vnd.linkedin.normalized+json+2.1";
                postResponse = objLdFunctions.GetInnerHttpHelper().PostRequest(afterUploadUrl, afterUploadPostData)
                    .Response;
                requestParameters.ContentType = lastContentType;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return postResponse;
        }
        public string UploadMultipleImagePage(ILdFunctions objLdFunctions, PublisherPostlistModel postDetails,
            string pageUrl)
        {
            var postResponse = string.Empty;
            try
            {
                DominatorHouseCore.Interfaces.IRequestParameters requestParameters = null;
                var lastContentType = "";
                var pageId = Utils.GetBetween(pageUrl + "$$", "company/", "$$").Replace("/", "");
                var postDescription = postDetails.PostDescription.Replace("\"", "\\\"").Replace("\r", "\\r")
                    .Replace("\n", "\\n").Replace("'", "").Replace("’", "");
                var url = LdConstants.GetLDMediaUploadAPI;
                var listofmedia = new List<string>();
                foreach (var imagepath in postDetails.MediaList)
                {
                    var file = new FileInfo(imagepath); //postDetails.MediaList[0]
                    var postData =
                        "{\"mediaUploadType\":\"IMAGE_SHARING\",\"fileSize\":" + file.Length +
                        ",\"organizationActor\":\"urn:li:fs_normalized_company:" + pageId + "\",\"filename\":\"" +
                        file.Name + "\"}";
                    //  "{\"mediaUploadType\":\"IMAGE_SHARING\",\"fileSize\":" + file.Length + ",\"filename\":\"" + file.Name + "\"}";

                    var resp = objLdFunctions.GetInnerHttpHelper().PostRequest(url, postData).Response;

                    var singleUploadUrl = Utilities.GetBetween(resp, "singleUploadUrl\":\"", "\"");
                    var digitalmediaAsset = Utilities.GetBetween(resp, "urn:li:digitalmediaAsset:", "\"");
                    var byteData = File.ReadAllBytes(file.FullName);
                    listofmedia.Add(digitalmediaAsset);
                    requestParameters =
                        objLdFunctions.GetInnerHttpHelper().GetRequestParameter();
                    var extension = file.Extension.Equals(".jpg") ? "jpeg" : file.Extension.Replace(".", "");
                    lastContentType = requestParameters.ContentType;
                    requestParameters.ContentType = $"image/{extension}";
                    objLdFunctions.GetInnerHttpHelper().PostRequest(singleUploadUrl, byteData);
                }
                var afterUploadUrl = LdConstants.GetMediaShareAPI;

                var afterUploadPostData = new LDModel.LdJsonElements
                {

                    VisibleToConnectionsOnly = false,
                    CommentsDisabled = false,
                    ExternalAudienceProviders = new List<LDModel.LdJsonElements>(),
                    Commentary = new LDModel.LdJsonElements
                    {
                        Text = postDescription,
                        Attributes = new List<LDModel.LdJsonElements>(),
                    },
                    OrganizationActor = $"urn:li: fs_normalized_company:{pageId}",
                    origin = "ORGANIZATION",
                };
                var images = new JArray();
                foreach (var mediaid in listofmedia)
                {
                    dynamic media = new JObject();
                    media.category = "IMAGE";
                    media.mediaUrn = $"urn:li:digitalmediaAsset:{mediaid}";
                    images.Add(media);
                }
                afterUploadPostData.Media = images;
                var postdata = GeneratePostBody(afterUploadPostData);
                //"{\"externalAudienceProviders\":[],\"visibleToConnectionsOnly\":false,\"commentary\":{\"text\":\"" + postDescription + "\",\"attributes\":[]},\"commentsDisabled\":false,\"containerEntity\":\"urn:li:group:" + groupId + "\",\"media\":[{\"mediaUrn\":\"urn:li:digitalmediaAsset:" + digitalmediaAsset + "\",\"category\":\"IMAGE\"}]}";

                requestParameters.Referer = pageUrl;
                if (!requestParameters.Headers.ToString().Contains("X-RestLi-Protocol-Version"))
                    requestParameters.Headers.Add("X-RestLi-Protocol-Version", "2.0.0");
                requestParameters.ContentType = "application/json; charset=UTF-8";
                requestParameters.Accept = "application/vnd.linkedin.normalized+json+2.1";
                postResponse = objLdFunctions.GetInnerHttpHelper().PostRequest(afterUploadUrl, postdata)
                    .Response;
                requestParameters.ContentType = lastContentType;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return postResponse;
        }
        public string UploadMultipleImageGroup(ILdFunctions objLdFunctions, PublisherPostlistModel postDetails,
            string groupUrl)
        {
            var postResponse = string.Empty;
            try
            {
                DominatorHouseCore.Interfaces.IRequestParameters requestParameters = null;
                var lastContentType = "";
                var groupId = Utils.GetBetween(groupUrl + "$$", "groups/", "$$").Replace("/", "");
                var postDescription = postDetails.PostDescription.Replace("\"", "\\\"").Replace("\r", "\\r")
                    .Replace("\n", "\\n").Replace("'", "").Replace("’", "");
                var url = LdConstants.GetLDMediaUploadAPI;
                var listofmedia = new List<string>();
                foreach (var imagepath in postDetails.MediaList)
                {
                    var file = new FileInfo(imagepath); //postDetails.MediaList[0]
                    var postData =
                        "{\"mediaUploadType\":\"IMAGE_SHARING\",\"fileSize\":" + file.Length + ",\"filename\":\"" +
                        file.Name + "\"}";

                    var resp = objLdFunctions.GetInnerHttpHelper().PostRequest(url, postData).Response;
                    var singleUploadUrl = Utilities.GetBetween(resp, "singleUploadUrl\":\"", "\"");
                    var digitalmediaAsset = Utilities.GetBetween(resp, "urn:li:digitalmediaAsset:", "\"");
                    listofmedia.Add(digitalmediaAsset);
                    var byteData = File.ReadAllBytes(file.FullName);

                    requestParameters =
                        objLdFunctions.GetInnerHttpHelper().GetRequestParameter();
                    var extension = file.Extension.Equals(".jpg") ? "jpeg" : file.Extension.Replace(".", "");
                    lastContentType = requestParameters.ContentType;
                    requestParameters.ContentType = $"image/{extension}";
                    objLdFunctions.GetInnerHttpHelper().PostRequest(singleUploadUrl, byteData);
                }
                var afterUploadUrl = LdConstants.GetMediaShareAPI;

                var afterUploadPostData = new LDModel.LdJsonElements
                {
                    ExternalAudienceProviders = new List<LDModel.LdJsonElements>(),
                    VisibleToConnectionsOnly = false,
                    Commentary = new LDModel.LdJsonElements
                    {
                        Text = postDescription,
                        Attributes = new List<LDModel.LdJsonElements>(),
                    },
                    CommentsDisabled = false,
                    ContainerEntity = $"urn:li:group:{groupId}",
                };

                var images = new JArray();
                foreach (var mediaid in listofmedia)
                {
                    dynamic media = new JObject();
                    media.mediaUrn = $"urn:li:digitalmediaAsset:{mediaid}";
                    media.category = "IMAGE";
                    images.Add(media);
                }
                afterUploadPostData.Media = images;
                var postdata = GeneratePostBody(afterUploadPostData);
                requestParameters.Referer = groupUrl;
                if (!requestParameters.Headers.ToString().Contains("X-RestLi-Protocol-Version"))
                    requestParameters.Headers.Add("X-RestLi-Protocol-Version", "2.0.0");
                requestParameters.ContentType = "application/json; charset=UTF-8";
                requestParameters.Accept = "application/vnd.linkedin.normalized+json+2.1";
                postResponse = objLdFunctions.GetInnerHttpHelper().PostRequest(afterUploadUrl, postdata)
                  .Response;
                requestParameters.ContentType = lastContentType;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return postResponse;
        }

        public string GeneratePostBody(LDModel.LdJsonElements elements)
        {
            Body = elements;
            var str = Body == null
                ? null
                : JsonConvert.SerializeObject(Body, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            return str;
        }


        public string PostWithLink(LdFunctions objLdFunctions, PublisherPostlistModel postDetails)
        {
            var postResponse = string.Empty;
            try
            {
                var postDescription = postDetails.PostDescription.Replace("\"", "\\\"").Replace("\r", "\\r")
                    .Replace("\n", "\\n").Replace("'", "").Replace("’", "");
                var url = LdConstants.GetLDMediaUploadAPI;
                var file = new FileInfo(postDetails.MediaList[0]); //postDetails.MediaList[0]
                var postData =
                    "{\"mediaUploadType\":\"IMAGE_SHARING\",\"fileSize\":" + file.Length + ",\"filename\":\"" +
                    file.Name + "\"}";
                var resp = objLdFunctions.GetInnerHttpHelper().PostRequest(url, postData).Response;

                var singleUploadUrl = Utilities.GetBetween(resp, "singleUploadUrl\":\"", "\"");
                var digitalmediaAsset = Utilities.GetBetween(resp, "urn:li:digitalmediaAsset:", "\"");
                var byteData = File.ReadAllBytes(file.FullName);

                var requestParameters =
                    objLdFunctions.GetInnerHttpHelper().GetRequestParameter();
                var extension = file.Extension.Equals(".jpg") ? "jpeg" : file.Extension.Replace(".", "");
                var lastContentType = requestParameters.ContentType;
                requestParameters.ContentType = $"image/{extension}";
                objLdFunctions.GetInnerHttpHelper().PostRequest(singleUploadUrl, byteData);
                var afterUploadUrl = LdConstants.GetMediaShareAPI;
                var afterUploadPostData =
                    "{\"externalAudienceProviders\":[],\"visibleToConnectionsOnly\":false,\"commentary\":{\"text\":\"" +
                    postDescription +
                    "\",\"attributes\":[]},\"commentsDisabled\":false,\"media\":[{\"mediaUrn\":\"urn:li:digitalmediaAsset:" +
                    digitalmediaAsset + "\",\"category\":\"IMAGE\"}]}";
                postResponse = objLdFunctions.GetInnerHttpHelper().PostRequest(afterUploadUrl, afterUploadPostData)
                    .Response;
                requestParameters.ContentType = lastContentType;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return postResponse;
        }

        public void DeletePost(LdFunctions objLdFunctions, string postUrl, string group = "")
        {
            try
            {
                //var reqParams = objLDFunctions.ObjLdHttpHelper.GetRequestParameter();
                //string groupId = Utils.GetBetween(group + "$$", "/groups/", "$$").Replace("/", "");
                //string postId = Utils.GetBetween(postUrl + "$$", "activity:", "$$").Replace("/", "");
                //string url = $"{LdConstants.GetMediaShareAPI}/urn:li:activity:{groupId}-{postId}";


                //// objLDFunctions.ObjLdHttpHelper.Request.Method = "DELETE";
                //reqParams.Headers.Add("X-li-page-instance", "urn:li:page:p_flagship3_groups_entity_member;MiiR2FmdStWGvd0RXzd2CA==");
                //objLDFunctions.ObjLdHttpHelper.Request.Accept = "application/vnd.linkedin.mobile.deduped+json";
                //reqParams.Accept =
                //    "com.linkedin.android/116400 (Linux; U; Android 6.0; en_US; V23GB; Build/MRA58K; Cronet/60.0.3082.0)";
                //var delete = objLDFunctions.ObjLdHttpHelper.GetRequest(url);
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }


        //public override bool DeletePost(string postId)
        //{
        //    return base.DeletePost(postId);
        //}
    }
}