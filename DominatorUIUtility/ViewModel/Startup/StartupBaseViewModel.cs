using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces.StartUp;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.StartupActivity;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using ProtoBuf;
using BindableBase = Prism.Mvvm.BindableBase;

namespace DominatorUIUtility.ViewModel.Startup
{
    public class StartupBaseViewModel : BindableBase, IStartUpSearchQuery, IStartupJobConfiguration
    {
        [field: NonSerialized] public static Func<string, BaseActivity> GetFaceBookActivity;

        public static int selectedIndex;

        [field: NonSerialized] public IRegionManager regionManager;

        public StartupBaseViewModel(IRegionManager region)
        {
            IsNonQuery = false;
            regionManager = region;
            AddQueryCommand = new DelegateCommand<dynamic>(AddQuery);
            regionManager.RequestNavigate("StartupRegion", "SelectActivity");
        }

        public static List<string> NavigationList { get; set; }
        public static Dictionary<Type, List<QueryInfo>> LstGlobalQuery { get; set; }
        public static List<ActivityConfig> ViewModelToSave { get; set; } = new List<ActivityConfig>();

        #region Commands

        public ICommand NextCommand { get; set; }
        public ICommand PreviousCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
        public ICommand AddQueryCommand { get; set; }

        #endregion

        #region Properties

        private bool _isExpanded;

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        private JobConfiguration _jobConfiguration;

        [ProtoMember(1)]
        public JobConfiguration JobConfiguration
        {
            get => _jobConfiguration;
            set
            {
                if (value == _jobConfiguration)
                    return;
                SetProperty(ref _jobConfiguration, value);
            }
        }

        private ObservableCollection<QueryInfo> _savedQueries = new ObservableCollection<QueryInfo>();

        [ProtoMember(2)]
        public ObservableCollection<QueryInfo> SavedQueries
        {
            get => _savedQueries;
            set => SetProperty(ref _savedQueries, value);
        }

        private List<string> _listQueryType = new List<string>();

        public List<string> ListQueryType
        {
            get => _listQueryType;
            set => SetProperty(ref _listQueryType, value);
        }

        private string _nextButtonContent = "LangKeyNext".FromResourceDictionary();

        [ProtoIgnore]
        public string NextButtonContent
        {
            get => _nextButtonContent;
            set => SetProperty(ref _nextButtonContent, value);
        }

        private bool _isNonQuery;

        public bool IsNonQuery
        {
            get => _isNonQuery;
            set => SetProperty(ref _isNonQuery, value);
        }


        private bool _isUseGlobalQuery;

        public bool IsUseGlobalQuery
        {
            get => _isUseGlobalQuery;
            set
            {
                SetProperty(ref _isUseGlobalQuery, value);
                if (_isUseGlobalQuery && LstGlobalQuery.ContainsKey(CurrentType))
                {
                    SavedQueries.Clear();
                    var getDict = LstGlobalQuery[CurrentType];

                    getDict.ForEach(query =>
                    {
                        var currentQuery = new QueryInfo
                        {
                            QueryType = query.QueryType,
                            QueryTypeDisplayName = query.QueryType,
                            QueryValue = query.QueryValue
                        };
                        SavedQueries.Add(currentQuery);
                    });
                }
            }
        }

        public Type CurrentType { get; set; }

        #endregion

        #region Methods

