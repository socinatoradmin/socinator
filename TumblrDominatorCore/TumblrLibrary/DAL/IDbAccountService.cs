using DominatorHouseCore.DatabaseHandler.TumblrTables.Account;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TumblrDominatorCore.TumblrLibrary.DAL
{
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

    public interface IDbAccountService
    {
        [Obsolete(
            "db operation class should registered as singleton and should be injected in all the class. Get rid of this property later")]
        DbOperations DbOperations { get; }

        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType);

        IReadOnlyCollection<InteractedUser> GetInteractedUsers(ActivityType activityType);
        IReadOnlyCollection<PrivateBlacklist> GetPrivateBlacklist();
        IReadOnlyCollection<PrivateWhitelist> GetPrivateWhitelist();

        List<Friendships> GetFriendships(params FollowType[] followTypes);

        IReadOnlyCollection<InteractedUser> GetSelectedUsers(string username);

        int GetPostfrmUniqueUser(string username, ActivityType activityType);

        void Add<T>(T t) where T : class, new();

        void Remove<T>(T t) where T : class, new();

        void RemoveAll<T>() where T : class, new();

        void Remove<T>(Expression<Func<T, bool>> expression) where T : class, new();
        void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new();
        void AddRange<T>(List<T> data) where T : class, new();
        Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> expression = null) where T : class, new();
        int Count<T>(Expression<Func<T, bool>> expression = null) where T : class, new();

        int GetInteractedtime(string username, ActivityType activityType);

        Friendships GetSpecificUser(string username);

        IReadOnlyCollection<UnFollowedUser> GetUnfollowedUsers(ActivityType activityType);
        IReadOnlyCollection<UnLikedPosts> GetUnLikedUsers(ActivityType activityType);
    }

    public class DbAccountService : DbAccountServiceBase, IDbAccountService
    {
        public DbAccountService(IEntityCountersManager entityCountersManager,
            DominatorAccountModel dominatorAccountModel) : base(entityCountersManager)
        {
            //should not use  dominatorAccountModel.AccountBaseModel.AccountNetwork 
            //because container not sending actual account only sending new object of DominatorAccountModel
            DbOperations = new DbOperations(dominatorAccountModel.AccountId, SocialNetworks.Tumblr,
                ConstantVariable.GetAccountDb);
        }

        [Obsolete(
            "db operation class should registered as singleton and should be injected in all the class. Get rid of this property later")]
        public DbOperations DbOperations { get; }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType)
        {
            var activity = activityType.ToString();
            return DbOperations.Get<InteractedPosts>(x => x.ActivityType == activity).ToList();
        }

        public IReadOnlyCollection<PrivateBlacklist> GetPrivateBlacklist()
        {
            return DbOperations.Get<PrivateBlacklist>()?.ToList() ?? new List<PrivateBlacklist>();
        }

        public IReadOnlyCollection<PrivateWhitelist> GetPrivateWhitelist()
        {
            return DbOperations.Get<PrivateWhitelist>()?.ToList() ?? new List<PrivateWhitelist>();
        }

        public IReadOnlyCollection<InteractedUser> GetInteractedUsers(ActivityType activityType)
        {
            var activity = activityType.ToString();
            return DbOperations.Get<InteractedUser>(x => x.ActivityType == activity)?.ToList() ?? new List<InteractedUser>();
        }

        public IReadOnlyCollection<UnFollowedUser> GetUnfollowedUsers(ActivityType activityType)
        {
            var activity = activityType.ToString();
            return DbOperations.Get<UnFollowedUser>(x => x.ActivityType == activity);
        }

        public IReadOnlyCollection<UnLikedPosts> GetUnLikedUsers(ActivityType activityType)
        {
            var activity = activityType.ToString();
            return DbOperations.Get<UnLikedPosts>(x => x.ActivityType == activity);
        }

        public int GetInteractedtime(string username, ActivityType activityType)
        {
            return DbOperations.GetSingle<InteractedUser>(y =>
                y.UserName == username && y.ActivityType == ActivityType.Follow.ToString()).InteractionTimeStamp;
        }

        public void Add<T>(T data) where T : class, new()
        {
            DbOperations.Add(data);
        }

        public List<Friendships> GetFriendships(params FollowType[] followTypes)
        {
            return DbOperations.Get<Friendships>(x => followTypes.Contains(x.FollowType)).ToList();
        }

        public void Remove<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            DbOperations.Remove(expression);
        }

        public IReadOnlyCollection<InteractedUser> GetSelectedUsers(string username)
        {
            return DbOperations.Get<InteractedUser>(x => x.UserName == username);
        }

        public int GetPostfrmUniqueUser(string post, ActivityType activityType)
        {
            var activity = activityType.ToString();
            return DbOperations.Get<InteractedPosts>(x => x.ActivityType == activity && x.InteractedUserName == post)
                .Count;
        }

        public Friendships GetSpecificUser(string username)
        {
            return DbOperations.GetSingle<Friendships>(x => x.Username == username);
        }

        public void Remove<T>(T t) where T : class, new()
        {
            DbOperations.Remove(t);
        }

        public void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            DbOperations.RemoveMatch(expression);
        }

        public int Count<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return DbOperations.Count(expression);
        }

        public void AddRange<T>(List<T> data) where T : class, new()
        {
            DbOperations.AddRange(data);
        }

        public void RemoveAll<T>() where T : class, new()
        {
            DbOperations.RemoveAll<T>();
        }


        public async Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return await DbOperations.GetAsync(expression);
        }
    }
}