#region

using DominatorHouseCore.Enums;
using System;
using System.Threading.Tasks;

#endregion

namespace DominatorHouseCore.Extensions
{
    public static class AggregateExceptionExtensions
    {
        public static void HandleOperationCancellation(this AggregateException ae)
        {
            foreach (var e in ae.InnerExceptions)
                if (e is TaskCanceledException || e is OperationCanceledException)
                    throw new OperationCanceledException(@"Cancellation Requested !", e);
                else
                    e.DebugLog(e.StackTrace + e.Message);
        }
    }
    public static class EnumsUtils
    {
        public static ActivityType ToActivityType(this string activityType)
        {
            if (Enum.TryParse(activityType, true, out ActivityType result))
                return result;
            throw new ArgumentException($"Invalid activity type: {activityType}");
        }
        public static SocialNetworks ToSocialNetwork(this string socialNetwork)
        {
            if (Enum.TryParse(socialNetwork, true, out SocialNetworks result))
                return result;
            throw new ArgumentException($"Invalid social network: {socialNetwork}");
        }
    }
}