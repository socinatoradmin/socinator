using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace PinDominatorCore.PDModel
{
    [ProtoContract]
    public class RepinCreateDestinationSelectModel : BindableBase
    {
        private string _accountId;

        private string _accountName; 
      private string _groupName;
        private AccountStatus _accountStatus;

        private string _customDestinationSelectorText = "0";


        private string _groupSelectorText = "NA";
        private bool _isAccountSelected;


        private bool _isEnableStatusSync;


        private string _pagesOrBoardsSelectorText = "NA";

        private bool _publishonOwnWall;

        private int _selectedGroups;

        private int _selectedPagesOrBoards;

        private string _statusSyncContent = ConstantVariable.FineStatusSync;


        private int _totalGroups;


        private int _totalPagesOrBoards;

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
            }
        }

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

        [ProtoMember(16)]
        public string GroupName
        {
            get => _groupName;
            set
            {
                _groupName = value;
                OnPropertyChanged(nameof(GroupName));
            }
        }
        [ProtoMember(4)] public SocialNetworks SocialNetworks { get; set; }

        [ProtoMember(5)] public bool IsGroupsAvailable { get; set; }

        [ProtoMember(6)] public bool IsPagesOrBoardsAvailable { get; set; }

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

        [ProtoMember(9)]
        public int TotalGroups
        {
            get => _totalGroups;
            set
            {
                if (_totalGroups == value)
                    return;
                _totalGroups = value;
                OnPropertyChanged(nameof(TotalGroups));
            }
        }

        [ProtoMember(10)]
        public int SelectedGroups
        {
            get => _selectedGroups;
            set
            {
                if (_selectedGroups == value)
                    return;
                _selectedGroups = value;
                OnPropertyChanged(nameof(SelectedGroups));
            }
        }

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

        [ProtoMember(13)]
        public bool PublishonOwnWall
        {
            get => _publishonOwnWall;
            set
            {
                if (_publishonOwnWall == value)
                    return;
                _publishonOwnWall = value;
                OnPropertyChanged(nameof(PublishonOwnWall));
            }
        }

        [ProtoMember(14)]
        public AccountStatus AccountStatus
        {
            get => _accountStatus;
            set
            {
                if (_accountStatus == value)
                    return;
                _accountStatus = value;
                OnPropertyChanged(nameof(_accountStatus));
            }
        }

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


        public void UpdateGroupText()
        {
            GroupSelectorText = IsGroupsAvailable ? SelectedGroups + "/" + TotalGroups : "NA";
        }

        public void UpdatePagesOrBoardsText()
        {
            PagesOrBoardsSelectorText =
                IsPagesOrBoardsAvailable ? SelectedPagesOrBoards + "/" + TotalPagesOrBoards : "NA";
        }
    }
}