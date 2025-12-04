using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDResponse.Publisher;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Unity;
using FacebookModel = DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel;

namespace FaceDominatorCore.FDFactories
{
    public class FdPublisherJobProcess : PublisherJobProcess
    {
        private IFdLoginProcess _fdLoginProcess;
        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly SoftwareSettingsModel _softwareSettingsModel;

        public ModuleSetting ModuleSetting { get; set; }
        public DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.RedditModel RedditModel { get; set; }

        private FacebookModel AdvanceModel { get; }

        private DominatorAccountModel _accountModel { get; set; }



        public FdPublisherJobProcess(string campaignId, string accountId, SocialNetworks network,
            List<string> groupDestinationLists, List<string> pageDestinationList, List<PublisherCustomDestinationModel> customDestinationModels, bool isPublishOnOwnWall, CancellationTokenSource campaignCancellationToken)
            : base(campaignId, accountId, network, groupDestinationLists, pageDestinationList, customDestinationModels, isPublishOnOwnWall, campaignCancellationToken)
        {

            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _fdLoginProcess = _accountScopeFactory[$"{accountId}_Publisher_{campaignId}"].Resolve<IFdLoginProcess>();
            _softwareSettingsModel = InstanceProvider.GetInstance<ISoftwareSettingsFileManager>().GetSoftwareSettings();
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();

            _accountModel = accountsFileManager.GetAccountById(accountId);
            _fdLoginProcess.RequestParameterInitialized(_accountModel);

            var advanceModel = GenericFileManager.GetModuleDetails<FacebookModel>
                    (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Facebook))
                .FirstOrDefault(x => x.CampaignId == campaignId);

            AdvanceModel = advanceModel ?? new FacebookModel();
        }


        public FdPublisherJobProcess(string campaignId, string campaignName, string accountId, SocialNetworks network,
            IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken)
            : base(campaignId, campaignName, accountId, network, destinationDetails, campaignCancellationToken)
        {

            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _fdLoginProcess = _accountScopeFactory[$"{accountId}_Publisher"].Resolve<IFdLoginProcess>();
            _softwareSettingsModel = InstanceProvider.GetInstance<ISoftwareSettingsFileManager>().GetSoftwareSettings();
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();

            _accountModel = accountsFileManager.GetAccountById(accountId);

            if (_accountModel == null)
                GlobusLogHelper.log.Info(Log.CustomMessage, network, destinationDetails.ToList().FirstOrDefault(x => x.AccountId == accountId).AccountName ?? string.Empty,
                    "", "Account is not available! Publishing failed...");

            _fdLoginProcess.RequestParameterInitialized(_accountModel);

            var advanceModel = GenericFileManager.GetModuleDetails<FacebookModel>
                (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Facebook))
                .FirstOrDefault(x => x.CampaignId == campaignId);

