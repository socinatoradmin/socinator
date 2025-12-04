using DominatorHouseCore.Models;

namespace QuoraDominatorCore.Interface
{
    public interface IQDSessionManager
    {
        void AddOrUpdateSession(ref DominatorAccountModel dominatorAccount, bool Update = false);
        void InitializeAllSession();
    }
}
