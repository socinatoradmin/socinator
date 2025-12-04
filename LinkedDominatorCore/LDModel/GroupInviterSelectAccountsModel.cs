using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces.SocioPublisher;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace LinkedDominatorCore.LDModel
{
    [ProtoContract]
    public class GroupInviterSelectAccountsModel : INotifyPropertyChanged
    {
        private List<KeyValuePair<string, string>> _accountGroupPair = new List<KeyValuePair<string, string>>();
        private List<GroupSelectDestination> _accountPagesBoardsPair = new List<GroupSelectDestination>();
        private AccountStatus _accountStatus;

        private List<KeyValuePair<string, PublisherCustomDestinationModel>> _customDestinations =
            new List<KeyValuePair<string, PublisherCustomDestinationModel>>();

        private List<PublisherDestinationDetailsModel> _destinationDetailsModels =
            new List<PublisherDestinationDetailsModel>();

        private string _destinationId;
        private string _destinationName;
        private bool _isAddedNewGroups;
        private bool _isRemoveGroupsRequiresApproval;


        private ObservableCollection<GroupCreateDestinationSelectModel> _listSelectDestination =
            new ObservableCollection<GroupCreateDestinationSelectModel>();

        private List<string> _selectedAccountIds = new List<string>();

        [ProtoMember(1)]
        public string DestinationId
        {
            get => _destinationId;
            set
            {
                if (_destinationId == value)
                    return;
                _destinationId = value;
                OnPropertyChanged(nameof(DestinationId));
            }
        }

        [ProtoMember(2)]
        public string DestinationName
        {
            get => _destinationName;
            set
            {
                if (_destinationName == value)
                    return;
                _destinationName = value;
                OnPropertyChanged(nameof(DestinationName));
            }
        }

        [ProtoMember(5)]
        public List<GroupSelectDestination> AccountPagesBoardsPair
        {
            get => _accountPagesBoardsPair;
            set
            {
                if (_accountPagesBoardsPair == value)
                    return;
                _accountPagesBoardsPair = value;
                OnPropertyChanged(nameof(AccountPagesBoardsPair));
            }
        }

        [ProtoMember(6)]
        public List<KeyValuePair<string, string>> AccountGroupPair
        {
            get => _accountGroupPair;
            set
            {
                if (_accountGroupPair == value)
                    return;
                _accountGroupPair = value;
                OnPropertyChanged(nameof(AccountGroupPair));
            }
        }

        [ProtoMember(7)]
        public List<string> SelectedAccountIds
        {
            get => _selectedAccountIds;
            set
            {
                if (_selectedAccountIds == value)
                    return;
                _selectedAccountIds = value;
                OnPropertyChanged(nameof(SelectedAccountIds));
            }
        }

        [ProtoMember(9)] public DateTime CreatedDate { get; set; }

        [ProtoMember(10)]
        public ObservableCollection<GroupCreateDestinationSelectModel> ListSelectDestination
        {
            get => _listSelectDestination;
            set
            {
                if (_listSelectDestination == value)
                    return;
                _listSelectDestination = value;
                OnPropertyChanged(nameof(ListSelectDestination));
            }
        }

        [ProtoMember(12)]
        public List<KeyValuePair<string, PublisherCustomDestinationModel>> CustomDestinations
        {
            get => _customDestinations;
            set
            {
                if (_customDestinations == value)
                    return;
                _customDestinations = value;
                OnPropertyChanged(nameof(CustomDestinations));
            }
        }

        [ProtoMember(13)]
        public List<PublisherDestinationDetailsModel> DestinationDetailsModels
        {
            get => _destinationDetailsModels;
            set
            {
                if (_destinationDetailsModels == value)
                    return;
                _destinationDetailsModels = value;
                OnPropertyChanged(nameof(DestinationDetailsModels));
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static GroupInviterSelectAccountsModel DestinationDefaultBuilder()
        {
            return new GroupInviterSelectAccountsModel
            {
                DestinationName = $"Default-{ConstantVariable.GetDateTime()}",
                DestinationId = Utilities.GetGuid(),
                AccountPagesBoardsPair = new List<GroupSelectDestination>(),
                AccountGroupPair = new List<KeyValuePair<string, string>>(),
                SelectedAccountIds = new List<string>(),
                CustomDestinations = new List<KeyValuePair<string, PublisherCustomDestinationModel>>(),
                CreatedDate = DateTime.Now,
                DestinationDetailsModels = new List<PublisherDestinationDetailsModel>()
            };
        }
    }

    [ProtoContract]
    public class PublisherDestinationDetailsModel : IPublisherDestinationDetailsModel
    {
        [ProtoMember(5)]
        public bool IsCustomDestintions { get; set; }

        [ProtoMember(6)] public string DestinationGuid { get; set; } = string.Empty;
        [ProtoMember(1)] public string DestinationUrl { get; set; } = string.Empty;

        [ProtoMember(2)] public string DestinationType { get; set; } = string.Empty;

        [ProtoMember(3)] public string AccountId { get; set; } = string.Empty;

        [ProtoMember(4)] public SocialNetworks SocialNetworks { get; set; }

        [ProtoMember(7)] public string AccountName { get; set; } = string.Empty;

        public PublisherPostlistModel PublisherPostlistModel{get;set;}
    }
}