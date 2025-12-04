using DominatorHouseCore.Utility;
using ProtoBuf;

namespace TumblrDominatorCore.Models
{
    /// <summary>
    ///     PostCategory is used to define the post categories of instagram
    /// </summary>
    [ProtoContract]
    public class PostCategory : BindableBase
    {
        private bool _filterPostCategory;
        private bool _ignorePostImages;
        private bool _ignorePosttext;
        private bool _ignorePostVideos;

        [ProtoMember(1)]
        // Filter the post by its categories
        public bool FilterPostCategory
        {
            get => _filterPostCategory;
            set
            {
                if (value == _filterPostCategory)
                    return;
                SetProperty(ref _filterPostCategory, value);
            }
        }

        [ProtoMember(4)]
        // Should not include the albums
        public bool IgnorePostText
        {
            get => _ignorePosttext;
            set
            {
                if (value == _ignorePosttext)
                    return;
                SetProperty(ref _ignorePosttext, value);
            }
        }


        [ProtoMember(2)]
        // Should not include the videos
        public bool IgnorePostVideos
        {
            get => _ignorePostVideos;
            set
            {
                if (value == _ignorePostVideos)
                    return;
                SetProperty(ref _ignorePostVideos, value);
            }
        }

        [ProtoMember(2)]
        // Should not include the videos
        public bool IgnorePostImages
        {
            get => _ignorePostImages;
            set
            {
                if (value == _ignorePostImages)
                    return;
                SetProperty(ref _ignorePostImages, value);
            }
        }
    }
}