            AdvanceModel = advanceModel ?? new FacebookModel();
        }

        private void AssignEnableDeletePost(PublisherPostlistModel postDetails, string postUrl, string destinationUrl, string destinationType)
        {
            PostDeletionModel objDeletionModel = new PostDeletionModel();
            objDeletionModel.AccountId = _accountModel.AccountId;
            objDeletionModel.CampaignId = CampaignId;
            objDeletionModel.DeletionTime = DateTime.Now.AddHours(AdvanceModel.DeletePostAfter.GetRandom());
            objDeletionModel.DestinationType = destinationType;
            objDeletionModel.DestinationUrl = destinationUrl;
            objDeletionModel.PostId = postDetails.PostId;
            objDeletionModel.PublishedIdOrUrl = postUrl;
            objDeletionModel.Networks = SocialNetworks.Facebook;
            PublishScheduler.EnableDeletePost(objDeletionModel);
        }


        //Todo : Need to implement
        public bool DeletePostOld(string postId)
        {

            try
            {
                var fdRequestLibrary =
                    _accountScopeFactory[$"{AccountModel.AccountId}_Publisher"].Resolve<IFdRequestLibrary>();

                var listPostDeletionModel = GenericFileManager.GetModuleDetails<PostDeletionModel>(ConstantVariable.GetDeletePublisherPostModel).Where(x => CampaignId != null && x.CampaignId == CampaignId).ToList();

                var postDeletionModel = listPostDeletionModel.FirstOrDefault(x => x.PublishedIdOrUrl.Contains(postId));

                if (postDeletionModel != null)
                {
                    var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                    var _accountModel = accountsFileManager.GetAccountById(postDeletionModel.AccountId);

                    string ftEnrtIdentifier = string.Empty;

                    // ftEnrtIdentifier = fdRequestLibrary.GetPostEntIdentifier(_accountModel, postId);

                    if (postDeletionModel.DestinationType == "Group")
                    {
                        fdRequestLibrary.DeletePostFromGroup(_accountModel, postId, postDeletionModel.DestinationUrl, ftEnrtIdentifier);
                    }
                    else if (!postDeletionModel.PostId.Contains(FdConstants.FbHomeUrl))
                    {
                        fdRequestLibrary.DeleteStoryPost(_accountModel, postDeletionModel.PostId, postDeletionModel.PostId);
                    }
                    else
                    {
                        fdRequestLibrary.DeletePostFromTimeline(_accountModel, postId, postDeletionModel, ftEnrtIdentifier);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return true;
        }

        public override bool DeletePost(string postId)
        {

            try
            {
                bool isSuccess = false;
                string ftEnrtIdentifier = string.Empty;
                var fdRequestLibrary =
                    _accountScopeFactory[$"{AccountModel.AccountId}_Publisher"].Resolve<IFdRequestLibrary>();

                var listPostDeletionModel = GenericFileManager.GetModuleDetails<PostDeletionModel>(ConstantVariable.GetDeletePublisherPostModel).Where(x => CampaignId != null && x.CampaignId == CampaignId).ToList();

                var postDeletionModel = listPostDeletionModel.FirstOrDefault(x => x.PublishedIdOrUrl.Contains(postId));

                if (postDeletionModel != null)
                {

                    var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
                    var _accountModel = accountsFileManager.GetAccountById(postDeletionModel.AccountId);

                    // ftEnrtIdentifier = fdRequestLibrary.GetPostEntIdentifier(_accountModel, postId);

                    if (!AccountModel.IsRunProcessThroughBrowser)
                        isSuccess = fdRequestLibrary.Login(_accountModel);
                    else
                    {
                        _fdLoginProcess = _accountScopeFactory[$"{_accountModel.AccountId}_Publisher_{postId}"].Resolve<IFdLoginProcess>();

                        _fdLoginProcess.LoginWithBrowserMethod(_accountModel, CampaignCancellationToken.Token);

                        isSuccess = _accountModel.IsUserLoggedIn;
                    }

                    if (!isSuccess)
                    {
                        if (AccountModel.IsRunProcessThroughBrowser)
                            _fdLoginProcess._browserManager.CloseBrowser(_accountModel);
                        return false;
                    }
                    if (!AccountModel.IsRunProcessThroughBrowser)
                    {
                        if (postDeletionModel.DestinationType == "Group")
                        {
                            fdRequestLibrary.DeletePostFromGroup(_accountModel, postId, postDeletionModel.DestinationUrl, ftEnrtIdentifier);
                        }
                        else if (!postDeletionModel.PostId.Contains(FdConstants.FbHomeUrl))
                        {
                            fdRequestLibrary.DeleteStoryPost(_accountModel, postDeletionModel.PostId, postDeletionModel.PostId);
                        }
                        else
                        {
                            fdRequestLibrary.DeletePostFromTimeline(_accountModel, postId, postDeletionModel, ftEnrtIdentifier);
                        }
                    }
                    else
                        _fdLoginProcess._browserManager.DeletePost(_accountModel, postId, postDeletionModel);


                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return true;
        }

        public override bool PublishOnCustomDestination(string accountId, PublisherCustomDestinationModel customDestinationModel, PublisherPostlistModel postDetails, bool isDelayNeed = true)
        {

            try
            {

                if (postDetails.FdPostSettings.IsReplaceDescriptionSelected)
                    postDetails.PostDescription = postDetails.FdPostSettings.PostReplaceDescription;

                //from insta post to facebook Post.
                if (string.IsNullOrEmpty(postDetails.PostDescription) && !string.IsNullOrEmpty(postDetails.PublisherInstagramTitle))
                    postDetails.PostDescription = postDetails.PublisherInstagramTitle;

                ApplyDynamicHashTag(ref postDetails);

                if (string.Compare("Groups", customDestinationModel.DestinationType,
                           StringComparison.CurrentCultureIgnoreCase) == 0 ||
                       string.Compare("Group", customDestinationModel.DestinationType,
                           StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    CampaignCancellationToken.Token.ThrowIfCancellationRequested();
                    return PublishOnGroups(accountId, customDestinationModel.DestinationValue, postDetails, isDelayNeed);
                }


                if (!postDetails.IsFdSellPost && (string.Compare("Pages", customDestinationModel.DestinationType,
                       StringComparison.CurrentCultureIgnoreCase) == 0 ||
                   string.Compare("Page", customDestinationModel.DestinationType,
                       StringComparison.CurrentCultureIgnoreCase) == 0))
                {
                    CampaignCancellationToken.Token.ThrowIfCancellationRequested();
                    return PublishOnPages(accountId, customDestinationModel.DestinationValue, postDetails, isDelayNeed);
                }

                if (!postDetails.IsFdSellPost && string.Compare("OwnWall", customDestinationModel.DestinationType,
                        StringComparison.CurrentCultureIgnoreCase) == 0 ||
                    string.Compare("Own Wall", customDestinationModel.DestinationType,
                        StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    CampaignCancellationToken.Token.ThrowIfCancellationRequested();
                    return PublishOnOwnWall(accountId, postDetails, isDelayNeed);
                }

                if (!postDetails.IsFdSellPost && (string.Compare("Events", customDestinationModel.DestinationType,
                            StringComparison.CurrentCultureIgnoreCase) == 0 ||
                        string.Compare("Event", customDestinationModel.DestinationType,
                            StringComparison.CurrentCultureIgnoreCase) == 0))
                {
                    CampaignCancellationToken.Token.ThrowIfCancellationRequested();
                    return PublishOnEvent(accountId, customDestinationModel.DestinationValue, postDetails, isDelayNeed);
                }

                if (!postDetails.IsFdSellPost && (string.Compare("Friends", customDestinationModel.DestinationType,
                            StringComparison.CurrentCultureIgnoreCase) == 0 ||
                        string.Compare("Friend", customDestinationModel.DestinationType,
                            StringComparison.CurrentCultureIgnoreCase) == 0))
                {
                    CampaignCancellationToken.Token.ThrowIfCancellationRequested();
                    return PublishOnFriend(accountId, customDestinationModel.DestinationValue, postDetails, isDelayNeed);
                }

                if (!postDetails.IsFdSellPost && (string.Compare("Private Message", customDestinationModel.DestinationType,
                        StringComparison.CurrentCultureIgnoreCase) == 0 ||
                    string.Compare("Private Messages", customDestinationModel.DestinationType,
                        StringComparison.CurrentCultureIgnoreCase) == 0 ||
                    string.Compare("PrivateMessages", customDestinationModel.DestinationType,
                        StringComparison.CurrentCultureIgnoreCase) == 0))
                {
                    CampaignCancellationToken.Token.ThrowIfCancellationRequested();
                    return ShareToProfileAsPrivateMessage(accountId, customDestinationModel.DestinationValue, postDetails, isDelayNeed);
                }

                if (postDetails.IsFdSellPost)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork, _accountModel.AccountBaseModel.UserName, "", "Buy Sell Post can only be posted on groups");
                }

            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancellated!");
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }


        public override bool PublishOnGroups(string accountId, string groupUrl, PublisherPostlistModel givenPostDetails, bool isDelayNeeded = true)
        {
            try
            {
                //from insta post to facebook Post.
                if (string.IsNullOrEmpty(givenPostDetails.PostDescription) && !string.IsNullOrEmpty(givenPostDetails.PublisherInstagramTitle))
                    givenPostDetails.PostDescription = givenPostDetails.PublisherInstagramTitle;

                var fdRequestLibrary =
                    _accountScopeFactory[$"{accountId}_Publisher"].Resolve<IFdRequestLibrary>();


                if (givenPostDetails.FdPostSettings.IsReplaceDescriptionSelected)
                    givenPostDetails.PostDescription = givenPostDetails.FdPostSettings.PostReplaceDescription;

                ApplyDynamicHashTag(ref givenPostDetails);

                GlobusLogHelper.log.Info(Log.AccountLogin, _accountModel.AccountBaseModel.AccountNetwork, _accountModel.AccountBaseModel.UserName);

                var isSuccess = false;

                if (!AccountModel.IsRunProcessThroughBrowser)
                    isSuccess = fdRequestLibrary.Login(_accountModel);
                else
                {
                    _fdLoginProcess = _accountScopeFactory[$"{accountId}_Publisher_{givenPostDetails.PostId}_Dest_{groupUrl}"].Resolve<IFdLoginProcess>();

                    _fdLoginProcess.LoginWithBrowserMethod(_accountModel, CampaignCancellationToken.Token);

                    isSuccess = _accountModel.IsUserLoggedIn;
                }

                if (!isSuccess)
                {
                    if (AccountModel.IsRunProcessThroughBrowser)
                        _fdLoginProcess._browserManager.CloseBrowser(_accountModel);
                    return false;
                }

                GlobusLogHelper.log.Info(Log.SuccessfulLogin, _accountModel.AccountBaseModel.AccountNetwork, _accountModel.AccountBaseModel.UserName);

                var postDetails = PerformGeneralSettings(givenPostDetails);
                if (string.IsNullOrEmpty(givenPostDetails.PostDescription) && postDetails?.MediaList?.Count > 0 && postDetails.PostDescription == new FileInfo(postDetails.MediaList.FirstOrDefault()).Name)
                    postDetails.PostDescription = givenPostDetails.PostDescription;
                if (OtherConfiguration.IsEnableSignatureChecked)
                    postDetails.PostDescription = postDetails.PostDescription + " ";
                IResponseHandler responseHandler;

                string ftEnrtIdentifier = string.Empty;

                // postDetails.PostDescription = WebUtility.HtmlEncode(postDetails.PostDescription);

                if (AccountModel.IsRunProcessThroughBrowser)
                {
                    _fdLoginProcess._browserManager.SearchPostsByGroupUrl(_accountModel, FbEntityType.Groups,
                       groupUrl, isFdBuySell: postDetails.IsFdSellPost);

                    responseHandler = _fdLoginProcess._browserManager.PostOnGroups(_accountModel, postDetails, CampaignCancellationToken,
                           GeneralSettingsModel, AdvanceModel);

                    if (responseHandler.Status && postDetails.FdPostSettings.SellPostTurnOffComment)
                    {
                        _fdLoginProcess._browserManager.TurnOffCommentsOrNotificationsForPost(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, CampaignCancellationToken, turnOffComment: AdvanceModel.IsDisableCommentsForNewPost);
                    }



                    if (!responseHandler.Status)
                    {
                        UpdatePostWithFailed(groupUrl, postDetails, responseHandler.PageletData);

                        GlobusLogHelper.log.Info(Log.ShareFailed, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Groups", groupUrl);

                        if (isDelayNeeded)
                            DelayBeforeNextPublish();
                    }
                    else
                    {
                        UpdatePostWithSuccessful(groupUrl, postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);
                        GlobusLogHelper.log.Info(Log.SharedSuccessfully, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Groups", groupUrl);

                        // ftEnrtIdentifier = fdRequestLibrary.GetPostEntIdentifier(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);


                        if (AdvanceModel.IsTurnOffNotificationsForNewPost)
                        {
                            var status = fdRequestLibrary.StopNotification(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, ftEnrtIdentifier);
                            if (!status && _accountModel.IsRunProcessThroughBrowser)
                                status = _fdLoginProcess._browserManager.TurnOffCommentsOrNotificationsForPost(_accountModel, responseHandler?.ObjFdScraperResponseParameters?.PostDetails?.PostUrl, CampaignCancellationToken, turnOffNotification: true);
                            GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork,
                                _accountModel.AccountBaseModel.UserName, "",
                                status ? "Successfully turned off notification for this post"
                                    : "Turn off notification failed for this post");
                        }
                        if (AdvanceModel.IsDisableCommentsForNewPost)
                        {
                            _fdLoginProcess._browserManager.TurnOffCommentsOrNotificationsForPost(_accountModel, responseHandler?.ObjFdScraperResponseParameters?.PostDetails?.PostUrl, CampaignCancellationToken, turnOffComment: AdvanceModel.IsDisableCommentsForNewPost);

                            GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork, _accountModel.AccountBaseModel.UserName, "", "Successfully disabled commenting for this post");

                        }

                        if (AdvanceModel.IsDeletePostAfter)
                        {
                            AssignEnableDeletePost(postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, groupUrl, "Group");
                        }

                        _fdLoginProcess._browserManager.CloseBrowser(AccountModel);

                        if (isDelayNeeded)
                            DelayBeforeNextPublish();
                        return true;
                    }

                    _fdLoginProcess._browserManager.CloseBrowser(_accountModel);
                }
                else if (!postDetails.IsFdSellPost && (postDetails.PostSource == PostSource.SharePost
                    || (postDetails.PostSource == PostSource.RssFeedPost && postDetails.MediaList.Count == 0)
                    || (postDetails.PostSource == PostSource.NormalPost && postDetails.MediaList.Count == 0 &&
                    !string.IsNullOrEmpty(postDetails.PdSourceUrl))))
                {
                    if (postDetails.PostSource == PostSource.NormalPost && postDetails.MediaList.Count == 0 &&
                     postDetails.PdSourceUrl != null)
                    {
                        var postdeatils = postDetails.DeepCloneObject();
                        postDetails.ShareUrl = postdeatils.PdSourceUrl;
                        responseHandler = fdRequestLibrary.ShareToGroups(_accountModel, groupUrl, postDetails,
                        CampaignCancellationToken, GeneralSettingsModel, AdvanceModel);
                    }
                    else if (!string.IsNullOrEmpty(postDetails.ShareUrl))
                    {
                        responseHandler = fdRequestLibrary.ShareToGroups(_accountModel, groupUrl, postDetails,
                            CampaignCancellationToken, GeneralSettingsModel, AdvanceModel);
                    }
                    else
                    {
                        responseHandler = fdRequestLibrary.ShareToGroups(_accountModel, groupUrl, postDetails,
                       CampaignCancellationToken, GeneralSettingsModel, AdvanceModel);

                    }

                    if (responseHandler.Status == false)
                    {
                        UpdatePostWithFailed(groupUrl, postDetails, responseHandler.PageletData);

                        GlobusLogHelper.log.Info(Log.ShareFailed, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Groups", groupUrl);

                        if (isDelayNeeded)
                            DelayBeforeNextPublish();
                    }
                    else
                    {
                        UpdatePostWithSuccessful(groupUrl, postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);
                        GlobusLogHelper.log.Info(Log.SharedSuccessfully, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Groups", groupUrl);

                        //ftEnrtIdentifier = fdRequestLibrary.GetPostEntIdentifier(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);


                        if (AdvanceModel.IsTurnOffNotificationsForNewPost)
                        {
                            var status = fdRequestLibrary.StopNotification(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, ftEnrtIdentifier);
                            if (!status && _accountModel.IsRunProcessThroughBrowser)
                                status = _fdLoginProcess._browserManager.TurnOffCommentsOrNotificationsForPost(_accountModel, responseHandler?.ObjFdScraperResponseParameters?.PostDetails?.PostUrl, CampaignCancellationToken, turnOffNotification: true);
                            GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork,
                                _accountModel.AccountBaseModel.UserName, "",
                                status ? "Successfully turned off notification for this post"
                                    : "Turn off notification failed for this post");
                        }
                        if (AdvanceModel.IsDisableCommentsForNewPost)
                        {
                            fdRequestLibrary.StopCommenting(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, ftEnrtIdentifier);

                            GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork, _accountModel.AccountBaseModel.UserName, "", "Successfully disabled commenting for this post");

                        }

                        if (AdvanceModel.IsDeletePostAfter)
                        {
                            AssignEnableDeletePost(postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, groupUrl, "Group");
                        }

                        if (isDelayNeeded)
                            DelayBeforeNextPublish();
                        return true;
                    }
                }
                else if (!postDetails.IsFdSellPost)
                {
                    postDetails.PostDescription += $"\r\n{postDetails.PdSourceUrl}";

                    PublisherPostlistModel updatedPostDetails = postDetails;

                    responseHandler = fdRequestLibrary.PostToGroups(_accountModel, groupUrl, postDetails,
                        CampaignCancellationToken, GeneralSettingsModel, AdvanceModel);

                    if (!responseHandler.Status)
                    {
                        UpdatePostWithFailed(groupUrl, postDetails, responseHandler.PageletData);
                        GlobusLogHelper.log.Info(Log.PublishingFailed, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Groups", groupUrl);
                        if (isDelayNeeded)
                            DelayBeforeNextPublish();
                    }
                    else
                    {
                        UpdatePostWithSuccessful(groupUrl, postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);

                        GlobusLogHelper.log.Info(Log.PublishingSuccessfully, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Groups", groupUrl);

                        // ftEnrtIdentifier = fdRequestLibrary.GetPostEntIdentifier(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);

                        if (AdvanceModel.IsTurnOffNotificationsForNewPost)
                        {
                            var status = fdRequestLibrary.StopNotificationGroups(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, groupUrl, ftEnrtIdentifier);
                            if (!status && _accountModel.IsRunProcessThroughBrowser)
                                status = _fdLoginProcess._browserManager.TurnOffCommentsOrNotificationsForPost(_accountModel, responseHandler?.ObjFdScraperResponseParameters?.PostDetails?.PostUrl, CampaignCancellationToken, turnOffNotification: true);
                            GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork,
                                _accountModel.AccountBaseModel.UserName, "",
                                status ? "Successfully turned off notification for this post"
                                    : "Turn off notification failed for this post");
                        }

                        if (AdvanceModel.IsDisableCommentsForNewPost)
                        {
                            fdRequestLibrary.StopCommenting(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, ftEnrtIdentifier);

                            GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork, _accountModel.AccountBaseModel.UserName, "", "Successfully disabled commenting for this post");

                        }

                        if (AdvanceModel.IsDeletePostAfter)
                        {
                            AssignEnableDeletePost(postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, groupUrl, "Group");
                        }

                        if (isDelayNeeded)
                            DelayBeforeNextPublish();

                        return true;
                    }
                }
                else
                {

                    PublisherPostlistModel updatedPostDetails = postDetails;



                    postDetails.PostDescription += $"\r\n{postDetails.PdSourceUrl}";

                    responseHandler = fdRequestLibrary.SellPostToGroups(_accountModel, groupUrl, postDetails,
                        CampaignCancellationToken, GeneralSettingsModel, AdvanceModel);


                    if (responseHandler.Status == false)
                    {
                        UpdatePostWithFailed(groupUrl, postDetails, responseHandler.PageletData);
                        GlobusLogHelper.log.Info(Log.PublishingFailed, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Groups", groupUrl);
                        if (isDelayNeeded)
                            DelayBeforeNextPublish();
                    }
                    else
                    {
                        UpdatePostWithSuccessful(groupUrl, postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);

                        GlobusLogHelper.log.Info(Log.PublishingSuccessfully, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Groups", groupUrl);

                        // ftEnrtIdentifier = fdRequestLibrary.GetPostEntIdentifier(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);

                        if (AdvanceModel.IsTurnOffNotificationsForNewPost)
                        {
                            var status = fdRequestLibrary.StopNotificationGroups(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, groupUrl, ftEnrtIdentifier);
                            if (!status && _accountModel.IsRunProcessThroughBrowser)
                                status = _fdLoginProcess._browserManager.TurnOffCommentsOrNotificationsForPost(_accountModel, responseHandler?.ObjFdScraperResponseParameters?.PostDetails?.PostUrl, CampaignCancellationToken, turnOffNotification: true);
                            GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork,
                                _accountModel.AccountBaseModel.UserName, "",
                                status ? "Successfully turned off notification for this post"
                                    : "Turn off notification failed for this post");
                        }

                        if (AdvanceModel.IsDisableCommentsForNewPost)
                        {
                            fdRequestLibrary.StopCommenting(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, ftEnrtIdentifier);

                            GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork, _accountModel.AccountBaseModel.UserName, "", "Successfully disabled commenting for this post");

                        }

                        if (AdvanceModel.IsDeletePostAfter)
                            AssignEnableDeletePost(postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, groupUrl, "Group");

                        if (isDelayNeeded)
                            DelayBeforeNextPublish();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _fdLoginProcess._browserManager.CloseBrowser(AccountModel);
                ex.DebugLog();
            }
            return false;
        }


        public override bool PublishOnOwnWall(string accountId, PublisherPostlistModel givenPostDetails,
            bool isDelayNeeded = true)
        {

            try
            {
                //from insta post to facebook Post.
                if (string.IsNullOrEmpty(givenPostDetails.PostDescription) && !string.IsNullOrEmpty(givenPostDetails.PublisherInstagramTitle))
                    givenPostDetails.PostDescription = givenPostDetails.PublisherInstagramTitle;

                var fdRequestLibrary =
                    _accountScopeFactory[$"{accountId}_Publisher"].Resolve<IFdRequestLibrary>();


                if (givenPostDetails.FdPostSettings.IsReplaceDescriptionSelected)
                    givenPostDetails.PostDescription = givenPostDetails.FdPostSettings.PostReplaceDescription;

                ApplyDynamicHashTag(ref givenPostDetails);

                GlobusLogHelper.log.Info(Log.AccountLogin, _accountModel.AccountBaseModel.AccountNetwork,
                    _accountModel.AccountBaseModel.UserName);

                var isSuccess = false;

                if (!AccountModel.IsRunProcessThroughBrowser)
                    isSuccess = fdRequestLibrary.Login(_accountModel);
                else
                {

                    _fdLoginProcess = _accountScopeFactory[$"{accountId}_Publisher_{givenPostDetails.PostId}_Dest_OwnWall"].Resolve<IFdLoginProcess>();

                    _fdLoginProcess.LoginWithBrowserMethod(_accountModel, CampaignCancellationToken.Token);

                    isSuccess = _accountModel.IsUserLoggedIn;
                }

                if (!isSuccess)
                {
                    if (AccountModel.IsRunProcessThroughBrowser)
                        _fdLoginProcess._browserManager.CloseBrowser(_accountModel);
                    return false;
                }

                GlobusLogHelper.log.Info(Log.SuccessfulLogin, _accountModel.AccountBaseModel.AccountNetwork,
                    _accountModel.AccountBaseModel.UserName);

                var postDetails = PerformGeneralSettings(givenPostDetails);

                if (string.IsNullOrEmpty(givenPostDetails.PostDescription) && postDetails.MediaList.Count > 0 && postDetails.PostDescription == new FileInfo(postDetails.MediaList.FirstOrDefault())?.Name)
                    postDetails.PostDescription = givenPostDetails.PostDescription;
                if (OtherConfiguration.IsEnableSignatureChecked)
                    postDetails.PostDescription = postDetails.PostDescription + " ";
                var mediaPath = string.Empty;

                var addMissingExtension = new ObservableCollection<string>();

                foreach (var media in postDetails.MediaList)
                {
                    if (!string.IsNullOrEmpty(media))
                    {
                        FileInfo fi = new FileInfo(media);
                        var extension = fi.Extension;
                        if (!string.IsNullOrEmpty(extension) && extension.ToLower().Contains(".mp4"))
                        {
                            addMissingExtension.Add(media);
                            continue;
                        }
                        if (!string.IsNullOrWhiteSpace(extension) && (!extension.Contains(".jpg") || !extension.Contains(".jpeg")))
                        {
                            addMissingExtension.Add(media);
                        }
                        else if (string.IsNullOrEmpty(extension))
                        {
                            mediaPath = Path.ChangeExtension(media, ".jpeg");
                            File.Move(media, mediaPath);
                            addMissingExtension.Add(mediaPath);
                        }
                        else
                            addMissingExtension.Add(media);
                    }

                }

                if (addMissingExtension != null && addMissingExtension.Count() > 0)
                    postDetails.MediaList = addMissingExtension;


                //  postDetails.PostDescription = WebUtility.HtmlEncode(postDetails.PostDescription);

                // apply facebook post settings, (ie apply only for create post)

                IResponseHandler responseHandler;

                if (AccountModel.IsRunProcessThroughBrowser)
                {
                    responseHandler = _fdLoginProcess._browserManager.PostOnOwnWall(_accountModel, postDetails, CampaignCancellationToken,
                            GeneralSettingsModel, AdvanceModel);

                    if (!responseHandler.Status)
                    {
                        var publisherresponseHandler = new PublisherResponseHandler(new ResponseParameter() { Response = string.Empty }, string.Empty);
                        PublisherAfterActionForFailed(_accountModel, publisherresponseHandler, postDetails, PostOptions.OwnWall,
                            _accountModel.AccountBaseModel.UserId, isDelayNeeded);
                        _fdLoginProcess._browserManager.CloseBrowser(_accountModel);

                    }
                    else
                    {
                        var publisherresponseHandler = new PublisherResponseHandler(new ResponseParameter() { Response = string.Empty }, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);

                        PublisherAfterActionForSuccess(_accountModel, publisherresponseHandler, postDetails, PostOptions.OwnWall,
                            _accountModel.AccountBaseModel.UserId, isDelayNeeded);

                        _fdLoginProcess._browserManager.CloseBrowser(_accountModel);
                        return true;
                    }


                }
                else if (postDetails != null /*&& !postDetails.IsFdWatchPartyPost*/ && (postDetails.PostSource == PostSource.SharePost ||
                                            (postDetails.PostSource == PostSource.RssFeedPost && postDetails.MediaList.Count == 0) ||
                                            (postDetails.PostSource == PostSource.NormalPost && postDetails.MediaList.Count == 0 &&
                                             !string.IsNullOrEmpty(postDetails.PdSourceUrl))))
                {
                    if (postDetails.PostSource == PostSource.NormalPost && postDetails.MediaList.Count == 0 &&
                     !string.IsNullOrEmpty(postDetails.PdSourceUrl))
                    {
                        var postdeatils = postDetails.DeepCloneObject();
                        postDetails.ShareUrl = postdeatils.PdSourceUrl;
                        responseHandler = fdRequestLibrary.ShareToOwnWall(_accountModel, postDetails,
                        CampaignCancellationToken, GeneralSettingsModel, AdvanceModel);

                    }
                    else if (!string.IsNullOrEmpty(postDetails.ShareUrl))
                    {
                        responseHandler = fdRequestLibrary.ShareToOwnWall(_accountModel, postDetails,
                            CampaignCancellationToken, GeneralSettingsModel, AdvanceModel);
                    }
                    else
                    {
                        responseHandler = fdRequestLibrary.PostToOwnWall(_accountModel, postDetails, CampaignCancellationToken,
                           GeneralSettingsModel, AdvanceModel);

                    }
                    if (responseHandler.Status == false)
                    {
                        PublisherAfterActionForFailed(_accountModel, (PublisherResponseHandler)responseHandler, postDetails, PostOptions.OwnWall,
                            _accountModel.AccountBaseModel.UserId, isDelayNeeded);

                    }
                    else
                    {
                        PublisherAfterActionForSuccess(_accountModel, (PublisherResponseHandler)responseHandler, postDetails, PostOptions.OwnWall,
                            _accountModel.AccountBaseModel.UserId, isDelayNeeded);
                        return true;
                    }
                }
                else
                {
                    if (postDetails != null)
                    {
                        postDetails.PostDescription += $"\r\n{postDetails.PdSourceUrl}";

                        responseHandler = fdRequestLibrary.PostToOwnWall(_accountModel, postDetails, CampaignCancellationToken,
                           GeneralSettingsModel, AdvanceModel);

                        if (!responseHandler.Status)
                        {
                            PublisherAfterActionForFailed(_accountModel, (PublisherResponseHandler)responseHandler, postDetails, PostOptions.OwnWall,
                                _accountModel.AccountBaseModel.UserId, isDelayNeeded);
                        }
                        else
                        {
                            PublisherAfterActionForSuccess(_accountModel, (PublisherResponseHandler)responseHandler, postDetails, PostOptions.OwnWall,
                                _accountModel.AccountBaseModel.UserId, isDelayNeeded);
                            return true;
                        }
                    }
                }

                _accountScopeFactory[$"{accountId}_{givenPostDetails.PostId}"].Resolve<IFdLoginProcess>()._browserManager.CloseBrowser(_accountModel);
            }
            catch (Exception ex)
            {
                _fdLoginProcess._browserManager.CloseBrowser(AccountModel);
                ex.DebugLog();
            }
            return false;
        }


        public override bool PublishOnPages(string accountId, string pageUrl, PublisherPostlistModel
            givenPostDetails, bool isDelayNeeded = true)
        {
            try
            {
                //from insta post to facebook Post.
                if (string.IsNullOrEmpty(givenPostDetails.PostDescription) && !string.IsNullOrEmpty(givenPostDetails.PublisherInstagramTitle))
                    givenPostDetails.PostDescription = givenPostDetails.PublisherInstagramTitle;

                if (!string.IsNullOrEmpty(givenPostDetails.PdSourceUrl))
                    givenPostDetails.PostDescription += givenPostDetails.PdSourceUrl;

                var fdRequestLibrary =
                    _accountScopeFactory[$"{accountId}_Publisher"].Resolve<IFdRequestLibrary>();


                GlobusLogHelper.log.Info(Log.AccountLogin, _accountModel.AccountBaseModel.AccountNetwork,
                    _accountModel.AccountBaseModel.UserName);

                if (givenPostDetails.FdPostSettings.IsReplaceDescriptionSelected)
                    givenPostDetails.PostDescription = givenPostDetails.FdPostSettings.PostReplaceDescription;

                ApplyDynamicHashTag(ref givenPostDetails);

                var isSuccess = false;

                if (!AccountModel.IsRunProcessThroughBrowser)
                    isSuccess = fdRequestLibrary.Login(_accountModel);
                else
                {
                    _fdLoginProcess = _accountScopeFactory[$"{accountId}_Publisher_{givenPostDetails.PostId}_Dest_{pageUrl}"].Resolve<IFdLoginProcess>();

                    _fdLoginProcess.LoginWithBrowserMethod(_accountModel, CampaignCancellationToken.Token);

                    isSuccess = _accountModel.IsUserLoggedIn;
                }

                if (!isSuccess)
                {
                    if (AccountModel.IsRunProcessThroughBrowser)
                        _fdLoginProcess._browserManager.CloseBrowser(_accountModel);
                    return false;
                }


                IResponseHandler responseHandler;


                var postDetails = PerformGeneralSettings(givenPostDetails);
                if (string.IsNullOrEmpty(givenPostDetails.PostDescription) && postDetails.PostDescription == new FileInfo(postDetails.MediaList.FirstOrDefault()).Name)
                    postDetails.PostDescription = givenPostDetails.PostDescription;
                if (OtherConfiguration.IsEnableSignatureChecked)
                    postDetails.PostDescription = postDetails.PostDescription + " ";
                var mediaPath = string.Empty;

                var addMissingExtension = new ObservableCollection<string>();

                foreach (var media in postDetails.MediaList)
                {
                    FileInfo fi = new FileInfo(media);
                    var extension = fi.Extension;
                    if (!string.IsNullOrEmpty(extension) && extension.ToLower().Contains(".mp4"))
                    {
                        addMissingExtension.Add(media);
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(extension) || !extension.Contains(".jpg") || !extension.Contains(".jpeg"))
                    {
                        mediaPath = Path.ChangeExtension(media, ".jpeg");
                        if (File.Exists(mediaPath))
                        {
                            addMissingExtension.Add(mediaPath);
                            continue;
                        }
                        File.Move(media, mediaPath);
                        addMissingExtension.Add(mediaPath);
                    }
                    else
                        addMissingExtension.Add(media);
                }

                if (addMissingExtension != null)
                    postDetails.MediaList = addMissingExtension;

                if (AccountModel.IsRunProcessThroughBrowser)
                {
                    bool postAsPage = false;
                    if (AdvanceModel.IsPostAsPage)
                    {
                        var urlList = new List<string>();
                        AdvanceModel.SelectPageDetailsModel.DestinationDetailsModels.ForEach(x =>
                        {
                            urlList.Add(x.DestinationUrl);
                        });
                        postAsPage = urlList.Any(x => x.Contains(pageUrl));
                    }

                    _fdLoginProcess._browserManager.SearchPostsByPageUrl(_accountModel, FbEntityType.Fanpage,
                        pageUrl, postAsPage);

                    responseHandler = _fdLoginProcess._browserManager.PostOnPages(_accountModel, postDetails, CampaignCancellationToken,
                            GeneralSettingsModel, AdvanceModel, pageUrl);

                    _fdLoginProcess._browserManager.CloseBrowser(_accountModel);

                    string ftEnrtIdentifier = string.Empty;

                    if (!responseHandler.Status)
                    {
                        UpdatePostWithFailed(pageUrl, postDetails, responseHandler.ObjFdScraperResponseParameters.FetchStream);

                        GlobusLogHelper.log.Info(Log.ShareFailed, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Pages", pageUrl);
                        if (isDelayNeeded)
                            DelayBeforeNextPublish();
                    }
                    else
                    {
                        UpdatePostWithSuccessful(pageUrl, postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);

                        // ftEnrtIdentifier = fdRequestLibrary.GetPostEntIdentifier(_accountModel, responseHandler.ObjFdScraperResponseParameters.FetchStream);

                        if (AdvanceModel.IsTurnOffNotificationsForNewPost)
                        {
                            var status = fdRequestLibrary.StopNotification(_accountModel, responseHandler.ObjFdScraperResponseParameters.FetchStream, ftEnrtIdentifier);
                            if (!status && _accountModel.IsRunProcessThroughBrowser)
                                status = _fdLoginProcess._browserManager.TurnOffCommentsOrNotificationsForPost(_accountModel, responseHandler?.ObjFdScraperResponseParameters?.PostDetails?.PostUrl, CampaignCancellationToken, turnOffNotification: true);
                            GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork,
                                _accountModel.AccountBaseModel.UserName, "",
                                status ? "Successfully turned off notification for this post"
                                    : "Turn off notification failed for this post");
                        }

                        GlobusLogHelper.log.Info(Log.SharedSuccessfully, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Pages", pageUrl);

                        if (AdvanceModel.IsDeletePostAfter)
                        {
                            AssignEnableDeletePost(postDetails, responseHandler.ObjFdScraperResponseParameters.FetchStream, pageUrl, "Pages");
                        }

                        if (isDelayNeeded)
                            DelayBeforeNextPublish();

                        return true;
                    }

                }
                else if (postDetails.PostSource == PostSource.SharePost ||
                    (postDetails.PostSource == PostSource.RssFeedPost && postDetails.MediaList.Count == 0) ||
                   (postDetails.PostSource == PostSource.NormalPost && postDetails.MediaList.Count == 0 &&
                     !string.IsNullOrEmpty(postDetails.PdSourceUrl)))
                {
                    if (postDetails.PostSource == PostSource.NormalPost && postDetails.MediaList.Count == 0 &&
                     !string.IsNullOrEmpty(postDetails.PdSourceUrl))
                    {
                        var postdeatils = postDetails.DeepCloneObject();
                        postDetails.ShareUrl = postdeatils.PdSourceUrl;
                        responseHandler = fdRequestLibrary.ShareToPages(_accountModel, pageUrl, postDetails,
                        CampaignCancellationToken, GeneralSettingsModel, AdvanceModel);

                    }
                    else if (!string.IsNullOrEmpty(postDetails.ShareUrl))
                    {
                        responseHandler = fdRequestLibrary.ShareToPages(_accountModel, pageUrl, postDetails,
                            CampaignCancellationToken, GeneralSettingsModel, AdvanceModel);
                    }
                    else
                    {

                        responseHandler = fdRequestLibrary.PostToPages(_accountModel, pageUrl, postDetails, CampaignCancellationToken,
                            GeneralSettingsModel, AdvanceModel);

                    }

                    string ftEnrtIdentifier = string.Empty;
                    if (!responseHandler.Status)
                    {
                        UpdatePostWithFailed(pageUrl, postDetails, responseHandler.ObjFdScraperResponseParameters.FetchStream);

                        GlobusLogHelper.log.Info(Log.ShareFailed, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Pages", pageUrl);
                        if (isDelayNeeded)
                            DelayBeforeNextPublish();
                    }
                    else
                    {
                        UpdatePostWithSuccessful(pageUrl, postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);

                        // ftEnrtIdentifier = fdRequestLibrary.GetPostEntIdentifier(_accountModel, responseHandler.ObjFdScraperResponseParameters.FetchStream);

                        if (AdvanceModel.IsTurnOffNotificationsForNewPost)
                        {
                            var status = fdRequestLibrary.StopNotification(_accountModel, responseHandler.ObjFdScraperResponseParameters.FetchStream, ftEnrtIdentifier);
                            if (!status && _accountModel.IsRunProcessThroughBrowser)
                                status = _fdLoginProcess._browserManager.TurnOffCommentsOrNotificationsForPost(_accountModel, responseHandler?.ObjFdScraperResponseParameters?.PostDetails?.PostUrl, CampaignCancellationToken, turnOffNotification: true);
                            GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork,
                                _accountModel.AccountBaseModel.UserName, "",
                                status ? "Successfully turned off notification for this post"
                                    : "Turn off notification failed for this post");
                        }

                        GlobusLogHelper.log.Info(Log.SharedSuccessfully, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Pages", pageUrl);

                        if (AdvanceModel.IsDeletePostAfter)
                        {
                            AssignEnableDeletePost(postDetails, responseHandler.ObjFdScraperResponseParameters.FetchStream, pageUrl, "Pages");
                        }

                        if (isDelayNeeded)
                            DelayBeforeNextPublish();

                        return true;
                    }
                }
                else
                {
                    postDetails.PostDescription += $"\r\n{postDetails.PdSourceUrl}";

                    responseHandler = fdRequestLibrary.PostToPages(_accountModel, pageUrl, postDetails, CampaignCancellationToken,
                           GeneralSettingsModel, AdvanceModel);

                    string ftEnrtIdentifier = string.Empty;

                    if (!responseHandler.Status)
                    {
                        UpdatePostWithFailed(pageUrl, postDetails, responseHandler.ObjFdScraperResponseParameters.FetchStream);

                        GlobusLogHelper.log.Info(Log.PublishingFailedWithError, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Pages", pageUrl, responseHandler.ObjFdScraperResponseParameters.FetchStream);
                        if (isDelayNeeded)
                            DelayBeforeNextPublish();
                    }
                    else
                    {
                        UpdatePostWithSuccessful(pageUrl, postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);

                        // ftEnrtIdentifier = fdRequestLibrary.GetPostEntIdentifier(_accountModel, responseHandler.ObjFdScraperResponseParameters.FetchStream);

                        if (AdvanceModel.IsTurnOffNotificationsForNewPost)
                        {
                            var status = fdRequestLibrary.StopNotification(_accountModel, responseHandler.ObjFdScraperResponseParameters.FetchStream, ftEnrtIdentifier);
                            if (!status && _accountModel.IsRunProcessThroughBrowser)
                                status = _fdLoginProcess._browserManager.TurnOffCommentsOrNotificationsForPost(_accountModel, responseHandler?.ObjFdScraperResponseParameters?.PostDetails?.PostUrl, CampaignCancellationToken, turnOffNotification: true);
                            GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork,
                                _accountModel.AccountBaseModel.UserName, "",
                                status ? "Successfully turned off notification for this post"
                                    : "Turn off notification failed for this post");
                        }

                        if (AdvanceModel.IsHidePostInFacebook)
                        {
                            var entityIs = fdRequestLibrary.GetPageIdFromUrl(_accountModel, pageUrl);

                            var databaseConnection = SocinatorInitialize.GetSocialLibrary(_accountModel.AccountBaseModel.AccountNetwork)
                                .GetNetworkCoreFactory().AccountDatabase;

                            var dataConext = databaseConnection.GetSqlConnection(_accountModel.AccountBaseModel.AccountId);

                            var dbOperation = new DbOperations(dataConext);

                            if (dbOperation.Any<OwnPages>(x => x.PageId == entityIs))
                            {
                                fdRequestLibrary.HidePostFromOwnPage(_accountModel, ftEnrtIdentifier, pageUrl);

                                GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork, _accountModel.AccountBaseModel.UserName, "", "Successfully hidden post from page timeline");
                            }

                        }

                        GlobusLogHelper.log.Info(Log.PublishingSuccessfully, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Pages", pageUrl);

                        if (AdvanceModel.IsDeletePostAfter)
                            AssignEnableDeletePost(postDetails, responseHandler.ObjFdScraperResponseParameters.FetchStream, pageUrl, "Pages");

                        if (isDelayNeeded)
                            DelayBeforeNextPublish();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }



        public bool PublishOnEvent(string accountId, string eventUrl,
            PublisherPostlistModel givenPostDetails, bool isDelayNeeded)
        {
            try
            {

                var fdRequestLibrary =
                    _accountScopeFactory[$"{accountId}_Publisher"].Resolve<IFdRequestLibrary>();

                GlobusLogHelper.log.Info(Log.AccountLogin, _accountModel.AccountBaseModel.AccountNetwork,
                    _accountModel.AccountBaseModel.UserName);

                var isSuccess = fdRequestLibrary.Login(_accountModel);

                if (!isSuccess)
                    return false;

                GlobusLogHelper.log.Info(Log.SuccessfulLogin, _accountModel.AccountBaseModel.AccountNetwork,
                    _accountModel.AccountBaseModel.UserName);

                var postDetails = PerformGeneralSettings(givenPostDetails);
                if (string.IsNullOrEmpty(givenPostDetails.PostDescription) && postDetails.PostDescription == new FileInfo(postDetails.MediaList.FirstOrDefault()).Name)
                    postDetails.PostDescription = givenPostDetails.PostDescription;
                if (OtherConfiguration.IsEnableSignatureChecked)
                    postDetails.PostDescription = postDetails.PostDescription + " ";
                if (postDetails.PostSource != PostSource.SharePost && postDetails.PostSource != PostSource.RssFeedPost)
                    return false;

                if (postDetails.PostSource == PostSource.RssFeedPost && postDetails.MediaList.Count > 0)
                    return false;

                var responseHandler = fdRequestLibrary.ShareToEvents(_accountModel, eventUrl, postDetails,
                    CampaignCancellationToken, GeneralSettingsModel, AdvanceModel);

                string ftEnrtIdentifier = string.Empty;

                if (!responseHandler.Status)
                {
                    UpdatePostWithFailed(eventUrl, postDetails, responseHandler.FbErrorDetails.Description);
                    // ftEnrtIdentifier = fdRequestLibrary.GetPostEntIdentifier(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);


                    if (AdvanceModel.IsTurnOffNotificationsForNewPost)
                    {
                        var status = fdRequestLibrary.StopNotification(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, ftEnrtIdentifier);
                        if (!status && _accountModel.IsRunProcessThroughBrowser)
                            status = _fdLoginProcess._browserManager.TurnOffCommentsOrNotificationsForPost(_accountModel, responseHandler?.ObjFdScraperResponseParameters?.PostDetails?.PostUrl, CampaignCancellationToken, turnOffNotification: true);
                        GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "",
                            status ? "Successfully turned off notification for this post"
                                : "Turn off notification failed for this post");
                    }

                    GlobusLogHelper.log.Info(Log.ShareFailed, _accountModel.AccountBaseModel.AccountNetwork,
                        _accountModel.AccountBaseModel.UserName, "Events", eventUrl);
                    if (isDelayNeeded)
                        DelayBeforeNextPublish();
                }
                else
                {
                    UpdatePostWithSuccessful(eventUrl, postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);

                    // ftEnrtIdentifier = fdRequestLibrary.GetPostEntIdentifier(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);

                    if (AdvanceModel.IsTurnOffNotificationsForNewPost)
                    {
                        var status = fdRequestLibrary.StopNotification(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, ftEnrtIdentifier);
                        if (!status && _accountModel.IsRunProcessThroughBrowser)
                            status = _fdLoginProcess._browserManager.TurnOffCommentsOrNotificationsForPost(_accountModel, responseHandler?.ObjFdScraperResponseParameters?.PostDetails?.PostUrl, CampaignCancellationToken, turnOffNotification: true);
                        GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "",
                            status ? "Successfully turned off notification for this post"
                                : "Turn off notification failed for this post");
                    }

                    if (AdvanceModel.IsDeletePostAfter)
                        AssignEnableDeletePost(postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, eventUrl, "Profile");


                    GlobusLogHelper.log.Info(Log.SharedSuccessfully, _accountModel.AccountBaseModel.AccountNetwork,
                        _accountModel.AccountBaseModel.UserName, "Events", eventUrl);

                    if (AdvanceModel.IsDeletePostAfter)
                        AssignEnableDeletePost(postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, eventUrl, "Event");

                    if (isDelayNeeded)
                        DelayBeforeNextPublish();

                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }



        //Todo : Need to implement functionality for publishing normal post to friend wall
        public bool PublishOnFriend(string accountId, string friendUrl, PublisherPostlistModel givenPostDetails, bool isDelayNeeded)
        {
            try
            {
                //from insta post to facebook Post.
                if (string.IsNullOrEmpty(givenPostDetails.PostDescription) && !string.IsNullOrEmpty(givenPostDetails.PublisherInstagramTitle))
                    givenPostDetails.PostDescription = givenPostDetails.PublisherInstagramTitle;

                var fdRequestLibrary =
                    _accountScopeFactory[$"{accountId}_Publisher"].Resolve<IFdRequestLibrary>();

                GlobusLogHelper.log.Info(Log.AccountLogin, _accountModel.AccountBaseModel.AccountNetwork, _accountModel.AccountBaseModel.UserName);


                var isSuccess = false;

                if (!AccountModel.IsRunProcessThroughBrowser)
                    isSuccess = fdRequestLibrary.Login(_accountModel);
                else
                {
                    _fdLoginProcess = _accountScopeFactory[$"{accountId}_Publisher_{givenPostDetails.PostId}"].Resolve<IFdLoginProcess>();

                    _fdLoginProcess.LoginWithBrowserMethod(_accountModel, CampaignCancellationToken.Token);

                    isSuccess = _accountModel.IsUserLoggedIn;
                }

                if (!isSuccess)
                {
                    if (AccountModel.IsRunProcessThroughBrowser)
                        _fdLoginProcess._browserManager.CloseBrowser(_accountModel);
                    return false;
                }

                GlobusLogHelper.log.Info(Log.SuccessfulLogin, _accountModel.AccountBaseModel.AccountNetwork, _accountModel.AccountBaseModel.UserName);
                IResponseHandler responseHandler1;
                var postDetails = PerformGeneralSettings(givenPostDetails);
                if (string.IsNullOrEmpty(givenPostDetails.PostDescription) && postDetails.PostDescription == new FileInfo(postDetails.MediaList.FirstOrDefault()).Name)
                    postDetails.PostDescription = givenPostDetails.PostDescription;
                if (OtherConfiguration.IsEnableSignatureChecked)
                    postDetails.PostDescription = postDetails.PostDescription + " ";
                if (AccountModel.IsRunProcessThroughBrowser)
                {

                    responseHandler1 = _fdLoginProcess._browserManager.ShareToFriendProfiles(_accountModel, friendUrl, postDetails, CampaignCancellationToken,
                        GeneralSettingsModel, AdvanceModel);

                    string ftEnrtIdentifier = string.Empty;

                    if (!responseHandler1.Status)
                    {
                        UpdatePostWithFailed(friendUrl, postDetails, responseHandler1.ObjFdScraperResponseParameters.FetchStream);

                        GlobusLogHelper.log.Info(Log.ShareFailed, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Friend Profile", friendUrl);
                        if (isDelayNeeded)
                            DelayBeforeNextPublish();
                    }
                    else
                    {
                        UpdatePostWithSuccessful(friendUrl, postDetails, responseHandler1.ObjFdScraperResponseParameters.PostDetails.PostUrl);

                        // ftEnrtIdentifier = fdRequestLibrary.GetPostEntIdentifier(_accountModel, responseHandler1.ObjFdScraperResponseParameters.PostDetails.PostUrl);

                        if (AdvanceModel.IsTurnOffNotificationsForNewPost)
                        {
                            var status = fdRequestLibrary.StopNotification(_accountModel, responseHandler1.ObjFdScraperResponseParameters.PostDetails.PostUrl, ftEnrtIdentifier);
                            if (!status && _accountModel.IsRunProcessThroughBrowser)
                                _fdLoginProcess._browserManager.TurnOffCommentsOrNotificationsForPost(_accountModel, responseHandler1?.ObjFdScraperResponseParameters?.PostDetails?.PostUrl, CampaignCancellationToken, turnOffNotification: true);
                            GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork,
                                _accountModel.AccountBaseModel.UserName, "",
                                status ? "Successfully turned off notification for this post"
                                    : "Turn off notification failed for this post");
                        }

                        if (AdvanceModel.IsDeletePostAfter)
                            AssignEnableDeletePost(postDetails, responseHandler1.ObjFdScraperResponseParameters.PostDetails.PostUrl, friendUrl, "Profile");

                        GlobusLogHelper.log.Info(Log.SharedSuccessfully, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Friend Profile", friendUrl);

                        if (AdvanceModel.IsDeletePostAfter)
                            AssignEnableDeletePost(postDetails, responseHandler1.ObjFdScraperResponseParameters.PostDetails.PostUrl, friendUrl, "Friend");

                        _fdLoginProcess._browserManager.CloseBrowser(_accountModel);

                        if (isDelayNeeded)
                            DelayBeforeNextPublish();

                        return true;
                    }

                }
                else if (postDetails.PostSource == PostSource.SharePost
                    || (postDetails.PostSource == PostSource.RssFeedPost && postDetails.MediaList.Count == 0) ||
                   (postDetails.PostSource == PostSource.NormalPost && postDetails.MediaList.Count == 0 &&
                     !string.IsNullOrEmpty(postDetails.PdSourceUrl)))
                {
                    CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                    PublisherResponseHandler responseHandler;
                    if (postDetails.PostSource == PostSource.NormalPost && postDetails.MediaList.Count == 0 &&
                     !string.IsNullOrEmpty(postDetails.PdSourceUrl))
                    {
                        var postdeatils = postDetails.DeepCloneObject();
                        postDetails.ShareUrl = postdeatils.PdSourceUrl;
                        responseHandler = fdRequestLibrary.ShareToFriendProfiles(_accountModel, friendUrl, postDetails,
                        CampaignCancellationToken, GeneralSettingsModel, AdvanceModel);

                    }
                    else
                    {
                        responseHandler = fdRequestLibrary.ShareToFriendProfiles(_accountModel, friendUrl, postDetails,
                        CampaignCancellationToken, GeneralSettingsModel, AdvanceModel);

                    }

                    if (!responseHandler.Status)
                    {
                        UpdatePostWithFailed(friendUrl, postDetails, responseHandler.FbErrorDetails.Description);

                        GlobusLogHelper.log.Info(Log.ShareFailed, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Friend Profile", friendUrl);
                        if (isDelayNeeded)
                            DelayBeforeNextPublish();
                    }
                    else
                    {
                        UpdatePostWithSuccessful(friendUrl, postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);

                        string ftEnrtIdentifier = string.Empty;

                        // ftEnrtIdentifier = fdRequestLibrary.GetPostEntIdentifier(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);

                        if (AdvanceModel.IsTurnOffNotificationsForNewPost)
                        {
                            var status = fdRequestLibrary.StopNotification(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, ftEnrtIdentifier);
                            if (!status && _accountModel.IsRunProcessThroughBrowser)
                                status = _fdLoginProcess._browserManager.TurnOffCommentsOrNotificationsForPost(_accountModel, responseHandler?.ObjFdScraperResponseParameters?.PostDetails?.PostUrl, CampaignCancellationToken, turnOffNotification: true);
                            GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork,
                                _accountModel.AccountBaseModel.UserName, "",
                                status ? "Successfully turned off notification for this post"
                                    : "Turn off notification failed for this post");
                        }

                        if (AdvanceModel.IsDeletePostAfter)
                        {
                            AssignEnableDeletePost(postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, friendUrl, "Profile");
                        }


                        GlobusLogHelper.log.Info(Log.SharedSuccessfully, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Friend Profile", friendUrl);

                        if (AdvanceModel.IsDeletePostAfter)
                        {
                            AssignEnableDeletePost(postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, friendUrl, "Friend");
                        }

                        if (isDelayNeeded)
                            DelayBeforeNextPublish();

                        _fdLoginProcess._browserManager.CloseBrowser(AccountModel);

                        return true;
                    }
                }
                else
                {
                    postDetails.PostDescription += $"\r\n{postDetails.PdSourceUrl}";

                    var responseHandler = fdRequestLibrary.PostToFriends(_accountModel, friendUrl, postDetails,
                        CampaignCancellationToken, GeneralSettingsModel, AdvanceModel);

                    if (!responseHandler.Status)
                    {
                        UpdatePostWithFailed(friendUrl, postDetails, responseHandler.FbErrorDetails.Description);

                        GlobusLogHelper.log.Info(Log.PublishingFailed, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Friends", friendUrl);
                        if (isDelayNeeded)
                            DelayBeforeNextPublish();
                    }
                    else
                    {
                        UpdatePostWithSuccessful(friendUrl, postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);

                        GlobusLogHelper.log.Info(Log.PublishingSuccessfully, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "Friends", friendUrl);

                        string ftEnrtIdentifier = string.Empty;

                        // ftEnrtIdentifier = fdRequestLibrary.GetPostEntIdentifier(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);

                        if (AdvanceModel.IsTurnOffNotificationsForNewPost)
                        {
                            var status = fdRequestLibrary.StopNotification(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, ftEnrtIdentifier);
                            if (!status && _accountModel.IsRunProcessThroughBrowser)
                                status = _fdLoginProcess._browserManager.TurnOffCommentsOrNotificationsForPost(_accountModel, responseHandler?.ObjFdScraperResponseParameters?.PostDetails?.PostUrl, CampaignCancellationToken, turnOffNotification: true);
                            GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork,
                                _accountModel.AccountBaseModel.UserName, "",
                                status ? "Successfully turned off notification for this post"
                                    : "Turn off notification failed for this post");
                        }

                        if (AdvanceModel.IsDeletePostAfter)
                        {
                            AssignEnableDeletePost(postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, friendUrl, "Friend");
                        }


                        if (isDelayNeeded)
                            DelayBeforeNextPublish();

                        _fdLoginProcess._browserManager.CloseBrowser(AccountModel);

                        return true;
                    }
                }

            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancellated!");
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                _fdLoginProcess._browserManager.CloseBrowser(AccountModel);
                ex.DebugLog();
            }
            return false;
        }



        public bool ShareToProfileAsPrivateMessage(string accountId, string profileUrl, PublisherPostlistModel givenPostDetails, bool isDelayNeeded)
        {
            try
            {
                // var fdRequestLibrary = InstanceProvider.GetInstance<IFdRequestLibrary>();
                //var fdRequestLibrary = new FdRequestLibrary();

                var fdRequestLibrary =
                    _accountScopeFactory[$"{accountId}_Publisher"].Resolve<IFdRequestLibrary>();

                GlobusLogHelper.log.Info(Log.AccountLogin, _accountModel.AccountBaseModel.AccountNetwork, _accountModel.AccountBaseModel.UserName);

                var isSuccess = fdRequestLibrary.Login(_accountModel);

                if (!isSuccess)
                {
                    return false;
                }

                GlobusLogHelper.log.Info(Log.SuccessfulLogin, _accountModel.AccountBaseModel.AccountNetwork, _accountModel.AccountBaseModel.UserName);

                var postDetails =
                    PerformGeneralSettings(givenPostDetails);
                if (string.IsNullOrEmpty(givenPostDetails.PostDescription) && postDetails.PostDescription == new FileInfo(givenPostDetails.MediaList.FirstOrDefault()).Name)
                    postDetails.PostDescription = givenPostDetails.PostDescription;
                var responseHandler = fdRequestLibrary.ShareToFriendAsPrivateMessage(_accountModel, profileUrl, postDetails, CampaignCancellationToken, AdvanceModel);

                if (!responseHandler.Status)
                {
                    UpdatePostWithFailed(profileUrl, postDetails, responseHandler.FbErrorDetails.Description);

                    string ftEnrtIdentifier = string.Empty;

                    // ftEnrtIdentifier = fdRequestLibrary.GetPostEntIdentifier(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);

                    if (AdvanceModel.IsTurnOffNotificationsForNewPost)
                    {
                        var status = fdRequestLibrary.StopNotification(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, ftEnrtIdentifier);
                        if (!status && _accountModel.IsRunProcessThroughBrowser)
                            status = _fdLoginProcess._browserManager.TurnOffCommentsOrNotificationsForPost(_accountModel, responseHandler?.ObjFdScraperResponseParameters?.PostDetails?.PostUrl, CampaignCancellationToken, turnOffNotification: true);
                        GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "",
                            status ? "Successfully turned off notification for this post"
                                : "Turn off notification failed for this post");
                    }

                    if (AdvanceModel.IsDeletePostAfter)
                    {
                        AssignEnableDeletePost(postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, profileUrl, "Profile");
                    }


                    GlobusLogHelper.log.Info(Log.ShareFailed, _accountModel.AccountBaseModel.AccountNetwork,
                        _accountModel.AccountBaseModel.UserName, "Custom profile as private message", profileUrl);
                    if (isDelayNeeded)
                        DelayBeforeNextPublish();
                }
                else
                {
                    UpdatePostWithSuccessful(profileUrl, postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);

                    GlobusLogHelper.log.Info(Log.SharedSuccessfully, _accountModel.AccountBaseModel.AccountNetwork,
                        _accountModel.AccountBaseModel.UserName, "Custom profile as private message", profileUrl);

                    string ftEnrtIdentifier = string.Empty;

                    // ftEnrtIdentifier = fdRequestLibrary.GetPostEntIdentifier(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl);

                    if (AdvanceModel.IsTurnOffNotificationsForNewPost)
                    {
                        var status = fdRequestLibrary.StopNotification(_accountModel, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, ftEnrtIdentifier);
                        if (!status && _accountModel.IsRunProcessThroughBrowser)
                            status = _fdLoginProcess._browserManager.TurnOffCommentsOrNotificationsForPost(_accountModel, responseHandler?.ObjFdScraperResponseParameters?.PostDetails?.PostUrl, CampaignCancellationToken, turnOffNotification: true);
                        GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork,
                            _accountModel.AccountBaseModel.UserName, "",
                            status ? "Successfully turned off notification for this post"
                                : "Turn off notification failed for this post");
                    }

                    if (AdvanceModel.IsDeletePostAfter)
                    {
                        AssignEnableDeletePost(postDetails, responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl, profileUrl, "Profile");
                    }


                    if (isDelayNeeded)
                        DelayBeforeNextPublish();

                    return true;
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }


        public void ApplyDynamicHashTag(ref PublisherPostlistModel postDetails)
        {
            try
            {
                var fullCaption = string.Empty;

                if (!string.IsNullOrEmpty(postDetails.PostDescription))
                    fullCaption = postDetails.PostDescription;

                // Enable Automatic Hashtags
                #region Enable Automatic Hashtags

                if (AdvanceModel.IsEnableAutomaticHashTags)
                {
                    try
                    {
                        List<string> lstHashKeywords = Regex.Split(AdvanceModel.HashWords, ",")
                            .Where(x => !string.IsNullOrEmpty(x)).ToList();

                        lstHashKeywords.Shuffle();

                        for (int hashtagAddCount = 0;
                            hashtagAddCount < AdvanceModel.MaxHashtagsPerPost;
                            hashtagAddCount++)
                        {
                            fullCaption += $" #{lstHashKeywords[hashtagAddCount].Trim()}";
                        }


                        if (lstHashKeywords.Count < AdvanceModel.MaxHashtagsPerPost)
                        {
                            var lstHashFromCaptionByWordLength = Regex.Split(postDetails.PostDescription, " ")
                                .Where(x => IsAlphaNumeric(x) &&
                                            x.Length >= AdvanceModel.MinimumWordLength)
                                .ToList();

                            lstHashFromCaptionByWordLength.Shuffle();

                            int hashtagCountToAdd =
                                AdvanceModel.MaxHashtagsPerPost - lstHashKeywords.Count;

                            for (int hashtagAddCount = 0;
                                hashtagAddCount < hashtagCountToAdd;
                                hashtagAddCount++)
                            {
                                fullCaption += $"#{lstHashFromCaptionByWordLength[hashtagAddCount].Trim()}";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                #endregion

                #region Enable Dynamic Hashtags

                if (AdvanceModel.IsEnableDynamicHashTags &&
                    AdvanceModel.IsAddHashTagEvenIfAlreadyHastags)
                {
                    try
                    {
                        int maxHashtagRangePerPost = AdvanceModel.MaxHashtagsPerPostRange.GetRandom();

                        var hashTagsPresentInCaption = Regex.Split(fullCaption, "#")
                            .Where(x => !string.IsNullOrEmpty(Utilities.GetBetween($"{x}##", "", "##")
                                .Trim()))
                            .ToList();

                        if (!fullCaption.StartsWith("#"))
                        {
                            hashTagsPresentInCaption.RemoveAt(0);
                        }

                        int presentHashtagCount = hashTagsPresentInCaption.Count;

                        int hashtagCountToAdd = (presentHashtagCount < maxHashtagRangePerPost)
                            ? maxHashtagRangePerPost - presentHashtagCount
                            : 0;

                        if (hashtagCountToAdd != 0)
                        {
                            #region Hashtags from List1

                            int percentCountFromHashtag =
                                hashtagCountToAdd * AdvanceModel.PickPercentHashTag / 100;
                            List<string> lstHashTagFromList1 = Regex
                                .Split(AdvanceModel.HashtagsFromList1, ",")
                                .Where(x => !string.IsNullOrEmpty(x)).ToList();

                            for (int hashtagcount = 0;
                                hashtagcount < percentCountFromHashtag;
                                hashtagcount++)
                            {
                                fullCaption += $" #{lstHashTagFromList1[hashtagcount].Trim()}";
                            }

                            #endregion

                            #region Hashtags from List2

                            int percentCountFromList = hashtagCountToAdd - percentCountFromHashtag;
                            List<string> lstHashTagFromList2 = Regex
                                .Split(AdvanceModel.HashtagsFromList2, ",")
                                .Where(x => !string.IsNullOrEmpty(x)).ToList();

                            for (int hashtagCount = 0; hashtagCount < percentCountFromList; hashtagCount++)
                            {
                                fullCaption += $" #{lstHashTagFromList2[hashtagCount].Trim()}";
                            }

                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                #endregion
                //if (string.IsNullOrEmpty(postDetails.PostDescription))
                postDetails.PostDescription = fullCaption;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        public void PublisherAfterActionForSuccess(DominatorAccountModel _accountModel, PublisherResponseHandler
            responseHandler, PublisherPostlistModel postDetails, PostOptions objPostOption, string destinationId,
            bool isDelayNeeded = true)
        {

            var fdRequestLibrary =
                _accountScopeFactory[$"{_accountModel.AccountId}_Publisher"].Resolve<IFdRequestLibrary>();

            UpdatePostWithSuccessful(_accountModel.AccountBaseModel.UserName, postDetails,
                responseHandler.ObjFdScraperResponseParameters?.PostDetails?.PostUrl);

            string ftEnrtIdentifier = string.Empty;

            // ftEnrtIdentifier = fdRequestLibrary.GetPostEntIdentifier(_accountModel, responseHandler?.ObjFdScraperResponseParameters?.PostDetails?.PostUrl);

            if (AdvanceModel.IsTurnOffNotificationsForNewPost)
            {
                var status = fdRequestLibrary.StopNotification(_accountModel, responseHandler.ObjFdScraperResponseParameters?.PostDetails?.PostUrl, ftEnrtIdentifier);
                if (!status && _accountModel.IsRunProcessThroughBrowser)
                    status = _fdLoginProcess._browserManager.TurnOffCommentsOrNotificationsForPost(_accountModel, responseHandler?.ObjFdScraperResponseParameters?.PostDetails?.PostUrl, CampaignCancellationToken, turnOffNotification: true);
                GlobusLogHelper.log.Info(Log.CustomMessage, _accountModel.AccountBaseModel.AccountNetwork,
                    _accountModel.AccountBaseModel.UserName, "",
                    status ? "Successfully turned off notification for this post"
                        : "Turn off notification failed for this post");
            }
            if (AdvanceModel.IsDisableCommentsForNewPost)
            {
                _fdLoginProcess._browserManager.DisablePostComment(_accountModel, responseHandler?.ObjFdScraperResponseParameters?.PostDetails, objPostOption);
            }

            GlobusLogHelper.log.Info(Log.SharedSuccessfully, _accountModel.AccountBaseModel.AccountNetwork,
                _accountModel.AccountBaseModel.UserName, objPostOption.ToString(), $"{FdConstants.FbHomeUrl}{destinationId}");
            if (AdvanceModel.IsDeletePostAfter)
            {
                AssignEnableDeletePost(postDetails, responseHandler?.ObjFdScraperResponseParameters?.PostDetails?.PostUrl, "", "Wall");
            }




            if (isDelayNeeded)
                DelayBeforeNextPublish();
        }

        public void PublisherAfterActionForFailed(DominatorAccountModel _accountModel, PublisherResponseHandler
            responseHandler, PublisherPostlistModel postDetails, PostOptions objPostOption, string destinationId,
            bool isDelayNeeded = true)
        {
            UpdatePostWithFailed(_accountModel.AccountBaseModel.UserName, postDetails,
                responseHandler?.FbErrorDetails?.Description);

            GlobusLogHelper.log.Info(Log.ShareFailed, _accountModel.AccountBaseModel.AccountNetwork,
                _accountModel.AccountBaseModel.UserName, objPostOption.ToString(), $"{FdConstants.FbHomeUrl}{destinationId}");
            if (isDelayNeeded)
                DelayBeforeNextPublish();
        }


        private bool IsAlphaNumeric(string word)
        {
            return new Regex("^[a-zA-Z0-9]*$").IsMatch(word);
        }

    }
}