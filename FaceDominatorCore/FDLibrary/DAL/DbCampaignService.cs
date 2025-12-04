using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.FdTables.Campaigns;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FaceDominatorCore.FDLibrary.DAL
{

    public interface IDbCampaignServiceScoped : IDbCampaignService
    {

    }

    public class DbCampaignServiceScoped : DbCampaignService, IDbCampaignServiceScoped
    {
        public DbCampaignServiceScoped(string campaignId) :
            base(campaignId)
        {
        }
    }

    public interface IDbCampaignService
    {

        /*
                bool DoesInteractedPostsExist(string interactedPostId, ActivityType activityType);
        */

        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType);


        bool DoesInteractedUserExist(string interactedUserId, ActivityType activityType);

        bool DoesInteractedUserExistForAccount(string interactedUserId, ActivityType activityType, string AccountId);

        IReadOnlyCollection<InteractedUsers> GetInteractedUsers(ActivityType activityType);

        /*
                bool DoesInteractedPagesExist(string interactedPageId, ActivityType activityType);
        */

        /*
                IReadOnlyCollection<InteractedPages> GetInteractedPages(ActivityType activityType);
        */

        /*
                bool DoesInteractedGroupsExist(string interactedGroupUrl, ActivityType activityType);
        */

        /*
                IReadOnlyCollection<InteractedGroups> GetInteractedGroups(ActivityType activityType);      
        */

        bool DoesInteractedCommentsExist(string interactedCommentId, ActivityType activityType);

        /*
                IReadOnlyCollection<InteractedComments> GetInteractedComments(ActivityType activityType);
        */

        bool DoesInteractedPagesExist(string interactedPageId, string interactedAccountEmail, ActivityType activityType);

        bool DoesInteractedPostsExist(string interactedPostId, string postUrl, ActivityType activityType);

        void Add<T>(T obj) where T : class, new();

    }

    public class DbCampaignService : IDbCampaignService
    {
        private readonly IDbOperations _dbOperations;

        public DbCampaignService(JobProcess jobProcess)
        {
            //ResolveAccountDbOperations
            _dbOperations = InstanceProvider.ResolveCampaignDbOperations(jobProcess.CampaignId, SocialNetworks.Facebook);
            //_dbOperations = new DbOperations(jobProcess.CampaignId, SocialNetworks.Facebook,
            //    ConstantVariable.GetCampaignDb);
        }

        public DbCampaignService(string campaignId)
        {
            //_dbOperations = new DbOperations(campaignId, SocialNetworks.Facebook,
            //    ConstantVariable.GetCampaignDb);
            _dbOperations = InstanceProvider.ResolveCampaignDbOperations(campaignId ?? string.Empty, SocialNetworks.Facebook);
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(ActivityType activityType)
        {
            string activity = activityType.ToString();
            return _dbOperations.Get<InteractedPosts>(
                 x => x.ActivityType == activity);
        }


        public bool DoesInteractedPagesExist(string interactedPageId, string interactedAccountEmail, ActivityType activityType)
        {
            string activity = activityType.ToString();
            return _dbOperations.Any<InteractedPages>(x =>
                 x.PageId == interactedPageId && x.ActivityType == activity && x.AccountEmail == interactedAccountEmail);
        }

        public bool DoesInteractedPostsExist(string interactedPostId, string postUrl, ActivityType activityType)
        {
            string activity = activityType.ToString();
            return _dbOperations.Any<InteractedPosts>(x =>
            ((interactedPostId != "" && x.PostId == interactedPostId) || (postUrl != "" && x.PostUrl == postUrl) || (postUrl != "" && x.QueryValue.Contains(postUrl)))
                        && x.ActivityType == activity);
        }


        public bool DoesInteractedUserExist(string interactedUserId, ActivityType activityType)
        {
            string activity = activityType.ToString();
            return _dbOperations.Any<InteractedUsers>(x =>
                x.UserId == interactedUserId && x.ActivityType == activity);
        }

        public bool DoesInteractedUserExistForAccount(string interactedUserId, ActivityType activityType,
            string accountEmail)
        {
            string activity = activityType.ToString();
            return _dbOperations.Any<InteractedUsers>(x =>
                x.UserId == interactedUserId && x.ActivityType == activity && x.AccountEmail == accountEmail);
        }


        /*
                public bool DoesInteractedPostsExist(string interactedPostId, ActivityType activityType)
                {
                    string activity = activityType.ToString();
                    return _dbOperations.Any<InteractedPosts>(x =>
                        x.PostId == interactedPostId && x.ActivityType == activity);
                }
        */

        /*
                public bool DoesInteractedPagesExist(string interactedPageId, ActivityType activityType)
                {
                    string activity = activityType.ToString();
                    return _dbOperations.Any<InteractedPages>(x =>
                         x.PageId == interactedPageId && x.ActivityType == activity);
                }
        */


        /*
                public bool DoesInteractedGroupsExist(string interactedGroupUrl, ActivityType activityType)
                {
                    string activity = activityType.ToString();
                    return _dbOperations.Any<InteractedGroups>(x =>
                         x.GroupUrl == interactedGroupUrl && x.ActivityType == activity);
                }
        */




        public bool DoesInteractedCommentsExist(string interactedCommentId, ActivityType activityType)
        {
            string activity = activityType.ToString();
            return _dbOperations.Any<InteractedComments>(x =>
                 x.CommentUrl == interactedCommentId && x.ActivityType == activity);
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

        /*
                public IReadOnlyCollection<InteractedGroups> GetInteractedGroups(ActivityType activityType)
                {
                    string activity = activityType.ToString();
                    return _dbOperations.Get<InteractedGroups>(
                         x => x.ActivityType == activity);
                }
        */

        /*
                public IReadOnlyCollection<InteractedComments> GetInteractedComments(ActivityType activityType)
                {
                    string activity = activityType.ToString();
                    return _dbOperations.Get<InteractedComments>(
                         x => x.ActivityType == activity);
                }
        */

        public void Add<T>(T data) where T : class, new()
        {
            _dbOperations.Add(data);
        }

        public List<T> GetAllInteractedData<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return expression == null ? _dbOperations.Get<T>() : _dbOperations.Get(expression);
        }

        public int Count<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return _dbOperations.Count(expression);
        }



    }

}
