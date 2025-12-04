#region

using System;
using DominatorHouseCore.Interfaces.SocioPublisher;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher.Settings
{
    [Serializable]
    [ProtoContract]
    public class FdPostSettings : BindableBase, IFdPostSettings
    {
        private bool _isPostAsStoryPost;
        private bool _sellPostSelected;
        private string _productName = string.Empty;
        private int _productPrice;
        private string _productAvailableLocation = string.Empty;
        private bool _sellPostTurnOffComment;
        private bool _isReplaceDescriptionSelected;
        private string _postReplaceDescription = string.Empty;

        [ProtoMember(1)]
        public bool SellPostSelected
        {
            get => _sellPostSelected;
            set
            {
                if (_sellPostSelected == value)
                    return;

                SetProperty(ref _sellPostSelected, value);
            }
        }

        [ProtoMember(2)]
        public string ProductName
        {
            get => _productName;
            set
            {
                if (_productName == value)
                    return;

                SetProperty(ref _productName, value);
            }
        }

        [ProtoMember(3)]
        public int ProductPrice
        {
            get => _productPrice;
            set => SetProperty(ref _productPrice, value);
        }

        [ProtoMember(4)]
        public string ProductAvailableLocation
        {
            get => _productAvailableLocation;
            set
            {
                if (_productAvailableLocation == value)
                    return;

                SetProperty(ref _productAvailableLocation, value);
            }
        }

        [ProtoMember(5)]
        public bool SellPostTurnOffComment
        {
            get => _sellPostTurnOffComment;
            set
            {
                if (_sellPostTurnOffComment == value)
                    return;

                SetProperty(ref _sellPostTurnOffComment, value);
            }
        }

        [ProtoMember(6)]
        public bool IsReplaceDescriptionSelected
        {
            get => _isReplaceDescriptionSelected;
            set
            {
                if (_isReplaceDescriptionSelected == value)
                    return;

                SetProperty(ref _isReplaceDescriptionSelected, value);
            }
        }

        [ProtoMember(7)]
        public string PostReplaceDescription
        {
            get => _postReplaceDescription;
            set
            {
                if (_postReplaceDescription == value)
                    return;
                SetProperty(ref _postReplaceDescription, value);
            }
        }

        [ProtoMember(8)]
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
    }
}