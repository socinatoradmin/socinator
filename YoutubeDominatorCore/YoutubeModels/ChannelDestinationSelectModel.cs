using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;

namespace YoutubeDominatorCore.YoutubeModels
{
    public class ChannelDestinationSelectModel : BindableBase
    {
        private string _accountId;

        private string _accountName;

        private string _accountNickName;

        private AccountStatus _accountStatus;

        private string _channelSelectorText = "NA";

        private string _customDestinationSelectorText = "0";

        private string _groupName;
        private bool _isAccountSelected;


        private bool _isEnableStatusSync;

        private List<string> _listOfChannel = new List<string>();

        private string _selectedChannel;

        private string _statusSyncContent = ConstantVariable.FineStatusSync;

        private int _totalChannels;

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

        [ProtoMember(4)]
        public List<string> ListOfChannel
        {
            get => _listOfChannel;
            set
            {
                if (_listOfChannel == value)
                    return;
                _listOfChannel = value;
                OnPropertyChanged(nameof(ListOfChannel));
            }
        }

        [ProtoMember(5)]
        public string SelectedChannel
        {
            get => _selectedChannel;
            set
            {
                if (_selectedChannel == value)
                    return;
                _selectedChannel = value;
                OnPropertyChanged(nameof(SelectedChannel));
            }
        }

        [ProtoMember(7)]
        public string ChannelSelectorText
        {
            get => _channelSelectorText;
            set
            {
                if (_channelSelectorText == value)
                    return;
                _channelSelectorText = value;
                OnPropertyChanged(nameof(ChannelSelectorText));
            }
        }

        [ProtoMember(9)]
        public int TotalChannels
        {
            get => _totalChannels;
            set
            {
                if (_totalChannels == value)
                    return;
                _totalChannels = value;
                // UpdateGroupText();
                OnPropertyChanged(nameof(TotalChannels));
            }
        }

        [ProtoMember(13)]
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

        [ProtoMember(14)]
        public AccountStatus AccountStatus
        {
            get => _accountStatus;
            set
            {
                if (_accountStatus == value)
                    return;
                _accountStatus = value;
                OnPropertyChanged(nameof(AccountStatus));
            }
        }

        [ProtoMember(15)]
        public string GroupName
        {
            get => _groupName;
            set
            {
                if (_groupName == value)
                    return;
                _groupName = value;
                OnPropertyChanged(nameof(GroupName));
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

        [ProtoMember(16)]
        public string AccountNickName
        {
            get => _accountNickName;
            set
            {
                if (_accountNickName == value)
                    return;
                _accountNickName = value;
                OnPropertyChanged(nameof(AccountNickName));
            }
        }
    }
}