using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Command;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Base class which handles events from IHeaderControl, IHelpContol, IFooterControl
    ///     Implements handlers for: _OnSelectAccountChanged, _OnCreateCampaignChanged, _OnCustomFilterChanged,
    ///     _OnInfoChanged, _OnAddQuery, SetDataContex
    ///     Uses as base for: Follower, Unfollower, Like, Comment, DownloadPhotos, etc.
    /// </summary>
    public abstract class ModuleSettingsUserControl<TViewModel, TModel> : UserControl, INotifyPropertyChanged,
        IHeaderControl, IHelpControl, IFooterControl, IAccountGrowthModeHeader
        where TModel : class, new()
        where TViewModel : class, new()
    {
        private readonly IAccountsCacheService _accountsCacheService;
        private readonly IAccountsFileManager _accountsFileManager;
        private readonly IDataBaseHandler _dataBaseHandler;
        private readonly IDominatorScheduler _dominatorScheduler;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly DbOperations globalDbOperation;

        public bool ShowWarningDialog(object context, string Title, string Message,
            string OkButtonText = "OK", string CancelButtonText = "")
        {
            return Dialog.ShowCustomDialog(Title,Message,OkButtonText,CancelButtonText)
                == MessageDialogResult.Affirmative;
        }
        #region Constructor

        protected ModuleSettingsUserControl()
        {
            _jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            _accountsCacheService = InstanceProvider.GetInstance<IAccountsCacheService>();
            _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            _dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
            _dataBaseHandler = InstanceProvider.GetInstance<IDataBaseHandler>();
            CreateCampaignCommand = new BaseCommand<object>(sender => true, CreateOrUpdateCampaign);
            SelectAccountCommand = new BaseCommand<object>(sender => true, sender => SelectAccount());
            CancelEditCommand = new BaseCommand<object>(sender => true, sender =>
            {
                SetDataContext();
                TabSwitcher.GoToCampaign();
            });
            InfoCommand = new BaseCommand<object>(sender => true, sender => { IsOpen = true; });
            SaveConfigurationsCommand = new BaseCommand<object>(sender => true, sender => SaveConfigurations());
            LoadedCommand = new BaseCommand<object>(sender => true, sender => SetSelectedAccounts());
            SelectionChangedCommand = new BaseCommand<object>(sender => true, sender => SetAccountModeDataContext());
            StatusChangedCommand = new BaseCommand<object>(sender => true, sender => AccountModeStatusChange());
            globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
            HeaderHelper.UpdateToggleButtonInCampaignMode += UpdateCampaignModeToggleButton;
            HeaderHelper.UpdateToggleButtonInAccountActivityMode += UpdateAccountActivityModeToggleButton;
        }

        #endregion

        private void UpdateCampaignModeToggleButton()
        {
            if (_headerControl != null)
            {
                var isAllCollapsed = HeaderHelper.IsAllExpanderCollapseOrNot(this);

                if (isAllCollapsed)
                    _headerControl.IsExpanded = false;
                else _headerControl.IsExpanded = true;
            }
        }

        private void UpdateAccountActivityModeToggleButton()
        {
            if (_accountGrowthModeHeader != null)
            {
                var isAllCollapse = HeaderHelper.IsAllExpanderCollapseOrNot(this);

                if (isAllCollapse)
                    _accountGrowthModeHeader.IsExpanded = false;
                else _accountGrowthModeHeader.IsExpanded = true;
            }
        }

        private void CreateOrUpdateCampaign(object sender)
        {
            var control = sender as FooterControl;

            if (control != null && control.CampaignManager.Equals(ConstantVariable.CreateCampaign(),
                    StringComparison.CurrentCultureIgnoreCase))
                CreateCampaign();
            else
                UpdateCampaign();
        }

        #region Module base Initializer

        public void InitializeBaseClass(
            Grid MainGrid,
            ActivityType activityType,
            string moduleName,
            HeaderControl header = null,
            FooterControl footer = null,
            SearchQueryControl queryControl = null,
            AccountGrowthModeHeader accountGrowthModeHeader = null
        )
        {
            if (_initialized) return;

            _headerControl = header;
            _footerControl = footer;
            _queryControl = queryControl;
            _mainGrid = MainGrid;
            _activityType = activityType;
            _moduleName = moduleName;
            _accountGrowthModeHeader = accountGrowthModeHeader;
            _initialized = true;
        }

        #endregion


        #region Set Data context

        protected virtual void SetDataContext()
        {
            IsEditCampaignName = true;

            CancelEditVisibility = Visibility.Collapsed;

            _footerControl.CampaignManager = ConstantVariable.CreateCampaign();

            SelectedAccountCount = ConstantVariable.NoAccountSelected();

            ObjViewModel = new TViewModel();

            _footerControl.list_SelectedAccounts = new List<string>();

            _mainGrid.DataContext = ObjViewModel;

            _headerControl.DataContext = _footerControl.DataContext = this;

            CampaignName =
                $"{SocialNetwork} {_activityType} [{DateTime.Now.GetCurrentDate()}]";
            if (_queryControl != null)
            {
                _queryControl.ActivityType = _activityType;
                _queryControl.Network = SocialNetwork.ToString();
            }

            UpdateJobConfigurationSetting();
        }

        #endregion

        #region Custom filter 

        /// <summary>
        ///     On Custom filter  changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SearchQueryControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)
        {
            UserFilterAction.UserFilterControl(_queryControl);
            //UserFiltersControl objUserFiltersControl = new UserFiltersControl();
            //Dialog objDialog = new Dialog();
            //var FilterWindow = objDialog.GetMetroWindow(objUserFiltersControl, "Filter");
            //objUserFiltersControl.SaveButton.Click += (senders, Events) =>
            //{
            //    _queryControl.CurrentQuery.CustomFilters = JsonConvert.SerializeObject(objUserFiltersControl.UserFilter);
            //    FilterWindow.Close();
            //};
            //FilterWindow.ShowDialog();
        }

        #endregion

        public virtual void SelectAccount()
        {
            try
            {
                var objSelectAccountControl = new SelectAccountControl(_footerControl.list_SelectedAccounts);

                var objDialog = new Dialog();

                var window = objDialog.GetMetroWindow(objSelectAccountControl,
                    "LangKeySelectAccount".FromResourceDictionary());

                objSelectAccountControl.btnSave.Click += (senders, Events) =>
                {
                    var selectedAccount = objSelectAccountControl.GetSelectedAccount();
                    if (selectedAccount.Count > 0)
                    {
                        _footerControl.list_SelectedAccounts = objSelectAccountControl.GetSelectedAccount().ToList();
                        SelectedAccountCount = _footerControl.list_SelectedAccounts.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                        GlobusLogHelper.log.Info(Log.SelectedAccount, SocinatorInitialize.ActiveSocialNetwork,
                            CampaignName, _footerControl.list_SelectedAccounts.Count, CampaignName);
                    }
                    else
                    {
                        SelectedAccountCount = ConstantVariable.NoAccountSelected();
                        _footerControl.list_SelectedAccounts = selectedAccount.ToList();
                    }

                    window.Close();
                };

                objSelectAccountControl.btnCancel.Click += (senders, events) => window.Close();
                window.ShowDialog();
            }
            catch (Exception Ex)
            {
                Ex.DebugLog();
            }
        }

        public void AddQuery(Type queryParameterType, List<string> listQueryType = null)
        {
            try
            {
                var IsQueryOfCurrentModule = false;
                if (string.IsNullOrEmpty(_queryControl.CurrentQuery.QueryValue.Trim()) &&
                    _queryControl.QueryCollection.Count == 0)
                    return;

                #region Getting listQueryTypeEnumNames for TuntoSocinator (won't affect the code for socincator) (Getting it for RunQueryScraper and for fixing languageIssue on queriesValue in tuntoSocinator)

                //var enumsList = queryParameterType.GetEnumNames();
                var enumsList = listQueryType == null
                    ? queryParameterType.GetEnumNames()
                    : queryParameterType.GetEnumNames().Where(x =>
                            listQueryType.Contains(x.GetDescriptionAttribute(queryParameterType)
                                .FromResourceDictionary()))
                        .ToArray();

                #endregion

                if (_queryControl.CurrentQuery.QueryValue.Contains(","))
                {
                    _queryControl.QueryCollection.Clear();
                    _queryControl.QueryCollection.AddRange(_queryControl.CurrentQuery.QueryValue.Split(',')
                        .Where(x => !string.IsNullOrEmpty(x.Trim())).Select(x => x.Trim()).Distinct());
                    _queryControl.CurrentQuery.QueryValue = string.Empty;
                }

                var queryValuIndex = new List<int>();
                if (string.IsNullOrEmpty(_queryControl.CurrentQuery.QueryValue) &&
                    _queryControl.QueryCollection.Count != 0)
                {
                    _queryControl.QueryCollection.ForEach(query =>
                    {
                        var currentQuery = _queryControl.CurrentQuery.Clone() as QueryInfo;

                        if (currentQuery == null) return;
                        var lstquery = query.Split('\t').ToList();
                        if (lstquery.Count > 1)
                        {
                            if (SocialNetwork.ToString() == lstquery[0] && lstquery[1] == _activityType.ToString())
                            {
                                currentQuery.QueryType = lstquery[2];
                                currentQuery.QueryValue = lstquery[3];
                                currentQuery.QueryTypeDisplayName = currentQuery.QueryType;
                                currentQuery.QueryPriority = Model.SavedQueries.Count + 1;
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
                            currentQuery.QueryPriority = Model.SavedQueries.Count + 1;
                            IsQueryOfCurrentModule = true;
                        }

                        if (IsQueryExistWithoutDialog(currentQuery, Model.SavedQueries))
                        {
                            queryValuIndex.Add(_queryControl.QueryCollection.IndexOf(query));
                            return;
                        }

                        SetQueryTypeEnumName(enumsList, currentQuery);
                        Model.SavedQueries.Add(currentQuery);
                        currentQuery.Index = Model.SavedQueries.IndexOf(currentQuery) + 1;
                    });
                    if (!IsQueryOfCurrentModule)
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetwork.ToString(), CampaignName,
                            _activityType,
                            $"Query can't add because it may not related to {SocialNetwork}  {_activityType} module.");

                    if (queryValuIndex.Count > 0)
                    {
                        if (queryValuIndex.Count <= 10)
                            GlobusLogHelper.log.Info(Log.AlreadyExistQuery, SocinatorInitialize.ActiveSocialNetwork,
                                CampaignName, _activityType,
                                "{ " + string.Join(" },{ ", queryValuIndex.ToArray()) + " }");
                        else
                            GlobusLogHelper.log.Info(Log.AlreadyExistQueryCount,
                                SocinatorInitialize.ActiveSocialNetwork, CampaignName, _activityType,
                                queryValuIndex.Count);
                    }
                }
                else
                {
                    _queryControl.CurrentQuery.QueryTypeDisplayName = _queryControl.CurrentQuery.QueryType;

                    // _queryControl.CurrentQuery.QueryTypeDisplayName = _queryControl.CurrentQuery.QueryTypeAsDisplayName(queryParameterType);

                    var currentQuery = _queryControl.CurrentQuery.Clone() as QueryInfo;

                    if (currentQuery == null) return;

                    currentQuery.QueryValue = currentQuery.QueryValue.Trim();

                    currentQuery.QueryPriority = Model.SavedQueries.Count + 1;

                    if (IsQueryExist(currentQuery, Model.SavedQueries)) return;

                    SetQueryTypeEnumName(enumsList, currentQuery);
                    Model.SavedQueries.Add(currentQuery);
                    currentQuery.Index = Model.SavedQueries.IndexOf(currentQuery) + 1;
                    _queryControl.CurrentQuery.QueryValue = string.Empty;
                }

                _queryControl.IsEnabled = true;

                ClearQueryCollection();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void ClearQueryCollection()
        {
            if (_queryControl.QueryCollection != null && _queryControl.QueryCollection.Any())
                _queryControl.QueryCollection.Clear();
        }

        public void SetQueryTypeEnumName(string[] enumsList, QueryInfo currentQuery)
        {
            try
            {
                var queryNameIndex = Model.ListQueryType.IndexOf(currentQuery.QueryType);
                currentQuery.QueryTypeEnum = enumsList[queryNameIndex];
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void CustomFilter()
        {
            try
            {
                UserFilterAction.UserFilterControl(_queryControl);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SetSelectedAccounts()
        {
            try
            {
                var networks = SocinatorInitialize.AccountModeActiveSocialNetwork;
                var accounts = new ObservableCollectionBase<string>(_accountsFileManager.GetAll()
                    .Where(x => x.AccountBaseModel.AccountNetwork == networks &&
                                (x.AccountBaseModel.Status == AccountStatus.Success ||
                                 x.AccountBaseModel.Status == AccountStatus.UpdatingDetails)).Select(x => x.UserName));

                _accountGrowthModeHeader.AccountItemSource = accounts;

                _accountGrowthModeHeader.SelectedItem = SocinatorInitialize.GetSocialLibrary(networks)
                    .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;
                SetAccountModeDataContext();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SetAccountModeDataContext()
        {
            try
            {
                var network = SocinatorInitialize.AccountModeActiveSocialNetwork;
                SocialNetwork = network;

                var accountDetails = _accountsFileManager.GetAccount(_accountGrowthModeHeader.SelectedItem, network);

                SocinatorInitialize.GetSocialLibrary(network)
                        .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount =
                    _accountGrowthModeHeader.SelectedItem;

                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration = jobActivityConfigurationManager[accountDetails.AccountId, _activityType];

                if (moduleConfiguration != null)
                {
                    var TemplatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                    var templateDetails = TemplatesFileManager.GetTemplateById(moduleConfiguration.TemplateId);

                    if (moduleConfiguration.Status == null ||
                        moduleConfiguration.Status == "Active" && !moduleConfiguration.IsEnabled ||
                        moduleConfiguration.Status == "Paused" && moduleConfiguration.IsEnabled)
                        jobActivityConfigurationManager[accountDetails.AccountId, _activityType].Status =
                            moduleConfiguration.IsEnabled ? "Active" : "Paused";

                    SetModuleValues(moduleConfiguration.IsEnabled, templateDetails);
                }
                else
                {
                    SetModuleValues(false, null);
                }

                _mainGrid.DataContext = ObjViewModel;
                _accountGrowthModeHeader.DataContext = this;
                if (_queryControl != null)
                {
                    _queryControl.ActivityType = _activityType;
                    _queryControl.Network = SocialNetwork.ToString();
                }

                UpdateJobConfigurationSetting();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void UpdateJobConfigurationSetting()
        {
            try
            {
                if (Model.JobConfiguration.IsAdvanceSetting)
                    return;

                if (Model.JobConfiguration.SelectedItem == "Slow")
                {
                    var slowSpeed = Model.SlowSpeed;
                    Model.JobConfiguration.ActivitiesPerDay = slowSpeed.ActivitiesPerDay;
                    Model.JobConfiguration.ActivitiesPerHour = slowSpeed.ActivitiesPerHour;
                    Model.JobConfiguration.ActivitiesPerWeek = slowSpeed.ActivitiesPerWeek;
                    Model.JobConfiguration.ActivitiesPerJob = slowSpeed.ActivitiesPerJob;
                    Model.JobConfiguration.DelayBetweenJobs = slowSpeed.DelayBetweenJobs;
                    Model.JobConfiguration.DelayBetweenActivity = slowSpeed.DelayBetweenActivity;
                }
                else if (Model.JobConfiguration.SelectedItem == "Medium")
                {
                    var mediumSpeed = Model.MediumSpeed;
                    Model.JobConfiguration.ActivitiesPerDay = mediumSpeed.ActivitiesPerDay;
                    Model.JobConfiguration.ActivitiesPerHour = mediumSpeed.ActivitiesPerHour;
                    Model.JobConfiguration.ActivitiesPerWeek = mediumSpeed.ActivitiesPerWeek;
                    Model.JobConfiguration.ActivitiesPerJob = mediumSpeed.ActivitiesPerJob;
                    Model.JobConfiguration.DelayBetweenJobs = mediumSpeed.DelayBetweenJobs;
                    Model.JobConfiguration.DelayBetweenActivity = mediumSpeed.DelayBetweenActivity;
                }
                else if (Model.JobConfiguration.SelectedItem == "Fast")
                {
                    var fastSpeed = Model.FastSpeed;
                    Model.JobConfiguration.ActivitiesPerDay = fastSpeed.ActivitiesPerDay;
                    Model.JobConfiguration.ActivitiesPerHour = fastSpeed.ActivitiesPerHour;
                    Model.JobConfiguration.ActivitiesPerWeek = fastSpeed.ActivitiesPerWeek;
                    Model.JobConfiguration.ActivitiesPerJob = fastSpeed.ActivitiesPerJob;
                    Model.JobConfiguration.DelayBetweenJobs = fastSpeed.DelayBetweenJobs;
                    Model.JobConfiguration.DelayBetweenActivity = fastSpeed.DelayBetweenActivity;
                }
                else if (Model.JobConfiguration.SelectedItem == "Superfast")
                {
                    var superfastSpeed = Model.SuperfastSpeed;
                    Model.JobConfiguration.ActivitiesPerDay = superfastSpeed.ActivitiesPerDay;
                    Model.JobConfiguration.ActivitiesPerHour = superfastSpeed.ActivitiesPerHour;
                    Model.JobConfiguration.ActivitiesPerWeek = superfastSpeed.ActivitiesPerWeek;
                    Model.JobConfiguration.ActivitiesPerJob = superfastSpeed.ActivitiesPerJob;
                    Model.JobConfiguration.DelayBetweenJobs = superfastSpeed.DelayBetweenJobs;
                    Model.JobConfiguration.DelayBetweenActivity = superfastSpeed.DelayBetweenActivity;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AccountModeStatusChange()
        {
            try
            {
                var network = SocinatorInitialize.AccountModeActiveSocialNetwork;
                if (!ChangeAccountsModuleStatus(Model.IsAccountGrowthActive, _accountGrowthModeHeader.SelectedItem,
                    network))
                    Model.IsAccountGrowthActive = false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region Commands

        public ICommand CreateCampaignCommand { get; set; }
        public ICommand SelectAccountCommand { get; set; }
        public ICommand CancelEditCommand { get; set; }
        public ICommand InfoCommand { get; set; }
        public ICommand SaveConfigurationsCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }
        public ICommand StatusChangedCommand { get; set; }

        #endregion

        #region Properties

        private HeaderControl _headerControl;
        public FooterControl _footerControl;
        public SearchQueryControl _queryControl;
        private Grid _mainGrid;
        public AccountGrowthModeHeader _accountGrowthModeHeader;
        private ActivityType _activityType;
        private string _moduleName;
        protected SocialNetworks SocialNetwork = SocinatorInitialize.ActiveSocialNetwork;
        private bool _initialized;
        private bool _isCancelledUpdate = false;

        #endregion

        #region IHeaderControl

        private string _campaignName;

        public string CampaignName
        {
            get => _campaignName;
            set
            {
                _campaignName = value;
                OnPropertyChanged(nameof(CampaignName));
            }
        }

        private bool _isEditCampaignName = true;

        public bool IsEditCampaignName
        {
            get => _isEditCampaignName;
            set
            {
                _isEditCampaignName = value;
                OnPropertyChanged(nameof(IsEditCampaignName));
            }
        }


        private Visibility _cancelEditVisibility = Visibility.Collapsed;

        public Visibility CancelEditVisibility
        {
            get => _cancelEditVisibility;
            set
            {
                _cancelEditVisibility = value;
                OnPropertyChanged(nameof(CancelEditVisibility));
            }
        }

        private string _templateId;

        public string TemplateId
        {
            get => _templateId;

            set
            {
                _templateId = value;
                OnPropertyChanged(nameof(_templateId));
            }
        }

        #endregion

        #region IHelpControl

        public string VideoTutorialLink { get; set; } = "!Pass ConstantHelpDetails.VideoTutorialsLink in Derived Class";

        public string KnowledgeBaseLink { get; set; } = "!Pass ConstantHelpDetails.KnowledgeBaseLink";

        public string ContactSupportLink { get; set; } = "!Pass ConstantHelpDetails.ContactLink";
        private bool _isOpen;

        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                _isOpen = value;
                OnPropertyChanged(nameof(IsOpen));
            }
        }

        #endregion

        #region IFooterControl

        private string _selectedAccountCount = ConstantVariable.NoAccountSelected();

        public string SelectedAccountCount
        {
            get => _selectedAccountCount;
            set
            {
                _selectedAccountCount = value;
                OnPropertyChanged(nameof(SelectedAccountCount));
            }
        }

        private string _campaignButtonContent = ConstantVariable.CreateCampaign();

        public string CampaignButtonContent
        {
            get => _campaignButtonContent;
            set
            {
                _campaignButtonContent = value;
                OnPropertyChanged(nameof(CampaignButtonContent));
            }
        }

        #endregion

        #region IAccountGrowthModeHeader

        public ObservableCollectionBase<string> AccountItemSource
        {
            get => _accountItemSource;
            set
            {
                _accountItemSource = value;
                OnPropertyChanged(nameof(_accountItemSource));
            }
        }

        public string SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(_selectedItem));
            }
        }

        #endregion

        #region Properties

        private TViewModel _ObjViewModel = new TViewModel();
        private ObservableCollectionBase<string> _accountItemSource;
        private string _selectedItem;


        public TViewModel ObjViewModel
        {
            get => _ObjViewModel;
            set
            {
                _ObjViewModel = value;
                OnPropertyChanged(nameof(ObjViewModel));
            }
        }

        // NOTE: ViewModel must contain Model field
        public dynamic Model => (ObjViewModel as dynamic).Model as TModel;

        #endregion

        #region PropertyChanged section

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Base Methods for module settings 

        public virtual void SaveDetails(List<string> lstSelectedAccounts, ActivityType moduleType)
        {
        }

        protected virtual void SetModuleValues(bool isToggleButtonActive, TemplateModel templateModel)
        {
        }

        private bool ValidateCampaignName()
        {
            if (string.IsNullOrEmpty(CampaignName))
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorCampaignNameShouldNotBeEmpty".FromResourceDictionary());
                return true;
            }
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            if (_footerControl.CampaignManager == ConstantVariable.CreateCampaign() &&
                campaignFileManager.Any(x => x.CampaignName == CampaignName))
            {
                var activityType = _activityType.ToString();
                var lastCampaignName = campaignFileManager.LastOrDefault(t => t.SocialNetworks == SocinatorInitialize.ActiveSocialNetwork
                && t.SubModule == activityType);
                var getCounter = GetCampaignNameCounter(lastCampaignName?.CampaignName);
                CampaignName = $"{CampaignName}__{getCounter}";
                return false;
            }

            return false;
        }
        private int GetCampaignNameCounter(string lastCampaignName)
        {
            try
            {
                var parts = string.IsNullOrEmpty(lastCampaignName) ? CampaignName.Split(new[] { "__" }, StringSplitOptions.None)?.ToList() :
                    lastCampaignName.Split(new[] { "__" }, StringSplitOptions.None)?.ToList();
                int.TryParse(parts.Count >= 2 ? parts[1] : string.Empty, out int counter);
                return counter += 1;
            }
            catch { }
            return RandomUtilties.GetRandomNumber(10000);
        }

        protected virtual bool ValidateCampaign()
        {
            if (_footerControl.list_SelectedAccounts.Count == 0)
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorSelectAtleastOneAccount".FromResourceDictionary());
                return false;
            }

            //Check Query
            //if (!ValidateQuery())
            //    return false;
            // Check timings
            return ValidateRunningTime();
        }

        private bool ValidateRunningTime()
        {
            if (((IEnumerable<RunningTimes>)Model.JobConfiguration.RunningTime).All(rt => rt.Timings.Count == 0))
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneTimeRange".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected virtual bool ValidateExtraProperty()
        {
            return true;
        }

        protected bool ValidateSavedQueries()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected virtual bool ValidateQuery()
        {
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return true;
        }

        #endregion

        #region Create Campaign 

        protected void CreateCampaign()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (ValidateCampaignName())
                    return;

                if (!ValidateCampaign())
                    return;

                if (!ValidateExtraProperty())
                    return;

                //var schedulePending = ImmutableQueue<Action>.Empty;
                if (IsNeedToSaveTemplate())
                {
                    var originalTemplateId = TemplateId.DeepCloneObject();
                    TemplateId = TemplateModel.SaveTemplate((TModel)Model, _activityType.ToString(), SocialNetwork,
                        CampaignName);
                    SaveTemplateToAccounts(TemplateId);
                    SaveTemplateToCampaigns(originalTemplateId);
                    var accountDetails =
                        _accountsFileManager.GetAllAccounts(_footerControl.list_SelectedAccounts, SocialNetwork);
                    try
                    {
                        Task.Factory.StartNew(() =>
                        {
                            foreach (var account in accountDetails)
                            {
                                _dominatorScheduler.ScheduleNextActivity(account, _activityType);
                            }

                        });
                    }
                    catch (AggregateException)
                    {
                    }
                    catch (Exception)
                    {
                    }

                    #region Old code

                    //try
                    //{
                    //new Thread(() =>
                    //{
                    //    foreach (var account in accountDetails)
                    //    {
                    //        Action scheduleAccount = () =>
                    //        {
                    //            _dominatorScheduler.ScheduleNextActivity(account, _activityType);
                    //        };
                    //        schedulePending = schedulePending.Enqueue(scheduleAccount);
                    //    }

                    //    while (!schedulePending.IsEmpty)
                    //    {
                    //        Action startSchedule;
                    //        schedulePending = schedulePending.Dequeue(out startSchedule);
                    //        startSchedule();
                    //    }

                    //})
                    //{ IsBackground = true }.Start();
                    //}
                    //catch (Exception ex)
                    //{
                    //    ex.DebugLog();
                    //} 

                    #endregion

                    var islogged = false;
                    SetDataContext();

                    TabSwitcher.GoToCampaign();
                    var softwareSettings = InstanceProvider.GetInstance<ISoftwareSettings>();
                    if (softwareSettings.Settings.IsThreadLimitChecked)
                        if (DominatorScheduler._lockWithThreadLimit?.CurrentCount == 0 && !islogged)
                        {
                            var maxThreadCount = softwareSettings.Settings.MaxThreadCount;
                            GlobusLogHelper.log.Info(
                                $"{"LangKeyThreadLimitReachedTo".FromResourceDictionary()} {maxThreadCount} {"LangKeyPendingStartsWhenRunnningStops".FromResourceDictionary()}");
                            islogged = true;
                        }
                }
            });
        }

        public void SaveTemplateToCampaigns(string originalTemplateId)
        {
            var campaignDetails = new CampaignDetails
            {
                CampaignName = CampaignName,
                MainModule = _moduleName,
                SubModule = _activityType.ToString(),
                SocialNetworks = SocialNetwork,
                SelectedAccountList = _footerControl.list_SelectedAccounts,
                TemplateId = TemplateId,
                CreationDate = DateTimeUtilities.GetEpochTime(),
                Status = "Active",
                LastEditedDate = DateTimeUtilities.GetEpochTime()
            };


            var dbOperations =
                new DbOperations(campaignDetails.CampaignId, SocialNetwork, ConstantVariable.GetCampaignDb);

            _dataBaseHandler.DbCampaignInitialCounters[SocialNetwork](dbOperations);

            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();

            if (_cancelEditVisibility == Visibility.Visible)
            {
                var mainCampaignDetails = Campaigns.GetCampaignsInstance(SocialNetwork).CampaignViewModel
                    .LstCampaignDetails.FirstOrDefault(x => x.TemplateId == originalTemplateId);

                if (mainCampaignDetails.SelectedAccountList.Count == 0)
                    mainCampaignDetails.Status = "Pause";
            }

            if (campaignFileManager.All(x => x.CampaignId != campaignDetails.CampaignId))
            {
                campaignFileManager.Add(campaignDetails);

                //Updating Campaign UI
                Campaigns.GetCampaignsInstance(SocialNetwork).CampaignViewModel.LstCampaignDetails.Add(campaignDetails);
            }
        }

        public bool IsNeedToSaveTemplate()
        {
            var needToCancel = false;

            #region Get the accounts which holds template Id

            var accountDetails =
                _accountsFileManager.GetAllAccounts(_footerControl.list_SelectedAccounts, SocialNetwork);

            var accountHavingTemplates = accountDetails
                .Where(x => _jobActivityConfigurationManager[x.AccountId, _activityType]?.TemplateId != null).ToList();

            if (accountHavingTemplates.Count == 0)
                return true;

            #endregion

            var objErrorModelControl = new ErrorModelControl { WarningText = GetWarningLangRsrc() };

            #region Adding the accounts to Warning window

            accountHavingTemplates.ForEach(account =>
            {
                if (_jobActivityConfigurationManager[account.AccountId, _activityType] != null)
                    objErrorModelControl.Accounts.Add(new AccountDetails
                    { UserName = account.AccountBaseModel.UserName });
            });

            var warningWindow =
                new Dialog().GetMetroWindowWithOutClose(objErrorModelControl,
                    "LangKeyWarning".FromResourceDictionary());

            #endregion

            #region Warning windows save button event

            objErrorModelControl.BtnSave.Click += (senders, events) =>
            {
                #region Remove not selected accounts from errorModelControl

                var nonSelectedAccounts = objErrorModelControl.Accounts.Where(x => !x.IsChecked).Select(x => x.UserName)
                    .ToList();

                nonSelectedAccounts.ForEach(removingAccount =>
                {
                    _footerControl.list_SelectedAccounts.Remove(removingAccount);
                    accountDetails.Remove(accountDetails.FirstOrDefault(x => x.UserName == removingAccount));
                });

                #endregion

                var remainingAccount = _footerControl.list_SelectedAccounts.DeepCloneObject();
                var selectedAccount = objErrorModelControl.Accounts.Where(x => x.IsChecked).Select(x => x.UserName)
                    .ToList();

                if (selectedAccount.Count == 0)
                {
                    warningWindow.Close();
                    return;
                }

                var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
                accountDetails.ForEach(account =>
                {
                    var moduleSettings = _jobActivityConfigurationManager[account.AccountId, _activityType];

                    if (moduleSettings == null)
                        return;

                    campaignFileManager.DeleteSelectedAccount(moduleSettings.TemplateId,
                        account.AccountBaseModel.UserName);
                    var campToUpdate = Campaigns.GetCampaignsInstance(SocialNetwork).CampaignViewModel
                        .LstCampaignDetails.FirstOrDefault(x => x.TemplateId == moduleSettings.TemplateId);
                    campToUpdate?.SelectedAccountList.Remove(account.AccountBaseModel.UserName);

                    _dominatorScheduler.StopActivity(account, _activityType.ToString(), moduleSettings.TemplateId,
                        false);

                    _jobActivityConfigurationManager.Delete(account.AccountId, _activityType);
                });

                _accountsCacheService.UpsertAccounts(accountDetails.ToArray());
                _footerControl.list_SelectedAccounts = remainingAccount;
                warningWindow.Close();
            };

            #endregion

            #region Warning windows cancel button event

            objErrorModelControl.BtnCancel.Click += (senders, events) =>
            {
                needToCancel = true;
                warningWindow.Close();
            };

            #endregion

            if (objErrorModelControl.Accounts.Count != 0)
                warningWindow.ShowDialog();

            SelectedAccountCount = _footerControl.list_SelectedAccounts.Count +
                                   $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            if (needToCancel || _footerControl.list_SelectedAccounts.Count == 0) return false;
            return true;
        }

        public void SaveTemplateToAccounts(string templateId)
        {
            List<RunningTimes> runningTime = Model.JobConfiguration.RunningTime;
            var accountDetails =
                _accountsFileManager.GetAllAccounts(_footerControl.list_SelectedAccounts, SocialNetwork);
            int accountCount = 0;
            accountDetails.ForEach(account => { AddTemplateToAccount(templateId, account, runningTime, accountCount++); });
            var accountsCacheService = InstanceProvider.GetInstance<IAccountsCacheService>();
            accountsCacheService.UpsertAccounts(accountDetails.ToArray());
        }

        private void AddTemplateToAccount(string templateId, DominatorAccountModel account,
            List<RunningTimes> runningTime, int accountCount = 0)
        {
            var moduleConfiguration =
                _jobActivityConfigurationManager[account.AccountBaseModel.AccountId, _activityType] ??
                new ModuleConfiguration { ActivityType = _activityType };

            moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();
            moduleConfiguration.IsEnabled = true;
            moduleConfiguration.Status = "Active";
            moduleConfiguration.TemplateId = templateId;
            moduleConfiguration.IsTemplateMadeByCampaignMode = true;
            moduleConfiguration.DelayBetweenJobs = Model.JobConfiguration.DelayBetweenJobs;
            moduleConfiguration.DelayBetweenAccounts = Model.JobConfiguration.DelayBetweenAccounts;
            moduleConfiguration.AccountCount = accountCount;
            runningTime.ForEach(x =>
            {
                foreach (var timingRange in x.Timings) timingRange.Module = _activityType.ToString();
            });
            moduleConfiguration.LstRunningTimes = new List<RunningTimes>(runningTime);

            moduleConfiguration.NextRun = DateTimeUtilities.GetStartTimeOfNextJob(moduleConfiguration, 0);
            _jobActivityConfigurationManager.AddOrUpdate(account.AccountBaseModel.AccountId,
                moduleConfiguration.ActivityType, moduleConfiguration);

            //Update ActivityManager of account in Db
            globalDbOperation.UpdateAccountActivityManager(account);
        }

        //public void SaveTemplateToAccounts(string templateId, List<RunningTimes> runningTime)
        //{
        //    var accountDetails = _accountsFileManager.GetAllAccounts(_footerControl.list_SelectedAccounts, SocialNetwork);

        //    accountDetails.ForEach(account =>
        //    {
        //        var moduleConfiguration = _jobActivityConfigurationManager[account.AccountId, _activityType] ?? new ModuleConfiguration { ActivityType = _activityType };
        //        moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();
        //        moduleConfiguration.IsEnabled = true;
        //        moduleConfiguration.Status = "Active";
        //        moduleConfiguration.TemplateId = templateId;
        //        moduleConfiguration.IsTemplateMadeByCampaignMode = true;

        //        _jobActivityConfigurationManager.AddOrUpdate(account.AccountId, _activityType, moduleConfiguration);

        //        runningTime.ForEach(x =>
        //        {
        //            foreach (var timingRange in x.Timings)
        //            {
        //                timingRange.Module = _activityType.ToString();
        //            }
        //        });

        //        account.ActivityManager.RunningTime = runningTime;

        //        moduleConfiguration.LstRunningTimes = new List<RunningTimes>(account.ActivityManager.RunningTime);

        //        _jobActivityConfigurationManager.AddOrUpdate(account.AccountBaseModel.AccountId, _activityType, moduleConfiguration);
        //        globalDbOperation.UpdateAccountActivityManager(account);
        //    });
        //    _accountsCacheService.UpsertAccounts(accountDetails.ToArray());

        //}

        #endregion

        #region Campaign related functions

        /// <summary>
        ///     Calls when user click 'Select Accounts' button in footer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FooterControl_OnSelectAccountChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                var objSelectAccountControl = new SelectAccountControl(_footerControl.list_SelectedAccounts);

                var objDialog = new Dialog();

                var window = objDialog.GetMetroWindow(objSelectAccountControl,
                    "LangKeySelectAccount".FromResourceDictionary());

                objSelectAccountControl.btnSave.Click += (senders, Events) =>
                {
                    var selectedAccount = objSelectAccountControl.GetSelectedAccount();
                    if (selectedAccount.Count > 0)
                    {
                        _footerControl.list_SelectedAccounts = objSelectAccountControl.GetSelectedAccount().ToList();
                        SelectedAccountCount = _footerControl.list_SelectedAccounts.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                        GlobusLogHelper.log.Info(Log.SelectedAccount, SocinatorInitialize.ActiveSocialNetwork,
                            CampaignName, _footerControl.list_SelectedAccounts.Count, CampaignName);
                    }
                    else
                    {
                        SelectedAccountCount = ConstantVariable.NoAccountSelected();
                        _footerControl.list_SelectedAccounts = selectedAccount.ToList();
                    }

                    window.Close();
                };

                objSelectAccountControl.btnCancel.Click += (senders, events) => window.Close();
                window.ShowDialog();
            }
            catch (Exception Ex)
            {
                Ex.DebugLog();
            }
        }

        protected void HeaderControl_OnCancelEditClick(object sender, RoutedEventArgs e)
        {
            SetDataContext();
            TabSwitcher.GoToCampaign();
        }

        public void FooterControl_OnSelectAccountChanged(List<string> listOfSelectedAccounts)
        {
            if (listOfSelectedAccounts.Count > 0)
            {
                _footerControl.list_SelectedAccounts = listOfSelectedAccounts;
                SelectedAccountCount = _footerControl.list_SelectedAccounts.Count +
                                       $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                GlobusLogHelper.log.Info(Log.SelectedAccount, SocinatorInitialize.ActiveSocialNetwork, "",
                    _footerControl.list_SelectedAccounts.Count, _activityType);
            }
            else
            {
                SelectedAccountCount = ConstantVariable.NoAccountSelected();
                _footerControl.list_SelectedAccounts = listOfSelectedAccounts;
            }
        }

        #endregion

        #region Update campaign

        protected void UpdateCampaign()
        {
            if (ValidateCampaignName())
                return;

            if (!ValidateCampaign())
                return;

            if (!ValidateExtraProperty())
                return;

            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var currentCampaign = campaignFileManager.FirstOrDefault(camp => camp.TemplateId == TemplateId);

            try
            {
                var newlyAddedAccounts = _footerControl.list_SelectedAccounts
                    .Except(currentCampaign?.SelectedAccountList).ToList();
                var existingAccounts = _footerControl.list_SelectedAccounts.Except(newlyAddedAccounts).ToList();
                var removedAccounts = currentCampaign?.SelectedAccountList.Except(existingAccounts).ToList();
                if (newlyAddedAccounts.Count != 0)
                {
                    if (!UpdateNewlyAddedAccounts(newlyAddedAccounts))
                        return;
                }
                else
                {
                    var selectedAccounts = _accountsFileManager.GetAllAccounts(currentCampaign.SelectedAccountList,SocialNetwork);
                    if(selectedAccounts != null && selectedAccounts.Count > 0)
                    {
                        selectedAccounts.ForEach(account =>
                        {
                            var moduleSetting = _jobActivityConfigurationManager[account.AccountId, _activityType];
                            if (moduleSetting != null)
                            {
                                moduleSetting.DelayBetweenJobs = Model.JobConfiguration.DelayBetweenJobs;
                                moduleSetting.DelayBetweenAccounts = Model.JobConfiguration.DelayBetweenAccounts;
                                _jobActivityConfigurationManager.AddOrUpdate(account.AccountId,_activityType, moduleSetting);
                            }
                        });
                    }
                }
                var accountToRemoveModuleConfiguration =
                    _accountsFileManager.GetAllAccounts(removedAccounts, SocialNetwork);

                #region Remove TemplateId from removed account from campaign selected account list

                accountToRemoveModuleConfiguration.ForEach(account =>
                {
                    try
                    {
                        var moduleSettings = _jobActivityConfigurationManager[account.AccountId, _activityType];
                        _jobActivityConfigurationManager.Delete(account.AccountId, _activityType);
                        _dominatorScheduler.StopActivity(account, _activityType.ToString(),
                            moduleSettings?.TemplateId, false);
                        _jobActivityConfigurationManager.Delete(account.AccountId, _activityType);
                        globalDbOperation.UpdateAccountActivityManager(account);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });

                _accountsCacheService.UpsertAccounts(accountToRemoveModuleConfiguration.ToArray());

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            // Update Template Details
            var TemplatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            TemplatesFileManager.ApplyFunc(template =>
            {
                if (template.Id != TemplateId)
                    return false;

                var model = JsonConvert.DeserializeObject<TModel>(template.ActivitySettings);
                var firstRunningTime = ((dynamic)model).JobConfiguration.RunningTime;
                var secondRunningTime = Model.JobConfiguration.RunningTime;
                var delayBetweenAccounts = Model.JobConfiguration.DelayBetweenAccounts;

                var result = _dominatorScheduler.CompareRunningTime(firstRunningTime, secondRunningTime);
                if (!result)
                {
                    try
                    {
                        var lstAccountDetails = _accountsFileManager.GetAll(SocinatorInitialize.ActiveSocialNetwork);

                        int accountCount = 0;
                        foreach (var accountModel in lstAccountDetails.Where(x =>
                            _footerControl.list_SelectedAccounts.Contains(x.AccountBaseModel.UserName)))
                        {
                            var moduleSettings =
                                _jobActivityConfigurationManager[accountModel.AccountId, _activityType];
                            moduleSettings.LstRunningTimes = secondRunningTime;
                            moduleSettings.DelayBetweenAccounts = delayBetweenAccounts;
                            moduleSettings.AccountCount = accountCount;
                            _dominatorScheduler.StopActivity(accountModel, _activityType.ToString(), TemplateId, false);
                            accountCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    // Update the template configuration 
                    template.ActivitySettings = JsonConvert.SerializeObject((TModel)Model);
                }

                return true;
            });


            // Update Campaign Details
            campaignFileManager.ToList().ForEach(campaign =>
            {
                if (campaign.TemplateId == TemplateId)
                {
                    campaign.CampaignName = CampaignName;
                    campaign.MainModule = _moduleName;
                    campaign.SubModule = _activityType.ToString();
                    campaign.SocialNetworks = SocialNetwork;
                    campaign.SelectedAccountList = _footerControl.list_SelectedAccounts;
                    campaign.TemplateId = TemplateId;
                    campaign.CreationDate = DateTimeUtilities.GetEpochTime();
                    campaign.Status = "Active";
                    campaign.LastEditedDate = DateTimeUtilities.GetEpochTime();

                    #region Updating Campaign UI

                    var LstCampaignDetails = Campaigns.GetCampaignsInstance(SocialNetwork).CampaignViewModel
                        .LstCampaignDetails;

                    var oldCampaignindex =
                        LstCampaignDetails.IndexOf(
                            LstCampaignDetails.FirstOrDefault(x => x.CampaignId == campaign.CampaignId));

                    if (oldCampaignindex >= 0)
                    {
                        LstCampaignDetails[oldCampaignindex].CampaignName = CampaignName;
                        LstCampaignDetails[oldCampaignindex].SelectedAccountList = _footerControl.list_SelectedAccounts;
                        LstCampaignDetails[oldCampaignindex].TemplateId = TemplateId;
                        LstCampaignDetails[oldCampaignindex].CreationDate = campaign.CreationDate;
                        LstCampaignDetails[oldCampaignindex].Status = "Active";
                        LstCampaignDetails[oldCampaignindex].LastEditedDate = campaign.LastEditedDate;
                    }

                    #endregion

                    campaignFileManager.Edit(campaign);
                }
            });
            var islogged = false;
            // Update Account Detail
            var accountDetails = _accountsFileManager.GetAll(SocialNetwork);
            if (UpdateSelectedAccountDetails(accountDetails, _footerControl.list_SelectedAccounts,
                Model.JobConfiguration))
                ToasterNotification.ShowSuccess(
                    $"{"LangKeyCampaign".FromResourceDictionary()}:- {CampaignName} {"LangKeyUpdatedSuccessfully".FromResourceDictionary()}");

            SetDataContext();

            TabSwitcher.GoToCampaign();

            var softwareSettings = InstanceProvider.GetInstance<ISoftwareSettings>();
            if (softwareSettings.Settings.IsThreadLimitChecked)
                if (DominatorScheduler._lockWithThreadLimit?.CurrentCount == 0 && !islogged)
                {
                    var maxThreadCount = softwareSettings.Settings.MaxThreadCount;
                    GlobusLogHelper.log.Info(
                        $"{"LangKeyThreadLimitReachedTo".FromResourceDictionary()} {maxThreadCount} {"LangKeyPendingStartsWhenRunnningStops".FromResourceDictionary()}");
                    islogged = true;
                }
        }

        private string GetWarningLangRsrc()
        {
            try
            {
                return Application.Current.FindResource("LangKeyStartWarning") + $" {_activityType}. " +
                       Application.Current.FindResource("LangKeyEndWarning");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return
                    $"These accounts are already having configuration settings for activity: {_activityType} . Saving this settings will override previous settings and remove this account from the campaign.";
            }
        }

        private bool UpdateNewlyAddedAccounts(List<string> newlyAddedAccounts)
        {
            var isProcessSuccessful = false;
            var needToCancel = false;

            #region Get the accounts which holds template Id

            var accountDetails = _accountsFileManager.GetAllAccounts(newlyAddedAccounts, SocialNetwork);

            var accountHavingTemplates = accountDetails
                .Where(x => _jobActivityConfigurationManager[x.AccountId, _activityType]?.TemplateId != null).ToList();

            #endregion

            #region If account having any template then Show ErrorModelControl with warning

            if (accountHavingTemplates.Count != 0)
            {
                var objErrorModelControl = new ErrorModelControl { WarningText = GetWarningLangRsrc() };

                #region Adding the accounts to Warning window

                accountHavingTemplates.ForEach(account =>
                {
                    var moduleSettings = _jobActivityConfigurationManager[account.AccountId, _activityType];

                    if (moduleSettings != null)
                        objErrorModelControl.Accounts.Add(new AccountDetails
                        {
                            UserName = account.AccountBaseModel.UserName
                        });
                });

                var warningWindow = new Dialog().GetMetroWindowWithOutClose(objErrorModelControl,
                    "LangKeyWarning".FromResourceDictionary());

                #endregion

                #region Warning windows save button event

                objErrorModelControl.BtnSave.Click += (senders, events) =>
                {
                    #region Remove not selected accounts from errorModelControl

                    var nonSelectedAccounts = objErrorModelControl.Accounts.Where(x => !x.IsChecked)
                        .Select(x => x.UserName)
                        .ToList();

                    nonSelectedAccounts.ForEach(removingAccount =>
                    {
                        _footerControl.list_SelectedAccounts.Remove(removingAccount);
                    });

                    #endregion

                    var selectedAccount = objErrorModelControl.Accounts.Where(x => x.IsChecked).Select(x => x.UserName)
                        .ToList();

                    if (selectedAccount.Count == 0)
                    {
                        isProcessSuccessful = true;
                        warningWindow.Close();
                        return;
                    }

                    #region Update already existing account setting

                    var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
                    accountDetails.ForEach(account =>
                    {
                        if (!selectedAccount.Contains(account.AccountBaseModel.UserName))
                            return;

                        var moduleSettings = _jobActivityConfigurationManager[account.AccountId, _activityType];

                        if (moduleSettings == null)
                            return;
                        campaignFileManager.DeleteSelectedAccount(moduleSettings.TemplateId,
                            account.AccountBaseModel.UserName);

                        var campToUpdate = Campaigns.GetCampaignsInstance(SocialNetwork).CampaignViewModel
                            .LstCampaignDetails.FirstOrDefault(x => x.TemplateId == moduleSettings.TemplateId);
                        campToUpdate?.SelectedAccountList.Remove(account.AccountBaseModel.UserName);

                        moduleSettings.TemplateId = TemplateId;
                        moduleSettings.NextRun = DateTimeUtilities.GetStartTimeOfNextJob(moduleSettings);
                        _dominatorScheduler.StopActivity(account, _activityType.ToString(),
                            moduleSettings.TemplateId, true);
                        _jobActivityConfigurationManager.AddOrUpdate(account.AccountBaseModel.AccountId, _activityType,
                            moduleSettings);
                        globalDbOperation.UpdateAccountActivityManager(account);
                    });

                    _accountsCacheService.UpsertAccounts(accountDetails.ToArray());
                    isProcessSuccessful = true;

                    #endregion

                    warningWindow.Close();
                };

                #endregion

                #region Warning windows cancel button event

                objErrorModelControl.BtnCancel.Click += (senders, events) =>
                {
                    needToCancel = true;
                    warningWindow.Close();
                };

                #endregion

                if (objErrorModelControl.Accounts.Count != 0)
                    warningWindow.ShowDialog();
            }

            #endregion

            #region If account don't have any template then set that account template to current template

            else
            {
                #region Update newly added account setting

                accountDetails.ForEach(account =>
                {
                    AddTemplateToAccount(TemplateId, account, Model.JobConfiguration.RunningTime);
                });
                isProcessSuccessful = true;

                #endregion
            }

            #endregion

            SelectedAccountCount = _footerControl.list_SelectedAccounts.Count +
                                   $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            if (_footerControl.list_SelectedAccounts.Count == 0 || needToCancel) return false;
            return isProcessSuccessful;
        }

        //public bool UpdateSelectedAccountDetails(List<DominatorAccountModel> allAccountDetails, List<string> listSelectedAccounts, List<RunningTimes> runningTime)
        //{
        //    var isAccountDetailsUpdated = false;
        //    bool islogged = false;
        //    var selectedAccounts = new List<DominatorAccountModel>(listSelectedAccounts.Count);
        //    var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
        //    foreach (var account in allAccountDetails)
        //    {
        //        if (!listSelectedAccounts.Contains(account.AccountBaseModel.UserName))
        //            continue;

        //        isAccountDetailsUpdated = true;
        //        try
        //        {
        //            if (account.ActivityManager.RunningTime == null)
        //                account.ActivityManager.RunningTime = RunningTimes.DayWiseRunningTimes;

        //            var moduleConfiguration =
        //                _jobActivityConfigurationManager[account.AccountId, _activityType] ??
        //                new ModuleConfiguration { ActivityType = _activityType };
        //            if (DominatorScheduler._lockWithThreadLimit?.CurrentCount == 0 && !islogged)
        //            {
        //                GlobusLogHelper.log.Info("Thread limit reached while Updating.");
        //                islogged = true;
        //            }
        //            _dominatorScheduler.StopActivity(account, _activityType.ToString(), moduleConfiguration.TemplateId, true);
        //            _jobActivityConfigurationManager.AddOrUpdate(account.AccountId, _activityType, moduleConfiguration);
        //            moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();

        //            moduleConfiguration.IsEnabled = true;

        //            moduleConfiguration.Status = "Active";

        //            moduleConfiguration.TemplateId = TemplateId;
        //            moduleConfiguration.NextRun = DateTimeUtilities.GetStartTimeOfNextJob(moduleConfiguration);

        //            moduleConfiguration.IsTemplateMadeByCampaignMode = true;

        //            runningTime.ForEach(x =>
        //            {
        //                foreach (var timingRange in x.Timings)
        //                {
        //                    timingRange.Module = _activityType.ToString();
        //                }
        //            });

        //            account.ActivityManager.RunningTime = runningTime;
        //            moduleConfiguration.LstRunningTimes = new List<RunningTimes>(account.ActivityManager.RunningTime);
        //            moduleConfiguration.IsTemplateMadeByCampaignMode = true;

        //            selectedAccounts.Add(account);
        //            _jobActivityConfigurationManager.AddOrUpdate(account.AccountBaseModel.AccountId, _activityType, moduleConfiguration);

        //            globalDbOperation.UpdateAccountActivityManager(account);
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.DebugLog();
        //        }
        //    }
        //    _accountsCacheService.UpsertAccounts(allAccountDetails.ToArray());


        //    // schedule actitvities of selected accounts            
        //    foreach (var account in selectedAccounts)
        //    {
        //        _dominatorScheduler.ScheduleNextActivity(account, _activityType);
        //    }
        //    return isAccountDetailsUpdated;

        //}


        /// <summary>
        ///     UpdateSelectedAccountDetails method will take AccountDetails and list of selected account to modify as  argument
        ///     and
        ///     will update that account with setting and return status
        /// </summary>
        /// <param name="allAccountDetails"></param>
        /// <param name="listSelectedAccounts"></param>
        /// <param name="jobConfiguration"></param>
        /// <returns></returns>
        protected bool UpdateSelectedAccountDetails(IEnumerable<DominatorAccountModel> allAccountDetails,
            List<string> listSelectedAccounts, JobConfiguration jobConfiguration)
        {
            var isAccountDetailsUpdated = false;

            ModuleConfiguration moduleConfiguration = null;

            var selectedAccounts = new List<DominatorAccountModel>(listSelectedAccounts.Count);
            foreach (var account in allAccountDetails)
            {
                if (!listSelectedAccounts.Contains(account.AccountBaseModel.UserName))
                    continue;

                isAccountDetailsUpdated = true;
                try
                {
                    moduleConfiguration = _jobActivityConfigurationManager[account.AccountId, _activityType] ??
                                              new ModuleConfiguration { ActivityType = _activityType };
                    moduleConfiguration.TemplateId = TemplateId;
                    moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();
                    moduleConfiguration.IsEnabled = true;
                    moduleConfiguration.Status = "Active";
                    moduleConfiguration.IsTemplateMadeByCampaignMode = true;
                    // Update running times for current activity
                    UpdateRunningTime(jobConfiguration, account);

                    selectedAccounts.Add(account);
                    _jobActivityConfigurationManager.AddOrUpdate(account.AccountBaseModel.AccountId, _activityType,
                        moduleConfiguration);
                    globalDbOperation.UpdateAccountActivityManager(account);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            _accountsCacheService.UpsertAccounts(allAccountDetails.ToArray());

            //schedule actitvities of selected accounts            
            foreach (var account in selectedAccounts) { _dominatorScheduler.ScheduleNextActivity(account, _activityType); }


            return isAccountDetailsUpdated;
        }

        #endregion

        #region Query adding 

        /// <summary>
        ///     Add Query handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="queryParameterType"></param>
        protected void SearchQueryControl_OnAddQuery(object sender, RoutedEventArgs e, Type queryParameterType)
        {
            try
            {
                var queryValuIndex = new List<int>();
                if (string.IsNullOrEmpty(_queryControl.CurrentQuery.QueryValue) &&
                    _queryControl.QueryCollection.Count != 0)
                {
                    _queryControl.QueryCollection.ForEach(query =>
                    {
                        var currentQuery = _queryControl.CurrentQuery.Clone() as QueryInfo;

                        if (currentQuery == null) return;

                        currentQuery.QueryValue = query;
                        currentQuery.QueryTypeDisplayName = currentQuery.QueryType;
                        //  currentQuery.QueryTypeDisplayName = currentQuery.QueryTypeAsDisplayName(queryParameterType);

                        currentQuery.QueryPriority = Model.SavedQueries.Count + 1;

                        if (IsQueryExistWithoutDialog(currentQuery, Model.SavedQueries))
                        {
                            queryValuIndex.Add(_queryControl.QueryCollection.IndexOf(query));
                            return;
                        }

                        Model.SavedQueries.Add(currentQuery);
                    });
                    if (queryValuIndex.Count > 0)
                    {
                        if (queryValuIndex.Count <= 10)
                            GlobusLogHelper.log.Info(Log.AlreadyExistQuery, SocinatorInitialize.ActiveSocialNetwork,
                                CampaignName, _activityType,
                                "{ " + string.Join(" },{ ", queryValuIndex.ToArray()) + " }");
                        else
                            GlobusLogHelper.log.Info(Log.AlreadyExistQueryCount,
                                SocinatorInitialize.ActiveSocialNetwork, CampaignName, _activityType,
                                queryValuIndex.Count);
                    }
                }
                else
                {
                    _queryControl.CurrentQuery.QueryTypeDisplayName = _queryControl.CurrentQuery.QueryType;

                    // _queryControl.CurrentQuery.QueryTypeDisplayName = _queryControl.CurrentQuery.QueryTypeAsDisplayName(queryParameterType);

                    var currentQuery = _queryControl.CurrentQuery.Clone() as QueryInfo;

                    if (currentQuery == null) return;

                    currentQuery.QueryPriority = Model.SavedQueries.Count + 1;

                    if (IsQueryExist(currentQuery, Model.SavedQueries)) return;

                    Model.SavedQueries.Add(currentQuery);

                    _queryControl.CurrentQuery = new QueryInfo();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool IsQueryExist(QueryInfo currentQuery, ObservableCollection<QueryInfo> queryToSave)
        {
            try
            {
                if (queryToSave.Any(x =>
                    x.QueryType == currentQuery.QueryType && x.QueryValue == currentQuery.QueryValue))
                {
                    Dialog.ShowDialog(Application.Current.MainWindow,
                        "LangKeyAlert".FromResourceDictionary(),
                        "LangKeyQueryAlreadyExist".FromResourceDictionary());
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private bool IsQueryExistWithoutDialog(QueryInfo currentQuery, ObservableCollection<QueryInfo> queryToSave)
        {
            try
            {
                if (queryToSave.Any(x =>
                    x.QueryType == currentQuery.QueryType && x.QueryValue == currentQuery.QueryValue))
                    return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        #endregion

        #region Save Configuration Implementation

        #region Toggle Module Status changes 

        protected bool ChangeAccountsModuleStatus(bool isStart, string selectedAccount, SocialNetworks socialNetworks)
        {
            try
            {
                var accountModel = _accountsFileManager.GetAccount(selectedAccount, socialNetworks);
                var moduleConfiguration = _jobActivityConfigurationManager[accountModel.AccountId, _activityType];
                if (moduleConfiguration?.TemplateId == null || moduleConfiguration.LstRunningTimes == null)
                {
                    Model.IsAccountGrowthActive = !isStart;

                    if (isStart)
                    {
                        Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                            "LangKeyPleaseSaveSettings".FromResourceDictionary());
                        return false;
                    }
                }

                if (moduleConfiguration != null)
                {
                    moduleConfiguration.IsEnabled = isStart;
                    try
                    {
                        var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
                        var campaignStatus = campaignFileManager
                            .FirstOrDefault(x => x.TemplateId == moduleConfiguration.TemplateId)
                            ?.Status;
                        if (campaignStatus == "Paused" && moduleConfiguration.IsEnabled)
                        {
                            Dialog.ShowDialog(this,
                                "LangKeyError".FromResourceDictionary(),
                                "LangKeyErrorCampaignConfigurationIsPaused".FromResourceDictionary());
                            moduleConfiguration.IsEnabled = false;
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    _jobActivityConfigurationManager.AddOrUpdate(accountModel.AccountBaseModel.AccountId, _activityType,
                        moduleConfiguration);
                    _accountsCacheService.UpsertAccounts(accountModel);
                    globalDbOperation.UpdateAccountActivityManager(accountModel);
                    if (!moduleConfiguration.IsEnabled)
                    {
                        _dominatorScheduler.StopActivity(accountModel, _activityType.ToString(),
                            moduleConfiguration.TemplateId, moduleConfiguration.IsEnabled);
                        ToasterNotification.ShowSuccess("LangKeySuccessfullyStopped".FromResourceDictionary());
                    }
                    else
                    {
                        ToasterNotification.ShowSuccess("LangKeySuccessfullyActivated".FromResourceDictionary());
                        _dominatorScheduler.ScheduleNextActivity(accountModel, _activityType);
                    }

                    return moduleConfiguration.IsEnabled;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }

        #endregion

        #region Account selection changed events

        protected virtual void SetAccountModeDataContext(SocialNetworks network)
        {
            try
            {
                SocialNetwork = network;

                var accountDetails = _accountsFileManager.GetAccount(_accountGrowthModeHeader.SelectedItem, network);

                SocinatorInitialize.GetSocialLibrary(network)
                        .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount =
                    _accountGrowthModeHeader.SelectedItem;

                var jobActivityConfigurationManager =
                    InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration = jobActivityConfigurationManager[accountDetails.AccountId, _activityType];
                if (moduleConfiguration != null)
                {
                    var TemplatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                    var templateDetails = TemplatesFileManager.GetTemplateById(moduleConfiguration.TemplateId);
                    SetModuleValues(moduleConfiguration.IsEnabled, templateDetails);
                }
                else
                {
                    SetModuleValues(false, null);
                }

                _mainGrid.DataContext = ObjViewModel;
                _accountGrowthModeHeader.DataContext = this;
                SetSelectedAccounts(accountDetails.AccountBaseModel.AccountNetwork);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        #region Save last selected accounts in account configuration mode

        public void SetSelectedAccounts(SocialNetworks networks)
        {
            var accounts = new ObservableCollectionBase<string>(_accountsFileManager.GetAll()
                .Where(x => x.AccountBaseModel.AccountNetwork == networks).Select(x => x.UserName));

            _accountGrowthModeHeader.AccountItemSource = accounts;

            _accountGrowthModeHeader.SelectedItem = SocinatorInitialize.GetSocialLibrary(networks)
                .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;
        }

        #endregion

        public void SaveConfigurations()
        {
            //Todo
            //if (!ValidateQuery()) return;

            if (!ValidateExtraProperty()) return;

            if (!ValidateRunningTime()) return;

            var accountModel = _accountsFileManager.GetAccount(_accountGrowthModeHeader.SelectedItem, SocialNetwork);
            //var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleConfiguration = _jobActivityConfigurationManager[accountModel.AccountId, _activityType];
            if (moduleConfiguration?.TemplateId != null)
            {
                if (moduleConfiguration.IsTemplateMadeByCampaignMode)
                {
                    var dialogResult = Dialog.ShowCustomDialog("LangKeyWarning".FromResourceDictionary(),
                        "LangKeyConfirmToOverrideAccountRunningWithAnotherCampaign".FromResourceDictionary(),
                        "LangKeyYes".FromResourceDictionary(), "LangKeyNo".FromResourceDictionary());
                    if (dialogResult == MessageDialogResult.Negative)
                        return;
                }

                #region Template Id present case

                var previousSettings = moduleConfiguration.DeepCloneObject();

                // update the template
                UpdateTemplate(accountModel, moduleConfiguration.TemplateId,
                    moduleConfiguration.IsTemplateMadeByCampaignMode);

                // need to check whether its running or not, if its running then need to stop process
                _dominatorScheduler.StopActivity(accountModel, _activityType.ToString(),
                    previousSettings.TemplateId, previousSettings.IsEnabled);

                // update the running time
                UpdateRunningTime(Model.JobConfiguration, accountModel);
                //_dominatorScheduler.ScheduleNextActivity(accountModel, _activityType);
                ToasterNotification.ShowSuccess("LangKeySuccessfullySaved".FromResourceDictionary());

                #endregion
            }
            else
            {
                #region Template not present case

                moduleConfiguration = new ModuleConfiguration { ActivityType = _activityType };
                _jobActivityConfigurationManager.AddOrUpdate(accountModel.AccountId, _activityType,
                    moduleConfiguration);

                moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();

                TemplateId = TemplateModel.SaveTemplate((TModel)Model, _activityType.ToString(), SocialNetwork,
                    $"{accountModel.AccountBaseModel.AccountId}-Configuration");

                moduleConfiguration.TemplateId = TemplateId;

                var runningTime = (List<RunningTimes>)Model.JobConfiguration.RunningTime;

                runningTime.ForEach(x =>
                {
                    foreach (var timingRange in x.Timings) timingRange.Module = _activityType.ToString();
                });

                accountModel.ActivityManager.RunningTime = runningTime;

                moduleConfiguration.LstRunningTimes = new List<RunningTimes>(accountModel.ActivityManager.RunningTime);

                moduleConfiguration.IsTemplateMadeByCampaignMode = false;

                moduleConfiguration.NextRun = DateTimeUtilities.GetStartTimeOfNextJob(moduleConfiguration);
                _jobActivityConfigurationManager.AddOrUpdate(accountModel.AccountBaseModel.AccountId, _activityType,
                    moduleConfiguration);
                _accountsCacheService.UpsertAccounts(accountModel);

                Dialog.ShowDialog("LangKeySuccess".FromResourceDictionary(),
                    "LangKeySuccessfullySaved".FromResourceDictionary());

                #endregion
            }

            globalDbOperation.UpdateAccountActivityManager(accountModel);
        }

        private static void AddNewTemplate<T>(T moduleToSave, string userName, ActivityType moduleType,
            DominatorAccountModel account) where T : class
        {
            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var accountModuleConfiguration = jobActivityConfigurationManager[account.AccountId, moduleType];
            if (accountModuleConfiguration == null)
                return;

            accountModuleConfiguration.TemplateId = TemplateModel.SaveTemplate(
                moduleToSave,
                moduleType.ToString(),
                account.AccountBaseModel.AccountNetwork,
                userName + "_" + moduleType + "_Template");

            accountModuleConfiguration.IsTemplateMadeByCampaignMode = false;
        }

        private void UpdateTemplate(DominatorAccountModel accountModel, string accountstemplateId,
            bool isTemplateMadeByCampaignMode)
        {
            try
            {
                if (isTemplateMadeByCampaignMode)
                {
                    var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
                    campaignFileManager.DeleteSelectedAccount(accountstemplateId,
                        _accountGrowthModeHeader.SelectedItem);

                    var campToUpdate = Campaigns.GetCampaignsInstance(SocialNetwork).CampaignViewModel
                        .LstCampaignDetails.FirstOrDefault(x => x.TemplateId == accountstemplateId);
                    campToUpdate?.SelectedAccountList.Remove(accountModel.AccountBaseModel.UserName);

                    AddNewTemplate((TModel)Model, _accountGrowthModeHeader.SelectedItem, _activityType, accountModel);
                }
                else
                {
                    var TemplatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                    if (string.IsNullOrEmpty(accountstemplateId))
                        AddNewTemplate((TModel)Model, _accountGrowthModeHeader.SelectedItem, _activityType,
                            accountModel);

                    // Updating existing template
                    else
                        TemplatesFileManager.UpdateActivitySettings(accountstemplateId,
                            JsonConvert.SerializeObject((TModel)Model));
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog("Accounts details not saved!");
            }
        }

        public void UpdateRunningTime(JobConfiguration jobConfiguration, DominatorAccountModel account)
        {
            try
            {
                jobConfiguration.RunningTime.ForEach(x =>
                {
                    foreach (var timingRange in x.Timings)
                        timingRange.Module = _activityType.ToString();
                });

                account.ActivityManager.RunningTime = jobConfiguration.RunningTime;

                //  var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                var accountModuleSettings = _jobActivityConfigurationManager[account.AccountId, _activityType];
                if (accountModuleSettings != null)
                {
                    if (accountModuleSettings.LstRunningTimes == null)
                        accountModuleSettings.LstRunningTimes = new List<RunningTimes>();

                    accountModuleSettings.LstRunningTimes = jobConfiguration.RunningTime;

                    accountModuleSettings.NextRun = DateTimeUtilities.GetStartTimeOfNextJob(accountModuleSettings);
                    if (accountModuleSettings.NextRun < DateTime.Now)
                        accountModuleSettings.NextRun = DateTime.Now.AddMinutes(1);
                    _jobActivityConfigurationManager.AddOrUpdate(account.AccountBaseModel.AccountId, _activityType,
                        accountModuleSettings);
                }
                else
                {
                    GlobusLogHelper.log.Debug(
                        $"{account.UserName} with Account Id {account.AccountId} not having {_activityType} Configuration");
                }

                _accountsCacheService.UpsertAccounts(account);
                globalDbOperation.UpdateAccountActivityManager(account);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}