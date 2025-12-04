using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RedditDominatorCore.RDLibrary.DAL
{
    public interface IDbAccountService
    {
        IReadOnlyCollection<InteractedSubreddit> GetInteractedSubreddit(ActivityType activityType);
        IReadOnlyCollection<InteractedPost> GetInteractedPosts(ActivityType activityType);
        IReadOnlyCollection<InteractedAutoActivityPost> GetInteractedAutoActivityPosts(ActivityType activityType);
        IReadOnlyCollection<InteractedUsers> GetInteractedUsers(ActivityType activityType);
        IReadOnlyCollection<UnfollowedUsers> GetUnfollowedUsers();
        IReadOnlyCollection<InteractedUsers> GetInteractedUserName(ActivityType activityType, string userName);
        IReadOnlyCollection<InteractedSubreddit> GetInteractedSubredditUrl(ActivityType activityType, string url);
        IReadOnlyCollection<InteractedPost> GetInteractedPostPermLink(ActivityType activityType, string permaLink);
        IReadOnlyCollection<InteractedSubreddit> GetInteractedSubreddits();
        IReadOnlyCollection<InteractedUsers> GetInteractedFollowers();

        IReadOnlyCollection<InteractedSubreddit> GetInteractedSubReddits();

        // IReadOnlyCollection<InteractedUsers> GetInteractedUserFriends();
        // IReadOnlyCollection<Friendships> GetFriendships(params FollowType[] followTypes);
        IReadOnlyCollection<InteractedUsers> GetInteractedFollower();
        IReadOnlyCollection<PrivateBlacklist> GetPrivateBlacklist();
        IReadOnlyCollection<PrivateWhitelist> GetPrivateWhitelist();
        void Add<T>(T data) where T : class, new();
        void Remove<T>(Expression<Func<T, bool>> expression) where T : class, new();
        void RemoveAll<T>() where T : class, new();
        void AddRange<T>(List<T> data) where T : class, new();
        void AddNormal<T>(T data) where T : class, new();
        void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new();

        IReadOnlyCollection<InteractedPost> GetInteractedPostPermLinkforComment(ActivityType activityType,
            string permaLink, string comment);
    }

    public interface IDbAccountServiceScoped : IDbAccountService
    {
    }

    public class DbAccountServiceScoped : DbAccountService, IDbAccountServiceScoped
    {
        public DbAccountServiceScoped(IEntityCountersManager entityCountersManager,
            IProcessScopeModel processScopeModel) :
            base(entityCountersManager, processScopeModel.Account)
        {
        }
    }

    public class DbAccountService : DbAccountServiceBase, IDbAccountService
    {
        private readonly DbOperations _dbOperations;

        public DbAccountService(IEntityCountersManager entityCountersManager,
            DominatorAccountModel dominatorAccountModel)
            : base(entityCountersManager)
        {
            _dbOperations = new DbOperations(dominatorAccountModel.AccountId, SocialNetworks.Reddit,
                ConstantVariable.GetAccountDb);
        }

        public IReadOnlyCollection<InteractedSubreddit> GetInteractedSubreddit(ActivityType activityType)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Get<InteractedSubreddit>(x => x.ActivityType == stractivityType).ToList();
        }

        public IReadOnlyCollection<InteractedPost> GetInteractedPosts(ActivityType activityType)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Get<InteractedPost>(x => x.ActivityType == stractivityType).ToList();
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUsers(ActivityType activityType)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Get<InteractedUsers>(x => x.ActivityType == stractivityType).ToList();
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedFollowers()
        {
            var strFollow = ActivityType.Follow.ToString();
            var strFollowBack = ActivityType.FollowBack.ToString();
            return _dbOperations.Get<InteractedUsers>(x => (x.ActivityType == strFollow ||
                                                            x.ActivityType == strFollowBack) && x.IsFollowing).ToList();
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUserName(ActivityType activityType, string userName)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Get<InteractedUsers>(x => x.ActivityType == stractivityType
                                                           && x.InteractedUsername == userName).ToList();
        }

        public IReadOnlyCollection<InteractedSubreddit> GetInteractedSubredditUrl(ActivityType activityType, string url)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Get<InteractedSubreddit>(x => x.ActivityType == stractivityType
                                                               && x.url == url).ToList();
        }

        public IReadOnlyCollection<InteractedPost> GetInteractedPostPermLink(ActivityType activityType,
            string permaLink)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Get<InteractedPost>(x => x.ActivityType == stractivityType
                                                          && x.Permalink == permaLink).ToList();
        }

        public IReadOnlyCollection<UnfollowedUsers> GetUnfollowedUsers()
        {
            return _dbOperations.Get<UnfollowedUsers>();
        }

        public IReadOnlyCollection<InteractedSubreddit> GetInteractedSubreddits()
        {
            var stractivityType = ActivityType.Subscribe.ToString();
            return _dbOperations.Get<InteractedSubreddit>(x => x.ActivityType == stractivityType);
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedFollower()
        {
            var stractivityType = ActivityType.Follow.ToString();
            return _dbOperations.Get<InteractedUsers>(x => x.ActivityType == stractivityType);
        }

        public IReadOnlyCollection<InteractedSubreddit> GetInteractedSubReddits()
        {
            var stractivityType = ActivityType.Follow.ToString();
            return _dbOperations.Get<InteractedSubreddit>(x => x.ActivityType == stractivityType);
        }

        public IReadOnlyCollection<PrivateBlacklist> GetPrivateBlacklist()
        {
            return _dbOperations.Get<PrivateBlacklist>().ToList();
        }

        public IReadOnlyCollection<PrivateWhitelist> GetPrivateWhitelist()
        {
            return _dbOperations.Get<PrivateWhitelist>().ToList();
        }

        public void Add<T>(T data) where T : class, new()
        {
            CountInteracted(_dbOperations.AccountId, _dbOperations.SocialNetworks, data);
            _dbOperations.Add(data);
        }

        public void AddNormal<T>(T data) where T : class, new()
        {
            _dbOperations.Add(data);
        }

        public void AddRange<T>(List<T> data) where T : class, new()
        {
            // CountInteracted<T>(_dbOperations.AccountId, _dbOperations.SocialNetworks, data.ToArray());
            _dbOperations.AddRange(data);
        }

        public void Remove<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            _dbOperations.Remove(expression);
        }

        public void RemoveAll<T>() where T : class, new()
        {
            _dbOperations.RemoveAll<T>();
        }

        public void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            _dbOperations.RemoveMatch(expression);
        }

        public IReadOnlyCollection<InteractedPost> GetInteractedPostPermLinkforComment(ActivityType activityType,
            string permaLink, string comment)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Get<InteractedPost>(x =>
                x.ActivityType == stractivityType && x.Permalink == permaLink && x.OldComment == comment).ToList();
        }

        public IReadOnlyCollection<InteractedAutoActivityPost> GetInteractedAutoActivityPosts(ActivityType activityType)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Get<InteractedAutoActivityPost>(x => x.ActivityType == stractivityType).ToList();
        }
    }
}