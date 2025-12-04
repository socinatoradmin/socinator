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

namespace LinkedDominatorCore.LDLibrary.Processor.Posts
{
    public class CompanyUrlPostsProcessor : BaseLinkedinPostProcessor
    {
        private readonly IDelayService _delayService;
        private readonly IProcessScopeModel _processScopeModel;

        public CompanyUrlPostsProcessor(ILdJobProcess jobProcess,
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
                var IsCheckSkipBlackListedUser = false;
                var isChkPrivateBlackList = false;
                var isChkGroupBlackList = false;

                List<string> lstCommentInDom = null;

                switch (ActivityType)
                {
                    case ActivityType.Like:
                    {
                        var likeModel = _processScopeModel.GetActivitySettingsAs<LikeModel>();
                        isChkPrivateBlackList = likeModel.IsChkPrivateBlackList;
                        IsCheckSkipBlackListedUser = likeModel.IsChkSkipBlackListedUser;
                        isChkGroupBlackList = likeModel.IsChkGroupBlackList;
                        break;
                    }

                    case ActivityType.Comment:
                    {
                        var commentModel = _processScopeModel.GetActivitySettingsAs<CommentModel>();
                        IsCheckSkipBlackListedUser = commentModel.IsChkSkipBlackListedUser;
                        isChkPrivateBlackList = commentModel.IsChkPrivateBlackList;
                        isChkGroupBlackList = commentModel.IsChkGroupBlackList;
                        lstCommentInDom = new List<string>();
                        commentModel.LstDisplayManageCommentModel.ForEach(x =>
                        {
                            if (!lstCommentInDom.Contains(x.CommentText))
                                lstCommentInDom.Add(x.CommentText);
                        });
                        break;
                    }

                    case ActivityType.Share:
                    {
                        var shareModel = _processScopeModel.GetActivitySettingsAs<ShareModel>();
                        IsCheckSkipBlackListedUser = shareModel.IsChkSkipBlackListedUser;
                        isChkPrivateBlackList = shareModel.IsChkPrivateBlackList;
                        isChkGroupBlackList = shareModel.IsChkGroupBlackList;
                        break;
                    }
                }

                #region Social Activities(Engage) On SomeonesPosts

                var companyUrl = queryInfo.QueryValue;
                if (companyUrl.Contains("http:") && companyUrl.Contains("https:") &&
                    (jobProcessResult.HasNoResult = true))
                    return;

                if (IsCheckSkipBlackListedUser && (isChkPrivateBlackList || isChkGroupBlackList) &&
                    manageBlacklistWhitelist.FilterBlackListedUser(companyUrl, isChkPrivateBlackList,
                        isChkGroupBlackList))
                {
                    jobProcessResult.HasNoResult = true;
                    return;
                }

                if (companyUrl.Contains("company") || companyUrl.Contains("showcase"))
                {
                    var getUserInfo = new GetDetailedUserInfo(_delayService);
                    var companyDetails = getUserInfo.ScrapeCompanyDetails(companyUrl, LdFunctions);
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "getting posts created by " + companyDetails.CompanyName + "");

                    jobProcessResult = ScrapeCompanyPostsAndMoveToFinalProcess(jobProcessResult, queryInfo,
                        companyDetails, lstCommentInDom);
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "Please select proper company url");
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