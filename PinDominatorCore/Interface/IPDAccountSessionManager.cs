using DominatorHouseCore.Models;

namespace PinDominatorCore.Interface
{
    public interface IPDAccountSessionManager
    {
        void AddOrUpdateSession(ref DominatorAccountModel dominatorAccount, bool Update = false);
        void InitializeAllSession();
    }
}
