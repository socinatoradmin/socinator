using DominatorHouseCore.DatabaseHandler.RdTables.Campaigns;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RedditDominatorCore.RDLibrary.DAL
{
    public interface IDbCampaignService
    {
        IReadOnlyCollection<InteractedPost> GetInteractedPosts(string user, ActivityType activityType);
        IReadOnlyCollection<InteractedPost> GetInteractedPosts(string sinAccUsername);
        IReadOnlyCollection<InteractedPost> GetAllInteractedPosts();
        bool DoesInteractedUserExist(string interactedUserId, ActivityType activityType);
        IReadOnlyCollection<InteractedUsers> GetAllInteractedUsers();
        IReadOnlyCollection<UnfollowedUsers> GetAllUnfollowedUsers();
        IReadOnlyCollection<InteractedPost> GetInteractedPostPermaLink(string permaLink, ActivityType activityType);
        IReadOnlyCollection<InteractedSubreddit> GetAllInteractedSubreddit(ActivityType activityType);
        bool AddToInteractedPost(InteractedPost interactedPost);

        InteractedPost GetSingleInteractedPost(string postPermalink, ActivityType activityType,
            DominatorAccountModel dominatorAccountModel);

        void Add<T>(T data) where T : class, new();
        void Update<T>(T data) where T : class, new();
        void Remove<T>(T data) where T : class, new();

        T GetSingle<T>(Expression<Func<T, bool>> func) where T : class, new();

        IReadOnlyCollection<InteractedPost> GetInteractedPost(string userName);
    }

    public class DbCampaignService : IDbCampaignService
    {
        private readonly DbOperations _dbOperations;

        public DbCampaignService(IRdJobProcess jobProcess)
        {
            _dbOperations = new DbOperations(jobProcess.CampaignId, SocialNetworks.Reddit,
                ConstantVariable.GetCampaignDb);
        }

        public DbCampaignService(string campaignId)
        {
            _dbOperations = new DbOperations(campaignId, SocialNetworks.Reddit,
                ConstantVariable.GetCampaignDb);
        }


        public IReadOnlyCollection<InteractedPost> GetInteractedPosts(string user, ActivityType activityType)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Get<InteractedPost>(
                x => x.InteractedUserName == user && x.ActivityType == stractivityType);
        }

        public IReadOnlyCollection<InteractedPost> GetInteractedPostPermaLink(string permaLink,
            ActivityType activityType)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Get<InteractedPost>(
                x => x.Permalink == permaLink && x.ActivityType == stractivityType);
        }

        /// <summary>
        ///     Selects <see cref="InteractedPost" /> by SinAccUsername
        /// </summary>
        /// <returns>returns a collection of <see cref="InteractedPost" /></returns>
        public IReadOnlyCollection<InteractedPost> GetInteractedPosts(string sinAccUsername)
        {
            return _dbOperations.Get<InteractedPost>(
                x => x.SinAccUsername == sinAccUsername);
        }

        public IReadOnlyCollection<InteractedPost> GetAllInteractedPosts()
        {
            return _dbOperations.Get<InteractedPost>();
        }

        public bool DoesInteractedUserExist(string interactedUserId, ActivityType activityType)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Any<InteractedUsers>(x =>
                x.InteractedUserId == interactedUserId && x.ActivityType == stractivityType);
        }

        public IReadOnlyCollection<InteractedUsers> GetAllInteractedUsers()
        {
            return _dbOperations.Get<InteractedUsers>();
        }

        public IReadOnlyCollection<UnfollowedUsers> GetAllUnfollowedUsers()
        {
            return _dbOperations.Get<UnfollowedUsers>();
        }

        public IReadOnlyCollection<InteractedSubreddit> GetAllInteractedSubreddit(ActivityType activityType)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Get<InteractedSubreddit>(x => x.ActivityType == stractivityType);
        }

        public bool AddToInteractedPost(InteractedPost interactedPost)
        {
            return _dbOperations.Add(interactedPost);
        }

        public InteractedPost GetSingleInteractedPost(string postPermalink, ActivityType activityType,
            DominatorAccountModel dominatorAccountModel)
        {
            var stractivityType = activityType.ToString();
            var userName = dominatorAccountModel.AccountBaseModel.UserName;
            return _dbOperations.GetSingle<InteractedPost>(x =>
                x.Permalink == postPermalink && x.ActivityType == stractivityType &&
                x.SinAccUsername == userName && (x.Status == "Pending" || x.Status == "Working"));
        }

        public void Add<T>(T data) where T : class, new()
        {
            _dbOperations.Add(data);
        }

        public void Update<T>(T data) where T : class, new()
        {
            _dbOperations.Update(data);
        }

        public void Remove<T>(T data) where T : class, new()
        {
            _dbOperations.Remove(data);
        }

        public T GetSingle<T>(Expression<Func<T, bool>> func) where T : class, new()
        {
            return _dbOperations.GetSingle(func);
        }


        public IReadOnlyCollection<InteractedPost> GetInteractedPost(string userName)
        {
            return _dbOperations.Get<InteractedPost>(x => x.SinAccUsername == userName);
        }
    }
}