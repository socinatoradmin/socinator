using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Factories;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Command;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.EmailService;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.IoC;
using DominatorUIUtility.ViewModel.Startup;
using DominatorUIUtility.Views;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Unity;
using BindableBase = Prism.Mvvm.BindableBase;
using Dialog = DominatorHouseCore.Utility.Dialog;

namespace DominatorUIUtility.ViewModel
{
    public class ScheduleActivity
    {
        public static event Action OnDependencyInstalled;
        public static void StartScheduling()
        {
            OnDependencyInstalled?.Invoke();
        }
    }
    public interface IDominatorAccountViewModel
    {
        IAccountCollectionViewModel LstDominatorAccountModel { get; }
        void AddSingleAccountInThread(DominatorAccountBaseModel objDominatorAccountBaseModel, bool isIgOrTik = false, bool RunThroughBrowserAutomation = false, Window dialogWindow = null, string JsonCookies = "", string BrowserCookies = "");
        string DefaultAccountNameFromModel(IEnumerable<DominatorAccountModel> listAcc,
            ref Dictionary<SocialNetworks, int> dictNetLasNum, SocialNetworks net);
    }

    [ProtoContract]
    public class DominatorAccountViewModel : BindableBase, IDominatorAccountViewModel
    {
        private readonly IAccountsFileManager _accountsFileManager;
        private readonly IDataBaseHandler _dataBaseHandler;
        private readonly IProxyFileManager _proxyFileManager;
        private readonly IProxyManagerViewModel _proxyManagerViewModel;
        private readonly ISoftwareSettings _softwareSettings;
        public List<SocialNetworks> AsyncLoginNetworks = new List<SocialNetworks>() { SocialNetworks.Quora};
        private readonly object _syncLoadAccounts = new object();
        public SemaphoreSlim _accountsToLoginDuringMultipleImport { get; set; }
        private bool _allAccountsQueued;

        private double _height;

        private bool _IsProgressActive;
        private readonly IMainViewModel _mainViewModel;
        private MenuHandlerModel _MenuHandlerModel = new MenuHandlerModel();
        public MenuHandlerModel MenuHandlerModel
        {
            get => _MenuHandlerModel;
            set => SetProperty(ref _MenuHandlerModel, value, nameof(MenuHandlerModel));
        }
        private ImmutableQueue<Action> _pendingActions = ImmutableQueue<Action>.Empty;

        private ObservableCollection<GridViewColumn> _visibleColumns;

        private List<SocialNetworks> AdsNetworks { get; set; } = new List<SocialNetworks> { SocialNetworks.Facebook, SocialNetworks.TikTok, SocialNetworks.Reddit, SocialNetworks.Quora };
        public DominatorAccountViewModel ( IMainViewModel mainViewModel,
            ISelectedNetworkViewModel selectedNetworkViewModel, IProxyManagerViewModel proxyManagerViewModel,
            ISoftwareSettings softwareSettings, IAccountsFileManager accountsFileManager,
            IAccountCollectionViewModel accountCollectionViewModel, IDataBaseHandler dataBaseHandler,
            IProxyFileManager proxyFileManager )
        {
            _mainViewModel = mainViewModel;
            SelectedNetworkViewModel = selectedNetworkViewModel;
            _proxyManagerViewModel = proxyManagerViewModel;
            _softwareSettings = softwareSettings;
            _accountsFileManager = accountsFileManager;
            strategyPack = mainViewModel.Strategies;
            Groups = new ObservableCollection<ContentSelectGroup>();
            BindingOperations.EnableCollectionSynchronization(Groups, AccountCollectionViewModel.SyncObject);
            LstDominatorAccountModel = accountCollectionViewModel;
            _dataBaseHandler = dataBaseHandler;
            _proxyFileManager = proxyFileManager;
            _accountsToLoginDuringMultipleImport = new SemaphoreSlim(10,10);
            BindingOperations.EnableCollectionSynchronization(LstDominatorAccountModel,
                AccountCollectionViewModel.SyncObject);

            var visibleHeaders = DominatorAccountCountFactory.Instance.GetColumnSpecificationProvider().VisibleHeaders;
            VisibleColumns = new ObservableCollection<GridViewColumn>(visibleHeaders.Select(( name, colIndex ) =>
                new GridViewColumn
                {
                    DisplayMemberBinding = new Binding($"DisplayColumnValue{colIndex + 1}"),
                    Header = name,
                    Width = 130
                }));
            BindingOperations.EnableCollectionSynchronization(VisibleColumns, AccountCollectionViewModel.SyncObject);

            InitialAccountDetails();

            #region Command Initialization

            AddSingleAccountCommand = new DelegateCommand(AddSingleAccountExecute);

            #region Import Multiple Accounts Command.
            LoadMultipleAccountsCommand = new DelegateCommand(() =>
                LoadMultipleAccountsExecute(strategyPack._determine_available, strategyPack._inform_warnings));
            DownloadAccountFormats = new DelegateCommand<string>(DownloadAccountsFormats);
            #endregion
            InfoCommand = new DelegateCommand(InfoCommandExecute);

            ExportCommand = new DelegateCommand(ExportExecute);

            DeleteAccountsCommand = new DelegateCommand(DeleteAccountsExecute);

            SelectAccountCommand = new DelegateCommand<bool?>(SelectAccountExecute);

            SelectAccountByStatusCommand = new DelegateCommand<string>(SelectAccountByStatusExecute);

            SelectAccountByGroupCommand = new DelegateCommand<ContentSelectGroup>(SelectAccountByGroupExecute);

            SingleAccountDeleteCommand = new DelegateCommand<DominatorAccountModel>(SingleAccountDeleteExecute);

            UpdateAccountDetailsCommand =
                new BaseCommand<object>(UpdateAccountDetailsCanExecute, UpdateAccountDetailsExecute);

            UpdateGroupCommand = new DelegateCommand(UpdateGroupDetailsExecute);

            UpdateUserCradCommand = new BaseCommand<object>(sender => true, UpdateUserCradExecute);

            ActivateBrowserAutomationCommand = new BaseCommand<object>(ActivateBrowserAutomationCanExecute,
                ActivateBrowserAutomationExecute);

            DeActivateBrowserAutomationCommand = new BaseCommand<object>(DeActivateBrowserAutomationCommandCanExecute,
                DeActivateBrowserAutomationCommandExecute);

            //ImportButtonSizeChangedCommand = new BaseCommand<object>(ImportButtonSizeChangedCommandCanExecute,
            //    ImportButtonSizeChangedCommandExecute);

            //SelectButtonSizeChangedCommand = new BaseCommand<object>(SelectButtonSizeChangedCommandCanExecute,
            //    SelectButtonSizeChangedCommandExecute);

            //UpdateButtonSizeChangedCommand = new BaseCommand<object>(UpdateButtonSizeChangedCommandCanExecute,
            //    UpdateButtonSizeChangedCommandExecute);

            //ExportButtonSizeChangedCommand = new BaseCommand<object>(ExportButtonSizeChangedCommandCanExecute,
            //    ExportButtonSizeChangedCommandExecute);

            //DeleteButtonSizeChangedCommand = new BaseCommand<object>(DeleteButtonSizeChangedCommandCanExecute,
            //    DeleteButtonSizeChangedCommandExecute);

            //BrowserButtonSizeChangedCommand = new BaseCommand<object>(BrowserButtonSizeChangedCommandCanExecute,
            //    BrowserButtonSizeChangedCommandExecute);

            //InfoButtonSizeChnagedCommand = new BaseCommand<object>(InfoButtonSizeChnagedCommandCanExecute,
            //    InfoButtonSizeChnagedCommandExecute);

            SwitchToBusinessAccountCommand =
                new BaseCommand<object>(SwitchAccountTypeCommandCanExecute, SwitchAccountTypeCommandExecute);


            #region Context Menu Command

            ProfileDetailsCommand = new DelegateCommand<DominatorAccountModel>(ProfileDetails);
            DeleteAccountCommand = new DelegateCommand<DominatorAccountModel>(DeleteAccountByContextMenu);
            BrowserLoginCommand = new DelegateCommand<DominatorAccountModel>(AccountBrowserLogin);
            GoToCaptchaServiceCommand = new DelegateCommand<DominatorAccountModel>(GoToCaptchaService);
            GotoToolsCommand = new DelegateCommand<DominatorAccountModel>(GotoTools);
            CheckLoginCommand = new DelegateCommand<DominatorAccountModel>(AccountStatusChecker);
            UpdateFriendshipCommand = new DelegateCommand<DominatorAccountModel>(AccountUpdate);
            EditNetworkProfileCommand = new DelegateCommand<DominatorAccountModel>(EditProfile);
            CopyAccountIdCommand = new DelegateCommand<DominatorAccountModel>(CopyAccountId);
            SwitchToSingleBusinessAccountCommand =
                new DelegateCommand<DominatorAccountModel>(SwitchToSingleBusinessAccountCommandExecute);
            SwitchToSingleNormalAccountCommand =
                new DelegateCommand<DominatorAccountModel>(SwitchToSingleNormalAccountCommandExecute);

            #endregion

            #endregion

            #region Custom Setting Command

            SettingWizardCommand = new DelegateCommand<DominatorAccountModel>(CustomSetting);

            #endregion

            SelectedNetworkViewModel.ItemSelected += SelectedNetworkViewModel_ItemSelected;
        }

        public ObservableCollection<ContentSelectGroup> Groups { get; }
        public ISelectedNetworkViewModel SelectedNetworkViewModel { get; }

        public bool IsProgressActive
        {
            get => _IsProgressActive;
            set
            {
                if (_IsProgressActive == value)
                    return;
                SetProperty(ref _IsProgressActive, value);
            }
        }

        public double Height
        {
            get => _height;
            set
            {
                if (_height == value)
                    return;
                SetProperty(ref _height, value);
            }
        }


        public bool IsSortAccounts { get; set; }
            = InstanceProvider.GetInstance<ISoftwareSettingsFileManager>().GetSoftwareSettings()
                .DoNotSortByUserNameChecked;

        public bool SortByUserNikeName { get; set; }
            = InstanceProvider.GetInstance<ISoftwareSettingsFileManager>().GetSoftwareSettings().SortByNikename;
        public ObservableCollection<GridViewColumn> VisibleColumns
        {
            get => _visibleColumns;
            set => SetProperty(ref _visibleColumns, value);
        }

        public IAccountCollectionViewModel LstDominatorAccountModel { get; }

