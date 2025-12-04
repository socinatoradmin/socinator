using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.QdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using QuoraDominatorCore.QdLibrary.Processors;
using QuoraDominatorCore.QdLibrary.Processors.Answers;
using QuoraDominatorCore.QdLibrary.Processors.Message;
using QuoraDominatorCore.QdLibrary.Processors.Post;
using QuoraDominatorCore.QdLibrary.Processors.Question;
using QuoraDominatorCore.QdLibrary.Processors.Users;
using Unity;
using UserQueryParameters = DominatorHouseCore.Enums.QdQuery.UserQueryParameters;

namespace QuoraDominatorCore.QdLibrary
{
    public interface IQuoraScraperActionTables : IScraperActionTables
    {
    }

    public class QuoraScraperActionTables : IQuoraScraperActionTables
    {
        public QuoraScraperActionTables(IQdJobProcess jobProcess, IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
            _jobProcess = jobProcess;

            #region Action with Query

            try
            {
                ScrapeWithQueriesActionTable = new Dictionary<string, Action<QueryInfo>>
                {
                    {
                        $"{ActivityType.Follow}{EnumUtility.GetQueryFromEnum(FollowerQuery.Keywords)}",
                        ScrapeKeywordWithJobProcess
                    },
                    {
                        $"{ActivityType.Follow}{EnumUtility.GetQueryFromEnum(FollowerQuery.SomeonesFollowers)}",
                        StartProcessForSomeonesFollowers
                    },
                    {
                        $"{ActivityType.Follow}{EnumUtility.GetQueryFromEnum(FollowerQuery.SomeonesFollowings)}",
                        StartProcessForSomeonesFollowings
                    },
                    {
                        $"{ActivityType.Follow}{EnumUtility.GetQueryFromEnum(FollowerQuery.FollowersOfFollowings)}",
                        StartProcessForFollowersOfFollowings
                    },
                    {
                        $"{ActivityType.Follow}{EnumUtility.GetQueryFromEnum(FollowerQuery.FollowersOfFollowers)}",
                        StartProcessForFollowersOfFollowers
                    },
                    {
                        $"{ActivityType.Follow}{EnumUtility.GetQueryFromEnum(FollowerQuery.CustomUsers)}",
                        StartProcessForCustomUsers
                    },

                    {
                        $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(BroadcastMessageQuery.Keywords)}",
                        ScrapeKeywordWithJobProcess
                    },
                    {
                        $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(BroadcastMessageQuery.CustomUsers)}",
                        StartProcessForCustomUsers
                    },
                    //{ $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(BroadcastMessageQuery.CustomUrl)}",StartProcessWithCustomUrl},
                    {
                        $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(BroadcastMessageQuery.SomeonesFollowers)}",
                        StartProcessForSomeonesFollowers
                    },
                    {
                        $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(BroadcastMessageQuery.SomeonesFollowings)}",
                        StartProcessForSomeonesFollowings
                    },
                    {
                        $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(BroadcastMessageQuery.FollowersOfFollowings)}",
                        StartProcessForFollowersOfFollowings
                    },
                    {
                        $"{ActivityType.BroadcastMessages}{EnumUtility.GetQueryFromEnum(BroadcastMessageQuery.FollowersOfFollowers)}",
                        StartProcessForFollowersOfFollowers
                    },


                    {
                        $"{ActivityType.ReportUsers}{EnumUtility.GetQueryFromEnum(UserQueryParameters.Keywords)}",
                        ScrapeKeywordWithJobProcess
                    },
                    {
                        $"{ActivityType.ReportUsers}{EnumUtility.GetQueryFromEnum(UserQueryParameters.SomeonesFollowers)}",
                        StartProcessForSomeonesFollowers
                    },
                    {
                        $"{ActivityType.ReportUsers}{EnumUtility.GetQueryFromEnum(UserQueryParameters.SomeonesFollowings)}",
                        StartProcessForSomeonesFollowings
                    },
                    {
                        $"{ActivityType.ReportUsers}{EnumUtility.GetQueryFromEnum(UserQueryParameters.CustomUsers)}",
                        StartProcessForCustomUsers
                    },
                    {
                        $"{ActivityType.ReportUsers}{EnumUtility.GetQueryFromEnum(UserQueryParameters.EngagedUsers)}",
                        StartProcessForEngagedUser
                    },
                    //{ $"{ActivityType.ReportUsers}{EnumUtility.GetQueryFromEnum(DominatorHouseCore.Enums.QdQuery.UserQueryParameters.TopicFollowers)}",StartReportUserProcessWithTopic},
                    {
                        $"{ActivityType.ReportUsers}{EnumUtility.GetQueryFromEnum(UserQueryParameters.AnswerUpvoters)}",
                        StartReportUserProcessWithUpvoters
                    },


                    {
                        $"{ActivityType.ReportAnswers}{EnumUtility.GetQueryFromEnum(AnswerQueryParameters.Keywords)}",
                        ScrapeAnswerByKeyword
                    },
                    {
                        $"{ActivityType.ReportAnswers}{EnumUtility.GetQueryFromEnum(AnswerQueryParameters.CustomUrl)}",
                        StartProcessAnswerWithCustomUrl
                    },
                    {
                        $"{ActivityType.ReportAnswers}{EnumUtility.GetQueryFromEnum(AnswerQueryParameters.CustomUser)}",
                        ScraperAnswerOfUser
                    },
                    {
                        $"{ActivityType.ReportAnswers}{EnumUtility.GetQueryFromEnum(AnswerQueryParameters.TopicList)}",
                        StartProcessWithTopicList
                    },

                    {
                        $"{ActivityType.AnswersScraper}{EnumUtility.GetQueryFromEnum(AnswerQueryParameters.Keywords)}",
                        ScrapeAnswerByKeyword
                    },
                    {
                        $"{ActivityType.AnswersScraper}{EnumUtility.GetQueryFromEnum(AnswerQueryParameters.CustomUser)}",
                        ScraperAnswerOfUser
                    },
                    {
                        $"{ActivityType.AnswersScraper}{EnumUtility.GetQueryFromEnum(AnswerQueryParameters.CustomUrl)}",
                        StartProcessAnswerWithCustomUrl
                    },
                    {
                        $"{ActivityType.AnswersScraper}{EnumUtility.GetQueryFromEnum(AnswerQueryParameters.TopicList)}",
                        StartProcessWithTopicList
                    },

                    {
                        $"{ActivityType.DownvoteAnswers}{EnumUtility.GetQueryFromEnum(AnswerQueryParameters.Keywords)}",
                        ScrapeAnswerByKeyword
                    },
                    {
                        $"{ActivityType.DownvoteAnswers}{EnumUtility.GetQueryFromEnum(AnswerQueryParameters.CustomUser)}",
                        ScraperAnswerOfUser
                    },
                    {
                        $"{ActivityType.DownvoteAnswers}{EnumUtility.GetQueryFromEnum(AnswerQueryParameters.CustomUrl)}",
                        StartProcessAnswerWithCustomUrl
                    },
                    {
                        $"{ActivityType.DownvoteAnswers}{EnumUtility.GetQueryFromEnum(AnswerQueryParameters.TopicList)}",
                        StartProcessWithTopicList
                    },

                    {
                        $"{ActivityType.UpvoteAnswers}{EnumUtility.GetQueryFromEnum(AnswerQueryParameters.Keywords)}",
                        ScrapeAnswerByKeyword
                    },
                    {
                        $"{ActivityType.UpvoteAnswers}{EnumUtility.GetQueryFromEnum(AnswerQueryParameters.CustomUser)}",
                        ScraperAnswerOfUser
                    },
                    {
                        $"{ActivityType.UpvoteAnswers}{EnumUtility.GetQueryFromEnum(AnswerQueryParameters.CustomUrl)}",
                        StartProcessAnswerWithCustomUrl
                    },
                    {
                        $"{ActivityType.UpvoteAnswers}{EnumUtility.GetQueryFromEnum(AnswerQueryParameters.TopicList)}",
                        StartProcessWithTopicList
                    },

                    {
                        $"{ActivityType.UserScraper}{EnumUtility.GetQueryFromEnum(UserQueryParameters.Keywords)}",
                        ScrapeKeywordWithJobProcess
                    },
                    {
                        $"{ActivityType.UserScraper}{EnumUtility.GetQueryFromEnum(UserQueryParameters.SomeonesFollowers)}",
                        StartProcessForSomeonesFollowers
                    },
                    {
                        $"{ActivityType.UserScraper}{EnumUtility.GetQueryFromEnum(UserQueryParameters.SomeonesFollowings)}",
                        StartProcessForSomeonesFollowings
                    },
                    {
                        $"{ActivityType.UserScraper}{EnumUtility.GetQueryFromEnum(UserQueryParameters.CustomUsers)}",
                        StartProcessForCustomUsers
                    },
                    {
                        $"{ActivityType.UserScraper}{EnumUtility.GetQueryFromEnum(UserQueryParameters.EngagedUsers)}",
                        StartProcessForEngagedUser
                    },
                    //{ $"{ActivityType.UserScraper}{EnumUtility.GetQueryFromEnum(DominatorHouseCore.Enums.QdQuery.UserQueryParameters.TopicFollowers)}",StartReportUserProcessWithTopic},
                    {
                        $"{ActivityType.UserScraper}{EnumUtility.GetQueryFromEnum(UserQueryParameters.AnswerUpvoters)}",
                        StartReportUserProcessWithUpvoters
                    },


                    {
                        $"{ActivityType.QuestionsScraper}{EnumUtility.GetQueryFromEnum(QuestionQueryParameters.Keywords)}",
                        ScrapeQuestionByKeyword
                    },
                    {
                        $"{ActivityType.QuestionsScraper}{EnumUtility.GetQueryFromEnum(QuestionQueryParameters.CustomUser)}",
                        ScrapeQuestionByUser
                    },
                    {
                        $"{ActivityType.QuestionsScraper}{EnumUtility.GetQueryFromEnum(QuestionQueryParameters.CustomUrl)}",
                        StartProcessQuestionsWithCustomUrl
                    },
                    {
                        $"{ActivityType.QuestionsScraper}{EnumUtility.GetQueryFromEnum(QuestionQueryParameters.TopicFaqs)}",
                        StartDownvoteQuestionProcessWithTopicFaqs
                    },

                    {
                        $"{ActivityType.DownvoteQuestions}{EnumUtility.GetQueryFromEnum(QuestionQueryParameters.Keywords)}",
                        ScrapeQuestionByKeyword
                    },
                    {
                        $"{ActivityType.DownvoteQuestions}{EnumUtility.GetQueryFromEnum(QuestionQueryParameters.CustomUser)}",
                        ScrapeQuestionByUser
                    },
                    {
                        $"{ActivityType.DownvoteQuestions}{EnumUtility.GetQueryFromEnum(QuestionQueryParameters.CustomUrl)}",
                        StartProcessQuestionsWithCustomUrl
                    },
                    {
                        $"{ActivityType.DownvoteQuestions}{EnumUtility.GetQueryFromEnum(QuestionQueryParameters.TopicFaqs)}",
                        StartDownvoteQuestionProcessWithTopicFaqs
                    },

                    {
                        $"{ActivityType.AnswerOnQuestions}{EnumUtility.GetQueryFromEnum(QuestionQueryParameters.Keywords)}",
                        AnswerOnQuestionsByKeyword
                    },
                    {
                        $"{ActivityType.AnswerOnQuestions}{EnumUtility.GetQueryFromEnum(QuestionQueryParameters.CustomUrl)}",
                        AnswerOnQuestionsByCustomUrl
                    },
                    {
                        $"{ActivityType.AnswerOnQuestions}{EnumUtility.GetQueryFromEnum(QuestionQueryParameters.CustomUser)}",
                        AnswerOnQuestionsByCustomUser
                    },
                    {
                        $"{ActivityType.AnswerOnQuestions}{EnumUtility.GetQueryFromEnum(QuestionQueryParameters.TopicFaqs)}",
                        AnswerOnQuestionsByTopicFaqs
                    },
                    {
                        $"{ActivityType.UpvotePost}{EnumUtility.GetQueryFromEnum(PostQueryParameters.Keywords)}",
                        VotePostByKeyword
                    },
                    {
                        $"{ActivityType.UpvotePost}{EnumUtility.GetQueryFromEnum(PostQueryParameters.CustomUrl)}",
                        VotePostByCustomUrl
                    },
                    {
                        $"{ActivityType.DownVotePost}{EnumUtility.GetQueryFromEnum(PostQueryParameters.Keywords)}",
                        VotePostByKeyword
                    },
                    {
                        $"{ActivityType.DownVotePost}{EnumUtility.GetQueryFromEnum(PostQueryParameters.CustomUrl)}",
                        VotePostByCustomUrl
                    },
                    {
                        $"{ActivityType.UpvotePost}{EnumUtility.GetQueryFromEnum(PostQueryParameters.CustomUsers)}",
                        VotePostByCustomUser
                    },
                    {
                        $"{ActivityType.DownVotePost}{EnumUtility.GetQueryFromEnum(PostQueryParameters.CustomUsers)}",
                        VotePostByCustomUser
                    }
                };
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

            #region Action without Query

            ScrapeWithoutQueriesActionTable = new Dictionary<string, Action>
            {
                {$"{ActivityType.Unfollow}", ScrapeToUnfollow},
                {$"{ActivityType.SendMessageToFollower}", SendMessageToFollower},
                {$"{ActivityType.AutoReplyToNewMessage}", AutoReplyToNewMessageProcessor}
            };

            #endregion
        }
        private void VotePostByCustomUser(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<PostCustomUserProcessor>();
            processor.Start(queryInfo);
        }
        private void VotePostByCustomUrl(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<PostCustomUrlProcessor>();
            processor.Start(queryInfo);
        }

        private void VotePostByKeyword(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<PostKeywordProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessForSomeonesFollowers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<SomeonesFollowersProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessForSomeonesFollowings(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<SomeonesFollowingsProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessForFollowersOfFollowers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<FollowersOfFollowersProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessForFollowersOfFollowings(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<FollowersOfFollowingsProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartDownvoteQuestionProcessWithTopicFaqs(QueryInfo queryinfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<QuestionTopicProcessor>();
            processor.Start(queryinfo);
        }

        protected void StartReportUserProcessWithUpvoters(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<UserAnswerUpvotersProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessForEngagedUser(QueryInfo queryinfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<UserEngagedUserProcessor>();
            processor.Start(queryinfo);
        }

        #region  properties

        public Dictionary<string, Action<QueryInfo>> ScrapeWithQueriesActionTable { get; set; }
        public Dictionary<string, Action> ScrapeWithoutQueriesActionTable { get; set; }
        private readonly IUnityContainer _unityContainer;
        private readonly IQdJobProcess _jobProcess;

        #endregion

        #region Answer On Questions Process

        private void AnswerOnQuestionsByCustomUrl(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<AnswerOnQuestionCustomUrlProcessor>();
            processor.Start(queryInfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void AnswerOnQuestionsByKeyword(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<AnswerOnQuestionKeywordProcessor>();
            processor.Start(queryInfo);
        }

        private void AnswerOnQuestionsByCustomUser(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<AnswerOnQuestionCustomUserProcessor>();
            processor.Start(queryInfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void AnswerOnQuestionsByTopicFaqs(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<AnswerOnQuestionTopicFaqsProcessor>();
            processor.Start(queryInfo);
        }

        #endregion

        #region Process with KeyWords

        public void ScrapeQuestionByKeyword(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<QuestionKeywordProcessor>();
            processor.Start(queryInfo);
        }

        public void ScrapeAnswerByKeyword(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<AnswerKeywordProcessor>();
            processor.Start(queryInfo);
        }

        private void ScrapeKeywordWithJobProcess(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<UserKeywordProcessor>();
            processor.Start(queryInfo);
        }

        #endregion

        #region Process with TopicList

        private void StartProcessWithTopicList(QueryInfo queryinfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<AnswerTopicProcessor>();
            processor.Start(queryinfo);
        }

        protected void StartReportUserProcessWithTopic(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<UserTopicProcessor>();
            processor.Start(queryInfo);
        }

        #endregion

        #region Process with CustomUrl

        private void StartProcessAnswerWithCustomUrl(QueryInfo queryinfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<AnswerCustomUrlProcessor>();
            processor.Start(queryinfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void StartProcessQuestionsWithCustomUrl(QueryInfo queryinfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<QuestionCustomUrlProcessor>();
            processor.Start(queryinfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        #endregion

        #region Process with CustomUser

        protected void StartProcessForCustomUsers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<UserCustomUsersProcessor>();
            processor.Start(queryInfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void ScraperAnswerOfUser(QueryInfo queryinfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<AnswerCustomUserProcessor>();
            processor.Start(queryinfo);
        }

        private void ScrapeQuestionByUser(QueryInfo queryinfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<QuestionCustomUserProcessor>();
            processor.Start(queryinfo);
        }

        #endregion

        #region Non-Query Process

        protected void ScrapeToUnfollow()
        {
            var processor = _unityContainer.Resolve<UnfollowProcessor>();
            processor.Start(new QueryInfo());
            if (processor._unFollowModel.IsChkCustomUsersListChecked)
                _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        private void SendMessageToFollower()
        {
            IQueryProcessor processor = _unityContainer.Resolve<SendMessageToFollowerProcessor>();
            var queryInfo = new QueryInfo {QueryValue = "LangKeyNewFollowersToSendMessage".FromResourceDictionary()};
            processor.Start(queryInfo);
        }

        private void AutoReplyToNewMessageProcessor()
        {
            IQueryProcessor processor = _unityContainer.Resolve<AutoReplyToNewMessageProcessor>();
            var queryInfo = new QueryInfo
                {QueryValue = "LangKeyUsersToSendAutoReplyMessageAccordingToMessageFilter".FromResourceDictionary()};
            processor.Start(queryInfo);
        }

        #endregion
    }
}