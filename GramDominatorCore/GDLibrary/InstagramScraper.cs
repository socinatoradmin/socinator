using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Threading;
using GramDominatorCore.GDModel;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Enums.GdQuery;
using GramDominatorCore.GDLibrary.Processor;
using GramDominatorCore.GDLibrary.Processor.Post;
using GramDominatorCore.GDLibrary.Processor.User;
using Unity;

namespace GramDominatorCore.GDLibrary
{
    public interface IInstagramScraperActionTables: IScraperActionTables
    {

    }
    public class InstagramQueryScraper : QueryScraper
    {
        public InstagramQueryScraper(IGdJobProcess jobprocess, Dictionary<string, Action<QueryInfo>> scrapeWithQueriesActionTable, Dictionary<string, Action> scrapeWithoutQueriesActionTable)
            : base(jobprocess, scrapeWithQueriesActionTable, scrapeWithoutQueriesActionTable)
        {

        }

    }

    public class InstagramScraperActionTables : IInstagramScraperActionTables
    {
        #region Global properties

        private ActivityType ActivityType { get;  }

        public Dictionary<string, Action<QueryInfo>> ScrapeWithQueriesActionTable { get; set; }

        public Dictionary<string, Action> ScrapeWithoutQueriesActionTable { get; set; }

        private  IUnityContainer _unityContainer;
        private readonly IGdJobProcess _jobProcess;
        #endregion

