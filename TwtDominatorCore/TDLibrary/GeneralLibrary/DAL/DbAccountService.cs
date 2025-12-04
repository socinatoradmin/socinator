using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Utility;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.DAL
{
    /// <summary>
    ///     Here we use interface to decoupled the classes
    /// </summary>
    public interface IDbAccountService
    {
        IReadOnlyCollection<FeedInfoes>
            GetFeedInfos(int startValue, int endValue, int isComment = 0);

        IReadOnlyCollection<FeedInfoes>
            GetFeedInfosRetweet(int startValue, int endValue, int isRetweet = 0);

        IReadOnlyCollection<FeedInfoes> GetAllFeedInfos();
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string user, ActivityType activityType);
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType);
        IReadOnlyCollection<InteractedUsers> GetInteractedUsers(ActivityType activityType);
        IReadOnlyCollection<InteractedUsers> GetInteractedUsers(params string[] activityTypes);
        IReadOnlyCollection<UnfollowedUsers> GetUnfollowedUsers();
        IReadOnlyCollection<InteractedUsers> GetInteractedUserFriends();
        IReadOnlyCollection<PrivateWhitelist> GetPrivateWhitelistUsers();
        IReadOnlyCollection<PrivateBlacklist> GetPrivateBlacklistUsers();
        IReadOnlyCollection<Friendships> GetFriendships(params FollowType[] followTypes);
        IReadOnlyCollection<Friendships> GetAllFriendships();
        Friendships GetAUserFromFriendships(string userId);
        int Count<T>(Expression<Func<T, bool>> expression = null) where T : class, new();

        bool IsAlreadyCommentedOnTweetwithSameQuery(string qeuryType, string queryValue, string tweetId,
            ActivityType activityType);

        bool IsActivtyDoneWithThisTweetId(string tweetId, ActivityType activityType);

        bool IsActivityDoneWithThisUserId(string userId, ActivityType activityType);

        #region CRUD operation

        void Add<T>(T data) where T : class, new();
        void AddRange<T>(List<T> data) where T : class, new();
        void Update<T>(T data) where T : class, new();
        T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class, new();

        void Remove<T>(T t) where T : class, new();
        void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new();
        void RemoveAll<T>() where T : class, new();

        List<T> GetList<T>(Expression<Func<T, bool>> expression = null) where T : class, new();

        #endregion
    }

    public interface IDbAccountServiceScoped : IDbAccountService
    {
    }

    public class DbAccountServiceScoped : DbAccountService, IDbAccountServiceScoped
    {
        public DbAccountServiceScoped(IEntityCountersManager entityCountersManager,
            IProcessScopeModel processScopeModel) : base(entityCountersManager, processScopeModel.Account)
        {
        }
    }

    /// <summary>
    ///     Data access layer implement the interface
    ///     contructor injection
    /// </summary>
    public class DbAccountService : DbAccountServiceBase, IDbAccountService
    {
        private readonly DbOperations _dbOperations;

        public DbAccountService(IEntityCountersManager entityCountersManager,
            DominatorAccountModel dominatorAccountModel) : base(entityCountersManager)
        {
            _dbOperations = new DbOperations(dominatorAccountModel.AccountId, SocialNetworks.Twitter,
                ConstantVariable.GetAccountDb);
        }
        public DbAccountService(DominatorAccountModel dominatorAccountModel) : base(null)
        {
            _dbOperations = new DbOperations(dominatorAccountModel.AccountId, SocialNetworks.Twitter,
                ConstantVariable.GetAccountDb);
        }
        public IReadOnlyCollection<FeedInfoes> GetFeedInfos(int startValue, int endValue, int isComment = 0)
        {
            return _dbOperations.Get<FeedInfoes>(x => x.IsComment == isComment &&
                                                      x.TweetedTimeStamp >= startValue &&
                                                      x.TweetedTimeStamp <= endValue).ToList();
        }

        public IReadOnlyCollection<FeedInfoes> GetFeedInfosRetweet(int startValue, int endValue,
            int isRetweet = 0)
        {
            return _dbOperations.Get<FeedInfoes>(x =>
                x.IsRetweet == isRetweet && startValue >= x.TweetedTimeStamp &&
                x.TweetedTimeStamp <= endValue).ToList();
        }

        public IReadOnlyCollection<FeedInfoes> GetAllFeedInfos()
        {
            return _dbOperations.Get<FeedInfoes>();
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Get<InteractedPosts>(x => x.ActivityType == stractivityType).ToList();
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUsers(ActivityType activityType)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Get<InteractedUsers>(x => x.ActivityType == stractivityType).ToList();
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string user, ActivityType activityType)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Get<InteractedPosts>(
                x => (x.SinAccUsername == user || x.Username == user) && x.ActivityType == stractivityType);
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUserFriends()
        {
            //Follow
            var strFollow = ActivityType.Follow.ToString();
            var strFollowBack = ActivityType.FollowBack.ToString();

            return _dbOperations.Get<InteractedUsers>(x => (x.ActivityType == strFollow ||
                                                            x.ActivityType == strFollowBack) && x.FollowStatus == 1)
                .ToList();
        }

        public IReadOnlyCollection<UnfollowedUsers> GetUnfollowedUsers()
        {
            return _dbOperations.Get<UnfollowedUsers>().ToList();
        }


        public IReadOnlyCollection<Friendships> GetFriendships(params FollowType[] followTypes)
        {
            // added DetailedInfoWillNotBeRetrieved(Default value 0) as a condition (DetailedInfoWillNotBeRetrieved has value 1 if user got Suspended or not found)
            return _dbOperations
                .Get<Friendships>(x => followTypes.Contains(x.FollowType) && x.DetailedInfoWillNotBeRetrieved == 0)
                .ToList();
        }


        public IReadOnlyCollection<Friendships> GetAllFriendships()
        {
            return _dbOperations.Get<Friendships>().ToList();
        }

        public Friendships GetAUserFromFriendships(string userId)
        {
            return _dbOperations.GetSingle<Friendships>(x => x.UserId == userId);
        }


        public bool IsAlreadyCommentedOnTweetwithSameQuery(string queryType, string queryValue, string tweetId,
            ActivityType activityType)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Any<InteractedPosts>(x =>
                x.TweetId == tweetId && x.ActivityType == stractivityType && x.QueryType == queryType &&
                x.QueryValue == queryValue);
        }

        public bool IsActivtyDoneWithThisTweetId(string tweetId, ActivityType activityType)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Any<InteractedPosts>(x =>
                x.TweetId == tweetId && x.ActivityType == stractivityType);
        }

        public bool IsActivityDoneWithThisUserId(string userId, ActivityType activityType)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Any<InteractedUsers>(x =>
                x.InteractedUserId == userId && x.ActivityType == stractivityType);
        }

        public IReadOnlyCollection<PrivateWhitelist> GetPrivateWhitelistUsers()
        {
            return _dbOperations.Get<PrivateWhitelist>();
        }

        public IReadOnlyCollection<PrivateBlacklist> GetPrivateBlacklistUsers()
        {
            return _dbOperations.Get<PrivateBlacklist>();
        }

        #region CRUD operation

        public void Add<T>(T data) where T : class, new()
        {
            //CountInteracted<T>(_dbOperations.AccountId, _dbOperations.SocialNetworks, data);
            _dbOperations.Add(data);
        }

        public void Update<T>(T data) where T : class, new()
        {
            _dbOperations.Update(data);
        }

        public T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return _dbOperations.GetSingle(expression);
        }

        public void Remove<T>(T t) where T : class, new()
        {
            _dbOperations.Remove(t);
        }

        public void RemoveAll<T>() where T : class, new()
        {
            _dbOperations.RemoveAll<T>();
        }

        public int Count<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return _dbOperations.Count(expression);
        }


        public void AddRange<T>(List<T> data) where T : class, new()
        {
            // CountInteracted<T>(_dbOperations.AccountId, _dbOperations.SocialNetworks, data.ToArray());
            _dbOperations.AddRange(data);
        }

        public void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            _dbOperations.RemoveMatch(expression);
        }
        //public void RemoveAll<T>() where T : class, new()
        //{
        //    _dbOperations.RemoveAll<T>();
        //}

        public List<T> GetList<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return _dbOperations.Get(expression);
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUsers(params string[] activityTypes)
        {
            return _dbOperations.Get<InteractedUsers>(x => activityTypes.Contains(x.ActivityType)).ToList();
        }

        #endregion
    }

    // we can create Business Logic Layer to inject dependency from constructor
}