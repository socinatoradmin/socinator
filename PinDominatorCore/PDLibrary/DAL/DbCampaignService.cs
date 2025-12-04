using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Campaigns;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Utility;

namespace PinDominatorCore.PDLibrary.DAL
{
    public interface IDbCampaignService
    {
        IReadOnlyCollection<InteractedPosts> GetAllInteractedPosts();
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string user, ActivityType activityType);
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string sinAccUsername);
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType);
        IReadOnlyCollection<InteractedUsers> GetInteractedUsers(ActivityType activityType);
        void Add<T>(T data) where T : class, new();
        void AddRange<T>(List<T> data) where T : class, new();
        List<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new();
        IReadOnlyCollection<InteractedBoards> GetInteractedBoards(ActivityType activityType);
    }

    public class DbCampaignService : IDbCampaignService
    {
        private readonly IDbOperations _dbOperations;

        public DbCampaignService(string campaignId)
        {
            try
            {
                _dbOperations =
                    InstanceProvider.ResolveCampaignDbOperations(campaignId, SocialNetworks.Pinterest);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public IReadOnlyCollection<InteractedPosts> GetAllInteractedPosts()
        {
            return _dbOperations.Get<InteractedPosts>();
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string user, ActivityType activityType)
        {
            var actType = activityType.ToString();
            var actTypeInt = ((int)ActivityType.Try).ToString();
            return _dbOperations.Get<InteractedPosts>(
                x => x.Username == user && (x.OperationType == actType || x.OperationType == actTypeInt));
        }

        /// <summary>
        ///     Selects <see cref="InteractedPosts" /> by SinAccUsername
        /// </summary>
        /// <returns>returns a collection of <see cref="InteractedPosts" /></returns>
        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string sinAccUsername)
        {
            return _dbOperations.Get<InteractedPosts>(
                x => x.SinAccUsername == sinAccUsername);
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType)
        {
            var actType = activityType.ToString();
            var actTypeInt = ((int)ActivityType.Try).ToString();
            return _dbOperations.Get<InteractedPosts>(x => x.OperationType == actType || x.OperationType == actTypeInt);
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUsers(ActivityType activityType)
        {
            var actType = activityType.ToString();
            return _dbOperations.Get<InteractedUsers>(x => x.ActivityType == actType);
        }

        public void Add<T>(T data) where T : class, new()
        {
            _dbOperations.Add(data);
        }

        public void AddRange<T>(List<T> data) where T : class, new()
        {
            _dbOperations.AddRange(data);
        }

        public List<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return _dbOperations.Get(expression);
        }

        public IReadOnlyCollection<InteractedBoards> GetInteractedBoards(ActivityType activityType)
        {
            return _dbOperations.Get<InteractedBoards>(x => x.OperationType == activityType).ToList();
        }
    }
}