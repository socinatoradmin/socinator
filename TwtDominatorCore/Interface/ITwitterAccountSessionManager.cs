using DominatorHouseCore.Models;

namespace TwtDominatorCore.Interface
{
    public interface ITwitterAccountSessionManager
    {
        void AddOrUpdateSession(ref DominatorAccountModel dominatorAccount, bool Update = false);
        void InitializeAllSession();
    }
}