        private void AddQuery(dynamic actvity)
        {
            try
            {
                var IsQueryOfCurrentModule = false;
                var viewModel = InstanceProvider.GetInstance<ISelectActivityViewModel>();
                var network = actvity.QueryControl.Network = viewModel.SelectedNetwork;
                var _activityType = actvity.QueryControl.ActivityType =
                    (ActivityType) Enum.Parse(typeof(ActivityType), NavigationList[selectedIndex]);

                if (string.IsNullOrEmpty(actvity.QueryControl.CurrentQuery.QueryValue) &&
                    actvity.QueryControl.QueryCollection.Count != 0)
                {
                    foreach (var query in actvity.QueryControl.QueryCollection)
                    {
                        var currentQuery = actvity.QueryControl.CurrentQuery.Clone() as QueryInfo;

                        if (currentQuery == null) return;
                        var lstquery = new List<string>(query.Split('\t'));
                        if (lstquery.Count > 1)
                        {
                            if (network.ToString() == lstquery[0] && lstquery[1] == _activityType.ToString())
                            {
                                currentQuery.QueryType = lstquery[2];
                                currentQuery.QueryValue = lstquery[3];
                                currentQuery.QueryTypeDisplayName = currentQuery.QueryType;
                                currentQuery.QueryPriority = SavedQueries.Count + 1;
                                IsQueryOfCurrentModule = true;
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            currentQuery.QueryValue = query;
                            currentQuery.QueryTypeDisplayName = currentQuery.QueryType;
                            currentQuery.QueryPriority = SavedQueries.Count + 1;
                            IsQueryOfCurrentModule = true;
                        }

                        if (SavedQueries.Any(x =>
                            x.QueryType == currentQuery.QueryType && x.QueryValue == currentQuery.QueryValue))
                        {
                            Dialog.ShowDialog("LangKeyWarning".FromResourceDictionary(),
                                "LangKeyQueryAlreadyExist".FromResourceDictionary());
                            return;
                        }

                        SavedQueries.Add(currentQuery);
                    }

                    if (!IsQueryOfCurrentModule)
                        GlobusLogHelper.log.Info(Log.CustomMessage, network, viewModel.SelectAccount.UserName,
                            _activityType,
                            string.Format("LangKeyQueryCantBeAdded".FromResourceDictionary(), network, _activityType));
                }
                else
                {
                    if (string.IsNullOrEmpty(actvity.QueryControl.CurrentQuery.QueryValue))
                    {
                        Dialog.ShowDialog("LangKeyWarning".FromResourceDictionary(),
                            "LangKeyTypeSomeQuery".FromResourceDictionary());
                        return;
                    }

                    actvity.QueryControl.CurrentQuery.QueryTypeDisplayName =
                        actvity.QueryControl.CurrentQuery.QueryType;
                    var currentQuery = actvity.QueryControl.CurrentQuery.Clone() as QueryInfo;

                    if (currentQuery == null) return;

                    currentQuery.QueryPriority = SavedQueries.Count + 1;
                    if (SavedQueries.Any(x =>
                        x.QueryType == currentQuery.QueryType && x.QueryValue == currentQuery.QueryValue))
                    {
                        Dialog.ShowDialog("LangKeyWarning".FromResourceDictionary(),
                            "LangKeyQueryAlreadyExist".FromResourceDictionary());
                        return;
                    }

                    currentQuery.Index = SavedQueries.Count + 1;
                    SavedQueries.Add(currentQuery);
                    actvity.QueryControl.CurrentQuery = new QueryInfo();
                }

                actvity.QueryControl.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        protected void NavigateNext()
        {
            if (CurrentType != null)
                if (!LstGlobalQuery.ContainsKey(CurrentType))
                    LstGlobalQuery.Add(CurrentType, SavedQueries.ToList());

            if (selectedIndex > 0 && !ValidateRunningTime())
                return;
            if (!IsNonQuery && selectedIndex > 0 && !ValidateQuery())
                return;
            var IsNeedToStart = false;
            if (NextButtonContent == "LangKeyFinish".FromResourceDictionary())
            {
                var saveSetting = InstanceProvider.GetInstance<ISaveSetting>();
                saveSetting.Save();
                ModuleSetting.Instance.Close();
                selectedIndex = 0;
                IsNeedToStart = true;
                regionManager.Regions["StartupRegion"].RemoveAll();
                regionManager.RequestNavigate("StartupRegion", "SelectActivity");
            }

            if (selectedIndex >= NavigationList.Count - 1 || IsNeedToStart)
                return;
            selectedIndex++;
            var next = NavigationList[selectedIndex];
            regionManager.RequestNavigate("StartupRegion", next);
        }

        public bool ValidateRunningTime()
        {
            if (JobConfiguration.RunningTime.All(time => time.Timings.Count == 0))
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyAddATimeRangeToRunStopActivity".FromResourceDictionary());
                return false;
            }

            return true;
        }

        public bool ValidateQuery()
        {
            if (SavedQueries.Count == 0)
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected void NavigatePrevious()
        {
            if (selectedIndex <= 0)
                return;
            selectedIndex--;
            var previous = NavigationList[selectedIndex];
            regionManager.RequestNavigate("StartupRegion", previous);
        }

        public void OnLoad(string activityType)
        {
            ListQueryType.Clear();
            var viewModel = InstanceProvider.GetInstance<ISelectActivityViewModel>();
            if (viewModel.SelectedNetwork == "Facebook")
            {
                var activity = GetFaceBookActivity(activityType);
                ListQueryType = activity.GetQueryType();
                CurrentType = activity.GetEnumType();
            }
            else
            {
                var activity = SocialNetworkActivity.GetNetworkActivity(viewModel.SelectedNetwork)
                    .GetActivity(activityType);
                ListQueryType = activity.GetQueryType();
                CurrentType = activity.GetEnumType();
            }

            if (selectedIndex == NavigationList.Count - 1)
                NextButtonContent = "LangKeyFinish".FromResourceDictionary();
            else
                NextButtonContent = "LangKeyNext".FromResourceDictionary();
        }

        #endregion
    }

    [ProtoContract]
    public class ActivityConfig
    {
        [ProtoMember(1)] public object Model { get; set; }

        [ProtoMember(2)] public ActivityType ActivityType { get; set; }
    }
}