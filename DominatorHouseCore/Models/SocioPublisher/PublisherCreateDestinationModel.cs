#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CommonServiceLocator;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces.SocioPublisher;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    /// <summary>
    ///     To hold the all neccessary data for creating destination
    /// </summary>
    [ProtoContract]
    public class PublisherCreateDestinationModel : INotifyPropertyChanged
    {
        private List<KeyValuePair<string, string>> _accountPagesBoardsPair = new List<KeyValuePair<string, string>>();
        private List<KeyValuePair<string, string>> _accountGroupPair = new List<KeyValuePair<string, string>>();
        private List<string> _selectedAccountIds = new List<string>();
        private List<string> _publishOwnWallAccount = new List<string>();
        private bool _isRemoveGroupsRequiresApproval;
        private bool _isAddedNewGroups;
        private string _destinationId;
        private string _destinationName;

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


        private List<KeyValuePair<string, PublisherCustomDestinationModel>> _customDestinations =
            new List<KeyValuePair<string, PublisherCustomDestinationModel>>();

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
        public static PublisherCreateDestinationModel DestinationDefaultBuilder()
        {
            return new PublisherCreateDestinationModel
            {
                DestinationName = $"Default-{ConstantVariable.GetDateTime()}",
                DestinationId = Utilities.GetGuid(),
                AccountPagesBoardsPair = new List<KeyValuePair<string, string>>(),
                AccountGroupPair = new List<KeyValuePair<string, string>>(),
                PublishOwnWallAccount = new List<string>(),
                SelectedAccountIds = new List<string>(),
                CustomDestinations = new List<KeyValuePair<string, PublisherCustomDestinationModel>>(),
                AccountsWithNetwork = new List<KeyValuePair<SocialNetworks, string>>(),
                CreatedDate = DateTime.Now,
                DestinationDetailsModels = new List<PublisherDestinationDetailsModel>()
            };
        }

        private readonly IBinFileHelper _binFileHelper;

        public PublisherCreateDestinationModel()
        {
            _binFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();
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

        public void UpdateNewGroup(string destinationId)
        {
            var publisherCreateDestinationModel = GetDestination(destinationId);

            if (publisherCreateDestinationModel.IsAddedNewGroups)
                publisherCreateDestinationModel.AccountsWithNetwork.ForEach(async x =>
                {
                    try
                    {
                        if (FeatureFlags.IsNetworkAvailable(x.Key))
                        {
                            var accountsDetailsSelector = SocinatorInitialize
                                .GetSocialLibrary(x.Key)
                                .GetNetworkCoreFactory().AccountDetailsSelectors;

                            if (accountsDetailsSelector != null && accountsDetailsSelector.IsGroupsAvailables)
                                try
                                {
                                    var groups = await accountsDetailsSelector.GetGroupUrls(x.Value,
                                        publisherCreateDestinationModel.CreatedDate);
                                    var alreadyPresentedGroups =
                                        publisherCreateDestinationModel.AccountGroupPair.Select(y => y.Value).ToList();
                                    foreach (var group in groups)
                                        if (!alreadyPresentedGroups.Contains(group))
                                            publisherCreateDestinationModel.AccountGroupPair.Add(
                                                new KeyValuePair<string, string>(x.Value, group));
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }

                            PublisherManageDestinationModel.UpdateDestinationsGroupCount(destinationId,
                                publisherCreateDestinationModel.AccountGroupPair.Count);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });
        }

        public void RemoveGroupsFromDestination(string destinationId, string accountId, SocialNetworks network,
            string groupUrl)
        {
            var updateCreateDestinationModel = GetDestination(destinationId);

            if (!updateCreateDestinationModel.IsRemoveGroupsRequiresApproval)
                return;

            var removeDestination =
                updateCreateDestinationModel.AccountGroupPair.FirstOrDefault(x =>
                    x.Key == accountId && x.Value == groupUrl);

            updateCreateDestinationModel.AccountGroupPair.Remove(removeDestination);

            PublisherManageDestinationModel.UpdateDestinationsGroupCount(destinationId,
                updateCreateDestinationModel.AccountGroupPair.Count);

            UpdateDestination(updateCreateDestinationModel);
        }
    }


    [ProtoContract]
    public class PublisherDestinationDetailsModel : IPublisherDestinationDetailsModel
    {
        [ProtoMember(1)] public string DestinationUrl { get; set; } = string.Empty;

        [ProtoMember(2)] public string DestinationType { get; set; } = string.Empty;

        [ProtoMember(3)] public string AccountId { get; set; } = string.Empty;

        [ProtoMember(4)] public SocialNetworks SocialNetworks { get; set; }

        [ProtoIgnore] public PublisherPostlistModel PublisherPostlistModel { get; set; } = new PublisherPostlistModel();

        [ProtoMember(5)] public bool IsCustomDestintions { get; set; }

        [ProtoMember(6)] public string DestinationGuid { get; set; } = string.Empty;

        [ProtoMember(7)] public string AccountName { get; set; } = string.Empty;

        [ProtoMember(8)] public string AccountGroupName { get; set; } = string.Empty;
        [ProtoMember(9)] public KeyValuePair<string, List<SectionDetails>> ListOfSection { get; set; } = new KeyValuePair<string, List<SectionDetails>>();
    }
}