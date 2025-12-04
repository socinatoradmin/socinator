using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction.TumblrBrowserManager;
using TumblrDominatorCore.TumblrRequest;
using TumblrDominatorCore.TumblrResponseHandler;
using Unity;

namespace TumblrDominatorCore.TumblrLibrary.TumblrProcesses
{
    public class TumblrPublisherJobProcess : PublisherJobProcess
    {
        private readonly ITumblrHttpHelper _httpHelper;
        private readonly ITumblrLoginProcess _logInProcess;
        private readonly ITumblrFunct _tumblrFunction;
        private readonly DominatorAccountModel dominatorAccountModel;

        public TumblrPublisherJobProcess(string campaignId, string accountId, SocialNetworks network,
            List<string> groupDestinationLists, List<string> pageDestinationList,
            List<PublisherCustomDestinationModel> customDestinationModels, bool isPublishOnOwnWall,
            CancellationTokenSource campaignCancellationToken)
            : base(campaignId, accountId, network, groupDestinationLists, pageDestinationList, customDestinationModels,
                isPublishOnOwnWall, campaignCancellationToken)
        {
        }

        public TumblrPublisherJobProcess(string campaignId, string campaignName, string accountId,
            SocialNetworks network,
            IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken)
            : base(campaignId, campaignName, accountId, network, destinationDetails, campaignCancellationToken)
        {
            dominatorAccountModel = new DominatorAccountModel();
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            dominatorAccountModel = accountsFileManager.GetAccountById(accountId);
            var accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            dominatorAccountModel.AccountId = accountId;
            _httpHelper = accountScopeFactory[accountId].Resolve<ITumblrHttpHelper>();
            _tumblrFunction = accountScopeFactory[dominatorAccountModel.AccountId].Resolve<ITumblrFunct>();
            _logInProcess = accountScopeFactory[dominatorAccountModel.AccountId].Resolve<ITumblrLoginProcess>();
        }

