using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.Factories;
using TwtDominatorCore.Requests;
using TwtDominatorCore.Response;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDLibrary.GeneralLibrary.Subprocesses;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary
{
    public class CommentProcess : TdJobProcessInteracted<InteractedPosts>
    {
        #region Constructor

        public CommentProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager,
            IJobActivityConfigurationManager jobActivityConfigurationManager,
            ITdQueryScraperFactory queryScraperFactory, ITdHttpHelper tdHttpHelper, ITwtLogInProcess twtLogInProcess,
            IDbInsertionHelper dbInsertionHelper, IDbCampaignService campaignService,
            ITwitterFunctionFactory twitterFunctionFactory,
            IEnumerable<ISubprocess<CommentModel>> subprocess, IDelayService threadUtility)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                tdHttpHelper, twtLogInProcess)
        {
            _jobActivityConfigurationManager = jobActivityConfigurationManager;
            _dbInsertionHelper = dbInsertionHelper;
            _campaignService = campaignService;
            _subprocess = subprocess;
            _twitterFunctionsFactory = twitterFunctionFactory;
            CommentModel = processScopeModel.GetActivitySettingsAs<CommentModel>();
            _templateId = processScopeModel.TemplateId;

            _delayService = threadUtility;

            InitializePerDayAndPercentageCount();
        }

        #endregion

        private ITwitterFunctions _twitterFunctions => _twitterFunctionsFactory.TwitterFunctions;

        #region Public Properties

        public CommentModel CommentModel { get; set; }

        #endregion

        #region Private fields

        private readonly IDbInsertionHelper _dbInsertionHelper;
        private readonly IDbCampaignService _campaignService;
        private readonly IDelayService _delayService;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly IEnumerable<ISubprocess<CommentModel>> _subprocess;

        private readonly ITwitterFunctionFactory _twitterFunctionsFactory;

        //TODO : Make non static
        private static readonly List<KeyValuePair<string, string>> uniqueCommentForPostEachAccount =
            new List<KeyValuePair<string, string>>();

        private static readonly object LockUniqueFromEachAccount = new object();
        private readonly string _templateId;
        private bool _isCampaignMode = true;

        #endregion

        #region Private Properties

        private int PerDayCountToLike { get; set; }
        private int PerDayCountToRetweet { get; set; }
        private int PerDayCountToFollow { get; set; }
        private List<string> ListAllPossibleCommentsOnPost { get; } = new List<string>();

        #endregion

        #region Public Methods

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                CheckCampaignMode(DominatorAccountModel);

                var tagForProcess = (TagDetails) scrapeResult.ResultUser;
                var listAlreadyCommentedOnPost = new List<string>();

                if (!CommentModel.IsAllowMultipleCommentOnSamePost &&
                    DbAccountService.IsAlreadyCommentedOnTweetwithSameQuery(
                        scrapeResult.QueryInfo.QueryType, scrapeResult.QueryInfo.QueryValue,
                        tagForProcess.Id, ActivityType))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.UserName, ActivityType,
                        string.Format("LangKeyFilteredTweetWithId".FromResourceDictionary(), tagForProcess.Id));
                    return jobProcessResult;
                }


                if (_campaignService != null && CommentModel.IsAllowMultipleCommentOnSamePost)
                    listAlreadyCommentedOnPost = _campaignService
                        .GetInteractedPosts(DominatorAccountModel.AccountBaseModel.UserName, ActivityType)
                        .Where(x => x.TweetId == tagForProcess.Id).Select(x => x.CommentedText).ToList();
                else if (CommentModel.IsAllowMultipleCommentOnSamePost)
                    listAlreadyCommentedOnPost = DbAccountService.GetInteractedPosts(ActivityType)
                        .Where(x => x.TweetId == tagForProcess.Id).Select(x => x.CommentedText).ToList();


                var listPossibleCommentsOnPost = new List<string>();
                var isAlreadyGetAllPossibleCommentsForCurrentPost = false;

                var queryInfo = scrapeResult.QueryInfo;
                var listCommentText = new List<string>();
                var allCommentText = "";

                #region text message

                listCommentText = GetCommentAsPerQuery(queryInfo);

                if (listCommentText.Count == 0)
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.UserName, ActivityType.Comment,
                        tagForProcess.Id + "LangKeyNoSpecificCommentFoundMessage".FromResourceDictionary());
                    jobProcessResult.IsProcessSuceessfull = false;
                    return jobProcessResult;
                }

                listCommentText.Shuffle();

                #endregion

                // one time it must have to run 
                var random = new Random();
                int count = 0;
                do
                {
                    
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    // Unique only for campaign mode
                    if (CommentModel.IsUniqueComment && _isCampaignMode)
                    {
                        #region  Is Unique

                        if (CommentModel.IsChkPostUniqueCommentOnPostFromEachAccount)
                        {
                            _delayService.ThreadSleep(random.Next(1000, 5000));

                            lock (LockUniqueFromEachAccount)
                            {
                                allCommentText = GetUniqueCommentPostForEachAccount(tagForProcess, listCommentText);
                            }

                            if (string.IsNullOrEmpty(allCommentText))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.UserName, ActivityType,
                                    $"{"LangKeyAddMoreCommentsForConfigMessage".FromResourceDictionary()}{"LangKeyPostUniqueCommentOnSpecificPost".FromResourceDictionary()}");
                                return jobProcessResult;
                            }
                        }
                        else if (CommentModel.IsChkCommentOnceFromEachAccount)
                        {
                            allCommentText = GetCommentOnceFromEachAccount(listCommentText);

                            if (string.IsNullOrEmpty(allCommentText))
                            {
                                var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                                dominatorScheduler.ChangeAccountsRunningStatus(false, AccountId, ActivityType);
                                GlobusLogHelper.log.Info(Log.CustomMessage,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.UserName, ActivityType,
                                    $"{"LangKeyAddMoreCommentsForConfigMessage".FromResourceDictionary()}{"LangKeyUseSpecificCommentMessage".FromResourceDictionary()}");
                                return jobProcessResult;
                            }
                        }

                        #endregion
                    }
                    else if (CommentModel.IsAllowMultipleCommentOnSamePost)
                    {
                        try
                        {
                            GetAllPossibleCommentsOnPost(listCommentText);

                            if (!isAlreadyGetAllPossibleCommentsForCurrentPost)
                                listPossibleCommentsOnPost = GetAllPossibleCommentsOnPost(
                                    out isAlreadyGetAllPossibleCommentsForCurrentPost, listAlreadyCommentedOnPost);


                            if (listPossibleCommentsOnPost.Count == 0)
                                return jobProcessResult;


                            allCommentText = listPossibleCommentsOnPost[0];
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                    else
                    {
                        allCommentText = listCommentText.FirstOrDefault()?.Trim();
                    }

                    if (CommentingOnPost(scrapeResult, allCommentText, jobProcessResult)) return jobProcessResult;

                    // remove already commented commented from possible comments and 
                    if (jobProcessResult.IsProcessSuceessfull && CommentModel.IsAllowMultipleCommentOnSamePost)
                    {
                        listPossibleCommentsOnPost.Remove(allCommentText);
                        listAlreadyCommentedOnPost.Add(allCommentText);
                        count++;
                    }

                    //  CommentResponse = TwtFunc.Comment(TagForProcess.Id, TagForProcess.Username, commentText);
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    if (CommentModel != null && CommentModel.IsEnableAdvancedUserMode && CommentModel.EnableDelayBetweenPerformingActionOnSamePost)
                        DelayBeforeNextActivity(CommentModel.DelayBetweenPerformingActionOnSamePost.GetRandom());
                    else
                        DelayBeforeNextActivity();
                    if(count > 1)
                    {
                        var seconds = JobConfiguration.DelayBetweenActivity.GetRandom();
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    $"Next Operation To PostComment Will Perform In {seconds} Seconds in tweet {tagForProcess.Id}");
                        _delayService.DelayAsync(seconds * 1000, JobCancellationTokenSource.Token).Wait();
                    }

                    
                } while (CommentModel.IsAllowMultipleCommentOnSamePost &&
                         listAlreadyCommentedOnPost.Count < CommentModel.NoOfMultipleCommentOnSamePost.GetRandom()
                         && listPossibleCommentsOnPost.Count > 0);

                //recursively calling  while not commented all possible comment or reached comment count post
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        public void CheckCampaignMode(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                _isCampaignMode = _jobActivityConfigurationManager[DominatorAccountModel.AccountId]
                    .Where(x => x.TemplateId == _templateId).Select(y => y.IsTemplateMadeByCampaignMode)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(_templateId)) ex.ErrorLog($"Campaign Id : {_templateId}");
            }
        }

        #endregion

        #region Private Methods

        private void InitializePerDayAndPercentageCount()
        {
            if (CommentModel.IsChkEnableLikeOthersComment)
                PerDayCountToLike = CommentModel.LikeOthersCommentRange.GetRandom();
            if (CommentModel.IsChkEnableRetweetComment)
                PerDayCountToRetweet = CommentModel.RetweetMaxRange.GetRandom();
            if (CommentModel.IsChkIncreaseEachDayFollow)
                PerDayCountToFollow = CommentModel.FollowMaxRange.GetRandom();
        }

        private bool CommentingOnPost(ScrapeResultNew scrapeResult, string allCommentText,
            JobProcessResult jobProcessResult)
        {
            var tagForProcess = (TagDetails) scrapeResult.ResultUser;
            var isReturn = false;
            try
            {
                // check spintax is check or not
                if (CommentModel.IsSpintax && !string.IsNullOrEmpty(allCommentText) &&
                    !CommentModel.IsAllowMultipleCommentOnSamePost)
                    allCommentText = SpinTexHelper.GetSpinText(allCommentText);

                #region  commenting with or without media

                List<string> imagePath = GetMediaPathAsPerQuery(scrapeResult.QueryInfo);

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                CommentResponseHandler commentResponse;

                if (imagePath.Count() == 0)
                    commentResponse = _twitterFunctions.Comment(DominatorAccountModel, tagForProcess.Id,
                        tagForProcess.Username,
                        allCommentText, scrapeResult.QueryInfo.QueryType);
                else
                    commentResponse = _twitterFunctions.Comment(DominatorAccountModel, tagForProcess.Id,
                        tagForProcess.Username,
                        allCommentText, scrapeResult.QueryInfo.QueryType, imagePath);

                #endregion
                
                if (commentResponse.Success)
                {
                    IncrementCounters();
                    _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(tagForProcess,
                        ActivityType.Comment.ToString(), ActivityType.Comment.ToString(), scrapeResult, allCommentText);

                    // Updated from normal mode

                    #region  GetModuleSetting

                    var jobActivityConfigurationManager =
                        InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                    var moduleModeSetting =
                        jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                    if (moduleModeSetting == null) return isReturn = true;

                    #endregion

                    if (moduleModeSetting.IsTemplateMadeByCampaignMode)
                        _dbInsertionHelper.AddInteractedTweetDetailInCampaignDb(tagForProcess,
                            ActivityType.Comment.ToString(), scrapeResult, allCommentText);

                    _dbInsertionHelper.AddFeedsInfo(
                        new TagDetails
                        {
                            Id = commentResponse.TweetId,
                            Caption = allCommentText,
                            TweetedTimeStamp = DateTime.UtcNow.GetCurrentEpochTime(),
                            IsComment = true
                        }, true);

                    jobProcessResult.IsProcessSuceessfull = true;
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                        ActivityType.Comment, TdUtility.GetTweetUrl(tagForProcess.Username, tagForProcess.Id) + $" with comment {TdUtility.GetTweetUrl(string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.UserName) ? tagForProcess.Username: DominatorAccountModel.AccountBaseModel.UserName, commentResponse.TweetId)}");

                    PostCommentProcess(tagForProcess);
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.UserName, ActivityType.Comment, TdUtility.GetTweetUrl(tagForProcess.Username, tagForProcess.Id)+" ==> "+
                        commentResponse.Issue?.Message);

                    jobProcessResult.IsProcessSuceessfull = false;

                    #region removing comment for that post

                    try
                    {
                        if(CommentModel.IsUniqueComment && CommentModel.IsChkPostUniqueCommentOnPostFromEachAccount)
                        {
                            var campaignIdWithPost = _templateId + tagForProcess.Id;
                            uniqueCommentForPostEachAccount.RemoveAll(x =>
                                x.Key == campaignIdWithPost && x.Value == allCommentText);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    #endregion
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return isReturn;
        }

        private void PostCommentProcess(TagDetails tweet)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, ActivityType,
                    string.Format("LangKeyPostCommentProcessStartedTweet".FromResourceDictionary(),TdUtility.GetTweetUrl(tweet.Username, tweet.Id)));
                
                _subprocess.ForEach(a => { a.Run(JobCancellationTokenSource, tweet, CommentModel); });
            }
            catch (Exception ex)
            {
                if (DominatorAccountModel?.AccountBaseModel?.UserName != null)
                    ex.DebugLog($"{DominatorAccountModel.AccountBaseModel.UserName}");
            }
        }

        private List<string> GetMediaPathAsPerQuery(QueryInfo queryInfo)
        {
            List<string> mediaPath = new List<string>();
            try
            {
                CommentModel.LstDisplayManageCommentModel.ForEach(comment =>
                {
                        comment.SelectedQuery.ForEach(query =>
                        {
                            if (query.Content.QueryValue == queryInfo.QueryValue &&
                                query.Content.QueryType == queryInfo.QueryType)
                                mediaPath = comment.MediaList.ToList();
                        });
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return mediaPath;
        }

        private List<string> GetCommentAsPerQuery(QueryInfo queryInfo)
        {
            var listCommentText = new List<string>();
            try
            {
                CommentModel.LstDisplayManageCommentModel.ForEach(comment =>
                {
                    if (!listCommentText.Contains(comment.CommentText))
                        comment.SelectedQuery.ForEach(query =>
                        {
                            if ((query.Content.QueryValue == queryInfo.QueryValue
                            || query.Content.QueryValue.Contains(queryInfo.QueryValue)) &&
                                query.Content.QueryType == queryInfo.QueryType)
                                listCommentText.Add(comment.CommentText);
                        });
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return listCommentText;
        }

        private string GetUniqueCommentPostForEachAccount(TagDetails tweet, List<string> listCommentText)
        {
            var comment = "";
            var campaignIdWithPost = _templateId + tweet.Id;
            try
            {
                var commentsOnCurrentPost = uniqueCommentForPostEachAccount.Where(x => x.Key == campaignIdWithPost)
                    .Select(y => y.Value).ToList();

                listCommentText.RemoveAll(x => commentsOnCurrentPost.Contains(x));

                comment = listCommentText.FirstOrDefault();

                uniqueCommentForPostEachAccount.Add(new KeyValuePair<string, string>(campaignIdWithPost, comment));

                if (uniqueCommentForPostEachAccount.Count > 1500) uniqueCommentForPostEachAccount.RemoveRange(0, 500);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return comment;
        }

        private string GetCommentOnceFromEachAccount(List<string> listCommentText)
        {
            var comment = "";
            try
            {
                var listMessages = _campaignService.GetAllInteractedPosts()
                    .Where(x => x.SinAccUsername == DominatorAccountModel.AccountBaseModel.UserName)
                    .Select(y => y.CommentedText).ToList();

                listCommentText.RemoveAll(x => listMessages.Contains(x));
                comment = listCommentText.FirstOrDefault();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return comment;
        }

        private List<string> GetAllPossibleCommentsOnPost(out bool isAlreadyGetAllPossibleCommentsForCurrentPost,
            List<string> listAlreadyCommentedText)
        {
            var listPossibleCommentsOnPost = new List<string>();
            isAlreadyGetAllPossibleCommentsForCurrentPost = true;
            try
            {
                listPossibleCommentsOnPost.AddRange(ListAllPossibleCommentsOnPost);
                listPossibleCommentsOnPost.RemoveAll(x => listAlreadyCommentedText.Contains(x));
                listPossibleCommentsOnPost.Shuffle();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return listPossibleCommentsOnPost;
        }

        /// <summary>
        ///     here we  take AllPossibleCommentsOnPost  once for account campaign
        /// </summary>
        /// <param name="listCommentText"></param>
        private void GetAllPossibleCommentsOnPost(List<string> listCommentText)
        {
            // if already get All possible comment on post return
            if (ListAllPossibleCommentsOnPost.Count > 0) return;

            try
            {
                for (var i = 0; i < listCommentText.Count; i++)
                    if (CommentModel.IsSpintax)
                        ListAllPossibleCommentsOnPost.AddRange(
                            SpintaxParser.SpintaxGenerator(listCommentText[i]));
                    else
                        ListAllPossibleCommentsOnPost.Add(listCommentText[i]);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}