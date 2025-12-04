using System.Collections.ObjectModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorHouseCore.Models.SocioPublisher;
using System.Collections.Generic;

namespace PinDominatorCore.PDModel
{
    public class AccountDetailsSelectorModel : BindableBase
    {
        private string _detailName;
        public List<SectionDetails> ListOfSection { get; set; } = new List<SectionDetails>();

        private string _detailUrl;

        private string _detailSection;
        private string _sectionValue;
        private bool _isSelected;

        private string _label;

        private ObservableCollection<RepinQueryContent> _queryType = new ObservableCollection<RepinQueryContent>();
        public string AccountId { get; set; }

        public string AccountName { get; set; }

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

        public string Label
        {
            get => _label;
            set
            {
                if (_label == value)
                    return;
                _label = value;
                OnPropertyChanged(nameof(Label));
            }
        }
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
        public string SectionValue
        {
            get => _sectionValue;
            set
            {
                if (_sectionValue == value)
                    return;
                _sectionValue = value;
                OnPropertyChanged(nameof(SectionValue));
            }
        }
        public bool IsSectionAvailable { get; set; } = false;
        public ObservableCollection<RepinQueryContent> QueryType
        {
            get => _queryType;
            set => SetProperty(ref _queryType, value);
        }

        public SocialNetworks Network { get; set; }
    }

    public class RepinQueryContent : BindableBase
    {
        private string _boardUrl;

        private string _content;


        private bool _isContentSelected;

        /// <summary>
        ///     Provide the BoardUrl for Uniqueness
        /// </summary>
        public string BoardUrl
        {
            get => _boardUrl;
            set => SetProperty(ref _boardUrl, value);
        }

        /// <summary>
        ///     Provide the content
        /// </summary>
        public string Content
        {
            get => _content;
            set
            {
                if (_content != null && value == _content)
                    return;
                SetProperty(ref _content, value);
            }
        }

        /// <summary>
        ///     IsContentSelected is used to give the status whether the content is selected or not
        /// </summary>
        public bool IsContentSelected
        {
            get => _isContentSelected;
            set
            {
                if (value == _isContentSelected)
                    return;
                SetProperty(ref _isContentSelected, value);
            }
        }
    }

}