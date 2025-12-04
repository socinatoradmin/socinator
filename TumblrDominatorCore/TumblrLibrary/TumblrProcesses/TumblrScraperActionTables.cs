using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.TumblrQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using TumblrDominatorCore.TumblrLibrary.Processors;
using TumblrDominatorCore.TumblrLibrary.Processors.Posts;
using TumblrDominatorCore.TumblrLibrary.Processors.Users;
using Unity;

namespace TumblrDominatorCore.TumblrLibrary.TumblrProcesses
{
    public class TumblrQueryScraper : QueryScraper
    {
        public TumblrQueryScraper(TumblrJobProcess jobprocess,
            Dictionary<string, Action<QueryInfo>> scrapeWithQueriesActionTable,
            Dictionary<string, Action> scrapeWithoutQueriesActionTable)
            : base(jobprocess, scrapeWithQueriesActionTable, scrapeWithoutQueriesActionTable)
        {
        }
    }

    public interface ITumblrScraperActionTables : IScraperActionTables
    {
        Dictionary<string, Action<QueryInfo>> ScrapeWithQueriesActionTable { get; set; }
        Dictionary<string, Action> ScrapeWithoutQueriesActionTable { get; set; }
    }

    /// <summary>
    ///     Jobs for each module starts and scrape feeds (RunScraper)
    /// </summary>
    public class TumblrScraperActionTables : ITumblrScraperActionTables
    {
        #region Fields

        private readonly ITumblrJobProcess _jobProcess;

        #endregion


