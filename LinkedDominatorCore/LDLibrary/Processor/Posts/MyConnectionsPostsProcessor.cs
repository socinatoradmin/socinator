using System;
using System.Collections.Generic;
using System.Linq;
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
using LinkedDominatorCore.LDUtility;

namespace LinkedDominatorCore.LDLibrary.Processor.Posts
{
    public class MyConnectionsPostsProcessor : BaseLinkedinPostProcessor
    {
        private readonly IProcessScopeModel _processScopeModel;
        protected readonly ILdUserFilterProcess LdUserFilterProcess;

        public MyConnectionsPostsProcessor(ILdJobProcess jobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory,
            IProcessScopeModel processScopeModel, IDelayService delayService) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
            _processScopeModel = processScopeModel;
            LdUserFilterProcess = InstanceProvider.GetInstance<ILdUserFilterProcess>();
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                var isCheckedFilterProfileImageCheckbox = false;
                var isChkPrivateBlackList = false;
                var isChkGroupBlackList = false;
                var maxNumberOfActionPerPost = 0;
                var IsUserFilterActive = false;
                List<string> lstCommentInDom = null;
                dynamic Model=null;
                if (ActivityType != ActivityType.Like && ActivityType != ActivityType.Comment &&
                    ActivityType != ActivityType.Share)
                {
                    jobProcessResult.HasNoResult = true;
                    return;
                }

                #region separate this region as a method later

                switch (ActivityType)
                {
                    case ActivityType.Like:
                    {
                        var likeModel = _processScopeModel.GetActivitySettingsAs<LikeModel>();
                        isCheckedFilterProfileImageCheckbox =
                            likeModel.LDUserFilterModel.IsCheckedFilterProfileImageCheckbox;
                        isChkPrivateBlackList = likeModel.IsChkPrivateBlackList;
                        isChkGroupBlackList = likeModel.IsChkGroupBlackList;
                        maxNumberOfActionPerPost = likeModel.IsNumberOfPostToLike ? likeModel.MaxNumberOfPostPerUserToLike : 0;
                        IsUserFilterActive = LdUserFilterProcess.IsUserFilterActive(likeModel.LDUserFilterModel);
                        Model = likeModel;
                        break;
                    }

                    case ActivityType.Comment:
                    {
                        var commentModel = _processScopeModel.GetActivitySettingsAs<CommentModel>();
                        isCheckedFilterProfileImageCheckbox =
                            commentModel.LDUserFilterModel.IsCheckedFilterProfileImageCheckbox;
                        isChkPrivateBlackList = commentModel.IsChkPrivateBlackList;
                        isChkGroupBlackList = commentModel.IsChkGroupBlackList;
                        lstCommentInDom = new List<string>();
                        commentModel.LstDisplayManageCommentModel.ForEach(x =>
                        {
                            if (!lstCommentInDom.Contains(x.CommentText))
                                lstCommentInDom.Add(x.CommentText);
                        });
                        maxNumberOfActionPerPost = commentModel.IsNumberOfPostToComment
                            ? commentModel.MaxNumberOfPostPerUserToComment
                            : 0;
                        IsUserFilterActive = LdUserFilterProcess.IsUserFilterActive(commentModel.LDUserFilterModel);
                        Model = commentModel;
                            break;
                    }

                    case ActivityType.Share:
                    {
                        var shareModel = _processScopeModel.GetActivitySettingsAs<ShareModel>();
                        isCheckedFilterProfileImageCheckbox =
                            shareModel.LDUserFilterModel.IsCheckedFilterProfileImageCheckbox;
                        isChkPrivateBlackList = shareModel.IsChkPrivateBlackList;
                        isChkGroupBlackList = shareModel.IsChkGroupBlackList;
                        maxNumberOfActionPerPost = shareModel.IsNumberOfPostToShare
                            ? shareModel.MaxNumberOfPostPerUserToShare
                            : 0;
                        IsUserFilterActive = LdUserFilterProcess.IsUserFilterActive(shareModel.LDUserFilterModel);
                        Model = shareModel;
                            break;
                    }
                }

                #endregion


                #region Social Activities(Engage) On MyConnectionsPosts

                var lstMyConnections = DbAccountService.GetConnections().ToList();
                if (lstMyConnections.Count > 0)
                {
                    if (isCheckedFilterProfileImageCheckbox)
                    {
                        var count = lstMyConnections.RemoveAll(x => string.IsNullOrEmpty(x.ProfilePicUrl));
                        if(count > 0)
                            GlobusLogHelper.log.Info(Log.CustomMessage,DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "successfully filtered [ " + count + " Connections ] having no profile picture");
                    }

                    if (isChkPrivateBlackList || isChkGroupBlackList)
                        FilterBlacklistedUsers(lstMyConnections, isChkPrivateBlackList, isChkGroupBlackList);

                    foreach (var connection in lstMyConnections)
                    {
                        LdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        var publicIdentifier = Utils.GetBetween(connection.ProfileUrl + "**", "/in/", "**").Trim();
                        if (IsUserFilterActive &&Model != null)
                        {
                            var isValidUser = LdUserFilterProcess.GetFilterStatus(connection.ProfileUrl,
                            Model.LDUserFilterModel, LdFunctions);
                            if (!isValidUser)
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                    string.Format("LangKeyNotAValidUserAccordingToTheFilter".FromResourceDictionary(),
                                        connection.FullName));
                                continue;
                            }
                        }
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            "getting posts created by " + connection.FullName.Replace("\"", "") + "");

                        if (IsReachedMaxPostActionPerUser(connection.ProfileUrl, maxNumberOfActionPerPost))
                            continue;

                        jobProcessResult = ScrapeUserPostsAndMoveToFinalProcess(jobProcessResult, queryInfo,
                            isCheckedFilterProfileImageCheckbox, connection.ProfileId, publicIdentifier,
                            lstCommentInDom, maxNumberOfActionPerPost);
                        if (jobProcessResult.IsProcessCompleted)
                            break;
                    }
                }
                else
                {
                    jobProcessResult.HasNoResult = true;
                    GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType);
                }

                #endregion
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
    }
}