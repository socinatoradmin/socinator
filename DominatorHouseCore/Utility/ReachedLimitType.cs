#region

using System;

#endregion

namespace DominatorHouseCore.Utility
{
    public enum ReachedLimitType
    {
        Daily,
        Weekly,
        Job,
        Hourly,
        NoLimit
    }

    public static class ReachedLimitTypeExtension
    {
        public static string ConvertToLogRecord(this ReachedLimitType reachedLimitType)
        {
            switch (reachedLimitType)
            {
                case ReachedLimitType.Daily:
                    return Log.DailyLimitReached;
                case ReachedLimitType.Weekly:
                    return Log.WeeklyLimitReached;
                case ReachedLimitType.Job:
                    return Log.JobLimitReached;
                case ReachedLimitType.Hourly:
                    return Log.HourlyLimitReached;
                case ReachedLimitType.NoLimit:
                    return string.Empty;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reachedLimitType), reachedLimitType, null);
            }
        }
    }
}