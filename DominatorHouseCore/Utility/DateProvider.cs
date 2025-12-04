#region

using System;

#endregion

namespace DominatorHouseCore.Utility
{
    public interface IDateProvider
    {
        DateTime UtcNow();
        DateTime Now();
        DateTime Today();
        int GetTimezoneOffset();
        long UtcNowUnix();
    }

    public sealed class DateProvider : IDateProvider
    {
        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
        public long UtcNowUnix()
        {
            return (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public DateTime Now()
        {
            return DateTime.Now;
        }

        public DateTime Today()
        {
            return DateTime.Today;
        }
        public int GetTimezoneOffset()
        {
            return (int)TimeZoneInfo.Local.GetUtcOffset(Now()).TotalSeconds;
        }
    }
}