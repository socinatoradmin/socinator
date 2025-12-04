using System;
using System.Collections.Generic;
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
    public class GroupUrlPostsProcessor : BaseLinkedinPostProcessor
    {
        private readonly IDelayService _delayService;
        private readonly IProcessScopeModel _processScopeModel;

        public GroupUrlPostsProcessor(ILdJobProcess jobProcess,
            IDbCampaignService campaignService, ILdFunctionFactory ldFunctionFactory,
            IProcessScopeModel processScopeModel, IDelayService delayService) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
            _processScopeModel = processScopeModel;
            _delayService = delayService;
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
                var maxNumberofActionPerGroup = 0;

                List<string> lstCommentInDom = null;

                switch (ActivityType)
                {
                    case ActivityType.Like:
                    {
                        var likeModel = _processScopeModel.GetActivitySettingsAs<LikeModel>();
                        isCheckedFilterProfileImageCheckbox =
                            likeModel.LDUserFilterModel.IsCheckedFilterProfileImageCheckbox;
                        isChkPrivateBlackList = likeModel.IsChkPrivateBlackList;
                        IsCheckSkipBlackListedUser = likeModel.IsChkSkipBlackListedUser;
                        isChkGroupBlackList = likeModel.IsChkGroupBlackList;
                        maxNumberOfActionPerUser =
                            likeModel.IsNumberOfPostToLike ? likeModel.MaxNumberOfPostPerUserToLike : 0;
                        maxNumberofActionPerGroup = likeModel.IsNumberOfGroupPostToLike
                            ? likeModel.MaxNumberOfPostPerGroupToLike
                            : 0;
                        break;
                    }

                    case ActivityType.Comment:
                    {
                        var commentModel = _processScopeModel.GetActivitySettingsAs<CommentModel>();
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
                        maxNumberofActionPerGroup = commentModel.IsNumberOfGroupPostToComment
                            ? commentModel.MaxNumberOfPostPerGroupToComment
                            : 0;
                        break;
                    }

                    case ActivityType.Share:
                    {
                        var shareModel = _processScopeModel.GetActivitySettingsAs<ShareModel>();
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

                var groupUrl = queryInfo.QueryValue;
                var groupId = Utils.GetBetween(groupUrl + "**", "groups/", "**").Replace("/", "");
                var actionUrl = IsBrowser
                    ? groupUrl
                    : $"https://www.linkedin.com/voyager/api/groups/groups/urn%3Ali%3Agroup%3A{groupId}";
                LdFunctions.CheckGroupMemberShip(actionUrl);
                LdFunctions.GetInnerHttpHelper().GetRequestParameter().Accept = null;
                LdFunctions.GetInnerHttpHelper().GetRequestParameter().Referer = "https://www.linkedin.in/";
                if (groupUrl.Contains("http:") && groupUrl.Contains("https:") && (jobProcessResult.HasNoResult = true))
                    return;

                if (IsCheckSkipBlackListedUser && (isChkPrivateBlackList || isChkGroupBlackList) &&
                    manageBlacklistWhitelist.FilterBlackListedUser(groupUrl, isChkPrivateBlackList, isChkGroupBlackList)
                )
                {
                    jobProcessResult.HasNoResult = true;
                    return;
                }
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    "getting posts created in " + groupUrl + "");

                jobProcessResult = ScrapeUserPostsAndMoveToFinalProcess(jobProcessResult, queryInfo,
                    isCheckedFilterProfileImageCheckbox, groupId, "", lstCommentInDom, maxNumberOfActionPerUser,
                    maxNumberofActionPerGroup);

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