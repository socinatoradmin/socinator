using DominatorHouseCore.Utility;
using ProtoBuf;

namespace QuoraDominatorCore.Settings
{
    public class PostCategory : BindableBase
    {
        private bool _filterPostCategory;
        private bool _ignorePostAlbums;
        private bool _ignorePostImages;
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
        public bool IgnorePostAlbums
        {
            get => _ignorePostAlbums;
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
            get => _ignorePostImages;
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
            get => _ignorePostVideos;
            set
            {
                if (value == _ignorePostVideos)
                    return;
                SetProperty(ref _ignorePostVideos, value);
            }
        }
    }
}