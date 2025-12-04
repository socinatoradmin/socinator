using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.TumblrTables.Campaign;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using System.Collections.Generic;

namespace TumblrDominatorCore.TumblrLibrary.DAL
{
    public interface IDbCampaignService
    {
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string post, ActivityType activityType, string status);
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType);
        IReadOnlyCollection<InteractedUser> GetInteractedUsers(ActivityType activityType);
        int GetCountOfInteractionForSpecificPost(ActivityType activityType, string post);
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(QueryInfo queryInfo, ActivityType activityType);
        IReadOnlyCollection<InteractedPosts> GetAllInteractedPosts();

        IReadOnlyCollection<InteractedUser> GetAllInteractedUsers();
        IReadOnlyCollection<UnFollowedUser> GetAllUnfollowedUsers();
        IReadOnlyCollection<UnLikedPosts> GetAllUnLikedPosts();
        IReadOnlyCollection<InteractedUser> GetSelectedUsers(string username);
        InteractedPosts GetSpecificPost(string post, ActivityType activityType, string accountmailId);

        void Add<T>(T t) where T : class, new();

        void Update<T>(T t) where T : class, new();
    }


    public class DbCampaignService : IDbCampaignService
    {
        private readonly IDbOperations _dbOperations;

        public DbCampaignService(JobProcess jobProcess)
        {
            //_dbOperations = new DbOperations(jobProcess.CampaignId, SocialNetworks.Tumblr,
            //    ConstantVariable.GetCampaignDb);
            _dbOperations =
                InstanceProvider.ResolveCampaignDbOperations(jobProcess.CampaignId ?? string.Empty,
                    SocialNetworks.Tumblr);
        }

        public DbCampaignService(string campaignId)
        {
            //_dbOperations = new DbOperations(campaignId, SocialNetworks.Tumblr,
            //    ConstantVariable.GetCampaignDb);
            _dbOperations =
                InstanceProvider.ResolveCampaignDbOperations(campaignId ?? string.Empty, SocialNetworks.Tumblr);
        }

        public IReadOnlyCollection<InteractedPosts> GetAllInteractedPosts()
        {
            return _dbOperations.Get<InteractedPosts>();
        }

        public IReadOnlyCollection<InteractedUser> GetAllInteractedUsers()
        {
            return _dbOperations.Get<InteractedUser>();
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(QueryInfo queryInfo, ActivityType activityType)
        {
            var activity = activityType.ToString();
            return _dbOperations.Get<InteractedPosts>(x =>
                x.ActivityType == activity && x.QueryType == queryInfo.QueryType &&
                x.QueryValue == queryInfo.QueryValue);
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType)
        {
            var activity = activityType.ToString();
            return _dbOperations.Get<InteractedPosts>(x => x.ActivityType == activity);
        }

        public void Add<T>(T data) where T : class, new()
        {
            _dbOperations.Add(data);
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string post, ActivityType activityType,
            string status)
        {
            var activity = activityType.ToString();
            return _dbOperations.Get<InteractedPosts>(x =>
                x.ActivityType == activity && x.ContentId == post && (x.Status == "Success" || x.Status == "Working"));
        }

        public int GetCountOfInteractionForSpecificPost(ActivityType activityType, string post)
        {
            var activity = activityType.ToString();
            return _dbOperations.Count<InteractedPosts>(x => x.ActivityType == activity && x.ContentId == post);
        }

        public IReadOnlyCollection<InteractedUser> GetInteractedUsers(ActivityType activityType)
        {
            var activity = activityType.ToString();
            return _dbOperations.Get<InteractedUser>(x => x.ActivityType == activity);
        }

        public IReadOnlyCollection<InteractedUser> GetSelectedUsers(string username)
        {
            return _dbOperations.Get<InteractedUser>(x => x.AccountEmail == username);
        }

        public InteractedPosts GetSpecificPost(string post, ActivityType activityType, string accountmailId)
        {
            return _dbOperations.GetSingle<InteractedPosts>(x =>
                x.ContentId == post && x.ActivityType == activityType.ToString() &&
                x.AccountEmail == accountmailId && (x.Status == "Pending" || x.Status == "Working"));
        }

        public void Update<T>(T data) where T : class, new()
        {
            _dbOperations.Update(data);
        }

        public IReadOnlyCollection<UnFollowedUser> GetAllUnfollowedUsers()
        {
            return _dbOperations.Get<UnFollowedUser>();
        }

        public IReadOnlyCollection<UnLikedPosts> GetAllUnLikedPosts()
        {
            return _dbOperations.Get<UnLikedPosts>();
        }
    }
}