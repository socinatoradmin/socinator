using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel;
using PublisherDestinationDetailsModel = LinkedDominatorCore.LDModel.PublisherDestinationDetailsModel;

namespace LinkedDominatorUI.Utility
{
    public class AccountDetailsSelectorViewModel : BindableBase
    {
        private ICollectionView _accountDetailsSelectorView;


        private List<string> _alreadySelectedList = new List<string>();

        private string _detailsLabelHeader = string.Empty;

        private string _detailsNameHeader = string.Empty;

        private string _detailsQueryTypeHeader = string.Empty;

        private string _detailsUrlHeader = string.Empty;


        private bool _isAllCampaignSelected;

        private bool _isProgressRingActive = true;

        private ObservableCollection<AccountdetailsSelectedModel> _listAccountDetailsSelectorModels =
            new ObservableCollection<AccountdetailsSelectedModel>();


        private string _statusText = "Fetching..";

        private string _textSearch;


        private string _title = string.Empty;

        public AccountDetailsSelectorViewModel()
        {
            AccountDetailsSelectorView = CollectionViewSource.GetDefaultView(ListAccountDetailsSelectorModels);
            TextSearchCommand = new BaseCommand<object>(TextSearchCanExecute, TextSearchExecute);
        }


        public ICommand TextSearchCommand { get; set; }

        public bool IsAllCampaignSelected
        {
            get => _isAllCampaignSelected;
            set
            {
                if (_isAllCampaignSelected == value)
                    return;
                SetProperty(ref _isAllCampaignSelected, value);

                if (_isAllCampaignSelected)
                    SelectAllCampaign();
                else
                    SelectNoneCampaign();
            }
        }

        public ObservableCollection<AccountdetailsSelectedModel> ListAccountDetailsSelectorModels
        {
            get => _listAccountDetailsSelectorModels;
            set
            {
                if (_listAccountDetailsSelectorModels == value)
                    return;
                _listAccountDetailsSelectorModels = value;
                OnPropertyChanged(nameof(ListAccountDetailsSelectorModels));
            }
        }

        public bool IsProgressRingActive
        {
            get => _isProgressRingActive;
            set
            {
                if (_isProgressRingActive == value)
                    return;
                _isProgressRingActive = value;
                OnPropertyChanged(nameof(IsProgressRingActive));
            }
        }

        public ICollectionView AccountDetailsSelectorView
        {
            get => _accountDetailsSelectorView;
            set
            {
                if (_accountDetailsSelectorView == value)
                    return;
                _accountDetailsSelectorView = value;
                OnPropertyChanged(nameof(AccountDetailsSelectorView));
            }
        }

        public string DetailsNameHeader
        {
            get => _detailsNameHeader;
            set
            {
                if (_detailsNameHeader == value)
                    return;
                _detailsNameHeader = value;
                OnPropertyChanged(nameof(DetailsNameHeader));
            }
        }

        public string DetailsUrlHeader
        {
            get => _detailsUrlHeader;
            set
            {
                if (_detailsUrlHeader == value)
                    return;
                _detailsUrlHeader = value;
                OnPropertyChanged(nameof(DetailsUrlHeader));
            }
        }

        public string DetailsQueryTypeHeader
        {
            get => _detailsQueryTypeHeader;
            set
            {
                if (_detailsQueryTypeHeader == value)
                    return;
                _detailsQueryTypeHeader = value;
                OnPropertyChanged(nameof(DetailsQueryTypeHeader));
            }
        }

        public string DetailsLabelHeader
        {
            get => _detailsLabelHeader;
            set
            {
                if (_detailsLabelHeader == value)
                    return;
                _detailsLabelHeader = value;
                OnPropertyChanged(nameof(DetailsLabelHeader));
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                if (_title == value)
                    return;
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string StatusText
        {
            get => _statusText;
            set
            {
                if (_statusText == value)
                    return;
                _statusText = value;
                OnPropertyChanged(nameof(StatusText));
            }
        }

        public List<string> AlreadySelectedList
        {
            get => _alreadySelectedList;
            set
            {
                if (_alreadySelectedList == value)
                    return;
                _alreadySelectedList = value;
                OnPropertyChanged(nameof(AlreadySelectedList));
            }
        }

        public string TextSearch
        {
            get => _textSearch;
            set
            {
                _textSearch = value;
                OnPropertyChanged(nameof(TextSearch));
            }
        }

        private bool TextSearchCanExecute(object sender)
        {
            return true;
        }

        private void TextSearchExecute(object sender)
        {
            if (string.IsNullOrEmpty(TextSearch) || string.IsNullOrWhiteSpace(TextSearch))
                AccountDetailsSelectorView.Filter += null;
            else
                AccountDetailsSelectorView.Filter += FilterByText;
        }

        private bool FilterByText(object sender)
        {
            try
            {
                var objAccountDetailsSelectorModel = sender as AccountDetailsSelectorModel;

                return objAccountDetailsSelectorModel != null &&
                       (objAccountDetailsSelectorModel.DetailName.IndexOf(TextSearch,
                            StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        objAccountDetailsSelectorModel.DetailUrl.IndexOf(TextSearch,
                            StringComparison.InvariantCultureIgnoreCase) >= 0);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        public void SelectAllCampaign()
        {
            ListAccountDetailsSelectorModels.Select(x =>
            {
                x.IsSelected = true;
                return x;
            }).ToList();
        }

        public void SelectNoneCampaign()
        {
            ListAccountDetailsSelectorModels.Select(x =>
            {
                x.IsSelected = false;
                return x;
            }).ToList();
        }


        public IEnumerable<GroupSelectDestination> GetSelectedItems(bool isAccountSelected = false)
        {
            var selectedItems = new List<GroupSelectDestination>();
            ListAccountDetailsSelectorModels.ForEach(x =>
            {
                if (x.IsSelected)
                    selectedItems.Add(new GroupSelectDestination
                    {
                        AccountId = x.AccountId,
                        ListofGroupsofAccounts =
                            new KeyValuePair<string, List<GroupQueryContent>>(x.DetailUrl, x.QueryType.ToList()),
                        Label = x.Label,
                        IsSelected = isAccountSelected
                    });
            });
            return selectedItems;
        }

        public IEnumerable<GroupSelectDestination> SelectAllItems()
        {
            var selectedItems = new List<GroupSelectDestination>();
            ListAccountDetailsSelectorModels.ForEach(x =>
            {
                if (x.IsSelected)
                    selectedItems.Add(new GroupSelectDestination
                    {
                        AccountId = x.AccountId,
                        ListofGroupsofAccounts =
                            new KeyValuePair<string, List<GroupQueryContent>>(x.DetailUrl, x.QueryType.ToList()),
                        Label = x.Label,
                        IsSelected = true
                    });
            });
            return selectedItems;
        }

        public IEnumerable<PublisherDestinationDetailsModel> GetSelectedItemsDestinations(string destinationType)
        {
            var selectedDestinations = new List<PublisherDestinationDetailsModel>();

            ListAccountDetailsSelectorModels.ForEach(x =>
            {
                if (x.IsSelected)
                    selectedDestinations.Add(new PublisherDestinationDetailsModel
                    {
                        AccountId = x.AccountId,
                        DestinationType = destinationType,
                        DestinationUrl = x.DetailUrl,
                        SocialNetworks = x.Network,
                        PublisherPostlistModel = new PublisherPostlistModel(),
                        DestinationGuid = Utilities.GetGuid(),
                        AccountName = x.AccountName
                    });
            });

            return selectedDestinations;
        }
    }
}