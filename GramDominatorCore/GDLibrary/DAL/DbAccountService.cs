using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.DatabaseHandler.GdTables;
using System.Linq.Expressions;
using DominatorHouseCore.Process.ExecutionCounters;

namespace GramDominatorCore.GDLibrary.DAL
{
    public interface IDbAccountService
    {
        // IReadOnlyCollection<FeedInfoes> GetFeedInfos(int startValue, int endValue);

        IReadOnlyCollection<FeedInfoes> GetFeedInfos();
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string sinAccUsername, ActivityType activityType);
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType);
        IReadOnlyCollection<InteractedUsers> GetInteractedUsers(ActivityType activityType);

        IReadOnlyCollection<InteractedUsers> GetInteractedUserFriends();

        IReadOnlyCollection<UnfollowedUsers> GetUnfollowedUser();
        IReadOnlyCollection<Friendships> GetFriendships(params FollowType[] followTypes);
        IReadOnlyCollection<Friendships> GetFollowers();
        IReadOnlyCollection<Friendships> GetFollowings();
        IReadOnlyCollection<UserConversation> GetConversationUser();
        IReadOnlyCollection<PrivateBlacklist> GetPrivateBlacklist();

        IReadOnlyCollection<PrivateWhitelist> GetPrivateWhitelist();
        void AddRange<T>(List<T> data) where T : class, new();

        void Remove<T>(Expression<Func<T, bool>> expression) where T : class, new();

        bool RemoveAll<T>() where T : class, new();

        IReadOnlyCollection<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new();

        T GetSingle<T>(Expression<Func<T, bool>> expression = null) where T : class, new();

        void RemoveMatch<T>(Expression<Func<T, bool>> expression = null) where T : class, new();

        //void RemoveList<T>(List<T> RemoveList) where T : class;

        IReadOnlyCollection<HashtagScrape> GetHashtagScrape(string username);
        bool Add<T>(T data) where T : class, new();

        //bool IsAlreadyCommentedOnPostwithSameQuery(string qeuryType, string queryValue, string postId,
        //    ActivityType activityType);

        //bool IsActivtyDoneWithThisPostId(string postId, ActivityType activityType);

        //bool IsActivityDoneWithThisUserId(string userId, ActivityType activityType);

        [Obsolete("db operation class should registered as singleton and should be injected in all the class. Get rid of this property later")]
        DbOperations DbOperations { get; }

        IReadOnlyCollection<InteractedUsers> GetInteractedUsersMessageData();
    }

    public interface IDbAccountServiceScoped : IDbAccountService
    {

    }

    public class DbAccountServiceScoped : DbAccountService, IDbAccountServiceScoped
    {
        public DbAccountServiceScoped(IProcessScopeModel processScopeModel)
            : base(processScopeModel.Account)
        {
        }
    }
    public class DbAccountService : IDbAccountService
    {
        //  private static readonly object DatabaseLock = new object();
        //private static readonly object FollowersDetailsDatabaseLock = new object();
        private readonly DbOperations _dbOperations;
        // private readonly object _dbLock;
        [Obsolete("db operation class should registered as singleton and should be injected in all the class. Get rid of this property later")]
        public DbOperations DbOperations => _dbOperations;

        public DbAccountService(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                _dbOperations = new DbOperations(dominatorAccountModel.AccountId, SocialNetworks.Instagram,
                      ConstantVariable.GetAccountDb);
            }
            catch (Exception)
            {
                // ignored
            }

            //_dbLock = new object();
        }


        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string sinAccUsername, ActivityType activityType)
        {
            return _dbOperations.Get<InteractedPosts>(x => x.Username == sinAccUsername && x.ActivityType == activityType).ToList();
        }
        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType)
        {
            return _dbOperations.Get<InteractedPosts>(x => x.ActivityType == activityType).ToList();
        }
        public IReadOnlyCollection<InteractedUsers> GetInteractedUsers(ActivityType activityType)
        {
            string strActivityType = activityType.ToString();
            return _dbOperations.Get<InteractedUsers>(x => x.ActivityType == strActivityType).ToList();
        }

       

        public IReadOnlyCollection<InteractedUsers> GetInteractedUserFriends()
        {
            return _dbOperations.Get<InteractedUsers>(x => (x.ActivityType == "Follow" || x.ActivityType == "FollowBack")).ToList();

            //return _dbOperations.Get<InteractedUsers>(x => (x.ActivityType == ActivityType.Follow.ToString() ||
            //                                                x.ActivityType == ActivityType.FollowBack.ToString()) && x.FollowStatus == 1).ToList();
        }
        public IReadOnlyCollection<Friendships> GetFriendships(params FollowType[] followTypes)
        {
            return _dbOperations.Get<Friendships>(x => followTypes.Contains(x.FollowType)).ToList();
        }
      
        public IReadOnlyCollection<Friendships> GetFollowers()
        {
            return _dbOperations.Get<Friendships>(x => x.Followers == 1).ToList();
        }
        public IReadOnlyCollection<Friendships> GetFollowings()
        {
            return _dbOperations.Get<Friendships>(x => x.Followings == 1).ToList();
        }
        public IReadOnlyCollection<Friendships> GetFollowingsWithoutRequestUser()
        {
            return _dbOperations.Get<Friendships>(x => x.Followings == 1 && !x.IsPrivate).ToList();
        }
        public IReadOnlyCollection<FeedInfoes> GetFeedInfos()
        {
            return _dbOperations.Get<FeedInfoes>().ToList();
        }
        public IReadOnlyCollection<HashtagScrape> GetHashtagScrape(string userName)
        {
            return _dbOperations.Get<HashtagScrape>(x => x.AccountUsername == userName);
        }
        public int GetInteractedUsersCount(ActivityType activityType, int dateTimeStamp)
        {
            string strActivityType = activityType.ToString();
            return _dbOperations.Count<InteractedUsers>(x => (x.ActivityType == strActivityType) && (x.Date >= dateTimeStamp));
        }
        public int GetInteractedUsersThisHourCount(ActivityType activityType, int dateTimeStamp, int hourTimeStamp)
        {
            string strActivityType = activityType.ToString();
            return _dbOperations.Count<InteractedUsers>(x => (x.ActivityType == strActivityType) && (x.Date >= hourTimeStamp && (x.Date >= dateTimeStamp)));
        }
        public int GetInteractedPostsCount(ActivityType activityType, int dateTimeStamp)
        {
            // string strActivityType = activityType.ToString();
            return _dbOperations.Count<InteractedPosts>(x => (x.ActivityType == activityType) && (x.InteractionDate >= dateTimeStamp));
        }

        public int GetInteractedPostsThisHourCount(ActivityType activityType, int dateTimeStamp, int hourTimeStamp)
        {
            // string strActivityType = activityType.ToString();
            return _dbOperations.Count<InteractedPosts>(x => (x.ActivityType == activityType) && (x.InteractionDate >= hourTimeStamp && (x.InteractionDate >= dateTimeStamp)));
        }
        public int GetUnfollowedUsersCount(int dateTimeStamp)
        {
            return _dbOperations.Count<UnfollowedUsers>(x => x.InteractionDate >= dateTimeStamp);
        }

        public int GetUnfollowedUsersThisHourCount(int dateTimeStamp, int hourTimeStamp)
        {
            return _dbOperations.Count<UnfollowedUsers>(x => x.InteractionDate >= hourTimeStamp && (x.InteractionDate >= dateTimeStamp));
        }
        public int GetScrapedHashtagsCount(int dateTimeStamp)
        {
            return _dbOperations.Count<HashtagScrape>(x => x.Date >= dateTimeStamp);
        }

        public int GetScrapedHashtagsThisHourCount(int dateTimeStamp, int hourTimeStamp)
        {
            return _dbOperations.Count<HashtagScrape>(x => x.Date >= hourTimeStamp && (x.Date >= dateTimeStamp));
        }


        public bool Add<T>(T data) where T : class, new()
        {
            return _dbOperations.Add(data);
        }
        public void AddToInteractedUsersTable(InteractedUsers data)
        {
            _dbOperations.Add(data);
        }
        public void AddToUnfollowedUsersTable(UnfollowedUsers data)
        {
            _dbOperations.Add(data);
        }
        public void AddToInteractedPostsTable(InteractedPosts data)
        {
            _dbOperations.Add(data);
        }
        public void AddToFeedInfoesTable(FeedInfoes data)
        {
            _dbOperations.Add(data);
        }
        public void AddToHashtagScrapeTable(HashtagScrape data)
        {
            _dbOperations.Add(data);
        }
        public void AddToFriendshipsTable(Friendships data)
        {
            _dbOperations.Add(data);
        }

        public IReadOnlyCollection<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return _dbOperations.Get(expression);
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUsersMessageData() 
        {
            var brodcastMessageActivity = ActivityType.BroadcastMessages.ToString();
            var SendMessageToNewFollowerActivity = ActivityType.SendMessageToFollower.ToString();
            var AutoReplyToNewMessageActivity = ActivityType.AutoReplyToNewMessage.ToString();
            return _dbOperations.Get<InteractedUsers>(x => x.ActivityType == brodcastMessageActivity || x.ActivityType == SendMessageToNewFollowerActivity || x.ActivityType == AutoReplyToNewMessageActivity);
            
        }

        public T GetSingle<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return _dbOperations.GetSingle(expression);
        }
        public void RemoveMatch<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            _dbOperations.RemoveMatch(expression);
        }

        IReadOnlyCollection<UnfollowedUsers> IDbAccountService.GetUnfollowedUser()
        {
            return _dbOperations.Get<UnfollowedUsers>().ToList();
        }

        //public void RemoveList<T>(List<T> RemoveList) where T : class
        //{
        //    try
        //    {
        //        _dbOperations.Context.Set<T>().RemoveRange(RemoveList);
        //        _dbOperations.Context.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
        public IReadOnlyCollection<PrivateBlacklist> GetPrivateBlacklist()
        {
            return _dbOperations.Get<PrivateBlacklist>().ToList();
        }

        public IReadOnlyCollection<PrivateWhitelist> GetPrivateWhitelist()
        {
            return _dbOperations.Get<PrivateWhitelist>().ToList();
        }


        public void AddRange<T>(List<T> data) where T : class, new()
        {
            _dbOperations.AddRange(data);
        }


        public void Remove<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            _dbOperations.Remove(expression);
        }
        public bool RemoveAll<T>() where T : class, new()
        {
            return _dbOperations.RemoveAll<T>();
        }

        public IReadOnlyCollection<UserConversation> GetConversationUser()
        {
            return _dbOperations.Get<UserConversation>().ToList();
        }
    }
}