        private void SwitchToSingleNormalAccountCommandExecute ( DominatorAccountModel dominatorAccountModel )
        {
            try
            {
                var result = Dialog.ShowCustomDialog(
                    "LangKeyDeActivatingBusinessAccountforPinterest".FromResourceDictionary(),
                    "LangKeyStartActivityNormalAccount".FromResourceDictionary(),
                    "LangKeyContinue".FromResourceDictionary(), "LangKeyCancel".FromResourceDictionary());

                if (result == MessageDialogResult.Affirmative)
                {
                    StopAllActivity(new List<DominatorAccountModel> { dominatorAccountModel }, true);

                    ThreadFactory.Instance.Start(() =>
                    {
                        var accountFactory = SocinatorInitialize
                            .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                            .GetNetworkCoreFactory().AccountUpdateFactory;
                        if (accountFactory is IAccountUpdateAccountTypeFactoryAsync)
                        {
                            var asyncAccount = (IAccountUpdateAccountTypeFactoryAsync)accountFactory;
                            asyncAccount.SwitchToBusinessAccount(dominatorAccountModel, new CancellationToken(), false);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        private void SwitchToSingleBusinessAccountCommandExecute ( DominatorAccountModel dominatorAccountModel )
        {
            try
            {
                var result = Dialog.ShowCustomDialog(
                    "LangKeyActivatingBusinessAccountforPinterest".FromResourceDictionary(),
                    "LangKeyStartActivityBusinessAccount".FromResourceDictionary(),
                    "LangKeyContinue".FromResourceDictionary(), "LangKeyCancel".FromResourceDictionary());

                if (result == MessageDialogResult.Affirmative)
                {
                    StopAllActivity(new List<DominatorAccountModel> { dominatorAccountModel }, true);

                    ThreadFactory.Instance.Start(() =>
                    {
                        var accountFactory = SocinatorInitialize
                            .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                            .GetNetworkCoreFactory().AccountUpdateFactory;
                        if (accountFactory is IAccountUpdateAccountTypeFactoryAsync)
                        {
                            var asyncAccount = (IAccountUpdateAccountTypeFactoryAsync)accountFactory;
                            asyncAccount.SwitchToBusinessAccount(dominatorAccountModel, new CancellationToken());
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool SwitchAccountTypeCommandCanExecute ( object sender )
        {
            return true;
        }

        private void SwitchAccountTypeCommandExecute ( object sender )
        {
            try
            {
                var accountType = sender as string;

                var selectedAccount = LstDominatorAccountModel.Where(x => x.IsAccountManagerAccountSelected &&
                                                                          x.AccountBaseModel.AccountNetwork ==
                                                                          SocialNetworks.Pinterest).ToList();

                SwitchAccountType(selectedAccount, accountType);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public void SwitchAccountType ( List<DominatorAccountModel> selectedAccount, string accountType )
        {
            try
            {
                var result = accountType == "BusinessAccount"
                    ? Dialog.ShowCustomDialog("LangKeyActivatingBusinessAccountforPinterest".FromResourceDictionary(),
                        "LangKeyStartActivityBusinessAccount".FromResourceDictionary(),
                        "LangKeyContinue".FromResourceDictionary(), "LangKeyCancel".FromResourceDictionary())
                    : Dialog.ShowCustomDialog("LangKeyDeActivatingBusinessAccountforPinterest".FromResourceDictionary(),
                        "LangKeyStartActivityNormalAccount".FromResourceDictionary(),
                        "LangKeyContinue".FromResourceDictionary(), "LangKeyCancel".FromResourceDictionary());

                if (result == MessageDialogResult.Affirmative)
                {
                    var accountsToProcess = selectedAccount;

                    if (accountsToProcess.Count() == 0)
                    {
                        ToasterNotification.ShowInfomation(
                            $"{"LangKeyNoAccountsFoundToPerformAction".FromResourceDictionary()}");
                        return;
                    }

                    StopAllActivity(accountsToProcess.ToList(), true);

                    Task.Factory.StartNew(() =>
                    {
                        selectedAccount.ForEach(account =>
                        {
                            var accountFactory = SocinatorInitialize
                                .GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
                                .GetNetworkCoreFactory().AccountUpdateFactory;
                            try
                            {
                                if (!_changeBusinessAccountList.Contains(account.AccountBaseModel.UserName))
                                    MultipleBusinessAccoutSwitch(account, accountType, accountFactory);
                                else
                                    GlobusLogHelper.log.Info(Log.AlreadyUpdatingAccount,
                                        account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName);

                                Task.Delay(5);
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        //private bool SwitchToBusinessAccountCommandCanExecute ( object sender )
        //{
        //    return true;
        //}

        //private void InfoButtonSizeChnagedCommandExecute ( object Sender )
        //{
        //    if (((Button)Sender).ActualHeight == 40)
        //    {
        //        MenuHandlerModel.IsInfoVisible = false;
        //    }
        //    else
        //    {
        //        MenuHandlerModel.IsInfoVisible = true;
        //        MenuHandlerModel.IsBrowserAutomationVisible = true;
        //    }

        //    ChangeMenuHandlerStatus();
        //}

        //private bool InfoButtonSizeChnagedCommandCanExecute ( object arg )
        //{
        //    return true;
        //}

        //private void BrowserButtonSizeChangedCommandExecute ( object Sender )
        //{
        //    if (((DropDownButton)Sender).ActualHeight == 40)
        //        MenuHandlerModel.IsBrowserAutomationVisible = false;
        //    else
        //        MenuHandlerModel.IsBrowserAutomationVisible = true;

        //    ChangeMenuHandlerStatus();
        //}

        //private void ChangeMenuHandlerStatus ()
        //{
        //    if (MenuHandlerModel.IsBrowserAutomationVisible || MenuHandlerModel.IsDeleteAccountVisible
        //                                                    || MenuHandlerModel.IsExportAccountVisible ||
        //                                                    MenuHandlerModel.IsImportMultipleAccountsVisible
        //                                                    || MenuHandlerModel.IsInfoVisible ||
        //                                                    MenuHandlerModel.IsSelectAccountsVisible ||
        //                                                    MenuHandlerModel.IsUpdateAccountVisible)
        //        MenuHandlerModel.IsMenuHandlerVisible = true;
        //    else
        //        MenuHandlerModel.IsMenuHandlerVisible = false;
        //}

        //private bool BrowserButtonSizeChangedCommandCanExecute ( object arg )
        //{
        //    return true;
        //}

        //private bool DeleteButtonSizeChangedCommandCanExecute ( object arg )
        //{
        //    return true;
        //}

        //private void DeleteButtonSizeChangedCommandExecute ( object Sender )
        //{
        //    if (((Button)Sender).ActualHeight == 40)
        //        MenuHandlerModel.IsDeleteAccountVisible = false;
        //    else
        //        MenuHandlerModel.IsDeleteAccountVisible = true;

        //    ChangeMenuHandlerStatus();
        //}

        //private bool ExportButtonSizeChangedCommandCanExecute ( object arg )
        //{
        //    return true;
        //}

        //private void ExportButtonSizeChangedCommandExecute ( object Sender )
        //{
        //    if (_mainViewModel.ScreenResolution.Key <= 1024)
        //    {
        //        MenuHandlerModel.IsDeleteAccountVisible = true;
        //        MenuHandlerModel.IsBrowserAutomationVisible = true;
        //        MenuHandlerModel.IsInfoVisible = true;
        //    }

        //    if (((Button)Sender).ActualHeight == 40)
        //        MenuHandlerModel.IsExportAccountVisible = false;
        //    else
        //        MenuHandlerModel.IsExportAccountVisible = true;

        //    ChangeMenuHandlerStatus();
        //}

        //private bool UpdateButtonSizeChangedCommandCanExecute ( object arg )
        //{
        //    return true;
        //}

        //private void UpdateButtonSizeChangedCommandExecute ( object Sender )
        //{
        //    if (((DropDownButton)Sender).ActualHeight == 40)
        //        MenuHandlerModel.IsUpdateAccountVisible = false;
        //    else
        //        MenuHandlerModel.IsUpdateAccountVisible = true;

        //    ChangeMenuHandlerStatus();
        //}

        //private bool SelectButtonSizeChangedCommandCanExecute ( object arg )
        //{
        //    return true;
        //}

        //private void SelectButtonSizeChangedCommandExecute ( object Sender )
        //{
        //    if (((DropDownButton)Sender).ActualHeight == 40)
        //        MenuHandlerModel.IsSelectAccountsVisible = false;
        //    else
        //        MenuHandlerModel.IsSelectAccountsVisible = true;

        //    ChangeMenuHandlerStatus();
        //}

        //private bool ImportButtonSizeChangedCommandCanExecute ( object arg )
        //{
        //    return true;
        //}

        //private void ImportButtonSizeChangedCommandExecute ( object Sender )
        //{
        //    if (((Button)Sender).ActualHeight == 40)
        //        MenuHandlerModel.IsImportMultipleAccountsVisible = false;
        //    else
        //        MenuHandlerModel.IsImportMultipleAccountsVisible = true;

        //    ChangeMenuHandlerStatus();
        //}

        private void CustomSetting ( DominatorAccountModel account )
        {
            var viewModel = InstanceProvider.GetInstance<ISelectActivityViewModel>();
            viewModel.SelectedNetwork = account.AccountBaseModel.AccountNetwork.ToString();
            viewModel.SelectAccount = account;
            ModuleSetting.Instance.Show();
        }

        private void SelectedNetworkViewModel_ItemSelected ( object sender, SocialNetworks? e )
        {
            if (e.HasValue)
            {
                var spec = e == SocialNetworks.Social
                    ? DominatorAccountCountFactory.Instance.GetColumnSpecificationProvider()
                    : SocinatorInitialize.GetSocialLibrary(e.Value)
                        .GetNetworkCoreFactory()
                        .AccountCountFactory.GetColumnSpecificationProvider();
                lock (_syncLoadAccounts)
                {
                    VisibleColumns.Clear();
                    VisibleColumns.AddRange(spec.VisibleHeaders.Select(( name, colIndex ) => new GridViewColumn
                    {
                        DisplayMemberBinding = name == "Pinterest Account Type" ? new Binding("DisplayColumnValue11")
                            : e == SocialNetworks.Pinterest ? new Binding($"DisplayColumnValue{colIndex}")
                            : new Binding($"DisplayColumnValue{colIndex + 1}"),
                        Header = name,
                        Width = 130
                    }));
                }
            }
        }

        #region Export Accounts

        private void ExportExecute ()
        {
            var selectedAccounts = GetSelectedAccount();
            if (selectedAccounts.Count == 0)
            {
                Dialog.ShowDialog("LangKeyAlert".FromResourceDictionary(),
                    "LangKeyErrorSelectAtleastOneAccount".FromResourceDictionary());
                return;
            }

            var exportPath = FileUtilities.GetExportPath();

            if (string.IsNullOrEmpty(exportPath))
                return;

            var header = string.Empty;

            var FirstCountHeader = SocinatorInitialize.GetSocialLibrary(SocinatorInitialize.ActiveSocialNetwork)
                .GetNetworkCoreFactory().AccountCountFactory.HeaderColumn1Value;

            var SecondCountHeader = SocinatorInitialize.GetSocialLibrary(SocinatorInitialize.ActiveSocialNetwork)
                .GetNetworkCoreFactory().AccountCountFactory.HeaderColumn2Value;

            var ThirdCountHeader = SocinatorInitialize.GetSocialLibrary(SocinatorInitialize.ActiveSocialNetwork)
                .GetNetworkCoreFactory().AccountCountFactory.HeaderColumn3Value;

            var FourthCountHeader = SocinatorInitialize.GetSocialLibrary(SocinatorInitialize.ActiveSocialNetwork)
                .GetNetworkCoreFactory().AccountCountFactory.HeaderColumn4Value;

            if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
                header =
                    $"Account Group,AccountNetwork,Username,Password,Proxy Address,Proxy Port,Proxy Username,Proxy Password,Status,Cookies,Alternate Email (For YouTube/Gplus),Banned,Browser Cookies,Browser Automation Status,Proxy Group Name,{FirstCountHeader},SocialUser,AccountName(Nick name),Pinterest Account Type,MailUsername,MailPassword,MailHostName,MailPort,SslRequired";

            else if (!string.IsNullOrEmpty(FourthCountHeader))
                header =
                    $"Account Group,AccountNetwork,Username,Password,Proxy Address,Proxy Port,Proxy Username,Proxy Password,Status,Cookies,Alternate Email (For YouTube/Gplus),Banned,Browser Cookies,Browser Automation Status,Proxy Group Name,{FirstCountHeader},{SecondCountHeader},{ThirdCountHeader},{FourthCountHeader},SocialUser,AccountName(Nick name),Pinterest Account Type,MailUsername,MailPassword,MailHostName,MailPort,SslRequired";

            else if (!string.IsNullOrEmpty(ThirdCountHeader))
                header =
                    $"Account Group,AccountNetwork,Username,Password,Proxy Address,Proxy Port,Proxy Username,Proxy Password,Status,Cookies,Alternate Email (For YouTube/Gplus),Banned,Browser Cookies,Browser Automation Status,Proxy Group Name,{FirstCountHeader},{SecondCountHeader},{ThirdCountHeader},SocialUser,AccountName(Nick name),Pinterest Account Type,MailUsername,MailPassword,MailHostName,MailPort,SslRequired";

            else if (!string.IsNullOrEmpty(SecondCountHeader))
                header =
                    $"Account Group,AccountNetwork,Username,Password,Proxy Address,Proxy Port,Proxy Username,Proxy Password,Status,Cookies,Alternate Email (For YouTube/Gplus),Banned,Browser Cookies,Browser Automation Status,Proxy Group Name,{FirstCountHeader},{SecondCountHeader},SocialUser,AccountName(Nick name),Pinterest Account Type,MailUsername,MailPassword,MailHostName,MailPort,SslRequired";


            var filename =
                $"{exportPath}\\{SocinatorInitialize.ActiveSocialNetwork}_Accounts {ConstantVariable.DateasFileName}.csv";

            if (!File.Exists(filename))
                using (var streamWriter = new StreamWriter(filename, true))
                {
                    streamWriter.WriteLine(header);
                }

            selectedAccounts.ForEach(account =>
            {
                try
                {
                    var pinterestAccountType = string.IsNullOrEmpty(account.DisplayColumnValue11)
                        ? PinterestAccountType.NotAvailable.ToString()
                        : account.DisplayColumnValue11;

                    var mailcreds = !string.IsNullOrWhiteSpace(account.MailCredentials?.Username) ? $",{account.MailCredentials.Username},{account.MailCredentials.Password},{account.MailCredentials.Hostname},{account.MailCredentials.Port}" : "";
                    mailcreds = !string.IsNullOrEmpty(mailcreds) ? $"{mailcreds},{(account.IsUseSSL ? "1" : "0")}" : "";

                    var csvData =
                        account.AccountBaseModel.AccountGroup.Content + ","
                                                                      + account.AccountBaseModel.AccountNetwork + ","
                                                                      + account.AccountBaseModel.UserName + ","
                                                                      + account.AccountBaseModel.Password + ","
                                                                      + account.AccountBaseModel.AccountProxy.ProxyIp +
                                                                      ","
                                                                      + account.AccountBaseModel.AccountProxy
                                                                          .ProxyPort + ","
                                                                      + account.AccountBaseModel.AccountProxy
                                                                          .ProxyUsername + ","
                                                                      + account.AccountBaseModel.AccountProxy
                                                                          .ProxyPassword + ","
                                                                      + account.AccountBaseModel.Status + ","
                                                                      + JsonConvert
                                                                          .SerializeObject(account.CookieHelperList)
                                                                          .Replace(",", "<>") + ","
                                                                      + account.AccountBaseModel.AlternateEmail + ","
                                                                      + account.AccountBaseModel.Banned + ","
                                                                      + JsonConvert
                                                                          .SerializeObject(
                                                                              account.BrowserCookieHelperList)
                                                                          .Replace(",", "<>") + ","
                                                                      + account.IsRunProcessThroughBrowser.ToString() +
                                                                      ","
                                                                      + account.AccountBaseModel.AccountProxy
                                                                          .ProxyGroup + ","
                                                                      + account.DisplayColumnValue1
                                                                      + (string.IsNullOrEmpty(SecondCountHeader)
                                                                          ? ""
                                                                          : $",{account.DisplayColumnValue2}")
                                                                      + (string.IsNullOrEmpty(ThirdCountHeader)
                                                                          ? ""
                                                                          : $",{account.DisplayColumnValue3}")
                                                                      + (string.IsNullOrEmpty(FourthCountHeader)
                                                                          ? ""
                                                                          : $",{account.DisplayColumnValue4}")
                                                                      + $",{account.AccountBaseModel.ProfileId}"
                                                                      + $",{account.AccountBaseModel.AccountName}"
                                                                      + $",{pinterestAccountType}"
                                                                      + mailcreds;

                    using (var streamWriter = new StreamWriter(filename, true))
                    {
                        streamWriter.WriteLine(csvData);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            });
            Dialog.ShowDialog("LangKeyExportAccounts".FromResourceDictionary(),
                string.Format("LangKeyAccountsSuccessfullyExportedTo".FromResourceDictionary(), filename));
        }

        #endregion

        #region Help Methods

        private void InfoCommandExecute ()
        {
            IsOpenHelpControl = true;
        }

        #endregion


        private bool ActivateBrowserAutomationCanExecute ( object sender )
        {
            return true;
        }

        private void ActivateBrowserAutomationExecute ( object sender )
        {
            var networks = new HashSet<SocialNetworks> { SocialNetworks.TikTok,SocialNetworks.Twitter };
            if (sender.Equals("ActiveBrowser") &&
                LstDominatorAccountModel.Where(x => x.IsAccountManagerAccountSelected).ToList().Count > 0)
            {
                var result = Dialog.ShowCustomDialog("LangKeyActivatingBrowserAutomation".FromResourceDictionary(),
                    "LangKeyStartActivityByBrowserStopByHttp".FromResourceDictionary(),
                    "LangKeyContinue".FromResourceDictionary(), "LangKeyCancel".FromResourceDictionary());
                if (result == MessageDialogResult.Affirmative)
                {
                    var accountsToProcess = LstDominatorAccountModel.Where(x =>
                                            x.IsAccountManagerAccountSelected 
                                            && !x.IsRunProcessThroughBrowser 
                                            && !networks.Contains(x.AccountBaseModel.AccountNetwork));

                    if (LstDominatorAccountModel.Any(x =>
                        x.IsAccountManagerAccountSelected && networks.Contains(x.AccountBaseModel.AccountNetwork)))
                        Dialog.ShowDialog("LangKeyNote".FromResourceDictionary(),
                            string.Format("LangIGTikTokWontRunWithBrowserAutoTryWithHttp".FromResourceDictionary(),"Twitter"));

                    if (accountsToProcess.Count() == 0)
                    {
                        ToasterNotification.ShowInfomation(
                            $"{"LangKeyNoAccountsFoundToPerformAction".FromResourceDictionary()}");
                        return;
                    }

                    StopAllActivity(accountsToProcess.ToList(), true, true);

                    #region commented code

                    //StopProcess(accountsToProcess.ToList());

                    //Task.Factory.StartNew(() =>
                    //{
                    //    GlobusLogHelper.log.Info(Log.CustomMessage, SelectedNetworkViewModel.Selected, "", "LangKeyAccountActivities".FromResourceDictionary(), String.Format("LangKeyWaitForNSecs".FromResourceDictionary(), 10));

                    //    IsProgressActive = true;

                    //    Thread.Sleep(TimeSpan.FromSeconds(10));

                    //    accountsToProcess.ForEach(x =>
                    //    {
                    //        x.CancellationSource = new CancellationTokenSource();
                    //    });

                    //    IsProgressActive = false;
                    //}); 

                    #endregion
                }
            }
            else
            {
                try
                {
                    var model = sender as DominatorAccountModel;
                    if (model != null)
                    {
                        if (networks.Contains(model.AccountBaseModel.AccountNetwork))
                        {
                            Dialog.ShowDialog("LangKeyNote".FromResourceDictionary(),
                                    string.Format("LangIGTikTokWontRunWithBrowserAutoTryWithHttp".FromResourceDictionary(), "Twitter"));
                            return;
                        }
                        var result = Dialog.ShowCustomDialog(
                            "LangKeyActivatingBrowserAutomation".FromResourceDictionary(),
                            "LangKeyStartActivityByBrowserStopByHttp".FromResourceDictionary(),
                            "LangKeyContinue".FromResourceDictionary(), "LangKeyCancel".FromResourceDictionary());
                        if (result == MessageDialogResult.Affirmative)
                        {
                            var accountsToProcess = LstDominatorAccountModel.Where(x =>
                                x.AccountId == model.AccountId && !x.IsRunProcessThroughBrowser && !networks.Contains(x.AccountBaseModel.AccountNetwork));
                            if (accountsToProcess.Count() == 0)
                            {
                                ToasterNotification.ShowInfomation(
                                    $"{"LangKeyNoAccountsFoundToPerformAction".FromResourceDictionary()}");
                                return;
                            }

                            StopAllActivity(accountsToProcess.ToList(), true, true);
                        }
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, "",
                            "LangKeyBrowserAutomation".FromResourceDictionary(),
                            "LangKeyErrorSelectAtleastOneAccount".FromResourceDictionary());
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private bool DeActivateBrowserAutomationCommandCanExecute ( object sender )
        {
            return true;
        }

        private void DeActivateBrowserAutomationCommandExecute ( object sender )
        {
            var networks = new HashSet<SocialNetworks> { SocialNetworks.Quora, SocialNetworks.Instagram, SocialNetworks.TikTok, SocialNetworks.Facebook, SocialNetworks.YouTube };
            if (sender.Equals("DeActiveBrowser") &&
                LstDominatorAccountModel.Where(x => x.IsAccountManagerAccountSelected).ToList().Count > 0)
            {
                var result = Dialog.ShowCustomDialog("LangKeyDeactivatingBrowserAutomation".FromResourceDictionary(),
                    "LangKeyStartActivityByHttpStopByBrowser".FromResourceDictionary(),
                    "LangKeyContinue".FromResourceDictionary(), "LangKeyCancel".FromResourceDictionary());
                if (result == MessageDialogResult.Affirmative)
                {
                    var accountsToProcess = LstDominatorAccountModel.Where(x =>
                                            x.IsAccountManagerAccountSelected 
                                            && x.IsRunProcessThroughBrowser 
                                            && !networks.Contains(x.AccountBaseModel.AccountNetwork));

                    if (LstDominatorAccountModel.Any(x =>
                        x.IsAccountManagerAccountSelected && networks.Contains(x.AccountBaseModel.AccountNetwork)))
                        Dialog.ShowDialog("LangKeyNote".FromResourceDictionary(),
                            string.Format("LangYtFbWontRunWithHttpTryWithBrowserAuto".FromResourceDictionary(), "Instagram, Facebook, Quora and YouTube"));
                    if (accountsToProcess.ToList().Count() == 0)
                    {
                        ToasterNotification.ShowInfomation(
                            $"{"LangKeyNoAccountsFoundToPerformAction".FromResourceDictionary()}");
                        return;
                    }

                    StopAllActivity(accountsToProcess.ToList(), true, activateHttp: true);
                }
            }
            else
            {
                try
                {
                    var model = sender as DominatorAccountModel;
                    if (model != null)
                    {
                        if(networks.Contains(model.AccountBaseModel.AccountNetwork))
                        {
                            Dialog.ShowDialog("LangKeyNote".FromResourceDictionary(),
                                       string.Format("LangYtFbWontRunWithHttpTryWithBrowserAuto".FromResourceDictionary(), "Instagram, Facebook, Quora and YouTube"));
                            return;
                        }
                        var result = Dialog.ShowCustomDialog(
                            "LangKeyDeactivatingBrowserAutomation".FromResourceDictionary(),
                            "LangKeyStartActivityByHttpStopByBrowser".FromResourceDictionary(),
                            "LangKeyContinue".FromResourceDictionary(), "LangKeyCancel".FromResourceDictionary());
                        if (result == MessageDialogResult.Affirmative)
                        {

                            var accountsToProcess = LstDominatorAccountModel.Where(x => 
                                                    x.AccountId == model.AccountId 
                                                    && x.IsRunProcessThroughBrowser 
                                                    && !networks.Contains(x.AccountBaseModel.AccountNetwork));
                            if (accountsToProcess.Count() == 0)
                            {
                                ToasterNotification.ShowInfomation(
                                    $"{"LangKeyNoAccountsFoundToPerformAction".FromResourceDictionary()}");
                                return;
                            }
                            StopAllActivity(accountsToProcess.ToList(), true, activateHttp: true);
                        }
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, "",
                            "LangKeyBrowserAutomation".FromResourceDictionary(),
                            "LangKeyErrorSelectAtleastOneAccount".FromResourceDictionary());
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void UpdateGroupDetailsExecute ()
        {
            lock (_syncLoadAccounts)
            {
                Groups.Clear();
                Groups.AddRange(LstDominatorAccountModel
                    .Where(x => SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social ||
                                x.AccountBaseModel.AccountNetwork == SocinatorInitialize.ActiveSocialNetwork)
                    .GroupBy(a => a.AccountBaseModel.AccountGroup).Select(a => a.Key).ToList());
            }
        }

        private void UpdateUserCradExecute ( object sender )
        {
            var lstcred = FileUtilities.FileBrowseAndReader();
            if (lstcred.Count != 0)
            {
                ToasterNotification.ShowInfomation("LangKeyCredentialsImportedStartingUpdate".FromResourceDictionary());
                var isAnyAccountUpdated = false;
                foreach (var cred in lstcred)
                {
                    var data = cred.Split('\t');
                    if (data.ToList().Any(x => string.IsNullOrEmpty(x)))
                        continue;
                    if (data.Length < 7)
                        continue;

                    var accountToUpdate = LstDominatorAccountModel.FirstOrDefault(x =>
                        x.AccountBaseModel.AccountNetwork.ToString() == data[0] &&
                        x.AccountBaseModel.UserName == data[1]);
                    if (accountToUpdate != null)
                    {
                        accountToUpdate.MailCredentials.Username = data[2];
                        accountToUpdate.MailCredentials.Password = data[3];
                        accountToUpdate.MailCredentials.Hostname = data[4];
                        accountToUpdate.MailCredentials.Port = int.Parse(data[5]);
                        accountToUpdate.IsUseSSL = bool.Parse(data[6]);
                        accountToUpdate.IsAutoVerifyByEmail = true;
                        isAnyAccountUpdated = true;
                    }
                }

                if (isAnyAccountUpdated)
                {
                    _accountsFileManager.UpdateAccounts(LstDominatorAccountModel);
                    ToasterNotification.ShowSuccess("LangKeyUpdatedCredentials".FromResourceDictionary());
                }
                else
                {
                    ToasterNotification.ShowInfomation("LangKeyNoAccountToUpdatecCredentialsOrFormatWrong"
                        .FromResourceDictionary());
                }
            }
        }
        public async Task<bool> IsProxyWorkingAsync(string proxyHost, string proxyPort = "", string username = "", string password = "",
            string testUrl = "https://api.ipify.org")
        {
            try
            {
                if (string.IsNullOrEmpty(proxyHost) || string.IsNullOrEmpty(proxyPort))
                    return true;
                var proxy = new WebProxy($"{proxyHost}:{proxyPort}", true);
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    proxy.Credentials = new NetworkCredential(username, password);
                }
                var httpClientHandler = new HttpClientHandler
                {
                    Proxy = proxy,
                    UseProxy = true
                };

                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(10); // avoid hanging too long
                    var response = await httpClient.GetAsync(testUrl);
                    if(response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        testUrl = "https://ifconfig.me/";
                        response = await httpClient.GetAsync(testUrl);
                        return response.IsSuccessStatusCode;
                    }
                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false; // proxy failed
            }
        }
        public void AccountBrowserLogin(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                Task.Run(async () =>
                {
                    try
                    {
                        var ProxyWorking = await IsProxyWorkingAsync(dominatorAccountModel?.AccountBaseModel?.AccountProxy?.ProxyIp,
                                dominatorAccountModel?.AccountBaseModel?.AccountProxy?.ProxyPort, dominatorAccountModel?.AccountBaseModel?.AccountProxy?.ProxyUsername,
                                dominatorAccountModel?.AccountBaseModel?.AccountProxy?.ProxyPassword);
                        if (!ProxyWorking)
                        {
                            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                            return;
                        }
                        var accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
                        var key = $"{dominatorAccountModel.AccountId}_BrowserLogin";
                        var networkKey = dominatorAccountModel.AccountBaseModel.AccountNetwork.ToString();
                        if (AsyncLoginNetworks.Contains(dominatorAccountModel.AccountBaseModel.AccountNetwork))
                        {
                            var browserManagerAsync = accountScopeFactory[key].Resolve<IBrowserManagerAsync>(networkKey);
                            await browserManagerAsync.BrowserLoginAsync(dominatorAccountModel, CancellationToken.None, LoginType.BrowserLogin);
                        }
                        else
                        {
                            var browserManager = accountScopeFactory[key].Resolve<IBrowserManager>(networkKey);
                            browserManager.BrowserLogin(dominatorAccountModel, CancellationToken.None, LoginType.BrowserLogin);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Browser login failed: " + ex);
                    }
                });
            }
            catch { }
        }

        public async void AccountStatusChecker(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.TryingToLogin)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName,
                        "LangKeyLogin".FromResourceDictionary(),
                        "LangKeyAlreadyCheckingLoginSoWait".FromResourceDictionary());
                    return;
                }

                if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.UpdatingDetails)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName,
                        "LangKeyLogin".FromResourceDictionary(),
                        "LangKeyAlreadyUpdatingDetailsSoWait".FromResourceDictionary());
                    return;
                }

                await Task.Run(async () =>
                {
                    try
                    {
                        var accountUpdateFactory = SocinatorInitialize
                        .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory().AccountUpdateFactory;

                        var accountUpdateFactoryAsync = (IAccountUpdateFactoryAsync)accountUpdateFactory;
                        var lastStatus = dominatorAccountModel.AccountBaseModel.Status;
                        dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                        var ProxyWorking = await IsProxyWorkingAsync(dominatorAccountModel?.AccountBaseModel?.AccountProxy?.ProxyIp,
                            dominatorAccountModel?.AccountBaseModel?.AccountProxy?.ProxyPort, dominatorAccountModel?.AccountBaseModel?.AccountProxy?.ProxyUsername,
                            dominatorAccountModel?.AccountBaseModel?.AccountProxy?.ProxyPassword);
                        if (!ProxyWorking)
                        {
                            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                            return;
                        }
                        await accountUpdateFactoryAsync.CheckStatusAsync(dominatorAccountModel, dominatorAccountModel.Token);
                        if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.Success)
                        {
                            var runningActivityManager = InstanceProvider.GetInstance<IRunningActivityManager>();
                            runningActivityManager.ScheduleIfAccountGotSucess(dominatorAccountModel);
                        }
                        else if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.TryingToLogin)
                        {
                            dominatorAccountModel.AccountBaseModel.Status = lastStatus;
                        }
                    }
                    catch(Exception ex)
                    {
                        ex.DebugLog();
                    }
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AccountUpdate ( DominatorAccountModel dominatorAccountModel )
        {
            try
            {
                ThreadFactory.Instance.Start(() =>
                {
                    var accountUpdateFactory = SocinatorInitialize
                        .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory().AccountUpdateFactory;
                    accountUpdateFactory.UpdateDetails(dominatorAccountModel);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void EditProfile ( DominatorAccountModel dominatorAccountModel )
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var profileFactory = SocinatorInitialize
                        .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory().ProfileFactory;
                    profileFactory.EditProfile(dominatorAccountModel);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void RemovePhoneVerification ( DominatorAccountModel dominatorAccountModel )
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var profileFactory = SocinatorInitialize
                        .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory().ProfileFactory;
                    profileFactory.RemovePhoneVerification(dominatorAccountModel);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void UpdateProxyStatus ( DominatorAccountBaseModel objDominatorAccountBaseModel )
        {
            try
            {
                var proxyToBeUpdated =
                    _proxyFileManager.GetProxyById(objDominatorAccountBaseModel.AccountProxy.ProxyId);
                if ( proxyToBeUpdated != null )
                {
                    proxyToBeUpdated.Status = "Working";
                    _proxyFileManager.EditProxy(proxyToBeUpdated);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ProfileDetails ( DominatorAccountModel acount )
        {
            try
            {
                AccountManager.GetSingletonAccountManager(string.Empty, acount,
                    acount.AccountBaseModel.AccountNetwork);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void CopyAccountId ( DominatorAccountModel account )
        {
            if (!string.IsNullOrEmpty(account.AccountId))
            {
                Clipboard.SetText(account.AccountId);
                ToasterNotification.ShowSuccess("LangKeyAccountIdCopied".FromResourceDictionary());
            }
        }

        public void GoToCaptchaService ( DominatorAccountModel dominatorAccountModel )
        {
            try
            {
                if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.TryingToLogin)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName,
                        "LangKeyLogin".FromResourceDictionary(),
                        "LangKeyAlreadyCheckingLoginSoWait".FromResourceDictionary());
                    return;
                }

                if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.UpdatingDetails)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.AccountBaseModel.UserName,
                        "LangKeyLogin".FromResourceDictionary(),
                        "LangKeyAlreadyUpdatingDetailsSoWait".FromResourceDictionary());
                    return;
                }

                ThreadFactory.Instance.Start(() =>
                {
                    var accountUpdateFactory = SocinatorInitialize
                        .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory().AccountUpdateFactory;

                    var lastStatus = dominatorAccountModel.AccountBaseModel.Status;
                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TryingToLogin;

                    accountUpdateFactory.SolveCaptchaManually(dominatorAccountModel);
                    if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.Success)
                    {
                        var runningActivityManager = InstanceProvider.GetInstance<IRunningActivityManager>();
                        runningActivityManager.ScheduleIfAccountGotSucess(dominatorAccountModel);
                    }
                    else if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.TryingToLogin)
                    {
                        dominatorAccountModel.AccountBaseModel.Status = lastStatus;
                    }
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void GotoTools ( DominatorAccountModel account )
        {
            if (account != null && (account.AccountBaseModel.Status == AccountStatus.Success ||
                                    account.AccountBaseModel.Status == AccountStatus.UpdatingDetails))
                TabSwitcher.ChangeTabWithNetwork(3, account.AccountBaseModel.AccountNetwork,
                    account.AccountBaseModel.UserName);
        }

        #region Property

        private string _contactSupportLink = ConstantVariable.ContactSupportLink;

        public string ContactSupportLink
        {
            get => _contactSupportLink;
            set => SetProperty(ref _contactSupportLink, value);
        }

        private string _knowledgeBaseLink = "https://help.socinator.com/support/solutions/folders/42000095344";

        public string KnowledgeBaseLink
        {
            get => _knowledgeBaseLink;
            set => SetProperty(ref _knowledgeBaseLink, value);
        }


        private bool _isOpenHelpControl;

        public bool IsOpenHelpControl
        {
            get => _isOpenHelpControl;
            set
            {
                if (_isOpenHelpControl == value)
                    return;
                SetProperty(ref _isOpenHelpControl, value);
            }
        }

        #endregion

        #region Command 

        public ICommand AddSingleAccountCommand { get; }
        public ICommand InfoCommand { get; }
        public ICommand LoadMultipleAccountsCommand { get; }
        public ICommand DeleteAccountsCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand SelectAccountCommand { get; }
        public ICommand SelectAccountByStatusCommand { get; }
        public ICommand SelectAccountByGroupCommand { get; }
        public ICommand SingleAccountDeleteCommand { get; }
        public ICommand UpdateAccountDetailsCommand { get; }
        public ICommand UpdateGroupCommand { get; }
        public ICommand UpdateUserCradCommand { get; }
        public ICommand ProfileDetailsCommand { get; }
        public ICommand DeleteAccountCommand { get; }
        public ICommand BrowserLoginCommand { get; }
        public ICommand GoToCaptchaServiceCommand { get; }
        public ICommand GotoToolsCommand { get; }
        public ICommand CheckLoginCommand { get; }
        public ICommand UpdateFriendshipCommand { get; }
        public ICommand EditNetworkProfileCommand { get; }
        public ICommand CopyAccountIdCommand { get; }
        public ICommand SettingWizardCommand { get; }
        public ICommand ActivateBrowserAutomationCommand { get; }
        public ICommand DeActivateBrowserAutomationCommand { get; }
        public ICommand ImportButtonSizeChangedCommand { get; }
        public ICommand SelectButtonSizeChangedCommand { get; }
        public ICommand UpdateButtonSizeChangedCommand { get; }
        public ICommand ExportButtonSizeChangedCommand { get; }
        public ICommand DeleteButtonSizeChangedCommand { get; }
        public ICommand BrowserButtonSizeChangedCommand { get; }
        public ICommand InfoButtonSizeChnagedCommand { get; }
        public ICommand SwitchToBusinessAccountCommand { get; }
        public ICommand SwitchToSingleBusinessAccountCommand { get; }
        public ICommand SwitchToSingleNormalAccountCommand { get; }
        public ICommand DownloadAccountFormats { get; }

        #endregion

        #region Add accounts

        private void AddSingleAccountExecute ()
        {
            var objDominatorAccountBaseModel = new DominatorAccountBaseModel();
            Application.Current.MainWindow.Opacity = 0.2;
            var objAddUpdateAccountControl = new AddUpdateAccountControl(objDominatorAccountBaseModel,
                "LangKeyAddAccount".FromResourceDictionary(), "LangKeySave".FromResourceDictionary(), false,
                SocinatorInitialize.ActiveSocialNetwork);
            objAddUpdateAccountControl.Owner = Application.Current.MainWindow;
            objDominatorAccountBaseModel.AccountNetwork = (SocialNetworks)Enum.Parse(typeof(SocialNetworks),
                objAddUpdateAccountControl.ComboBoxSocialNetworks.Text);

            var isIgOrTik = false; /*objDominatorAccountBaseModel.AccountNetwork == SocialNetworks.Instagram ||*/
                            //objDominatorAccountBaseModel.AccountNetwork == SocialNetworks.TikTok;

            var visibility = /*isIgOrTik ? Visibility.Collapsed : */Visibility.Visible;
            objAddUpdateAccountControl.RunThroughBrowserAutomation.Visibility = objDominatorAccountBaseModel.AccountNetwork == SocialNetworks.Instagram
                || objDominatorAccountBaseModel.AccountNetwork == SocialNetworks.TikTok ? Visibility.Collapsed : visibility;
            objAddUpdateAccountControl.CopyJsonCookieGrid.Visibility = visibility;
            var dictNetLasNum = new Dictionary<SocialNetworks, int>();
            var nickName = DefaultAccountNameFromModel(
                LstDominatorAccountModel.BySocialNetwork(objDominatorAccountBaseModel.AccountNetwork),
                ref dictNetLasNum, objDominatorAccountBaseModel.AccountNetwork);
            objDominatorAccountBaseModel.AccountName = nickName;
            objAddUpdateAccountControl.Closing += (sender, eventArgs) => Application.Current.MainWindow.Opacity = 1;
            objAddUpdateAccountControl.btnSave.Click += ( senders, events ) =>
            {
                try
                {
                    //if (!string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyIp) &&
                    //    !_proxyValidationService.IsValidProxy(objDominatorAccountBaseModel.AccountProxy.ProxyIp, objDominatorAccountBaseModel.AccountProxy.ProxyPort))
                    //{
                    //    Dialog.ShowDialog("Proxy Warning", $"Invalid Proxy IP format :- \"{objDominatorAccountBaseModel.AccountProxy.ProxyIp}\". ");
                    //    return;
                    //}

                    #region OLD Logic.
                    //if (string.IsNullOrEmpty(objDominatorAccountBaseModel.UserName) ||
                    //    string.IsNullOrEmpty(objDominatorAccountBaseModel.Password)) return;

                    //if (!string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyIp) &&
                    //    string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyPort)
                    //    || string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyIp) &&
                    //    !string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyPort))
                    //{
                    //    var filledOne =
                    //        (!string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyIp)
                    //            ? "LangKeyProxyIp"
                    //            : "LangKeyProxyPort").FromResourceDictionary();
                    //    var emptyOne =
                    //        (filledOne == "LangKeyProxyIp".FromResourceDictionary()
                    //            ? "LangKeyProxyPort"
                    //            : "LangKeyProxyIp").FromResourceDictionary();
                    //    ToasterNotification.ShowError(string.Format(
                    //        "LangKeyOneFieldCantBeEmptyIfAnotherHasValue".FromResourceDictionary(), emptyOne,
                    //        filledOne));
                    //    return;
                    //}

                    //if (!string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyUsername) &&
                    //    string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyPassword)
                    //    || string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyUsername) &&
                    //    !string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyPassword))
                    //{
                    //    var filledOne =
                    //        (!string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyUsername)
                    //            ? "LangKeyProxyUsername"
                    //            : "LangKeyProxyPassword").FromResourceDictionary();
                    //    var emptyOne =
                    //        (filledOne == "LangKeyProxyUsername".FromResourceDictionary()
                    //            ? "LangKeyProxyPassword"
                    //            : "LangKeyProxyUsername").FromResourceDictionary();
                    //    ToasterNotification.ShowError(string.Format(
                    //        "LangKeyOneFieldCantBeEmptyIfAnotherHasValue".FromResourceDictionary(), emptyOne,
                    //        filledOne));
                    //    return;
                    //}


                    //objDominatorAccountBaseModel.Status = AccountStatus.NotChecked;
                    //dialogWindow.Close();

                    //if (LstDominatorAccountModel.Count + 1 >=
                    //    SocinatorInitialize.MaximumAccountCount)
                    //    GlobusLogHelper.log.Info("LangKeyAddedMaxAccountAsPerYourPlan".FromResourceDictionary());

                    //var httpCookies = isIgOrTik ? "" : objAddUpdateAccountControl.JsonCookies;
                    //var browserCookies = isIgOrTik ? "" : objAddUpdateAccountControl.JsonBrowserCookies;
                    //var browserActivated = isIgOrTik
                    //    ? false
                    //    : objAddUpdateAccountControl.RunThroughBrowserAutomation.IsChecked ?? false;

                    //var pinterestAccountType = objDominatorAccountBaseModel.AccountNetwork == SocialNetworks.Pinterest
                    //    ? PinterestAccountType.Inactive.ToString()
                    //    : PinterestAccountType.NotAvailable.ToString();

                    //ThreadFactory.Instance.Start(() =>
                    //{
                    //    AddAccount(objDominatorAccountBaseModel, httpCookies, act =>
                    //    {
                    //        var th = new Thread(() => act()) { IsBackground = true };
                    //        th.Start();
                    //        return () => th.Abort();
                    //    }, browserCookies, browserActivated, pinterestAccountType);
                    //});
                    #endregion

                    AddSingleAccountInThread(objDominatorAccountBaseModel, isIgOrTik, objAddUpdateAccountControl.RunThroughBrowserAutomation.IsChecked ?? false, objAddUpdateAccountControl,
                    isIgOrTik ? "" : objAddUpdateAccountControl.JsonCookies, isIgOrTik ? "" : objAddUpdateAccountControl.JsonBrowserCookies);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                
            };

            objAddUpdateAccountControl.ComboBoxSocialNetworks.SelectionChanged += ( senders, events ) =>
            {
                try
                {
                    dictNetLasNum = new Dictionary<SocialNetworks, int>();
                    nickName = DefaultAccountNameFromModel(
                        LstDominatorAccountModel.BySocialNetwork(objDominatorAccountBaseModel.AccountNetwork),
                        ref dictNetLasNum, objDominatorAccountBaseModel.AccountNetwork);
                    objDominatorAccountBaseModel.AccountName = nickName;
                    isIgOrTik = objDominatorAccountBaseModel.AccountNetwork == SocialNetworks.Instagram ||
                                objDominatorAccountBaseModel.AccountNetwork == SocialNetworks.TikTok;
                    visibility = isIgOrTik ? Visibility.Collapsed : Visibility.Visible;
                    objAddUpdateAccountControl.RunThroughBrowserAutomation.Visibility = visibility;
                    objAddUpdateAccountControl.CopyJsonCookieGrid.Visibility = visibility;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            };

            objAddUpdateAccountControl.btnCancel.Click += ( senders, events ) => objAddUpdateAccountControl.Close();

            objAddUpdateAccountControl.ShowDialog();
        }

        public void AddSingleAccountInThread(DominatorAccountBaseModel objDominatorAccountBaseModel,bool isIgOrTik=false,bool RunThroughBrowserAutomation = false, Window dialogWindow = null, string JsonCookies="",string BrowserCookies="")
        {
            try
            {
                //if (!string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyIp) &&
                //    !_proxyValidationService.IsValidProxy(objDominatorAccountBaseModel.AccountProxy.ProxyIp, objDominatorAccountBaseModel.AccountProxy.ProxyPort))
                //{
                //    Dialog.ShowDialog("Proxy Warning", $"Invalid Proxy IP format :- \"{objDominatorAccountBaseModel.AccountProxy.ProxyIp}\". ");
                //    return;
                //}


                if (string.IsNullOrEmpty(objDominatorAccountBaseModel.UserName) ||
                    string.IsNullOrEmpty(objDominatorAccountBaseModel.Password)) return;

                if (!string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyIp) &&
                    string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyPort)
                    || string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyIp) &&
                    !string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyPort))
                {
                    var filledOne =
                        (!string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyIp)
                            ? "LangKeyProxyIp"
                            : "LangKeyProxyPort").FromResourceDictionary();
                    var emptyOne =
                        (filledOne == "LangKeyProxyIp".FromResourceDictionary()
                            ? "LangKeyProxyPort"
                            : "LangKeyProxyIp").FromResourceDictionary();
                    ToasterNotification.ShowError(string.Format(
                        "LangKeyOneFieldCantBeEmptyIfAnotherHasValue".FromResourceDictionary(), emptyOne,
                        filledOne));
                    return;
                }

                if (!string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyUsername) &&
                    string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyPassword)
                    || string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyUsername) &&
                    !string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyPassword))
                {
                    var filledOne =
                        (!string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyUsername)
                            ? "LangKeyProxyUsername"
                            : "LangKeyProxyPassword").FromResourceDictionary();
                    var emptyOne =
                        (filledOne == "LangKeyProxyUsername".FromResourceDictionary()
                            ? "LangKeyProxyPassword"
                            : "LangKeyProxyUsername").FromResourceDictionary();
                    ToasterNotification.ShowError(string.Format(
                        "LangKeyOneFieldCantBeEmptyIfAnotherHasValue".FromResourceDictionary(), emptyOne,
                        filledOne));
                    return;
                }


                objDominatorAccountBaseModel.Status = AccountStatus.NotChecked;
                if(dialogWindow!=null)
                    dialogWindow.Close();
                if (LstDominatorAccountModel.Count + 1 >=
                    SocinatorInitialize.MaximumAccountCount)
                    GlobusLogHelper.log.Info("LangKeyAddedMaxAccountAsPerYourPlan".FromResourceDictionary());

                var httpCookies = isIgOrTik ? "" : JsonCookies;
                var browserCookies = isIgOrTik ? "" : BrowserCookies;
                var browserActivated = isIgOrTik
                    ? false
                    : RunThroughBrowserAutomation;

                var pinterestAccountType = objDominatorAccountBaseModel.AccountNetwork == SocialNetworks.Pinterest
                    ? PinterestAccountType.Inactive.ToString()
                    : PinterestAccountType.NotAvailable.ToString();

                ThreadFactory.Instance.Start(() =>
                {
                    AddAccount(objDominatorAccountBaseModel, httpCookies, act =>
                    {
                        var th = new Thread(() => act()) { IsBackground = true };
                        th.Start();
                        return () => th.Abort();
                    }, browserCookies, browserActivated, pinterestAccountType);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     LoadMultipleAccountsExecute is used to load multiple accounts at a time
        ///     GroupName:Username:Password:ProxyIp:ProxyPort:ProxyUsername:ProxyPassword
        ///     GroupName:Username:Password:ProxyIp:ProxyPort
        ///     GroupName:Username:Password
        ///     Can load , instead of :
        ///     If any values are null, we can use NA
        /// </summary>
        /// <param name="sender"></param>
        private void LoadMultipleAccountsExecute ( Func<SocialNetworks, bool> isNetworkAvailable, Action<string> warn )
        {
            //Read the accounts from text or csv files
            try
            {
                var loadedAccountlist = FileUtilities.FileBrowseAndReader();

                //if loaded text or csv contains no accounts then return
                if (loadedAccountlist == null || loadedAccountlist.Count == 0) return;
                var split = loadedAccountlist[0].Trim().Split('\t');
                var haveNickName = loadedAccountlist[0].StartsWith("Account Group\tAccountNetwork")
                    ? loadedAccountlist[0].Contains("AccountName(Nick name)")
                    : loadedAccountlist[0].Trim().Split('\t').Count() == 16;

                var dictNetLasNum = new Dictionary<SocialNetworks, int>();

                #region Add to bin files which are valid accounts

                ////add the account to DominatorAccountModel list and bin file
                _allAccountsQueued = false;


                if (loadedAccountlist.Count + LstDominatorAccountModel.Count >
                    SocinatorInitialize.MaximumAccountCount)
                    GlobusLogHelper.log.Info("LangKeyAddedMaxAccountAsPerYourPlan".FromResourceDictionary());

                try
                {
                    new Thread(() =>
                    {
                        while (!_allAccountsQueued)
                        {
                            Thread.Sleep(50);
                            while (!_pendingActions.IsEmpty)
                            {
                                Action act;
                                _pendingActions = _pendingActions.Dequeue(out act);
                                act();
                            }
                        }
                    })
                    { IsBackground = true }.Start();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                Thread.Sleep(50);
                //Iterate the all accounts one by one
                foreach (var singleAccount in loadedAccountlist)
                    try
                    {
                        #region Getting Account Details from loadedAccountlist

                        // var finalAccount = singleAccount.Replace(",", ":").Replace("<NA>", "");
                        var finalAccount = singleAccount.Replace("<NA>", "\t");
                        var splitAccount = Regex.Split(finalAccount.TrimEnd(), "\t");
                        //var splitAccount = Regex.Split(finalAccount, ":");
                        if (splitAccount.Length <= 1) continue;

                        //assign the username, password and groupname
                        var groupname = splitAccount[0];

                        var socialNetwork = splitAccount[1];
                        if (socialNetwork == "AccountNetwork" || socialNetwork == SocialNetworks.Social.ToString())
                            continue;
                        var username = splitAccount[2];
                        var password = splitAccount[3];

                        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                            continue;

                        var proxyaddress = string.Empty;
                        var proxyport = string.Empty;
                        var proxyusername = string.Empty;
                        var proxypassword = string.Empty;
                        var proxyGroup = string.Empty;
                        var nickName = string.Empty;
                        var status = AccountStatus.NotChecked.ToString();
                        var cookies = string.Empty;
                        var browserCookies = string.Empty;
                        var alternetEmail = string.Empty;
                        var banned = string.Empty;
                        var isBrowserAutomationActive = string.Empty;
                        var pinterestAccountType = PinterestAccountType.NotAvailable.ToString();
                        var mailCreds = new MailCredentials();
                        int useSsl = 0;

                        switch (splitAccount.Length)
                        {
                            case 5:
                                alternetEmail = splitAccount[4];
                                break;
                            case 6:
                                proxyaddress = splitAccount[4];
                                proxyport = splitAccount[5];
                                break;
                            case 7:
                                proxyaddress = splitAccount[4];
                                proxyport = splitAccount[5];
                                alternetEmail = splitAccount[6];
                                break;
                            case 8:
                                proxyaddress = splitAccount[4];
                                proxyport = splitAccount[5];
                                proxyusername = splitAccount[6];
                                proxypassword = splitAccount[7];
                                break;
                            case 9:
                                proxyaddress = splitAccount[4];
                                proxyport = splitAccount[5];
                                proxyusername = splitAccount[6];
                                proxypassword = splitAccount[7];
                                alternetEmail = splitAccount[8];
                                break;
                            case 10:
                                proxyaddress = splitAccount[4];
                                proxyport = splitAccount[5];
                                proxyusername = splitAccount[6];
                                proxypassword = splitAccount[7];
                                status = splitAccount[8];
                                cookies = splitAccount[9].Replace("<>", ",");
                                break;
                            case 11:
                                proxyaddress = splitAccount[4];
                                proxyport = splitAccount[5];
                                proxyusername = splitAccount[6];
                                proxypassword = splitAccount[7];
                                status = splitAccount[8];
                                cookies = splitAccount[9].Replace("<>", ",");
                                alternetEmail = splitAccount[10];
                                break;
                            case 12:
                                proxyaddress = splitAccount[4];
                                proxyport = splitAccount[5];
                                proxyusername = splitAccount[6];
                                proxypassword = splitAccount[7];
                                status = splitAccount[8];
                                cookies = splitAccount[9].Replace("<>", ",");
                                alternetEmail = splitAccount[10];
                                banned = splitAccount[11];
                                break;
                            case 14:
                                proxyaddress = splitAccount[4];
                                proxyport = splitAccount[5];
                                proxyusername = splitAccount[6];
                                proxypassword = splitAccount[7];
                                status = splitAccount[8];
                                cookies = splitAccount[9].Replace("<>", ",");
                                alternetEmail = splitAccount[10];
                                banned = splitAccount[11];
                                browserCookies = splitAccount[12].Replace("<>", ",");
                                isBrowserAutomationActive = splitAccount[13];
                                break;
                            case 15:
                            case 16:
                            case 17:
                            case 18:
                            case 19:
                            case 20:
                            case 21:
                            case 22:
                                proxyaddress = splitAccount[4];
                                proxyport = splitAccount[5];
                                proxyusername = splitAccount[6];
                                proxypassword = splitAccount[7];
                                status = splitAccount[8];
                                cookies = splitAccount[9].Replace("<>", ",");
                                alternetEmail = splitAccount[10];
                                banned = splitAccount[11];
                                browserCookies = splitAccount[12].Replace("<>", ",");
                                isBrowserAutomationActive = splitAccount[13];
                                proxyGroup = splitAccount[14];
                                nickName = haveNickName && splitAccount.Length >= 15
                                    ? splitAccount[splitAccount.Length - 2] ?? ""
                                    : "";
                                pinterestAccountType = splitAccount.Length >= 16
                                    ? splitAccount[splitAccount.Length - 1]
                                    : pinterestAccountType;
                                break;
                            case 24:
                            case 25:
                            case 26:
                            case 27:
                                proxyaddress = splitAccount[4];
                                proxyport = splitAccount[5];
                                proxyusername = splitAccount[6];
                                proxypassword = splitAccount[7];
                                status = splitAccount[8];
                                cookies = splitAccount[9].Replace("<>", ",");
                                alternetEmail = splitAccount[10];
                                banned = splitAccount[11];
                                browserCookies = splitAccount[12].Replace("<>", ",");
                                isBrowserAutomationActive = splitAccount[13];
                                proxyGroup = splitAccount[14];
                                nickName = haveNickName ? splitAccount[splitAccount.Length - 7] ?? "" : "";
                                pinterestAccountType = splitAccount[splitAccount.Length - 6];
                                mailCreds = SplitedAccMailCreds(splitAccount);
                                useSsl = splitAccount[splitAccount.Length - 1] == "1" || splitAccount[splitAccount.Length - 1] == "0" ? Convert.ToInt32(splitAccount[splitAccount.Length - 1]) : 0;
                                break;
                        }

                        if (string.IsNullOrWhiteSpace(nickName))
                            nickName = DefaultAccountNameFromModel(LstDominatorAccountModel.GetCopySync(),
                                ref dictNetLasNum, (SocialNetworks)Enum.Parse(typeof(SocialNetworks), socialNetwork));

                        if (string.IsNullOrWhiteSpace(status))
                            status = "NotChecked";

                        if (splitAccount.Length > 4)
                            if (string.IsNullOrEmpty(proxyaddress) || string.IsNullOrEmpty(proxyport))
                                proxyaddress = proxyport = proxyusername = proxypassword = proxyGroup = string.Empty;
                        //valid the proxy ip and port
                        //else if (!Proxy.IsValidProxyIp(proxyaddress) || !Proxy.IsValidProxyPort(proxyport))
                        //{
                        //    GlobusLogHelper.log.Info(Log.ImportFailed, socialNetwork, username, "Proxy address or Proxy port");
                        //    continue;

                        //}

                        #endregion

                        #region Creating new Account with Account Details

                        var objDominatorAccountBaseModel = new DominatorAccountBaseModel
                        {
                            AccountGroup =
                            {
                                Content = groupname ?? ConstantVariable.UnGrouped()
                            },
                            UserName = username,
                            Password = password,
                            AccountProxy =
                            {
                                ProxyGroup = proxyGroup,
                                ProxyIp = proxyaddress,
                                ProxyPort = proxyport,
                                ProxyUsername = proxyusername,
                                ProxyPassword = proxypassword
                            },
                            AccountNetwork = (SocialNetworks)Enum.Parse(typeof(SocialNetworks), socialNetwork),
                            Status = string.IsNullOrWhiteSpace(status)
                                ? AccountStatus.NotChecked
                                : (AccountStatus)Enum.Parse(typeof(AccountStatus), status),
                            AlternateEmail = alternetEmail,
                            Banned = banned,
                            AccountName = nickName,
                        };

                        #endregion

                        var browserAutomationStatus = false;

                        bool.TryParse(isBrowserAutomationActive, out browserAutomationStatus);

                        if (isNetworkAvailable(objDominatorAccountBaseModel.AccountNetwork))
                        {
                            if (SocinatorInitialize.ActiveSocialNetwork ==
                                objDominatorAccountBaseModel.AccountNetwork ||
                                SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
                                _pendingActions = _pendingActions.Enqueue(() => AddAccount(objDominatorAccountBaseModel,
                                    cookies,
                                    action =>
                                    {
                                        _pendingActions = _pendingActions.Enqueue(action);
                                        return () =>
                                        {
                                            var oldqueue = _pendingActions;
                                            _pendingActions = ImmutableQueue<Action>.Empty;
                                            oldqueue
                                                .Except(new[] { action })
                                                .ForEach(it => _pendingActions = _pendingActions.Enqueue(it));
                                        };
                                    }, browserCookies, browserAutomationStatus, pinterestAccountType, mailCreds, useSsl == 1));
                        }
                        else
                        {
                            warn(string.Format(
                                "LangKeyTheAccountCantBeImportedNetworkNotAvailable".FromResourceDictionary(),
                                objDominatorAccountBaseModel,
                                objDominatorAccountBaseModel.AccountNetwork));
                            GlobusLogHelper.log.Info(
                                SocinatorInitialize.ActiveSocialNetwork + "\t" +
                                "LangKeyTheAccountCantBeImportedNetworkNotAvailable".FromResourceDictionary(),
                                objDominatorAccountBaseModel.UserName,
                                objDominatorAccountBaseModel.AccountNetwork);
                        }
                    }
                    catch (Exception ex)
                    {
                        /*INFO*/
                        Console.WriteLine(ex.StackTrace);
                        ex.DebugLog();
                    }

                _allAccountsQueued = true;

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void DownloadAccountsFormats ( string sender )
        {
            try
            {
                var FormatString = sender as string;
                var DownloadLocation = FileUtilities.GetExportPath(Application.Current.MainWindow);
                Dictionary<string, string> formats = new Dictionary<string, string>()
                {
                    {"AccountFormat_1.csv","Group Name,SocialNetwork,User Name,Password,Alternate Email" },
                    {"AccountFormat_2.csv","Group Name,SocialNetwork,User Name,Password,Proxy Address,Proxy Port,Alternate Email" },
                    {"AccountFormat_3.csv","Group Name,SocialNetwork,User Name,Password,Proxy Address,Proxy Port,Proxy User Name,Proxy Password,Alternate Email" },
                    {"AccountFormat_4.csv","Group Name,SocialNetwork,User Name,Password,Proxy Address,Proxy Port,Proxy User Name,Proxy Password,Status,Cookies" }
                };
                foreach (var item in formats)
                {
                    var file = $"{DownloadLocation}\\{item.Key}";
                    using (var streamWriter = new StreamWriter(file, false, Encoding.UTF8))
                    {
                        streamWriter.WriteLine(item.Value);
                    }
                }
                formats.Clear();
                ToasterNotification.ShowSuccess("LangKeySuccessfullyDownloadedFormats".FromResourceDictionary());
            }
            catch (Exception ex) { ex.DebugLog(ex.StackTrace); }
        }
        MailCredentials SplitedAccMailCreds ( string[] splitAccount )
        {
            return new MailCredentials
            {
                Username = splitAccount[splitAccount.Length - 5],
                Password = splitAccount[splitAccount.Length - 4],
                Hostname = splitAccount[splitAccount.Length - 3],
                Port = Convert.ToInt32(splitAccount[splitAccount.Length - 2]),
            };
        }


        public void AddAccount ( DominatorAccountBaseModel objDominatorAccountBaseModel, string cookies,
            Func<Action, Action> secondaryTaskStrategyReturningCancellation, string browserCookies
            , bool isBrowserAutomationActive = false, string pinterestAccountType = "", MailCredentials mailCredentials = null, bool useSsl = false )
        {
            #region Check account limits

            if (LstDominatorAccountModel.Count >= SocinatorInitialize.MaximumAccountCount) return;

            #endregion

            #region Add Account

            //check the account is already present or not
            if (LstDominatorAccountModel.Any(x =>
                x.AccountBaseModel.UserName == objDominatorAccountBaseModel.UserName &&
                x.AccountBaseModel.AccountNetwork == objDominatorAccountBaseModel.AccountNetwork))
            {
                /*INFO*/
                GlobusLogHelper.log.Info(Log.AlreadyAddedAccount, objDominatorAccountBaseModel.AccountNetwork,
                    objDominatorAccountBaseModel.UserName);
                return;
            }

            objDominatorAccountBaseModel.AccountGroup.Content =
                string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountGroup.Content)
                    ? ConstantVariable.UnGrouped()
                    : objDominatorAccountBaseModel.AccountGroup.Content;

            //Initialize the given account to account model
            var dominatorAccountBaseModel = new DominatorAccountBaseModel
            {
                AccountGroup =
                {
                    Content = string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountGroup.Content)
                        ? ConstantVariable.UnGrouped()
                        : objDominatorAccountBaseModel.AccountGroup.Content
                },
                UserName = objDominatorAccountBaseModel.UserName,
                Password = objDominatorAccountBaseModel.Password,
                AccountProxy =
                {
                    ProxyGroup = string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyGroup)
                        ? "Ungrouped"
                        : objDominatorAccountBaseModel.AccountProxy.ProxyGroup,
                    ProxyIp = objDominatorAccountBaseModel.AccountProxy.ProxyIp,
                    ProxyPort = objDominatorAccountBaseModel.AccountProxy.ProxyPort,
                    ProxyUsername = objDominatorAccountBaseModel.AccountProxy.ProxyUsername,
                    ProxyPassword = objDominatorAccountBaseModel.AccountProxy.ProxyPassword
                },
                Status = string.IsNullOrEmpty(objDominatorAccountBaseModel.Status.ToString())
                    ? AccountStatus.NotChecked
                    : objDominatorAccountBaseModel.Status,
                AccountNetwork = objDominatorAccountBaseModel.AccountNetwork,
                AccountId = objDominatorAccountBaseModel.AccountId,
                IsChkTwoFactorLogin = objDominatorAccountBaseModel.IsChkTwoFactorLogin,
                AlternateEmail = objDominatorAccountBaseModel.AlternateEmail,
                AccountName = objDominatorAccountBaseModel.AccountName
            };

            var dominatorAccountModel = new DominatorAccountModel
            {
                AccountBaseModel = dominatorAccountBaseModel,
                RowNo = LstDominatorAccountModel.Count + 1,
                AccountId = dominatorAccountBaseModel.AccountId,
                DisplayColumnValue11 = pinterestAccountType
            };
            if (mailCredentials != null)
            {
                dominatorAccountModel.IsAutoVerifyByEmail = true;
                dominatorAccountModel.MailCredentials = mailCredentials;
                dominatorAccountModel.IsUseSSL = useSsl;
            }

            if (!string.IsNullOrEmpty(cookies))
                try
                {
                    dominatorAccountModel.CookieHelperList =
                        JArray.Parse(cookies.Replace("<>", ",")).ToObject<HashSet<CookieHelper>>();
                }
                catch
                {
                }

            if (!string.IsNullOrEmpty(browserCookies))
                try
                {
                    dominatorAccountModel.BrowserCookieHelperList = JArray.Parse(browserCookies.Replace("<>", ","))
                        .ToObject<HashSet<CookieHelper>>();
                }
                catch
                {
                }

            var oldproxies = _proxyFileManager.GetAllProxy();

            //facebook accounts getting banned due to httpAutomation, as for now YouTube accounts are not working with http automation, So setting value to IsRunProcessThroughBrowser as true by default for fb and yd accounts only.
            dominatorAccountModel.IsRunProcessThroughBrowser =
                dominatorAccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook || dominatorAccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.YouTube || dominatorAccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.Instagram
                    ? true
                    : isBrowserAutomationActive;

            //var cancel = secondaryTaskStrategyReturningCancellation(() => UpdateProxy(objDominatorAccountBaseModel));
            //dominatorAccountModel.Token.Register(cancel);
            if (!string.IsNullOrEmpty(dominatorAccountBaseModel.AccountProxy.ProxyIp) &&
                !string.IsNullOrEmpty(dominatorAccountBaseModel.AccountProxy.ProxyPort))
                if (!IsDuplicatProxyAvailable(objDominatorAccountBaseModel, oldproxies, null))
                    if (!UpdateProxy(dominatorAccountBaseModel))
                        AddProxyIfNotExist(dominatorAccountBaseModel);

            var cancel = secondaryTaskStrategyReturningCancellation(() => UpdateProxy(objDominatorAccountBaseModel));
            dominatorAccountModel.Token.Register(cancel);


            //serialize the given account, if its success then add to account model list
            if (_accountsFileManager.Add(dominatorAccountModel))
            {
                if (!Application.Current.Dispatcher.CheckAccess())
                    Application.Current.Dispatcher.Invoke(() => LstDominatorAccountModel.Add(dominatorAccountModel));
                else
                    LstDominatorAccountModel.Add(dominatorAccountModel);

                GlobusLogHelper.log.Info(Log.Added, objDominatorAccountBaseModel.AccountNetwork,
                    objDominatorAccountBaseModel.UserName, "LangKeyAccounts".FromResourceDictionary());
            }
            else
            {
                /*INFO*/
                GlobusLogHelper.log.Info(Log.NotAddedAccount, objDominatorAccountBaseModel.AccountNetwork,
                    objDominatorAccountBaseModel.UserName);
            }

            //Testing 
            var databaseCreation = secondaryTaskStrategyReturningCancellation(() =>
            {
                var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());

                #region Saving Account detail to AccountDetails database

                globalDbOperation.Add(new AccountDetails
                {
                    AccountNetwork = objDominatorAccountBaseModel.AccountNetwork.ToString(),
                    AccountId = objDominatorAccountBaseModel.AccountId,
                    AccountGroup = dominatorAccountBaseModel.AccountGroup.Content,
                    UserName = objDominatorAccountBaseModel.UserName,
                    Password = objDominatorAccountBaseModel.Password,
                    UserFullName = objDominatorAccountBaseModel.UserFullName,
                    Status = objDominatorAccountBaseModel.Status.ToString(),
                    ProxyIP = objDominatorAccountBaseModel.AccountProxy.ProxyIp,
                    ProxyPort = objDominatorAccountBaseModel.AccountProxy.ProxyPort,
                    ProxyUserName = objDominatorAccountBaseModel.AccountProxy.ProxyUsername,
                    ProxyPassword = objDominatorAccountBaseModel.AccountProxy.ProxyPassword,
                    AccountName = objDominatorAccountBaseModel.AccountName,
                    UserAgent = dominatorAccountModel.UserAgentWeb,
                    AddedDate = DateTime.Now,
                    Cookies = cookies
                });

                #endregion
            });
            dominatorAccountModel.Token.Register(databaseCreation);

            #endregion

            var softwareSettingsFileManager = InstanceProvider.GetInstance<ISoftwareSettingsFileManager>();
            var softwareSettings = softwareSettingsFileManager.GetSoftwareSettings();
            if (!softwareSettings.IsDoNotAutoLoginAccountsWhileAddingToSoftware)
                try
                {
                    var accountFactory = SocinatorInitialize
                        .GetSocialLibrary(objDominatorAccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory().AccountUpdateFactory;

                    dominatorAccountModel.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                    dominatorAccountModel.AccountBaseModel.NeedToCloseBrowser = false;
                    if (typeof(IAccountUpdateFactoryAsync).IsAssignableFrom(accountFactory.GetType()))
                    {
                        // this account supports async modules
                        var asyncAccount = (IAccountUpdateFactoryAsync)accountFactory;

                        try
                        {
                            asyncAccount
                                .CheckStatusAsync(dominatorAccountModel, dominatorAccountModel.Token)
                                .ContinueWith(checkSucceeded =>
                                {
                                    try
                                    {
                                        if (checkSucceeded.Result)
                                        {
                                            return asyncAccount.UpdateDetailsAsync(dominatorAccountModel,
                                                dominatorAccountModel.Token);
                                        }

                                        return new Task(() => { });
                                    }
                                    catch (OperationCanceledException)
                                    {
                                        return Task.CompletedTask;
                                    }
                                    catch (AggregateException ae)
                                    {
                                        ae.HandleOperationCancellation();

                                        return Task.CompletedTask;
                                    }
                                    catch (Exception)
                                    {
                                        return Task.CompletedTask;
                                    }
                                })
                                .Start();
                        }
                        catch (OperationCanceledException)
                        {
                            throw new OperationCanceledException();
                        }
                        catch (AggregateException ae)
                        {
                            ae.HandleOperationCancellation();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                    else
                    {
                        // TODO: Add on-deleted cancellation mechanics for non-async modules 
                        var cancelUpdate = secondaryTaskStrategyReturningCancellation(() =>
                        {
                            accountFactory.CheckStatus(dominatorAccountModel);

                            accountFactory.UpdateDetails(dominatorAccountModel);
                        });
                        dominatorAccountModel.Token.Register(cancelUpdate);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
        }

        public bool UpdateProxy ( DominatorAccountBaseModel objDominatorAccountBaseModel )
        {
            var oldproxies = _proxyFileManager.GetAllProxy();

            var isProxyUpdated = false;
            try
            {
                var oldAccount = _accountsFileManager
                    .GetAccount(objDominatorAccountBaseModel.UserName, objDominatorAccountBaseModel.AccountNetwork)
                    .AccountBaseModel;

                isProxyUpdated = IsProxyUpdated(objDominatorAccountBaseModel, oldproxies, oldAccount);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return isProxyUpdated;
        }

        private bool IsDuplicatProxyAvailable ( DominatorAccountBaseModel objAccountBaseModel,
            List<ProxyManagerModel> oldProxies,
            DominatorAccountBaseModel oldAccount )
        {
            var isDuplicatProxyAvailable = false;
            foreach (var proxy in oldProxies)
            {
                #region To check for duplicate proxy 

                try
                {
                    if (objAccountBaseModel.AccountProxy.ProxyIp == proxy.AccountProxy.ProxyIp
                        && objAccountBaseModel.AccountProxy.ProxyPort == proxy.AccountProxy.ProxyPort
                        && objAccountBaseModel.AccountProxy.ProxyUsername == proxy.AccountProxy.ProxyUsername
                        && objAccountBaseModel.AccountProxy.ProxyPassword == proxy.AccountProxy.ProxyPassword)
                    {
                        #region If other proxy with same ip/port is present

                        try
                        {
                            if (string.IsNullOrEmpty(proxy.AccountProxy.ProxyUsername) ||
                                proxy.AccountProxy.ProxyUsername != objAccountBaseModel.AccountProxy.ProxyUsername)
                                proxy.AccountProxy.ProxyUsername = objAccountBaseModel.AccountProxy.ProxyUsername;

                            if (string.IsNullOrEmpty(proxy.AccountProxy.ProxyPassword) ||
                                proxy.AccountProxy.ProxyPassword != objAccountBaseModel.AccountProxy.ProxyPassword)
                                proxy.AccountProxy.ProxyPassword = objAccountBaseModel.AccountProxy.ProxyPassword;

                            if (string.IsNullOrEmpty(proxy.AccountProxy.ProxyGroup) || proxy.AccountProxy.ProxyGroup !=
                                objAccountBaseModel.AccountProxy.ProxyGroup)
                                proxy.AccountProxy.ProxyGroup = objAccountBaseModel.AccountProxy.ProxyGroup;

                            objAccountBaseModel.AccountProxy = proxy.AccountProxy;

                            var accountTomodified = new AccountAssign
                            {
                                UserName = objAccountBaseModel.UserName,
                                AccountNetwork = objAccountBaseModel.AccountNetwork
                            };

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                try
                                {
                                    if (oldAccount != null)
                                    {
                                        oldProxies.ForEach(pr =>
                                        {
                                            var accountToRemove = pr.AccountsAssignedto.FirstOrDefault(acc =>
                                                acc.UserName == oldAccount.UserName &&
                                                acc.AccountNetwork == oldAccount.AccountNetwork);

                                            if (accountToRemove != null)
                                            {
                                                pr.AccountsAssignedto.Remove(accountToRemove);
                                                _proxyFileManager.EditProxy(pr);
                                            }
                                        });
                                        var proxyToUpdate = _proxyManagerViewModel.LstProxyManagerModel.FirstOrDefault(
                                            x => x.AccountProxy.ProxyIp == oldAccount.AccountProxy.ProxyIp
                                                 && x.AccountProxy.ProxyPort == oldAccount.AccountProxy.ProxyPort);
                                        // ReSharper disable once ConstantConditionalAccessQualifier
                                        proxyToUpdate?.AccountsAssignedto.Remove(
                                            proxyToUpdate?.AccountsAssignedto.FirstOrDefault(x =>
                                                x.UserName == oldAccount.UserName &&
                                                x.AccountNetwork == oldAccount.AccountNetwork));

                                        proxyToUpdate = _proxyManagerViewModel.LstProxyManagerModel.FirstOrDefault(x =>
                                            x.AccountProxy.ProxyIp == objAccountBaseModel.AccountProxy.ProxyIp
                                            && x.AccountProxy.ProxyPort == objAccountBaseModel.AccountProxy.ProxyPort);
                                        proxyToUpdate?.AccountsAssignedto.Add(
                                            new AccountAssign
                                            {
                                                UserName = objAccountBaseModel.UserName,
                                                AccountNetwork = objAccountBaseModel.AccountNetwork
                                            });
                                    }
                                    else
                                    {
                                        var proxyToUpdate = _proxyManagerViewModel.LstProxyManagerModel.FirstOrDefault(
                                            x => x.AccountProxy.ProxyIp == objAccountBaseModel.AccountProxy.ProxyIp
                                                 && x.AccountProxy.ProxyPort ==
                                                 objAccountBaseModel.AccountProxy.ProxyPort);
                                        if (proxyToUpdate != null && !proxyToUpdate.AccountsAssignedto.Any(assignedProxy => assignedProxy.UserName == objAccountBaseModel.UserName && assignedProxy.AccountNetwork == objAccountBaseModel.AccountNetwork))
                                            proxyToUpdate?.AccountsAssignedto.Add(
                                                new AccountAssign
                                                {
                                                    UserName = objAccountBaseModel.UserName,
                                                    AccountNetwork = objAccountBaseModel.AccountNetwork
                                                });
                                        if (!string.IsNullOrEmpty(proxy.AccountProxy.ProxyGroup))
                                            proxyToUpdate.AccountProxy.ProxyGroup = proxy.AccountProxy.ProxyGroup;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }

                                if (proxy != null && !proxy.AccountsAssignedto.Any(assignedTo => assignedTo.AccountNetwork == accountTomodified.AccountNetwork && assignedTo.UserName == accountTomodified.UserName))
                                    proxy.AccountsAssignedto.Add(accountTomodified);

                                _proxyFileManager.EditProxy(proxy);
                                if (_proxyManagerViewModel != null && !_proxyManagerViewModel.AccountsAlreadyAssigned.Any(proxyExist => proxyExist.UserName == accountTomodified.UserName && proxyExist.AccountNetwork == accountTomodified.AccountNetwork))
                                    _proxyManagerViewModel.AccountsAlreadyAssigned.Add(
                                        new AccountAssign
                                        {
                                            UserName = accountTomodified.UserName,
                                            AccountNetwork = accountTomodified.AccountNetwork
                                        });
                            });
                            isDuplicatProxyAvailable = true;
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        break;
                        // }

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }

                #endregion
            }

            return isDuplicatProxyAvailable;
        }

        private bool IsProxyUpdated ( DominatorAccountBaseModel objDominatorAccountBaseModel,
            List<ProxyManagerModel> oldProxies,
            DominatorAccountBaseModel oldAccount )
        {
            var isProxyUpdated = false;
            foreach (var proxy in oldProxies)
            {
                #region If old proxy for account is updated

                if (objDominatorAccountBaseModel.AccountProxy.ProxyIp != oldAccount.AccountProxy.ProxyIp
                    || objDominatorAccountBaseModel.AccountProxy.ProxyPort != oldAccount.AccountProxy.ProxyPort)
                    if (objDominatorAccountBaseModel.AccountProxy.ProxyId == proxy.AccountProxy.ProxyId)
                    {
                        isProxyUpdated = true;
                        proxy.AccountProxy.ProxyIp = objDominatorAccountBaseModel.AccountProxy.ProxyIp;
                        proxy.AccountProxy.ProxyPort = objDominatorAccountBaseModel.AccountProxy.ProxyPort;
                        proxy.AccountProxy.ProxyUsername = objDominatorAccountBaseModel.AccountProxy.ProxyUsername;
                        proxy.AccountProxy.ProxyPassword = objDominatorAccountBaseModel.AccountProxy.ProxyPassword;

                        _proxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
                        UpdateProxyList(proxy);
                        _proxyFileManager.EditProxy(proxy);
                        break;
                    }

                #endregion

                #region To check proxy is Exist or not

                if (objDominatorAccountBaseModel.AccountProxy.ProxyIp == proxy.AccountProxy.ProxyIp
                    && objDominatorAccountBaseModel.AccountProxy.ProxyPort == proxy.AccountProxy.ProxyPort)
                {
                    #region If other proxy with same ip/port not there

                    if (objDominatorAccountBaseModel.AccountProxy.ProxyId == proxy.AccountProxy.ProxyId)
                    {
                        if (objDominatorAccountBaseModel.AccountProxy.ProxyUsername != proxy.AccountProxy.ProxyUsername
                            || objDominatorAccountBaseModel.AccountProxy.ProxyPassword !=
                            proxy.AccountProxy.ProxyPassword)
                        {
                            proxy.AccountProxy.ProxyUsername = objDominatorAccountBaseModel.AccountProxy.ProxyUsername;
                            proxy.AccountProxy.ProxyPassword = objDominatorAccountBaseModel.AccountProxy.ProxyPassword;
                            _proxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
                            UpdateProxyList(proxy);
                            //  ProxyFileManager.EditProxy(proxy);
                        }

                        var account = proxy.AccountsAssignedto.FirstOrDefault(x =>
                            x.UserName == objDominatorAccountBaseModel.UserName);
                        if (account == null)
                        {
                            #region Add account to AccountsAssignedto list if current proxy is not Assigned to current Account

                            proxy.AccountsAssignedto.Add(new AccountAssign
                            {
                                UserName = objDominatorAccountBaseModel.UserName,
                                AccountNetwork = objDominatorAccountBaseModel.AccountNetwork
                            });

                            #endregion

                            _proxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
                            isProxyUpdated = true;
                        }

                        break;
                    }

                    #endregion
                }

                #endregion
            }

            return isProxyUpdated;
        }


        private void UpdateProxyList ( ProxyManagerModel proxy )
        {
            try
            {
                var proxyToupdate = _proxyManagerViewModel.LstProxyManagerModel.FirstOrDefault(x =>
                    x.AccountProxy.ProxyId == proxy.AccountProxy.ProxyId);

                if (proxyToupdate != null)
                    proxyToupdate.AccountProxy = proxy.AccountProxy;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddProxyIfNotExist ( DominatorAccountBaseModel objAccount )
        {
            var ProxyManagerModel = new ProxyManagerModel
            {
                AccountProxy =
                {
                    ProxyName =
                        $"Proxy {objAccount.AccountProxy.ProxyIp.Replace(".", "")}{objAccount.AccountProxy.ProxyPort}",
                    ProxyId = objAccount.AccountProxy.ProxyId,
                    ProxyIp = objAccount.AccountProxy.ProxyIp,
                    ProxyPort = objAccount.AccountProxy.ProxyPort,
                    ProxyUsername = objAccount.AccountProxy.ProxyUsername,
                    ProxyPassword = objAccount.AccountProxy.ProxyPassword,
                    ProxyGroup = objAccount.AccountProxy.ProxyGroup
                }
            };

            Application.Current.Dispatcher.Invoke(() =>
            {
                _proxyManagerViewModel.LstProxyManagerModel.ForEach(x =>
                {
                    if (x.AccountsAssignedto.Any(y => y.UserName == objAccount.UserName &&
                                                      y.AccountNetwork == objAccount.AccountNetwork))
                        x.AccountsAssignedto.Remove(x.AccountsAssignedto.FirstOrDefault(y =>
                            y.UserName == objAccount.UserName &&
                            y.AccountNetwork == objAccount.AccountNetwork));
                });
                if (_proxyManagerViewModel != null && !_proxyManagerViewModel.LstProxyManagerModel.Any(proxy => proxy.AccountProxy.ProxyIp == ProxyManagerModel.AccountProxy.ProxyIp && proxy.AccountProxy.ProxyPort == ProxyManagerModel.AccountProxy.ProxyPort))
                {
                    _proxyManagerViewModel.LstProxyManagerModel.Add(ProxyManagerModel);
                    ProxyManagerModel.Index =
                        _proxyManagerViewModel.LstProxyManagerModel.IndexOf(ProxyManagerModel) + 1;
                }
                if (_proxyManagerViewModel != null && !_proxyManagerViewModel.AccountsAlreadyAssigned.Any(proxy => proxy.UserName == objAccount.UserName && proxy.AccountNetwork == objAccount.AccountNetwork))
                    _proxyManagerViewModel.AccountsAlreadyAssigned.Add(
                        new AccountAssign
                        {
                            UserName = objAccount.UserName,
                            AccountNetwork = objAccount.AccountNetwork
                        });
            });

            ProxyManagerModel.AccountsAssignedto.Add(new AccountAssign
            {
                UserName = objAccount.UserName,
                AccountNetwork = objAccount.AccountNetwork
            });

            _proxyFileManager.SaveProxy(ProxyManagerModel);

            _proxyFileManager.UpdateProxyStatusAsync(ProxyManagerModel, ConstantVariable.GoogleLink);
        }

        #endregion

        #region Delete Accounts

        public void DeleteAccountFromProxy ( List<DominatorAccountModel> objAccountBaseModel )
        {
            var allProxy = _proxyFileManager.GetAllProxy();
            ThreadFactory.Instance.Start(() =>
            {
                allProxy?.ForEach(proxy =>
                {
                    try
                    {
                        objAccountBaseModel.ForEach(account =>
                        {
                            _proxyManagerViewModel.AccountsAlreadyAssigned.Remove(
                                _proxyManagerViewModel.AccountsAlreadyAssigned.FirstOrDefault(x =>
                                    x.UserName == account.UserName
                                    && x.AccountNetwork == account.AccountBaseModel.AccountNetwork));

                            proxy.AccountsAssignedto.Remove(proxy.AccountsAssignedto.FirstOrDefault(x =>
                                x.UserName == account.UserName
                                && x.AccountNetwork == account.AccountBaseModel.AccountNetwork));

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                try
                                {
                                    var proxyToupdate = _proxyManagerViewModel.LstProxyManagerModel
                                        .FirstOrDefault(pr => pr.AccountProxy.ProxyId == proxy.AccountProxy.ProxyId);

                                    proxyToupdate?.AccountsAssignedto.Remove(
                                        proxyToupdate.AccountsAssignedto.FirstOrDefault(x =>
                                            x.UserName == account.UserName
                                            && x.AccountNetwork == account.AccountBaseModel.AccountNetwork));
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }
                            });

                            // proxy.AccountsToBeAssign.Remove(proxy.AccountsToBeAssign.FirstOrDefault(x => x.UserName == account.UserName));
                        });
                    }
                    catch (Exception ex)
                    {
                        ex.ErrorLog();
                    }

                    _proxyFileManager.EditProxy(proxy);
                });
            });
        }

        private void SingleAccountDeleteExecute ( DominatorAccountModel selectedRow )
        {
            DeleteAccountByContextMenu(selectedRow);
        }

        private List<DominatorAccountModel> GetSelectedAccount ()
        {
            return LstDominatorAccountModel.Where(x =>
                x.IsAccountManagerAccountSelected &&
                (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social ||
                 x.AccountBaseModel.AccountNetwork == SocinatorInitialize.ActiveSocialNetwork)).ToList();
        }

        private void DeleteAccountsExecute ()
        {
            try
            {
                //collect the selected account
                var selectAccounts = GetSelectedAccount();

                if (selectAccounts.Count == 0)
                {
                    Dialog.ShowDialog("LangKeyAlert".FromResourceDictionary(),
                        "LangKeyErrorSelectAtleastOneAccount".FromResourceDictionary());
                    return;
                }

                var dialogResult = Dialog.ShowCustomDialog("LangKeyConfirmation".FromResourceDictionary(),
                    "LangKeyConfirmToDeleteSelectedAccounts".FromResourceDictionary(),
                    "LangKeyDeleteAnyway".FromResourceDictionary(), "LangKeyDontDelete".FromResourceDictionary());
                if (dialogResult != MessageDialogResult.Affirmative)
                    return;

                // ThreadFactory.Instance.Start(() => { DeleteAccounts(selectAccounts); });
                DeleteAccounts(selectAccounts);
            }
            catch (Exception ex)
            {
                /*INFO*/
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void DeleteAccounts ( IEnumerable<DominatorAccountModel> selectAccounts )
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var _dbOperations = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());

                selectAccounts.ToList().ForEach(item =>
                {
                    LstDominatorAccountModel.Remove(
                        LstDominatorAccountModel.FirstOrDefault(x => x.AccountId == item.AccountId));
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var network = item.AccountBaseModel.AccountNetwork.ToString();

                            _dbOperations.RemoveMatch<AccountDetails>(user =>
                                user.UserName == item.UserName && user.AccountNetwork == network);

                            item.NotifyCancelled();
                        }
                        catch
                        {
                        }
                    });
                });
            });

            // remove from file
            DeleteAccountFromCampaign(selectAccounts);

            DeleteAccountFromProxy(selectAccounts.ToList());

            //also delete the associated files
            _dataBaseHandler.DeleteDatabase(selectAccounts.Select(acct => acct.AccountId));
        }

        private void DeleteAccountFromCampaign ( IEnumerable<DominatorAccountModel> selectAccounts )
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var account in selectAccounts)
                    try
                    {
                        var jobActivityConfigurationManager =
                            InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                        var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
                        var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                        foreach (var moduleConfiguration in jobActivityConfigurationManager[account.AccountId].ToList())
                        {
                            dominatorScheduler.StopActivity(account, moduleConfiguration.ActivityType.ToString(),
                                moduleConfiguration.TemplateId, false);
                            if (moduleConfiguration.IsTemplateMadeByCampaignMode)
                            {
                                campaignFileManager.DeleteSelectedAccount(moduleConfiguration.TemplateId,
                                    account.AccountBaseModel.UserName);
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    var campToUpdate = Campaigns
                                        .GetCampaignsInstance(account.AccountBaseModel.AccountNetwork).CampaignViewModel
                                        .LstCampaignDetails.FirstOrDefault(x =>
                                            x.TemplateId == moduleConfiguration.TemplateId);
                                    campToUpdate?.SelectedAccountList.Remove(account.AccountBaseModel.UserName);
                                });
                            }
                        }

                        //Remove Account from Account bin file
                        _accountsFileManager.Delete(x => x.AccountId == account.AccountId);

                        GlobusLogHelper.log.Info(Log.Deleted, account.AccountBaseModel.AccountNetwork,
                            account.AccountBaseModel.UserName, "LangKeyAccounts".FromResourceDictionary());

                        Thread.Sleep(5);
                    }
                    catch (Exception)
                    {
                    }
            });
        }

        private void DeleteAccountFromCampaign ( DominatorAccountModel account )
        {
            // account = _accountsFileManager.GetAccountById(account.AccountId);
            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
            foreach (var moduleConfiguration in jobActivityConfigurationManager[account.AccountId])
            {
                dominatorScheduler.StopActivity(account, moduleConfiguration.ActivityType.ToString(),
                    moduleConfiguration.TemplateId, false);
                if (moduleConfiguration.IsTemplateMadeByCampaignMode)
                {
                    campaignFileManager.DeleteSelectedAccount(moduleConfiguration.TemplateId,
                        account.AccountBaseModel.UserName);
                    var campToUpdate = Campaigns.GetCampaignsInstance(account.AccountBaseModel.AccountNetwork)
                        .CampaignViewModel.LstCampaignDetails
                        .FirstOrDefault(x => x.TemplateId == moduleConfiguration.TemplateId);
                    campToUpdate?.SelectedAccountList.Remove(account.AccountBaseModel.UserName);
                }
            }
        }

        public void DeleteAccountByContextMenu ( DominatorAccountModel selectedRow )
        {
            var selectedAccount = LstDominatorAccountModel.FirstOrDefault(x =>
                selectedRow != null && x.AccountBaseModel.AccountId == selectedRow.AccountBaseModel.AccountId);

            if (selectedAccount == null)
                return;
            var dialogResult = Dialog.ShowCustomDialog(
                "LangKeyConfirmation".FromResourceDictionary(),
                "LangKeyConfirmToDeleteSelectedAccounts".FromResourceDictionary(),
                "LangKeyDeleteAnyway".FromResourceDictionary(),
                "LangKeyDontDelete".FromResourceDictionary());
            if (dialogResult != MessageDialogResult.Affirmative)
                return;
            DeleteAccounts(new[] { selectedAccount });
        }

        #endregion

        #region Select Accounts

        private void SelectAccountExecute ( bool? sender )
        {
            SelectAllAccounts(sender ?? false);
        }

        private void SelectAccountByStatusExecute ( string sender )
        {
            SelectAccount(sender);
        }

        public void SelectAllAccounts ( bool select )
        {
            LstDominatorAccountModel
                .Where(x => SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social ||
                            x.AccountBaseModel.AccountNetwork == SocinatorInitialize.ActiveSocialNetwork).ForEach(
                    x => { x.IsAccountManagerAccountSelected = select; });
        }

        public void SelectAccount ( string menu )
        {
            SelectAllAccounts(false);

            switch (menu)
            {
                case "Working":
                    if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
                        LstDominatorAccountModel.Where(x => x.AccountBaseModel.Status == AccountStatus.Success)
                            .ForEach(x => { x.IsAccountManagerAccountSelected = true; });
                    else
                        LstDominatorAccountModel.Where(x =>
                            x.AccountBaseModel.Status == AccountStatus.Success && x.AccountBaseModel.AccountNetwork ==
                            SocinatorInitialize.ActiveSocialNetwork).ForEach(x =>
                            {
                                x.IsAccountManagerAccountSelected = true;
                            });
                    break;
                case "NotWorking":

                    if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
                        LstDominatorAccountModel.ForEach(x =>
                        {
                            switch (x.AccountBaseModel.Status)
                            {
                                case AccountStatus.Success:
                                case AccountStatus.NotChecked:
                                case AccountStatus.TryingToLogin:
                                case AccountStatus.UpdatingDetails:
                                    break;
                                default:
                                    x.IsAccountManagerAccountSelected = true;
                                    break;
                            }
                        });
                    else
                        LstDominatorAccountModel.Where(x =>
                            x.AccountBaseModel.AccountNetwork == SocinatorInitialize.ActiveSocialNetwork).ForEach(x =>
                            {
                                switch (x.AccountBaseModel.Status)
                                {
                                    case AccountStatus.Success:
                                    case AccountStatus.NotChecked:
                                    case AccountStatus.TryingToLogin:
                                    case AccountStatus.UpdatingDetails:
                                        break;
                                    default:
                                        x.IsAccountManagerAccountSelected = true;
                                        break;
                                }
                            });
                    break;
                case "NotChecked":
                    if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
                        LstDominatorAccountModel.Where(x => x.AccountBaseModel.Status == AccountStatus.NotChecked)
                            .ForEach(x => { x.IsAccountManagerAccountSelected = true; });
                    else
                        LstDominatorAccountModel
                            .Where(x => x.AccountBaseModel.Status == AccountStatus.NotChecked &&
                                        x.AccountBaseModel.AccountNetwork == SocinatorInitialize.ActiveSocialNetwork)
                            .ForEach(x => { x.IsAccountManagerAccountSelected = true; });
                    break;
            }
        }

        private void SelectAccountByGroupExecute ( ContentSelectGroup currentGroup )
        {
            if (currentGroup != null)
                lock (_syncLoadAccounts)
                {
                    LstDominatorAccountModel.Where(y => y.AccountBaseModel.AccountGroup.Content == currentGroup.Content)
                        .ForEach(
                            y => { y.IsAccountManagerAccountSelected = currentGroup.IsContentSelected; });
                }
        }

        #endregion

        #region Initialize AccountManager

        private readonly AccessorStrategies strategyPack;

        public void InitialAccountDetails ()
        {
            try
            {
                lock (_syncLoadAccounts)
                {
                    try
                    {
                        var accountList = _accountsFileManager.GetAll();
                        var Proxies = _proxyFileManager.GetAllProxy();
                        var availablenetworks = InstanceProvider.GetAllInstance<ISocialNetworkModule>()
                            .Select(y => y.Network);

                        if (accountList == null || accountList.Count == 0)
                        {
                            var filePath = ConstantVariable.GetIndexAccountFile();
                            if (File.Exists(filePath))
                                File.Delete(filePath);

                            UpdateAccountFromDb(availablenetworks);
                        }
                        else if (accountList != null && IsEqualDbBin(availablenetworks, accountList.Count))
                        {
                            var filePath = ConstantVariable.GetIndexAccountFile();
                            if (File.Exists(filePath))
                                File.Delete(filePath);

                            UpdateAccountFromDb(availablenetworks);

                        }
                        else
                        {
                            var globalDbOperation =
                                new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
                            var savedAccounts = accountList.Where(x =>
                                availablenetworks.Contains(x.AccountBaseModel.AccountNetwork));
                            var dictNetLasNum = new Dictionary<SocialNetworks, int>();
                            foreach (var account in savedAccounts)
                                if (SocinatorInitialize.AvailableNetworks.Contains(account.AccountBaseModel
                                    .AccountNetwork))
                                {
                                    if (LstDominatorAccountModel.Count >= SocinatorInitialize.MaximumAccountCount)
                                    {
                                        GlobusLogHelper.log.Info("LangKeyAddedMaxAccountAsPerYourPlan"
                                            .FromResourceDictionary());
                                        break;
                                    }

                                    if (!LstDominatorAccountModel.Any(x =>
                                        x.AccountBaseModel.UserName == account.UserName &&
                                        x.AccountBaseModel.AccountNetwork == account.AccountBaseModel.AccountNetwork))
                                    {
                                        var needDbUpdate = false;

                                        if (account.AccountBaseModel.Status == AccountStatus.TryingToLogin)
                                        {
                                            if (account.CookieHelperList == null || account.CookieHelperList.Count == 0)
                                                account.AccountBaseModel.Status = AccountStatus.NotChecked;
                                            else
                                            {
                                                account.AccountBaseModel.Status = AccountStatus.Success;
                                                account.IsUserLoggedIn = true;
                                                needDbUpdate = true;
                                            }
                                        }
                                        else if (account.AccountBaseModel.Status == AccountStatus.UpdatingDetails)
                                        {
                                            account.AccountBaseModel.Status = AccountStatus.Success;
                                            account.IsUserLoggedIn = true;
                                            needDbUpdate = true;
                                        }

                                        if (string.IsNullOrEmpty(account.AccountBaseModel.AccountName))
                                        {
                                            account.AccountBaseModel.AccountName =
                                                DefaultAccountNameFromModel(savedAccounts, ref dictNetLasNum,
                                                    account.AccountBaseModel.AccountNetwork);
                                            needDbUpdate = true;
                                        }
                                        if ((account.AccountBaseModel.AccountNetwork == SocialNetworks.Instagram || account.AccountBaseModel.AccountNetwork == SocialNetworks.Quora || account.AccountBaseModel.AccountNetwork == SocialNetworks.TikTok || account.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook || account.AccountBaseModel.AccountNetwork == SocialNetworks.YouTube) && !account.IsRunProcessThroughBrowser)
                                        {
                                            account.IsRunProcessThroughBrowser = true;
                                            needDbUpdate = true;
                                        }

                                        if (needDbUpdate)
                                        {
                                            _accountsFileManager.Edit(account);
                                            UpdateToDb(account.AccountBaseModel.AccountId, account.AccountBaseModel.AccountName, globalDbOperation);
                                        }

                                        if (account.AccountBaseModel.AccountNetwork == SocialNetworks.Pinterest)
                                            account.DisplayColumnValue11 =
                                                string.IsNullOrEmpty(account.DisplayColumnValue11)
                                                    ? PinterestAccountType.Inactive.GetDescriptionAttr()
                                                        .FromResourceDictionary()
                                                    : account.DisplayColumnValue11;
                                        AssignProxyIfNotAvailable(account, Proxies);
                                        LstDominatorAccountModel.AddSync(account);
                                    }
                                }
                        }

                        _mainViewModel.AccountList =
                            new ObservableCollection<DominatorAccountModel>(_accountsFileManager.GetAll());
                        ScheduleActivity.OnDependencyInstalled += StartSchedulingOnDependencyInstalled;
                    }
                    catch (Exception ex)
                    {
                        /*DEBUG*/
                        Console.WriteLine(ex.StackTrace);
                    }
                }


                // Open extra window for facebook in advance and close to avoid hanging issue
                //Task.Factory.StartNew(() =>
                //{
                //    try
                //    {
                //        var accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();

                //        var availablenetworks = InstanceProvider.GetAllInstances<ISocialNetworkModule>()
                //            .Select(y => y.Network);

                //        if (availablenetworks.Contains(SocialNetworks.Facebook))
                //        {
                //            var account = new DominatorAccountModel
                //            {
                //                AccountId = "ActivateBrowserLogin",

                //                AccountBaseModel = new DominatorAccountBaseModel
                //                {
                //                    UserName = "socinator",
                //                    Password = "socinator",
                //                    AccountProxy = new Proxy()
                //                }
                //            };

                //            var browserManager = accountScopeFactory[$"{account.AccountId}_BrowserLogin"]
                //                .Resolve<IBrowserManager>(account.AccountBaseModel.AccountNetwork.ToString());

                //            browserManager.BrowserLogin(account, account.Token, LoginType.InitialiseBrowser);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine(ex.StackTrace);
                //    }
                //});
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void StartSchedulingOnDependencyInstalled()
        {
            try
            {
                #region Start schedu67ling 

                var runningActivityManager = InstanceProvider.GetInstance<IRunningActivityManager>();
                runningActivityManager.Initialize(LstDominatorAccountModel);

                #endregion

                var softwareSetting = InstanceProvider.GetInstance<ISoftwareSettings>();

                softwareSetting.ScheduleAutoUpdation();
                //Ads Scrapping Scheduling.
                //#if !DEBUG
                //                        foreach (var adsnetwork in AdsNetworks)
                //                            if (SocinatorInitialize.GetSocialLibrary(adsnetwork) != null)
                //                                softwareSetting.ScheduleAdsScraping(adsnetwork);
                //#endif
            }
            catch { }
            finally
            {
                ScheduleActivity.OnDependencyInstalled -= StartSchedulingOnDependencyInstalled;
            }
        }

        public void AssignProxyIfNotAvailable ( DominatorAccountModel accountModel, List<ProxyManagerModel> Proxies )
        {
            try
            {
                if (string.IsNullOrEmpty(accountModel.AccountBaseModel.AccountProxy.ProxyIp) && string.IsNullOrEmpty(accountModel.AccountBaseModel.AccountProxy.ProxyPort))
                {
                    var proxy = Proxies.FirstOrDefault(y => y.AccountsAssignedto.Any(z => z.AccountNetwork == accountModel.AccountBaseModel.AccountNetwork && z.UserName == accountModel.UserName));
                    if (proxy != null)
                        accountModel.AccountBaseModel.AccountProxy = new Proxy { ProxyGroup = proxy.AccountProxy.ProxyGroup, ProxyId = proxy.AccountProxy.ProxyId, ProxyIp = proxy.AccountProxy.ProxyIp, ProxyName = proxy.AccountProxy.ProxyName, ProxyPassword = proxy.AccountProxy.ProxyPassword, ProxyPort = proxy.AccountProxy.ProxyPort, ProxyUsername = proxy.AccountProxy.ProxyUsername, HasCredentials = proxy.AccountProxy.HasCredentials };
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog($"Exception==>{ex.Message} Trace==>{ex.StackTrace}");
            }
        }
        private void UpdateToDb ( string accountId, string accountName, DbOperations globalDbOperation = null )
        {
            if (globalDbOperation == null)
                globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
            var updateIt = globalDbOperation.GetSingle<AccountDetails>(x => x.AccountId == accountId);
            if (updateIt == null)
                return;
            updateIt.AccountName = accountName;
            globalDbOperation.Update(updateIt);
        }

        private bool IsEqualDbBin ( IEnumerable<SocialNetworks> availablenetworks, int binAccountCount )
        {
            var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
            int dbAccountCount = 0;
            var accounts = globalDbOperation.Get<AccountDetails>();
            foreach (var account in accounts)
            {
                var net = account.AccountNetwork == "Youtube" ? "YouTube" : account.AccountNetwork;
                var network = (SocialNetworks)Enum.Parse(typeof(SocialNetworks),net);

                if (availablenetworks.Contains(network))
                {
                    dbAccountCount++;
                }
            }
            return (binAccountCount < dbAccountCount) ? true : false;
        }

        private void UpdateAccountFromDb ( IEnumerable<SocialNetworks> availablenetworks )
        {
            var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());

            var accounts = globalDbOperation.Get<AccountDetails>();
            var oldproxies = _proxyFileManager.GetAllProxy();
            var dictNetLasNum = new Dictionary<SocialNetworks, int>();
            foreach (var account in accounts)
            {
                var net = account?.AccountNetwork == "Youtube" ?"YouTube":account?.AccountNetwork;
                var network = (SocialNetworks)Enum.Parse(typeof(SocialNetworks),net);

                if (availablenetworks.Contains(network))
                    if (!LstDominatorAccountModel.Any(x => x.AccountBaseModel.UserName == account.UserName &&
                                                           x.AccountBaseModel.AccountNetwork == network))
                    {
                        var dominatorAccountModel = new DominatorAccountModel
                        {
                            AccountBaseModel = new DominatorAccountBaseModel
                            {
                                AccountNetwork = network,
                                AccountId = account.AccountId,
                                AccountGroup = new ContentSelectGroup { Content = account.AccountGroup },
                                UserName = account.UserName,
                                Password = account.Password,
                                UserFullName = account.UserFullName,
                                AccountProxy = new Proxy
                                {
                                    ProxyIp = account.ProxyIP,
                                    ProxyPort = account.ProxyPort,
                                    ProxyUsername = account.ProxyUserName,
                                    ProxyPassword = account.ProxyPassword
                                }
                            },
                            AccountId = account.AccountId,
                            DisplayColumnValue1 = account.DisplayColumnValue1,
                            DisplayColumnValue2 = account.DisplayColumnValue2,
                            DisplayColumnValue3 = account.DisplayColumnValue3,
                            DisplayColumnValue4 = account.DisplayColumnValue4,
                            DisplayColumnValue11 = account.DisplayColumnValue11,
                            IsRunProcessThroughBrowser = network == SocialNetworks.Facebook || network == SocialNetworks.Quora || network == SocialNetworks.TikTok || network == SocialNetworks.YouTube || network == SocialNetworks.Instagram
                        };

                        if (!string.IsNullOrEmpty(account.Cookies))
                            try
                            {
                                dominatorAccountModel.CookieHelperList = JArray
                                    .Parse(account.Cookies.Replace("<>", ",")).ToObject<HashSet<CookieHelper>>();
                            }
                            catch (Exception)
                            {
                            }

                        if (!string.IsNullOrEmpty(account.Status))
                            dominatorAccountModel.AccountBaseModel.Status =
                                (AccountStatus)Enum.Parse(typeof(AccountStatus), account.Status);
                        if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.TryingToLogin)
                            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.NotChecked;
                        else if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.UpdatingDetails)
                            dominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;

                        if (!string.IsNullOrEmpty(account.ActivityManager))
                            dominatorAccountModel.ActivityManager =
                                JsonConvert.DeserializeObject<JobActivityManager>(account.ActivityManager);

                        if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountName))
                        {
                            dominatorAccountModel.AccountBaseModel.AccountName =
                                DefaultAccountNameFromDB(accounts, ref dictNetLasNum, network);
                            UpdateToDb(dominatorAccountModel.AccountBaseModel.AccountId,
                                dominatorAccountModel.AccountBaseModel.AccountName, globalDbOperation);
                        }
                        AssignProxyIfNotAvailable(dominatorAccountModel, oldproxies);
                        LstDominatorAccountModel.AddSync(dominatorAccountModel);

                        #region Update Proxies

                        if (!string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp) &&
                            !string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPort))
                            if (!IsDuplicatProxyAvailable(dominatorAccountModel.AccountBaseModel, oldproxies, null))
                                if (!UpdateProxy(dominatorAccountModel.AccountBaseModel))
                                    AddProxyIfNotExist(dominatorAccountModel.AccountBaseModel);

                        #endregion

                        _accountsFileManager.Add(dominatorAccountModel);
                    }
            }
        }

        private string DefaultAccountNameFromDB ( List<AccountDetails> listAcc,
            ref Dictionary<SocialNetworks, int> dictNetLasNum, SocialNetworks net )
        {
            var preName = $"{net.ToString()} Account ";
            try
            {
                if (!dictNetLasNum.ContainsKey(net))
                {
                    var lastNum = 0;

                    var netNumbers = listAcc
                        .Where(x => x.AccountNetwork == net.ToString() &&
                                    (x.AccountName?.StartsWith($"{net.ToString()} Account ") ?? false))
                        .Select(y => y.AccountName?.Replace(preName, "")?.Trim());
                    if (netNumbers != null && netNumbers.Count() > 0)
                    {
                        var numList = new List<int>();
                        foreach (var each in netNumbers)
                        {
                            int.TryParse(each, out var num);
                            if (num != 0) numList.Add(num);
                        }

                        numList.Sort();
                        lastNum = numList.Last();
                    }

                    dictNetLasNum.Add(net, lastNum);
                }

                return $"{preName}{++dictNetLasNum[net]}";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return preName;
            }
        }

        public string DefaultAccountNameFromModel ( IEnumerable<DominatorAccountModel> listAcc,
            ref Dictionary<SocialNetworks, int> dictNetLasNum, SocialNetworks net )
        {
            var preName = $"{net.ToString()} Account ";
            try
            {
                if (!dictNetLasNum.ContainsKey(net))
                {
                    var lastNum = 0;

                    var netNumbers = listAcc
                        .Where(x => x.AccountBaseModel.AccountNetwork == net &&
                                    (x.AccountBaseModel.AccountName?.StartsWith($"{net.ToString()} Account ") ?? false))
                        .Select(y => y.AccountBaseModel.AccountName?.Replace(preName, "")?.Trim());
                    if (netNumbers != null && netNumbers.Count() > 0)
                    {
                        var numList = new List<int>();
                        foreach (var each in netNumbers)
                        {
                            int.TryParse(each, out var num);
                            if (num != 0) numList.Add(num);
                        }

                        numList.Sort();
                        lastNum = numList.Last();
                    }

                    dictNetLasNum.Add(net, lastNum);
                }

                return $"{preName}{++dictNetLasNum[net]}";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return preName;
            }
        }

#endregion

        #region Update Account status & details

        private ImmutableQueue<Action> _checkPendingList = ImmutableQueue<Action>.Empty;

        public List<string> _updateAccountList { get; set; } = new List<string>();

        public List<string> _changeBusinessAccountList { get; set; } = new List<string>();

        public object AccountUpdateLock { get; set; } = new object();

        private async void UpdateAccountDetailsExecute(object sender)
        {
            var selectedAccount = LstDominatorAccountModel.Where(x => x.IsAccountManagerAccountSelected).ToList();

            if (selectedAccount.Count == 0)
            {
                Dialog.ShowDialog("LangKeyAlert".FromResourceDictionary(),
                    "LangKeySelectAccountsToUpdate".FromResourceDictionary());
                return;
            }

            var updateMenuItem = sender as string;

            //if user clicked on Stop Process
            if (updateMenuItem == "StopProcess")
            {
                StopProcess(selectedAccount);
                return;
            }

            //if user clicked on Stop All Activity
            if (updateMenuItem == "StopAllActivity")
            {
                StopAllActivity(selectedAccount);
                return;
            }

            //if user clicked on Update all details

            #region Update all details

            try
            {
                var tasks = new List<Task>();

                foreach (var account in selectedAccount)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        await _accountsToLoginDuringMultipleImport.WaitAsync();
                        try
                        {
                            var accountFactory = SocinatorInitialize
                                .GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
                                .GetNetworkCoreFactory().AccountUpdateFactory;

                            if (!_updateAccountList.Contains(account.AccountBaseModel.UserName))
                            {
                                await MultipleUpdate(account, updateMenuItem, accountFactory);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(Log.AlreadyUpdatingAccount,
                                    account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName);
                            }

                            await Task.Delay(1000);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                        finally
                        {
                            _accountsToLoginDuringMultipleImport.Release();
                        }
                    }));
                }

                await Task.WhenAll(tasks);

                //    currentPage = 0;
                //   await Task.Run(() => LoadNextBatch());
            }
            catch (AggregateException ex)
            {
                ex.DebugLog("At Account Updation");
            }
            catch (Exception ex)
            {
                ex.DebugLog("At Account Updation");
            }
            finally
            {
                GlobusLogHelper.log.Info($"Accounts Updated Successfully!");
                //await BrowserProcessTracker.KillAllAsync();
            }
            #endregion
        }

        public void MultipleBusinessAccoutSwitch ( DominatorAccountModel account, string accountType,
            IAccountUpdateFactory accountFactory )
        {
            if (accountFactory is IAccountUpdateAccountTypeFactoryAsync)
            {
                // this account supports async modules
                var asyncAccount = (IAccountUpdateFactoryAsync)accountFactory;

                var asyncBusinessAccount = (IAccountUpdateAccountTypeFactoryAsync)accountFactory;

                var businessAccountCancellationTRoken = new CancellationToken();


                var updateAccount = new Task(async () =>
                {
                    try
                    {
                        _updateAccountList.Add(account.UserName);

                        account.Token.ThrowIfCancellationRequested();

                        account.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                        var checkResult =
                            await asyncAccount.CheckStatusAsync(account, businessAccountCancellationTRoken);

                        if (checkResult)
                            await asyncBusinessAccount.SwitchToBusinessAccountAsync(account,
                                businessAccountCancellationTRoken, accountType == "NormalAccount" ? false : true);
                    }
                    catch (OperationCanceledException ex)
                    {
                        ex.DebugLog("Cancellation Requested!");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                    finally
                    {
                        _updateAccountList.Remove(account.UserName);
                    }
                }, account.Token);
                updateAccount.Start();
            }
        }


        public async Task MultipleUpdate(DominatorAccountModel account, string updateMenuItem,
            IAccountUpdateFactory accountFactory)
        {
            if (accountFactory is IAccountUpdateFactoryAsync)
            {
                // this account supports async modules
                var asyncAccount = (IAccountUpdateFactoryAsync)accountFactory;
                if (updateMenuItem == "UpdateAllDetail")
                {
                    if (account.Token.IsCancellationRequested)
                        account.CancellationSource = new CancellationTokenSource();
                    try
                    {
                        _updateAccountList.Add(account.UserName);

                        account.Token.ThrowIfCancellationRequested();

                        account.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                        account.AccountBaseModel.NeedToCloseBrowser = false;
                        var ProxyWorking = await IsProxyWorkingAsync(account?.AccountBaseModel?.AccountProxy?.ProxyIp,
                            account?.AccountBaseModel?.AccountProxy?.ProxyPort, account?.AccountBaseModel?.AccountProxy?.ProxyUsername,
                            account?.AccountBaseModel?.AccountProxy?.ProxyPassword);
                        if (!ProxyWorking)
                        {
                            account.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                            return;
                        }
                        var checkResult = await asyncAccount.CheckStatusAsync(account, account.Token);
                        if (checkResult)
                        {
                            var runningActivityManager =
                                InstanceProvider.GetInstance<IRunningActivityManager>();
                            runningActivityManager.ScheduleIfAccountGotSucess(account);
                            account.Token.ThrowIfCancellationRequested();

                            await asyncAccount.UpdateDetailsAsync(account, account.Token);

                            SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                                .UpdateLastUpdateTime(DateTimeUtilities.GetEpochTime())
                                .SaveToBinFile();
                            if (account.AccountBaseModel.AccountNetwork != SocialNetworks.Facebook)
                            {
                                var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
                                var accounts = await Task.Run(() => globalDbOperation.UpdateAccountDetails(account));
                            }

                        }
                    }
                    catch (OperationCanceledException ex)
                    {
                        ex.DebugLog("Cancellation Requested!");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                    finally
                    {
                        _updateAccountList.Remove(account.UserName);
                    }


                }
                else if (updateMenuItem == "CheckAccountStatus")
                {
                    var lastStatus = account.AccountBaseModel.Status;
                    account.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                    var ProxyWorking = await IsProxyWorkingAsync(account?.AccountBaseModel?.AccountProxy?.ProxyIp,
                            account?.AccountBaseModel?.AccountProxy?.ProxyPort, account?.AccountBaseModel?.AccountProxy?.ProxyUsername,
                            account?.AccountBaseModel?.AccountProxy?.ProxyPassword);
                    if (!ProxyWorking)
                    {
                        account.AccountBaseModel.Status = AccountStatus.ProxyNotWorking;
                        return;
                    }
                    await asyncAccount.CheckStatusAsync(account, account.Token);

                    if (account.AccountBaseModel.Status == AccountStatus.Success)
                    {
                        var runningActivityManager = InstanceProvider.GetInstance<IRunningActivityManager>();
                        runningActivityManager.ScheduleIfAccountGotSucess(account);
                        //To update proxy status
                        UpdateProxyStatus(account.AccountBaseModel);
                    }
                    else if (account.AccountBaseModel.Status == AccountStatus.TryingToLogin)
                    {
                        account.AccountBaseModel.Status = lastStatus;
                    }
                }
            }
        }

        private void StopAllActivity ( List<DominatorAccountModel> selectedAccounts, bool isNeedToSchedule = false,
            bool activateBrowser = false, bool activateHttp = false )
        {
            ThreadFactory.Instance.Start(() =>
            {
                if (selectedAccounts.Count == 0)
                    return;
                try
                {
                    ToasterNotification.ShowInfomation(
                        $"{"LangKeyAccountActivities".FromResourceDictionary()}\n{string.Format("LangKeyWaitForNSecs".FromResourceDictionary(), 10)}");

                    IsProgressActive = true;

                    selectedAccounts.ForEach(account =>
                    {
                        var jobActivityConfigurationManager =
                            InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
                        var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                        jobActivityConfigurationManager[account.AccountId].ToList().ForEach(x =>
                        {
                            if (x.IsEnabled)
                            {
                                x.IsEnabled = false;
                                dominatorScheduler.StopActivity(account, x.ActivityType.ToString(), x.TemplateId,
                                    isNeedToSchedule);
                            }
                        });

                        if (activateBrowser)
                        {
                            account.IsRunProcessThroughBrowser = true;
                            SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                                .AddOrUpdateBrowserSettings(true)
                                .SaveToBinFile();
                        }
                        else if (activateHttp)
                        {
                            account.IsRunProcessThroughBrowser = false;
                            SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                                .AddOrUpdateBrowserSettings(false)
                                .SaveToBinFile();
                        }

                        // ReSharper disable once ConstantConditionalAccessQualifier
                        account?.NotifyCancelled();

                        GlobusLogHelper.log.Info(Log.StopAllActivitiesOfAccount,
                            account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName);
                    });

                    var BinFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();
                    BinFileHelper.UpdateAllAccounts(LstDominatorAccountModel.ToList());

                    Thread.Sleep(TimeSpan.FromSeconds(10));
                    selectedAccounts.ForEach(x => { x.CancellationSource = new CancellationTokenSource(); });
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                finally
                {
                    IsProgressActive = false;
                }
            });
        }

        private void StopProcess ( List<DominatorAccountModel> selectedAccounts )
        {
            ThreadFactory.Instance.Start(() =>
            {
                if (selectedAccounts.Count == 0)
                    return;
                try
                {
                    ToasterNotification.ShowInfomation(
                        $"{"LangKeyAccountActivities".FromResourceDictionary()}\n{string.Format("LangKeyWaitForNSecs".FromResourceDictionary(), 10)}");

                    IsProgressActive = true;

                    selectedAccounts.ForEach(accountSelected =>
                    {
                        var accountFullDetails =
                            LstDominatorAccountModel.FirstOrDefault(x => x.UserName == accountSelected.UserName);

                        accountFullDetails?.NotifyCancelled();

                        if (accountFullDetails != null)
                            GlobusLogHelper.log.Info(Log.StopUpdatingAccount,
                                accountFullDetails.AccountBaseModel.AccountNetwork,
                                accountFullDetails.AccountBaseModel.UserName);
                    });
                    _updateAccountList.Clear();

                    Thread.Sleep(TimeSpan.FromSeconds(10));
                    selectedAccounts.ForEach(x => { x.CancellationSource = new CancellationTokenSource(); });
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                finally
                {
                    IsProgressActive = false;
                }
            });
        }

        private bool UpdateAccountDetailsCanExecute ( object sender )
        {
            return true;
        }

        #endregion
    }


    public class GridViewHeader : BindableBase
    {
        private string _headers;

        private bool _headerVisible;

        public string Header
        {
            get => _headers;
            set
            {
                if (_headers != null && _headers == value)
                    return;
                SetProperty(ref _headers, value);
            }
        }

        public bool HeaderVisible
        {
            get => _headerVisible;
            set
            {
                if (_headerVisible == value)
                    return;
                SetProperty(ref _headerVisible, value);
            }
        }
    }
}