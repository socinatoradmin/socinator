#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Models
{
    public class ReportModel : BindableBase
    {
        public ObservableCollection<ContentSelectGroup> AccountList { get; set; } =
            new ObservableCollection<ContentSelectGroup>();

        public ObservableCollection<ContentSelectGroup> QueryList { get; set; } =
            new ObservableCollection<ContentSelectGroup>();

        public ObservableCollection<ContentSelectGroup> StatusList { get; set; } =
            new ObservableCollection<ContentSelectGroup>();

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        private ICollectionView _reportCollection;

        public ICollectionView ReportCollection
        {
            get => _reportCollection;
            set => SetProperty(ref _reportCollection, value);
        }

        private ObservableCollection<object> _lstReports;

        public ObservableCollection<object> LstReports
        {
            get => _lstReports;
            set
            {
                if (_lstReports == value) return;
                _lstReports = value;
                OnPropertyChanged(nameof(LstReports));
            }
        }
        private ObservableCollection<object> _currentPageLstReports;

        public ObservableCollection<object> CurrentPageLstReports
        {
            get => _currentPageLstReports;
            set => SetProperty(ref _currentPageLstReports, value);
        }

        private int _currentPage = 1;

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value && value > 0 && value <= TotalPages)
                {
                    _currentPage = value;
                    OnPropertyChanged(nameof(CurrentPage));
                }
            }
        }
        private int _totalPages = 1;

        public int TotalPages
        {
            get
            {
                return _totalPages;
            }
            set
            {
                if (_totalPages == value) return;
                _totalPages = value;
                OnPropertyChanged(nameof(TotalPages));
            }
        }

        private int _pageSize=30;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value;
        }

        private int _totalReportCount;

        public int TotalReportCount
        {
            get
            {
                return _totalReportCount;
            }
            set
            {
                if (_totalReportCount == value) return;
                _totalReportCount = value;
                OnPropertyChanged(nameof(TotalReportCount));
            }
        }

        private bool _loadPreviousEnable;

        public bool LoadPreviousEnable
        {
            get
            {
                return _loadPreviousEnable;
            }
            set
            {
                if (_loadPreviousEnable == value) return;
                _loadPreviousEnable = value;
                OnPropertyChanged(nameof(LoadPreviousEnable));
            }
        }

        private bool _loadNextEnable;

        public bool LoadNextEnable
        {
            get
            {
                return _loadNextEnable;
            }
            set
            {
                if (_loadNextEnable == value) return;
                _loadNextEnable = value;
                OnPropertyChanged(nameof(LoadNextEnable));
            }
        }
        public ActivityType ActivityType { get; set; }

        public ObservableCollection<GridViewColumnDescriptor> GridViewColumn { get; set; } =
            new ObservableCollection<GridViewColumnDescriptor>();

        public List<KeyValuePair<string, string>> LstCurrentQueries = new List<KeyValuePair<string, string>>();

        public string CampaignId { get; set; } = string.Empty;
        private bool _FollowRate;

        public bool FollowRate
        {
            get => _FollowRate;
            set => SetProperty(ref _FollowRate, value);
        }
    }
}