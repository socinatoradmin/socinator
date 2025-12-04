using System.Collections.Generic;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;

namespace QuoraDominatorCore.Interface
{
    public interface IUserFilter
    {
        // Ignore the user who does not contain profile picture
        bool IgnoreNoProfilePicUsers { get; set; }

        bool FilterUserHasInvalidWord { get; set; }

        List<string> UserNameRestrictedWords { get; set; }

        // Ignore the user who is followers for current accounts
        bool IgnoreCurrentUsersFollowers { get; set; }

        // User filter by gender wise
        GenderFilter GenderFilters { get; set; }

        // Ignore the user who is not an english user
        bool IgnoreNonEnglishUser { get; set; }

        // Filter by restricted group list 
        BlacklistSettings RestrictedGrouplist { get; set; }

        // Filter by restricted profile list 
        BlacklistSettings RestrictedProfileList { get; set; }

        bool SaveCloseButtonVisible { get; set; }

        #region Filter Users By Answers Count

        bool FilterAnswersCount { get; set; }
        RangeUtilities AnswersCount { get; set; }

        #endregion

        #region Filter Users By Questions Count

        bool FilterQuestionsCount { get; set; }
        RangeUtilities QuestionsCount { get; set; }

        #endregion

        // Filter the user by whose post count lies between specified range

        #region Filter Users By Posts Count

        bool FilterPostsCounts { get; set; }

        // User by whose post count lies between specified range
        RangeUtilities PostCounts { get; set; }

        #endregion

        #region Filter Users By Blogs Count

        bool FilterBlogsCounts { get; set; }

        // User by whose post count lies between specified range
        RangeUtilities BlogsCounts { get; set; }

        #endregion

        #region Filter Users By Topics Count

        bool FilterTopicsCounts { get; set; }

        // User by whose post count lies between specified range
        RangeUtilities TopicsCounts { get; set; }

        #endregion

        #region Filter Users By Edits Count

        bool FilterEditsCounts { get; set; }

        // User by whose post count lies between specified range
        RangeUtilities EditsCounts { get; set; }

        #endregion

        #region Filter Users By Answer Views Count

        bool FilterAnswerViewsCounts { get; set; }

        // User by whose post count lies between specified range
        RangeUtilities AnswerViewsCounts { get; set; }

        #endregion

        #region Filter Users Which Are Have Not Lived In Mentioned Places

        // Is Filter by the user who should not contain restrict words(BlackListWords)
        bool FilterBlacklistedLivesIn { get; set; }


        // Collection of words for Filter by the user who should not contain restricted words(BlackListWords)
        List<string> BlacklistedLivesInPlaces { get; set; }

        #endregion

        #region Filter Users Which Are Have Not Studied In Mentioned Places

        // Is Filter by the user who should not contain restiricted words(BlackListWords)
        bool FilterBlacklistedStudiedPlaces { get; set; }


        // Collection of words for Filter by the user who should not contain restiricted words(BlackListWords)
        List<string> BlacklistedStudiedPlaces { get; set; }

        #endregion

        #region Filter Users Who has not worked In Mentioned Places

        // Is Filter by the user who should not contain restiricted words(BlackListWords)
        bool FilterBlacklistedWorkPlaces { get; set; }


        // Collection of words for Filter by the user who should not contain restiricted words(BlackListWords)
        List<string> BlacklistedWorkPlaces { get; set; }

        #endregion

        #region Filter By Minimum Characters in Bio Description

        bool FilterMinimumCharacterInBio { get; set; }

        // In User Bio graphy should contain minimum count of character
        int MinimumCharacterInBio { get; set; }

        #endregion

        // Is Filter by the user who should contains minimum to maximum specific following counts

        #region Filter By Following Count

        bool FilterFollowingsCount { get; set; }


        //User following counts should be lies between specific range values   
        RangeUtilities FollowingsCount { get; set; }

        #endregion

        // Is Filter by the user who should contains minimum to maximum specific followers counts

        #region Filter By Followers Count

        bool FilterFollowersCount { get; set; }


        //User followers counts should be lies between specific range values   
        RangeUtilities FollowersCount { get; set; }

        #endregion

        #region Filter Users Containing Blacklisted Words In Bio Description

        // Is Filter by the user who should not contain restiricted words(BlackListWords)
        bool FilterBioRestrictedWords { get; set; }

        //bool FilterUsernameRestrictedWords { get; set; }

        //bool FilterWorkRestrictedWords { get; set; }

        //bool FilterStudyRestrictedWords { get; set; }

        //bool FilterLocationRestrictedWords { get; set; }


        // Collection of words for Filter by the user who should not contain restiricted words(BlackListWords)
        List<string> BioRestrictedWords { get; set; }

        #endregion

        #region Filter with Follow Ratio Range

        // Filter the user from their (Follower/Following) ratio should exactly equal to specified ratio range
        bool FilterSpecificFollowRatio { get; set; }


        // User from their (Follower/Following) ratio should exactly equal to specified  ratio range
        RangeUtilities SpecificFollowRatio { get; set; }

        #endregion

        #region To Filter users who has not answered in recent x-y days

        // Is Filter by user who Answered on recent days
        bool FilterAnsweredInRecentDays { get; set; }


        // User who should recently posted on specific days 
        RangeUtilities AnsweredInRecentDays { get; set; }

        #endregion
    }
}