using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.RdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.Processors;
using RedditDominatorCore.RDLibrary.Processors.AutoActivity;
using RedditDominatorCore.RDLibrary.Processors.Channel;
using RedditDominatorCore.RDLibrary.Processors.CommentScraper;
using RedditDominatorCore.RDLibrary.Processors.EditComment;
using RedditDominatorCore.RDLibrary.Processors.Messanger;
using RedditDominatorCore.RDLibrary.Processors.Post;
using RedditDominatorCore.RDLibrary.Processors.User;
using System;
using System.Collections.Generic;
using Unity;

namespace RedditDominatorCore.RDLibrary
{
    public class RedditQueryScraper : QueryScraper
    {
        public RedditQueryScraper(IRdJobProcess jobProcess,
            Dictionary<string, Action<QueryInfo>> scrapeWithQueriesActionTable,
            Dictionary<string, Action> scrapeWithoutQueriesActionTable)
            : base(jobProcess, scrapeWithQueriesActionTable, scrapeWithoutQueriesActionTable)
        {
        }
    }

    public interface IRedditScraperActionTables : IScraperActionTables
    {
    }

    public class RedditScraperActionTables : IRedditScraperActionTables
    {
        private readonly IRdJobProcess _jobProcess;
        private readonly IUnityContainer _unityContainer;