        public InstagramScraperActionTables(IGdJobProcess jobProcess,IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
            _jobProcess = jobProcess;
            ActivityType = _jobProcess.ActivityType;           

            ScrapeWithQueriesActionTable = new Dictionary<string, Action<QueryInfo>>()
            {
                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdPostQuery.SpecificUsersPosts)}", StartProcessForSpecificUsersPosts},

                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdPostQuery.HashtagPost)}", StartProcessWithHashtagPost},
                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdUserQuery.HashtagUsers)}", StartProcessHashtagUsers},
                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdPostQuery.HashtagUsersPost)}", StartProcessWithHashtagPostUser},

                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdUserQuery.Keywords)}", StartProcessWithKeyword},

                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdUserQuery.SomeonesFollowers)}", StartProcessForSomeonesFollowers},
                //{$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdPostQuery.SomeonesFollowersPost)}", StartProcessForSomeonesFollowersPost},

                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdUserQuery.SomeonesFollowings)}", StartProcessForSomeonesFollowings},
                //{$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdPostQuery.SomeonesFollowingsPost)}", StartProcessForSomeonesFollowingsPost},

                //{$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdUserQuery.FollowersOfFollowers)}", StartProcessForFollowersOfFollowers},
                //{$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdPostQuery.FollowersOfFollowersPost)}", StartProcessForFollowersOfFollowersPost},

                //{$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdUserQuery.FollowersOfFollowings)}", StartProcessForFollowersOfFollowings},
                //{$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdPostQuery.FollowersOfFollowingsPost)}", StartProcessForFollowersOfFollowingsPost},

                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdUserQuery.LocationUsers)}", StartProcessWithLocationUsers},
                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdPostQuery.LocationPosts)}", StartProcessWithPostLocation},
                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdPostQuery.LocationUsersPost)}", StartProcessWithUserPostLocation},

                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdUserQuery.CustomUsers)}", StartProcessForCustomUsers},
                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdPostQuery.CustomPhotos)}", StartProcessForCustomPhotos},
                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdPostQuery.SocinatorPublisherCampaign)}", StartProcessForPublisherCampaignPosts},

                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdUserQuery.SuggestedUsers)}", StartProcessForSuggestedusers},
                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdPostQuery.SuggestedUsersPosts)}", StartProcessForSuggestedUsersPosts},

                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdUserQuery.UsersWhoLikedPost)}", StartProcessWithUsersWhoLikedPost},
                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdPostQuery.PostOfUsersWhoLikedPost)}", StartProcessWithPostOfUsersWhoLikedPost},


                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdUserQuery.UsersWhoCommentedOnPost)}", StartProcessForMediaCommenters},
                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdPostQuery.PostOfUsersWhoCommentedOnPost)}", StartProcessForPostOfUsersWhoCommentedOnPost},

                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdUserQuery.ScrapUserWhoMessagedUs)}",StartProcessForWhoMessagedUs },
                { $"{ActivityType}{EnumUtility.GetQueryFromEnum(GdPostQuery.ScrapAllLikedPost)}",StartProcessToGetAllLikedPost},

                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdUserQuery.OwnFollowers)}",StartProcessOwnFollower },
                {$"{ActivityType}{EnumUtility.GetQueryFromEnum(GdUserQuery.OwnFollowings)}",StartProcessOwnFollowing },
                //{ $"{ActivityType}{EnumUtility.GetQueryFromEnum(GdPostQuery.TaggedPost)}",StartProcessTaggedPost},
            };

            ScrapeWithoutQueriesActionTable = new Dictionary<string, Action>()
            {
                {$"{ActivityType.Unfollow}", ScrapeToUnfollow},
                {$"{ActivityType.CloseFriend}", StartMakeCloseFriendProcess},
                {$"{ActivityType.FollowBack}", StartFollowBackProcess },
                {$"{ActivityType.BlockFollower}", StartBlockFollowerProcess},
                {$"{ActivityType.HashtagsScraper}", StartProcessWithHashtagScraper},
                {$"{ActivityType.DeletePost}", StartDeletingPost},
                {$"{ActivityType.AutoReplyToNewMessage}", StartAutoReplyToNewMessageProcess },
                {$"{ActivityType.SendMessageToFollower}", StartSendMessageToNewFollowersProcess },
                {$"{ActivityType.Unlike}", StartUnlikingPost }
            };
        }

        protected void StartProcessWithKeyword(QueryInfo queryInfo)
        {
           
            IQueryProcessor processor = _unityContainer.Resolve<KeywordProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessForSomeonesFollowers(QueryInfo queryInfo)
        {           
            IQueryProcessor processor = _unityContainer.Resolve<SomeonesFollowersProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessForSomeonesFollowersPost(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<SomeonesFollowersPostProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessForSomeonesFollowings(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<SomeonesFollowingsProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessForSomeonesFollowingsPost(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<SomeonesFollowingsPostProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessForSuggestedusers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<SuggestedUsersProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessForSuggestedUsersPosts(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<SuggestedUsersPostsProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessWithUsersWhoLikedPost(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<UsersWhoLikedPostProcessor>();
            processor.Start(queryInfo);       
        }
        protected void StartProcessWithPostOfUsersWhoLikedPost(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<PostOfUsersWhoLikedPostProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessToGetAllLikedPost(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<ScrapAllLikedPostProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessOwnFollower(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<OwnFollowersProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessOwnFollowing(QueryInfo qureryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<OwnFollowingsProcessor>();
            processor.Start(qureryInfo);
        }
        protected void StartProcessForMediaCommenters(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<UsersWhoCommentedOnPostProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessForPostOfUsersWhoCommentedOnPost(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<PostOfUsersWhoCommentedOnPostProcessor>();
            processor.Start(queryInfo);
        }
        protected void StartProcessForSpecificUsersPosts(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<SpecificUsersPostsProcessor>();
            processor.Start(queryInfo);
        }
        protected void StartProcessForFollowersOfFollowers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<FollowersOfFollowersProcessor>();
            processor.Start(queryInfo);
        }
        protected void StartProcessForFollowersOfFollowersPost(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<FollowersOfFollowersPostProcessor>();
            processor.Start(queryInfo);
        }
        protected void StartProcessForFollowersOfFollowings(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<FollowersOfFollowingsProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessForFollowersOfFollowingsPost(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<FollowersOfFollowingsPostProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessForWhoMessagedUs(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<ScrapUserWhoMessagedUsProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessForCustomUsers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomUsersProcessors>();
            processor.Start(queryInfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }
        protected void StartProcessForCustomPhotos(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<CustomPostProcessor>();
            processor.Start(queryInfo);
            _jobProcess.DominatorAccountModel.IsNeedToSchedule = false;
        }

        protected void StartProcessForPublisherCampaignPosts(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<PublisherCampaignPostProcessor>();
            processor.Start(queryInfo);
        }

        protected void StartProcessWithHashtagPost(QueryInfo queryInfo)
       {
            IQueryProcessor processor = _unityContainer.Resolve<HashTagPostProcessor>();
            processor.Start(queryInfo);
        }
        protected void StartProcessWithHashtagPostUser(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<HashtagUserPostProcessors>();
            processor.Start(queryInfo);
        }
        protected void StartProcessHashtagUsers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<HashTagUsersProcessors>();
            processor.Start(queryInfo);
        }

        protected void StartProcessWithPostLocation(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<LocationPostProcessor>();
            processor.Start(queryInfo);
        }
        protected void StartProcessWithUserPostLocation(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<LocationUserPostProcessor>();
            processor.Start(queryInfo);
        }
        protected void StartProcessWithLocationUsers(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<LocationUsersProcessor>();
            processor.Start(queryInfo);
        }
        protected void ScrapeToUnfollow()
        {
            IQueryProcessor processor = _unityContainer.Resolve<UnfollowProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }
        protected void StartMakeCloseFriendProcess()
        {
            IQueryProcessor processor = _unityContainer.Resolve<MakeCloseFriendProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }
        protected void StartProcessWithHashtagScraper()
        {
            IQueryProcessor processor = _unityContainer.Resolve<HashtagScraperProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }
        protected void StartDeletingPost()
        {
            IQueryProcessor processor = _unityContainer.Resolve<DeletePostProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }
        private void StartUnlikingPost()
        {
            IQueryProcessor processor = _unityContainer.Resolve<UnlikingPostProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }
        protected void StartFollowBackProcess()
        {
            IQueryProcessor processor = _unityContainer.Resolve<FollowBackProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }

        protected void StartBlockFollowerProcess()
        {
            IQueryProcessor processor = _unityContainer.Resolve<BlockFollowerProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }
        protected void StartAutoReplyToNewMessageProcess()
        {       
            IQueryProcessor processor = _unityContainer.Resolve<AutoReplyToNewMessageProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }
        private void StartSendMessageToNewFollowersProcess()
        {
            IQueryProcessor processor = _unityContainer.Resolve<SendMessageToNewFollowersProcessor>();
            processor.Start(QueryInfo.NoQuery);
        }
        private void StartProcessTaggedPost(QueryInfo queryInfo)
        {
            IQueryProcessor processor = _unityContainer.Resolve<TaggedPostProcessor>();
            processor.Start(queryInfo);
        }
    }

}
