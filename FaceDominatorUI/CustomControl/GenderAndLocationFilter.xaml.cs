using DominatorHouseCore.Annotations;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.FilterModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FaceDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for GenderAndLocationFilter.xaml
    /// </summary>
    public partial class GenderAndLocationFilter : INotifyPropertyChanged
    {
        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GenderFilterProperty =
            DependencyProperty.Register("GenderandLocationFilter", typeof(FdGenderAndLocationFilterModel),
                typeof(GenderAndLocationFilter), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty IsSaveCloseButtonVisisbleProperty =
            DependencyProperty.Register("IsSaveCloseButtonVisisble", typeof(bool), typeof(GenderAndLocationFilter),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });


        public static readonly DependencyProperty IsMutualFriendFilterRequiredProperty =
            DependencyProperty.Register("IsMutualFriendFilterRequired", typeof(bool), typeof(GenderAndLocationFilter),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });


        public static readonly DependencyProperty IsGroupMemberFilterNeededProperty =
            DependencyProperty.Register("IsGroupMemberFilterNeeded", typeof(bool), typeof(GenderAndLocationFilter),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });
        public static readonly DependencyProperty IsInteractedDaysFilterNeededProperty =
            DependencyProperty.Register("IsInteractedDaysFilterNeeded", typeof(bool), typeof(GenderAndLocationFilter),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty RadioGroupNameProperty =
            DependencyProperty.Register("RadioGroupName", typeof(string), typeof(GenderAndLocationFilter),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        private bool _isAllCitySelected;

        private bool _isCitiesLoadProgressActive;

        private bool _isCityListVisible;

        private bool _isProgressActive = true;


        //        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        //        {
        //            SaveUserFilterEventArgsHandler();
        //        }

        //        void SaveUserFilterEventArgsHandler()
        //        {
        //            var rountedargs = new RoutedEventArgs(SaveGenderFilterEvent);
        //            RaiseEvent(rountedargs);
        //
        //        }

        //        private static readonly RoutedEvent SaveGenderFilterEvent =
        //            EventManager.RegisterRoutedEvent("SaveGenderFilterEventHandler", RoutingStrategy.Bubble,
        //                typeof(RoutedEventHandler), typeof(GenderAndLocationFilter));
        //
        //        public event RoutedEventHandler SaveUserFilterEventHandler
        //        {
        //            add { AddHandler(SaveGenderFilterEvent, value); }
        //            remove { RemoveHandler(SaveGenderFilterEvent, value); }
        //        }

        private ObservableCollection<LocationModel> _listlocationModelView = new ObservableCollection<LocationModel>();

        private string _searchText;

        private string _selectedCountry;

        private string _selectedText;

        //Get Global Database
        private readonly IGlobalDatabaseConnection dataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();

        public GenderAndLocationFilter()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            var dbGlobalContext = dataBaseConnectionGlb.GetSqlConnection();
            DbGlobalListOperations = new DbOperations(dbGlobalContext);
            //InitializeLocations();
        }

        private DbOperations DbGlobalListOperations { get; }

        public FdGenderAndLocationFilterModel GenderandLocationFilter
        {
            get => (FdGenderAndLocationFilterModel)GetValue(GenderFilterProperty);
            set => SetValue(GenderFilterProperty, value);
        }

        public bool IsSaveCloseButtonVisisble
        {
            get => (bool)GetValue(IsSaveCloseButtonVisisbleProperty);
            set => SetValue(IsSaveCloseButtonVisisbleProperty, value);
        }


        public bool IsMutualFriendFilterRequired
        {
            get => (bool)GetValue(IsMutualFriendFilterRequiredProperty);
            set => SetValue(IsMutualFriendFilterRequiredProperty, value);
        }

        public bool IsGroupMemberFilterNeeded
        {
            get => (bool)GetValue(IsGroupMemberFilterNeededProperty);
            set => SetValue(IsGroupMemberFilterNeededProperty, value);
        }
        public bool IsInteractedDaysFilterNeeded
        {
            get => (bool)GetValue(IsInteractedDaysFilterNeededProperty);
            set => SetValue(IsInteractedDaysFilterNeededProperty, value);
        }
        public string RadioGroupName
        {
            get => (string)GetValue(RadioGroupNameProperty);
            set => SetValue(RadioGroupNameProperty, value);
        }

        public ObservableCollection<LocationModel> ListLocationModelView
        {
            get => _listlocationModelView;
            set
            {
                _listlocationModelView = value;
                NotifyPropertyChanged(nameof(ListLocationModelView));
            }
        }

        private List<LocationModel> ListLocationModelStorage { get; set; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                NotifyPropertyChanged(nameof(SearchText));
                InputChangExecute(_searchText);
            }
        }

        public string SelectedText
        {
            get => _selectedText;
            set
            {
                if (_selectedText == value)
                    return;

                _selectedText = value;
                NotifyPropertyChanged(nameof(SelectedText));
                CountryInputChangExecute(value);
            }
        }

        public string SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                _selectedCountry = value;
                NotifyPropertyChanged(nameof(SelectedCountry));
                CitiesList(value);
            }
        }

        public bool IsProgressActive
        {
            get => _isProgressActive;
            set
            {
                _isProgressActive = value;
                NotifyPropertyChanged(nameof(IsProgressActive));
            }
        }

        public bool IsCitiesLoadProgressActive
        {
            get => _isCitiesLoadProgressActive;
            set
            {
                _isCitiesLoadProgressActive = value;
                NotifyPropertyChanged(nameof(IsCitiesLoadProgressActive));
            }
        }

        public bool IsCityListVisible
        {
            get => _isCityListVisible;
            set
            {
                _isCityListVisible = value;
                NotifyPropertyChanged(nameof(IsCityListVisible));
            }
        }

        public bool IsAllCitySelected
        {
            get => _isAllCitySelected;
            set
            {
                _isAllCitySelected = value;
                SelectCitiesList(value);
                NotifyPropertyChanged(nameof(IsAllCitySelected));
            }
        }

        private CancellationTokenSource CancellationToken { get; set; } = new CancellationTokenSource();
        private CancellationTokenSource LoadUiCancellationToken { get; set; }

        private int CurrentSearchCount { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void Combo_Category_SelectionChanged(object sender, EventArgs e)
        {
        }

        //private List<LocationModel> ListLocationModelStorage

        private void SelectCitiesList(bool value)
        {
            ListLocationModelView.ForEach(x => x.IsSelected = value);
        }


        private void CitiesList(string value)
        {
            var isRunningProces = true;
            CancellationToken.Cancel();
            IsCitiesLoadProgressActive = true;
            var tempLocationUrls = new List<string>();
            var selectedLocation = ListLocationModelView.Where(x => x.IsSelected).ToList();
            GenderandLocationFilter.ListLocationUrlPair =
                GenderandLocationFilter.ListLocationUrlPair ?? new List<KeyValuePair<string, string>>();
            ListLocationModelView.Clear();
            Task.Factory.StartNew(() =>
            {
                CancellationToken = new CancellationTokenSource();
                List<LocationModel> temp;
                while (IsProgressActive)
                    Thread.Sleep(100);

                var selectedCityList = (from location in selectedLocation
                                        select new KeyValuePair<string, string>
                                            (location.CountryName, location.CityName)).ToList();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    GenderandLocationFilter.ListLocationUrlPair.AddRange(selectedCityList);
                    tempLocationUrls = GenderandLocationFilter.ListLocationUrlPair.Where(x => x.Key == value)
                        .Select(x => x.Value).ToList();
                    isRunningProces = false;
                });

                while (isRunningProces)
                    Thread.Sleep(100);

                tempLocationUrls = tempLocationUrls.Distinct().ToList();

                var currentCities = DbGlobalListOperations.Get<LocationList>(x => x.CountryName == value);

                try
                {
                    CancellationToken.Token.ThrowIfCancellationRequested();
                    temp = (from cityData in currentCities
                            let city = cityData.CityName
                            let country = cityData.CountryName
                            let isChecked = tempLocationUrls.Contains(city)
                            select new LocationModel
                            {
                                CityName = city,
                                CountryName = country,
                                IsSelected = isChecked
                            }).ToList();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GenderandLocationFilter.ListLocationUrlPair.RemoveAll(x => x.Key == SelectedCountry);
                        IsAllCitySelected = temp.All(y => y.IsSelected);
                        ListLocationModelView.AddRange(temp);
                        if (ListLocationModelView.Count > 0)
                            IsCityListVisible = true;
                        IsCitiesLoadProgressActive = false;
                    });
                }
                catch (Exception)
                {
                }
            });
        }


        private void InputChangExecute(string input)
        {
            if (input.Length < CurrentSearchCount)
                ListLocationModelView = new ObservableCollection<LocationModel>(ListLocationModelStorage);

            var temp = ListLocationModelView.Where(x =>
                x.CountryName.Equals(SelectedCountry) &&
                x.CityName.StartsWith(input, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (temp.Count == ListLocationModelView.Count)
            {
            }
            else
            {
                if (CurrentSearchCount == 0)
                    ListLocationModelStorage = ListLocationModelView.ToList();
                ListLocationModelView.Clear();
                ListLocationModelView.AddRange(temp);
                CurrentSearchCount = input.Length;
            }
        }

        private void CountryInputChangExecute(string input)
        {
            var temp = GenderandLocationFilter.ListLocationModel.Where(x => x.CountryName.Equals(input)).ToList();
            ListLocationModelView.Clear();
            ListLocationModelView.AddRange(temp);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsProgressActive = true;

            if (GenderandLocationFilter.ListLocationUrlPair.Count == 0 &&
                !ListLocationModelView.Any(x => x.IsSelected))
            {
                Dialog.ShowDialog("Error", "Please Select Atleast One Location Detail");
                IsProgressActive = false;
                return;
            }

            var selectedLocationList = ListLocationModelView.Where(x => x.IsSelected).ToList();


            Task.Factory.StartNew(() =>
            {
                var selectedCityList = (from location in selectedLocationList
                                        select new KeyValuePair<string, string>
                                            (location.CountryName, location.CityName)).ToList();

                while (selectedLocationList.Count == 0)
                    Thread.Sleep(200);


                Application.Current.Dispatcher.Invoke(() =>
                {
                    GenderandLocationFilter.ListLocationUrlPair.AddRange(selectedCityList);
                    GenderandLocationFilter.ListLocationUrlPair = GenderandLocationFilter.ListLocationUrlPair
                        .GroupBy(c => new { c.Key, c.Value }).Select(c => c.First()).ToList();
                });


                Application.Current.Dispatcher.Invoke(() => { IsProgressActive = false; });

                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Facebook, "", "",
                    "Locations Detail Added Successfully");
            });
        }

        private void ComboBox_SelectionChanged(object sender, EventArgs e)
        {
            if (GenderandLocationFilter.ListCountry == null || GenderandLocationFilter.ListCountry?.Count <= 0)
                Dialog.ShowDialog("Location Details Are Empty",
                    "Please Download Location Details For Specific Country From Other Configurations");
        }

        private async Task InitializeLocations()
        {
            try
            {
                if (LoadUiCancellationToken != null)
                    return;

                LoadUiCancellationToken = new CancellationTokenSource();

                while (GenderandLocationFilter == null)
                    await Task.Delay(100);

                if (GenderandLocationFilter.ListLocationModel.Count > 0)
                    return;

                GenderandLocationFilter.ListCountry = new ObservableCollection<string>();


                await Task.Factory.StartNew(() =>
                {
                    var countryList = DbGlobalListOperations
                        .GetUniqueSingleColumn<LocationList>(x => x.CountryName, y => y.CountryName)
                        .Where(x => x != "India" || x != "China").ToList();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GenderandLocationFilter.ListCountry.AddRange(countryList);
                        IsProgressActive = false;
                    });
                });
            }
            catch (Exception)
            {
            }

            LoadUiCancellationToken = null;
        }

        private void InitializeLocation(object sender, RoutedEventArgs e)
        {
            InitializeLocations();
        }
    }
}