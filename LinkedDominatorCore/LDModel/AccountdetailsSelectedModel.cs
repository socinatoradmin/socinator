using System.Collections.ObjectModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace LinkedDominatorCore.LDModel
{
    public class AccountdetailsSelectedModel : BindableBase
    {
        private string _detailName;


        private string _detailUrl;


        private bool _isSelected;

        private string _label;

        private ObservableCollection<GroupQueryContent> _queryType = new ObservableCollection<GroupQueryContent>();
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

        public ObservableCollection<GroupQueryContent> QueryType
        {
            get => _queryType;
            set => SetProperty(ref _queryType, value);
        }

        public SocialNetworks Network { get; set; }
    }

    public class GroupQueryContent : BindableBase
    {
        private string _content;
        private string _groupUrl;


        private bool _isContentSelected;

        /// <summary>
        ///     Provide the BoardUrl for Uniqueness
        /// </summary>
        public string GroupUrl
        {
            get => _groupUrl;
            set => SetProperty(ref _groupUrl, value);
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