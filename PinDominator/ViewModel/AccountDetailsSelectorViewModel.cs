using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PinDominator.CustomControl;
using PinDominatorCore.PDModel;
using PinDominatorCore.Utility;
using AccountDetailsSelectorModel = PinDominatorCore.PDModel.AccountDetailsSelectorModel;
using PublisherDestinationDetailsModel = DominatorHouseCore.Models.SocioPublisher.PublisherDestinationDetailsModel;
namespace PinDominator.ViewModel
{
    public class AccountDetailsSelectorViewModel : BindableBase
    {
        private ICollectionView _accountDetailsSelectorView;

        private List<string> _alreadySelectedList = new List<string>();
        private List<AccountDetailsSelectorModel> _alreadySelectedSection = new List<AccountDetailsSelectorModel>();

        private string _detailsLabelHeader = string.Empty;

        private string _detailsNameHeader = string.Empty;

        private string _detailsQueryTypeHeader = string.Empty;

        private string _detailsUrlHeader = string.Empty;

        private string _detailUrlSection = string.Empty;
        private bool _isAllCampaignSelected;

        private bool _isProgressRingActive = true;

        private ObservableCollection<AccountDetailsSelectorModel> _listAccountDetailsSelectorModels =
            new ObservableCollection<AccountDetailsSelectorModel>();


        private string _statusText = "Fetching..";

        private string _textSearch;


        private string _title = string.Empty;

        public AccountDetailsSelectorViewModel()
        {
            AccountDetailsSelectorView = CollectionViewSource.GetDefaultView(ListAccountDetailsSelectorModels);
            TextSearchCommand = new BaseCommand<object>(TextSearchCanExecute, TextSearchExecute);
            SelectSectionCommand = new BaseCommand<object>(sender => true, SelectSectionExecute);
            SelectSection = new BaseCommand<object>(sender => true, OnSelectExecute);
        }
        public ICommand TextSearchCommand { get; set; }
        public ICommand SelectSectionCommand { get; set; }
        public ICommand SelectSection { get; set; }

        public bool IsAllCampaignSelected
        {
            get => _isAllCampaignSelected;
            set
            {
                if (_isAllCampaignSelected == value)
                    return;
                SetProperty(ref _isAllCampaignSelected, value);
                SelectCampaign(_isAllCampaignSelected);
                IsUncheckedFromList = false;
            }
        }

