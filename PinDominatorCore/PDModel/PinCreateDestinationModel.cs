using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace PinDominatorCore.PDModel
{
    public class PinCreateDestinationModel : INotifyPropertyChanged
    {
        private List<KeyValuePair<string, string>> _accountGroupPair = new List<KeyValuePair<string, string>>();

        private List<Tuple<string, KeyValuePair<string, List<RepinQueryContent>>, string>> _accountPagesBoardsPair =
            new List<Tuple<string, KeyValuePair<string, List<RepinQueryContent>>, string>>();

        private AccountStatus _accountStatus;

        private List<KeyValuePair<string, PinCreateDestinationModel>> _customDestinations =
            new List<KeyValuePair<string, PinCreateDestinationModel>>();

        private List<PinCreateDestinationModel> _destinationDetailsModels = new List<PinCreateDestinationModel>();
        private string _destinationId;
        private string _destinationName;


        private ObservableCollection<RepinCreateDestinationSelectModel> _listSelectDestination =
            new ObservableCollection<RepinCreateDestinationSelectModel>();

        private List<string> _selectedAccountIds = new List<string>();

        /// <summary>
        ///     To specify the destination Id
        /// </summary>
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


        /// <summary>
        ///     To specify the destination name
        /// </summary>
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

        /// <summary>
        ///     To hold all selected pages or boards along with account Id
        ///     Key should be account Id and value should be page or board Url
        /// </summary>
        [ProtoMember(3)]
        public List<Tuple<string, KeyValuePair<string, List<RepinQueryContent>>, string>> AccountPagesBoardsPair
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

        /// <summary>
        ///     To hold all selected groups along with account Id
        ///     Key should be account Id and value should be group Url
        /// </summary>
        [ProtoMember(4)]
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

        /// <summary>
        ///     To hold all selected account Id
        /// </summary>
        [ProtoMember(5)]
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

        /// <summary>
        ///     To specify the date when destination created
        /// </summary>
        [ProtoMember(6)]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        ///     To hold all destination list which holds all group,page count both selected and total
        /// </summary>
        [ProtoMember(7)]
        public ObservableCollection<RepinCreateDestinationSelectModel> ListSelectDestination
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

        [ProtoMember(8)]
        public List<KeyValuePair<string, PinCreateDestinationModel>> CustomDestinations
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

        [ProtoMember(9)]
        public List<PinCreateDestinationModel> DestinationDetailsModels
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

        /// <summary>
        ///     To assign the default for the destination
        /// </summary>
        /// <returns>returns as filled default value of  <see cref="PinCreateDestinationModel" /></returns>
        public static PinCreateDestinationModel DestinationDefaultBuilder()
        {
            return new PinCreateDestinationModel
            {
                DestinationName = $"Default-{ConstantVariable.GetDateTime()}",
                DestinationId = Utilities.GetGuid(),
                AccountPagesBoardsPair =
                    new List<Tuple<string, KeyValuePair<string, List<RepinQueryContent>>, string>>(),
                AccountGroupPair = new List<KeyValuePair<string, string>>(),
                SelectedAccountIds = new List<string>(),
                CustomDestinations = new List<KeyValuePair<string, PinCreateDestinationModel>>(),
                CreatedDate = DateTime.Now,
                DestinationDetailsModels = new List<PinCreateDestinationModel>()
            };
        }
    }
}