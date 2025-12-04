using System.Collections.Generic;
using DominatorHouseCore.DatabaseHandler.QdTables.Campaigns;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace QuoraDominatorCore.QdLibrary.DAL
{
    public interface IDbCampaignService
    {
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string user, ActivityType activityType);
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string sinAccUsername);
        IReadOnlyCollection<InteractedPosts> GetAllInteractedPosts();

        int GetCountOfInteractionForSpecificPost(ActivityType activityType, string post);

        bool DoesInteractedUserExist(string interactedUserId, ActivityType activityType);
        IReadOnlyCollection<InteractedUsers> GetAllInteractedUsers();
        IReadOnlyCollection<UnfollowedUsers> GetAllUnfollowedUsers();
        IReadOnlyCollection<InteractedMessage> GetInteractedMessage();
        void Add<T>(T data) where T : class, new();
        IReadOnlyCollection<InteractedAnswers> GetInteracteractedAnswers();
    }

    public class DbCampaignService : IDbCampaignService
    {
        private readonly DbOperations _dbOperations;

        public DbCampaignService(string campaignId)
        {
            _dbOperations = new DbOperations(campaignId, SocialNetworks.Quora, ConstantVariable.GetCampaignDb);
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string user, ActivityType activityType)
        {
            var activityTypeString = activityType.ToString();
            return _dbOperations.Get<InteractedPosts>(
                x => x.Username == user && x.ActivityType == activityTypeString);
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string sinAccUsername)
        {
            return _dbOperations.Get<InteractedPosts>(x => x.SinAccUsername == sinAccUsername);
        }

        public int GetCountOfInteractionForSpecificPost(ActivityType activityType, string post)
        {
            var activity = activityType.ToString();
            return _dbOperations.Count<InteractedQuestion>(x => x.ActivityType == activity && x.QuestionUrl == post);
        }

        public IReadOnlyCollection<InteractedPosts> GetAllInteractedPosts()
        {
            return _dbOperations.Get<InteractedPosts>();
        }

        public bool DoesInteractedUserExist(string interactedUserId, ActivityType activityType)
        {
            var activityTypeString = activityType.ToString();
            return _dbOperations.Any<InteractedUsers>(x =>
                x.InteractedUserId == interactedUserId && x.ActivityType == activityTypeString);
        }

        public IReadOnlyCollection<InteractedUsers> GetAllInteractedUsers()
        {
            return _dbOperations.Get<InteractedUsers>();
        }

        public IReadOnlyCollection<UnfollowedUsers> GetAllUnfollowedUsers()
        {
            return _dbOperations.Get<UnfollowedUsers>();
        }

        public IReadOnlyCollection<InteractedMessage> GetInteractedMessage()
        {
            return _dbOperations.Get<InteractedMessage>();
        }

        public void Add<T>(T data) where T : class, new()
        {
            _dbOperations.Add(data);
        }

        public IReadOnlyCollection<InteractedAnswers> GetInteracteractedAnswers()
        {
            return _dbOperations.Get<InteractedAnswers>();
        }

        public IReadOnlyCollection<InteractedAnswers> GetInteractedAnswers()
        {
            return _dbOperations.Get<InteractedAnswers>();
        }

        public IReadOnlyCollection<InteractedQuestion> GetInteractedQuestion()
        {
            return _dbOperations.Get<InteractedQuestion>();
        }
    }
}