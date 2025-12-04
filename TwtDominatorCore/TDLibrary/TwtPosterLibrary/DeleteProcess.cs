using System;
using DominatorHouseCore;
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
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary
{
    public class DeleteProcess : TdJobProcessInteracted<InteractedPosts>
    {
        #region Construction

        public DeleteProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ITwitterFunctionFactory twitterFunctionFactory,
            IJobActivityConfigurationManager jobActivityConfigurationManager,
            ITdQueryScraperFactory queryScraperFactory, ITdHttpHelper tdHttpHelper, ITwtLogInProcess twtLogInProcess,
            IDbInsertionHelper dbInsertionHelper)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                tdHttpHelper, twtLogInProcess)
        {
            _twtFuncFactory = twitterFunctionFactory;
            _jobActivityConfigurationManager = jobActivityConfigurationManager;
            _dbInsertionHelper = dbInsertionHelper;
            DeleteModel = processScopeModel.GetActivitySettingsAs<DeleteModel>();
        }

        #endregion

        #region Public Properties

        public DeleteModel DeleteModel { get; set; }

        #endregion

        // changement in to assign functions factory beacuse of NewUi error to slove this we use this way.
        private ITwitterFunctions _twtFunc => _twtFuncFactory.TwitterFunctions;

        #region Public methods

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                var tweet = (TagDetails) scrapeResult.ResultUser;

                var success = false;

                if (scrapeResult.QueryInfo.QueryType == "Tweet") success = DeleteTweet(tweet.Id,tweet.Username);
                if (scrapeResult.QueryInfo.QueryType == "Comment") success = DeleteComment(tweet.Id, tweet.Username);
                if (scrapeResult.QueryInfo.QueryType == "Retweet") success = UndoRetweet(tweet.IsRetweetedOwnTweet? tweet.Id:tweet.OriginalTweetId, tweet.Username);

                if (success)
                {
                    // Update tweet count in UI by decrementing count after each post delete
                    LogInProcess.UpdateDominatorAccountModel(DominatorAccountModel, 0, -1);

                    IncrementCounters();

                    _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(tweet, ActivityType.ToString(),
                        ActivityType.ToString(), scrapeResult);

                    // Updated from normal mode

                    #region  GetModuleSetting

                    var moduleSetting =
                        _jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                    if (moduleSetting == null) return jobProcessResult;

                    #endregion

                    if (moduleSetting.IsTemplateMadeByCampaignMode)
                        _dbInsertionHelper.AddInteractedTweetDetailInCampaignDb(tweet, ActivityType.ToString(),
                            scrapeResult);

                    _dbInsertionHelper.DeleteTweetFromFeedInfo(tweet.Id);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    jobProcessResult.IsProcessSuceessfull = false;
                }

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                if (DominatorAccountModel?.AccountBaseModel?.UserName != null)
                    ex.DebugLog($"{DominatorAccountModel.AccountBaseModel.UserName} {ActivityType.ToString()}");
            }

            return jobProcessResult;
        }

        #endregion

        #region Private Fields

        private readonly ITwitterFunctionFactory _twtFuncFactory;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly IDbInsertionHelper _dbInsertionHelper;

        #endregion

        #region MyRegion

        private bool DeleteTweet(string id,string Username)
        {
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var responseHandler = _twtFunc.Delete(DominatorAccountModel, id);
                var Tweeturl = TdUtility.GetTweetUrl(Username, id);
                if (responseHandler.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                        "Delete Tweet", Tweeturl);
                    return true;
                }

                GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, "Delete Tweet",
                    Tweeturl + " " + responseHandler.Issue?.Message + " or that page does not exist");


                if (responseHandler.Issue.Message.Contains("that page does not exist") ||
                    responseHandler.Issue.Message.Contains("Empty Response"))
                    _dbInsertionHelper.DeleteTweetFromFeedInfo(id);
                return false;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
        }

        private bool DeleteComment(string id, string Username)
        {
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var responseHandler = _twtFunc.Delete(DominatorAccountModel, id, ActivityType.DeleteComment);
                var Tweeturl = TdUtility.GetTweetUrl(Username, id);
                if (responseHandler.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                        "Delete Comment", Tweeturl);
                    return true;
                }

                GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, "Delete Comment", Tweeturl + " " + responseHandler.Issue?.Message);

                if (responseHandler.Issue.Message.Contains("that page does not exist"))
                    _dbInsertionHelper.DeleteTweetFromFeedInfo(id);

                return false;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
        }

        private bool UndoRetweet(string id, string Username)
        {
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var responseHandler = _twtFunc.UndoRetweet(DominatorAccountModel, id);

                var Tweeturl = TdUtility.GetTweetUrl(Username, id);
                if (responseHandler.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                        "Undo Retweet", Tweeturl);
                    return true;
                }

                GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.UserName, "Undo Retweet", Tweeturl + " " + responseHandler.Issue?.Message);

                if (responseHandler.Issue.Message.Contains("that page does not exist"))
                    _dbInsertionHelper.DeleteTweetFromFeedInfo(id);

                return false;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
        }

        #endregion
    }
}