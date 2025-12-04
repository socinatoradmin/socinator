using DominatorHouseCore.Models;

namespace RedditDominatorCore.Interface
{
    public interface IRDAccountSessionManager
    {
        void AddOrUpdateSession(ref DominatorAccountModel dominatorAccount, bool Update = false);
        void InitializeAllSession();
    }
}
