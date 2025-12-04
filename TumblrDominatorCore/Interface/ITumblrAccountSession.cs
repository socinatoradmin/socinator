using DominatorHouseCore.Models;

namespace TumblrDominatorCore.Interface
{
    public interface ITumblrAccountSession
    {
        void AddOrUpdateSession(ref DominatorAccountModel dominatorAccount, bool Update = false);
        void InitializeAllSession();
    }
}
