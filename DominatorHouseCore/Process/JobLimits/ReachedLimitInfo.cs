#region

using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Process.JobLimits
{
    public struct ReachedLimitInfo
    {
        public ReachedLimitType ReachedLimitType { get; }
        public int LimitValue { get; }

        public ReachedLimitInfo(ReachedLimitType reachedLimitType, int limitValue)
        {
            ReachedLimitType = reachedLimitType;
            LimitValue = limitValue;
        }
    }
}