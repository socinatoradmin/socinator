using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.Behaviours
{
    public class Lang
    {
        private static readonly string NumberOf = "LangKeyNumberOf".FromResourceDictionary();
        private static readonly string PerDay = "LangKeyPerDay".FromResourceDictionary();

        public static string GetPerJob(ActivityType activityType)
        {
            var PerJob = "LangKeyPerJob".FromResourceDictionary();
            return NumberOf + " " + GetlangActivity(activityType) + " " + PerJob;
        }

        public static string GetPerHour(ActivityType activityType)
        {
            var PerHour = "LangKeyPerHour".FromResourceDictionary();
            return NumberOf + " " + GetlangActivity(activityType) + " " + PerHour;
        }

        public static string GetPerDay(ActivityType activityType)
        {
            return NumberOf + " " + GetlangActivity(activityType) + " " + PerDay;
        }

        public static string GetPerWeek(ActivityType activityType)
        {
            var PerWeek = "LangKeyPerWeek".FromResourceDictionary();
            return NumberOf + " " + GetlangActivity(activityType) + " " + PerWeek;
        }

        public static string GetlangActivity(ActivityType activityType)
        {
            return $"LangKey{activityType}".FromResourceDictionary();
        }
    }
}