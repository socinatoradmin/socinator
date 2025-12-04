using System.Collections.Generic;
using DominatorHouseCore.Utility;

namespace TwtDominatorCore.Interface
{
    public interface IUserFilter
    {
        bool IsFilterEnglishCharactersInBio { get; set; }
        bool IsFilterProfileImage { get; set; }
        bool IsFilterMinimumNumberOfTweets { get; set; }
        bool IsFilterFollowersRange { get; set; }
        bool IsFilterFollowingsRange { get; set; }
        bool IsFilterFollowRatioGreaterThan { get; set; }
        bool IsFilterFollowRatioSmallerThan { get; set; }
        bool IsFilterBioHasMinimumCharacters { get; set; }
        bool IsFilterMustNotContainInvalidWords { get; set; }
        bool IsFilterMustContainSpecificWords { get; set; }
        bool IsFilterTweetedWithinLastDays { get; set; }
        bool IsFilterUserIsNotFollowingThisAccount { get; set; }
        bool IsFilterPrivateUser { get; set; }
        int FilterMinimumNumberOfTweetsValue { get; set; }
        RangeUtilities FilterFollowersRange { get; set; }
        RangeUtilities FilterFollowingsRange { get; set; }
        int FilterFollowRatioGreaterThanValue { get; set; }
        int FilterFollowRatioSmallerThanValue { get; set; }
        int FilterMinimumCharactersValue { get; set; }
        int FilterTweetedWithinTheLastValue { get; set; }
        string InvalidWordsInBio { get; set; }
        string SpecificWordsInBio { get; set; }
        string SpecificAccountNotFollowing { get; set; }
        List<string> LstInvalidWords { get; set; }
        List<string> LstSpecificWords { get; set; }
        List<string> LstSpecificAccountNotFollowing { get; set; }
        bool SaveCloseButtonVisible { get; set; }
        bool IsFilterMutedUser { get; set; }
        bool IsFilterByVerification { get; set; }
        bool IsFilterActiveVerfiedUser { get; set; }
        bool IsFilterActiveWhoAreNotVerified { get; set; }
    }
}