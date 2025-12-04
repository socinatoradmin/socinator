using System.Collections.ObjectModel;
using QuoraDominatorCore.Settings;

namespace QuoraDominatorCore.Interface
{
    public interface IPostFilter
    {
        ObservableCollection<string> RestrictedPostCaptionList { get; set; }


        bool FilterAcceptedPostCaptionList { get; set; }

        ObservableCollection<string> AcceptedPostCaptionList { get; set; }

        // Filter the posts comment should be lies between the specified range
        bool FilterComments { get; set; }

        // Filter the posts likes should be lies between the specified range
        bool FilterLikes { get; set; }

        // Filter the post which should be posted on recent days
        bool FilterPostAge { get; set; }


        // Post which should be posted on recent days
        int MaxPostAge { get; set; }


        // Filter the post with their type such as albums, images and videos
        PostCategory PostCategory { get; set; }
        bool IgnoreAdPost { get; set; }
    }
}