        public ObservableCollection<AccountDetailsSelectorModel> ListAccountDetailsSelectorModels
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
        public string DetailsUrlSection
        {
            get => _detailUrlSection;
            set
            {
                if (_detailUrlSection == value)
                    return;
                _detailUrlSection = value;
                OnPropertyChanged(nameof(DetailsUrlSection));
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
        public List<AccountDetailsSelectorModel> AlreadySelectedSection
        {
            get => _alreadySelectedSection;
            set
            {
                if (_alreadySelectedSection == value)
                    return;
                _alreadySelectedSection = value;
                OnPropertyChanged(nameof(AlreadySelectedSection));
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

        public bool IsUncheckedFromList { get; private set; }

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
        public void OnSelectExecute(object sender)
        {
            var moduleName = sender.ToString();
            switch (moduleName)
            {
                case "SelectNone":
                    IsAllCampaignSelected = false;
                    break;
                case "SelectAll":
                    IsAllCampaignSelected = true;
                    break;
                case "Manually":
                    if (ListAccountDetailsSelectorModels.All(x => x.IsSelected))
                        IsAllCampaignSelected = true;
                    else
                    {
                        if (IsAllCampaignSelected)
                            IsUncheckedFromList = true;
                        IsAllCampaignSelected = false;
                    }
                    break;
            }
        }
        private void SelectSectionExecute(object sender)
        {
            var AccountDetailsSelectorModel = sender as AccountDetailsSelectorModel;
            var alreadySelectedPages = GetSelectedItems().Select(x=>x.LstofPinsToRepin.Key).ToList();
            var alreadySelectedSection = GetSelectedSection().ToList();
            var SectionSector = new AccountDetailsSelector(UpdateSingleSectionDetails, AccountDetailsSelectorModel)
            {
                AccountDetailsSelectorViewModel =
                    {
                        Title = "LangKeySelectSections".FromResourceDictionary(),
                        DetailsUrlHeader = $"Section Url",
                        DetailsNameHeader = $"Section Title",
                        DetailsUrlSection= $"Section ID",
                        AlreadySelectedList = alreadySelectedPages,
                        AlreadySelectedSection=alreadySelectedSection
                    }
            };
            var dialog = new Dialog();
            var window = dialog.GetMetroWindow(SectionSector,"LangKeySelectSections".FromResourceDictionary());
            SectionSector.QueryHeader.Width = 0;
            SectionSector.LabelHeader.Width = 0;
            SectionSector.BtnCancel.Click += (senderDetails, events) => window.Close();
            SectionSector.BtnSave.Click += (senderDetails, events) =>
            {
                var SelectedSection = SectionSector.AccountDetailsSelectorViewModel.GetSelectedSection();
                var ListOfBoard = BoardCreateDestinationsViewModel.accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels;
                var CurrentSelectedBoard = ListOfBoard.FirstOrDefault(x=>x.IsSelected&&x.DetailUrl== AccountDetailsSelectorModel.DetailUrl);
                if(BoardCreateDestinationsViewModel.accountDetailsSelector!=null && CurrentSelectedBoard != null)
                {
                    CurrentSelectedBoard.ListOfSection.Clear();
                    SelectedSection.ForEach(section =>
                    {
                        CurrentSelectedBoard.ListOfSection.Add(new SectionDetails() {SectionTitle=section.DetailName,SectionId=section.DetailSection,IsSelected=section.IsSelected,AccountId=section.AccountId,AccountName=section.AccountName,BoardName=section.SectionValue,BoardUrl=section.DetailUrl,Network=section.Network.ToString() });
                    });
                    CurrentSelectedBoard.DetailSection = $"{SelectedSection.Count(x=>x.IsSelected)}/{SectionSector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Count}";
                }
                window.Close();
            };
            window.Show();
            SectionSector.UpdateSingleSectionDetails();
        }

        private async Task UpdateSingleSectionDetails(AccountDetailsSelector accountDetailsSelector, AccountDetailsSelectorModel selectorModel)
        {
            try
            {
                var alreadySelectedPages = accountDetailsSelector.AccountDetailsSelectorViewModel.AlreadySelectedSection.FirstOrDefault(y=>(y.AccountId==selectorModel.AccountId||y.AccountName==selectorModel.AccountName)&&y.IsSelected).ListOfSection;
                var Sections = JsonConvert.DeserializeObject<JArray>(selectorModel.SectionValue);
                var jsonHandler = JsonJArrayHandler.GetInstance;
                if (Sections.HasValues)
                    Sections.ForEach(section =>
                    {
                        var ID = jsonHandler.GetJTokenValue(section, "SectionID");
                        var Title = jsonHandler.GetJTokenValue(section, "SectionTitle");
                        var SectionDetails = new AccountDetailsSelectorModel
                        {
                            DetailName = Title,
                            DetailUrl=selectorModel.DetailUrl,
                            Network=selectorModel.Network,
                            AccountId=selectorModel.AccountId,
                            AccountName=selectorModel.AccountName,
                            DetailSection=ID,
                            SectionValue=selectorModel.DetailName,
                            IsSelected=alreadySelectedPages.Any(x=>(x.SectionTitle==Title||x.SectionId==ID)&&x.IsSelected)
                        };
                        if (!Application.Current.Dispatcher.CheckAccess())
                            Application.Current.Dispatcher.Invoke(() =>accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(SectionDetails));
                        else
                            accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(SectionDetails);
                    });
            }
            catch(Exception ex) { ex.DebugLog(); }
            finally
            {
                BoardCreateDestinationsViewModel.UpdateStatus(accountDetailsSelector);
            }
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

        public void SelectCampaign(bool IsChecked)
        {
            if (IsUncheckedFromList)
                return;
            ListAccountDetailsSelectorModels.Select(x =>{x.IsSelected = IsChecked;return x;}).ToList();
        }
        public IEnumerable<RepinSelectDestination> GetSelectedItems(bool isAccountSelected = false)
        {
            var selectedItems = new List<RepinSelectDestination>();
            ListAccountDetailsSelectorModels.ForEach(x =>
            {
                if (x.IsSelected)
                    selectedItems.Add(new RepinSelectDestination
                    {
                        AccountId = x.AccountId,
                        LstofPinsToRepin =
                            new KeyValuePair<string, List<RepinQueryContent>>(x.DetailUrl, x.QueryType.ToList()),
                        Label = x.Label,
                        IsSelected = isAccountSelected,
                        LstSection=new KeyValuePair<string, List<SectionDetails>>(x.DetailUrl,x.ListOfSection)
                    });
            });
            return selectedItems;
        }
        public IEnumerable<AccountDetailsSelectorModel> GetSelectedSection()
        {
            var selectedSection = new List<AccountDetailsSelectorModel>();
            ListAccountDetailsSelectorModels.ForEach(x=>
            {
                if (x.IsSelected)
                    selectedSection.Add(x);
            });
            return selectedSection;
        }
        public IEnumerable<RepinSelectDestination> SelectAllItems()
        {
            var selectedItems = new List<RepinSelectDestination>();
            ListAccountDetailsSelectorModels.ForEach(x =>
            {
                if (x.IsSelected)
                    selectedItems.Add(new RepinSelectDestination
                    {
                        AccountId = x.AccountId,
                        LstofPinsToRepin =
                            new KeyValuePair<string, List<RepinQueryContent>>(x.DetailUrl, x.QueryType.ToList()),
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
                        AccountName = x.AccountName,
                        ListOfSection = new KeyValuePair<string, List<SectionDetails>>(x.DetailUrl, x.ListOfSection)
                    });
            });
            return selectedDestinations;
        }
    }
}