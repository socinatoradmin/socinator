using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
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

namespace LinkedDominatorCore.LDLibrary.Processor.Posts
{
    public class HashtagUrlPostProcessor : BaseLinkedinPostProcessor
    {
        private readonly IDelayService _delayService;
        private readonly IProcessScopeModel _processScopeModel;

        public HashtagUrlPostProcessor(ILdJobProcess jobProcess,
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

                var hashtagUrl = queryInfo.QueryValue;
                if (hashtagUrl.Contains("http:") && hashtagUrl.Contains("https:") &&
                    (jobProcessResult.HasNoResult = true))
                    return;
                if(!hashtagUrl.Contains("www.linkedin.com/feed/hashtag"))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"Please Provide the Correct HashtagUrl {hashtagUrl}");
                    return;
                }

                if (IsCheckSkipBlackListedUser && (isChkPrivateBlackList || isChkGroupBlackList) &&
                    manageBlacklistWhitelist.FilterBlackListedUser(hashtagUrl, isChkPrivateBlackList,
                        isChkGroupBlackList))
                {
                    jobProcessResult.HasNoResult = true;
                    return;
                }
                var hashtagName = Utilities.GetBetween(hashtagUrl, "hashtag/", "/");
                hashtagName=string.IsNullOrEmpty(hashtagName)?hashtagUrl?.Split('/')?.FirstOrDefault(x => x.Contains("keywords"))?.Replace("?keywords=", ""):hashtagName;
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"getting posts created by hashtag {hashtagName}");
                jobProcessResult = ScrapeUserPostsAndMoveToFinalProcess(jobProcessResult, queryInfo,
                    isCheckedFilterProfileImageCheckbox, hashtagName, hashtagUrl, lstCommentInDom,
                    maxNumberOfActionPerUser);

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