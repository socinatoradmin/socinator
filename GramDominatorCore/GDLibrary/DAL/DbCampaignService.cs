using System.Collections.Generic;
using DominatorHouseCore.DatabaseHandler.GdTables.Campaigns;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using System.Linq.Expressions;
using System;
using System.Data.Entity;

namespace GramDominatorCore.GDLibrary.DAL
{
    public interface IDbCampaignService
    {
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string user, ActivityType activityType);
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string sinAccUsername);
        IReadOnlyCollection<MakeCloseFriendCampaign> GetCloseFriend(string AccountUserName);
        IReadOnlyCollection<InteractedPosts> GetAllInteractedPosts();
        IReadOnlyCollection<InteractedUsers> GetAllInteractedUsers();
        // IReadOnlyCollection<UnfollowedUsers> GetAllUnfollowedUsers();
        bool Add<T>(T data) where T : class, new();
        IReadOnlyCollection<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new();

        T GetSingle<T>(Expression<Func<T, bool>> expression = null) where T : class, new();


        // bool DoesInteractedUserExist(string interactedUserId, ActivityType activityType);
    }

    public class DbCampaignService : IDbCampaignService
    {
        private readonly DbOperations _dbOperations;
        private readonly object _dbLock;

        public DbContext _context;
        public DbContext Context => _context;
        public DbCampaignService(JobProcess jobProcess)
        {
            _dbOperations = new DbOperations(jobProcess.CampaignId, SocialNetworks.Instagram,
                ConstantVariable.GetCampaignDb);
            _dbLock = new object();
        }
        public DbCampaignService(string campaignId)
        {
            _dbOperations = new DbOperations(campaignId, SocialNetworks.Instagram,
                ConstantVariable.GetCampaignDb);
            _dbLock = new object();
        }

        public DbCampaignService(DbContext context)
        {
            _context = context;
            context.Configuration.AutoDetectChangesEnabled = false;
            context.Configuration.ValidateOnSaveEnabled = false;
        }
        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string user, ActivityType activityType)
        {

            return _dbOperations.Get<InteractedPosts>(x => x.Username == user && x.ActivityType == activityType);
        }

        /// <summary>
        /// Selects <see cref="InteractedPosts"/> by SinAccUsername
        /// </summary>
        /// <returns>returns a collection of <see cref="InteractedPosts"/></returns>
        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string sinAccUsername)
        {

            return _dbOperations.Get<InteractedPosts>(x => x.Username == sinAccUsername);
        }

        public IReadOnlyCollection<InteractedPosts> GetAllInteractedPosts()
        {

            return _dbOperations.Get<InteractedPosts>();
        }

        //public bool DoesInteractedUserExist(string interactedUserId, ActivityType activityType)
        //{

        //    return _dbOperations.Any<InteractedUsers>(x =>
        //    x.InteractedUserId == interactedUserId && x.ActivityType == activityType.ToString());
        //}

        public IReadOnlyCollection<InteractedUsers> GetAllInteractedUsers()
        {

            return _dbOperations.Get<InteractedUsers>();
        }

        public bool Add<T>(T data) where T : class, new()
        {
            return _dbOperations.Add(data);
        }
        //public IReadOnlyCollection<UnfollowedUsers> GetAllUnfollowedUsers()
        //{

        //    return _dbOperations.Get<UnfollowedUsers>();
        //}
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
        public void AddToHashtagScrapeTable(HashtagScrape data)
        {

            _dbOperations.Add(data);
        }
        public IReadOnlyCollection<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return _dbOperations.Get(expression);
        }


        public T GetSingle<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return _dbOperations.GetSingle(expression);
        }

        public IReadOnlyCollection<MakeCloseFriendCampaign> GetCloseFriend(string AccountUserName)
        {
            return _dbOperations.Get<MakeCloseFriendCampaign>(x => x.AccountUserName == AccountUserName);
        }
    }
}
