using System.Collections.Generic;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;

namespace GramDominatorCore.Interface
{
    public interface IUserFilter 
    {

        // Ignore the user who doesnot contain profile picture
        bool IgnoreNoProfilePicUsers { get; set; }

        // Filter the user by whose post count lies between specified range
        bool FilterPostCounts { get; set; }

        // User by whose post count lies between specified range
        RangeUtilities PostCounts { get; set; }

        bool FilterMinimumCharacterInBio { get; set; }

        // In User Bio graphy should contain minimum count of character
        int MinimumCharacterInBio { get; set; }



        // Is Filter by the user who should contains minimum to maximum specific following counts
        bool FilterFollowingsCount { get; set; }


        //User following counts should be lies between specific range values   
        RangeUtilities FollowingsCount { get; set; }


        // Is Filter by the user who should contains minimum to maximum specific followers counts
        bool FilterFollowersCount { get; set; }


        //User followers counts should be lies between specific range values   
        RangeUtilities FollowersCount { get; set; }



        // Is Filter by the user who should not contain restiricted words(BlackListWords)
        bool FilterBioRestrictedWords { get; set; }


        // Collection of words for Filter by the user who should not contain restiricted words(BlackListWords)
      //  ObservableCollection<string> BioRestrictedWords { get; set; }


      //  bool UserHasInvalidWord { get; set; }

        List<string> LstInvalidWord { get; set; }


        // Filter the user from their (Follower/Following) ratio should lesser than specified maximum ratio range
        bool FilterMaximumFollowRatio { get; set; }


        // User from their (Follower/Following) ratio should lesser than specified maximum ratio range
        double MaximumFollowRatio { get; set; }


        // Filter the user from their (Follower/Following) ratio should greater than specified minimum ratio range
        bool FilterMinimumFollowRatio { get; set; }


        // User from their (Follower/Following) ratio should greater than specified minimum ratio range
        double MinimumFollowRatio { get; set; }


        // Filter the user from their (Follower/Following) ratio should exactly equal to specified ratio range
        bool FilterSpecificFollowRatio { get; set; }


        // User from their (Follower/Following) ratio should exactly equal to specified  ratio range
       // RangeUtilities SpecificFollowRatio { get; set; }




        // Is Filter by user who should posted on recent days
        bool FilterPostedInRecentDays { get; set; }


        // User who should recently posted on specific days 
        int PostedInRecentDays { get; set; }

        // Is Filter by user who should not posted on recent days
        bool FilterNotPostedInRecentdDays { get; set; }


        // User who should not recently posted on specific days 
        int NotPostedInRecentDays { get; set; }


        // Ignore the user who is followers for current accounts
        //bool IgnoreCurrentUsersFollowers { get; set; }


        // Ignore the user who is the private user
       // bool IgnorePrivateUser { get; set; }


        // User filter by gender wise
        GenderFilter GenderFilters { get; set; }


        // Ignore the user who is not an english user
        bool IgnoreNonEnglishUser { get; set; }


        // Ignore the user who is the business user
       // bool IgnoreBusinessUser { get; set; }

        // Ignore the user who is the verified user
       // bool IgnoreVerifiedUser { get; set; }

        // Filter by restricted group list 
        BlacklistSettings RestrictedGrouplist { get; set; }

        // Filter by restricted profile list 
        BlacklistSettings RestrictedProfilelist { get; set; }

//List<string> LstPostCaption { get; set; }

        bool FilterInvaildWord { get; set; }


        // bool SaveCloseButtonVisible { get; set; }

        bool FilterBioNotRestrictedWords { get; set; }

        List<string> LstvalidWord { get; set; }

    }
}