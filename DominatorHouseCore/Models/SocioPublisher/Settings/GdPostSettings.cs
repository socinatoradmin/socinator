#region

using System;
using System.Collections.Generic;
using DominatorHouseCore.Interfaces.SocioPublisher;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher.Settings
{
    [Serializable]
    [ProtoContract]
    public class GdPostSettings : BindableBase, IGdPostSettings
    {
        private string _postTitle;
        private bool _isPostAsStoryPost;
        private bool _isDeletePostAfterHours;
        private int _deletePostAfterHours;
        private bool _isGeoLocation;
        private string _geoLocationList;
        private bool _isTagUser;
        private string _tagUserList;
        private bool _isLocationName = true;
        private bool _isLocationId;
        private bool _isCheckedCropMedia;
        private List<string> _MediaResolution = new List<string> { "Original", "1:1", "4:5", "16:9" };
        private string _SelectedResolution = "Original";
        [ProtoMember(1)]
        public string PostTitle
        {
            get => _postTitle;
            set
            {
                if (_postTitle == value)
                    return;
                SetProperty(ref _postTitle, value);
            }
        }

        [ProtoMember(2)]
        public bool IsPostAsStoryPost
        {
            get => _isPostAsStoryPost;
            set
            {
                if (_isPostAsStoryPost == value)
                    return;
                SetProperty(ref _isPostAsStoryPost, value);
            }
        }

        [ProtoMember(3)]
        public bool IsDeletePostAfterHours
        {
            get => _isDeletePostAfterHours;
            set
            {
                if (_isDeletePostAfterHours == value)
                    return;

                SetProperty(ref _isDeletePostAfterHours, value);
            }
        }

        [ProtoMember(4)]
        public int DeletePostAfterHours
        {
            get => _deletePostAfterHours;
            set
            {
                if (_deletePostAfterHours == value)
                    return;

                SetProperty(ref _deletePostAfterHours, value);
            }
        }

        [ProtoMember(5)]
        public bool IsGeoLocation
        {
            get => _isGeoLocation;
            set
            {
                if (_isGeoLocation == value)
                    return;

                SetProperty(ref _isGeoLocation, value);
            }
        }

        [ProtoMember(6)]
        public string GeoLocationList
        {
            get => _geoLocationList;
            set
            {
                if (_geoLocationList == value)
                    return;

                SetProperty(ref _geoLocationList, value);
            }
        }

        [ProtoMember(7)]
        public bool IsTagUser
        {
            get => _isTagUser;
            set
            {
                if (_isTagUser == value)
                    return;

                SetProperty(ref _isTagUser, value);
            }
        }

        [ProtoMember(8)]
        public string TagUserList
        {
            get => _tagUserList;
            set
            {
                if (_tagUserList == value)
                    return;

                SetProperty(ref _tagUserList, value);
            }
        }

        [ProtoMember(9)]
        public bool IsGeoLocationName
        {
            get => _isLocationName;
            set
            {
                SetProperty(ref _isLocationName, value);
                if(!value)
                    GeoLocationList = string.Empty;
            }
        }

        [ProtoMember(10)]
        public bool IsGeoLocationId
        {
            get => _isLocationId;
            set
            {
                SetProperty(ref _isLocationId, value);
                if (!value)
                    GeoLocationList = string.Empty;
            }
        }

        private bool _IsMentionUser;

        [ProtoMember(11)]
        public bool IsMentionUser
        {
            get => _IsMentionUser;
            set
            {
                if (value)
                    _IsMentionUser = false;

                SetProperty(ref _IsMentionUser, value);
            }
        }

        private string _MentionUserList;

        [ProtoMember(12)]
        public string MentionUserList
        {
            get => _MentionUserList;
            set
            {
                if (_MentionUserList == value)
                    return;

                SetProperty(ref _MentionUserList, value);
            }
        }
        private bool _isReelPost;
        [ProtoMember(13)]
        public bool IsReelPost
        {
            get => _isReelPost;
            set
            {
                if (_isReelPost == value)
                    return;

                SetProperty(ref _isReelPost, value);
            }
        }
        [ProtoMember(14)]
        public bool IsCheckedCropMedias
        {
            get => _isCheckedCropMedia;
            set
            {
                if (_isCheckedCropMedia == value)
                    return;

                SetProperty(ref _isCheckedCropMedia, value);
            }
        }
        [ProtoMember(15)]
        public List<string> MediaResolution
        {
            get => _MediaResolution;
            set
            {
                if (_MediaResolution == value)
                    return;

                SetProperty(ref _MediaResolution, value);
            }
        }
        [ProtoMember(16)]
        public string SelectedResolution
        {
            get => _SelectedResolution;
            set
            {
                if (_SelectedResolution == value)
                    return;

                SetProperty(ref _SelectedResolution, value);
            }
        }
    }
}