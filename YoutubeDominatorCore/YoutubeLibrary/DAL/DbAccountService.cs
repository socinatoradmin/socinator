using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.DatabaseHandler.YdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace YoutubeDominatorCore.YoutubeLibrary.DAL
{
    public interface IDbAccountService
    {
        [Obsolete(
            "db operation class should registered as singleton and should be injected in all the class. Get rid of this property later")]
        DbOperations DbOperations { get; }

        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType);

        IReadOnlyCollection<InteractedPosts> GetInteractedPostsWithSameQuery(ActivityType activityType,
            string myChannelId, string postUrl);

        IReadOnlyCollection<InteractedChannels> GetInteractedChannels(ActivityType activityType, string myChannelId);

        IReadOnlyCollection<InteractedChannels> GetSpecificInteractedChannels(ActivityType activityType,
            string myChannelId, string interactedChannelId, string interactedChannelUsername);

        IReadOnlyCollection<PrivateBlacklist> GetPrivateBlacklist();
        IReadOnlyCollection<PrivateWhitelist> GetPrivateWhitelist();
        IReadOnlyCollection<InteractedChannels> GetUnsubscribedChannels(string myChannelId);
        T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class, new();
        bool Update<T>(T t) where T : class, new();
        void Add<T>(T data) where T : class, new();
        List<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new();
        Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> expression = null) where T : class, new();
        int Count<T>(Expression<Func<T, bool>> expression = null) where T : class, new();
        void Remove<T>(Expression<Func<T, bool>> expression) where T : class, new();
        void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new();
        void RemoveAll<T>() where T : class, new();
        bool AddMultiple<T>(List<T> listData) where T : class, new();

        IReadOnlyCollection<InteractedPosts> GetInteractedPost(ActivityType activityType, string myUserName,
            string postUrl);

        IReadOnlyCollection<InteractedPosts> GetInteractedPost(ActivityType activityType, string myUserName,
            string postUrl, string commentText);
    }

    public interface IDbAccountServiceScoped : IDbAccountService
    {
    }

    public class DbAccountServiceScoped : DbAccountService, IDbAccountServiceScoped
    {
        public DbAccountServiceScoped(IEntityCountersManager entityCountersManager,
            IProcessScopeModel processScopeModel)
            : base(entityCountersManager, processScopeModel.Account)
        {
        }
    }

    public class DbAccountService : DbAccountServiceBase, IDbAccountService
    {
        //public DbAccountService(DominatorAccountModel dominatorAccountModel)
        //{
        //    _dbOperations = new DbOperations(dominatorAccountModel.AccountId, SocialNetworks.YouTube,
        //        ConstantVariable.GetAccountDb);
        //}

        public DbAccountService(IEntityCountersManager entityCountersManager,
            DominatorAccountModel dominatorAccountModel)
            : base(entityCountersManager)
        {
            DbOperations = new DbOperations(dominatorAccountModel.AccountId, SocialNetworks.YouTube,
                ConstantVariable.GetAccountDb);
        }

        [Obsolete(
            "db operation class should registered as singleton and should be injected in all the class. Get rid of this property later")]
        public DbOperations DbOperations { get; }

        //public IReadOnlyCollection<FeedInfoes> GetFeedInfos(int startValue, int endValue, int isComment = 0, int isRetweet = 0)
        //{
        //    return _dbOperations.Get<FeedInfoes>(x =>
        //        x.IsComment == isComment && x.IsRetweet == isRetweet && startValue <= x.TweetedTimeStamp &&
        //        x.TweetedTimeStamp <= endValue).ToList();
        //}

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType)
        {
            var activity = activityType.ToString();
            return DbOperations.Get<InteractedPosts>(x => x.ActivityType == activity).ToList();
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPostsWithSameQuery(ActivityType activityType,
            string myChannelId, string postUrl)
        {
            var activity = activityType.ToString();
            return DbOperations.Get<InteractedPosts>(x => x.ActivityType == activity
                                                           && x.VideoUrl == postUrl)
                .ToList();
        }

        public IReadOnlyCollection<InteractedChannels> GetInteractedChannels(ActivityType activityType,
            string myChannelId)
        {
            var activity = activityType.ToString();
            return DbOperations.Get<InteractedChannels>(x => x.ActivityType == activity && x.MyChannelId == myChannelId)
                .ToList();
        }

        public IReadOnlyCollection<InteractedChannels> GetSpecificInteractedChannels(ActivityType activityType,
            string myChannelId, string interactedChannelId, string interactedChannelUsername)
        {
            var activity = activityType.ToString();
            if (!string.IsNullOrEmpty(interactedChannelId) && !string.IsNullOrEmpty(interactedChannelUsername))
                return DbOperations.Get<InteractedChannels>(x =>
                    x.ActivityType == activity && x.MyChannelId == myChannelId &&
                    (x.InteractedChannelId == interactedChannelId ||
                     x.InteractedChannelUsername == interactedChannelUsername)).ToList();
            if (!string.IsNullOrEmpty(interactedChannelId))
                return DbOperations.Get<InteractedChannels>(x =>
                    x.ActivityType == activity && x.MyChannelId == myChannelId &&
                    x.InteractedChannelId == interactedChannelId).ToList();
            return DbOperations.Get<InteractedChannels>(x =>
                x.ActivityType == activity && x.MyChannelId == myChannelId &&
                x.InteractedChannelUsername == interactedChannelUsername).ToList();
        }


        public IReadOnlyCollection<PrivateBlacklist> GetPrivateBlacklist()
        {
            return DbOperations.Get<PrivateBlacklist>().ToList();
        }

        public IReadOnlyCollection<PrivateWhitelist> GetPrivateWhitelist()
        {
            return DbOperations.Get<PrivateWhitelist>().ToList();
        }

        public IReadOnlyCollection<InteractedChannels> GetUnsubscribedChannels(string myChannelId)
        {
            var activity = ActivityType.UnSubscribe.ToString();
            return DbOperations.Get<InteractedChannels>(x =>
                (x.ActivityType == activity || x.ActivityType == "Unsubscribe") && x.MyChannelId == myChannelId);
        }

        public void Add<T>(T data) where T : class, new()
        {
            DbOperations.Add(data);
        }

        public List<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return DbOperations.Get(expression);
        }

        public async Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return await DbOperations.GetAsync(expression);
        }

        public void Remove<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            DbOperations.Remove(expression);
        }

        public int Count<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return DbOperations.Count(expression);
        }

        public T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return DbOperations.GetSingle(expression);
        }

        public bool Update<T>(T t) where T : class, new()
        {
            return DbOperations.Update(t);
        }

        public void RemoveAll<T>() where T : class, new()
        {
            DbOperations.RemoveAll<T>();
        }

        public void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            DbOperations.RemoveMatch(expression);
        }

        public bool AddMultiple<T>(List<T> listData) where T : class, new()
        {
            return DbOperations.AddRange(listData);
        }


        public IReadOnlyCollection<InteractedPosts> GetInteractedPost(ActivityType activityType, string myUserName,
            string postUrl)
        {
            var activity = activityType.ToString();

            return DbOperations.Get<InteractedPosts>(x => x.ActivityType == activity
                                                          && x.AccountUsername == myUserName && x.VideoUrl == postUrl)
                .ToList();
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPost(ActivityType activityType, string myUserName,
            string postUrl, string commentText)
        {
            var activity = activityType.ToString();
            return DbOperations.Get<InteractedPosts>(x => x.ActivityType == activity
                                                          && x.AccountUsername == myUserName && x.VideoUrl == postUrl &&
                                                          x.MyCommentedText == commentText).ToList();
        }
    }
}