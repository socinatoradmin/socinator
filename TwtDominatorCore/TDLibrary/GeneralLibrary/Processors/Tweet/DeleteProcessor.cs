using System;
using ThreadUtils;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.Tweet
{
    internal class DeleteProcessor : BaseTwitterTweetsProcessor, IQueryProcessor
    {
        private readonly DeleteSetting DeleteSetting;
        private RangeUtilities DeleteCommentDateRange { get; set; }= new RangeUtilities(0, DateTime.UtcNow.GetCurrentEpochTime());
        private RangeUtilities DeleteTweetDateRange { get; set; }= new RangeUtilities(0, DateTime.UtcNow.GetCurrentEpochTime());
        private RangeUtilities UndoRetweetDateRange { get; set; } = new RangeUtilities(0, DateTime.UtcNow.GetCurrentEpochTime());

        public DeleteProcessor(ITdJobProcess jobProcess, IBlackWhiteListHandler blackWhiteListHandler,
            IDbCampaignService campaignService, ITwitterFunctionFactory twitterFunctionFactory,
            IDelayService threadUtility, IDbInsertionHelper dbInsertionHelper)
            : base(jobProcess, blackWhiteListHandler, campaignService, twitterFunctionFactory.TwitterFunctions,
                threadUtility, dbInsertionHelper)
        {
            DeleteSetting = _jobProcess.ModuleSetting.DeleteSetting;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            if (_jobProcess.checkJobCompleted())
                return;
            CheckIsFeedsUpdated(DeleteSetting.IsChkDeleteComment);
            jobProcessResult = new JobProcessResult();
            InitializeDeleteSetttingsVariable();
            if (DeleteSetting.IsChkDeleteTweet)
            {
                queryInfo = new QueryInfo {QueryType = "LangKeyTweet".FromResourceDictionary()};
                StartProcessDeleteTweet(queryInfo, out jobProcessResult);
            }

            if (DeleteSetting.IsChkDeleteComment && !jobProcessResult.IsProcessCompleted)
            {
                queryInfo = new QueryInfo {QueryType = "LangKeyComment".FromResourceDictionary()};
                StartProcessDeleteComment(queryInfo, out jobProcessResult);
            }

            if (DeleteSetting.IsChkUndoRetweet && !jobProcessResult.IsProcessCompleted)
            {
                queryInfo = new QueryInfo {QueryType = "LangKeyRetweet".FromResourceDictionary()};
                StartProcessUndoRetweet(queryInfo, out jobProcessResult);
            }
        }

        private void StartProcessDeleteTweet(QueryInfo queryInfo, out JobProcessResult jobProcessResult)
        {
            jobProcessResult = new JobProcessResult();
            var tweetToBeDeletedList =_dbAccountService.GetFeedInfos(DeleteTweetDateRange.StartValue,DeleteTweetDateRange.EndValue);
            if (tweetToBeDeletedList.Count == 0)
            {
                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName,
                    "LangKeyDeleteTweets".FromResourceDictionary());
                return;
            }


            if (_jobProcess.ModuleSetting.DeleteSetting.IsDeleteRandomTweets ||
                _jobProcess.ModuleSetting.DeleteSetting.IsChkTweetedDateMustBeInSpecificRange)
                foreach (var singleTweet in tweetToBeDeletedList)
                {
                    _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var tweet = new TagDetails
                    {
                        Id = singleTweet.TweetId,
                        Caption = singleTweet.TwtMessage,
                        TweetedTimeStamp = singleTweet.TweetedTimeStamp,
                        LikeCount = singleTweet.LikeCount,
                        RetweetCount = singleTweet.RetweetCount,
                        CommentCount = singleTweet.CommentCount,
                        IsRetweet = singleTweet.IsRetweet == 1 ? true : false
                    };
                    FinalProcessForEachTag(queryInfo, out jobProcessResult, tweet);
                    if (jobProcessResult.IsProcessCompleted)
                        break;
                }
        }

        private void StartProcessDeleteComment(QueryInfo queryInfo, out JobProcessResult jobProcessResult)
        {
            jobProcessResult = new JobProcessResult();
            var commentToBeDeletedList = _dbAccountService.GetFeedInfos(DeleteCommentDateRange.StartValue,
                DeleteCommentDateRange.EndValue, 1);

            if (commentToBeDeletedList.Count == 0)
            {
                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName,
                    ActivityType);
                return;
            }

            foreach (var singleTweet in commentToBeDeletedList)
            {
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var tweet = new TagDetails
                {
                    Id = singleTweet.TweetId,
                    Caption = singleTweet.TwtMessage,
                    IsComment = true,
                    TweetedTimeStamp = singleTweet.TweetedTimeStamp,
                    LikeCount = singleTweet.LikeCount,
                    RetweetCount = singleTweet.RetweetCount,
                    CommentCount = singleTweet.CommentCount
                };
                FinalProcessForEachTag(queryInfo, out jobProcessResult, tweet);
            }
        }

        private void StartProcessUndoRetweet(QueryInfo queryInfo, out JobProcessResult jobProcessResult)
        {
            jobProcessResult = new JobProcessResult();
            var tweetToBeDeletedList = _dbAccountService.GetFeedInfosRetweet(UndoRetweetDateRange.StartValue,
                UndoRetweetDateRange.EndValue, 1);
            if (tweetToBeDeletedList.Count == 0)
            {
                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.UserName,
                    "LangKeyUndoRetweets".FromResourceDictionary());
                return;
            }

            foreach (var singleTweet in tweetToBeDeletedList)
            {
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var tweet = new TagDetails
                {
                    Id = singleTweet.TweetId,
                    Caption = singleTweet.TwtMessage,
                    IsRetweet = true,
                    TweetedTimeStamp = singleTweet.TweetedTimeStamp,
                    LikeCount = singleTweet.LikeCount,
                    RetweetCount = singleTweet.RetweetCount,
                    OriginalTweetId=singleTweet.SourceTweetId,
                    IsRetweetedOwnTweet=singleTweet.IsRetweetedOwnTweet
                };
                FinalProcessForEachTag(queryInfo, out jobProcessResult, tweet);
            }
        }

        private void InitializeDeleteSetttingsVariable()
        {
            if (DeleteSetting.IsChkDeleteTweet && DeleteSetting.IsChkTweetedDateMustBeInSpecificRange)
            {
                if(DeleteSetting.StartDateForTweet != null &&
                    DeleteSetting.EndDateForTweet != null)
                {
                    var startdate = (DateTime)DeleteSetting.StartDateForTweet;
                    var endDate = (DateTime)DeleteSetting.EndDateForTweet;
                    var startDate = new DateTime(startdate.Year, startdate.Month, startdate.Day, startdate.Hour, startdate.Minute, startdate.Second);
                    var enddate = new DateTime(endDate.Year, endDate.Month, endDate.Day, endDate.Hour, endDate.Minute, endDate.Second);
                    DeleteTweetDateRange = new RangeUtilities(startDate.ConvertToEpoch(),
                        enddate.ConvertToEpoch());
                }
                else
                {
                    DeleteTweetDateRange = new RangeUtilities(0, DateTime.UtcNow.ConvertToEpoch());
                }
            }
            if (DeleteSetting.IsChkDeleteComment && DeleteSetting.IsChkCommentedDateMustBeInSpecificRange)
            {
                if(DeleteSetting.StartDateForComment != null &&
                    DeleteSetting.EndDateForComment != null)
                {
                    var startdate = (DateTime)DeleteSetting.StartDateForComment;
                    var endDate = (DateTime)DeleteSetting.EndDateForComment;
                    var startDate = new DateTime(startdate.Year, startdate.Month, startdate.Day, startdate.Hour, startdate.Minute, startdate.Second);
                    var enddate = new DateTime(endDate.Year, endDate.Month, endDate.Day, endDate.Hour, endDate.Minute, endDate.Second);
                    DeleteCommentDateRange = new RangeUtilities(startDate.ConvertToEpoch(),
                        enddate.ConvertToEpoch());
                }
                else
                {
                    DeleteCommentDateRange = new RangeUtilities(0, DateTime.UtcNow.ConvertToEpoch());
                }
            }
            if (DeleteSetting.IsChkRetweetedDateMustBeInSpecificRange)
            {
                if(DeleteSetting.StartDateForRetweet != null &&
                    DeleteSetting.EndDateForRetweet != null)
                {
                    var startdate = (DateTime)DeleteSetting.StartDateForRetweet;
                    var endDate = (DateTime)DeleteSetting.EndDateForRetweet;
                    var startDate = new DateTime(startdate.Year, startdate.Month, startdate.Day, startdate.Hour, startdate.Minute, startdate.Second);
                    var enddate = new DateTime(endDate.Year, endDate.Month, endDate.Day, endDate.Hour, endDate.Minute, endDate.Second);
                    UndoRetweetDateRange = new RangeUtilities(startDate.ConvertToEpoch(),
                        enddate.ConvertToEpoch());
                }
                else
                {
                    UndoRetweetDateRange = new RangeUtilities(0, DateTime.UtcNow.ConvertToEpoch());
                }   
            }
        }
    }
}