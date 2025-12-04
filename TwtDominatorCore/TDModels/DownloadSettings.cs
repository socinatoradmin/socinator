using DominatorHouseCore.Utility;
using ProtoBuf;

namespace TwtDominatorCore.TDModels
{
    [ProtoContract]
    public class DownloadSettings : BindableBase
    {
        private bool _isChkCategoryAll = true;
        private bool _isChkCategoryImage;
        private bool _isChkCategoryVideo;
        private bool _isChkQualityHigh = true;
        private bool _isChkQualityNormal;

        [ProtoMember(1)]
        public bool IsChkCategoryAll
        {
            get => _isChkCategoryAll;
            set
            {
                if (_isChkCategoryAll == value)
                    return;
                SetProperty(ref _isChkCategoryAll, value);
            }
        }

        [ProtoMember(2)]
        public bool IsChkCategoryImage
        {
            get => _isChkCategoryImage;
            set
            {
                if (_isChkCategoryImage == value)
                    return;
                SetProperty(ref _isChkCategoryImage, value);
            }
        }

        [ProtoMember(3)]
        public bool IsChkCategoryVideo
        {
            get => _isChkCategoryVideo;
            set
            {
                if (_isChkCategoryVideo == value)
                    return;
                SetProperty(ref _isChkCategoryVideo, value);
            }
        }

        [ProtoMember(4)]
        public bool IsChkQualityHigh
        {
            get => _isChkQualityHigh;
            set
            {
                if (_isChkQualityHigh == value)
                    return;
                SetProperty(ref _isChkQualityHigh, value);
            }
        }

        [ProtoMember(5)]
        public bool IsChkQualityNormal
        {
            get => _isChkQualityNormal;
            set
            {
                if (_isChkQualityNormal == value)
                    return;
                SetProperty(ref _isChkQualityNormal, value);
            }
        }
    }
}