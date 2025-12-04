#region

using System.Collections.ObjectModel;
using System.Windows;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class SoftwareSettingsModel : BindableBase
    {
        private bool _isRunDHAtStartUpChecked;

        [ProtoMember(1)]
        public bool IsRunDHAtStartUpChecked
        {
            get => _isRunDHAtStartUpChecked;
            set
            {
                if (value == _isRunDHAtStartUpChecked)
                    return;
                SetProperty(ref _isRunDHAtStartUpChecked, value);
            }
        }

        private bool _isShowToolTipToSystemTrayChecked;

        [ProtoMember(2)]
        public bool IsShowToolTipToSystemTrayChecked
        {
            get => _isShowToolTipToSystemTrayChecked;
            set
            {
                if (value == _isShowToolTipToSystemTrayChecked)
                    return;
                SetProperty(ref _isShowToolTipToSystemTrayChecked, value);
            }
        }

        private bool _isRecreateDesktopshortcutAtStartupChecked;

        [ProtoMember(3)]
        public bool IsRecreateDesktopshortcutAtStartupChecked
        {
            get => _isRecreateDesktopshortcutAtStartupChecked;
            set
            {
                if (value == _isRecreateDesktopshortcutAtStartupChecked)
                    return;
                SetProperty(ref _isRecreateDesktopshortcutAtStartupChecked, value);
            }
        }

        private bool _isStartAppMinimizedChecked;

        [ProtoMember(4)]
        public bool IsStartAppMinimizedChecked
        {
            get => _isStartAppMinimizedChecked;
            set
            {
                if (value == _isStartAppMinimizedChecked)
                    return;
                SetProperty(ref _isStartAppMinimizedChecked, value);
            }
        }

        private bool _isShowSameAccountsChecked;

        [ProtoMember(5)]
        public bool IsShowSameAccountsChecked
        {
            get => _isShowSameAccountsChecked;
            set
            {
                if (value == _isShowSameAccountsChecked)
                    return;
                SetProperty(ref _isShowSameAccountsChecked, value);
            }
        }

        private bool _isEnableGlobalNightModeChecked;

        [ProtoMember(6)]
        public bool IsEnableGlobalNightModeChecked
        {
            get => _isEnableGlobalNightModeChecked;
            set
            {
                if (value == _isEnableGlobalNightModeChecked)
                    return;
                SetProperty(ref _isEnableGlobalNightModeChecked, value);
            }
        }

        private RangeUtilities _executeAnyActionsBetween = new RangeUtilities();

        [ProtoMember(7)]
        public RangeUtilities ExecuteAnyActionsBetween
        {
            get => _executeAnyActionsBetween;
            set
            {
                if (value == _executeAnyActionsBetween)
                    return;
                SetProperty(ref _executeAnyActionsBetween, value);
            }
        }

        private bool _isRandomiseNightModeChecked;

        [ProtoMember(8)]
        public bool IsRandomiseNightModeChecked
        {
            get => _isRandomiseNightModeChecked;
            set
            {
                if (value == _isRandomiseNightModeChecked)
                    return;
                SetProperty(ref _isRandomiseNightModeChecked, value);
            }
        }

        private RangeUtilities _startBetween = new RangeUtilities();

        [ProtoMember(9)]
        public RangeUtilities StartBetween
        {
            get => _startBetween;
            set
            {
                if (value == _startBetween)
                    return;
                SetProperty(ref _startBetween, value);
            }
        }

        private RangeUtilities _endBetween = new RangeUtilities();

        [ProtoMember(10)]
        public RangeUtilities EndBetween
        {
            get => _endBetween;
            set
            {
                if (value == _endBetween)
                    return;
                SetProperty(ref _endBetween, value);
            }
        }

        private bool _isIgnoreGlobalChecked;

        [ProtoMember(11)]
        public bool IsIgnoreGlobalChecked
        {
            get => _isIgnoreGlobalChecked;
            set
            {
                if (value == _isIgnoreGlobalChecked)
                    return;
                SetProperty(ref _isIgnoreGlobalChecked, value);
            }
        }

        private bool _isEnableGlobalFindReplaceChecked;

        [ProtoMember(12)]
        public bool IsEnableGlobalFindReplaceChecked
        {
            get => _isEnableGlobalFindReplaceChecked;
            set
            {
                if (value == _isEnableGlobalFindReplaceChecked)
                    return;
                SetProperty(ref _isEnableGlobalFindReplaceChecked, value);
            }
        }

        private int _waitMinimumOf;

        [ProtoMember(13)]
        public int WaitMinimumOf
        {
            get => _waitMinimumOf;
            set
            {
                if (value == _waitMinimumOf)
                    return;
                SetProperty(ref _waitMinimumOf, value);
            }
        }

        private bool _isUseTabInsteadOfCommaChecked;

        [ProtoMember(14)]
        public bool IsUseTabInsteadOfCommaChecked
        {
            get => _isUseTabInsteadOfCommaChecked;
            set
            {
                if (value == _isUseTabInsteadOfCommaChecked)
                    return;
                SetProperty(ref _isUseTabInsteadOfCommaChecked, value);
            }
        }

        private bool _isEnableContactDifferentUserChecked;

        [ProtoMember(15)]
        public bool IsEnableContactDifferentUserChecked
        {
            get => _isEnableContactDifferentUserChecked;
            set
            {
                if (value == _isEnableContactDifferentUserChecked)
                    return;
                SetProperty(ref _isEnableContactDifferentUserChecked, value);
            }
        }

        private bool _isWhenPostingVideosSelectRandomFrameChecked;

        [ProtoMember(16)]
        public bool IsWhenPostingVideosSelectRandomFrameChecked
        {
            get => _isWhenPostingVideosSelectRandomFrameChecked;
            set
            {
                if (value == _isWhenPostingVideosSelectRandomFrameChecked)
                    return;
                SetProperty(ref _isWhenPostingVideosSelectRandomFrameChecked, value);
            }
        }

        private bool _isChangeVideoHashBeforePostingChecked;

        [ProtoMember(17)]
        public bool IsChangeVideoHashBeforePostingChecked
        {
            get => _isChangeVideoHashBeforePostingChecked;
            set
            {
                if (value == _isChangeVideoHashBeforePostingChecked)
                    return;
                SetProperty(ref _isChangeVideoHashBeforePostingChecked, value);
            }
        }

        private bool _isDoNotShowInfoMessegesChecked;

        [ProtoMember(18)]
        public bool IsDoNotShowInfoMessegesChecked
        {
            get => _isDoNotShowInfoMessegesChecked;
            set
            {
                if (value == _isDoNotShowInfoMessegesChecked)
                    return;
                SetProperty(ref _isDoNotShowInfoMessegesChecked, value);
            }
        }

        private bool _isUseLocalPcCultureChecked;

        [ProtoMember(19)]
        public bool IsUseLocalPcCultureChecked
        {
            get => _isUseLocalPcCultureChecked;
            set
            {
                if (value == _isUseLocalPcCultureChecked)
                    return;
                SetProperty(ref _isUseLocalPcCultureChecked, value);
            }
        }

        private bool _isEnableAutoRestartSocinatorChecked;

        [ProtoMember(20)]
        public bool IsEnableAutoRestartSocinatorChecked
        {
            get => _isEnableAutoRestartSocinatorChecked;
            set
            {
                if (value == _isEnableAutoRestartSocinatorChecked)
                    return;
                SetProperty(ref _isEnableAutoRestartSocinatorChecked, value);
            }
        }

        private int _minHour;

        [ProtoMember(21)]
        public int MinHour
        {
            get => _minHour;
            set
            {
                if (value == _minHour)
                    return;
                SetProperty(ref _minHour, value);
            }
        }

        private RangeUtilities _availableUpdatesBetween = new RangeUtilities();

        [ProtoMember(22)]
        public RangeUtilities AvailableUpdatesBetween
        {
            get => _availableUpdatesBetween;
            set
            {
                if (value == _availableUpdatesBetween)
                    return;
                SetProperty(ref _availableUpdatesBetween, value);
            }
        }

        private int _limitNumberOfRecords;

        [ProtoMember(23)]
        public int LimitNumberOfRecords
        {
            get => _limitNumberOfRecords;
            set
            {
                if (value == _limitNumberOfRecords)
                    return;
                SetProperty(ref _limitNumberOfRecords, value);
            }
        }

        private int _limitNumberOfDeshboardSummary;

        [ProtoMember(24)]
        public int LimitNumberOfDeshboardSummary
        {
            get => _limitNumberOfDeshboardSummary;
            set
            {
                if (value == _limitNumberOfDeshboardSummary)
                    return;
                SetProperty(ref _limitNumberOfDeshboardSummary, value);
            }
        }

        private int _limitNumberOfPost;

        [ProtoMember(25)]
        public int LimitNumberOfPost
        {
            get => _limitNumberOfPost;
            set
            {
                if (value == _limitNumberOfPost)
                    return;
                SetProperty(ref _limitNumberOfPost, value);
            }
        }

        private bool _isRecreateDeshboardWindowChecked;

        [ProtoMember(26)]
        public bool IsRecreateDeshboardWindowChecked
        {
            get => _isRecreateDeshboardWindowChecked;
            set
            {
                if (value == _isRecreateDeshboardWindowChecked)
                    return;
                SetProperty(ref _isRecreateDeshboardWindowChecked, value);
            }
        }

        private bool _isForceCampaignsToKeepSameCreationDateChecked;

        [ProtoMember(27)]
        public bool IsForceCampaignsToKeepSameCreationDateChecked
        {
            get => _isForceCampaignsToKeepSameCreationDateChecked;
            set
            {
                if (value == _isForceCampaignsToKeepSameCreationDateChecked)
                    return;
                SetProperty(ref _isForceCampaignsToKeepSameCreationDateChecked, value);
            }
        }

        private bool _isEnablePerformanceAnalysisChecked;

        [ProtoMember(28)]
        public bool IsEnablePerformanceAnalysisChecked
        {
            get => _isEnablePerformanceAnalysisChecked;
            set
            {
                if (value == _isEnablePerformanceAnalysisChecked)
                    return;
                SetProperty(ref _isEnablePerformanceAnalysisChecked, value);
            }
        }

        private string embaddedBrowserUserAgent;

        [ProtoMember(29)]
        public string EmbaddedBrowserUserAgent
        {
            get => embaddedBrowserUserAgent;
            set
            {
                if (value == embaddedBrowserUserAgent)
                    return;
                SetProperty(ref embaddedBrowserUserAgent, value);
            }
        }

        private bool _isResetOpenFileDialogChecked;

        [ProtoMember(30)]
        public bool IsResetOpenFileDialogChecked
        {
            get => _isResetOpenFileDialogChecked;
            set
            {
                if (value == _isResetOpenFileDialogChecked)
                    return;
                SetProperty(ref _isResetOpenFileDialogChecked, value);
            }
        }

        private bool _isDisableSocialProfilesDescriptionTooltipChecked;

        [ProtoMember(31)]
        public bool IsDisableSocialProfilesDescriptionTooltipChecked
        {
            get => _isDisableSocialProfilesDescriptionTooltipChecked;
            set
            {
                if (value == _isDisableSocialProfilesDescriptionTooltipChecked)
                    return;
                SetProperty(ref _isDisableSocialProfilesDescriptionTooltipChecked, value);
            }
        }

        private bool _isFullChecked;

        [ProtoMember(32)]
        public bool IsFullChecked
        {
            get => _isFullChecked;
            set
            {
                if (value == _isFullChecked)
                    return;
                SetProperty(ref _isFullChecked, value);
            }
        }

        private bool _isNormalChecked;

        [ProtoMember(33)]
        public bool IsNormalChecked
        {
            get => _isNormalChecked;
            set
            {
                if (value == _isNormalChecked)
                    return;
                SetProperty(ref _isNormalChecked, value);
            }
        }

        private string _globalFindReplaceText;

        [ProtoMember(34)]
        public string GlobalFindReplaceText
        {
            get => _globalFindReplaceText;
            set
            {
                if (value == _globalFindReplaceText)
                    return;
                SetProperty(ref _globalFindReplaceText, value);
            }
        }

        [ProtoMember(35, IsRequired = false)]
        public bool IsEnableParallelActivitiesChecked
        {
            get => _isEnableParallelActivitiesChecked;
            set
            {
                if (value == _isEnableParallelActivitiesChecked)
                    return;
                SetProperty(ref _isEnableParallelActivitiesChecked, value);
            }
        }

        private int _accountSynchronizationHours = 24;

        [ProtoMember(36)]
        public int AccountSynchronizationHours
        {
            get => _accountSynchronizationHours;
            set
            {
                if (value == _accountSynchronizationHours)
                    return;
                SetProperty(ref _accountSynchronizationHours, value);
            }
        }

        private int _simultaneousAccountUpdate = 5;
        private bool _isEnableParallelActivitiesChecked;

        [ProtoMember(37)]
        public int SimultaneousAccountUpdateCount
        {
            get => _simultaneousAccountUpdate;
            set
            {
                if (value == _simultaneousAccountUpdate)
                    return;
                SetProperty(ref _simultaneousAccountUpdate, value);
            }
        }

        private bool _isEnableAdvancedUserMode;

        [ProtoMember(38)]
        public bool IsEnableAdvancedUserMode
        {
            get => _isEnableAdvancedUserMode;
            set
            {
                if (value == _isEnableAdvancedUserMode)
                    return;
                SetProperty(ref _isEnableAdvancedUserMode, value);
            }
        }

        private bool _isDoNotAutoLoginAccountsWhileAddingToSoftware;

        [ProtoMember(39)]
        public bool IsDoNotAutoLoginAccountsWhileAddingToSoftware
        {
            get => _isDoNotAutoLoginAccountsWhileAddingToSoftware;
            set
            {
                if (value == _isDoNotAutoLoginAccountsWhileAddingToSoftware)
                    return;
                SetProperty(ref _isDoNotAutoLoginAccountsWhileAddingToSoftware, value);
            }
        }

        private int _simultaneousAdsScreperThreadCount = 30;

        [ProtoMember(40)]
        public int SimultaneousAdsScreperThreadCount
        {
            get => _simultaneousAdsScreperThreadCount;
            set
            {
                if (value == _simultaneousAdsScreperThreadCount)
                    return;
                SetProperty(ref _simultaneousAdsScreperThreadCount, value);
            }
        }

        private bool _isStopAutoSynchronizeAccount;

        [ProtoMember(41)]
        public bool IsStopAutoSynchronizeAccount
        {
            get => _isStopAutoSynchronizeAccount;
            set => SetProperty(ref _isStopAutoSynchronizeAccount, value);
        }

        private bool _isDefaultExportPathSelected;

        [ProtoMember(42)]
        public bool IsDefaultExportPathSelected
        {
            get => _isDefaultExportPathSelected;
            set
            {
                SetProperty(ref _isDefaultExportPathSelected, value);
                if (!_isDefaultExportPathSelected) ExportPath = string.Empty;
            }
        }

        private string _exportPath = string.Empty;

        [ProtoMember(43)]
        public string ExportPath
        {
            get => _exportPath;
            set => SetProperty(ref _exportPath, value);
        }

        private bool _isThreadLimitChecked;

        [ProtoMember(45)]
        public bool IsThreadLimitChecked
        {
            get => _isThreadLimitChecked;
            set => SetProperty(ref _isThreadLimitChecked, value);
        }

        private int _maxThreadCount = 10;

        [ProtoMember(46)]
        public int MaxThreadCount
        {
            get => _maxThreadCount;
            set => SetProperty(ref _maxThreadCount, value);
        }

        private bool _runQueriesTopToBottom;

        [ProtoMember(47)]
        public bool RunQueriesTopToBottom
        {
            get => _runQueriesTopToBottom;
            set => SetProperty(ref _runQueriesTopToBottom, value);
        }

        private bool _runQueriesBottomToTop;

        [ProtoMember(48)]
        public bool RunQueriesBottomToTop
        {
            get => _runQueriesBottomToTop;
            set => SetProperty(ref _runQueriesBottomToTop, value);
        }

        private bool _runQueriesRandomly;

        [ProtoMember(49)]
        public bool RunQueriesRandomly
        {
            get => _runQueriesRandomly;
            set => SetProperty(ref _runQueriesRandomly, value);
        }

        private bool _stopIfNoMoreData;

        [ProtoMember(50)]
        public bool StopIfNoMoreData
        {
            get => _stopIfNoMoreData;
            set => SetProperty(ref _stopIfNoMoreData, value);
        }

        private ObservableCollection<LocationModel> _listlocationModel = new ObservableCollection<LocationModel>();

        [ProtoMember(51)]
        public ObservableCollection<LocationModel> ListLocationModel
        {
            get => _listlocationModel;
            set => SetProperty(ref _listlocationModel, value);
        }

        private bool _isTestMode;

        [ProtoIgnore]
        public bool IsTestMode
        {
            get => _isTestMode;
            set => SetProperty(ref _isTestMode, value);
        }

        private Visibility _debugVisibility;

        [ProtoIgnore]
        public Visibility DebugVisibility
        {
            get => _debugVisibility;
            set => SetProperty(ref _debugVisibility, value);
        }

        private bool _isSelectCountriesFilter;

        [ProtoMember(58)]
        public bool IsSelectCountriesFilter
        {
            get => _isSelectCountriesFilter;
            set => SetProperty(ref _isSelectCountriesFilter, value);
        }


        private ObservableCollection<LocationModel> _listlocationModelTemp = new ObservableCollection<LocationModel>();

        public ObservableCollection<LocationModel> ListLocationModelTemp
        {
            get => _listlocationModelTemp;
            set => SetProperty(ref _listlocationModelTemp, value);
        }


        private bool _doNotSortByUserNameChecked;

        [ProtoMember(53)]
        public bool DoNotSortByUserNameChecked
        {
            get => _doNotSortByUserNameChecked;
            set => SetProperty(ref _doNotSortByUserNameChecked, value);
        }

        private bool _sortByUsername;

        [ProtoMember(54)]
        public bool SortByUsername
        {
            get => _sortByUsername;
            set => SetProperty(ref _sortByUsername, value);
        }

        private bool _sortByNikename;

        [ProtoMember(55)]
        public bool SortByNikename
        {
            get => _sortByNikename;
            set => SetProperty(ref _sortByNikename, value);
        }

        private bool _skipAlreadyProcessedQueryValue;

        [ProtoMember(56)]
        public bool SkipAlreadyProcessedQueryValue
        {
            get => _skipAlreadyProcessedQueryValue;
            set => SetProperty(ref _skipAlreadyProcessedQueryValue, value);
        }

        private bool _debugRespData;

        [ProtoMember(57)]
        public bool DebugRespData
        {
            get => _debugRespData;
            set => SetProperty(ref _debugRespData, value);
        }
    }
}