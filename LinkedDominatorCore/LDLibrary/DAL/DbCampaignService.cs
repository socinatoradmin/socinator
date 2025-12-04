using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.LdTables.Campaign;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace LinkedDominatorCore.LDLibrary.DAL
{
    public interface IDbCampaignService
    {
        IReadOnlyCollection<InteractedUsers> GetAllInteractedUsers();
        IReadOnlyCollection<InteractedUsers> GetInteractedUsers(string activityType);
        IReadOnlyCollection<InteractedUsers> GetInteractedUsers(string activityType, string emailAddress);
        IReadOnlyCollection<InteractedGroups> GetInteractedGroups(string activityType);
        IReadOnlyCollection<InteractedPage> GetInteractedPages(string activityType);
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string activityType);
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string activityType, string profileUrl);
        IReadOnlyCollection<InteractedPosts> GetInteractedGroups(string activityType, string profileUrl);
        IReadOnlyCollection<InteractedJobs> GetInteractedJobs(string activityType);
        IReadOnlyCollection<InteractedCompanies> GetInteractedCompanies(string activityType);
        bool DoesInteractedUserExist(string interactedUserId, string activityType);
        IReadOnlyCollection<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new();

        void Add<T>(T data) where T : class, new();
        void AddRange<T>(List<T> data) where T : class, new();
        bool Update<T>(T data) where T : class, new();
        void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new();
        bool RemoveAll<T>() where T : class, new();
    }

    public class DbCampaignService : IDbCampaignService
    {
        public readonly IDbOperations DbOperations;


        public DbCampaignService(string campaignId)
        {
            if (!string.IsNullOrEmpty(campaignId))
                DbOperations = InstanceProvider.ResolveCampaignDbOperations(campaignId, SocialNetworks.LinkedIn);
        }

        public IReadOnlyCollection<InteractedUsers> GetAllInteractedUsers()
        {
            return DbOperations.Get<InteractedUsers>();
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUsers(string activityType)
        {
            return DbOperations.Get<InteractedUsers>(x => x.ActivityType == activityType);
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUsers(string activityType, string emailAddress)
        {
            return DbOperations.Get<InteractedUsers>(
                x => x.ActivityType == activityType && x.QueryValue == emailAddress);
        }

        public IReadOnlyCollection<InteractedGroups> GetInteractedGroups(string activityType)
        {
            return DbOperations.Get<InteractedGroups>(x => x.ActivityType == activityType);
        }

        public IReadOnlyCollection<InteractedPage> GetInteractedPages(string activityType)
        {
            return DbOperations.Get<InteractedPage>(x => x.ActivityType == activityType);
        }


        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string activityType, string profileUrl)
        {
            return DbOperations.Get<InteractedPosts>(x =>
                x.ActivityType == activityType && x.PostOwnerProfileUrl == profileUrl);
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string activityType)
        {
            return DbOperations.Get<InteractedPosts>(x => x.ActivityType == activityType);
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedGroups(string activityType, string profileUrl)
        {
            return DbOperations.Get<InteractedPosts>(x => x.ActivityType == activityType && x.QueryValue == profileUrl);
        }

        public IReadOnlyCollection<InteractedJobs> GetInteractedJobs(string activityType)
        {
            return DbOperations.Get<InteractedJobs>(x => x.ActivityType == activityType);
        }

        public IReadOnlyCollection<InteractedCompanies> GetInteractedCompanies(string activityType)
        {
            return DbOperations.Get<InteractedCompanies>(x => x.ActivityType == activityType);
        }

        public bool DoesInteractedUserExist(string interactedUserId, string activityType)
        {
            return DbOperations.Any<InteractedUsers>(x =>
                x.ProfileId == interactedUserId && x.ActivityType == activityType);
        }

        public IReadOnlyCollection<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            try
            {
                // Get the matched expression records, If expression is null returns full details
                return DbOperations.Get(expression);
            }
            catch
            {
                return new List<T>();
            }
        }

        public void Add<T>(T data) where T : class, new()
        {
            DbOperations.Add(data);
        }

        public void AddRange<T>(List<T> data) where T : class, new()
        {
            DbOperations.AddRange(data);
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUsers(string activityType,
            DominatorAccountModel dominatorAccountModel)
        {
            var activityTpe = ActivityType.ConnectionRequest.ToString();
            return DbOperations.Get<InteractedUsers>(x =>
                x.AccountEmail == dominatorAccountModel.AccountBaseModel.UserName && x.ActivityType == activityTpe);
        }

        public InteractedUsers GetSingleInteractedUser(string activityType, string profileUrl)
        {
            return DbOperations.GetSingle<InteractedUsers>(x =>
                x.ActivityType == activityType && x.UserProfileUrl == profileUrl);
        }

        public bool Update<T>(T data) where T : class, new()
        {
            return DbOperations.Update(data);
        }

        public void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            DbOperations.RemoveMatch(expression);
        }

        public bool RemoveAll<T>() where T : class, new()
        {
            return DbOperations.RemoveAll<T>();
        }
    }
}