using System;
using System.Collections.Generic;
using System.Threading;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.TdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Helper;
using TwtDominatorCore.TDLibrary.GeneralLibrary.Processors;
using TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.Tweet;
using TwtDominatorCore.TDLibrary.GeneralLibrary.Processors.User;
using Unity;
using static TwtDominatorCore.TDEnums.Enums;

namespace TwtDominatorCore.TDLibrary
{
    public class TwitterQueryScraper : QueryScraper
    {
        public TwitterQueryScraper(ITdJobProcess jobProcess,
            Dictionary<string, Action<QueryInfo>> scrapeWithQueriesActionTable,
            Dictionary<string, Action> scrapeWithoutQueriesActionTable)
            : base(jobProcess, scrapeWithQueriesActionTable, scrapeWithoutQueriesActionTable)
        {
        }
    }

    public interface ITwitterScraperActionTables : IScraperActionTables
    {
    }

    public class TwitterScraperActionTables : ITwitterScraperActionTables
    {
        private readonly ActivityType _activityType;
        private readonly ITdJobProcess _jobProcess;
        private readonly IUnityContainer _unityContainer;


        public TwitterScraperActionTables(ITdJobProcess jobProcess, IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
            _activityType = jobProcess.ActivityType;
            _jobProcess = jobProcess;
            var ScrapeCase = GetTwitterElementsByActivityType(_activityType);
            try
            {
                jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                ScrapeWithQueriesActionTable = new Dictionary<string, Action<QueryInfo>>();
                switch (ScrapeCase)
                {
                    case TwitterElements.UsersTweetFunction:
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdUserInteractionQueryEnum.Keywords)}",
                            StartProcessForUserOrTag);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdUserInteractionQueryEnum.Hashtags)}",
                            StartProcessForUserOrTag);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdUserInteractionQueryEnum.LocationUsers)}",
                            StartUserProcessForTag);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdUserInteractionQueryEnum.SomeonesFollowers)}",
                            StartUserProcessForSomeonesFollowers);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdUserInteractionQueryEnum.SomeonesFollowings)}",
                            StartUserProcessForSomeonesFollowings);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdUserInteractionQueryEnum.FollowersOfFollowers)}",
                            StartUserProcessForFollowersOfFollowers);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdUserInteractionQueryEnum.FollowersOfFollowings)}",
                            StartUserProcessForFollowersOfFollowings);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdUserInteractionQueryEnum.CustomUsersList)}",
                            StartProcessForCustomUsers);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdUserInteractionQueryEnum.NearMyLocation)}",
                            StartProcessForUserOrTag);
                        //ScrapeWithQueriesActionTable.Add(
                        //    $"{_activityType}{EnumUtility.GetQueryFromEnum(TdUserInteractionQueryEnum.UsersWhoLikedOnTweet)}",
                        //    StartUserProcessForMediaLikers);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdUserInteractionQueryEnum.UsersWhoRetweetedTweet)}",
                            StartUserProcessForRetweetedusers);

                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdUserInteractionQueryEnum.UsersWhoCommentedOnTweet)}",
                            StartProcessForMediaCommenters);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdUserInteractionQueryEnum.UserScraperCampaign)}",
                            StartProcessForScrapedUserCampaign);
                        break;

                    case TwitterElements.TweetFunction:
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdUserInteractionQueryEnum.Keywords)}",
                            StartProcessForUserOrTag);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdUserInteractionQueryEnum.Hashtags)}",
                            StartProcessForUserOrTag);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.TweetScraperCampaign)}",
                            StartProcessForScrapedTweetCampaign);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.UsersWhoRetweetedTweet)}",
                            StartTweetProcessForRetweetedusers);
                        //ScrapeWithQueriesActionTable.Add(
                        //    $"{_activityType}{EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.UsersWhoLikedOnTweet)}",
                        //    StartTweetProcessForMediaLikers);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.LocationTweets)}",
                            StartTweetProcessForTag);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.CustomTweetsList)}",
                            StartProcessWithCustomTweets);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.SocinatorPublisherCampaign)}",
                            StartProcessWithSocinatorPublishedPosts);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.NearMyLocation)}",
                            StartProcessForUserOrTag);


                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.CommentedTweet)}",
                            StartProcessForCommentedTweets);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.UsersWhoCommentedOnTweet)}",
                            StartProcessForCommentedUsers);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.SomeonesFollowers)}",
                            StartTweetProcessForSomeonesFollowers);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.SomeonesFollowings)}",
                            StartTweetProcessForSomeonesFollowings);

                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.FollowersOfFollowings)}",
                            StartTweetProcessForFollowersOfFollowings);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.FollowersOfFollowers)}",
                            StartTweetProcessForFollowersOfFollowers);
                        ScrapeWithQueriesActionTable.Add(
                            $"{_activityType}{EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.SpecificUserTweets)}",
                            StartTweetProcessForTag);
                        //ScrapeWithQueriesActionTable.Add(
                        //    $"{_activityType}{EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.TweetsLikedBySpecificUser)}",
                        //    StartProcessForSpecficUserLikedTweets);
                        break;
                }

                ScrapeWithoutQueriesActionTable = new Dictionary<string, Action>
                {
                    {"Unfollow", ScrapeToUnfollow},
                    {"FollowBack", ScrapeToFollowBack},
                    {"Delete", ScrapeToDelete},
                    {"BroadcastMessages", ScrapeToMessage},
                    {"AutoReplyToNewMessage", ScrapeNewMessageForAutoReply},
                    {"SendMessageToFollower", ScrapeNewFollowersForSendMessageORWelcomeTweet},
                    {"WelcomeTweet", ScrapeNewFollowersForSendMessageORWelcomeTweet},
                    {"Unlike", StartProcessForUnlikeTweet}
                };

                if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
                    Thread.CurrentThread.Name = jobProcess.AccountName;
            }
            catch (OperationCanceledException ex)
            {
                ex.DebugLog("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public Dictionary<string, Action<QueryInfo>> ScrapeWithQueriesActionTable { get; }
        public Dictionary<string, Action> ScrapeWithoutQueriesActionTable { get; }

        public TwitterElements GetTwitterElementsByActivityType(ActivityType activity)
        {
            if (activity == ActivityType.Reposter || activity == ActivityType.Retweet ||
                activity == ActivityType.TweetScraper || activity == ActivityType.Like ||
                activity == ActivityType.Comment)
                return TwitterElements.TweetFunction;
            if (activity == ActivityType.UserScraper || activity == ActivityType.Follow ||
                activity == ActivityType.Mute || activity == ActivityType.TweetTo)
                return TwitterElements.UsersTweetFunction;
            return TwitterElements.None;
        }


        #region Start Scrape Process for all modules

        #region With Queries

        public void StartUserProcessForTag(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<KeywordHashLocationProcessor>();
            baseTwitterProcess.Start(queryInfo);
        }

        public void StartTweetProcessForTag(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<KeywordHashLocationTweetProcessor>();
            baseTwitterProcess.Start(queryInfo);
        }

        private void StartUserProcessForSomeonesFollowers(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<SomeonesFollowers>();
            baseTwitterProcess.Start(queryInfo);
        }

        private void StartTweetProcessForSomeonesFollowers(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<SomeonesFollowersTweet>();
            baseTwitterProcess.Start(queryInfo);
        }

        private void StartUserProcessForSomeonesFollowings(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<SomeonesFollowings>();
            baseTwitterProcess.Start(queryInfo);
        }

        private void StartTweetProcessForSomeonesFollowings(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<SomeonesFollowingsTweet>();
            baseTwitterProcess.Start(queryInfo);
        }

        private void StartUserProcessForFollowersOfFollowers(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<FollowersOfFollowers>();
            baseTwitterProcess.Start(queryInfo);
        }

        private void StartTweetProcessForFollowersOfFollowers(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<FollowersOfFollowersTweet>();
            baseTwitterProcess.Start(queryInfo);
        }

        public void StartProcessForUserOrTag(QueryInfo queryInfo)
        {
            if (TwtStringHelper.IsUserProcessor(_activityType))
                StartUserProcessForTag(queryInfo);
            else
                StartTweetProcessForTag(queryInfo);
        }

        /// <summary>
        ///     Getting user tweets of specific user it include useritself
        /// </summary>
        /// <param name="queryInfo"></param>
        private void StartProcessForSpecficUserLikedTweets(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<SpecificUserLikedTweets>();
            baseTwitterProcess.Start(queryInfo);
        }

        /// <summary>
        ///     StartProcessForFollowersOfFollowings for followers and their tweets
        /// </summary>
        /// <param name="queryInfo"></param>
        private void StartUserProcessForFollowersOfFollowings(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<FollowersOfFollowings>();
            baseTwitterProcess.Start(queryInfo);
        }

        private void StartTweetProcessForFollowersOfFollowings(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<FollowersOfFollowingsTweet>();
            baseTwitterProcess.Start(queryInfo);
        }

        private void StartProcessForCommentedTweets(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<CommentedTweets>();
            baseTwitterProcess.Start(queryInfo);
        }

        /// <summary>
        ///     get users who commented on tweet and get his tweets
        /// </summary>
        /// <param name="queryInfo"></param>
        private void StartProcessForCommentedUsers(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<CommentedUsers>();
            baseTwitterProcess.Start(queryInfo);
        }

        public void StartUserProcessForRetweetedusers(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<RetweetedUsers>();
            baseTwitterProcess.Start(queryInfo);
        }

        public void StartTweetProcessForRetweetedusers(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<RetweetedUsersTweet>();
            baseTwitterProcess.Start(queryInfo);
        }

        private void StartUserProcessForMediaLikers(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<MediaLikers>();
            baseTwitterProcess.Start(queryInfo);
        }

        private void StartTweetProcessForMediaLikers(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<MediaLikersTweet>();
            baseTwitterProcess.Start(queryInfo);
        }

        private void StartProcessForMediaCommenters(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<MediaCommenters>();
            baseTwitterProcess.Start(queryInfo);
        }

        private void StartProcessForCustomUsers(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<CustomUsers>();
            baseTwitterProcess.Start(queryInfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartProcessWithCustomTweets(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<CustomTweet>();
            baseTwitterProcess.Start(queryInfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartProcessWithSocinatorPublishedPosts(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<SocinatorPublishedPosts>();
            baseTwitterProcess.Start(queryInfo);
        }


        private void StartProcessForScrapedUserCampaign(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<ScrapedUserCampaign>();
            baseTwitterProcess.Start(queryInfo);
        }

        private void StartProcessForScrapedTweetCampaign(QueryInfo queryInfo)
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<ScrapedTweetCampaign>();
            baseTwitterProcess.Start(queryInfo);
        }

        #endregion

        #region Without queries

        #region  Unfollow

        private void ScrapeToUnfollow()
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<UnfollowUser>();
            baseTwitterProcess.Start(QueryInfo.NoQuery);
        }

        #endregion

        #region FollowBack

        private void ScrapeToFollowBack()
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<FollowBack>();
            baseTwitterProcess.Start(QueryInfo.NoQuery);
        }

        #endregion

        #region Delete

        private void ScrapeToDelete()
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<DeleteProcessor>();
            baseTwitterProcess.Start(QueryInfo.NoQuery);
        }

        #endregion

        #region BroadCast Message

        private void ScrapeToMessage()
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<BroadcastMessages>();
            baseTwitterProcess.Start(QueryInfo.NoQuery);
        }

        #endregion

        #region AutoReply

        private void ScrapeNewMessageForAutoReply()
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<AutoReply>();
            baseTwitterProcess.Start(QueryInfo.NoQuery);
        }

        #endregion

        #region Message or welcome tweet to _unityContainer.Resolve<followers

        private void ScrapeNewFollowersForSendMessageORWelcomeTweet()
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<SendMessageORWelcomeTweet>();
            baseTwitterProcess.Start(QueryInfo.NoQuery);
        }

        #endregion

        #region Unlike Tweets

        private void StartProcessForUnlikeTweet()
        {
            IQueryProcessor baseTwitterProcess = _unityContainer.Resolve<UnlikeTweet>();
            baseTwitterProcess.Start(QueryInfo.NoQuery);
        }

        #endregion

        #endregion

        #endregion
    }
}