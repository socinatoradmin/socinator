using System;
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
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Database;
using TwtDominatorCore.Factories;
using TwtDominatorCore.Requests;
using TwtDominatorCore.Response;
using TwtDominatorCore.TDEnums;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;

// ReSharper disable once CheckNamespace
namespace TwtDominatorCore.TDLibrary
{
    public class FollowProcess : TdJobProcessInteracted<InteractedUsers>
    {
        public FollowProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ITdQueryScraperFactory queryScraperFactory,
            ITdHttpHelper tdHttpHelper, ITwtLogInProcess twtLogInProcess,
            ITwitterFunctionFactory twitterFunctionFactory, IDbInsertionHelper dbInsertionHelper,
            IDelayService threadUtility)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                tdHttpHelper, twtLogInProcess)
        {
            FollowerModel = processScopeModel.GetActivitySettingsAs<FollowerModel>();
            _twitterFunctionsFactory = twitterFunctionFactory;
            _dbInsertionHelper = dbInsertionHelper;
            InitializePerDayAndPercentageCount();
            _delayService = threadUtility;
        }

        // assign the classes of Itwitterfunction with respect to factory.
        private ITwitterFunctions _twitterFunctions => _twitterFunctionsFactory.TwitterFunctions;


        /// <summary>
        ///     Initializes per day count and percentages for individual campaigns
        /// </summary>
        private void InitializePerDayAndPercentageCount()
        {
            UserCountToLike = FollowerModel.AfterFollowAction.LikeMaxRange.GetRandom();
            UserCountToRetweet = FollowerModel.AfterFollowAction.RetweetMaxRange.GetRandom();
            UserCountToComment = FollowerModel.AfterFollowAction.CommentsPerDayMaxRange.GetRandom();
            UserCountToTweet = FollowerModel.AfterFollowAction.TweetMaxRange.GetRandom();

            if (FollowerModel.AfterFollowAction.IsChkCommentPercentage)
                CommentCountFromPercentage = Utilities.PercentageCalculator(JobConfiguration.ActivitiesPerJob.EndValue,
                    FollowerModel.AfterFollowAction.CommentPercentage);

            if (FollowerModel.AfterFollowAction.IsChkTweetPercentage)
                TweetCountFromPercentage = Utilities.PercentageCalculator(JobConfiguration.ActivitiesPerJob.EndValue,
                    FollowerModel.AfterFollowAction.TweetPercentage);
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();

            try
            {
                var twitterUser = (TwitterUser) scrapeResult.ResultUser;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                // Updated from normal mode

                #region  GetModuleSetting

                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleSetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                if (moduleSetting == null)
                    return jobProcessResult;

                #endregion

                var followResponse = _twitterFunctions.Follow(DominatorAccountModel, twitterUser.UserId,
                    twitterUser.Username, scrapeResult.QueryInfo.QueryType);

                if (followResponse.Success)
                {
                    // Update following count in UI by incrementing count after each follow
                    //LogInProcess.UpdateDominatorAccountModel(DominatorAccountModel, 1);

                    IncrementCounters();

                    _dbInsertionHelper.AddInteractedUserDetailsInAccountDb(twitterUser, ActivityType.ToString(),
                        ActivityType.ToString(), ActivityType == ActivityType.Follow ? scrapeResult : null);

                    if (moduleSetting.IsTemplateMadeByCampaignMode)
                        _dbInsertionHelper.AddInteractedUserDetailsInCampaignDb(twitterUser, ActivityType.ToString(),
                            ActivityType == ActivityType.Follow ? scrapeResult : null);

                    _dbInsertionHelper.AddFriendshipData(twitterUser, FollowType.Following, 1);
                    // throw cancellation token before showing message
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                        ActivityType, TdUtility.GetProfileUrl(twitterUser.Username));

                    jobProcessResult.IsProcessSuceessfull = true;

                    PostFollowProcess(twitterUser, scrapeResult);

                    StartOtherConfigurationAfterEachFollow(scrapeResult);
                }
                else
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var message = followResponse.Issue?.Message;
                    message = UpdateAccountStatus(message, twitterUser, followResponse);
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.UserName, ActivityType.Follow, message);


                    GlobusLogHelper.log.Error(message);


                    jobProcessResult.IsProcessSuceessfull = false;

                    #region remove user who successfull not completed their in Unique case

                    //if (moduleSetting.IsTemplateMadeByCampaignMode && FollowerModel.IsCampaignWiseUniqueChecked)
                    //{
                    //    factory.CampaignInteractedUtility.RemoveInteractedData(CampaignId, TwitterUser.UserId);
                    //}

                    #endregion
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
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private string UpdateAccountStatus(string message, TwitterUser twitterUser,
            FollowResponseHandler followResponse)
        {
            if (message.Contains("blocked from following"))
            {
                message += $" {twitterUser.Username}";
                _dbInsertionHelper.UpdateAccountStatus(twitterUser.UserId, twitterUser.Username,
                    TdConstants.AccountBlockedToYou);
            }
            else if (followResponse.Issue?.Message == "Cannot find specified user.")
            {
                message += $" {twitterUser.Username}";
                _dbInsertionHelper.UpdateAccountStatus(twitterUser.UserId, twitterUser.Username,
                    TdConstants.AccountSuspended);
            }
            else if (followResponse.Issue?.Message == "Cannot find specified user.")
            {
                message += $" {twitterUser.Username}";
                _dbInsertionHelper.UpdateAccountStatus(twitterUser.UserId, twitterUser.Username,
                    TdConstants.AccountSuspended);
            }

            return message;
        }

        #region Private Fields

        private readonly ITwitterFunctionFactory _twitterFunctionsFactory;
        private readonly IDbInsertionHelper _dbInsertionHelper;

        private readonly IDelayService _delayService;
        //   private  ITwitterFunctions _twitterFunctions;

        #endregion

        #region Private Properties

        // Per day Like count for individual campaigns
        private int UserCountToLike { get; set; }

        // Per day Retweet count for individual campaigns
        private int UserCountToRetweet { get; set; }

        // Per day Comment count for individual campaigns
        private int UserCountToComment { get; set; }

        // Per day messages count for individual campaigns
        private int UserCountToTweet { get; set; }
        public int CommentCountFromPercentage { get; set; }
        public int TweetCountFromPercentage { get; set; }
        private FollowerModel FollowerModel { get; }

        #endregion

        #region Private Methods

        /// <summary>
        ///     After follow process
        /// </summary>
        /// <param name="twitterUser"></param>
        private void PostFollowProcess(TwitterUser twitterUser, ScrapeResultNew scrapeResult)
        {
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (FollowerModel.AfterFollowAction.IsChkTurnOnUserNotifications) TurnOnUserNotifications(twitterUser);

                if (FollowerModel.AfterFollowAction.IsChkMuteUser) MuteUser(twitterUser);

                if (FollowerModel.AfterFollowAction.IsChkLikeTweets) LikeUsersLatestPosts(twitterUser);

                if (FollowerModel.AfterFollowAction.IsChkRetweetTweets) RetweetUsersLatestPosts(twitterUser);

                if (FollowerModel.AfterFollowAction.IsChkTweetToUser) TweetToUser(twitterUser);

                if (FollowerModel.AfterFollowAction.IsChkCommentOnTweets)
                    CommentOnUsersLatestTweet(twitterUser, scrapeResult);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                if (DominatorAccountModel?.AccountBaseModel?.UserName != null)
                    ex.DebugLog(
                        $"TwtDominator : [Account: {DominatorAccountModel.AccountBaseModel.UserName}] => Error : {ex.Message}. StackTrace => {ex.StackTrace}  (Module => {ActivityType.ToString()})");
            }
        }

        private void TurnOnUserNotifications(TwitterUser user)
        {
            try
            {
                _delayService.ThreadSleep(TdConstants.ConsecutivePostReqInterval);

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var response = _twitterFunctions.TurnOnUserNotifications(DominatorAccountModel, user.UserId, user.Username);

                if (response.Success)
                {
                    // Adding to black list
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                        ActivityType.TurnOnUserNotifications, user.Username);

                    _dbInsertionHelper.AddInteractedUserDetailsInAccountDb(user, ActivityType.TurnOnUserNotifications.ToString(),
                        ActivityType.Equals(ActivityType.Follow)
                            ? Enums.ProcessType.AfterFollow.ToString()
                            : Enums.ProcessType.AfterFollowBack.ToString());
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.UserName, ActivityType.TurnOnUserNotifications, user.Username, response.Issue?.Message);
                }

                var delay = TdConstants.ConsecutivePostReqInterval / 1000;

                //GlobusLogHelper.log.Info(Log.DelayBetweenActivity,
                //    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                //    DominatorAccountModel.AccountBaseModel.UserName, ActivityType.Mute, delay);

                _delayService.ThreadSleep(TimeSpan.FromSeconds(delay));
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                if (DominatorAccountModel?.AccountBaseModel?.UserName != null)
                    ex.DebugLog(
                        $"TwtDominator : [Account: {DominatorAccountModel.AccountBaseModel.UserName}] => Error : {ex.Message}. StackTrace => {ex.StackTrace}  (Module => {ActivityType.ToString()})");
            }
        }

        private void MuteUser(TwitterUser user)
        {
            try
            {
                _delayService.ThreadSleep(TdConstants.ConsecutivePostReqInterval);

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var response = _twitterFunctions.Mute(DominatorAccountModel, user.UserId, user.Username);

                if (response.Success)
                {
                    // Adding to black list
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                        ActivityType.Mute, user.Username);

                    _dbInsertionHelper.AddInteractedUserDetailsInAccountDb(user, ActivityType.Mute.ToString(),
                        ActivityType.Equals(ActivityType.Follow)
                            ? Enums.ProcessType.AfterFollow.ToString()
                            : Enums.ProcessType.AfterFollowBack.ToString());
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.UserName, ActivityType.Mute, user.Username, response.Issue?.Message);
                }

                var delay = TdConstants.ConsecutivePostReqInterval / 1000;

                //GlobusLogHelper.log.Info(Log.DelayBetweenActivity,
                //    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                //    DominatorAccountModel.AccountBaseModel.UserName, ActivityType.Mute, delay);

                _delayService.ThreadSleep(TimeSpan.FromSeconds(delay));
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                if (DominatorAccountModel?.AccountBaseModel?.UserName != null)
                    ex.DebugLog(
                        $"TwtDominator : [Account: {DominatorAccountModel.AccountBaseModel.UserName}] => Error : {ex.Message}. StackTrace => {ex.StackTrace}  (Module => {ActivityType.ToString()})");
            }
        }

        private bool IsPostFollowProcessCompleted(ActivityType activityType)
        {
            var today = DateTime.Today;

            if (activityType == ActivityType.Like)
            {
                var lstInteractedPosts = DbAccountService.GetInteractedPosts(activityType).Where(x =>
                        x.ProcessType == Enums.ProcessType.AfterFollow.ToString() && x.InteractionDate.Date == today)
                    .ToList();

                if (FollowerModel.AfterFollowAction.IsChkMaxLike &&
                    UserCountToLike <= lstInteractedPosts.Count)
                    return true;
            }

            if (activityType == ActivityType.Retweet)
            {
                var lstInteractedPosts = DbAccountService.GetInteractedPosts(activityType).Where(x =>
                    x.ProcessType == Enums.ProcessType.AfterFollow.ToString() && x.InteractionDate == today).ToList();

                if (FollowerModel.AfterFollowAction.IsChkMaxLike &&
                    UserCountToLike <= lstInteractedPosts.Count)
                    return true;
            }
            else if (activityType == ActivityType.Tweet)
            {
                var lstInteractedUser = DbAccountService.GetInteractedUsers(activityType).ToList();

                if (FollowerModel.AfterFollowAction.IsChkMaxTweet &&
                    UserCountToTweet <= lstInteractedUser.Count)
                    return true;

                if (FollowerModel.AfterFollowAction.IsChkTweetPercentage &&
                    TweetCountFromPercentage <= lstInteractedUser.Count)
                    return true;
            }
            else if (activityType == ActivityType.Comment)
            {
                var lstInteractedPosts = DbAccountService.GetInteractedPosts(activityType).Where(x =>
                    x.ProcessType == Enums.ProcessType.AfterFollow.ToString() && x.InteractionDate == today).ToList();

                if (FollowerModel.AfterFollowAction.IsChkMaxComment &&
                    UserCountToComment <= lstInteractedPosts.Count)
                    return true;

                if (FollowerModel.AfterFollowAction.IsChkCommentPercentage &&
                    CommentCountFromPercentage <= lstInteractedPosts.Count)
                    return true;
            }

            return false;
        }

        private void LikeUsersLatestPosts(TwitterUser user)
        {
            try
            {
                var currentLikeCount = 0;
                var likeCountForCurrentUser = FollowerModel.AfterFollowAction.LikeTweetsRange.GetRandom();

                if (IsPostFollowProcessCompleted(ActivityType.Like)) return;

                var hasNoMorePosts = false;
                string maxPosition = null;

                _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);

                while (!hasNoMorePosts)
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var userFeedDetails = _twitterFunctions.GetTweetsFromUserFeedAsync(DominatorAccountModel,
                        user.Username,
                        JobCancellationTokenSource.Token, maxPosition,ActivityType.Tweet,false).Result;

                    if (userFeedDetails.Success)
                    {
                        var lstTweets = userFeedDetails.UserTweetsDetail;
                        lstTweets.RemoveAll(x => x.IsComment);
                        hasNoMorePosts = !userFeedDetails.hasmore;
                        if (FollowerModel.AfterFollowAction.IsChkLikeButSkipRetweets)
                            lstTweets.RemoveAll(x => x.IsRetweet);

                        if (FollowerModel.AfterFollowAction.IsCheckLikeRandomTweets) lstTweets.Shuffle();
                        
                        foreach (var singleTweetDetails in lstTweets)
                        {
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            var likeResponse = _twitterFunctions.Like(DominatorAccountModel, singleTweetDetails.Id,
                                singleTweetDetails.Username, QueryInfo.NoQuery.QueryType);

                            if (likeResponse.Success)
                            {
                                GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.UserName, ActivityType.Like, singleTweetDetails.Id);
                                _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(singleTweetDetails,
                                    ActivityType.Like.ToString(),
                                    ActivityType.Equals(ActivityType.Follow)
                                        ? Enums.ProcessType.AfterFollow.ToString()
                                        : Enums.ProcessType.AfterFollowBack.ToString());
                                currentLikeCount++;
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(Log.ActivityFailed,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.UserName, ActivityType.Like,$"{singleTweetDetails.Id} ==> {likeResponse.Issue?.Message}");
                            }
                            if (IsPostFollowProcessCompleted(ActivityType.Like) ||
                                singleTweetDetails == lstTweets.LastOrDefault() || likeCountForCurrentUser <= currentLikeCount)
                                return;
                            var randomTime = FollowerModel.AfterFollowAction.DelayBetweenLikeRange.GetRandom();
                            GlobusLogHelper.log.Info(Log.DelayBetweenActivity,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType.Like, randomTime);
                            _delayService.ThreadSleep(TimeSpan.FromSeconds(randomTime));
                        }

                        maxPosition = userFeedDetails.MinPosition;

                        if (string.IsNullOrEmpty(maxPosition) ) break;
                    }
                    else
                    {
                        hasNoMorePosts = true;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                if (DominatorAccountModel?.AccountBaseModel?.UserName != null)
                    ex.DebugLog(
                        $"TwtDominator : [Account: {DominatorAccountModel.AccountBaseModel.UserName}] => Error : {ex.Message}. StackTrace => {ex.StackTrace}  (Module => {ActivityType.ToString()})");
            }
        }

        private void RetweetUsersLatestPosts(TwitterUser User)
        {
            try
            {
                var currentRetweetCount = 0;
                var retweetCountForCurrentUser = FollowerModel.AfterFollowAction.RetweetTweetsRange.GetRandom();

                if (IsPostFollowProcessCompleted(ActivityType.Retweet)) return;

                var hasNoMorePosts = false;
                string maxPosition = null;

                _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);

                while (!hasNoMorePosts)
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var userFeedDetails = _twitterFunctions.GetTweetsFromUserFeedAsync(DominatorAccountModel,
                        User.Username,
                        JobCancellationTokenSource.Token, maxPosition,ActivityType.Tweet,false).Result;

                    if (userFeedDetails.Success)
                    {
                        var lstTweets = userFeedDetails.UserTweetsDetail;
                        lstTweets.RemoveAll(x => x.IsComment);

                        if (FollowerModel.AfterFollowAction.IsChkRetweetTweetButSkipRetweets)
                            lstTweets.RemoveAll(x => x.IsRetweet);

                        if (FollowerModel.AfterFollowAction.IsCheckLikeRandomTweets) lstTweets.Shuffle();

                        foreach (var singleTweetDetails in lstTweets)
                        {
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            var retweetResponse = _twitterFunctions.Retweet(DominatorAccountModel,
                                singleTweetDetails.Id,
                                singleTweetDetails.Username);

                            if (retweetResponse.Success)
                            {
                                GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.UserName, ActivityType.Retweet, singleTweetDetails.Id);

                                _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(singleTweetDetails,
                                    ActivityType.Retweet.ToString(),
                                    ActivityType.Equals(ActivityType.Follow)
                                        ? Enums.ProcessType.AfterFollow.ToString()
                                        : Enums.ProcessType.AfterFollowBack.ToString());

                                _dbInsertionHelper.AddFeedsInfo(singleTweetDetails, true);

                                currentRetweetCount++;
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(Log.ActivityFailed,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.UserName, ActivityType.Retweet, singleTweetDetails.Id+" ==> "+
                                    retweetResponse.Issue?.Message);
                            }
                            if (IsPostFollowProcessCompleted(ActivityType.Retweet) || retweetCountForCurrentUser <= currentRetweetCount)
                                return;
                            var randomTime = FollowerModel.AfterFollowAction.DelayBetweenRetweetRange.GetRandom();
                            GlobusLogHelper.log.Info(Log.DelayBetweenActivity,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType.Retweet, randomTime);
                            _delayService.ThreadSleep(TimeSpan.FromSeconds(randomTime));
                        }

                        maxPosition = userFeedDetails.MinPosition;

                        if (string.IsNullOrEmpty(maxPosition)) break;
                    }
                    else
                    {
                        hasNoMorePosts = true;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                if (DominatorAccountModel?.AccountBaseModel?.UserName != null)
                    ex.DebugLog(
                        $"TwtDominator : [Account: {DominatorAccountModel.AccountBaseModel.UserName}] => Error : {ex.Message}. StackTrace => {ex.StackTrace}  (Module => {ActivityType.ToString()})");
            }
        }

        private void TweetToUser(TwitterUser User)
        {
            try
            {
                if (IsPostFollowProcessCompleted(ActivityType.Tweet)) return;

                var tweetBody = "@" + User.Username + " " +
                                FollowerModel.AfterFollowAction.LstTweetToUserInput.GetRandomItem();

                var tweetResponse = _twitterFunctions.Tweet(DominatorAccountModel, tweetBody,
                    JobCancellationTokenSource.Token, "", "", "", ActivityType);

                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (tweetResponse.Success)
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                        ActivityType.Retweet, tweetResponse.TweetId);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        private void CommentOnUsersLatestTweet(TwitterUser User, ScrapeResultNew scrapeResult)
        {
            try
            {
                var currentCommentCount = 0;
                var postCommentCount = FollowerModel.AfterFollowAction.CommentsPerUser.GetRandom();

                if (IsPostFollowProcessCompleted(ActivityType.Comment)) return;

                var hasNoMorePosts = false;
                string maxPosition = null;

                while (!hasNoMorePosts)
                {
                    JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var userFeedDetails = _twitterFunctions.GetTweetsFromUserFeedAsync(DominatorAccountModel,
                        User.Username,
                        JobCancellationTokenSource.Token, maxPosition,ActivityType.Tweet,false).Result;

                    if (userFeedDetails.Success)
                    {
                        var lstTweets = userFeedDetails.UserTweetsDetail;
                        lstTweets.RemoveAll(x => x.IsComment);
                        hasNoMorePosts = !userFeedDetails.hasmore;
                        if (FollowerModel.AfterFollowAction.IsChkCommentButSkipRetweets)
                            lstTweets.RemoveAll(x => x.IsRetweet);

                        foreach (var singleTweetDetail in lstTweets)
                        {
                            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            var commentBody =
                                FollowerModel.AfterFollowAction.LstCommentOnUsersTweetInput.GetRandomItem();

                            //spintax
                            if (FollowerModel.AfterFollowAction.IsSpintax)
                                commentBody = SpinTexHelper.GetSpinText(commentBody);


                            var commentResponse = _twitterFunctions.Comment(DominatorAccountModel, singleTweetDetail.Id,
                                singleTweetDetail.Username, commentBody, scrapeResult.QueryInfo.QueryType);

                            if (commentResponse.Success)
                            {
                                GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.UserName, ActivityType.Comment, singleTweetDetail.Id);

                                _dbInsertionHelper.AddInteractedTweetDetailInAccountDb(singleTweetDetail,
                                    ActivityType.Comment.ToString(),
                                    ActivityType.Equals(ActivityType.Follow)
                                        ? Enums.ProcessType.AfterFollow.ToString()
                                        : Enums.ProcessType.AfterFollowBack.ToString(), null, commentBody);

                                currentCommentCount++;
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(Log.ActivityFailed,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.UserName, ActivityType.Comment, singleTweetDetail.Id+" ==> "+
                                    commentResponse.Issue?.Message);
                            }
                            if (IsPostFollowProcessCompleted(ActivityType.Comment) || postCommentCount <= currentCommentCount)
                                return;
                            var randomTime = FollowerModel.AfterFollowAction.DelayBetweenCommentRange.GetRandom();
                            GlobusLogHelper.log.Info(Log.DelayBetweenActivity,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType.Comment, randomTime);
                            _delayService.ThreadSleep(TimeSpan.FromSeconds(randomTime));
                        }

                        maxPosition = userFeedDetails.MinPosition;

                        if (string.IsNullOrEmpty(maxPosition)) break;
                    }
                    else
                    {
                        hasNoMorePosts = true;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                if (DominatorAccountModel?.AccountBaseModel?.UserName != null)
                    ex.DebugLog(
                        $"TwtDominator : [Account: {DominatorAccountModel.AccountBaseModel.UserName}] => Error : {ex.Message}. StackTrace => {ex.StackTrace}  (Module => {ActivityType.ToString()})");
            }
        }


        private void StartOtherConfigurationAfterEachFollow(ScrapeResultNew scrapeResult) //ScrapeResultNew scrapeResult
        {
            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                var jobCountersManager = InstanceProvider.GetInstance<IJobCountersManager>();
                var currentFollowingCount = AccountModel.FollowingCount + jobCountersManager[Id];

                #region  Follow and unfollow

                if (FollowerModel.IsChkEnableAutoFollowUnfollow)
                {
                    var followFollowingRatio =
                        (double) AccountModel.FollowersCount / currentFollowingCount; //(int)Math.Round(();
                    var reachedStartStopFollowingLimit =
                        FollowerModel.StartUnFollowToolWhenReach.GetRandom() <=
                        currentFollowingCount; //FollowerModel.StartUnFollowToolWhenReach.InRange(FollowFollowingRatio);
                    var reachedOnlyStopFollowingLimit =
                        FollowerModel.StopFollowToolWhenReach.GetRandom() <=
                        currentFollowingCount; //InRange(FollowFollowingRatio);

                    if (FollowerModel.IsChkStartUnFollow)
                    {
                        if (FollowerModel.IsChkStopFollowToolWhenReach && reachedStartStopFollowingLimit
                            || FollowerModel.IsChkFollowToolGetsTemporaryBlocked && scrapeResult.IsAccountLocked ||
                            FollowerModel.IsChkWhenFollowerFollowingsIsSmallerThan &&
                            FollowerModel.UnFollowerFollowingsMaxValue > followFollowingRatio)
                            try
                            {
                                var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();

                                dominatorScheduler.EnableDisableModules(ActivityType.Follow, ActivityType.Unfollow,
                                    DominatorAccountModel.AccountId);
                            }
                            catch (InvalidOperationException ex)
                            {
                                if (ex.Message.Contains("1001"))
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.UserName, ActivityType,
                                        "Follow activity has met your Auto Enable configuration for unfollow, but you do not have Unfollow configuration saved. Please save the unfollow configuration manually, to restart the Follow/Unfollow activity from this account");
                                else
                                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        DominatorAccountModel.UserName, ActivityType, "");
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                    }
                    else if (FollowerModel.IsChkStopFollowToolWhenReach && reachedOnlyStopFollowingLimit
                             || FollowerModel.IschkwhenTheUnFollowToolGetsTemporaryBlocked &&
                             scrapeResult.IsAccountLocked ||
                             FollowerModel.IsChkStopFollowWhenFollowerFollowingsIsSmallerThan &&
                             FollowerModel.FollowerFollowingsMaxValue > followFollowingRatio)
                    {
                        try
                        {
                            var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();

                            dominatorScheduler.ChangeAccountsRunningStatus(false, AccountId, ActivityType);

                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                                ActivityType, "LangKeyMeetStopFollowingConfigMessage".FromResourceDictionary());
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                }

                if (FollowerModel.IsChkAcceptPendingFollowRequest) AcceptPendingFollowRequest();

                #endregion

                // Process for Unfollow previously followed user

                #region Process for Unfollow previously followed user

                //if (FollowerModel.IsChkStartUnFollow && FollowerModel.UnfollowPrevious.GetRandom() > 0)
                //{
                //    DateTime MaxFollowTime = DateTime.Now.AddHours(-FollowerModel.UnfollowPrevious.GetRandom());

                //    int MaxFollowEpochTime = (int)(DateTime.Now - MaxFollowTime).TotalSeconds;

                //    lstTotalFollowedusers.Where(x => x.InteractionTimeStamp < MaxFollowEpochTime).ToList().ForEach(x =>
                //    {
                //        try
                //        {
                //            GetFollowersAwait(TwtFunct, DominatorAccountModel.AccountBaseModel.UserName); //new  FollowerFollowingResponseHandler().ListOfTwitterUser;
                //            List<string> lstFollowerUsernames = OtherConfigResponseHandler.ListOfTwitterUser.Select(eachFollower => eachFollower.Username).ToList();

                //            if (FollowerModel.IsChkUnfollowfollowedback)
                //            {
                //                if (lstFollowerUsernames.Contains(x.InteractedUsername))
                //                {
                //                    TwtFunct.Unfollow(x.InteractedUserId);
                //                }

                //            }
                //            if (FollowerModel.IsChkUnfollownotfollowedback)
                //            {
                //                if (!lstFollowerUsernames.Contains(x.InteractedUsername))
                //                {
                //                    TwtFunct.Unfollow(x.InteractedUserId);
                //                }
                //            }
                //        }
                //        catch (Exception e)
                //        {
                //            e.DebugLog();
                //        }

                //    });
                //}

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AcceptPendingFollowRequest()
        {
            var account = new AccountModel(DominatorAccountModel);
            var noOfReqToBeAccepted = FollowerModel.AcceptBetween.GetRandom();
            var currentAcceptedReq = 0;

            if (account.IsPrivate)
            {
                string maxPosition = null;
                var hasNoMore = false;

                while (!hasNoMore)
                {
                    var responseHandler = _twitterFunctions.GetPendingRequests(DominatorAccountModel, maxPosition);

                    if (responseHandler.Success)
                    {
                        responseHandler.ListOfTwitterUser?.ForEach(x =>
                        {
                            var acceptReqHandler =
                                _twitterFunctions.AcceptPendingRequest(DominatorAccountModel, x.UserId);

                            if (acceptReqHandler.Success)
                            {
                                currentAcceptedReq++;

                                GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.UserName,
                                    "LangKeyAcceptPendingRequest".FromResourceDictionary(), x.Username);

                                _delayService.ThreadSleep(TdConstants.ConsecutivePostReqInterval);

                                if (noOfReqToBeAccepted <= currentAcceptedReq) return;
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(Log.ActivityFailed,
                                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.UserName,
                                    "LangKeyAcceptPendingRequest".FromResourceDictionary(), x.Username,
                                    acceptReqHandler.Issue?.Message);
                            }
                        });

                        maxPosition = responseHandler.MinPosition;

                        if (!responseHandler.HasMoreResults) hasNoMore = true;
                    }
                    else
                    {
                        hasNoMore = true;
                    }
                }
            }
            //else
            //{
            //    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
            //        DominatorAccountModel.UserName, ActivityType,
            //        "LangKeyAcceptPendingRequestConditionMessage".FromResourceDictionary());
            //}
        }

        #endregion
    }
}