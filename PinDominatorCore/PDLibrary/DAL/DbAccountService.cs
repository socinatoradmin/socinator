using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Utility;

namespace PinDominatorCore.PDLibrary.DAL
{
    public interface IDbAccountServiceScoped : IDbAccountService
    {
    }

    public class DbAccountServiceScoped : DbAccountService, IDbAccountServiceScoped
    {
        public DbAccountServiceScoped(IEntityCountersManager entityCountersManager,
            IProcessScopeModel processScopeModel) :
            base(processScopeModel.Account)
        {
        }
    }

    public interface IDbAccountService
    {
        [Obsolete(
            "db operation class should registered as singleton and should be injected in all the class. Get rid of this property later")]
        DbOperations DbOperations { get; }
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType);
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string activityType);

        IReadOnlyCollection<InteractedPosts> GetInteractedPostsWithSameQuery(ActivityType activityType,
            QueryInfo queryInfo);

        IReadOnlyCollection<ScrapPins> GetScrapPins(string activityType);
        IReadOnlyCollection<ScrapPins> GetScrapPinsWithSameQuery(string activityType, QueryInfo queryInfo);

        IReadOnlyCollection<InteractedPosts> GetInteractedPostsWithSameQueryAndSameBoards(ActivityType activityType,
            QueryInfo queryInfo, string boardUrl);

        IReadOnlyCollection<InteractedUsers> GetInteractedUsers(ActivityType activityType);

        IReadOnlyCollection<InteractedUsers> GetInteractedUsers(string activityType);
        IReadOnlyCollection<InteractedUsers> GetInteractedUsersForMessages();

        IReadOnlyCollection<InteractedUsers> GetInteractedUsersWithSameQuery(ActivityType activityType,
            QueryInfo queryInfo);

        IReadOnlyCollection<InteractedUsers> GetInteractedUsersWithSameQuery(string activityType, QueryInfo queryInfo);

        IReadOnlyCollection<InteractedBoards> GetInteractedBoards(ActivityType activityType);
        IReadOnlyCollection<ScrapBoards> GetScrapBoardsWithSameQuery(string activityType, QueryInfo queryInfo);

        IReadOnlyCollection<PrivateBlacklist> GetPrivateBlacklist();
        IReadOnlyCollection<PrivateWhitelist> GetPrivateWhitelist();
        IReadOnlyCollection<UnfollowedUsers> GetUnfollowedUsers();
        IReadOnlyCollection<Friendships> GetFriendships(params FollowType[] followTypes);
        IReadOnlyCollection<DailyStatitics> GetDailyStatitics();
        IReadOnlyCollection<OwnBoards> GetOwnBoards();

        void Add<T>(T data) where T : class, new();
        void AddRange<T>(List<T> data) where T : class, new();
        T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class, new();
        List<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new();
        Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> expression = null) where T : class, new();
        bool Update<T>(T t) where T : class, new();
        int Count<T>(Expression<Func<T, bool>> expression = null) where T : class, new();
        void Remove<T>(Expression<Func<T, bool>> expression) where T : class, new();
        void RemoveAll<T>() where T : class, new();
        void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new();

        List<Friendships> GetUnfollowingUsersCustomExpression(Expression<Func<Friendships, bool>> unfollowExpression);

        List<InteractedUsers> GetUnfollowingUsersCustomExpression(
            Expression<Func<InteractedUsers, bool>> unfollowExpression);
    }

    public class DbAccountService : IDbAccountService
    {
        public DbAccountService(DominatorAccountModel dominatorAccountModel)
        {
            DbOperations = new DbOperations(dominatorAccountModel.AccountId, SocialNetworks.Pinterest,
                ConstantVariable.GetAccountDb);
        }

        [Obsolete(
            "db operation class should registered as singleton and should be injected in all the class. Get rid of this property later")]
        public DbOperations DbOperations { get; }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType)
        {
            var actType = activityType.ToString();
            return DbOperations.Get<InteractedPosts>(x => x.OperationType == actType).ToList();
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string activityType)
        {
            return DbOperations.Get<InteractedPosts>(x => x.OperationType == activityType).ToList();
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPostsWithSameQuery(ActivityType activityType,
            QueryInfo queryInfo)
        {
            var actType = activityType.ToString();
            return DbOperations.Get<InteractedPosts>(x => x.OperationType == actType
                                                          && x.QueryType == queryInfo.QueryType
                                                          && x.Query == queryInfo.QueryValue).ToList();
        }

        public IReadOnlyCollection<ScrapPins> GetScrapPins(string activityType)
        {
            return DbOperations.Get<ScrapPins>(x => x.ActivityType == activityType).ToList();
        }

        public IReadOnlyCollection<ScrapPins> GetScrapPinsWithSameQuery(string activityType, QueryInfo queryInfo)
        {
            return DbOperations.Get<ScrapPins>(x => x.ActivityType == activityType
                                                    && x.QueryType == queryInfo.QueryType &&
                                                    x.QueryValue == queryInfo.QueryValue).ToList();
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPostsWithSameQueryAndSameBoards(
            ActivityType activityType,
            QueryInfo queryInfo, string board)
        {
            var actType = activityType.ToString();
            return DbOperations.Get<InteractedPosts>(x => x.OperationType == actType
                                                          && x.QueryType == queryInfo.QueryType
                                                          && x.Query == queryInfo.QueryValue
                                                          && x.DestinationBoard == board).ToList();
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUsers(ActivityType activityType)
        {
            var actType = activityType.ToString();
            return DbOperations.Get<InteractedUsers>(x => x.ActivityType == actType).ToList();
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUsers(string activityType)
        {
            return DbOperations.Get<InteractedUsers>(x => x.ActivityType == activityType).ToList();
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUsersForMessages()
        {
            var sendMessageToFollowerActivity = ActivityType.SendMessageToFollower.ToString();
            var autoReplyToNewMessageActivity = ActivityType.AutoReplyToNewMessage.ToString();
            var broadcastMessagesActivity = ActivityType.BroadcastMessages.ToString();
            return DbOperations.Get<InteractedUsers>(x => x.ActivityType == sendMessageToFollowerActivity
                                                          || x.ActivityType == autoReplyToNewMessageActivity ||
                                                          x.ActivityType == broadcastMessagesActivity).ToList();
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUsersWithSameQuery(ActivityType activityType,
            QueryInfo queryInfo)
        {
            var actType = activityType.ToString();
            return DbOperations.Get<InteractedUsers>(x => x.ActivityType == actType
                                                          && x.QueryType == queryInfo.QueryType &&
                                                          x.Query == queryInfo.QueryValue).ToList();
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUsersWithSameQuery(string activityType,
            QueryInfo queryInfo)
        {
            return DbOperations.Get<InteractedUsers>(x => x.ActivityType == activityType
                                                          && x.QueryType == queryInfo.QueryType &&
                                                          x.Query == queryInfo.QueryValue).ToList();
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUsersWithSameQueryType(ActivityType activityType,
            string queryType)
        {
            var actType = activityType.ToString();
            return DbOperations.Get<InteractedUsers>(x => x.ActivityType == actType
                                                          && x.QueryType == queryType).ToList();
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUsersWithInteractionTime(ActivityType activityType,
            int interactionTime)
        {
            var actType = activityType.ToString();
            return DbOperations.Get<InteractedUsers>(x => x.ActivityType == actType
                                                          && x.InteractionTime <= interactionTime).ToList();
        }

        public IReadOnlyCollection<InteractedBoards> GetInteractedBoards(ActivityType activityType)
        {
            return DbOperations.Get<InteractedBoards>(x => x.OperationType == activityType).ToList();
        }

        public IReadOnlyCollection<InteractedBoards> GetInteractedBoardsWithSameQuery(ActivityType activityType,
            QueryInfo queryInfo)
        {
            return DbOperations.Get<InteractedBoards>(x => x.OperationType == activityType
                                                           && x.QueryType == queryInfo.QueryType &&
                                                           x.Query == queryInfo.QueryValue).ToList();
        }

        public IReadOnlyCollection<ScrapBoards> GetScrapBoardsWithSameQuery(string activityType, QueryInfo queryInfo)
        {
            return DbOperations.Get<ScrapBoards>(x => x.OperationType == activityType
                                                      && x.QueryType == queryInfo.QueryType &&
                                                      x.Query == queryInfo.QueryValue).ToList();
        }

        public IReadOnlyCollection<PrivateBlacklist> GetPrivateBlacklist()
        {
            return DbOperations.Get<PrivateBlacklist>().ToList();
        }

        public IReadOnlyCollection<PrivateWhitelist> GetPrivateWhitelist()
        {
            return DbOperations.Get<PrivateWhitelist>().ToList();
        }

        public IReadOnlyCollection<UnfollowedUsers> GetUnfollowedUsers()
        {
            return DbOperations.Get<UnfollowedUsers>();
        }

        public IReadOnlyCollection<Friendships> GetFriendships(params FollowType[] followTypes)
        {
            return DbOperations.Get<Friendships>(x => followTypes.Contains(x.FollowType)).ToList();
        }

        public IReadOnlyCollection<OwnBoards> GetOwnBoards()
        {
            return DbOperations.Get<OwnBoards>().ToList();
        }

        public bool IsActivityDoneWithThisUserId(string userId, ActivityType activityType)
        {
            var actType = activityType.ToString();
            return DbOperations.Any<InteractedUsers>(x =>
                x.InteractedUserId == userId && x.ActivityType == actType);
        }

        public void Add<T>(T data) where T : class, new()
        {
            DbOperations.Add(data);
        }

        public void AddRange<T>(List<T> data) where T : class, new()
        {
            DbOperations.AddRange(data);
        }

        public T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return DbOperations.GetSingle(expression);
        }

        public bool Update<T>(T t) where T : class, new()
        {
            return DbOperations.Update(t);
        }

        public IReadOnlyCollection<DailyStatitics> GetDailyStatitics()
        {
            return DbOperations.Get<DailyStatitics>();
        }

        public List<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return DbOperations.Get(expression);
        }

        public int Count<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return DbOperations.Count(expression);
        }

        public async Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return await DbOperations.GetAsync(expression);
        }

        public void Remove<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            DbOperations.Remove(expression);
        }

        public void RemoveAll<T>() where T : class, new()
        {
            DbOperations.RemoveAll<T>();
        }

        public void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            DbOperations.RemoveMatch(expression);
        }

        public List<Friendships> GetUnfollowingUsersCustomExpression(
            Expression<Func<Friendships, bool>> unfollowExpression)
        {
            var data = new List<Friendships>();
            try
            {
                DbOperations.Get(unfollowExpression).ForEach(y =>
                {
                    if (!data.Any(x => x.Username.Contains(y.Username)))
                        data.Add(y);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return data;
        }

        public List<InteractedUsers> GetUnfollowingUsersCustomExpression(
            Expression<Func<InteractedUsers, bool>> unfollowExpression)
        {
            var data = new List<InteractedUsers>();
            try
            {
                DbOperations.Get(unfollowExpression).ForEach(y =>
                {
                    if (!data.Any(x => x.InteractedUsername.Contains(y.InteractedUsername)))
                        data.Add(y);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return data;
        }
    }
}