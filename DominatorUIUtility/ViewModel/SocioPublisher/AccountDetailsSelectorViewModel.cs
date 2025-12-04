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
using DominatorUIUtility.CustomControl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class AccountDetailsSelectorViewModel : BindableBase
    {
        private ICollectionView _accountDetailsSelectorView;


        private List<string> _alreadySelectedList = new List<string>();

        private string _detailsNameHeader = string.Empty;

        private string _detailsUrlHeader = string.Empty;
        private string _detailsUrlSection = string.Empty;


        private bool _isAllCampaignSelected;


        private bool _isGroupOptionVisible = true;


        private bool _isLikedPageSelected = true;

        private bool _isOwnPageSelected = true;

        private bool _isPageOptionVisible = true;

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
            OwnPageCheckedCommand = new BaseCommand<object>(OwnPageCheckedCanExecute, OwnPageCheckedExecute);
            LikedPageCheckedCommand = new BaseCommand<object>(LikedPageCheckedCanExecute, LikedPageCheckedExecute);
            OwnGroupCheckedCommand = new BaseCommand<object>(OwnGroupCheckedCanExecute, OwnGroupCheckedExecute);
            JoinedGroupCheckedCommand =
                new BaseCommand<object>(JoinedGroupCheckedCanExecute, JoinedGroupCheckedExecute);
            SelectSectionCommand = new BaseCommand<object>(sender => true, SelectSectionExecute);
        }


        public ICommand TextSearchCommand { get; set; }
        public ICommand OwnPageCheckedCommand { get; set; }
        public ICommand LikedPageCheckedCommand { get; set; }
        public ICommand OwnGroupCheckedCommand { get; set; }
        public ICommand JoinedGroupCheckedCommand { get; set; }

        public ICommand SelectSectionCommand { get; set; }
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

        public string DetailsUrlSection
        {
            get => _detailsUrlSection;
            set
            {
                if (_detailsUrlSection == value)
                    return;
                _detailsUrlSection = value;
                OnPropertyChanged(nameof(DetailsUrlSection));
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

        public bool IsOwnPageSelected
        {
            get => _isOwnPageSelected;
            set
            {
                if (_isOwnPageSelected == value)
                    return;
                _isOwnPageSelected = value;
                OnPropertyChanged(nameof(IsOwnPageSelected));
            }
        }

        public bool IsPageOptionVisible
        {
            get => _isPageOptionVisible;
            set
            {
                if (_isPageOptionVisible == value)
                    return;
                _isPageOptionVisible = value;
                OnPropertyChanged(nameof(IsPageOptionVisible));
            }
        }

        public bool IsLikedPageSelected
        {
            get => _isLikedPageSelected;
            set
            {
                if (_isLikedPageSelected == value)
                    return;
                _isLikedPageSelected = value;
                OnPropertyChanged(nameof(IsLikedPageSelected));
            }
        }

        public bool IsGroupOptionVisible
        {
            get => _isGroupOptionVisible;
            set
            {
                if (_isGroupOptionVisible == value)
                    return;
                _isGroupOptionVisible = value;
                OnPropertyChanged(nameof(_isGroupOptionVisible));
            }
        }

        private void SelectSectionExecute(object sender)
        {
            var accountDetailsSelectorModel = sender as AccountDetailsSelectorModel;
            var valuesPair = GetSelectedItems();
            var alreadySelectedPages = valuesPair.Select(x => x.Value).ToList();
            var SectionSelector = new AccountDetailsSelector(UpdateSingleBoardSection, accountDetailsSelectorModel)
            {
                AccountDetailsSelectorViewModel =
                {
                        Title =
                            $"{"LangKeySelectSections".FromResourceDictionary()}",
                        DetailsUrlHeader =
                            $"Section {"LangKeyUrl".FromResourceDictionary()}",
                        DetailsNameHeader =
                            $"Section {"LangKeyName".FromResourceDictionary()}",
                        DetailsUrlSection="Section ID",
                        AlreadySelectedList = alreadySelectedPages
                }
            };
            var dialog = new Dialog();
            var window = dialog.GetMetroWindow(SectionSelector, "LangKeySelectSections".FromResourceDictionary());
            SectionSelector.SectionHeader.Width = 180;
            SectionSelector.SectionName.Width = 140;
            SectionSelector.SectionUrl.Width = 140;
            SectionSelector.btnSave.Click += (Sender, events) =>
            {
                var SelectedItem = SectionSelector.AccountDetailsSelectorViewModel.GetSelectedSection();
                var CurrentViewModel = PublisherCreateDestinationsViewModel.accountDetailsSelector.AccountDetailsSelectorViewModel;
                var CurrentSelected = CurrentViewModel.ListAccountDetailsSelectorModels.FirstOrDefault(x => x.IsSelected && accountDetailsSelectorModel.DetailUrl == x.DetailUrl);
                if (PublisherCreateDestinationsViewModel.accountDetailsSelector != null && CurrentSelected != null)
                {
                    CurrentSelected.ListOfSelectedSections.Clear();
                    SelectedItem.ForEach(item => {
                        CurrentSelected.ListOfSelectedSections.Add(new SectionDetails { AccountName = CurrentSelected.AccountName, Network = CurrentSelected.Network.ToString(), SectionTitle = item.Key, SectionId = item.Value, AccountId = CurrentSelected.AccountId, IsSelected = true, BoardName = CurrentSelected.DetailName, BoardUrl = CurrentSelected.DetailUrl });
                    });
                    CurrentSelected.DetailSection = $"{CurrentSelected.ListOfSelectedSections.Count}/{SectionSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Count}";
                }
                window.Close();
            };
            SectionSelector.btnCancel.Click += (senderDetails, events) => { window.Close(); };
            window.Show();
            SectionSelector.UpdateSingleBoardSection();
        }

        private async Task UpdateSingleBoardSection(AccountDetailsSelector AccountDetails, AccountDetailsSelectorModel model)
        {
            try
            {
                var Sections = JsonConvert.DeserializeObject<JArray>(model.DetailSectionValue);
                var alreadySelectedSection = model.ListOfSelectedSections.Where(x => x.IsSelected).ToList();
                var jsonHandler = JsonHandler.GetInstance;
                if (Sections.HasValues)
                    Sections.ForEach(section =>
                    {
                        var jsonObject = jsonHandler.ParseJsonToJsonObject(section.ToString());
                        var Id = jsonHandler.GetJTokenValue(jsonObject, "SectionID");
                        var Title = jsonHandler.GetJTokenValue(jsonObject, "SectionTitle");
                        var SectionDetails = new AccountDetailsSelectorModel
                        {
                            AccountId = model.AccountId,
                            AccountName = model.AccountName,
                            CurrentIndex = AccountDetails.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Count + 1,
                            Network = model.Network,
                            DetailName = Title,
                            DetailSection = Id,
                            DetailUrl = model.DetailUrl + Title?.ToLower()?.Replace(" ", "-") + "/",
                            IsSelected = alreadySelectedSection.Any(x => x.SectionTitle == Title || x.SectionId == Id || x.AccountId == model.AccountId)
                        };
                        if (!Application.Current.Dispatcher.CheckAccess())
                            Application.Current.Dispatcher.Invoke(() =>
                                AccountDetails.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels
                                    .Add(SectionDetails));
                        else
                            AccountDetails.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels
                                .Add(SectionDetails);
                    });
            }
            catch (Exception e) { e.DebugLog(); }
            PublisherCreateDestinationsViewModel.UpdateStatus(AccountDetails);
        }

        private bool OwnPageCheckedCanExecute(object sender)
        {
            return true;
        }

        private bool LikedPageCheckedCanExecute(object sender)
        {
            return true;
        }

        private bool OwnGroupCheckedCanExecute(object sender)
        {
            return true;
        }

        private bool JoinedGroupCheckedCanExecute(object sender)
        {
            return true;
        }

        private void OwnPageCheckedExecute(object sender)
        {
            AccountDetailsSelectorView.Filter += null;

            if (IsOwnPageSelected)
                AccountDetailsSelectorView.Filter += FilterByOwnPageSelect;
            else
                AccountDetailsSelectorView.Filter += FilterByOwnPageRemove;
        }

        private bool FilterByOwnPageRemove(object sender)
        {
            try
            {
                var objAccountDetailsSelectorModel = sender as AccountDetailsSelectorModel;

                if (IsLikedPageSelected)
                    return objAccountDetailsSelectorModel != null &&
                           objAccountDetailsSelectorModel.IsFanpage &&
                           !objAccountDetailsSelectorModel.IsOwnPage;
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private bool FilterByOwnPageSelect(object sender)
        {
            try
            {
                var objAccountDetailsSelectorModel = sender as AccountDetailsSelectorModel;

                if (!IsLikedPageSelected)
                    return objAccountDetailsSelectorModel != null &&
                           objAccountDetailsSelectorModel.IsFanpage &&
                           objAccountDetailsSelectorModel.IsOwnPage;
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private void LikedPageCheckedExecute(object sender)
        {
            AccountDetailsSelectorView.Filter += null;
            if (IsLikedPageSelected)
                AccountDetailsSelectorView.Filter += FilterByLikedPageSelected;
            else
                AccountDetailsSelectorView.Filter += FilterByLikedPageRemove;
        }

        private bool FilterByLikedPageSelected(object sender)
        {
            try
            {
                var objAccountDetailsSelectorModel = sender as AccountDetailsSelectorModel;

                if (!IsOwnPageSelected)
                    return objAccountDetailsSelectorModel != null &&
                           objAccountDetailsSelectorModel.IsFanpage &&
                           objAccountDetailsSelectorModel.IsLikePage;
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private bool FilterByLikedPageRemove(object sender)
        {
            try
            {
                var objAccountDetailsSelectorModel = sender as AccountDetailsSelectorModel;

                if (IsOwnPageSelected)
                    return objAccountDetailsSelectorModel != null &&
                           objAccountDetailsSelectorModel.IsFanpage &&
                           !objAccountDetailsSelectorModel.IsLikePage;
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private void OwnGroupCheckedExecute(object sender)
        {
            AccountDetailsSelectorView.Filter += null;
            if (IsOwnPageSelected)
                AccountDetailsSelectorView.Filter += FilterByOwnGroupSelect;
            else
                AccountDetailsSelectorView.Filter += FilterByOwnGroupRemove;
        }

        private void JoinedGroupCheckedExecute(object sender)
        {
            AccountDetailsSelectorView.Filter += null;

            if (IsLikedPageSelected)
                AccountDetailsSelectorView.Filter += FilterByJoinedGroupSelected;
            else
                AccountDetailsSelectorView.Filter += FilterByJoinedGroupRemove;
        }

        private bool FilterByJoinedGroupRemove(object sender)
        {
            try
            {
                var objAccountDetailsSelectorModel = sender as AccountDetailsSelectorModel;

                if (IsOwnPageSelected)
                    return objAccountDetailsSelectorModel != null &&
                           objAccountDetailsSelectorModel.IsGroup &&
                           !objAccountDetailsSelectorModel.IsJoinedGroup;
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }


        private bool FilterByOwnGroupSelect(object sender)
        {
            try
            {
                var objAccountDetailsSelectorModel = sender as AccountDetailsSelectorModel;

                if (!IsOwnPageSelected)
                    return objAccountDetailsSelectorModel != null &&
                           objAccountDetailsSelectorModel.IsGroup &&
                           objAccountDetailsSelectorModel.IsOwnGroup;
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private bool FilterByOwnGroupRemove(object sender)
        {
            try
            {
                var objAccountDetailsSelectorModel = sender as AccountDetailsSelectorModel;

                if (IsLikedPageSelected)
                    return objAccountDetailsSelectorModel != null &&
                           objAccountDetailsSelectorModel.IsGroup &&
                           !objAccountDetailsSelectorModel.IsOwnGroup;
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private bool FilterByJoinedGroupSelected(object sender)
        {
            try
            {
                var objAccountDetailsSelectorModel = sender as AccountDetailsSelectorModel;

                if (!IsLikedPageSelected)
                    return objAccountDetailsSelectorModel != null &&
                           objAccountDetailsSelectorModel.IsGroup &&
                           objAccountDetailsSelectorModel.IsJoinedGroup;
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
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
            var CallsList = AccountDetailsSelectorView
                .Cast<AccountDetailsSelectorModel>().ToList();

            CallsList.Select(x =>
            {
                x.IsSelected = true;
                return x;
            }).ToList();
            //ListAccountDetailsSelectorModels.Select(x =>
            //{
            //    x.IsSelected = true; return x;
            //}).ToList();
        }

        public void SelectNoneCampaign()
        {
            var CallsList = AccountDetailsSelectorView
                .Cast<AccountDetailsSelectorModel>().ToList();

            CallsList.Select(x =>
            {
                x.IsSelected = false;
                return x;
            }).ToList();
            //ListAccountDetailsSelectorModels.Select(x =>
            //{
            //    x.IsSelected = false; return x;
            //}).ToList();
        }

        public IEnumerable<KeyValuePair<string, string>> GetSelectedItems()
        {
            var selectedItems = new List<KeyValuePair<string, string>>();
            ListAccountDetailsSelectorModels.ForEach(x =>
            {
                if (x.IsSelected)
                    selectedItems.Add(new KeyValuePair<string, string>(x.AccountId, x.DetailUrl));
            });
            return selectedItems;
        }

        public IEnumerable<KeyValuePair<string, string>> GetSelectedSection()
        {
            var SelectedSection = new List<KeyValuePair<string, string>>();
            ListAccountDetailsSelectorModels.ForEach(x =>
            {
                if (x.IsSelected)
                    SelectedSection.Add(new KeyValuePair<string, string>(x.DetailName, x.DetailSection));
            });
            return SelectedSection;
        }
        public IEnumerable<KeyValuePair<string, string>> GetNonSelectedItems()
        {
            var selectedItems = new List<KeyValuePair<string, string>>();
            ListAccountDetailsSelectorModels.ForEach(x =>
            {
                if (!x.IsSelected)
                    selectedItems.Add(new KeyValuePair<string, string>(x.AccountId, x.DetailUrl));
            });
            return selectedItems;
        }

        public IEnumerable<PublisherDestinationDetailsModel> GetSelectedItemsDestinations(string destinationType)
        {
            var selectedDestinations = new List<PublisherDestinationDetailsModel>();

            ListAccountDetailsSelectorModels.ForEach(x =>
            {
                if (x.IsSelected)
                {
                    var Section = new List<SectionDetails>();
                    x.ListOfSelectedSections.ForEach(section => { Section.Add(new SectionDetails() { AccountId = section.AccountId, AccountName = section.AccountName, SectionId = section.SectionId, SectionTitle = section.SectionTitle, BoardUrl = section.BoardUrl, BoardName = section.BoardName, Network = section.Network, IsSelected = x.IsSelected }); });
                    selectedDestinations.Add(new PublisherDestinationDetailsModel
                    {
                        AccountId = x.AccountId,
                        DestinationType = destinationType,
                        DestinationUrl = x.DetailUrl,
                        SocialNetworks = x.Network,
                        PublisherPostlistModel = new PublisherPostlistModel(),
                        DestinationGuid = Utilities.GetGuid(),
                        AccountName = x.AccountName,
                        ListOfSection = new KeyValuePair<string, List<SectionDetails>>(x.DetailUrl, Section)
                    });
                }
            });

            return selectedDestinations;
        }
    }
}