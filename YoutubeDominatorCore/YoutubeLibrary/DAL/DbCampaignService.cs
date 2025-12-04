using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.DatabaseHandler.YdTables.Campaign;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace YoutubeDominatorCore.YoutubeLibrary.DAL
{
    public interface IDbCampaignService
    {
        IReadOnlyCollection<InteractedPosts> GetAllInteractedPosts();
        IReadOnlyCollection<InteractedChannels> GetAllInteractedChannels();

        IReadOnlyCollection<InteractedPosts> GetSpecificInterectedPost(ActivityType activityType, string userName,
            string myChannelId, string postUrl);

        void Add<T>(T data) where T : class, new();
        List<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new();

        T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class, new();
        bool Update<T>(T t) where T : class, new();
    }

    public class DbCampaignService : IDbCampaignService
    {
        private readonly DbOperations _dbOperations;

        public DbCampaignService(string campaignId)
        {
            _dbOperations = new DbOperations(campaignId, SocialNetworks.YouTube,
                ConstantVariable.GetCampaignDb);
        }

        public IReadOnlyCollection<InteractedPosts> GetAllInteractedPosts()
        {
            return _dbOperations.Get<InteractedPosts>();
        }

        public IReadOnlyCollection<InteractedChannels> GetAllInteractedChannels()
        {
            return _dbOperations.Get<InteractedChannels>();
        }

        public IReadOnlyCollection<InteractedPosts> GetSpecificInterectedPost(ActivityType activityType,
            string userName, string myChannelId, string postUrl)
        {
            var activity = activityType.ToString();
            return _dbOperations.Get<InteractedPosts>(x =>
                x.ActivityType == activity && x.AccountUsername == userName && x.MyChannelId == myChannelId &&
                x.VideoUrl == postUrl).ToList();
        }

        public void Add<T>(T data) where T : class, new()
        {
            _dbOperations.Add(data);
        }

        public List<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return _dbOperations.Get(expression);
        }

        public T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return _dbOperations.GetSingle(expression);
        }

        public bool Update<T>(T t) where T : class, new()
        {
            return _dbOperations.Update(t);
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string user, ActivityType activityType)
        {
            var activity = activityType.ToString();
            return _dbOperations.Get<InteractedPosts>(
                x => x.AccountUsername == user && x.ActivityType == activity);
        }

        /// <summary>
        ///     Selects <see cref="InteractedPosts" /> by SinAccUsername
        /// </summary>
        /// <returns>returns a collection of <see cref="InteractedPosts" /></returns>
        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string sinAccUsername)
        {
            return _dbOperations.Get<InteractedPosts>(
                x => x.AccountUsername == sinAccUsername);
        }
    }
}