        public RedditScraperActionTables(IRdJobProcess jobProcess, IUnityContainer unityContainer)
        {
            _jobProcess = jobProcess;

            _unityContainer = unityContainer;

            ScrapeWithoutQueriesActionTable = new Dictionary<string, Action>
            {
                {$"{ActivityType.Unfollow}", StartProcessToUnfollow},
                {$"{ActivityType.UnSubscribe}", StartProcessToUnSubscribe},
                {$"{ActivityType.EditComment}", StartProcessToEditComment},
                {$"{ActivityType.AutoReplyToNewMessage}",StartProcessToAutoReply },
                {$"{ActivityType.AutoActivity}",StartAutoActivity }
            };


            ScrapeWithQueriesActionTable = new Dictionary<string, Action<QueryInfo>>
            {
                #region Grow Followers

                {
                    $"{ActivityType.Follow}{EnumUtility.GetQueryFromEnum(UserQuery.Keywords)}",
                    StartProcessToFollowWithKeywords
                },
                {
                    $"{ActivityType.Follow}{EnumUtility.GetQueryFromEnum(UserQuery.CustomUsers)}",
                    StartProcessToFollowWithCustomUrl
                },
                {
                    $"{ActivityType.Follow}{EnumUtility.GetQueryFromEnum(UserQuery.UsersWhoCommentedOnPost)}",
                    StartProcessToFollowWhoCommentedOnPost
                },

                #endregion

                #region Voting

                {
                    $"{ActivityType.Upvote}{EnumUtility.GetQueryFromEnum(PostQuery.Keywords)}",
                    StartProcessToVoteWithKeywords
                },
                {
                    $"{ActivityType.Upvote}{EnumUtility.GetQueryFromEnum(PostQuery.CustomUrl)}",
                    StartProcessForPostsWithCustomUrl
                },
                {
                    $"{ActivityType.Upvote}{EnumUtility.GetQueryFromEnum(PostQuery.SocinatorPublisherCampaign)}",
                    StartProcessForPostsFromPublisher
                },
                {
                    $"{ActivityType.Upvote}{EnumUtility.GetQueryFromEnum(PostQuery.CommunityUrl)}",
                    StartProcessForCommunityPost
                },
                {
                    $"{ActivityType.Upvote}{EnumUtility.GetQueryFromEnum(PostQuery.SpecificUserPost)}",
                    StartProcessForSpecificUserPost
                },

                {
                    $"{ActivityType.Downvote}{EnumUtility.GetQueryFromEnum(PostQuery.Keywords)}",
                    StartProcessToVoteWithKeywords
                },
                {
                    $"{ActivityType.Downvote}{EnumUtility.GetQueryFromEnum(PostQuery.CustomUrl)}",
                    StartProcessForPostsWithCustomUrl
                },
                {
                    $"{ActivityType.Downvote}{EnumUtility.GetQueryFromEnum(PostQuery.SocinatorPublisherCampaign)}",
                    StartProcessForPostsFromPublisher
                },
                {
                    $"{ActivityType.Downvote}{EnumUtility.GetQueryFromEnum(PostQuery.CommunityUrl)}",
                    StartProcessForCommunityPost
                },
                {
                    $"{ActivityType.Downvote}{EnumUtility.GetQueryFromEnum(PostQuery.SpecificUserPost)}",
                    StartProcessForSpecificUserPost
                },

                {
                    $"{ActivityType.RemoveVote}{EnumUtility.GetQueryFromEnum(PostQuery.CustomUrl)}",
                    StartProcessForPostsWithCustomUrl
                },
                {
                    $"{ActivityType.RemoveVote}{EnumUtility.GetQueryFromEnum(PostQuery.CommunityUrl)}",
                    StartProcessForCommunityPost
                },
                {
                    $"{ActivityType.RemoveVote}{EnumUtility.GetQueryFromEnum(PostQuery.SpecificUserPost)}",
                    StartProcessForSpecificUserPost
                },

                #endregion

                #region Sub Reddit

                {
                    $"{ActivityType.Subscribe}{EnumUtility.GetQueryFromEnum(CommunityQuery.Keywords)}",
                    StartProcessForSubRedditWithKeywords
                },
                {
                    $"{ActivityType.Subscribe}{EnumUtility.GetQueryFromEnum(CommunityQuery.CustomUrl)}",
                    StartProcessForSubRedditWithCustomUrl
                },

                #endregion

                #region Scraping

                {
                    $"{ActivityType.UserScraper}{EnumUtility.GetQueryFromEnum(UserQuery.Keywords)}",
                    StartProcessToFollowWithKeywords
                },
                {
                    $"{ActivityType.UserScraper}{EnumUtility.GetQueryFromEnum(UserQuery.CustomUsers)}",
                    StartProcessToFollowWithCustomUrl
                },

                {
                    $"{ActivityType.UrlScraper}{EnumUtility.GetQueryFromEnum(PostQuery.Keywords)}",
                    StartProcessForUrlScraperWithKeyword
                },
                {
                    $"{ActivityType.UrlScraper}{EnumUtility.GetQueryFromEnum(PostQuery.CustomUrl)}",
                    StartProcessForCommunityPost
                },


                {
                    $"{ActivityType.ChannelScraper}{EnumUtility.GetQueryFromEnum(CommunityQuery.Keywords)}",
                    StartProcessForSubRedditWithKeywords
                },
                {
                    $"{ActivityType.ChannelScraper}{EnumUtility.GetQueryFromEnum(CommunityQuery.CustomUrl)}",
                    StartProcessForSubRedditWithCustomUrl
                },

                {
                    $"{ActivityType.CommentScraper}{EnumUtility.GetQueryFromEnum(PostQuery.Keywords)}",
                    StartProcessForCommentScraperWithKeyword
                },
                {
                    $"{ActivityType.CommentScraper}{EnumUtility.GetQueryFromEnum(PostQuery.CustomUrl)}",
                    StartProcessForCommentScraperWithCustomUrl
                },
                {
                    $"{ActivityType.CommentScraper}{EnumUtility.GetQueryFromEnum(PostQuery.CommunityUrl)}",
                    StartProcessForCommentScraperWithCommunityUrl
                },

                #endregion

                #region Comment

                {
                    $"{ActivityType.Comment}{EnumUtility.GetQueryFromEnum(PostQuery.Keywords)}",
                    StartProcessToVoteWithKeywords
                },
                {
                    $"{ActivityType.Comment}{EnumUtility.GetQueryFromEnum(PostQuery.CustomUrl)}",
                    StartProcessForPostsWithCustomUrl
                },
                {
                    $"{ActivityType.Comment}{EnumUtility.GetQueryFromEnum(PostQuery.SocinatorPublisherCampaign)}",
                    StartProcessForPostsFromPublisher
                },

                #endregion

                #region Reply

                {
                    $"{ActivityType.Reply}{EnumUtility.GetQueryFromEnum(PostQuery.CustomUrl)}",
                    StartProcessToReplyWithCustomUrl
                },

                #endregion

                #region Messanger

                {
                    $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(UserQuery.Keywords)}",
                    StartProcessToFollowWithKeywords
                },
                {
                    $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(UserQuery.CustomUsers)}",
                    StartProcessToFollowWithCustomUrl
                },

                #endregion

                #region CommentVoting

