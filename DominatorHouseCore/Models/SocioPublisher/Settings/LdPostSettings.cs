#region

using System;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.Interfaces.SocioPublisher;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher.Settings
{
    [Serializable]
    [ProtoContract]
    public class LdPostSettings : BindableBase, ILdPostSettings
    {
        private LdGroupPostType _groupPostType = LdGroupPostType.General;

        [ProtoMember(1)]
        public LdGroupPostType GroupPostType
        {
            get => _groupPostType;
            set
            {
                if (_groupPostType == value)
                    return;

                SetProperty(ref _groupPostType, value);
            }
        }

        private bool _isJobPost;

        [ProtoMember(2)]
        public bool IsJobPost
        {
            get => _isJobPost;
            set
            {
                if (_isJobPost == value)
                    return;

                SetProperty(ref _isJobPost, value);
            }
        }

        private bool _isGeneralPost;

        [ProtoMember(3)]
        public bool IsGeneralPost
        {
            get => _isGeneralPost;
            set
            {
                if (_isGeneralPost == value)
                    return;

                SetProperty(ref _isGeneralPost, value);
            }
        }

        private bool _isAnnouncementPost;

        [ProtoMember(4)]
        public bool IsAnnouncementPost
        {
            get => _isAnnouncementPost;
            set
            {
                if (_isAnnouncementPost == value)
                    return;

                SetProperty(ref _isAnnouncementPost, value);
            }
        }

        private bool _isDocTypePosts;

        [ProtoMember(5)]
        public bool IsDocTypePosts
        {
            get => _isDocTypePosts;
            set
            {
                if (_isDocTypePosts == value)
                    return;

                SetProperty(ref _isDocTypePosts, value);
            }
        }
    }
}