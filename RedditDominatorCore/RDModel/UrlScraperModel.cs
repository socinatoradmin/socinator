using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RedditDominatorCore.RDModel
{
    public class UrlScraperModel : ModuleSetting, IGeneralSettings
    {
        private bool _isAccountGrowthActive;
        private List<string> _lstOfSortbyFilter = new List<string>
            {"Relevance","Hot", "Top", "New","Most Comments"};

        private List<string> _lstOfSortbyTime = new List<string>
            {"All Time","Past Year", "Past Month", "Past Week","Past 24 Hours","Past Hour"};
        private List<string> _lstOfSortbyTopTime = new List<string>
            {"All Time", "Now","Today","This Week","This Month","This Year"};
        private string _selectedValue = "Relevance";
        private string _selectedValueTimeFilter = "All Time";
        private string _selectedValueTopTimeFilter = "All Time";
        private bool _IsCheckedSortByCategory;
        private bool _IsCheckedSortByTime;
        private bool _IsEnabledSafeSearch;
        private Visibility _SortByTimeVisibility;
        public Visibility SortByTimeVisibility
        {
            get => _SortByTimeVisibility;
            set
            {
                if (_SortByTimeVisibility != value)
                {
                    SetProperty(ref _SortByTimeVisibility, value);
                }
            }
        }
        public bool IsCheckedSortByCategory
        {
            get => _IsCheckedSortByCategory;
            set
            {
                if (_IsCheckedSortByCategory == value)
                    return;
                SetProperty(ref _IsCheckedSortByCategory, value);
            }
        }

        private bool _IsCheckedSearchPostFilter;

        public bool IsCheckedSearchPostFilter
        {
            get => _IsCheckedSearchPostFilter;
            set
            {
                if (_IsCheckedSearchPostFilter == value)
                    return;
                SetProperty(ref _IsCheckedSearchPostFilter, value);
            }
        }

        private bool _IsEnabledCommunityOrUserPostFilter;

        public bool IsEnabledCommunityOrUserPostFilter
        {
            get => _IsEnabledCommunityOrUserPostFilter;
            set
            {
                if (_IsEnabledCommunityOrUserPostFilter == value)
                    return;
                SetProperty(ref _IsEnabledCommunityOrUserPostFilter, value);
            }
        }

        private bool _IsCheckedSortByNew;

        public bool IsCheckedSortByNew
        {
            get => _IsCheckedSortByNew;
            set
            {
                if (_IsCheckedSortByNew == value)
                    return;
                if (value)
                {
                    IsCheckedSortByHot = false;
                    IsCheckedSortByTop = false;
                }
                SetProperty(ref _IsCheckedSortByNew, value);
            }
        }

        private bool _IsCheckedSortByHot;

        public bool IsCheckedSortByHot
        {
            get => _IsCheckedSortByHot;
            set
            {
                if (_IsCheckedSortByHot == value)
                    return;
                if (value)
                {
                    IsCheckedSortByNew = false;
                    IsCheckedSortByTop = false;
                }
                SetProperty(ref _IsCheckedSortByHot, value);
            }
        }

        private bool _IsCheckedSortByTop;

        public bool IsCheckedSortByTop
        {
            get => _IsCheckedSortByTop;
            set
            {
                if (_IsCheckedSortByTop == value)
                    return;
                if (value)
                {
                    IsCheckedSortByHot = false;
                    IsCheckedSortByNew = false;
                }
                SetProperty(ref _IsCheckedSortByTop, value);
            }
        }


        public bool IsCheckedSortByTime
        {
            get => _IsCheckedSortByTime; set
            {
                if (_IsCheckedSortByTime == value)
                    return;
                SetProperty(ref _IsCheckedSortByTime, value);
            }
        }
        public bool IsEnabledSafeSearch
        {
            get => _IsEnabledSafeSearch; set
            {
                if (_IsEnabledSafeSearch == value)
                    return;
                SetProperty(ref _IsEnabledSafeSearch, value);
            }
        }
        public JobConfiguration FastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(334, 500),
            ActivitiesPerHour = new RangeUtilities(40, 60),
            ActivitiesPerWeek = new RangeUtilities(2000, 3000),
            ActivitiesPerJob = new RangeUtilities(35, 50),
            DelayBetweenJobs = new RangeUtilities(65, 90),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(200, 300),
            ActivitiesPerHour = new RangeUtilities(30, 40),
            ActivitiesPerWeek = new RangeUtilities(1200, 1800),
            ActivitiesPerJob = new RangeUtilities(25, 35),
            DelayBetweenJobs = new RangeUtilities(65, 98),
            DelayBetweenActivity = new RangeUtilities(50, 90),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SlowSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(68, 100),
            ActivitiesPerHour = new RangeUtilities(10, 15),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(74, 110),
            DelayBetweenActivity = new RangeUtilities(60, 120),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration SuperfastSpeed = new JobConfiguration
        {
            ActivitiesPerDay = new RangeUtilities(668, 1000),
            ActivitiesPerHour = new RangeUtilities(80, 100),
            ActivitiesPerWeek = new RangeUtilities(4000, 6000),
            ActivitiesPerJob = new RangeUtilities(60, 80),
            DelayBetweenJobs = new RangeUtilities(70, 104),
            DelayBetweenActivity = new RangeUtilities(15, 30),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        [ProtoMember(1)]
        public override ObservableCollection<QueryInfo> SavedQueries { get; set; } =
            new ObservableCollection<QueryInfo>();

        public List<string> ListQueryType { get; set; } = new List<string>();

        public RunningTimes RunningTimes { get; set; } = new RunningTimes();

        [ProtoMember(2)] public override PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();

        public List<string> LstOfSortbyFilter
        {
            get => _lstOfSortbyFilter.Distinct().ToList();
            set
            {
                if (_lstOfSortbyFilter == value)
                    return;
                SetProperty(ref _lstOfSortbyFilter, value);
            }
        }
        public List<string> LstOfSortbyTime
        {
            get => _lstOfSortbyTime.Distinct().ToList();
            set
            {
                if (_lstOfSortbyTime == value)
                    return;
                SetProperty(ref _lstOfSortbyTime, value);
            }
        }
        public List<string> LstOfSortbyTopTime
        {
            get => _lstOfSortbyTopTime.Distinct().ToList();
            set
            {
                if (_lstOfSortbyTopTime == value)
                    return;
                SetProperty(ref _lstOfSortbyTopTime, value);
            }
        }
        [ProtoMember(4)]
        public string SelectedValue
        {
            get => _selectedValue;
            set
            {
                if (_selectedValue == value)
                    return;
                SortByTimeVisibility = (value != null && value != "New" && value != "Hot") ? Visibility.Visible : Visibility.Collapsed;
                if (SortByTimeVisibility == Visibility.Collapsed)
                    IsCheckedSortByTime = false;
                SetProperty(ref _selectedValue, value);
            }
        }
        public string SelectedValueTimeFilter
        {
            get => _selectedValueTimeFilter;
            set
            {
                if (_selectedValueTimeFilter == value)
                    return;
                SetProperty(ref _selectedValueTimeFilter, value);
            }
        }

        public string SelectedValueTopTimeFilter
        {
            get => _selectedValueTopTimeFilter;
            set
            {
                if (_selectedValueTopTimeFilter == value)
                    return;
                SetProperty(ref _selectedValueTopTimeFilter, value);
            }
        }
        public override JobConfiguration JobConfiguration { get; set; } = new JobConfiguration();

        public bool IsAccountGrowthActive
        {
            get => _isAccountGrowthActive;
            set
            {
                if (_isAccountGrowthActive == value) return;
                SetProperty(ref _isAccountGrowthActive, value);
            }
        }
    }
}