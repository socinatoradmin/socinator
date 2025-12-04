using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.DetailedInfo;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.Engage;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;

namespace LinkedDominatorCore.LDLibrary.EngageProcesses
{
    public class
        CommentProcess : LDJobProcessInteracted<InteractedPosts>
    {
        private readonly ILdFunctions _ldFunctions;
        private readonly IDbInsertionHelper _dbInsertionHelper;
        private LdDataHelper _ldDataHelper= LdDataHelper.GetInstance;

        public CommentProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped accountServiceScoped,
            IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper,
            ILdLogInProcess logInProcess, ILdFunctionFactory ldFunctionFactory,
            IDbInsertionHelper dbInsertionHelper)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            CommentModel = processScopeModel.GetActivitySettingsAs<CommentModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _dbInsertionHelper = dbInsertionHelper;
            blackListWhitelistHandler =
                new BlackListWhiteListHandler(ModuleSetting, DominatorAccountModel, ActivityType);
        }

        private BlackListWhiteListHandler blackListWhitelistHandler { get; }
        private CommentModel CommentModel { get; }
        private Queue<string> QueueComment { get; } = new Queue<string>();

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = null;
            #region Comment Process.
            try
            {
                jobProcessResult = new JobProcessResult();
                var objLinkedinPost = (LinkedinPost)scrapeResult.ResultPost;
                if (CommentModel.IsChkSkipBlackListedUser && CommentModel.IsChkPrivateBlackList || CommentModel.IsChkGroupBlackList)
                {
                    var blackListUser = blackListWhitelistHandler.GetBlackListUsers();
                    foreach (var Items in blackListUser)
                        if (objLinkedinPost.ProfileId.Equals(Items))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "Skip User " + objLinkedinPost.ProfileId + ", Present in Blacklist ");
                            return jobProcessResult;
                        }
                }
                var userScraperDetailedInfo = DetailsFetcher.GetUserScraperDetailedInfo(DominatorAccountModel);

                #region Filters After Visiting Profile

                try
                {
                    if (LdUserFilterProcess.IsUserFilterActive(CommentModel.LDUserFilterModel))
                    {
                        var isValidUser = LdUserFilterProcess.GetFilterStatus(objLinkedinPost.ProfileUrl,
                            CommentModel.LDUserFilterModel, _ldFunctions);
                        if (!isValidUser)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                string.Format("LangKeyNotAValidUserAccordingToTheFilter".FromResourceDictionary(),
                                    objLinkedinPost.FullName));

                            jobProcessResult.IsProcessSuceessfull = false;
                            return jobProcessResult;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion


                var comment = GetCommentText(scrapeResult.QueryInfo);

                if (string.IsNullOrEmpty(comment))
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "Not found any comment for this post [ " + objLinkedinPost.PostLink + " ]", "");
                    jobProcessResult.IsProcessSuceessfull = false;
                    return jobProcessResult;
                }

                if (!string.IsNullOrEmpty(comment) && CommentModel.IsChkSpintaxChecked)
                {
                    var lstMessages = new List<string>();
                    try
                    {
                        lstMessages = SpinTexHelper.GetSpinMessageCollection(comment);
                        lstMessages.Shuffle();
                    }
                    catch (Exception ex)
                    {

                        ex.DebugLog();
                    }
                    try
                    {
                        comment =
                            lstMessages[RandomUtilties.GetRandomNumber(lstMessages.Count-1,0)];
                    }
                    catch (Exception ex)
                    {

                        ex.DebugLog();
                    }
                }

                objLinkedinPost.MyComment = Utils.InsertSpecialCharactersInCsv(comment);
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,DominatorAccountModel.AccountBaseModel.UserName,ActivityType,$"Trying to comment on {{{objLinkedinPost.PostLink}}}");
                var response = IsBrowser
                    ? _ldFunctions.Comment(objLinkedinPost.MyComment, objLinkedinPost.NodeId, objLinkedinPost.PostLink,
                        scrapeResult.QueryInfo.QueryType)
                    : _ldFunctions.CommentWithAlternateMethod(DetailsFetcher, objLinkedinPost, userScraperDetailedInfo, comment, scrapeResult.QueryInfo.QueryType);
                if (response.Contains("groups"))
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"Sorry,Unable to comment on this post Please join {response} this group to comment on this post.");
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                else if (!string.IsNullOrEmpty(response))
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "[ " + objLinkedinPost.PostLink + " ]");
                    IncrementCounters();
                    _dbInsertionHelper.DatabaseInsertionPost(scrapeResult, objLinkedinPost);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        "[ " + objLinkedinPost.PostLink + " ]", "");
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                if(CommentModel!=null && CommentModel.IsEnableAdvancedUserMode && CommentModel.EnableDelayBetweenPerformingActionOnSamePost)
                    DelayBeforeNextActivity(CommentModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
                else
                    DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            #endregion
            return jobProcessResult;
        }
        private string GetCommentText(QueryInfo queryInfo)
        {
            if (QueueComment.Count == 0)
            {
                var commentText = CommentModel.LstDisplayManageCommentModel.FirstOrDefault(x =>
                        x.LstQueries.FirstOrDefault(y =>
                            y.Content.QueryType == queryInfo.QueryType &&
                            y.Content.QueryValue == queryInfo.QueryValue) != null)
                    ?.CommentText;

                if (string.IsNullOrEmpty(commentText))
                    commentText = CommentModel.LstDisplayManageCommentModel.FirstOrDefault()?.CommentText;
                if (CommentModel.IsChkMultilineComment)
                {
                    QueueComment.Enqueue(commentText);
                }
                else
                {
                    var lstComment = Regex.Split(commentText, "\r\n").ToList();
                    lstComment.ForEach(x => { QueueComment.Enqueue(x); });
                }

            }
            if (QueueComment.Count <= 0)
                return "";
            var comment = QueueComment.Dequeue();
            QueueComment.Enqueue(comment);

            return comment;
        }
    }
}