        public override bool PublishOnPages(string accountId, string blogUrl, PublisherPostlistModel postDetails,
            bool isDelayNeeded = true)
        {
            if (string.IsNullOrEmpty(blogUrl))
                return false;
            var isLoggedIn = _logInProcess.CheckAutomationLogin(AccountModel, AccountModel.CancellationSource.Token).Result;

            if (!isLoggedIn || AccountModel.AccountBaseModel.Status == AccountStatus.InvalidCredentials
                 || AccountModel.AccountBaseModel.Status == AccountStatus.PermanentlyBlocked ||
                 AccountModel.AccountBaseModel.Status == AccountStatus.TemporarilyBlocked)
                return false;
            var xTumblrFormKey = string.Empty;
            var url = string.Empty;
            var status = false;
            if (postDetails.sharePostModel.IsShareCustomPostList && !string.IsNullOrEmpty(postDetails.ShareUrl))
            {
                var lstPostList = new List<TumblrPost>();
                ReblogPostResponse shareResponse = new ReblogPostResponse();
                Regex.Split(postDetails.ShareUrl, "\r\n").ToList().ForEach(x =>
                {
                    lstPostList.Add(TumblrUtility.getTumblrPostFromPostUrl(x));
                });
                foreach (var post in lstPostList)
                {
                    SearchPostsResonseHandler searchUserPostResponseHandler = new SearchPostsResonseHandler();
                    TumblrPost tumblrPost = post;
                    if (!dominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        var postDetailsUrl = ConstantHelpDetails.GetUserPostDetailsAPIByName(tumblrPost.OwnerUsername, tumblrPost.Id);
                        searchUserPostResponseHandler = new SearchPostsResonseHandler(_tumblrFunction.GetApiResponse(dominatorAccountModel, postDetailsUrl
                           , ConstantHelpDetails.BearerToken, tumblrPost.PostUrl).Response);
                        tumblrPost = searchUserPostResponseHandler?.LstTumblrPosts?.Where(x => x.Id == tumblrPost.Id).FirstOrDefault();
                        if (tumblrPost != null && tumblrPost.CanReblog)
                            shareResponse = _tumblrFunction.ReblogPost(dominatorAccountModel, tumblrPost, "");
                        else if (searchUserPostResponseHandler.Success && tumblrPost != null && !tumblrPost.CanReblog)
                        {
                            var accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
                            var browserManager = accountScopeFactory[$"{dominatorAccountModel.AccountId}_AutomationLogin"]
                                .Resolve<ITumblrBrowserManager>();
                            browserManager.BrowserLogin(dominatorAccountModel, dominatorAccountModel.Token, LoginType.AutomationLogin);
                            shareResponse = browserManager.ReblogPost(dominatorAccountModel, tumblrPost);
                            if (browserManager != null && browserManager.BrowserWindow != null
                                && lstPostList.Last() == post)
                                browserManager.CloseBrowser(dominatorAccountModel);
                        }
                    }
                    else
                    {
                        _logInProcess._browser.SearchPostDetails(dominatorAccountModel, ref tumblrPost);
                        if (tumblrPost.CanReblog)
                            shareResponse = _logInProcess._browser.ReblogPost(dominatorAccountModel, tumblrPost);
                    }

                    if (shareResponse.Success)
                    {
                        GlobusLogHelper.log.Info(Log.SharedSuccessfully, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, "Page", blogUrl);
                        var rebloggedUrl = ConstantHelpDetails.GetPostUrlByUserNameAndPostId(dominatorAccountModel.AccountBaseModel.UserId, shareResponse.rebloggedPostId);
                        UpdatePostWithSuccessful(blogUrl, postDetails, rebloggedUrl);
                        status = true;

                    }
                    else if (!shareResponse.Success && !tumblrPost.CanReblog)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                                   dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                   dominatorAccountModel.AccountBaseModel.UserName, "Page", "Can Not Share to the Page...");
                        UpdatePostWithFailed(blogUrl, postDetails, "Can Not Share to the Page...");
                        status = false;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.ShareFailed, AccountModel.AccountBaseModel.AccountNetwork,
                           AccountModel.AccountBaseModel.UserName, "Page", blogUrl);
                        UpdatePostWithFailed(blogUrl, postDetails, "Error while sharing");
                        status = false;
                    }
                    if (isDelayNeeded)
                        DelayBeforeNextPublish();

                }
                return status;
            }
            else
            {

                PublishResponseHandler publisherResponse = new PublishResponseHandler();
                var updatedPostModel = PerformGeneralSettings(postDetails);
                if (string.IsNullOrEmpty(postDetails.PostDescription) && postDetails.PostDetailModel.IsSinglePost) updatedPostModel.PostDescription = postDetails.PostDescription;
                try
                {
                    try
                    {
                        #region settings

                        AdvanceSetting objSetting = null;
                        Application.Current.Invoke(() => { objSetting = new AdvanceSetting(); });

                        var tagaction = new List<string>();

                        if (objSetting.TumblrModel.IsRemoveTagsFromPostText)
                            if (updatedPostModel.TumblrPostSettings.IsTagUser)
                            {
                                updatedPostModel.TumblrPostSettings.TagUserList = string.Empty;
                                updatedPostModel.TumblrPostSettings.IsTagUser = false;
                            }

                        if (objSetting.TumblrModel.IsEnableAutomaticHashTags)
                        {
                            updatedPostModel.TumblrPostSettings.IsTagUser = true;
                            var spilttag = Regex.Split(objSetting.TumblrModel.HashWords, "#").Skip(1).ToArray();
                            foreach (var item in spilttag)
                                try
                                {
                                    if (tagaction.Count >= objSetting.TumblrModel.MaxHashtagsPerPost)
                                    {
                                        break;
                                    }

                                    updatedPostModel.TumblrPostSettings.TagUserList =
                                        updatedPostModel.TumblrPostSettings.TagUserList + item;
                                    tagaction.Add(item);
                                }
                                catch (Exception)
                                {
                                    // ignored
                                }

                            updatedPostModel.TumblrPostSettings.TagUserList = updatedPostModel.TumblrPostSettings
                                .TagUserList.Replace(" ", "").TrimEnd(',');
                        }

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                    try
                    {
                        if (blogUrl.Contains(ConstantHelpDetails.TumblrUrl))
                            url = Regex.Matches(blogUrl, "https://(.*?)$")[0].Groups[1].ToString();
                        else
                            url = Regex.Matches(blogUrl, "https://(.*?)/")[0].Groups[1].ToString();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    updatedPostModel.CurrentMediaUrl = url;
                    if (updatedPostModel.TumblrPostSettings.IsTagUser)
                    {
                        if (updatedPostModel.TumblrPostSettings.TagUserList.Contains("#") && !updatedPostModel.TumblrPostSettings.TagUserList.Contains("@"))
                            updatedPostModel.TumblrPostSettings.TagUserList =
                                updatedPostModel.TumblrPostSettings.TagUserList.Replace("#", "#@").Replace("\r\n", ",");
                        if (!updatedPostModel.TumblrPostSettings.TagUserList.Contains("#") && updatedPostModel.TumblrPostSettings.TagUserList.Contains("@"))
                        {
                            updatedPostModel.TumblrPostSettings.TagUserList = updatedPostModel.TumblrPostSettings.TagUserList.Replace("@", ",#@").Replace("\r\n", ",");
                        }

                    }

                    if (!AccountModel.IsRunProcessThroughBrowser)
                        publisherResponse = _tumblrFunction.JobforPublishPost(xTumblrFormKey, dominatorAccountModel, updatedPostModel);
                    else
                        publisherResponse = _logInProcess._browser.PublishPost(dominatorAccountModel, updatedPostModel);
                    if (publisherResponse.Success)
                    {
                        var SuccessLogMessage = !string.IsNullOrEmpty(publisherResponse.Message) && publisherResponse.Message == ConstantHelpDetails.ProcessingMediaJsonResponse ?
                            $"{publisherResponse?.PublishedUrl} with status => {publisherResponse.Message}" : publisherResponse?.PublishedUrl;
                        GlobusLogHelper.log.Info(Log.PublishingSuccessfully, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, "Page", SuccessLogMessage);
                        UpdatePostWithSuccessful(blogUrl, updatedPostModel, publisherResponse?.PublishedUrl);
                        status = true;
                    }
                    else
                    {
                        var FailedLog = !string.IsNullOrEmpty(publisherResponse?.Message) ? $"{blogUrl} with error => {publisherResponse.Message}" : blogUrl;
                        GlobusLogHelper.log.Info(Log.PublishingFailed, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, "Page", FailedLog);
                        UpdatePostWithFailed(blogUrl, updatedPostModel, "Error while Sharing..");
                        status = false;
                    }

                    if (isDelayNeeded)
                        DelayBeforeNextPublish();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            return status;
        }
    }
}