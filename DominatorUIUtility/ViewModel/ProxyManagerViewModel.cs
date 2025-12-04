using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.ProxyServerManagment;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel
{
    public interface IProxyManagerViewModel : ITabViewModel
    {
        ObservableCollection<ProxyManagerModel> LstProxyManagerModel { get; }
        ObservableCollection<AccountAssign> AccountsAlreadyAssigned { get; }
        DataGrid ProxyDataGrid { get; set; }
        bool UpdateProxy(DominatorAccountBaseModel objDominatorAccountBaseModel, AccessorStrategies strategy);

        void UpdateProxy(DominatorAccountBaseModel objAccountBaseModel, List<ProxyManagerModel> ProxyDetail,
            AccessorStrategies strategy);

        bool IsProxyAvailable(DominatorAccountBaseModel objAccountBaseModel, List<ProxyManagerModel> oldProxies,
            DominatorAccountBaseModel oldAccount, AccessorStrategies strategy);

        void AddProxyIfNotExist(DominatorAccountBaseModel objAccount, AccessorStrategies strategyPack);
    }

    public class ProxyManagerViewModel : BaseTabViewModel, IProxyManagerViewModel
    {
        public ProxyManagerViewModel(IMainViewModel mainViewModel, IVerifyProxiesViewModel verifyProxiesViewModel,
            IProxyServerParserService proxyServerParserService, IAccountsFileManager accountsFileManager,
            IAccountCollectionViewModel accountCollectionViewModel, IProxyFileManager proxyFileManager) : base(
            "LangKeyProxyManager", "ProxyManagerControlTemplate")
        {
            _mainViewModel = mainViewModel;
            VerifyProxiesViewModel = verifyProxiesViewModel;
            _proxyServerParserService = proxyServerParserService;
            _accountsFileManager = accountsFileManager;
            _accountCollectionViewModel = accountCollectionViewModel;
            _proxyFileManager = proxyFileManager;
            SettingsModel = _proxyFileManager.GetProxyManagerSettings();
            AddProxyCommand = new DelegateCommand(AddProxyExecute);
            ImportProxyCommand = new DelegateCommand(ImportProxyExecute);
            GroupTextChangedCommand = new BaseCommand<object>(sender=>true, GroupTextChangedCommandExecute);
            ShowByGroupCommand = new DelegateCommand<object>(ShowByGroupExecute);
            ExportProxyCommand = new DelegateCommand(ExportProxyExecute);
            DeleteCommand = new DelegateCommand<ProxyManagerModel>(DeleteExecute);
            SelectProxyCommand = new DelegateCommand(SelectProxyExecute);
            UpdateProxyCommand = new DelegateCommand<ProxyManagerModel>(UpdateProxyExecute);
            RefreshProxyCommand = new DelegateCommand(RefreshProxyExecute);
            SaveSettingCommand = new DelegateCommand(SaveSettingsExecute);
            VerifyProxyCommand = new DelegateCommand<ProxyManagerModel>(VerifyProxyExecute);
            RemoveAccountFromProxyCommand = new DelegateCommand<AccountAssign>(RemoveAccountFromProxyExecute);
            AccountToAddToProxyCommand = new DelegateCommand<object>(AccountToAddToProxyExecute);
            DropDownCommand = new BaseCommand<object>(DropDownCanExecute, DropDownExecute);
            AssignRandomProxyCommand = new DelegateCommand<object>(AssignRandomProxyExecute);
            LstProxyManagerModel = new ObservableCollection<ProxyManagerModel>();
            AccountsAlreadyAssigned = new ObservableCollection<AccountAssign>();
            Groups = new ObservableCollection<string>();
            BindingOperations.EnableCollectionSynchronization(LstProxyManagerModel, _lock);
            BindingOperations.EnableCollectionSynchronization(Groups, _lock);
            StartAddingItems();
        }

        private void GroupTextChangedCommandExecute(object param)
        {
            dynamic Param = param;
            var args = Param.EventArgs;
            var Item = Param.Row;
            if (args is KeyEventArgs ke)
            {
                if (ke.Key != Key.Enter)
                    return;  // ignore other keys
                if(Item is ProxyManagerModel model)
                {
                    _proxyFileManager.EditProxy(model);
                    AddGroup(model);
                }
            }
        }

        public DataGrid ProxyDataGrid { get; set; }


        private void StartAddingItems()
        {
            ThreadFactory.Instance.Start(() =>
            {
                try
                {
                    _proxyFileManager.GetAllProxy().ForEach(proxy =>
                    {
                        lock (_lock)
                        {
                            if (Application.Current.Dispatcher.CheckAccess())
                                Application.Current.Dispatcher.InvokeAsync(() => { AddToModelAndToGroup(proxy); });
                            else
                                AddToModelAndToGroup(proxy);

                            proxy.AccountsAssignedto.ForEach(x =>
                            {
                                if (AccountsAlreadyAssigned != null && !AccountsAlreadyAssigned.Any(y => y.UserName == x.UserName && y.AccountNetwork == x.AccountNetwork))
                                    AccountsAlreadyAssigned.Add(new AccountAssign
                                    {
                                        UserName = x.UserName,
                                        AccountNetwork = x.AccountNetwork
                                    });
                            });
                        }
                    });
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
        }

        private void AddToModelAndToGroup(ProxyManagerModel proxy)
        {
            if (LstProxyManagerModel != null && !LstProxyManagerModel.Any(x => x.AccountProxy.ProxyIp == proxy.AccountProxy.ProxyIp && x.AccountProxy.ProxyPort == proxy.AccountProxy.ProxyPort))
                LstProxyManagerModel.Add(proxy);
            AddGroup(proxy);
            proxy.Index = LstProxyManagerModel.IndexOf(proxy) + 1;
        }

        private void SelectAllProxies(bool isAllProxySelected)
        {
            if (IsUnCheckedFromList)
                return;
            var view = (CollectionView)CollectionViewSource.GetDefaultView(ProxyDataGrid.ItemsSource);
            var visibleProxy = view.SourceCollection.Cast<ProxyManagerModel>();
            ThreadFactory.Instance.Start(() =>
            {
                LstProxyManagerModel.ForEach(proxy =>
                {
                    if (visibleProxy.Any(x => x.AccountProxy.ProxyId == proxy.AccountProxy.ProxyId))
                        proxy.IsProxySelected = isAllProxySelected;
                });
            });
        }

        private void AddProxyExecute()
        {
            var objAddProxyControl = new AddOrUpdateProxyControl(this);
            ProxyManagerModel = new ProxyManagerModel();
            try
            {
                IsShowByGroup = false;
                var dialog = new Dialog();
                var window = dialog.GetMetroWindow(objAddProxyControl,
                    Application.Current.FindResource("LangKeyAddProxy").ToString());
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void UpdateAccountsToBeAssign(ProxyManagerModel proxyManagerModel)
        {
            _accountsFileManager.GetAll()?.ForEach(account =>
            {
                if (!proxyManagerModel.AccountsToBeAssign.Any(acc =>
                    acc.AccountNetwork == account.AccountBaseModel.AccountNetwork && acc.UserName == account.UserName))
                    proxyManagerModel.AccountsToBeAssign.Add(new AccountAssign
                    {
                        UserName = account.UserName,
                        AccountNetwork = account.AccountBaseModel.AccountNetwork
                    });
            });
        }

        private void ImportProxyExecute()
        {
            var loadedProxylist = FileUtilities.FileBrowseAndReader();
            if (loadedProxylist == null)
                return;

            IsShowByGroup = false;
            var noOfExistingProxies = 0;
            var noOfProxyAdded = 0;

            ThreadFactory.Instance.Start(() =>
            {
                var parsingResult = _proxyServerParserService.ParseProxies(loadedProxylist);
                foreach (var givenProxy in parsingResult.Proxies)
                    try
                    {
                        if (Proxy.IsLuminatiProxy(givenProxy.AccountProxy.ProxyIp)
                        && !Proxy.IsValidProxy(givenProxy.AccountProxy.ProxyIp, givenProxy.AccountProxy.ProxyPort))
                            continue;

                        var existingProxy = LstProxyManagerModel.Where(x =>
                            x.AccountProxy.ProxyIp == givenProxy.AccountProxy.ProxyIp
                            && x.AccountProxy.ProxyPort == givenProxy.AccountProxy.ProxyPort
                            && x.AccountProxy.ProxyUsername == givenProxy.AccountProxy.ProxyUsername
                            && x.AccountProxy.ProxyPassword == givenProxy.AccountProxy.ProxyPassword);

                        if (existingProxy != null && existingProxy.Count() != 0)
                        {
                            if (Proxy.IsLuminatiProxy(givenProxy.AccountProxy.ProxyIp))
                            {
                                if (existingProxy.Any(x =>
                                x.AccountProxy.ProxyIp == givenProxy.AccountProxy.ProxyIp
                                && x.AccountProxy.ProxyPort == givenProxy.AccountProxy.ProxyPort
                                    && x.AccountProxy.ProxyUsername == givenProxy.AccountProxy.ProxyUsername
                                    && x.AccountProxy.ProxyPassword == givenProxy.AccountProxy.ProxyPassword))
                                {
                                    noOfExistingProxies++;
                                    continue;
                                }
                            }
                            else
                            {
                                noOfExistingProxies++;
                                continue;
                            }
                        }

                        if (string.IsNullOrEmpty(givenProxy.AccountProxy.ProxyGroup))
                            givenProxy.AccountProxy.ProxyGroup = ConstantVariable.UnGrouped();

                        if (string.IsNullOrEmpty(givenProxy.AccountProxy.ProxyName))
                            givenProxy.AccountProxy.ProxyName = "Proxy " + LstProxyManagerModel.Count + 1;

                        UpdateAccountsToBeAssign(givenProxy);
                        _proxyFileManager.SaveProxy(givenProxy);

                        LstProxyManagerModel.Add(givenProxy);
                        AddGroup(givenProxy);

                        givenProxy.Index = LstProxyManagerModel.IndexOf(givenProxy) + 1;
                        noOfProxyAdded++;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(ex);
                    }

                #region Update Import Proxies Status in Logger

                if (noOfExistingProxies > 0)
                    GlobusLogHelper.log.Info(SocialNetworks.Social + "\t" +
                                             string.Format("LangKeySkippedExistingProxies".FromResourceDictionary(),
                                                 noOfExistingProxies));
                if (noOfProxyAdded > 0)
                    GlobusLogHelper.log.Info(SocialNetworks.Social + "\t" +
                                                 string.Format("LangKeyAddedProxies".FromResourceDictionary(),
                                                     noOfProxyAdded));
                if (parsingResult.InvalidProxies.Any())
                {
                    var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Socinator";
                    DirectoryUtilities.CreateDirectory(path);
                    var filename = $"{path}\\invalidProxies {ConstantVariable.DateasFileName}.txt";
                    using (var streamWriter = new StreamWriter(filename, true))
                    {
                        parsingResult.InvalidProxies.ForEach(p => { streamWriter.WriteLine(p); });
                    }

                    GlobusLogHelper.log.Info(SocialNetworks.Social + "\t" +
                                             string.Format(
                                                 "LangKeySkippedProxiesAsItDoesntMatchImportFormat"
                                                     .FromResourceDictionary(), parsingResult.InvalidProxies.Count,
                                                 filename));
                }

                #endregion
            });
        }

        public void AddGroup(ProxyManagerModel givenProxy)
        {
            if (!Groups.Any(x =>
                x.Equals(givenProxy.AccountProxy.ProxyGroup, StringComparison.InvariantCultureIgnoreCase)))
                Groups.Add(givenProxy.AccountProxy.ProxyGroup);
        }

        private void ExportProxyExecute()
        {
            var proxiesToExport = LstProxyManagerModel.Where(proxy => proxy.IsProxySelected).ToList();
            if (!proxiesToExport.Any())
                proxiesToExport = _proxyFileManager.GetAllProxy().Where(proxy => proxy.IsProxySelected).ToList();
            if (!proxiesToExport.Any())
            {
                Dialog.ShowDialog("LangKeyAlert".FromResourceDictionary(),
                            "Please Select Atleast One Proxy To Export!");
                return;
            }
            ExportProxies(proxiesToExport);
        }

        private void ExportProxies(List<ProxyManagerModel> Proxies)
        {
            var exportPath = FileUtilities.GetExportPath();

            if (string.IsNullOrEmpty(exportPath))
                return;

            var header = "Proxy Group,Proxy name,Proxy IP,Proxy Port,Proxy Username,Proxy Password,Status";

            var filename = $"{exportPath}\\Proxies {ConstantVariable.DateasFileName}.csv";

            try
            {
                if (!File.Exists(filename))
                    using (var streamWriter = new StreamWriter(filename, true))
                    {
                        streamWriter.WriteLine(header);
                    }

                Proxies.ForEach(proxy =>
                {
                    try
                    {
                        var csvData =
                            $"{proxy.AccountProxy.ProxyGroup},{proxy.AccountProxy.ProxyName},{proxy.AccountProxy.ProxyIp},{proxy.AccountProxy.ProxyPort},{proxy.AccountProxy.ProxyUsername},{proxy.AccountProxy.ProxyPassword},{proxy.Status}";

                        using (var streamWriter = new StreamWriter(filename, true))
                        {
                            streamWriter.WriteLine(csvData);
                            GlobusLogHelper.log.Info(Log.Exported, SocialNetworks.Social,
                                proxy.AccountProxy.ProxyIp + " : " + proxy.AccountProxy.ProxyPort,
                                "LangKeyProxy".FromResourceDictionary());
                        }
                    }
                    catch
                    {
                        GlobusLogHelper.log.Error("LangKeyErrorInExportProxies".FromResourceDictionary());
                    }
                });
                Dialog.ShowDialog(Application.Current.MainWindow,
                    "LangKeySuccess".FromResourceDictionary(),
                    string.Format("LangKeyExportedProxiesTo".FromResourceDictionary(), filename));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ShowByGroupExecute(object isChecked)
        {
            try
            {
                lock (_lock)
                {
                    ProxyDataGrid = isChecked as DataGrid;
                    ApplyGrouping();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ApplyGrouping()
        {
            try
            {
                var view = (CollectionView)CollectionViewSource.GetDefaultView(ProxyDataGrid?.ItemsSource);
                var groupDescription = new PropertyGroupDescription("AccountProxy.ProxyGroup");
                view?.GroupDescriptions?.Clear();
                if (IsShowByGroup)
                    view.GroupDescriptions.Add(groupDescription);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteExecute(ProxyManagerModel sender)
        {
            try
            {
                #region Delete Selected proxy

                if (sender == null)
                {
                    var selectedProxies = LstProxyManagerModel.Where(proxy => proxy.IsProxySelected).ToList();
                    if (selectedProxies.Count == 0)
                    {
                        Dialog.ShowDialog("LangKeyAlert".FromResourceDictionary(),
                            "LangKeySelectAtLeastOneProxy".FromResourceDictionary());
                        return;
                    }

                    if (ShowWarningMessage() == MessageDialogResult.Affirmative)
                    {
                        Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            selectedProxies.ForEach(selectedProxy =>
                            {
                                RemoveProxy(selectedProxy);
                                Thread.Sleep(50);
                            });
                            Dialog.ShowDialog("LangKeySuccess".FromResourceDictionary(),
                                string.Format("LangKeyNProxiesDeleted".FromResourceDictionary(),
                                    selectedProxies.Count));
                            GlobusLogHelper.log.Info(Log.Deleted, SocialNetworks.Social,
                                $"{selectedProxies.Count} {"LangKeyProxies".FromResourceDictionary()}",
                                "LangKeyProxy".FromResourceDictionary());
                        });

                        IsAllProxySelected = false;
                    }
                }

                #endregion

                #region Delete current proxy

                else
                {
                    if (ShowWarningMessage() == MessageDialogResult.Affirmative)
                    {
                        RemoveProxy(sender);
                        Application.Current.Dispatcher.Invoke(() => Dialog.ShowDialog(
                            "LangKeySuccess".FromResourceDictionary(),
                            $"{sender.AccountProxy.ProxyIp}:{sender.AccountProxy.ProxyPort} {"LangKeySuccessfullyDeleted".FromResourceDictionary()}"));
                    }
                }

                IsShowByGroup = false;

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private MessageDialogResult ShowWarningMessage()
        {
            return Dialog.ShowCustomDialog("LangKeyWarning".FromResourceDictionary(),
                "LangKeyProxyWillbeRemovedFromAccount".FromResourceDictionary(), "LangKeyYes".FromResourceDictionary(),
                "LangKeyNo".FromResourceDictionary());
        }

        private void RemoveProxy(ProxyManagerModel selectedProxy)
        {
            try
            {
                _proxyFileManager.Delete(proxy => proxy.AccountProxy.ProxyId == selectedProxy.AccountProxy.ProxyId);
                RemoveProxyFromAccount(selectedProxy);

                LstProxyManagerModel.Remove(selectedProxy);
                Groups.Remove(selectedProxy.AccountProxy.ProxyGroup);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void RemoveProxyFromAccount(ProxyManagerModel selectedProxy)
        {
            try
            {
                _accountCollectionViewModel.GetCopySync().Where(account =>
                    account.AccountBaseModel.AccountProxy.ProxyIp == selectedProxy.AccountProxy.ProxyIp
                    && account.AccountBaseModel.AccountProxy.ProxyPort == selectedProxy.AccountProxy.ProxyPort).ForEach(
                    account =>
                    {
                        AccountsAlreadyAssigned.Remove(AccountsAlreadyAssigned.FirstOrDefault(x =>
                            x.UserName == account.UserName &&
                            x.AccountNetwork == account.AccountBaseModel.AccountNetwork));

                        account.AccountBaseModel.AccountProxy = new Proxy();

                        if (account.AccountBaseModel.Status == AccountStatus.ProxyNotWorking)
                            account.AccountBaseModel.Status = AccountStatus.NotChecked;

                        SocinatorAccountBuilder.Instance(account.AccountBaseModel.AccountId)
                            .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                            .SaveToBinFile();
                    });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SelectProxyExecute()
        {
            if (LstProxyManagerModel.All(x => x.IsProxySelected))
            {
                IsAllProxySelected = true;
            }
            else
            {
                if (IsAllProxySelected)
                    IsUnCheckedFromList = true;
                IsAllProxySelected = false;
            }
        }

        private void UpdateProxyExecute(ProxyManagerModel currentProxy)
        {
            var oldProxy = _proxyFileManager.GetProxyById(currentProxy.AccountProxy.ProxyId);
            if (!Proxy.IsValidProxy(currentProxy.AccountProxy.ProxyIp, currentProxy.AccountProxy.ProxyPort))
            {
                Dialog.ShowDialog(Application.Current.MainWindow,
                    "LangKeyWarning".FromResourceDictionary(),
                    "LangKeyPleaseEnterValidProxy".FromResourceDictionary());
                return;
            }

            var isAvailable = false;
            LstProxyManagerModel.ForEach(proxy =>
            {
                if (proxy.AccountProxy.ProxyId != currentProxy.AccountProxy.ProxyId)
                {
                    if (proxy.AccountProxy.ProxyName == currentProxy.AccountProxy.ProxyName)
                    {
                        Dialog.ShowDialog(Application.Current.MainWindow,
                            "LangKeyWarning".FromResourceDictionary(),
                            string.Format("LangKeyThisProxyAlreadyExist".FromResourceDictionary(),
                                currentProxy.AccountProxy.ProxyName));
                        currentProxy.AccountProxy.ProxyName = oldProxy.AccountProxy.ProxyName;
                        isAvailable = true;
                    }

                    else if (proxy.AccountProxy.ProxyIp == currentProxy.AccountProxy.ProxyIp &&
                             proxy.AccountProxy.ProxyPort == currentProxy.AccountProxy.ProxyPort)
                    {
                        Dialog.ShowDialog(Application.Current.MainWindow,
                            "LangKeyWarning".FromResourceDictionary(),
                            string.Format("LangKeyProxyWithIpPortAlreadyExist".FromResourceDictionary(),
                                currentProxy.AccountProxy.ProxyIp, currentProxy.AccountProxy.ProxyPort));
                        currentProxy.AccountProxy = oldProxy.AccountProxy;
                        isAvailable = true;
                        // ReSharper disable once RedundantJumpStatement
                        return;
                    }
                }
            });
            if (isAvailable)
                return;

            var accountToUpdateProxy
                = _accountsFileManager.GetAll().Where(x =>
                    x.AccountBaseModel.AccountProxy.ProxyIp == oldProxy.AccountProxy.ProxyIp
                    && x.AccountBaseModel.AccountProxy.ProxyPort == oldProxy.AccountProxy.ProxyPort).ToList();

            oldProxy = currentProxy;

            _proxyFileManager.EditProxy(oldProxy);

            accountToUpdateProxy.ForEach(acc =>
            {
                acc.AccountBaseModel.AccountProxy = oldProxy.AccountProxy;

                SocinatorAccountBuilder.Instance(acc.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(acc.AccountBaseModel)
                    .SaveToBinFile();

                var accountToUpdate =
                    _accountsFileManager.GetAccount(acc.UserName, acc.AccountBaseModel.AccountNetwork);
                UpdateAccountsProxy(accountToUpdate);
            });

            ToasterNotification.ShowSuccess(string.Format("LangKeyIpPortUpdated".FromResourceDictionary(),
                oldProxy.AccountProxy.ProxyIp, oldProxy.AccountProxy.ProxyPort));
        }

        private async void VerifyProxyExecute(ProxyManagerModel currentProxyManager)
        {
            var urlToVerify = SettingsModel?.VerificationUrl ?? "https://www.google.com";
            if (currentProxyManager != null)
            {
                await VerifyProxiesViewModel.Verify(urlToVerify,currentProxyManager);
                return;
            }
            var proxies = LstProxyManagerModel.Where(proxy => proxy.IsProxySelected).ToArray();
            if (!proxies.Any())
            {
                Dialog.ShowDialog("LangKeyAlert".FromResourceDictionary(),
                            "Please Select Proxy To Verify");
                return;
            }
            await VerifyProxiesViewModel.Verify(urlToVerify, proxies);
        }

        private async void RefreshProxyExecute()
        {
            RefreshEnable = Visibility.Hidden;
            await ThreadFactory.Instance.Start(() =>
            {
                var accnts = _accountsFileManager.GetAll()
                    .Where(x => !string.IsNullOrEmpty(x.AccountBaseModel.AccountProxy.ProxyIp)).ToList();
                accnts.ForEach(account =>
                {
                    try
                    {
                        if (!LstProxyManagerModel.Any(px =>
                            px.AccountProxy.ProxyIp.Trim() == account.AccountBaseModel.AccountProxy.ProxyIp.Trim()))
                            AddProxyFromsAccountsIfNotExist(account.AccountBaseModel, accnts);
                        else
                            UpdateExisting(account.AccountBaseModel);
                    }
                    catch (Exception)
                    {
                    }
                });

                _proxyFileManager.EditAllProxy(LstProxyManagerModel.ToList());
                RefreshEnable = Visibility.Visible;
            });
        }

        private void SaveSettingsExecute()
        {
            if (_proxyFileManager.SaveProxyManagerSettings(SettingsModel))
                ToasterNotification.ShowSuccess("LangKeyProxySettingsSavedSuccessfully".FromResourceDictionary());
            else
                ToasterNotification.ShowError("LangKeyOopsAnErrorOccured".FromResourceDictionary());
        }

        private void AddProxyFromsAccountsIfNotExist(DominatorAccountBaseModel objAccount,
            List<DominatorAccountModel> accounts)
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
                    ProxyPassword = objAccount.AccountProxy.ProxyPassword
                },
                Status = objAccount.Status != AccountStatus.Success
                    ? objAccount.Status == AccountStatus.ProxyNotWorking ? "Not Working" : "Not Checked"
                    : "Working"
            };

            #region remove account from AccountsAssignedto if any proxy having account

            LstProxyManagerModel.ForEach(proxy =>
            {
                accounts.ForEach(acc =>
                {
                    if (acc.AccountBaseModel.AccountProxy.ProxyIp.Trim() != proxy.AccountProxy.ProxyIp.Trim()
                        && proxy.AccountsAssignedto.Any(x =>
                            x.UserName == acc.UserName && x.AccountNetwork == acc.AccountBaseModel.AccountNetwork))
                        proxy.AccountsAssignedto.Remove(proxy.AccountsAssignedto.FirstOrDefault(x => x.UserName ==
                                                                                                     acc.UserName
                                                                                                     &&
                                                                                                     x.AccountNetwork ==
                                                                                                     acc
                                                                                                         .AccountBaseModel
                                                                                                         .AccountNetwork));
                });
            });

            #endregion

            if (Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(() => { AddProxyFromAccount(objAccount, ProxyManagerModel); });
            else
                AddProxyFromAccount(objAccount, ProxyManagerModel);

            ProxyManagerModel.AccountsAssignedto.Add(new AccountAssign
            {
                UserName = objAccount.UserName,
                AccountNetwork = objAccount.AccountNetwork
            });
        }

        public async Task UpdateExisting(DominatorAccountBaseModel objDominatorAccountBaseModel)
        {
            foreach (var proxy in LstProxyManagerModel)
            {
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
                            await _proxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
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

                            await _proxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
                        }

                        break;
                    }

                    #endregion
                }

                #endregion
            }
        }

        private void RemoveAccountFromProxyExecute(AccountAssign account)
        {
            var proxy = _proxyFileManager.GetAllProxy().FirstOrDefault(x =>
                x.AccountProxy.ProxyIp == ProxyManagerModel.AccountProxy.ProxyIp
                && x.AccountProxy.ProxyPort == ProxyManagerModel.AccountProxy.ProxyPort);


            //var proxy = ProxyFileManager.GetProxyById(CurrentProxyManagerModel.AccountProxy.ProxyId);
            var accountToDelete = proxy.AccountsAssignedto.FirstOrDefault(x => x.UserName == account.UserName
                                                                               && x.AccountNetwork ==
                                                                               account.AccountNetwork);
            try
            {
                proxy.AccountsAssignedto.Remove(accountToDelete);

                var item = LstProxyManagerModel.FirstOrDefault(Proxy =>
                    Proxy.AccountProxy.ProxyIp == proxy.AccountProxy.ProxyIp &&
                    Proxy.AccountProxy.ProxyPort == proxy.AccountProxy.ProxyPort);
                var indexToUpdate = LstProxyManagerModel.IndexOf(item);
                LstProxyManagerModel[indexToUpdate].AccountsAssignedto = proxy.AccountsAssignedto;
                // LstProxyManagerModel[indexToUpdate].AccountsToBeAssign.Add(accountToDelete);

                var accountToDeleteProxy = _accountsFileManager.GetAccount(account.UserName, account.AccountNetwork);
                accountToDeleteProxy.AccountBaseModel.AccountProxy = new Proxy();

                // AccountsFileManager.Edit(accountToDeleteProxy);

                if (accountToDeleteProxy.AccountBaseModel.Status == AccountStatus.ProxyNotWorking)
                    accountToDeleteProxy.AccountBaseModel.Status = AccountStatus.NotChecked;
                var socinatorAccountBuilder =
                    SocinatorAccountBuilder.Instance(accountToDeleteProxy.AccountBaseModel.AccountId);
                    socinatorAccountBuilder.AddOrUpdateDominatorAccountBase(accountToDeleteProxy.AccountBaseModel)
                    .SaveToBinFile();

                UpdateAccountsProxy(accountToDeleteProxy);
                if (!SettingsModel.DontLogin)
                    StartLogin(accountToDeleteProxy, socinatorAccountBuilder);
                LstProxyManagerModel.ForEach(oldProxy =>
                    AccountsAlreadyAssigned.Remove(AccountsAlreadyAssigned.FirstOrDefault(x =>
                        x.UserName == accountToDelete.UserName && x.AccountNetwork == accountToDelete.AccountNetwork))
                );
                _proxyFileManager.EditAllProxy(LstProxyManagerModel.ToList());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AccountToAddToProxyExecute(object sender)
        {
            var account = sender as AccountAssign;

            try
            {
                var methodCallBy = new StackTrace().GetFrame(1).GetMethod().Name;
                if (methodCallBy == "Execute")
                    if (SettingsModel.IsNumOfAccountPerProxy && ProxyManagerModel.AccountsAssignedto.Count >=
                        SettingsModel.NumOfAccountPerProxy)
                    {
                        ToasterNotification.ShowInfomation(string.Format(
                            "LangKeyCantaddAccountToProxyAddingLimitReached".FromResourceDictionary(),
                            SettingsModel.NumOfAccountPerProxy));
                        return;
                    }

                var accountToUpdateProxy = _accountsFileManager.GetAccount(account.UserName, account.AccountNetwork);

                if (methodCallBy == "Execute" && !string.IsNullOrWhiteSpace(accountToUpdateProxy.AccountBaseModel
                                                  .AccountProxy.ProxyIp)
                                              && MatchedProxyWithFromList(accountToUpdateProxy.AccountBaseModel
                                                  .AccountProxy)?.Status == "Working")
                {
                    var proceed = Dialog.ShowCustomDialog("LangKeyConfirmation".FromResourceDictionary(),
                                      "LangKeyAccountHasWorkingProxyStillWannaReplace".FromResourceDictionary(),
                                      "LangKeyYes".FromResourceDictionary(), "LangKeyNo".FromResourceDictionary()) ==
                                  MessageDialogResult.Affirmative;
                    if (!proceed)
                        return;
                }

                var accountToAdd = ProxyManagerModel.AccountsToBeAssign.FirstOrDefault(x =>
                    x.UserName == account.UserName && x.AccountNetwork == account.AccountNetwork);
                LstProxyManagerModel.ForEach(proxy =>
                {
                    proxy.AccountsToBeAssign.Remove(proxy.AccountsToBeAssign.FirstOrDefault(x =>
                        x.UserName == accountToAdd.UserName
                        && x.AccountNetwork == accountToAdd.AccountNetwork));
                });
                if (!ProxyManagerModel.AccountsAssignedto.Any(x => x.UserName == account.UserName
                                                                   && x.AccountNetwork == account.AccountNetwork))
                {
                    ProxyManagerModel.AccountsAssignedto.Add(accountToAdd);
                    AccountsAlreadyAssigned.Add(accountToAdd);
                }

                _proxyFileManager.EditProxy(ProxyManagerModel);

                var proxyToAdd = new Proxy
                {
                    ProxyId = ProxyManagerModel.AccountProxy.ProxyId,
                    ProxyPort = ProxyManagerModel.AccountProxy.ProxyPort,
                    ProxyIp = ProxyManagerModel.AccountProxy.ProxyIp,
                    ProxyUsername = ProxyManagerModel.AccountProxy.ProxyUsername,
                    ProxyPassword = ProxyManagerModel.AccountProxy.ProxyPassword
                };
                accountToUpdateProxy.AccountBaseModel.AccountProxy = proxyToAdd;

                if (accountToUpdateProxy.AccountBaseModel.Status == AccountStatus.ProxyNotWorking)
                    accountToUpdateProxy.AccountBaseModel.Status = AccountStatus.NotChecked;
                var socinatorAccountBuilder =
                    SocinatorAccountBuilder.Instance(accountToUpdateProxy.AccountBaseModel.AccountId);
                socinatorAccountBuilder.AddOrUpdateDominatorAccountBase(accountToUpdateProxy.AccountBaseModel)
                    .SaveToBinFile();
                UpdateAccountsProxy(accountToUpdateProxy);
                if (!SettingsModel.DontLogin)
                    StartLogin(accountToUpdateProxy, socinatorAccountBuilder);

                var item = LstProxyManagerModel.FirstOrDefault(proxy =>
                    proxy.AccountProxy.ProxyName == ProxyManagerModel.AccountProxy.ProxyName);
                var indexToUpdate = LstProxyManagerModel.IndexOf(item);
                LstProxyManagerModel[indexToUpdate].AccountsAssignedto = ProxyManagerModel.AccountsAssignedto;
                LstProxyManagerModel[indexToUpdate].AccountsToBeAssign = ProxyManagerModel.AccountsToBeAssign;


                _proxyFileManager.EditAllProxy(LstProxyManagerModel.ToList());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void StartLogin(DominatorAccountModel accountToUpdateProxy,
            SocinatorAccountBuilder SocinatorAccountBuilder)
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    var networkCoreFactory = SocinatorInitialize
                        .GetSocialLibrary(accountToUpdateProxy.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory();

                    var asyncAccount = (IAccountUpdateFactoryAsync)networkCoreFactory.AccountUpdateFactory;

                    accountToUpdateProxy.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                    UpdateUserInfoCountToAccountManagerUi(accountToUpdateProxy);
                    await asyncAccount.CheckStatusAsync(accountToUpdateProxy, accountToUpdateProxy.Token);
                    if (accountToUpdateProxy.AccountBaseModel.Status == AccountStatus.Success)
                    {
                        await asyncAccount.UpdateDetailsAsync(accountToUpdateProxy, accountToUpdateProxy.Token);
                        SocinatorAccountBuilder.UpdateLastUpdateTime(DateTimeUtilities.GetEpochTime())
                            .SaveToBinFile();
                    }
                    else
                    {
                        UpdateUserInfoCountToAccountManagerUi(accountToUpdateProxy);

                        SocinatorAccountBuilder
                            .AddOrUpdateDisplayColumn1(accountToUpdateProxy.DisplayColumnValue1)
                            .AddOrUpdateDisplayColumn2(accountToUpdateProxy.DisplayColumnValue2)
                            .AddOrUpdateDisplayColumn3(accountToUpdateProxy.DisplayColumnValue3)
                            .AddOrUpdateDisplayColumn4(accountToUpdateProxy.DisplayColumnValue4)
                            .AddOrUpdateProxy(accountToUpdateProxy.AccountBaseModel.AccountProxy)
                            .AddOrUpdateMailCredentials(accountToUpdateProxy.MailCredentials)
                            .AddOrUpdateIsAutoVerifyByEmail(accountToUpdateProxy.IsAutoVerifyByEmail)
                            .SaveToBinFile();
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
        }

        private void UpdateUserInfoCountToAccountManagerUi(DominatorAccountModel account)
        {
            account.DisplayColumnValue1 = 0;
            account.DisplayColumnValue2 = 0;
            account.DisplayColumnValue3 = 0;
            account.DisplayColumnValue4 = 0;
        }

        private void UpdateAccountsProxy(DominatorAccountModel accountToUpdateProxy)
        {
            try
            {
                var updateAccountsDetails = _accountCollectionViewModel.GetCopySync().FirstOrDefault(x =>
                    x.AccountBaseModel.AccountId == accountToUpdateProxy.AccountBaseModel.AccountId);

                if (updateAccountsDetails != null)
                    updateAccountsDetails.AccountBaseModel.AccountProxy =
                        accountToUpdateProxy.AccountBaseModel.AccountProxy;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool DropDownCanExecute(object sender)
        {
            return true;
        }

        private void DropDownExecute(object sender)
        {
            ProxyManagerModel = sender as ProxyManagerModel;
            AvailableAccountsToBeAssigned(ProxyManagerModel);
        }

        private void AvailableAccountsToBeAssigned(ProxyManagerModel proxyManagerModel)
        {
            try
            {
                proxyManagerModel.AccountsToBeAssign.Clear();

                _accountsFileManager.GetAll()?.ForEach(account =>
                {
                    if (!AccountsAlreadyAssigned.Any(acc =>
                        acc.AccountNetwork == account.AccountBaseModel.AccountNetwork &&
                        acc.UserName == account.UserName))
                        if (!proxyManagerModel.AccountsToBeAssign.Any(x =>
                            x.UserName == account.UserName &&
                            x.AccountNetwork == account.AccountBaseModel.AccountNetwork))
                            proxyManagerModel.AccountsToBeAssign.Add(new AccountAssign
                            {
                                UserName = account.UserName,
                                AccountNetwork = account.AccountBaseModel.AccountNetwork
                            });
                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        private void AssignRandomProxyExecute(object sender)
        {
            try
            {
                var accounts = _accountCollectionViewModel.GetCopySync().Where(x =>
                    string.IsNullOrEmpty(x.AccountBaseModel.AccountProxy.ProxyIp) &&
                    string.IsNullOrEmpty(x.AccountBaseModel.AccountProxy.ProxyPort)).ToList();

                if (LstProxyManagerModel.Count != 0)
                {
                    var conditionalString = SettingsModel.IsNumOfAccountPerProxy
                        ? string.Format("LangKeyConditionAddNAccPerProxy".FromResourceDictionary(),
                            SettingsModel.NumOfAccountPerProxy)
                        : ".";
                    var proceed = Dialog.ShowCustomDialog("LangKeyConfirmation".FromResourceDictionary(),
                                      $"{"LangKeyConfirmToAssignRandProxies".FromResourceDictionary()}{conditionalString}",
                                      "LangKeyYes".FromResourceDictionary(), "LangKeyNo".FromResourceDictionary()) ==
                                  MessageDialogResult.Affirmative;
                    if (!proceed)
                        return;

                    var random = new Random();
                    var randomIndex = 0;
                    foreach (var acc in accounts)
                    {
                        var workingOne = (SettingsModel.IsNumOfAccountPerProxy
                                             ? LstProxyManagerModel.Where(x =>
                                                 x.Status == "Working" && x.AccountsAssignedto.Count <
                                                 SettingsModel.NumOfAccountPerProxy)
                                             : LstProxyManagerModel.Where(x => x.Status == "Working"))?.ToList()
                                         ?? new List<ProxyManagerModel>();

                        if (workingOne.Count == 0)
                        {
                            ToasterNotification.ShowInfomation(
                                $"{"LangKeyNoWorkingProxiesAvailableToAssign".FromResourceDictionary()}{conditionalString}");
                            return;
                        }

                        randomIndex = random.Next(workingOne.Count);

                        if (!string.IsNullOrWhiteSpace(acc.AccountBaseModel.AccountProxy.ProxyIp)
                            && MatchedProxyWithFromList(acc.AccountBaseModel.AccountProxy)?.Status == "Working")
                            continue;

                        SocinatorAccountBuilder.Instance(acc.AccountBaseModel.AccountId)
                            .AddOrUpdateProxy(workingOne[randomIndex].AccountProxy)
                            .SaveToBinFile();
                        var accountAssignTo = new AccountAssign
                        {
                            UserName = acc.UserName,
                            AccountNetwork = acc.AccountBaseModel.AccountNetwork
                        };

                        ProxyManagerModel = workingOne[randomIndex];
                        ProxyManagerModel.AccountsAssignedto.Add(accountAssignTo);
                        AccountsAlreadyAssigned.Add(accountAssignTo);
                        AccountToAddToProxyExecute(accountAssignTo);
                    }
                }
                else
                {
                    ToasterNotification.ShowInfomation("LangKeyNoProxyToAssign".FromResourceDictionary());
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }
            finally
            {
                IsRandomSelected = false;
            }
        }

        private ProxyManagerModel MatchedProxyWithFromList(Proxy proxy)
        {
            return LstProxyManagerModel.FirstOrDefault(x =>
                x.AccountProxy.ProxyIp == proxy.ProxyIp && x.AccountProxy.ProxyPort == proxy.ProxyPort
                                                        && (x.AccountProxy.ProxyUsername ?? "") ==
                                                        (proxy.ProxyUsername ?? "") &&
                                                        (x.AccountProxy.ProxyPassword ?? "") ==
                                                        (proxy.ProxyPassword ?? ""));
        }

        #region Properties

        private readonly IMainViewModel _mainViewModel;
        private readonly IProxyServerParserService _proxyServerParserService;
        private readonly IAccountsFileManager _accountsFileManager;
        private readonly IAccountCollectionViewModel _accountCollectionViewModel;
        private readonly IProxyFileManager _proxyFileManager;
        private static readonly object _lock = new object();
        private bool _isAddProxyEnabled = true;
        private ProxyManagerModel _proxyManagerModel = new ProxyManagerModel();
        private bool _isAllProxySelected;

        public bool IsAddProxyEnabled
        {
            get => _isAddProxyEnabled;
            set => SetProperty(ref _isAddProxyEnabled, value);
        }

        public IVerifyProxiesViewModel VerifyProxiesViewModel { get; }
        public ObservableCollection<ProxyManagerModel> LstProxyManagerModel { get; }
        public ObservableCollection<string> Groups { get; }

        public ObservableCollection<AccountAssign> AccountsAlreadyAssigned { get; }

        public ProxyManagerModel ProxyManagerModel
        {
            get => _proxyManagerModel;
            set => SetProperty(ref _proxyManagerModel, value);
        }

        private bool _isUnCheckedFromList;

        public bool IsUnCheckedFromList
        {
            get => _isUnCheckedFromList;
            set => SetProperty(ref _isUnCheckedFromList, value);
        }


        public bool IsAllProxySelected
        {
            get => _isAllProxySelected;
            set
            {
                if (_isAllProxySelected == value)
                    return;
                SetProperty(ref _isAllProxySelected, value);

                SelectAllProxies(_isAllProxySelected);
                IsUnCheckedFromList = false;
            }
        }

        private ProxyManagerSettings _proxyManagerSettings;

        public ProxyManagerSettings SettingsModel
        {
            get => _proxyManagerSettings;
            set
            {
                SetProperty(ref _proxyManagerSettings, value);
                IsShowByGroup = _proxyManagerSettings.IsShowByGroup;
            }
        }
        private bool _isRandomSelected;

        public bool IsRandomSelected
        {
            get => _isRandomSelected;
            set => SetProperty(ref _isRandomSelected, value);
        }

        private bool _isShowByGroup;

        public bool IsShowByGroup
        {
            get => _isShowByGroup;
            set
            {
                SetProperty(ref _isShowByGroup, value);
                SettingsModel.IsShowByGroup = value;
                if (!_isShowByGroup)
                    ApplyGrouping();
            }
        }

        private Visibility _refreshEnable;

        public Visibility RefreshEnable
        {
            get => _refreshEnable;
            set => SetProperty(ref _refreshEnable, value);
        }

        #endregion

        #region Commands

        public ICommand AddProxyCommand { get; }
        public ICommand GroupTextChangedCommand { get; }
        public ICommand ImportProxyCommand { get; }
        public ICommand ShowByGroupCommand { get; }
        public ICommand ExportProxyCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SelectProxyCommand { get; }
        public ICommand UpdateProxyCommand { get; }
        public ICommand VerifyProxyCommand { get; }
        public ICommand RefreshProxyCommand { get; }
        public ICommand RemoveAccountFromProxyCommand { get; }
        public ICommand AccountToAddToProxyCommand { get; }
        public ICommand DropDownCommand { get; }
        public ICommand AssignRandomProxyCommand { get; }
        public ICommand SaveSettingCommand { get; }

        #endregion

        #region Methods for account proxy updation

        public void UpdateProxy(DominatorAccountBaseModel objAccountBaseModel, List<ProxyManagerModel> ProxyDetail,
            AccessorStrategies strategy)
        {
            try
            {
                #region if proxy | port empty then that account will remove from proxy AccountsAssignedto list and that account will add to all proxies

                foreach (var proxy in ProxyDetail)
                    try
                    {
                        var account = proxy.AccountsAssignedto.FirstOrDefault(x =>
                            x.UserName == objAccountBaseModel.UserName &&
                            x.AccountNetwork == objAccountBaseModel.AccountNetwork);

                        if (account != null)
                        {
                            proxy.AccountsAssignedto.Remove(account);
                            _proxyFileManager.EditProxy(proxy);
                            LstProxyManagerModel
                                .FirstOrDefault(x => x.AccountProxy.ProxyId == proxy.AccountProxy.ProxyId)
                                .AccountsAssignedto = proxy.AccountsAssignedto;
                            AccountsAlreadyAssigned.Remove(AccountsAlreadyAssigned.FirstOrDefault(x =>
                                x.UserName == objAccountBaseModel.UserName
                                && x.AccountNetwork == objAccountBaseModel.AccountNetwork));
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public bool IsProxyAvailable(DominatorAccountBaseModel objAccountBaseModel, List<ProxyManagerModel> oldProxies,
            DominatorAccountBaseModel oldAccount, AccessorStrategies strategy)
        {
            var isDuplicatProxyAvailable = false;
            foreach (var proxy in oldProxies)
            {
                #region To check for available proxy 

                try
                {
                    if (objAccountBaseModel.AccountProxy.ProxyIp == proxy.AccountProxy.ProxyIp
                        && objAccountBaseModel.AccountProxy.ProxyPort == proxy.AccountProxy.ProxyPort)
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
                                        var proxyToUpdate = LstProxyManagerModel.FirstOrDefault(x =>
                                            x.AccountProxy.ProxyIp == oldAccount.AccountProxy.ProxyIp
                                            && x.AccountProxy.ProxyPort == oldAccount.AccountProxy.ProxyPort);
                                        proxyToUpdate?.AccountsAssignedto.Remove(
                                            proxyToUpdate.AccountsAssignedto.FirstOrDefault(x =>
                                                x.UserName == oldAccount.UserName &&
                                                x.AccountNetwork == oldAccount.AccountNetwork));

                                        proxyToUpdate = LstProxyManagerModel.FirstOrDefault(x =>
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
                                        var proxyToUpdate = LstProxyManagerModel.FirstOrDefault(x =>
                                            x.AccountProxy.ProxyIp == objAccountBaseModel.AccountProxy.ProxyIp
                                            && x.AccountProxy.ProxyPort == objAccountBaseModel.AccountProxy.ProxyPort);
                                        proxyToUpdate?.AccountsAssignedto.Add(
                                            new AccountAssign
                                            {
                                                UserName = objAccountBaseModel.UserName,
                                                AccountNetwork = objAccountBaseModel.AccountNetwork
                                            });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }


                                proxy.AccountsAssignedto.Add(accountTomodified);

                                _proxyFileManager.EditProxy(proxy);
                                AccountsAlreadyAssigned.Add(
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

        public async Task<bool> IsProxyUpdated(DominatorAccountBaseModel objDominatorAccountBaseModel,
            List<ProxyManagerModel> oldProxies,
            DominatorAccountBaseModel oldAccount, AccessorStrategies strategy)
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

                        await _proxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
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
                            await _proxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
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

                            await _proxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
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

        public void UpdateProxyList(ProxyManagerModel proxy)
        {
            try
            {
                var proxyToupdate = LstProxyManagerModel.FirstOrDefault(x =>
                    x.AccountProxy.ProxyId == proxy.AccountProxy.ProxyId);

                if (proxyToupdate != null)
                    proxyToupdate.AccountProxy = proxy.AccountProxy;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public async void AddProxyIfNotExist(DominatorAccountBaseModel objAccount, AccessorStrategies strategyPack)
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
                    ProxyPassword = objAccount.AccountProxy.ProxyPassword
                }
            };


            #region remove account from AccountsAssignedto if any proxy having account

            _proxyFileManager.GetAllProxy().ForEach(proxy =>
            {
                _accountsFileManager.GetAll().ForEach(acc =>
                {
                    if (proxy.AccountsAssignedto.Any(x =>
                        x.UserName == acc.UserName && x.AccountNetwork == acc.AccountBaseModel.AccountNetwork))
                        proxy.AccountsAssignedto.Remove(proxy.AccountsAssignedto.FirstOrDefault(x => x.UserName ==
                                                                                                     acc.UserName
                                                                                                     &&
                                                                                                     x.AccountNetwork ==
                                                                                                     acc
                                                                                                         .AccountBaseModel
                                                                                                         .AccountNetwork));
                });

                _proxyFileManager.EditProxy(proxy);
            });

            #endregion

            if (Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(() => { AddProxyFromAccount(objAccount, ProxyManagerModel); });
            else
                AddProxyFromAccount(objAccount, ProxyManagerModel);

            ProxyManagerModel.AccountsAssignedto.Add(new AccountAssign
            {
                UserName = objAccount.UserName,
                AccountNetwork = objAccount.AccountNetwork
            });

            _proxyFileManager.SaveProxy(ProxyManagerModel);

            await _proxyFileManager.UpdateProxyStatusAsync(ProxyManagerModel, ConstantVariable.GoogleLink);
        }

        private void AddProxyFromAccount(DominatorAccountBaseModel objAccount, ProxyManagerModel ProxyManagerModel)
        {
            LstProxyManagerModel.ForEach(x =>
            {
                if (x.AccountsAssignedto.Any(y => y.UserName == objAccount.UserName &&
                                                  y.AccountNetwork == objAccount.AccountNetwork) &&
                    objAccount.AccountProxy.ProxyIp.Trim() != ProxyManagerModel.AccountProxy.ProxyIp.Trim())
                    x.AccountsAssignedto.Remove(x.AccountsAssignedto.FirstOrDefault(y =>
                        y.UserName == objAccount.UserName &&
                        y.AccountNetwork == objAccount.AccountNetwork));
            });
            LstProxyManagerModel.Add(ProxyManagerModel);
            ProxyManagerModel.Index = LstProxyManagerModel.IndexOf(ProxyManagerModel) + 1;
            AccountsAlreadyAssigned.Add(
                new AccountAssign
                {
                    UserName = objAccount.UserName,
                    AccountNetwork = objAccount.AccountNetwork
                });
        }

        public bool UpdateProxy(DominatorAccountBaseModel objDominatorAccountBaseModel, AccessorStrategies strategy)
        {
            var oldproxies = _proxyFileManager.GetAllProxy();

            var isProxyUpdated = false;
            try
            {
                var oldAccount = _accountsFileManager
                    .GetAccount(objDominatorAccountBaseModel.UserName, objDominatorAccountBaseModel.AccountNetwork)
                    ?.AccountBaseModel;

                isProxyUpdated = IsProxyUpdated(objDominatorAccountBaseModel, oldproxies, oldAccount, strategy).Result;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return isProxyUpdated;
        }

        #endregion
    }
}