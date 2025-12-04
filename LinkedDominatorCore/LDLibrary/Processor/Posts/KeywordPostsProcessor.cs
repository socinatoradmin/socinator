using System;
using System.Collections.Generic;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using ThreadUtils;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel.Engage;
using DominatorHouseCore.Utility;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Campaign;
using DominatorHouseCore.Interfaces;
using System.Linq;
using CommonServiceLocator;

namespace LinkedDominatorCore.LDLibrary.Processor.Posts
{
    public class KeywordPostsProcessor : BaseLinkedinPostProcessor
    {
        private readonly IDelayService _delayService;
        private readonly IProcessScopeModel _processScopeModel;
        public KeywordPostsProcessor(ILdJobProcess jobProcess,
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
                var start =  0;
                var keyword = queryInfo.QueryValue;
                var isCheckedFilterProfileImageCheckbox = false;
                var IsCheckSkipBlackListedUser = false;
                var isChkPrivateBlackList = false;
                var isChkGroupBlackList = false;
                var maxNumberOfActionPerUser = 0;
                var constructedActionUrl = string.Empty;
                var isNotFirst = false;
                List<string> lstCommentInDom = null;

                switch (ActivityType)
                {

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

                }

                // Social Activities(Comment) On keywords
                constructedActionUrl = ConstractedApiToGetKeywordPosts(keyword, ActivityType);
                if (LdJobProcess.ModuleSetting.IsSavePagination)
                {
                    int.TryParse(GetPaginationId(queryInfo), out start);
                }
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    if (LdJobProcess.ModuleSetting.IsSavePagination)
                        AddOrUpdatePaginationId(queryInfo, start.ToString(), ref isNotFirst);

                    var actionUrl = constructedActionUrl.Contains("start=0") ? constructedActionUrl : $"{constructedActionUrl}&count=40&start={start}";
                    // search for posts
                    var searchlinkedinkeywordposts = LdFunctions.SearchPostByKeywordResponseHandler(actionUrl, ActivityType, "", "", lstCommentInDom);

                    if (searchlinkedinkeywordposts.Success)
                    {
                        RemoveAlreadyPerformedPosts(searchlinkedinkeywordposts);
                        if (searchlinkedinkeywordposts.PostsList.Count > 0)
                        {
                            ProcessLinkedinPostsFromPost(queryInfo, ref jobProcessResult,
                            searchlinkedinkeywordposts.PostsList);
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "no more results found");
                            jobProcessResult.HasNoResult = true;
                        }
                        start +=  40;
                        _delayService.ThreadSleep(5000);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "no more results found");
                        jobProcessResult.HasNoResult = true;
                    }
                }

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
