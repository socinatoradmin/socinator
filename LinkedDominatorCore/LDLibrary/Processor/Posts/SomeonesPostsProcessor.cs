using System;
using System.Collections.Generic;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel.Engage;
using LinkedDominatorCore.LDModel.Filters;

namespace LinkedDominatorCore.LDLibrary.Processor.Posts
{
    public class SomeonesPostsProcessor : BaseLinkedinPostProcessor
    {
        private readonly IDelayService _delayService;
        private readonly IProcessScopeModel _processScopeModel;
        private readonly ILdUserFilterProcess ldUserFilterProcess;
        private LDUserFilterModel userFilterModel;
        private readonly ILdFunctions _ldFunctions;
        public SomeonesPostsProcessor(ILdJobProcess jobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory,
            IProcessScopeModel processScopeModel, IDelayService delayService) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
            _processScopeModel = processScopeModel;
            _delayService = delayService;
            ldUserFilterProcess = InstanceProvider.GetInstance<ILdUserFilterProcess>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var isCheckedFilterProfileImageCheckbox = false;
                var IsCheckSkipBlackListedUser = false;
                var isChkPrivateBlackList = false;
                var isChkGroupBlackList = false;
                var maxNumberOfActionPerUser = 0;
                List<string> lstCommentInDom = null;

                switch (ActivityType)
                {
                    case ActivityType.Like:
                        {
                            var likeModel = _processScopeModel.GetActivitySettingsAs<LikeModel>();
                            userFilterModel = likeModel.LDUserFilterModel;
                            isCheckedFilterProfileImageCheckbox =
                                likeModel.LDUserFilterModel.IsCheckedFilterProfileImageCheckbox;
                            isChkPrivateBlackList = likeModel.IsChkPrivateBlackList;
                            IsCheckSkipBlackListedUser = likeModel.IsChkSkipBlackListedUser;
                            isChkGroupBlackList = likeModel.IsChkGroupBlackList;
                            maxNumberOfActionPerUser =
                                likeModel.IsNumberOfPostToLike ? likeModel.MaxNumberOfPostPerUserToLike : 0;
                            break;
                        }

                    case ActivityType.Comment:
                        {
                            var commentModel = _processScopeModel.GetActivitySettingsAs<CommentModel>();
                            userFilterModel = commentModel.LDUserFilterModel;
                            isCheckedFilterProfileImageCheckbox =
                                commentModel.LDUserFilterModel.IsCheckedFilterProfileImageCheckbox;
                            IsCheckSkipBlackListedUser = commentModel.IsChkSkipBlackListedUser;
                            isChkPrivateBlackList = commentModel.IsChkPrivateBlackList;
                            isChkGroupBlackList = commentModel.IsChkGroupBlackList;
                            lstCommentInDom = new List<string>();
                            commentModel.LstDisplayManageCommentModel.ForEach(x =>
                            {
                                if (!lstCommentInDom.Contains(x.CommentText))
                                    lstCommentInDom.Add(x.CommentText);
                            });
                            maxNumberOfActionPerUser = commentModel.IsNumberOfPostToComment
                                ? commentModel.MaxNumberOfPostPerUserToComment
                                : 0;
                            break;
                        }

                    case ActivityType.Share:
                        {
                            var shareModel = _processScopeModel.GetActivitySettingsAs<ShareModel>();
                            userFilterModel = shareModel.LDUserFilterModel;
                            isCheckedFilterProfileImageCheckbox =
                                shareModel.LDUserFilterModel.IsCheckedFilterProfileImageCheckbox;
                            IsCheckSkipBlackListedUser = shareModel.IsChkSkipBlackListedUser;
                            isChkPrivateBlackList = shareModel.IsChkPrivateBlackList;
                            isChkGroupBlackList = shareModel.IsChkGroupBlackList;
                            maxNumberOfActionPerUser = shareModel.IsNumberOfPostToShare
                                ? shareModel.MaxNumberOfPostPerUserToShare
                                : 0;
                            break;
                        }
                }
                #region Social Activities(Engage) On SomeonesPosts

                var profileUrl = queryInfo.QueryValue;
                if (profileUrl.Contains("http:") && profileUrl.Contains("https:") &&
                    (jobProcessResult.HasNoResult = true))
                    return;
                if(profileUrl.Contains("recent-activity/shares/") || profileUrl.Length>100)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,LdJobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,LdJobProcess.DominatorAccountModel.UserName, LdJobProcess.ActivityType,$"Invalid Profile Url has been Provided,Please provide Someone's profile Url to perform {LdJobProcess.ActivityType} operation.");
                    jobProcessResult.HasNoResult = true;
                    return;
                }
                if (!profileUrl.Contains("www.linkedin.com"))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,LdJobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,LdJobProcess.DominatorAccountModel.UserName, LdJobProcess.ActivityType,$"LinkedIn Profile Url Expected To Perform {LdJobProcess.ActivityType} Operation, {{{profileUrl}}} Doesn't Belongs To Any LinkedIn Profile.");
                    jobProcessResult.HasNoResult = true;
                    return;
                }
                if (IsCheckSkipBlackListedUser && (isChkPrivateBlackList || isChkGroupBlackList) &&
                    manageBlacklistWhitelist.FilterBlackListedUser(profileUrl, isChkPrivateBlackList,
                        isChkGroupBlackList))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"Skipped Black Listed User");
                    jobProcessResult.HasNoResult = true;
                    return;
                }

                if (profileUrl.Contains("company"))
                {
                    var getUserInfo = new GetDetailedUserInfo(_delayService);
                    var companyDetails = getUserInfo.ScrapeCompanyDetails(profileUrl, LdFunctions);
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "getting posts created by " + companyDetails?.CompanyName?.Replace("\"", "") + "");

                    jobProcessResult = ScrapeCompanyPostsAndMoveToFinalProcess(jobProcessResult, queryInfo,
                        companyDetails, lstCommentInDom);
                }
                else
                {
                    var linkedinUser = GetUserInformation(profileUrl, isCheckedFilterProfileImageCheckbox);
                    if (ldUserFilterProcess.IsUserFilterActive(userFilterModel))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "Filtering User ==> " + linkedinUser.FullName);
                        var isValidUser = ldUserFilterProcess.GetFilterStatus(linkedinUser.ProfileUrl,userFilterModel, _ldFunctions);
                        if (!isValidUser)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,DominatorAccountModel.AccountBaseModel.AccountNetwork,DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "[ " + linkedinUser.FullName +" ] is not a valid user according to the filter.");
                            jobProcessResult.HasNoResult = true;
                            return;
                        }else
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"All Filtered Matched SuccessFully,Proceeding For {ActivityType} Of {linkedinUser.FullName}");
                    }
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "getting posts created by " + linkedinUser?.FullName?.Replace("\"", "") + "");

                    jobProcessResult = ScrapeUserPostsAndMoveToFinalProcess(jobProcessResult, queryInfo,
                        isCheckedFilterProfileImageCheckbox, linkedinUser.ProfileId, linkedinUser.PublicIdentifier,
                        lstCommentInDom, maxNumberOfActionPerUser);
                }

                #endregion
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                jobProcessResult.HasNoResult = true;
                ex.DebugLog();
            }
        }
    }
}