        public TumblrScraperActionTables(ITumblrJobProcess jobProcess, IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
            DominatorAccountModel = jobProcess.DominatorAccountModel;
            DominatorAccountModel.Token.ThrowIfCancellationRequested();

            ScrapeWithQueriesActionTable = new Dictionary<string, Action<QueryInfo>>
            {
                {
                    $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(TumblrBroadcastMessageQuery.Keyword)}",
                    StartProcessToFollowWithKeywords
                },
                {
                    $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(TumblrBroadcastMessageQuery.UserFollowing)}",
                    StartProcessToFollowWithFollowings
                },
                {
                    $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(TumblrBroadcastMessageQuery.OwnFollowers)}",
                    StartProcessWithOwnFollowers
                },
                {
                    $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(TumblrBroadcastMessageQuery.HashtagUsers)}",
                    StartProcessToFollowWithHashtag
                },
                {
                    $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(TumblrBroadcastMessageQuery.UserLikedThePost)}",
                    StartProcessToFollowWithfeeds
                },
                {
                    $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(TumblrBroadcastMessageQuery.UserCommentedOnPost)}",
                    StartProcessToFollowWithfeeds
                },
                {
                    $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(TumblrBroadcastMessageQuery.UserReblogedThePost)}",
                    StartProcessToFollowWithfeeds
                },
                {
                    $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(TumblrBroadcastMessageQuery.UserLikedCommentedReblogedThePost)}",
                    StartProcessToFollowWithfeeds
                },


                {
                    $"{ActivityType.Follow}{EnumUtility.GetQueryFromEnum(TumblrQuery.Keyword)}",
                    StartProcessToFollowWithKeywords
                },
                {
                    $"{ActivityType.Follow}{EnumUtility.GetQueryFromEnum(TumblrQuery.UserFollowing)}",
                    StartProcessToFollowWithFollowings
                },
                {
                    $"{ActivityType.Follow}{EnumUtility.GetQueryFromEnum(TumblrQuery.HashtagUsers)}",
                    StartProcessToFollowWithHashtag
                },
                {
                    $"{ActivityType.Follow}{EnumUtility.GetQueryFromEnum(TumblrQuery.UserLikedThePost)}",
                    StartProcessToFollowWithfeeds
                },
                {
                    $"{ActivityType.Follow}{EnumUtility.GetQueryFromEnum(TumblrQuery.UserCommentedOnPost)}",
                    StartProcessToFollowWithfeeds
                },
                {
                    $"{ActivityType.Follow}{EnumUtility.GetQueryFromEnum(TumblrQuery.UserReblogedThePost)}",
                    StartProcessToFollowWithfeeds
                },
                {
                    $"{ActivityType.Follow}{EnumUtility.GetQueryFromEnum(TumblrQuery.UserLikedCommentedReblogedThePost)}",
                    StartProcessToFollowWithfeeds
                },
                {
                    $"{ActivityType.Follow}{EnumUtility.GetQueryFromEnum(TumblrQuery.PostOwner)}",
                    StartProcessToFollowWithPostOwner
                },
                {
                    $"{ActivityType.Follow}{EnumUtility.GetQueryFromEnum(TumblrQuery.CustomUsersList )}",
                    StartProcessToFollowCustomUser
                },

                {
                    $"{ActivityType.Like}{EnumUtility.GetQueryFromEnum(TumblrQuery.Keyword)}",
                    StartProcessToPostWithKeywords
                },
                {
                    $"{ActivityType.Like}{EnumUtility.GetQueryFromEnum(TumblrQuery.HashtagUsers)}",
                    StartProcessToPostWithHashtag
                },


                {
                    $"{ActivityType.Comment}{EnumUtility.GetQueryFromEnum(TumblrQuery.Keyword)}",
                    StartProcessToPostWithKeywords
                },
                {
                    $"{ActivityType.Comment}{EnumUtility.GetQueryFromEnum(TumblrQuery.HashtagUsers)}",
                    StartProcessToPostWithHashtag
                },

                {
                    $"{ActivityType.Reblog}{EnumUtility.GetQueryFromEnum(TumblrQuery.Keyword)}",
                    StartProcessToPostWithKeywords
                },
                {
                    $"{ActivityType.Reblog}{EnumUtility.GetQueryFromEnum(TumblrQuery.HashtagUsers)}",
                    StartProcessToPostWithHashtag
                },


                {
                    $"{ActivityType.PostScraper}{EnumUtility.GetQueryFromEnum(TumblrQuery.Keyword)}",
                    StartProcessToPostWithKeywords
                },
                {
                    $"{ActivityType.PostScraper}{EnumUtility.GetQueryFromEnum(TumblrQuery.HashtagUsers)}",
                    StartProcessToPostWithHashtag
                },

                {
                    $"{ActivityType.UserScraper}{EnumUtility.GetQueryFromEnum(TumblrQuery.Keyword)}",
                    StartProcessToFollowWithKeywords
                },
                {
                    $"{ActivityType.UserScraper}{EnumUtility.GetQueryFromEnum(TumblrQuery.HashtagUsers)}",
                    StartProcessToFollowWithHashtag
                },

                {
                    $"{ActivityType.CommentScraper}{EnumUtility.GetQueryFromEnum(TumblrQuery.Keyword)}",
                    StartProcessToPostWithKeywords
                },
                {
                    $"{ActivityType.CommentScraper}{EnumUtility.GetQueryFromEnum(TumblrQuery.HashtagUsers)}",
                    StartProcessToPostWithHashtag
                }
            };


            ScrapeWithoutQueriesActionTable = new Dictionary<string, Action>
            {
                {$"{ActivityType.Unfollow}", StarProcessForUnfollow},
                {$"{ActivityType.Unlike}", StarProcessForUnLike}
            };
        }

        /// <summary>
        ///     Processor to Scrape Users from Keyword Query
        /// </summary>
        /// <param name="queryInfo"></param>
        private void StartProcessToFollowWithKeywords(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<KeywordsProcessor>();
            processor.Start(queryInfo);
            // IQueryProcessor processor = new KeywordsProcessor(_jobProcess, _dbAccountService, _globalService, _campaignService, TumblrFunct);
            // processor.Start(queryInfo);
        }

        /// <summary>
        ///     Processor to Scrape Posts from Keyword Query
        /// </summary>
        /// <param name="queryInfo"></param>
        private void StartProcessToPostWithKeywords(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<KeywordPostProcessor>();
            processor.Start(queryInfo);
        }

        /// <summary>
        ///     Processor to Scrape Posts from Hashtag Query
        /// </summary>
        /// <param name="queryInfo"></param>
        private void StartProcessToPostWithHashtag(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<HashtagPostProcessor>();
            processor.Start(queryInfo);
        }

        //StartProcessWithOwnFollowers

        private void StartProcessWithOwnFollowers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<OwnFollowerProcerssor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToFollowWithFollowings(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<UserFollowingProcerssor>();
            processor.Start(queryInfo);
            //  IQueryProcessor processor = new UserFollowingProcerssor(_jobProcess, _dbAccountService, _globalService, _campaignService, TumblrFunct);
            //   processor.Start(queryInfo);
        }

        /// <summary>
        ///     Processor to Scrape Users from HashTag Query
        /// </summary>
        /// <param name="queryInfo"></param>
        private void StartProcessToFollowWithHashtag(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<HashtagProcessor>();
            processor.Start(queryInfo);
            //  IQueryProcessor processor = new HashtagProcessor(_jobProcess, _dbAccountService, _globalService, _campaignService, TumblrFunct);
            //  processor.Start(queryInfo);
        }

        /// <summary>
        ///     Processor to Follow Users from Post Owner
        /// </summary>
        /// <param name="queryInfo"></param>
        private void StartProcessToFollowWithPostOwner(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<PostOwnerProcessor>();
            processor.Start(queryInfo);
            //   IQueryProcessor processor = new FeedsProcessor(_jobProcess, _dbAccountService, _globalService, _campaignService, TumblrFunct);
            //  processor.Start(queryInfo);
        }
        private void StartProcessToFollowCustomUser(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomUserProcessor>();
            processor.Start(queryInfo);
        }
        /// <summary>
        ///     Processor to Scrape Users from homefeeds
        /// </summary>
        /// <param name="queryInfo"></param>
        private void StartProcessToFollowWithfeeds(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<FeedsProcessor>();
            processor.Start(queryInfo);
            //   IQueryProcessor processor = new FeedsProcessor(_jobProcess, _dbAccountService, _globalService, _campaignService, TumblrFunct);
            //  processor.Start(queryInfo);
        }

        private void StarProcessForUnfollow()
        {
            IQueryProcessor processor = _unityContainer.Resolve<UnfollowProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        private void StarProcessForUnLike()
        {
            IQueryProcessor processor = _unityContainer.Resolve<UnLikeProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        #region Global properties

        private DominatorAccountModel DominatorAccountModel { get; }

        public Dictionary<string, Action<QueryInfo>> ScrapeWithQueriesActionTable { get; set; }

        public Dictionary<string, Action> ScrapeWithoutQueriesActionTable { get; set; }

        private readonly IUnityContainer _unityContainer;

        #endregion
    }
}