#region

using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System;
using System.Collections.Generic;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    [Serializable]
    [ProtoContract]
    public class AccountDetailsSelectorModel : BindableBase
    {
        public string AccountId { get; set; }

        public string AccountName { get; set; }

        private int _currentIndex;
        private List<SectionDetails> _listOfSelectedSections = new List<SectionDetails>();
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (_currentIndex == value)
                    return;
                _currentIndex = value;
                OnPropertyChanged(nameof(CurrentIndex));
            }
        }

        public List<SectionDetails> ListOfSelectedSections
        {
            get => _listOfSelectedSections;
            set
            {
                if (_listOfSelectedSections == value)
                    return;
                SetProperty(ref _listOfSelectedSections, value);
            }
        }
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        private string _detailName;

        public string DetailName
        {
            get => _detailName;
            set
            {
                if (_detailName == value)
                    return;
                _detailName = value;
                OnPropertyChanged(nameof(DetailName));
            }
        }

        private string _detailSection;
        public string DetailSection
        {
            get => _detailSection;
            set
            {
                if (_detailSection == value)
                    return;
                _detailSection = value;
                OnPropertyChanged(nameof(DetailSection));
            }
        }
        private bool _isSectionAvailable;
        public bool IsSectionAvailable
        {
            get => _isSectionAvailable;
            set
            {
                if (_isSectionAvailable == value)
                    return;
                _isSectionAvailable = value;
                OnPropertyChanged(nameof(IsSectionAvailable));
            }
        }
        private string _detailSectionValue;
        public string DetailSectionValue
        {
            get => _detailSectionValue;
            set
            {
                if (_detailSectionValue == value)
                    return;
                _detailSectionValue = value;
                OnPropertyChanged(nameof(DetailSectionValue));
            }
        }
        private string _detailUrl;

        public string DetailUrl
        {
            get => _detailUrl;
            set
            {
                if (_detailUrl == value)
                    return;
                _detailUrl = value;
                OnPropertyChanged(nameof(DetailUrl));
            }
        }

        private bool _isOwnPage;

        public bool IsOwnPage
        {
            get => _isOwnPage;
            set
            {
                if (_isOwnPage == value)
                    return;
                _isOwnPage = value;
                OnPropertyChanged(nameof(IsOwnPage));
            }
        }


        private bool _isLikePage;

        public bool IsLikePage
        {
            get => _isLikePage;
            set
            {
                if (_isLikePage == value)
                    return;
                _isLikePage = value;
                OnPropertyChanged(nameof(IsLikePage));
            }
        }


        private bool _isFanpage;

        public bool IsFanpage
        {
            get => _isFanpage;
            set
            {
                if (_isFanpage == value)
                    return;
                _isFanpage = value;
                OnPropertyChanged(nameof(IsFanpage));
            }
        }

        private bool _isGroup;

        public bool IsGroup
        {
            get => _isGroup;
            set
            {
                if (_isGroup == value)
                    return;
                _isGroup = value;
                OnPropertyChanged(nameof(IsGroup));
            }
        }

        private bool _isOwnGroup;

        public bool IsOwnGroup
        {
            get => _isOwnGroup;
            set
            {
                if (_isOwnGroup == value)
                    return;
                _isOwnGroup = value;
                OnPropertyChanged(nameof(IsOwnGroup));
            }
        }


        private bool _isJoinedGroup;

        public bool IsJoinedGroup
        {
            get => _isJoinedGroup;
            set
            {
                if (_isJoinedGroup == value)
                    return;
                _isJoinedGroup = value;
                OnPropertyChanged(nameof(IsJoinedGroup));
            }
        }

        public SocialNetworks Network { get; set; }
    }
    [ProtoContract]
    public class SectionDetails
    {
        [ProtoMember(1)] public string SectionTitle { get; set; } = string.Empty;
        [ProtoMember(2)] public string SectionId { get; set; } = string.Empty;
        [ProtoMember(3)] public string BoardName { get; set; } = string.Empty;
        [ProtoMember(4)] public string BoardUrl { get; set; } = string.Empty;
        [ProtoMember(5)] public string AccountName { get; set; } = string.Empty;
        [ProtoMember(6)] public string AccountId { get; set; } = string.Empty;
        [ProtoMember(7)] public string Network { get; set; } = string.Empty;
        [ProtoMember(8)] public bool IsSelected { get; set; } = false;
    }
}