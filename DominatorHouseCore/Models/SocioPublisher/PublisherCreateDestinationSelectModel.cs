#region

using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class PublisherCreateDestinationSelectModel : BindableBase
    {
        private bool _isAccountSelected;

        [ProtoMember(1)]
        public bool IsAccountSelected
        {
            get => _isAccountSelected;
            set
            {
                if (_isAccountSelected == value)
                    return;
                _isAccountSelected = value;
                OnPropertyChanged(nameof(IsAccountSelected));
                if (!IsAccountSelected)
                    PublishonOwnWall = IsAccountSelected;
            }
        }


        private string _accountId;

        [ProtoMember(2)]
        public string AccountId
        {
            get => _accountId;
            set
            {
                _accountId = value;
                OnPropertyChanged(nameof(AccountId));
            }
        }

        private string _accountName;

        [ProtoMember(3)]
        public string AccountName
        {
            get => _accountName;
            set
            {
                _accountName = value;
                OnPropertyChanged(nameof(AccountName));
            }
        }

        [ProtoMember(4)] public SocialNetworks SocialNetworks { get; set; }

        [ProtoMember(5)] public bool IsGroupsAvailable { get; set; }

        [ProtoMember(6)] public bool IsPagesOrBoardsAvailable { get; set; }


        private string _groupSelectorText = "LangKeyNA".FromResourceDictionary();

        [ProtoMember(7)]
        public string GroupSelectorText
        {
            get => _groupSelectorText;
            set
            {
                if (_groupSelectorText == value)
                    return;
                _groupSelectorText = value;
                OnPropertyChanged(nameof(GroupSelectorText));
            }
        }


        private string _pagesOrBoardsSelectorText = "LangKeyNA".FromResourceDictionary();

        [ProtoMember(8)]
        public string PagesOrBoardsSelectorText
        {
            get => _pagesOrBoardsSelectorText;
            set
            {
                if (_pagesOrBoardsSelectorText == value)
                    return;
                _pagesOrBoardsSelectorText = value;
                OnPropertyChanged(nameof(PagesOrBoardsSelectorText));
            }
        }

        private string _customDestinationSelectorText = "0";

        [ProtoMember(14)]
        public string CustomDestinationSelectorText
        {
            get => _customDestinationSelectorText;
            set
            {
                if (_customDestinationSelectorText == value)
                    return;
                _customDestinationSelectorText = value;
                OnPropertyChanged(nameof(CustomDestinationSelectorText));
            }
        }


        private int _totalGroups;

        [ProtoMember(9)]
        public int TotalGroups
        {
            get => _totalGroups;
            set
            {
                if (_totalGroups == value)
                    return;
                _totalGroups = value;
                // UpdateGroupText();
                OnPropertyChanged(nameof(TotalGroups));
            }
        }

        private int _selectedGroups;

        [ProtoMember(10)]
        public int SelectedGroups
        {
            get => _selectedGroups;
            set
            {
                if (_selectedGroups == value)
                    return;
                _selectedGroups = value;
                // UpdateGroupText();
                OnPropertyChanged(nameof(SelectedGroups));
            }
        }


        private int _totalPagesOrBoards;

        [ProtoMember(11)]
        public int TotalPagesOrBoards
        {
            get => _totalPagesOrBoards;
            set
            {
                if (_totalPagesOrBoards == value)
                    return;
                _totalPagesOrBoards = value;
                OnPropertyChanged(nameof(TotalPagesOrBoards));
            }
        }

        private int _selectedPagesOrBoards;

        [ProtoMember(12)]
        public int SelectedPagesOrBoards
        {
            get => _selectedPagesOrBoards;
            set
            {
                if (_selectedPagesOrBoards == value)
                    return;
                _selectedPagesOrBoards = value;
                OnPropertyChanged(nameof(SelectedPagesOrBoards));
            }
        }

        private bool _publishonOwnWall;

        [ProtoMember(13)]
        public bool PublishonOwnWall
        {
            get => _publishonOwnWall;
            set
            {
                if (_publishonOwnWall == value || !IsOwnWallAvailable)
                    return;
                _publishonOwnWall = value;
                OnPropertyChanged(nameof(PublishonOwnWall));
            }
        }

        private bool _isEnableStatusSync;

        public bool IsEnableStatusSync
        {
            get => _isEnableStatusSync;
            set
            {
                if (_isEnableStatusSync == value)
                    return;
                _isEnableStatusSync = value;
                OnPropertyChanged(nameof(IsEnableStatusSync));
            }
        }

        private string _statusSyncContent = ConstantVariable.FineStatusSync;

        public string StatusSyncContent
        {
            get => _statusSyncContent;
            set
            {
                if (_statusSyncContent == value)
                    return;
                _statusSyncContent = value;
                OnPropertyChanged(nameof(StatusSyncContent));
            }
        }


        private bool _isOwnWallAvailable;

        [ProtoMember(15)]
        public bool IsOwnWallAvailable
        {
            get => _isOwnWallAvailable;
            set
            {
                if (_isOwnWallAvailable == value)
                    return;
                _isOwnWallAvailable = value;
                OnPropertyChanged(nameof(IsOwnWallAvailable));
            }
        }


        public void UpdateGroupText()
        {
            GroupSelectorText = IsGroupsAvailable
                ? SelectedGroups + "/" + TotalGroups
                : "LangKeyNA".FromResourceDictionary();
        }

        public void UpdatePagesOrBoardsText()
        {
            PagesOrBoardsSelectorText = IsPagesOrBoardsAvailable
                ? SelectedPagesOrBoards + "/" + TotalPagesOrBoards
                : "LangKeyNA".FromResourceDictionary();
        }


        [ProtoMember(16)] public bool IsFriendsAvailable { get; set; }


        private string _friendsSelectorText = "LangKeyNA".FromResourceDictionary();

        [ProtoMember(17)]
        public string FriendsSelectorText
        {
            get => _friendsSelectorText;
            set
            {
                if (_friendsSelectorText == value)
                    return;
                _friendsSelectorText = value;
                OnPropertyChanged(nameof(FriendsSelectorText));
            }
        }


        private int _totalFriends;

        [ProtoMember(18)]
        public int TotalFriends
        {
            get => _totalFriends;
            set
            {
                if (_totalFriends == value)
                    return;
                _totalFriends = value;
                OnPropertyChanged(nameof(TotalFriends));
            }
        }

        private int _selectedFriends;

        [ProtoMember(19)]
        public int SelectedFriends
        {
            get => _selectedFriends;
            set
            {
                if (_selectedFriends == value)
                    return;
                _selectedFriends = value;
                OnPropertyChanged(nameof(SelectedFriends));
            }
        }

        private bool _isCustomDestinationInNetworks;

        [ProtoMember(20)]
        public bool IsCustomDestinationInNetworks
        {
            get => _isCustomDestinationInNetworks;
            set => SetProperty(ref _isCustomDestinationInNetworks, value);
        }

        private string _accountGroupName;

        [ProtoMember(21)]
        public string AccountGroupName
        {
            get => _accountGroupName;
            set => SetProperty(ref _accountGroupName, value);
        }


        private bool _isScrapingAvailableInNetworks;

        [ProtoMember(23)]
        public bool IsScrapingAvailableInNetworks
        {
            get => _isScrapingAvailableInNetworks;
            set
            {
                if (_isScrapingAvailableInNetworks == value)
                    return;
                _isScrapingAvailableInNetworks = value;
                OnPropertyChanged(nameof(IsScrapingAvailableInNetworks));
            }
        }

        private bool _isForScrapingOnly;

        [ProtoMember(24)]
        public bool IsScrapeFromAccount
        {
            get => _isForScrapingOnly;
            set
            {
                if (_isForScrapingOnly == value || !IsScrapingAvailableInNetworks)
                    return;
                _isForScrapingOnly = value;
                OnPropertyChanged(nameof(IsScrapeFromAccount));
            }
        }
    }
}