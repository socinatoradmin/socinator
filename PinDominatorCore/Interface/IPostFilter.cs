using System.Collections.ObjectModel;
using DominatorHouseCore.Utility;
using PinDominatorCore.Settings;

namespace PinDominatorCore.Interface
{
    public interface IPostFilter
    {
        // Posts caption should contains minimum specified count
        int MinimumPostCaptionChars { get; set; }


        //Filter by post should not contains the restricted words 
        bool FilterRestrictedPostCaptionList { get; set; }


        // Post should not contains the restricted words
        ObservableCollection<string> RestrictedPostCaptionList { get; set; }


        bool FilterAcceptedPostCaptionList { get; set; }

        ObservableCollection<string> AcceptedPostCaptionList { get; set; }

        // Filter the posts comment should be lies between the specified range
        bool FilterComments { get; set; }


        // Posts comment should be lies between the specified range
        RangeUtilities CommentsCountRange { get; set; }

        // Posts Like should be lies between the specified range
        RangeUtilities LikesCountRange { get; set; }

        // Filter the posts likes should be lies between the specified range
        bool FilterTried { get; set; }


        // Posts likes should be lies between the specified range
        RangeUtilities TriedCountRange { get; set; }


        // Filter the post which should be posted on recent days
        bool FilterPostAge { get; set; }


        // Post which should be posted on recent days
        int MaxPostAge { get; set; }


        // Filter the post with their type such as albums, images and videos
        PostCategory PostCategory { get; set; }


        bool IgnoreTopPost { get; set; }
    }
}