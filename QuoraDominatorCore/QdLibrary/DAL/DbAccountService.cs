using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DominatorHouseCore.DatabaseHandler.QdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Utility;

namespace QuoraDominatorCore.QdLibrary.DAL
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

        IReadOnlyCollection<FeedInfoes> GetFeedInfos();
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType);
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType, string AccountName);
        IReadOnlyCollection<InteractedUsers> GetInteractedUsers(ActivityType activityType);
        List<string> GetInteractedUser();
        IReadOnlyCollection<Friendships> GetFriendships(params FollowType[] followTypes);
        IReadOnlyCollection<InteractedMessage> GetInteractedMessage();
        List<string> GetUnfollowedUsers();
        IReadOnlyCollection<UnfollowedUsers> GetUnfollowedUser();
        List<string> GetPrivateBlacklist();
        int GetInteractedAnswersCurrentWeek(ActivityType activityType, DateTime dateTime);
        int GetInteractedAnswersCurrentHour(ActivityType activityType, int hour);
        IReadOnlyCollection<InteractedAnswers> GetInteractedAnswers(ActivityType activityType);
        int GetPostfrmUniqueUser(string username, ActivityType activityType);

        IReadOnlyCollection<InteractedAnswers> GetInteractedAnswers();
        IReadOnlyCollection<InteractedQuestion> GetInteractedQuestion();
        IReadOnlyCollection<InteractedQuestion> GetInteractedQuestion(ActivityType activityType);
        int GetInteractedAnswersCurrentDay(ActivityType activityType, DateTime dateTime);

        bool IsAlreadyCommentedOnTweetwithSameQuery(string qeuryType, string queryValue, string commentId,
            ActivityType activityType);

        bool IsActivityDoneWithThisTweetId(string tweetId, ActivityType activityType);

        bool IsActivityDoneWithThisUserId(string userId, ActivityType activityType);

        void Add<T>(T data) where T : class, new();
        void AddRange<T>(List<T> data) where T : class, new();

        void DeleteInteractedUsers(ActivityType activity, string username);
        void DeleteInteractedAnswers(ActivityType activity, string username);
        void DeleteInteractedQuestion(ActivityType activity, string username);
        void DeleteInteractedMessage(ActivityType activity, string username);

        List<string> GetPrivateWhitelist();
        void Remove<T>(Expression<Func<T, bool>> expression) where T : class, new();
        void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new();
        void RemoveAll<T>() where T : class, new();
    }

    public class DbAccountService : DbAccountServiceBase, IDbAccountService
    {
        public DbAccountService(IEntityCountersManager entityCountersManager,
            DominatorAccountModel dominatorAccountModel) : base(entityCountersManager)
        {
            //should not use  dominatorAccountModel.AccountBaseModel.AccountNetwork 
            //because container not sending actual account only sending new object of DominatorAccountModel
            DbOperations = new DbOperations(dominatorAccountModel.AccountId, SocialNetworks.Quora,
                ConstantVariable.GetAccountDb);
        }

        [Obsolete(
            "db operation class should registered as singleton and should be injected in all the class. Get rid of this property later")]
        public DbOperations DbOperations { get; }

        public void Remove<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            DbOperations.Remove(expression);
        }

        public void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            DbOperations.RemoveMatch(expression);
        }

        public IReadOnlyCollection<FeedInfoes> GetFeedInfos()
        {
            return DbOperations.Get<FeedInfoes>().ToList();
        }

        public int GetPostfrmUniqueUser(string post, ActivityType activityType)
        {
            var activity = activityType.ToString();
            var a = DbOperations.Get<InteractedAnswers>(x => x.ActivityType == activity && x.AnswersUrl == post).Count;
            return a;
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType)
        {
            return DbOperations.Get<InteractedPosts>(x => x.ActivityType == activityType).ToList();
        }
        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType,string AccountName)
        {
            return DbOperations.Get<InteractedPosts>(x => x.ActivityType == activityType && x.AccountName.Contains(AccountName)).ToList();
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUsers(ActivityType activityType)
        {
            var activityTypeString = activityType.ToString();
            return DbOperations.Get<InteractedUsers>(x => x.ActivityType == activityTypeString).ToList();
        }

        public List<string> GetInteractedUser()
        {
            return DbOperations.Get<InteractedUsers>().Select(x => x.InteractedUsername).ToList();
        }

        public IReadOnlyCollection<Friendships> GetFriendships(params FollowType[] followTypes)
        {
            return DbOperations.Get<Friendships>(x => followTypes.Contains(x.FollowType)).ToList();
        }

        public bool IsAlreadyCommentedOnTweetwithSameQuery(string queryType, string queryValue, string commentId,
            ActivityType activityType)
        {
            return DbOperations.Any<InteractedPosts>(x => x.CommentId == commentId);
        }

        public bool IsActivityDoneWithThisTweetId(string tweetId, ActivityType activityType)
        {
            return DbOperations.Any<InteractedPosts>(x => x.ActivityType == activityType);
        }

        public bool IsActivityDoneWithThisUserId(string userId, ActivityType activityType)
        {
            var activityTypeString = activityType.ToString();
            return DbOperations.Any<InteractedUsers>(x =>
                x.InteractedUserId == userId && x.ActivityType == activityTypeString);
        }

        public IReadOnlyCollection<InteractedMessage> GetInteractedMessage()
        {
            return DbOperations.Get<InteractedMessage>().ToList();
        }

        public int GetInteractedAnswersCurrentWeek(ActivityType activityType, DateTime dateTime)
        {
            var activityTypeString = activityType.ToString();
            return DbOperations.Count<InteractedAnswers>(x =>
                x.ActivityType == activityTypeString && x.InteractionDateTime >= dateTime);
        }

        public int GetInteractedAnswersCurrentHour(ActivityType activityType, int hour)
        {
            var activityTypeString = activityType.ToString();
            return DbOperations.Count<InteractedAnswers>(x =>
                x.ActivityType == activityTypeString && hour - x.InteractionDateTime.Hour <= 3600);
        }

        public int GetInteractedAnswersCurrentDay(ActivityType activityType, DateTime dateTime)
        {
            var activityTypeString = activityType.ToString();
            return DbOperations.Count<InteractedAnswers>(x =>
                x.ActivityType == activityTypeString && x.InteractionDateTime >= dateTime);
        }

        public List<string> GetPrivateBlacklist()
        {
            return DbOperations.Get<PrivateBlacklist>()?.Select(x => x.UserName).ToList();
        }

        public List<string> GetPrivateWhitelist()
        {
            return DbOperations.Get<PrivateWhitelist>().Select(x => x.UserName).ToList();
        }

        public List<string> GetUnfollowedUsers()
        {
            return DbOperations.Get<UnfollowedUsers>().Select(x => x.UnfollowedUsername).ToList();
        }

        public IReadOnlyCollection<InteractedAnswers> GetInteractedAnswers(ActivityType activityType)
        {
            var activityTypeString = activityType.ToString();
            return DbOperations.Get<InteractedAnswers>(x => x.ActivityType == activityTypeString).ToList();
        }

        public IReadOnlyCollection<InteractedAnswers> GetInteractedAnswers()
        {
            return DbOperations.Get<InteractedAnswers>().ToList();
        }

        public IReadOnlyCollection<InteractedQuestion> GetInteractedQuestion(ActivityType activityType)
        {
            var activityTypeString = activityType.ToString();
            return DbOperations.Get<InteractedQuestion>(x => x.ActivityType == activityTypeString).ToList();
        }

        public IReadOnlyCollection<InteractedQuestion> GetInteractedQuestion()
        {
            return DbOperations.Get<InteractedQuestion>().ToList();
        }

        public IReadOnlyCollection<UnfollowedUsers> GetUnfollowedUser()
        {
            return DbOperations.Get<UnfollowedUsers>().ToList();
        }

        public void Add<T>(T data) where T : class, new()
        {
            DbOperations.Add(data);
        }

        public void AddRange<T>(List<T> data) where T : class, new()
        {
            DbOperations.AddRange(data);
        }

        public void DeleteInteractedUsers(ActivityType activityType, string username)
        {
            var activityTypeString = activityType.ToString();
            DbOperations.RemoveMatch<InteractedUsers>(x =>
                x.ActivityType == activityTypeString && x.Username == username);
        }

        public void DeleteInteractedAnswers(ActivityType activityType, string username)
        {
            var activityTypeString = activityType.ToString();
            DbOperations.RemoveMatch<InteractedAnswers>(x =>
                x.ActivityType == activityTypeString && x.Accountusername == username);
        }

        public void DeleteInteractedQuestion(ActivityType activityType, string username)
        {
            var activityTypeString = activityType.ToString();
            DbOperations.RemoveMatch<InteractedQuestion>(x =>
                x.ActivityType == activityTypeString && x.Accountusername == username);
        }

        public void DeleteInteractedMessage(ActivityType activityType, string username)
        {
            var activityTypeString = activityType.ToString();
            DbOperations.RemoveMatch<InteractedMessage>(x =>
                x.ActivityType == activityTypeString && x.SinAccUsername == username);
        }

        public void RemoveAll<T>() where T : class, new()
        {
            DbOperations.RemoveAll<T>();
        }
    }
}