using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FaceDominatorCore.FDLibrary.DAL
{
    public interface IDbAccountServiceScoped : IDbAccountService
    {

    }

    public class DbAccountServiceScoped : DbAccountService, IDbAccountServiceScoped
    {
        public DbAccountServiceScoped(IProcessScopeModel processScopeModel) :
            base(processScopeModel.Account)
        {
        }
    }

    public interface IDbAccountService
    {
        #region region
        bool DoesInteractedPostsExist(string interactedPostId, string postUrl, ActivityType activityType, bool isCommentProcessComplete = true);

        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType, bool isCommentProcessComplete = true, string postId = "");

        bool DoesInteractedUserExist(string interactedUserId, ActivityType activityType, bool isSkippedUsers = false);

        IReadOnlyCollection<InteractedUsers> GetInteractedUsers(ActivityType activityType);


        bool DoesInteractedPagesExist(string interactedPageId, ActivityType activityType);

        bool DoesInteractedPagesExistCustom(string interactedPageUrl, ActivityType activityType);

        /*
                IReadOnlyCollection<InteractedPages> GetInteractedPages(ActivityType activityType);
        */

        bool DoesInteractedEventsExist(string interactedEventId, ActivityType activityType);


        bool DoesInteractedGroupsExist(string interactedGroupUrl, ActivityType activityType);

        bool DoesInteractedUserExistCustom(string interactedGroupUrl, ActivityType activityType);


        IReadOnlyCollection<InteractedGroups> GetInteractedGroups(ActivityType activityType);

        Task<List<T>> GetUserDetails<T>(Expression<Func<T, bool>> expression = null) where T : class, new();

        bool DoesInteractedCommentsExist(string interactedCommentId, ActivityType activityType);

        /*
                IReadOnlyCollection<InteractedComments> GetInteractedComments(ActivityType activityType);
        */

        IReadOnlyCollection<T> Get<T>(Expression<Func<T, bool>> expr = null) where T : class, new();

        bool Any<T>(Expression<Func<T, bool>> expr) where T : class, new();

        IReadOnlyCollection<string> GetSingleColumn<T>(Func<T, string> query, Expression<Func<T, bool>> expression = null) where T : class, new();

        /*
                [Obsolete("db operation class should registered as singleton and should be injected in all the class. Get rid of this property later")]
                DbOperations DbOperations { get; }
        */

        bool AddRange<T>(List<T> obj) where T : class, new();
        //DbOperation

        void Remove<T>(Expression<Func<T, bool>> expression) where T : class, new();



        IReadOnlyCollection<PrivateBlacklist> GetPrivateBlacklist();

        IReadOnlyCollection<PrivateWhitelist> GetPrivateWhitelist();
        #endregion

        #region CRUD operations
        void UpdateRange<T>(List<T> obj) where T : class, new();
        void Add<T>(T obj) where T : class, new();
        void RemoveAllMatches<T>(Expression<Func<T, bool>> expr = null) where T : class, new();
        void Remove<T>(T obj) where T : class, new();
        void RemoveAll<T>() where T : class, new();
        int Count<T>(Expression<Func<T, bool>> expr = null) where T : class, new();
        void Update<T>(T obj) where T : class, new();
        T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class, new();
        #endregion

    }

    public class DbAccountService : IDbAccountService
    {
        private readonly IDbOperations _dbOperations;

        public DbAccountService(DominatorAccountModel dominatorAccountModel)
        {
            _dbOperations = InstanceProvider.ResolveAccountDbOperations(dominatorAccountModel.AccountId, SocialNetworks.Facebook);
        }

        public DbAccountService(string accountId)
        {
            _dbOperations = InstanceProvider.ResolveAccountDbOperations(accountId, SocialNetworks.Facebook);
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType,
            bool isCommentProcessComplete = true, string postId = "")
        {
            string activity = activityType.ToString();

            if (!string.IsNullOrEmpty(postId))
                return _dbOperations.Get<InteractedPosts>(
                      x => x.ActivityType == activity && x.PostId == postId);

            return isCommentProcessComplete ? _dbOperations.Get<InteractedPosts>(
                 x => x.ActivityType == activity) : _dbOperations.Get<InteractedPosts>(
                     x => x.ActivityType == activity && x.IsMoreCommentsNeeded == true);
        }


        public bool DoesInteractedUserExist(string interactedUserId, ActivityType activityType, bool isSkippedUsers = false)
        {
            string activity = isSkippedUsers ? "SkippedUsers" : activityType.ToString();
            return _dbOperations.Any<InteractedUsers>(x =>
                x.UserId == interactedUserId && x.ActivityType == activity);
        }

        public bool DoesInteractedUserExistCustom(string interactedUserUrl, ActivityType activityType)
        {
            string activity = activityType.ToString();
            return _dbOperations.Any<InteractedUsers>(x =>
               (interactedUserUrl.Contains(x.UserProfileUrl) || x.DetailedUserInfo.Contains(interactedUserUrl))
               && x.ActivityType == activity);
        }

        public bool DoesInteractedPostsExist(string interactedPostId, string postUrl, ActivityType activityType
            , bool isCommentProcessComplete = true)
        {
            string activity = activityType.ToString();
            if (isCommentProcessComplete)
                return _dbOperations.Any<InteractedPosts>(x => ((interactedPostId != "" && x.PostId == interactedPostId) || (postUrl != "" && x.PostUrl == postUrl) || (postUrl != "" && x.QueryValue.Contains(postUrl)))
                && x.ActivityType == activity);
            else
            {
                var lstData = _dbOperations.Get<InteractedPosts>(x => ((interactedPostId != "" && x.PostId == interactedPostId) || (postUrl != "" && x.PostUrl == postUrl) || (postUrl != "" && x.QueryValue.Contains(postUrl)))
                        && x.ActivityType == activity);
                if (lstData.Any(x => x.IsMoreCommentsNeeded == false))
                    return true;
                else
                    return false;
            }

        }

        public bool DoesInteractedPagesExist(string interactedPageId, ActivityType activityType)
        {
            string activity = activityType.ToString();
            return _dbOperations.Any<InteractedPages>(x =>
                 x.PageId == interactedPageId && x.ActivityType == activity);
        }

        public bool DoesInteractedPagesExistCustom(string interactedPageUrl, ActivityType activityType)
        {
            string activity = activityType.ToString();
            return _dbOperations.Any<InteractedPages>(x =>
                 x.PageUrl == interactedPageUrl || x.PageFullDetails.Contains(interactedPageUrl) && x.ActivityType == activity);
        }


        public bool DoesInteractedGroupsExist(string interactedGroupUrl, ActivityType activityType)
        {
            string activity = activityType.ToString();
            return _dbOperations.Any<InteractedGroups>(x =>
                 x.GroupUrl == interactedGroupUrl && x.ActivityType == activity);
        }

        public bool DoesInteractedCommentsExist(string interactedCommentId, ActivityType activityType)
        {
            string activity = activityType.ToString();
            return _dbOperations.Any<InteractedComments>(x =>
                 x.CommentId == interactedCommentId && x.ActivityType == activity);
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUsers(ActivityType activityType)
        {
            string activity = activityType.ToString();
            return _dbOperations.Get<InteractedUsers>(
             x => x.ActivityType == activity);
        }

        /*
                public IReadOnlyCollection<InteractedPages> GetInteractedPages(ActivityType activityType)
                {
                    string activity = activityType.ToString();
                    return _dbOperations.Get<InteractedPages>(
                     x => x.ActivityType == activity);
                }
        */

        public IReadOnlyCollection<InteractedGroups> GetInteractedGroups(ActivityType activityType)
        {
            string activity = activityType.ToString();
            return _dbOperations.Get<InteractedGroups>(
             x => x.ActivityType == activity);
        }
        public IReadOnlyCollection<PrivateBlacklist> GetPrivateBlacklist()
        {
            return _dbOperations.Get<PrivateBlacklist>().ToList();
        }

        public IReadOnlyCollection<PrivateWhitelist> GetPrivateWhitelist()
        {
            return _dbOperations.Get<PrivateWhitelist>().ToList();
        }

        /*
                public IReadOnlyCollection<InteractedComments> GetInteractedComments(ActivityType activityType)
                {
                    string activity = activityType.ToString();
                    return _dbOperations.Get<InteractedComments>(
                        x => x.ActivityType == activity);
                }
        */

        public void Add<T>(T obj) where T : class, new()
        {
            _dbOperations.Add(obj);
        }

        public bool AddRange<T>(List<T> obj) where T : class, new()
        {
            return _dbOperations.AddRange(obj);
        }


        public void UpdateRange<T>(List<T> obj) where T : class, new()
        {
            _dbOperations.UpdateRange(obj);

        }

        public void Remove<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            _dbOperations.Remove(expression);
        }

        public void Remove<T>(T obj) where T : class, new()
        {
            _dbOperations.Remove(obj);
        }


        public void RemoveAll<T>() where T : class, new()
        {
            _dbOperations.RemoveAll<T>();
        }


        public void RemoveAllMatches<T>(Expression<Func<T, bool>> expr = null) where T : class, new()
        {
            _dbOperations.RemoveMatch(expr);
        }


        public IReadOnlyCollection<T> Get<T>(Expression<Func<T, bool>> expr = null) where T : class, new()
        {
            return expr == null ? _dbOperations.Get<T>() : _dbOperations.Get(expr);
        }

        public async Task<IReadOnlyCollection<T>> GetAsync<T>(Expression<Func<T, bool>> expr = null) where T : class, new()
        {
            return expr == null ? await _dbOperations.GetAsync<T>() : await _dbOperations.GetAsync(expr);
        }

        public void Update<T>(T obj) where T : class, new()
        {
            _dbOperations.Update(obj);
        }

        public int Count<T>(Expression<Func<T, bool>> expr = null) where T : class, new()
        {
            return expr == null ? _dbOperations.Count<T>() : _dbOperations.Count(expr);
        }

        public bool Any<T>(Expression<Func<T, bool>> expr) where T : class, new()
        {
            return _dbOperations.Any(expr);
        }



        public IReadOnlyCollection<string> GetSingleColumn<T>
            (Func<T, string> query, Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            var dbOperations = (DbOperations)_dbOperations;
            return expression == null ? dbOperations.GetSingleColumn(query) : dbOperations.GetSingleColumn(query, expression);
        }

        public T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return _dbOperations.GetSingle(expression);
        }

        public bool DoesInteractedEventsExist(string interactedEventId, ActivityType activityType)
        {
            string activity = activityType.ToString();
            return _dbOperations.Any<InteractedEvents>(x =>
                x.EventGUid == interactedEventId && x.ActivityType == activity);
        }

        public async Task<List<T>> GetUserDetails<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return expression != null ? await _dbOperations.GetAsync<T>(expression) : await _dbOperations.GetAsync<T>();
        }
    }
}
