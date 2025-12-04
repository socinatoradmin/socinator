using System.Collections.Generic;
using DominatorHouseCore.DatabaseHandler.TdTables.Campaign;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.DAL
{
    public interface IDbCampaignService
    {
        string CampaignId { get; }
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string user, ActivityType activityType);
        IReadOnlyCollection<InteractedPosts> GetAllInteractedPosts();
        bool DoesInteractedUserExist(string interactedUserId, ActivityType activityType);
        IReadOnlyCollection<InteractedUsers> GetAllInteractedUsers();
        IReadOnlyCollection<UnfollowedUsers> GetAllUnfollowedUsers();
        bool IsActivityDoneWithThisUserId(string userId, ActivityType activityType);

        #region CRUD operation

        void Add<T>(T data) where T : class, new();

        #endregion
    }

    public class DbCampaignService : IDbCampaignService
    {
        private readonly DbOperations _dbOperations;

        public DbCampaignService(string campaignId)
        {
            CampaignId = campaignId;
            _dbOperations = new DbOperations(campaignId, SocialNetworks.Twitter,
                ConstantVariable.GetCampaignDb);
        }

        public string CampaignId { get; }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string user, ActivityType activityType)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Get<InteractedPosts>(
                x => (x.SinAccUsername == user || x.Username == user) && x.ActivityType == stractivityType);
        }

        public IReadOnlyCollection<InteractedPosts> GetAllInteractedPosts()
        {
            return _dbOperations.Get<InteractedPosts>();
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

        public void Add<T>(T data) where T : class, new()
        {
            _dbOperations.Add(data);
        }

        public bool IsActivityDoneWithThisUserId(string userId, ActivityType activityType)
        {
            var stractivityType = activityType.ToString();
            return _dbOperations.Any<InteractedUsers>(x =>
                x.InteractedUserId == userId && x.ActivityType == stractivityType);
        }
    }
}