                {
                    $"{ActivityType.UpvoteComment}{EnumUtility.GetQueryFromEnum(PostQuery.CustomUrl)}",
                    StartProcessToReplyWithCustomUrl
                },
                {
                    $"{ActivityType.DownvoteComment}{EnumUtility.GetQueryFromEnum(PostQuery.CustomUrl)}",
                    StartProcessToReplyWithCustomUrl
                },
                {
                    $"{ActivityType.RemoveVoteComment}{EnumUtility.GetQueryFromEnum(PostQuery.CustomUrl)}",
                    StartProcessToReplyWithCustomUrl
                },
                {
                    $"{ActivityType.RemoveVoteComment}{EnumUtility.GetQueryFromEnum(PostQuery.CommunityUrl)}",
                    StartProcessToReplyWithCommunityUrl
                },
                {
                    $"{ActivityType.RemoveVoteComment}{EnumUtility.GetQueryFromEnum(PostQuery.SpecificUserPost)}",
                    StartProcessToReplyWithSpecificUserPost
                }

                #endregion
            };
        }

        private void StartAutoActivity()
        {
            var objQueryInfo = new QueryInfo { QueryType = "LangKeyRedditAutoActivity".FromResourceDictionary() };
            IQueryProcessor processor = _unityContainer.Resolve<PostAutoActivityProcessor>();
            processor.Start(objQueryInfo);
        }

        public Dictionary<string, Action> ScrapeWithoutQueriesActionTable { get; set; }
        public Dictionary<string, Action<QueryInfo>> ScrapeWithQueriesActionTable { get; set; }

        private void StartProcessToVoteWithKeywords(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<KeywordsPostProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToAutoReply()
        {
            var objQueryInfo = new QueryInfo { QueryType = "LangKeyAutoReplyToNewMessage".FromResourceDictionary() };
            IQueryProcessor processor = _unityContainer.Resolve<AutoReplyProcessor>();
            processor.Start(objQueryInfo);
        }

        private void StartProcessForSubRedditWithKeywords(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<KeywordsChannelProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessForPostsWithCustomUrl(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomUserPostProcessor>();
            processor.Start(queryInfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartProcessForPostsFromPublisher(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<SocianatorPublisherCampaignProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToFollowWithKeywords(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<KeywordsProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToFollowWithCustomUrl(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomUrlProcessor>();
            processor.Start(queryInfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartProcessToFollowWhoCommentedOnPost(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<UserWhocommentedOnPostProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessForSubRedditWithCustomUrl(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomUserChannelProcessor>();
            processor.Start(queryInfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartProcessToReplyWithCustomUrl(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomUserReplyProcessor>();
            processor.Start(queryInfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }
        private void StartProcessToReplyWithCommunityUrl(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomUserReplyProcessor>();
            processor.Start(queryInfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }
        private void StartProcessToReplyWithSpecificUserPost(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomUserReplyProcessor>();
            processor.Start(queryInfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }
        private void StartProcessToUnSubscribe()
        {
            var objQueryInfo = new QueryInfo { QueryType = "unsubscribe" };
            IQueryProcessor processor = _unityContainer.Resolve<UnsubscribeProcessor>();
            processor.Start(objQueryInfo);
        }

        private void StartProcessToUnfollow()
        {
            var objQueryInfo = new QueryInfo { QueryType = "unfollow" };
            IQueryProcessor processor = _unityContainer.Resolve<UnfollowProcessor>();
            processor.Start(objQueryInfo);
        }

        private void StartProcessForCommentScraperWithKeyword(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<KeywordCommentScraperProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessForCommentScraperWithCustomUrl(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomCommentScraperProcessor>();
            processor.Start(queryInfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartProcessForCommentScraperWithCommunityUrl(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CommunityCommentScraperProcessor>();
            processor.Start(queryInfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartProcessForUrlScraperWithKeyword(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<KeywordsPostProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessToEditComment()
        {
            var objQueryInfo = new QueryInfo { QueryType = "EditComment" };
            IQueryProcessor processor = _unityContainer.Resolve<EditCommentProcessor>();
            processor.Start(objQueryInfo);
        }

        private void StartProcessForCommunityPost(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CommunityAndUserPostProcessor>();
            processor.Start(queryInfo);
        }

        private void StartProcessForSpecificUserPost(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CommunityAndUserPostProcessor>();
            processor.Start(queryInfo);
        }
    }
}