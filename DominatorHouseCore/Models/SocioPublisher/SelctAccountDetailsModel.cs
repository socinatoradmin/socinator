#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CommonServiceLocator;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    /// <summary>
    ///     To hold the all neccessary data for creating destination
    /// </summary>
    [ProtoContract]
    public class SelectAccountDetailsModel : INotifyPropertyChanged
    {
        private readonly IBinFileHelper _binFileHelper;


        public SelectAccountDetailsModel()
        {
            _binFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();
        }

        public SelectAccountDetailsModel(bool isDisplaySingleAccount) : this()
        {
            IsDisplaySingleAccount = isDisplaySingleAccount;
        }

        private List<KeyValuePair<string, string>> _accountPagesBoardsPair = new List<KeyValuePair<string, string>>();
        private List<KeyValuePair<string, string>> _accountGroupPair = new List<KeyValuePair<string, string>>();
        private List<KeyValuePair<string, string>> _accountFriendsPair = new List<KeyValuePair<string, string>>();
        private List<Tuple<string, string, string>> _groupInviterDetails = new List<Tuple<string, string, string>>();
        private List<Tuple<string, string, string>> _pageInviterDetails = new List<Tuple<string, string, string>>();
        private List<string> _selectedAccountIds = new List<string>();
        private List<string> _publishOwnWallAccount = new List<string>();
        private bool _isRemoveGroupsRequiresApproval;
        private bool _isAddedNewGroups;
        private string _destinationId;
        private string _destinationName;
        private string _pageColWidth = "130";
        private string _groupColWidth = "130";
        private string _friendColWidth = "130";
        private string _customDestinationColWidth = "130";
        private bool _isDisplaySingleAccount;
        private string _displayAccount = string.Empty;

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
        ///     To specify the groups should be remove if its requires admin verification
        /// </summary>
        [ProtoMember(3)]
        public bool IsRemoveGroupsRequiresApproval
        {
            get => _isRemoveGroupsRequiresApproval;
            set
            {
                if (_isRemoveGroupsRequiresApproval == value)
                    return;
                _isRemoveGroupsRequiresApproval = value;
                OnPropertyChanged(nameof(IsRemoveGroupsRequiresApproval));
            }
        }


        /// <summary>
        ///     To specify whether need to consider newly added groups to destination list
        /// </summary>
        [ProtoMember(4)]
        public bool IsAddedNewGroups
        {
            get => _isAddedNewGroups;
            set
            {
                if (_isAddedNewGroups == value)
                    return;
                _isAddedNewGroups = value;
                OnPropertyChanged(nameof(IsAddedNewGroups));
            }
        }

        /// <summary>
        ///     To hold all selected pages or boards along with account Id
        ///     Key should be account Id and value should be page or board Url
        /// </summary>
        [ProtoMember(5)]
        public List<KeyValuePair<string, string>> AccountPagesBoardsPair
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

        /// <summary>
        ///     To hold all selected account Id
        /// </summary>
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

        /// <summary>
        ///     To hold account Id which should post on own wall
        /// </summary>
        [ProtoMember(8)]
        public List<string> PublishOwnWallAccount
        {
            get => _publishOwnWallAccount;
            set
            {
                if (_publishOwnWallAccount == value)
                    return;
                _publishOwnWallAccount = value;
                OnPropertyChanged(nameof(PublishOwnWallAccount));
            }
        }


        /// <summary>
        ///     To specify the date when destination created
        /// </summary>
        [ProtoMember(9)]
        public DateTime CreatedDate { get; set; }


        private ObservableCollection<PublisherCreateDestinationSelectModel> _listSelectDestination =
            new ObservableCollection<PublisherCreateDestinationSelectModel>();

        /// <summary>
        ///     To hold all destination list which holds all group,page count both selected and total
        /// </summary>
        [ProtoMember(10)]
        public ObservableCollection<PublisherCreateDestinationSelectModel> ListSelectDestination
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


        private List<KeyValuePair<SocialNetworks, string>> _accountsWithNetwork =
            new List<KeyValuePair<SocialNetworks, string>>();

        [ProtoMember(11)]
        public List<KeyValuePair<SocialNetworks, string>> AccountsWithNetwork
        {
            get => _accountsWithNetwork;
            set
            {
                if (_accountsWithNetwork == value)
                    return;
                _accountsWithNetwork = value;
                OnPropertyChanged(nameof(AccountsWithNetwork));
            }
        }


        private List<PublisherDestinationDetailsModel> _destinationDetailsModels =
            new List<PublisherDestinationDetailsModel>();

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


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     To assign the default for the destination
        /// </summary>
        /// <returns>
        ///     returns as filled default value of
        ///     <see cref="DominatorHouseCore.Models.SocioPublisher.PublisherCreateDestinationModel" />
        /// </returns>
        public static SelectAccountDetailsModel DestinationDefaultBuilder()
        {
            return new SelectAccountDetailsModel
            {
                DestinationName = $"Default-{ConstantVariable.GetDateTime()}",
                DestinationId = Utilities.GetGuid(),
                AccountPagesBoardsPair = new List<KeyValuePair<string, string>>(),
                AccountGroupPair = new List<KeyValuePair<string, string>>(),
                AccountFriendsPair = new List<KeyValuePair<string, string>>(),
                PublishOwnWallAccount = new List<string>(),
                SelectedAccountIds = new List<string>(),
                AccountsWithNetwork = new List<KeyValuePair<SocialNetworks, string>>(),
                CreatedDate = DateTime.Now,
                DestinationDetailsModels = new List<PublisherDestinationDetailsModel>()
            };
        }


        /// <summary>
        ///     To add the given destination to bin file
        /// </summary>
        /// <param name="publisherCreateDestinationModel">
        ///     pass parameter as filled default value of
        ///     <see cref="DominatorHouseCore.Models.SocioPublisher.PublisherCreateDestinationModel" />
        /// </param>
        /// <returns></returns>
        public bool AddDestination(PublisherCreateDestinationModel publisherCreateDestinationModel)
        {
            return _binFileHelper.AddDestination(publisherCreateDestinationModel);
        }

        /// <summary>
        ///     To update the given destination to list
        /// </summary>
        /// <param name="publisherCreateDestinationModel">
        ///     pass parameter as update value of
        ///     <see cref="DominatorHouseCore.Models.SocioPublisher.PublisherCreateDestinationModel" />
        /// </param>
        /// <returns></returns>
        public bool UpdateDestination(PublisherCreateDestinationModel publisherCreateDestinationModel)
        {
            return _binFileHelper.UpdateDestination(publisherCreateDestinationModel);
        }

        /// <summary>
        ///     To get the destination details
        /// </summary>
        /// <param name="destinationId">Id of the destination</param>
        /// <returns>
        ///     returns as matched condition of
        ///     <see cref="DominatorHouseCore.Models.SocioPublisher.PublisherCreateDestinationModel" />
        /// </returns>
        public PublisherCreateDestinationModel GetDestination(string destinationId)
        {
            var publisherCreateDestinationModel = _binFileHelper.GetDestination(destinationId);

            if (publisherCreateDestinationModel == null || publisherCreateDestinationModel.Count == 0)
                return new PublisherCreateDestinationModel();

            return publisherCreateDestinationModel[0];
        }

        [ProtoMember(14)]
        public List<KeyValuePair<string, string>> AccountFriendsPair
        {
            get => _accountFriendsPair;
            set
            {
                if (_accountFriendsPair == value)
                    return;
                _accountFriendsPair = value;
                OnPropertyChanged(nameof(AccountFriendsPair));
            }
        }

        [ProtoMember(15)]
        public string PageColWidth
        {
            get => _pageColWidth;
            set
            {
                if (_pageColWidth == value)
                    return;
                _pageColWidth = value;
                OnPropertyChanged(nameof(PageColWidth));
            }
        }


        [ProtoMember(16)]
        public string GroupColWidth
        {
            get => _groupColWidth;
            set
            {
                if (_groupColWidth == value)
                    return;
                _groupColWidth = value;
                OnPropertyChanged(nameof(GroupColWidth));
            }
        }


        [ProtoMember(17)]
        public string FriendColWidth
        {
            get => _friendColWidth;
            set
            {
                if (_friendColWidth == value)
                    return;
                _friendColWidth = value;
                OnPropertyChanged(nameof(FriendColWidth));
            }
        }


        [ProtoMember(18)]
        public List<Tuple<string, string, string>> GroupInviterDetails
        {
            get => _groupInviterDetails;
            set
            {
                if (_groupInviterDetails == value)
                    return;
                _groupInviterDetails = value;
                OnPropertyChanged(nameof(GroupInviterDetails));
            }
        }


        [ProtoMember(19)]
        public List<Tuple<string, string, string>> PageInviterDetails
        {
            get => _pageInviterDetails;
            set
            {
                if (_pageInviterDetails == value)
                    return;
                _pageInviterDetails = value;
                OnPropertyChanged(nameof(PageInviterDetails));
            }
        }

        [ProtoMember(20)]
        public bool IsDisplaySingleAccount
        {
            get => _isDisplaySingleAccount;
            set
            {
                if (_isDisplaySingleAccount == value)
                    return;
                _isDisplaySingleAccount = value;
                OnPropertyChanged(nameof(IsDisplaySingleAccount));
            }
        }


        [ProtoMember(21)]
        public string DisplayAccount
        {
            get => _displayAccount;
            set
            {
                if (_displayAccount == value)
                    return;
                _displayAccount = value;
                OnPropertyChanged(nameof(DisplayAccount));
            }
        }

        [ProtoMember(22)]
        public string CustomDestinationColWidth
        {
            get => _customDestinationColWidth;
            set
            {
                if (_customDestinationColWidth == value)
                    return;
                _customDestinationColWidth = value;
                OnPropertyChanged(nameof(CustomDestinationColWidth));
            }
        }


        private List<KeyValuePair<string, PublisherCustomDestinationModel>> _customDestinations =
            new List<KeyValuePair<string, PublisherCustomDestinationModel>>();

        [ProtoMember(23)]
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


        public SelectAccountDetailsModel GetGroupInviterDetails(SelectAccountDetailsModel model)
        {
            var updatedModel = new SelectAccountDetailsModel();

            var listGroupInviterDetails = new List<Tuple<string, string, string>>();
            foreach (var accountGroup in model.AccountGroupPair)
            {
                var accountFriendPair = model.AccountFriendsPair;
                var friendList = accountFriendPair.Where(x => x.Key == accountGroup.Key).Select(y => y.Value).ToList();
                var customList = model.CustomDestinations;

                var accountCustomDestination = customList
                    .Where(x => x.Key == accountGroup.Key && x.Value.DestinationType == "Friend")
                    .Select(x => x.Value.DestinationValue).ToList();
                friendList.ForEach(x =>
                {
                    var groupInviterDetail =
                        new Tuple<string, string, string>(accountGroup.Key, accountGroup.Value, x);

                    listGroupInviterDetails.Add(groupInviterDetail);
                });

                accountCustomDestination.ForEach(x =>
                {
                    var customGroupInviterDetail =
                        new Tuple<string, string, string>(accountGroup.Key, accountGroup.Value, x);

                    listGroupInviterDetails.Add(customGroupInviterDetail);
                });
            }

            updatedModel.GroupInviterDetails = listGroupInviterDetails;

            return updatedModel;
        }


        public SelectAccountDetailsModel GetPageInviterDetails(SelectAccountDetailsModel model)
        {
            var updatedModel = new SelectAccountDetailsModel();

            var listPageInviterDetails = new List<Tuple<string, string, string>>();
            foreach (var accountPage in
                model.AccountPagesBoardsPair)
            {
                var accountFriendPair = model.AccountFriendsPair;
                var friendList = accountFriendPair.Where(x => x.Key == accountPage.Key).Select(y => y.Value).ToList();
                friendList.ForEach(x =>
                {
                    var pageInviterDetail =
                        new Tuple<string, string, string>(accountPage.Key, accountPage.Value, x);

                    listPageInviterDetails.Add(pageInviterDetail);
                });
                var customList = model.CustomDestinations;
                var accountCustomDestination = customList
                    .Where(x => x.Key == accountPage.Key && x.Value.DestinationType == "Friend")
                    .Select(x => x.Value.DestinationValue).ToList();
                accountCustomDestination.ForEach(x =>
                {
                    var customGroupInviterDetail =
                        new Tuple<string, string, string>(accountPage.Key, accountPage.Value, x);

                    listPageInviterDetails.Add(customGroupInviterDetail);
                });
            }

            updatedModel.PageInviterDetails = listPageInviterDetails;

            return updatedModel;
        }
    }
}