using DominatorHouseCore.Utility;
using ProtoBuf;

namespace YoutubeDominatorCore.Settings
{
    [ProtoContract]
    public class UserCategory : BindableBase
    {
        private bool _filterUserCategory;
        private bool _ignoreUserAlbums;
        private bool _ignoreUserImages;
        private bool _ignoreUserVideos;

        [ProtoMember(1)]
        // Filter the User by its categories
        public bool FilterUserCategory
        {
            get => _filterUserCategory;
            set
            {
                if (value == _filterUserCategory)
                    return;
                SetProperty(ref _filterUserCategory, value);
            }
        }

        [ProtoMember(4)]
        // Should not include the albums
        public bool IgnoreUserAlbums
        {
            get => _ignoreUserAlbums;
            set
            {
                if (value == _ignoreUserAlbums)
                    return;
                SetProperty(ref _ignoreUserAlbums, value);
            }
        }

        [ProtoMember(3)]
        // Should not include the images
        public bool IgnoreUserImages
        {
            get => _ignoreUserImages;
            set
            {
                if (value == _ignoreUserImages)
                    return;
                SetProperty(ref _ignoreUserImages, value);
            }
        }

        [ProtoMember(2)]
        // Should not include the videos
        public bool IgnoreUserVideos
        {
            get => _ignoreUserVideos;
            set
            {
                if (value == _ignoreUserVideos)
                    return;
                SetProperty(ref _ignoreUserVideos, value);
            }
        }
    }
}