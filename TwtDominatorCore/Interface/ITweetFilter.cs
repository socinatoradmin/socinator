using System.Collections.Generic;
using DominatorHouseCore.Utility;

namespace TwtDominatorCore.Interface
{
    public interface ITweetFilter
    {
        bool IsFilterTweetsLessThanSpecificHoursOld { get; set; }
        bool IsFilterTweetsLessThanSpecificDaysOld { get; set; }
        bool IsFilterTweetsHaveBetweenRetweets { get; set; }
        bool IsFilterTweetsHaveBetweenFavorites { get; set; }
        bool IsFilterSkipTweetsContainingSpecificWords { get; set; }
        bool IsFilterSkipTweetsContainingAtSign { get; set; }
        bool IsFilterSkipRetweets { get; set; }
        bool IsFilterSkipTweetsWithLinks { get; set; }
        bool IsFilterSkipTweetsWithoutLinks { get; set; }
        bool IsFilterSkipTweetsContainNonEnglishChar { get; set; }
        bool IsFilterUseRealTimeResults { get; set; }
        bool IsFilterAlreadyLiked { get; set; }
        bool IsFilterAlreadyRetweeted { get; set; }
        int FilterTweetsLessThanSpecificDaysOldValue { get; set; }
        int FilterTweetsLessThanSpecificHoursOldValue { get; set; }
        RangeUtilities FilterTweetsHaveBetweenRetweets { get; set; }
        RangeUtilities FilterTweetsHaveBetweenFavorites { get; set; }

        string SkipTweetsContainingWords { get; set; }

        List<string> LstSkipTweetsContainingWords { get; set; }
    }
}