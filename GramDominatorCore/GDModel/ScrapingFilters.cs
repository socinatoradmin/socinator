using System;
using GramDominatorCore.GDEnums;
using ProtoBuf;

namespace GramDominatorCore.GDModel
{
    [ProtoContract]
    public class ScrapingFilters : IEquatable<ScrapingFilters>
    {
        [ProtoMember(1)]
        public ScrapingParameters Filter { get; set; }

        [ProtoMember(2)]
        public string Value { get; }

      //  public string ValueReadable => GetFilterDetails(Filter).Item2(Value);

        //[return: TupleElementNames(new[] { "title", "conversion", "showMessage" })]
        //public static ValueTuple<string, Func<string, string>, bool> GetFilterDetails(ScrapingParameters filter)
        //{
        //    string str;
        //    Func<string, string> func;
        //    bool flag;
        //    switch (filter)
        //    {
        //        case ScrapingParameters.Hashtag:
        //             str = DominatorHouseCore.Resources.ScrapingFilters_GetFilterDetails_Enter_a_hastag;
        //            func = x => "#" + x;
        //            flag = true;
        //            break;
        //        case ScrapingParameters.UsersFollowers:
        //            str = DominatorHouseCore.Resources.ScrapingFilters_GetFilterDetails_Enter_a_username;
        //            func = x => x;
        //            flag = true;
        //            break;
        //        case ScrapingParameters.UsersFollowing:
        //            str = DominatorHouseCore.Resources.ScrapingFilters_GetFilterDetails_Enter_a_username;
        //            func = x => x;
        //            flag = true;
        //            break;
        //        case ScrapingParameters.FollowBack:
        //            str = string.Empty;
        //            func = x => x;
        //            flag = false;
        //            break;
        //        case ScrapingParameters.SingleUser:
        //            str = DominatorHouseCore.Resources.ScrapingFilters_GetFilterDetails_Enter_a_username;
        //            func = x => x;
        //            flag = true;
        //            break;
        //        case ScrapingParameters.UsersCommented:
        //            str = DominatorHouseCore.Resources.ScrapingFilters_GetFilterDetails_Enter_a_Instagram_url__https___www_instagram_com_p____;
        //            func = x => x;
        //            flag = true;
        //            break;
        //        case ScrapingParameters.UsersLiked:
        //            str = DominatorHouseCore.Resources.ScrapingFilters_GetFilterDetails_Enter_a_Instagram_url__https___www_instagram_com_p____;
        //            func = x => x;
        //            flag = true;
        //            break;
        //        case ScrapingParameters.RecentActivity:
        //            str = DominatorHouseCore.Resources.ScrapingFilters_GetFilterDetails_Enter_a_username_and_timespan___Username_hour_minute_;
        //            func = x => x;
        //            flag = true;
        //            break;
        //        case ScrapingParameters.SuggestedUsers:
        //            str = DominatorHouseCore.Resources.ScrapingFilters_GetFilterDetails_Enter_a_username;
        //            func = x => x;
        //            flag = true;
        //            break;
        //        case ScrapingParameters.Location:
        //            str = DominatorHouseCore.Resources.ScrapingFilters_GetFilterDetails_Enter_a_Instagram_url__https___www_instagram_com_explore_locations____;
        //            func = x => x;
        //            flag = true;
        //            break;
        //        case ScrapingParameters.Timeline:
        //            str = string.Empty;
        //            func = x => x;
        //            flag = false;
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
        //    }
        //    return new ValueTuple<string, Func<string, string>, bool>(str, func, flag);
        //}

        public bool Equals(ScrapingFilters other)
        {
            if (other == null)
                return false;
            if (Equals(other))
                return true;
            if (Filter == other.Filter)
                return string.Equals(Value, other.Value);
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (this == obj)
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((ScrapingFilters)obj);
        }

        public override int GetHashCode()
        {
            return (int)Filter * 397 ^ (Value != null ? Value.GetHashCode() : 0);
        }
    }
}
