using DominatorHouseCore.Utility;
using ProtoBuf;

namespace GramDominatorCore.Settings
{
    /// <summary>
    /// PostCategory is used to define the post categories of instagram
    /// </summary>
    [ProtoContract]
    public class PostCategory:BindableBase
    {
        private bool _filterPostCategory;
        private bool _ignorePostAlbums;
        private bool _ignorePostImages;
        private bool _ignorePostVideos;

        [ProtoMember(1)]
        // Filter the post by its categories
        public bool FilterPostCategory
        {
            get { return _filterPostCategory; }
            set
            {
                if (value == _filterPostCategory)
                    return;
                SetProperty(ref _filterPostCategory, value);
            }
        }

        [ProtoMember(4)]
        // Should not include the albums
        public bool IgnorePostAlbums
        {
            get { return _ignorePostAlbums; }
            set
            {
                if (value == _ignorePostAlbums)
                    return;
                SetProperty(ref _ignorePostAlbums, value);
            }
        }

        [ProtoMember(3)]
        // Should not include the images
        public bool IgnorePostImages
        {
            get { return _ignorePostImages; }
            set
            {
                if (value == _ignorePostImages)
                    return;
                SetProperty(ref _ignorePostImages, value);
            }
        }

        [ProtoMember(2)]
        // Should not include the videos
        public bool IgnorePostVideos
        {
            get { return _ignorePostVideos; }
            set
            {
                if (value == _ignorePostVideos)
                    return;
                SetProperty(ref _ignorePostVideos, value);
            }
        }
    }
}
