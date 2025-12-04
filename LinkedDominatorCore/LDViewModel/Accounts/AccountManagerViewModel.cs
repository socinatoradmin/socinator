using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.DatabaseHandler;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.LDLibrary;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Response;
using LinkedDominatorCore.Settings;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LinkedDominatorCore.LDViewModel.Accounts
{
    public class AccountManagerViewModel : BindableBase, ICloneable
    {
        private static AccountManagerViewModel accountManagerViewModel;

        public AccountManagerViewModel()
        {
            #region Command Initialization

            AddSingleAccountCommand = new BaseCommand<object>(AddSingleAccountCanExecute, AddSingleAccountExecute);

            LoadMultipleAccountsCommand = new BaseCommand<object>(LoadMultipleAccountsCanExecute, LoadMultipleAccountsExecute);

            InfoCommand = new BaseCommand<object>(InfoCommandCanExecute, InfoCommandExecute);

            ContextMenuOpenCommand = new BaseCommand<object>(OpenContextMenuCanExecute, OpenContextMenuExecute);

            ExportCommand = new BaseCommand<object>(ExportCanExecute, ExportExecute);

            DeleteAccountsCommand = new BaseCommand<object>(DeleteAccountsCanExecute, DeleteAccountsExecute);

            SelectAccountCommand = new BaseCommand<object>(SelectAccountCanExecute, SelectAccountExecute);

            SelectAccountByStatusCommand = new BaseCommand<object>(SelectAccountByStatusCanExecute, SelectAccountByStatusExecute);

            SelectAccountByGroupCommand = new BaseCommand<object>(SelectAccountByGroupCanExecute, SelectAccountByGroupExecute);

            SingleAccountEditCommand = new BaseCommand<object>(SingleAccountEditCanExecute, SingleAccountEditExecute);

            SingleAccountDeleteCommand = new BaseCommand<object>(SingleAccountDeleteCanExecute, SingleAccountDeleteExecute);

            #endregion
        }

        public static AccountManagerViewModel GetAccountManagerViewModel()
        {
            if (accountManagerViewModel == null)
            {
                accountManagerViewModel = new AccountManagerViewModel();

            }
            return accountManagerViewModel;
        }


        #region Commands

        public ICommand AddSingleAccountCommand { get; set; }
        public ICommand InfoCommand { get; set; }
        public ICommand LoadMultipleAccountsCommand { get; set; }
        public ICommand ContextMenuOpenCommand { get; set; }
        public ICommand DeleteAccountsCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand SelectAccountCommand { get; set; }
        public ICommand SelectAccountByStatusCommand { get; set; }
        public ICommand SelectAccountByGroupCommand { get; set; }

        public ICommand SingleAccountEditCommand { get; set; }

        public ICommand SingleAccountDeleteCommand { get; set; }

        #endregion

        #region Properties

        public ObservableCollection<DominatorAccountModel> LstAccountModel { get; set; } = new ObservableCollection<DominatorAccountModel>();

        public ObservableCollection<ContentSelectGroup> Groups { get; set; } = new ObservableCollection<ContentSelectGroup>();



        private bool _isOpenHelpControl = false;

        public bool IsOpenHelpControl
        {
            get { return _isOpenHelpControl; }
            set
            {
                if (_isOpenHelpControl != false && value == _isOpenHelpControl)
                    return;
                SetProperty(ref _isOpenHelpControl, value);
            }
        }

        private ICollectionView _accountsDetailCollection;


        public ICollectionView AccountsDetailCollection
        {
            get { return _accountsDetailCollection; }
            set
            {
                if (_accountsDetailCollection != null && value == _accountsDetailCollection)
                    return;
                SetProperty(ref _accountsDetailCollection, value);
            }
        }

        #endregion

        #region Add accounts

        private bool AddSingleAccountCanExecute(object sender) => true;

        private void AddSingleAccountExecute(object sender)
        {

            var objSingleAccountModel = new SingleAccountModel
            {
                BtnContent = "Save",
                PageTitle = "Add Single Account"
            };

            var objSingleAccountControl = new SingleAccountControl(objSingleAccountModel);

            var customDialog = new CustomDialog()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = objSingleAccountControl
            };

            var objDialog = new Dialog();
            var dialogWindow = objDialog.GetCustomDialog(customDialog);

            objSingleAccountControl.btnSave.Click += (senders, events) =>
            {
                if (string.IsNullOrEmpty(objSingleAccountModel.UserName) ||
                    string.IsNullOrEmpty(objSingleAccountModel.Password)) return;

                AddAccount(objSingleAccountModel);
                dialogWindow.Close();
            };

            objSingleAccountControl.btnCancel.Click += (senders, events) => dialogWindow.Close();

            dialogWindow.ShowDialog();

        }

        private bool LoadMultipleAccountsCanExecute(object sender)
        {
            return true;
        }

        /// <summary>
        ///LoadMultipleAccountsExecute is used to load multiple accounts at a time
        ///GroupName:Username:Password:ProxyIp:ProxyPort:ProxyUsername:ProxyPassword
        ///GroupName:Username:Password:ProxyIp:ProxyPort
        ///GroupName:Username:Password
        ///Can load , instead of :
        ///If any values are null, we can use NA        
        /// </summary>
        /// <param name="sender"></param>
        private void LoadMultipleAccountsExecute(object sender)
        {
            //Read the accounts from text or csv files
            var loadedAccountlist = FileUtilities.FileBrowseAndReader();

            //if loaded text or csv contains no accounts then return
            if (loadedAccountlist == null) return;

            #region Add to bin files which are valid accounts 

            //Iterate the all accounts one by one
            foreach (var singleAccount in loadedAccountlist)
            {
                try
                {
                    var finalAccount = singleAccount.Replace(",", ":").Replace("NA", "");
                    var splitAccount = Regex.Split(finalAccount, ":");
                    if (splitAccount.Length <= 1) continue;

                    //assign the username, password and groupname
                    var groupname = splitAccount[0];
                    var username = splitAccount[1];
                    var password = splitAccount[2];

                    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                        continue;

                    var proxyaddress = string.Empty;
                    var proxyport = string.Empty;
                    var proxyusername = string.Empty;
                    var proxypassword = string.Empty;

                    switch (splitAccount.Length)
                    {
                        case 5:
                            proxyaddress = splitAccount[3];
                            proxyport = splitAccount[4];
                            break;
                        case 7:
                            proxyaddress = splitAccount[3];
                            proxyport = splitAccount[4];
                            proxyusername = splitAccount[5];
                            proxypassword = splitAccount[6];
                            break;
                    }

                    if (splitAccount.Length > 3)
                    {
                        if (string.IsNullOrEmpty(proxyaddress) || string.IsNullOrEmpty(proxyport))
                        {
                            proxyaddress = proxyport = proxyusername = proxypassword = string.Empty;
                        }
                        //valid the proxy ip and port
                        else if (!Proxy.IsValidProxyIp(proxyaddress) || !Proxy.IsValidProxyPort(proxyport))
                            continue;
                    }


                    var objSingleAccountModel = new SingleAccountModel
                    {
                        GroupName = groupname ?? ConstantVariable.UnGrouped,
                        UserName = username,
                        Password = password,
                        AccountProxy =
                            {
                                ProxyIp = proxyaddress,
                                ProxyPort = proxyport,
                                ProxyUsername = proxyusername,
                                ProxyPassword = proxypassword
                            }
                    };

                    //add the account to accountmodel list and bin file
                    AddAccount(objSingleAccountModel);
                }
                catch (Exception ex)
                {
                    /*INFO*/
                    Console.WriteLine(ex.StackTrace);
                    GlobusLogHelper.log.Error(ex.Message);
                }
            }

            #endregion

        }

        public void AddAccount(SingleAccountModel objSingleAccountModel)
        {

            #region Add Account
            //check the account is already present or not
            if (LstAccountModel.Any(x => x.UserName == objSingleAccountModel.UserName))
            {
                /*INFO*/
                Console.WriteLine($@"Account [{objSingleAccountModel.UserName}] already added!");
                GlobusLogHelper.log.Info($@"Account [{objSingleAccountModel.UserName}] already added!");
                return;
            }

            //Initialize the given account to account model
            //var objAccountModel = new DominatorAccountModel
            //{

            //    RowNo = LstAccountModel.Count + 1,
            //    AccountBaseModel = { AccountGroup =
            //    {
            //        Content = objSingleAccountModel.GroupName ?? ConstantVariable.UnGrouped
            //    },

            //    UserName = objSingleAccountModel.UserName,
            //    Password = objSingleAccountModel.Password,
            //    AccountProxy =
            //    {
            //        ProxyIp = objSingleAccountModel.AccountProxy.ProxyIp,
            //        ProxyPort = objSingleAccountModel.AccountProxy.ProxyPort,
            //        ProxyUsername = objSingleAccountModel.AccountProxy.ProxyUsername,
            //        ProxyPassword = objSingleAccountModel.AccountProxy.ProxyPassword
            //    }
            //    }

            //   UpdateModulePrivateDetailsValue
            //  {
            //        ConnectionsCount = 0,
            //        GroupsCount = 0,
            //        PostsCount = 0,
            //    }


            //    Status = ConstantVariable.NotChecked
            //};
            var objAccountModel = new DominatorAccountModel
            {
                RowNo = LstAccountModel.Count + 1,
                AccountBaseModel = new DominatorAccountBaseModel()
                {
                    AccountGroup = new ContentSelectGroup()
                    {
                        Content = objSingleAccountModel.GroupName ?? ConstantVariable.UnGrouped
                    },
                    UserName = objSingleAccountModel.UserName,
                    Password = objSingleAccountModel.Password,
                    AccountProxy = new Proxy()
                    {
                        ProxyIp = objSingleAccountModel.AccountProxy.ProxyIp,
                        ProxyPort = objSingleAccountModel.AccountProxy.ProxyPort,
                        ProxyUsername = objSingleAccountModel.AccountProxy.ProxyUsername,
                        ProxyPassword = objSingleAccountModel.AccountProxy.ProxyPassword
                    },
                    Status = ConstantVariable.NotChecked,
                }
            };

            objAccountModel.UpdateModulePrivateDetailsValue(new AccountModel(objAccountModel)
            {
                ConnectionsCount = 0,
                GroupsCount = 0,
                PostsCount = 0,
            });

            //serialize the given account, if its success then add to account model list
            LstAccountModel.Add(objAccountModel);
            AccountsFileManager.Add(objAccountModel);

            DirectoryUtilities.CreateDirectory(ConstantVariable.GetIndexAccountFile());

            //serialize the given account, if its success then add to account model list
            if (AccountsFileManager.Add(objAccountModel))
            {
                LstAccountModel.Add(objAccountModel);
            }
            else
            {
                /*INFO*/
                Console.WriteLine($@"Account [{objAccountModel.UserName}] isn't saved!");
                GlobusLogHelper.log.Info($@"Account [{objAccountModel.UserName}] isn't saved!");
            }

            //DataBaseHandler.CreateAccountDataBase(objSingleAccountModel.UserName);

            #endregion

            #region Login Account And Update Follower And Following Count

            UpdateAccount(objAccountModel);

            #endregion
        }

        public void UpdateAccount(DominatorAccountModel objAccountModel)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;

            Task.Factory.StartNew(() =>
            {
                //UpdateAccountDetails(objAccountModel);
            }, ct);
        }

        public void CheckAccount(DominatorAccountModel objDominatorAccountModel)
        {
           
        }


       



        #endregion

        #region Delete Accounts

        private bool SingleAccountDeleteCanExecute(object sender) => true;

        private void SingleAccountDeleteExecute(object sender)
        {
            DeleteAccountByContextMenu(sender);
        }


        private bool DeleteAccountsCanExecute(object sender)
        {
            return true;
        }

        private void DeleteAccountsExecute(object sender)
        {
            try
            {
                AccountsFileManager.Delete((DominatorAccountModel x) => x.IsAccountManagerAccountSelected == true);
            }
            catch (Exception ex)
            {
                /*INFO*/
                Console.WriteLine(ex.StackTrace);
            }
        }



        public void DeleteAccountByContextMenu(object sender)
        {
            var selectedRow = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

            AccountsFileManager.Delete((DominatorAccountModel x) => x.AccountId == selectedRow.AccountId);

            GlobusLogHelper.log.Info(selectedRow.UserName + " Deleted");
        }


        #endregion

        #region ContextMenuIsOpen 

        private bool OpenContextMenuCanExecute(object sender) => true;

        private void OpenContextMenuExecute(object sender)
        {


            ContextMenuOpen(sender);
            var button = sender as Button;
            if (button == null || button.Name != "BtnSelect") return;

            var currentGroups = Groups.Select(x => x.Content).Distinct().ToList();

            Groups.Clear();

            var availableGroups = Groups.Select(x => x.Content).ToList();

            var newGroups = currentGroups.Except(availableGroups).ToList();

            if (newGroups.Count <= 0)
                return;

            newGroups.ForEach(x =>
            {
                Groups.Add(new ContentSelectGroup()
                {
                    Content = x,
                    IsContentSelected = false
                });
            });

        }

        private static void ContextMenuOpen(object sender)
        {
            try
            {
                var contextMenu = ((Button)sender).ContextMenu;
                if (contextMenu == null) return;
                contextMenu.DataContext = ((Button)sender).DataContext;
                contextMenu.IsOpen = true;
            }
            catch (Exception ex)
            {
                /*INFO*/
                Console.WriteLine(ex.StackTrace);
                GlobusLogHelper.log.Error(ex.Message);
            }
        }

        #endregion

        #region Export Accounts

        private bool ExportCanExecute(object sender) => true;

        private void ExportExecute(object sender)
        {

            var selectedAccounts = LstAccountModel.Where(x => x.IsAccountManagerAccountSelected == true).ToList();

            if (selectedAccounts.Count <= 0)
                return;

            var exportPath = FileUtilities.GetExportPath();

            if (string.IsNullOrEmpty(exportPath))
                return;

            const string header = "Username,Password,Account Group,Status,Follower,Followings,Posts,Proxy Address,Proxy Port,Proxy Username,Proxy Password";

            var filename = $"{exportPath}\\AccountExport {ConstantVariable.DateasFileName}.csv";

            if (!File.Exists(filename))
            {
                using (var streamWriter = new StreamWriter(filename, true))
                {
                    streamWriter.WriteLine(header);
                }
            }

            selectedAccounts.ForEach(account =>
            {
                try
                {


                    var csvData = account.AccountBaseModel.UserName + "," + account.AccountBaseModel.Password + "," + account.AccountBaseModel.AccountGroup.Content + "," + account.AccountBaseModel.Status + "," + account.GetModulePrivateDetailsValue("ConnectionsCount") + "," +
                                  account.GetModulePrivateDetailsValue("GroupsCount") + "," + account.GetModulePrivateDetailsValue("PostsCount") + "," + account.AccountBaseModel.AccountProxy.ProxyIp + "," + account.AccountBaseModel.AccountProxy.ProxyPort + "," + account.AccountBaseModel.AccountProxy.ProxyUsername + "," + account.AccountBaseModel.AccountProxy.ProxyPassword;

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
        }

        #endregion

        #region Help Methods

        private bool InfoCommandCanExecute(object sender) => true;

        private void InfoCommandExecute(object sender) => IsOpenHelpControl = true;

        #endregion

        #region Edit Accounts

        private bool SingleAccountEditCanExecute(object sender) => true;

        private void SingleAccountEditExecute(object sender)
        {
            EditAccount(sender);
        }

        public void EditAccount(object sender)
        {

            var selectedRow = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

            var selectedAccount = LstAccountModel.FirstOrDefault<DominatorAccountModel>(x => selectedRow != null && x.RowNo == selectedRow.RowNo);

            if (selectedAccount == null) return;

            var objSingleAccountModel = new SingleAccountModel
            {
                BtnContent = "Update",
                PageTitle = "Update Single Account",
                GroupName = selectedAccount.AccountBaseModel.AccountGroup.Content,
                UserName = selectedAccount.AccountBaseModel.UserName,
                Password = selectedAccount.AccountBaseModel.Password,
                AccountProxy =
                {
                    ProxyIp = selectedAccount.AccountBaseModel.AccountProxy.ProxyIp,
                    ProxyPort = selectedAccount.AccountBaseModel.AccountProxy.ProxyPort,
                    ProxyUsername = selectedAccount.AccountBaseModel.AccountProxy.ProxyUsername,
                    ProxyPassword = selectedAccount.AccountBaseModel.AccountProxy.ProxyPassword
                }
            };

            var objSingleAccountControl = new SingleAccountControl(objSingleAccountModel);

            var customDialog = new CustomDialog()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = objSingleAccountControl
            };

            var objDialog = new Dialog();

            var dialogWindow = objDialog.GetCustomDialog(customDialog);

            objSingleAccountControl.btnSave.Click += (senders, events) =>
            {

                if (string.IsNullOrEmpty(objSingleAccountModel.UserName) ||
                    string.IsNullOrEmpty(objSingleAccountModel.Password)) return;

                selectedAccount.AccountBaseModel.AccountGroup.Content = objSingleAccountModel.GroupName;
                selectedAccount.AccountBaseModel.UserName = objSingleAccountModel.UserName;
                selectedAccount.AccountBaseModel.Password = objSingleAccountModel.Password;
                selectedAccount.AccountBaseModel.AccountProxy.ProxyIp = objSingleAccountModel.AccountProxy.ProxyIp;
                selectedAccount.AccountBaseModel.AccountProxy.ProxyPort = objSingleAccountModel.AccountProxy.ProxyPort;
                selectedAccount.AccountBaseModel.AccountProxy.ProxyUsername = objSingleAccountModel.AccountProxy.ProxyUsername;
                selectedAccount.AccountBaseModel.AccountProxy.ProxyPassword = objSingleAccountModel.AccountProxy.ProxyPassword;

                var socinatorAccountBuilder = new SocinatorAccountBuilder(selectedAccount.AccountBaseModel.AccountId)
                                                 .AddOrUpdateDominatorAccountBase(selectedAccount.AccountBaseModel)
                                                 .AddOrUpdateCookies(selectedAccount.Cookies)
                                                 .SaveToBinFile();

                dialogWindow.Close();

            };
            objSingleAccountControl.btnCancel.Click += (senders, events) =>
            {
                dialogWindow.Close();
            };

            dialogWindow.ShowDialog();
        }


        #endregion

        #region Select Accounts

        private bool SelectAccountCanExecute(object sender) => true;

        private void SelectAccountExecute(object sender)
        {
            var selection = sender as string;

            if (selection == "SelectAll")
                SelectAllAccounts();
            else
                DeselectAllAccounts();
        }

        private bool SelectAccountByStatusCanExecute(object sender) => true;

        private void SelectAccountByStatusExecute(object sender)
        {
            SelectAccount(sender);
        }

        public void SelectAllAccounts()
        {
            LstAccountModel.Select(x =>
            {
                x.IsAccountManagerAccountSelected = true; return x;
            }).ToList();
        }

        public void DeselectAllAccounts()
        {
            LstAccountModel.Select(x =>
            {
                x.IsAccountManagerAccountSelected = false; return x;
            }).ToList();
        }

        public void SelectAccount(object sender)
        {

            DeselectAllAccounts();

            var menu = sender as string;

            switch (menu)
            {
                case "Working":
                    LstAccountModel.Where(x => x.AccountBaseModel.Status == "Success").Select(x =>
                    {
                        x.IsAccountManagerAccountSelected = true;
                        return x;
                    }).ToList();
                    break;
                case "NotWorking":
                    LstAccountModel.Where(x => x.AccountBaseModel.Status == "Failed").Select(x =>
                    {
                        x.IsAccountManagerAccountSelected = true;
                        return x;
                    }).ToList();
                    break;
                case "NotChecked":
                    LstAccountModel.Where(x => x.AccountBaseModel.Status == "Not Checked").Select(x =>
                    {
                        x.IsAccountManagerAccountSelected = true;
                        return x;
                    }).ToList();
                    break;
            }
        }

        private bool SelectAccountByGroupCanExecute(object sender) => true;

        private void SelectAccountByGroupExecute(object sender)
        {
            SelectAccountByGroup(sender);
        }


        public void SelectAccountByGroup(object sender)
        {
            try
            {
                var checkedItem = sender as CheckBox;
                if (checkedItem == null) return;

                var currentGroup = ((FrameworkElement)sender).DataContext as ContentSelectGroup;

                Groups.Where(x => currentGroup != null && x.Content == currentGroup.Content.ToString()).Select(x =>
                {
                    LstAccountModel.Where(y => y.AccountBaseModel.AccountGroup.Content == x.Content).Select(y =>
                    {
                        if (currentGroup != null) y.IsAccountManagerAccountSelected = currentGroup.IsContentSelected;
                        return y;
                    }).ToList();
                    return x;
                }).ToList();
            }
            catch (Exception ex)
            {
                /*INFO*/
                Console.WriteLine(ex.StackTrace);
            }
        }

        #endregion

        #region Initialize AccountManager

        public void InitialAccountDetails()
        {
            var savedAccounts = AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.LinkedIn);

            var allGroups = new List<ContentSelectGroup>();

            try
            {
                savedAccounts.ForEach(account =>
                {
                    LstAccountModel.Add(account);
                    allGroups.Add(account.AccountBaseModel.AccountGroup);
                    //Global.ScheduleForEachModule(null, account);
                });

                foreach (var group in allGroups)
                {
                    if (Groups.Any(x => x.Content.Equals
                            (group.Content, StringComparison.CurrentCultureIgnoreCase)) == false)
                        Groups.Add(group);
                }
            }
            catch (Exception ex)
            {
                /*DEBUG*/
                Console.WriteLine(ex.StackTrace);
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

    }
}
