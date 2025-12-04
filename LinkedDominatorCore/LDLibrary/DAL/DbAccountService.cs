using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace LinkedDominatorCore.LDLibrary.DAL
{
    public interface IDbAccountService
    {
        IReadOnlyCollection<Connections> GetConnections();
        IReadOnlyCollection<Connections> GetNewConnections(long timeStampFromUserInput);
        IReadOnlyCollection<RemovedConnections> GetRemovedConnections();
        IReadOnlyCollection<InteractedUsers> GetConnectionRequestsSendFromSoftware();
        IReadOnlyCollection<Groups> GetGroups();
        IReadOnlyCollection<string> GetPendingConnectionRequestFromSoftware();
        IReadOnlyCollection<InteractedUsers> GetInteractedUsers(string activityType);
        IReadOnlyCollection<InteractedUsers> GetInteractedUserConnections();
        IReadOnlyCollection<InteractedGroups> GetInteractedGroups(string activityType);

        IReadOnlyCollection<InteractedPage> GetInteractedPages(string activityType);
        IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string activityType);
        IReadOnlyCollection<InteractedJobs> GetInteractedJobs(string activityType);
        IReadOnlyCollection<InteractedCompanies> GetInteractedCompanies(string activityType);
        IReadOnlyCollection<InteractedUsers> GetInteractedUsers(string activityType, string emailAddress);
        IReadOnlyCollection<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new();
        IReadOnlyCollection<Groups> GetJoinedGroups();
        IReadOnlyCollection<UnjoinedGroups> GetUnJoinedGroups();
        Connections GetSingleConnection(string profileUrl);
        InteractedUsers GetSingleInteractedUser(string activityType, int count);
        Groups GetSingleGroup(string groupUrl);
        InvitationsSent GetSingleInvitationSent(string profileUrl);

        T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class, new();
        int Count<T>(Expression<Func<T, bool>> expression = null) where T : class, new();
        bool Add<T>(T data) where T : class, new();
        bool AddRange<T>(List<T> data) where T : class, new();
        bool Update<T>(T data) where T : class, new();
        void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new();
        bool RemoveAll<T>() where T : class, new();
        Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> expression = null) where T : class, new();

        IReadOnlyCollection<SkipInteractedAttachments> GetSkippedInteractedAttachments();
    }

    public interface IDbAccountServiceScoped : IDbAccountService
    {
    }

    public class DbAccountServiceScoped : DbAccountService, IDbAccountServiceScoped
    {
        public DbAccountServiceScoped(IProcessScopeModel processScopeModel)
            : base(processScopeModel.Account)
        {
        }
    }

    public class DbAccountService : DbAccountServiceBase, IDbAccountService
    {
        private readonly IDbOperations _dbOperations;
        public DbAccountService(DominatorAccountModel dominatorAccountModel) : base(null)
        {
            try
            {
                if (!string.IsNullOrEmpty(dominatorAccountModel?.AccountId))
                    _dbOperations = InstanceProvider.ResolveAccountDbOperations(
                        dominatorAccountModel.AccountId, SocialNetworks.LinkedIn);
            }
            catch (Exception)
            {
                //
            }
        }


        public IReadOnlyCollection<Connections> GetConnections()
        {
            return _dbOperations.Get<Connections>().ToList();
        }

        public IReadOnlyCollection<Connections> GetNewConnections(long timeStampFromUserInput)
        {
            return _dbOperations.Get<Connections>(x => x.ConnectedTimeStamp >= timeStampFromUserInput);
        }

        public Connections GetSingleConnection(string profileUrl)
        {
            return _dbOperations.GetSingle<Connections>(x => x.ProfileUrl == profileUrl);
        }

        public IReadOnlyCollection<Groups> GetGroups()
        {
            return _dbOperations.Get<Groups>();
        }

        public IReadOnlyCollection<Groups> GetJoinedGroups()
        {
            return _dbOperations.Get<Groups>(x =>
                x.MembershipStatus == "MEMBER" || x.MembershipStatus == "OWNER" || x.MembershipStatus == "ADMIN");
        }

        public Groups GetSingleGroup(string groupUrl)
        {
            return _dbOperations.GetSingle<Groups>(x => x.GroupUrl == groupUrl);
        }

        public IReadOnlyCollection<UnjoinedGroups> GetUnJoinedGroups()
        {
            return _dbOperations.Get<UnjoinedGroups>();
        }

        public IReadOnlyCollection<RemovedConnections> GetRemovedConnections()
        {
            return _dbOperations.Get<RemovedConnections>().ToList();
        }

        public InvitationsSent GetSingleInvitationSent(string profileUrl)
        {
            return _dbOperations.GetSingle<InvitationsSent>(x => x.ProfileUrl == profileUrl);
        }

        public IReadOnlyCollection<InteractedUsers> GetConnectionRequestsSendFromSoftware()
        {
            var activityType = ActivityType.ConnectionRequest.ToString();
            return _dbOperations.Get<InteractedUsers>(x => x.ActivityType == activityType);
        }

        public IReadOnlyCollection<string> GetPendingConnectionRequestFromSoftware()
        {
            var activityType = ActivityType.ConnectionRequest.ToString();
            return _dbOperations.Get<InteractedUsers>(x => x.ActivityType == activityType)
                .Select(x => x.UserProfileUrl == "N/A" ? x.QueryValue : x.UserProfileUrl).ToList();
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUsers(string activityType)
        {
            return _dbOperations.Get<InteractedUsers>(x => x.ActivityType == activityType).ToList();
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUsers(string activityType, string emailAddress)
        {
            return _dbOperations.Get<InteractedUsers>(x =>
                x.ActivityType == activityType && x.QueryValue == emailAddress);
        }

        public IReadOnlyCollection<InteractedUsers> GetInteractedUserConnections()
        {
            var activityTpe = ActivityType.ConnectionRequest.ToString();
            return _dbOperations.Get<InteractedUsers>(x => x.ActivityType == activityTpe).ToList();
        }

        public InteractedUsers GetSingleInteractedUser(string activityType, int count)
        {
            return _dbOperations.GetSingle<InteractedUsers>(x => x.ActivityType == activityType && x.Id == count);
        }

        public IReadOnlyCollection<InteractedGroups> GetInteractedGroups(string activityType)
        {
            return _dbOperations.Get<InteractedGroups>(x => x.ActivityType == activityType).ToList();
        }

        public IReadOnlyCollection<InteractedPage> GetInteractedPages(string activityType)
        {
            return _dbOperations.Get<InteractedPage>(x => x.ActivityType == activityType).ToList();
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string activityType)
        {
            return _dbOperations.Get<InteractedPosts>(x => x.ActivityType == activityType).ToList();
        }

        public IReadOnlyCollection<InteractedJobs> GetInteractedJobs(string activityType)
        {
            return _dbOperations.Get<InteractedJobs>(x => x.ActivityType == activityType).ToList();
        }

        public IReadOnlyCollection<InteractedCompanies> GetInteractedCompanies(string activityType)
        {
            return _dbOperations.Get<InteractedCompanies>(x => x.ActivityType == activityType).ToList();
        }

        public int Count<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            try
            {
                return _dbOperations.Count(expression);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public IReadOnlyCollection<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            try
            {
                // Get the matched expression records, If expression is null returns full details
                return _dbOperations.Get(expression);
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        public async Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            try
            {
                // Get the matched expression records, If expression is null returns full details
                return await _dbOperations.GetAsync(expression);
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        public T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return _dbOperations.GetSingle(expression);
        }

        public bool Add<T>(T data) where T : class, new()
        {
            return _dbOperations.Add(data);
        }

        public bool AddRange<T>(List<T> data) where T : class, new()
        {
            return _dbOperations.AddRange(data);
        }

        public bool Update<T>(T data) where T : class, new()
        {
            return _dbOperations.Update(data);
        }

        public void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            _dbOperations.RemoveMatch(expression);
        }

        public bool RemoveAll<T>() where T : class, new()
        {
            return _dbOperations.RemoveAll<T>();
        }

        public IReadOnlyCollection<SkipInteractedAttachments> GetSkippedInteractedAttachments()
        {
            return _dbOperations.Get<SkipInteractedAttachments>();
        }

        public IReadOnlyCollection<InteractedPosts> GetInteractedPosts(string activityType,
            List<string> lstCommentInDom)
        {
            return _dbOperations.Get<InteractedPosts>(x =>
                x.ActivityType == activityType && lstCommentInDom.Any(y => y.ToLower() == x.MyComment.ToLower()));
        }
    }
}