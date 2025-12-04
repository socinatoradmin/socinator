using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Command;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using RedditDominatorCore.RDLibrary;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.Response;
using RedditDominatorCore.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RedditDominatorCore.RDViewModel.Accounts
{
   public class AccountViewModel : BindableBase
    {
        static AccountViewModel accountViewModel;
        private static AccountViewModel _AccountrViewModel { get; set; }

        private AccountViewModel()
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
        public static AccountViewModel GetAccountViewModel()
        {

            if (accountViewModel == null)
            {
                accountViewModel = new AccountViewModel();
            }
            return accountViewModel;
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


        private System.ComponentModel.ICollectionView _accountsDetailCollection;

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

            //objSingleAccountControl.btnSave.Click += (senders, events) =>
            //{
            //    if (string.IsNullOrEmpty(objSingleAccountModel.UserName) ||
            //        string.IsNullOrEmpty(objSingleAccountModel.Password)) return;

            //    AddAccount(objSingleAccountModel);
            //    dialogWindow.Close();
            //};

            //objSingleAccountControl.btnCancel.Click += (senders, events) => dialogWindow.Close();

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
                    var splitAccount = System.Text.RegularExpressions.Regex.Split(finalAccount, ":");
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
                }
            }

            #endregion

        }
        public void AddAccount(SingleAccountModel objSingleAccountModel)
        {

            #region Add Account
            //check the account is already present or not
            if (LstAccountModel.Any(x => x.AccountBaseModel.UserName == objSingleAccountModel.UserName))
            {
                /*INFO*/
                Console.WriteLine($@"Account [{objSingleAccountModel.UserName}] already added!");
                GlobusLogHelper.log.Info($@"Account [{objSingleAccountModel.UserName}] already added!");
                return;
            }

            //Initialize the given account to account model
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
                    //accountproxy = new proxy()
                    //{
                    //    proxyip = objsingleaccountmodel.accountproxy.proxyip,
                    //    proxyport = objsingleaccountmodel.accountproxy.proxyport,
                    //    proxyusername = objsingleaccountmodel.accountproxy.proxyusername,
                    //    proxypassword = objsingleaccountmodel.accountproxy.proxypassword
                    //},

                    Status = ConstantVariable.NotChecked,


                }
              ,
                //ModulePrivateDetails = JsonHelper.GetJson(new AccountModel()
                //{
                //    FollowersCount = 0,
                //    FollowingCount = 0,
                //    PostsCount = 0
                //}),


            };


            // serialize the given account, if its success then add to account model list
            LstAccountModel.Add(objAccountModel);
            AccountsFileManager.Add(objAccountModel);


           // DataBaseHandler.CreateDataBase(objSingleAccountModel.UserName, SocialNetworks.Reddit, DatabaseType.AccountType);


            #endregion

            #region Login Account And Update Follower And Following Count

            UpdateAccount(objAccountModel);

            #endregion
        }


        public void UpdateAccount(DominatorAccountModel objDominatorAccountModel)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;




            Task.Factory.StartNew(() =>
            {
                UpdateAccountFollowerFollowing(objDominatorAccountModel);
            }, ct);

        }
        

        private static void UpdateAccountFollowerFollowing(DominatorAccountModel objDominatorAccountModel)
        {
            try
            {

                string loginResponse = string.Empty;
                LogInProcess logInProcess = new LogInProcess();
                //if (!logInProcess.CheckLogin(objDominatorAccountModel))
                //{
                    logInProcess.LoginWithAlternativeMethod(objDominatorAccountModel);
                //}

                RedditFunction RDFunct = new RedditFunction(objDominatorAccountModel);

                //if (!objDominatorAccountModel.IsUserLoggedIn)
                //{
                //    return;
                //}

                UserNameInfoRDResponseHandler userInfo = RDFunct.GetUserDetails(objDominatorAccountModel.AccountBaseModel.UserName);
                //objDominatorAccountModel.AccountBaseModel.UserName = userInfo.Username;
                //objDominatorAccountModel.AccountBaseModel.UserId = userInfo.UserId;
                //objDominatorAccountModel.AccountBaseModel.UserFullName = userInfo.FullName;
                objDominatorAccountModel.AccountBaseModel.ProfilePictureUrl = userInfo.Profilepicurl;

                //objDominatorAccountModel.DisplayColumnValue2 = tmpAccount.FollowingCount;
                //objDominatorAccountModel.DisplayColumnValue3 = tmpAccount.FollowersCount;
                //objDominatorAccountModel.DisplayColumnValue4 = tmpAccount.BoardCount;
                //List<RedditUser> lstUserDetails = new List<RedditUser>();

                //DominatorHouseCore.DatabaseHandler.CoreModels.DataBaseConnection databaseConnection =
                //    DataBaseHandler.GetDataBaseConnectionInstance(objDominatorAccountModel.AccountId, SocialNetworks.Reddit);

                //try
                //{
                //    FollowerAndFollowingRDResponseHandler objFollowerHandler =
                //        RDFunct.GetUserFollowers(objDominatorAccountModel.AccountBaseModel.UserId);
                //    while (true)
                //    {
                //        objFollowerHandler.UsersList.ForEach((Action<RedditUser>)(x =>
                //        {
                //            try
                //            {
                //                Friendships friendship = new Friendships()
                //                {
                //                    Username = x.Username,
                //                    Followers = x.FollowersCount,
                //                    Followings = x.FollowingsCount,
                //                    FullName = x.FullName,
                //                    ProfilePicUrl = x.ProfilePicUrl,
                //                    HasAnonymousProfilePicture = x.HasProfilePic,
                //                    FollowType = FollowType.NotFollowing

                //                };
                //                databaseConnection.Add<Friendships>(friendship);
                //            }

                //            catch (Exception e)
                //            {
                //                GlobusLogHelper.log.Error(e.Message);
                //            }
                //        }));
                //        if (!objFollowerHandler.HasMoreResults)
                //            break;
                //        objFollowerHandler = RDFunct.GetUserFollowers(objDominatorAccountModel.AccountBaseModel.UserName, objFollowerHandler.BookMark);

                //    }
                //}
                //catch (Exception Ex)
                //{
                //    GlobusLogHelper.log.Error(Ex.Message);
                //}

                //try
                //{
                //    FollowerAndFollowingRDResponseHandler objFollowingHandler =
                //        RDFunct.GetUserFollowings(objDominatorAccountModel.AccountBaseModel.UserId);
                //    while (true)
                //    {

                //        List<Friendships> lst_Friendships = databaseConnection.Get<Friendships>(x => x.FollowType == AccountType.Following);

                //        objFollowingHandler.UsersList.ForEach(x =>
                //        {
                //            try
                //            {
                //                Friendships friendship = new Friendships()
                //                {
                //                    Username = x.Username,
                //                    Followers = x.FollowersCount,
                //                    Followings = x.FollowingsCount,
                //                    FullName = x.FullName,
                //                    ProfilePicUrl = x.ProfilePicUrl,
                //                    HasAnonymousProfilePicture = x.HasProfilePic,
                //                    FollowType = Fo.Following

                //                };

                //                databaseConnection.Add<Friendships>(friendship);
                //            }
                //            catch (Exception e)
                //            {
                //                GlobusLogHelper.log.Error(e.Message);
                //            }
                //        });
                //        if (!objFollowingHandler.HasMoreResults)
                //        {
                //            break;
                //        }
                //        objFollowingHandler = RDFunct.GetUserFollowings(objDominatorAccountModel.AccountBaseModel.UserName, objFollowingHandler.BookMark);
                //    }
                //}
                //catch (Exception ex)
                //{
                //    GlobusLogHelper.log.Error(ex.Message);
                //}

                objDominatorAccountModel.Token.ThrowIfCancellationRequested();
                var socinatorAccountBuilder = new SocinatorAccountBuilder(objDominatorAccountModel.AccountBaseModel.AccountId)
                 .AddOrUpdateDominatorAccountBase(objDominatorAccountModel.AccountBaseModel)
                 .AddOrUpdateCookies(objDominatorAccountModel.Cookies)
                 .SaveToBinFile();

            }
            catch (OperationCanceledException)
            {
                throw new System.OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
                }
            }
            catch (Exception Ex)
            {
                GlobusLogHelper.log.Error(Ex.Message + Ex.StackTrace);
            }
            
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
                //collect the selected account
                var selectAccounts = LstAccountModel.Where(x => x.IsAccountSelected == true).ToList();

                DeleteAccounts(selectAccounts);

            }
            catch (Exception ex)
            {
                /*INFO*/
                Console.WriteLine(ex.StackTrace);
            }
        }

        private bool DeleteAccounts(List<DominatorAccountModel> selectAccounts)
        {

            //if selectedaccount count is zero, it wont delete the bin file
            if (selectAccounts.Count == 0) return true;

            // remove the selected accounts from account model
            selectAccounts.ForEach(item => LstAccountModel.Remove(item));


            // after removed serialize the remaining accounts 

            //AccountsFileManager.Delete<AccountModel>(p => selectAccounts.FirstOrDefault(a => a.AccountId == p.AccountId) != null);




            // after removed serialize the remaining accounts             
            AccountsFileManager.Delete(p => selectAccounts.FirstOrDefault(a => a.AccountId == p.AccountId) != null);

            return false;
        }

        public void DeleteAccountByContextMenu(object sender)
        {
            var selectedRow = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

            var selectedAccount = LstAccountModel.FirstOrDefault(x => selectedRow != null && x.AccountBaseModel.AccountId == selectedRow.AccountBaseModel.AccountId);

            DeleteAccounts(new List<DominatorAccountModel> { selectedAccount });
        }

        public void CheckAccountByContextMenu(Object sender)
        {
            DominatorAccountModel accountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
            //var selectedAccount = LstAccountModel.FirstOrDefault(x => selectedRow != null && x.AccountId == selectedRow.AccountId);
            UpdateAccount(accountModel);
        }


        #endregion
        #region ContextMenuIsOpen 

        private bool OpenContextMenuCanExecute(object sender) => true;

        private void OpenContextMenuExecute(object sender)
        {


            ContextMenuOpen(sender);
            var button = sender as Button;
            if (button == null || button.Name != "BtnSelect") return;

            var currentGroups = LstAccountModel.Select(x => x.AccountBaseModel.AccountGroup.Content).Distinct().ToList();

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
            }
        }

        #endregion

        #region Export Accounts

        private bool ExportCanExecute(object sender) => true;

        private void ExportExecute(object sender)
        {

            var selectedAccounts = LstAccountModel.Where(x => x.IsAccountSelected == true).ToList();

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
                    var csvData = account.AccountBaseModel.UserName + ","
                                  + account.AccountBaseModel.Password + ","
                                  + account.AccountBaseModel.AccountGroup.Content + ","
                                  + account.AccountBaseModel.Status + ","
                                   + JsonHelper.GetModel<AccountModel>(account.ModulePrivateDetails.ToString()).FollowersCount + ","
                                  + JsonHelper.GetModel<AccountModel>(account.ModulePrivateDetails.ToString()).FollowingCount + ","
                                  + account.AccountBaseModel.AccountProxy.ProxyIp + ","
                                  + account.AccountBaseModel.AccountProxy.ProxyPort + ","
                                  + account.AccountBaseModel.AccountProxy.ProxyUsername + ","
                                  + account.AccountBaseModel.AccountProxy.ProxyPassword;

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
                Password = selectedAccount.AccountBaseModel.UserName,
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

            objSingleAccountControl.btnCancel.Click += (senders, events) => dialogWindow.Close();

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
                x.IsAccountSelected = true; return x;
            }).ToList();
        }

        public void DeselectAllAccounts()
        {
            LstAccountModel.Select(x =>
            {
                x.IsAccountSelected = false; return x;
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
                        x.IsAccountSelected = true;
                        return x;
                    }).ToList();
                    break;
                case "NotWorking":
                    LstAccountModel.Where(x => x.AccountBaseModel.Status == "Failed").Select(x =>
                    {
                        x.IsAccountSelected = true;
                        return x;
                    }).ToList();
                    break;
                case "NotChecked":
                    LstAccountModel.Where(x => x.AccountBaseModel.Status == "Not Checked").Select(x =>
                    {
                        x.IsAccountSelected = true;
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

                var currentGroup = ((FrameworkElement)sender).DataContext as AccountGroup;

                Groups.Where(x => currentGroup != null && x.Content == currentGroup.AccountGroupName.ToString()).Select(x =>
                {
                    LstAccountModel.Where(y => y.AccountBaseModel.AccountGroup.Content == x.Content).Select(y =>
                    {
                        if (currentGroup != null) y.IsAccountSelected = currentGroup.IsAccountGroupSelected;
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

        private object syncLoadAccounts = new object();
        private static object tmpAccount;

        /// <summary>
        /// Loads account details to UI and schedule activities for each one
        /// </summary>
        public void InitializeAccountDetails()
        {
            lock (syncLoadAccounts)
            {
                var savedAccounts = AccountsFileManager.GetAll();

                var allGroups = new List<ContentSelectGroup>();

                try
                {
                    LstAccountModel.Clear();

                    // Populate UI with accounts and schedule activities for each one
                    foreach (var account in savedAccounts)
                    {
                        LstAccountModel.Add(account);
                        allGroups.Add(account.AccountBaseModel.AccountGroup);
                        DominatorScheduler.ScheduleForEachModule(null, account, SocialNetworks.Pinterest);
                    };


                    // Populate UI for groups
                    foreach (var group in allGroups)
                    {
                        if (Groups.Any(x => x.Content.Equals
                                (group.Content, StringComparison.CurrentCultureIgnoreCase)) == false)
                            Groups.Add(group);
                    }
                }
                catch (Exception ex)
                {
                    ex.ErrorLog("Unable to initialize accounts");
                }
            }
        }

        #endregion
        #region Helper Method
        
       


        //public static DominatorHouseCore.DatabaseHandler.CoreModels.DataBaseConnection GetDataBaseConnectionInstance(string UserName)
        //{
        //    try
        //    {
        //        string directoryName = ConstantVariable.GetIndexAccountFile() + $"\\DB";


        //        if (!Directory.Exists(directoryName))
        //        {
        //            Directory.CreateDirectory(directoryName);
        //        }
        //        string connectionString = directoryName + $"\\{UserName}.db";
        //        return new DominatorHouseCore.DatabaseHandler.CoreModels.DataBaseConnection(connectionString, SocialNetworks.Reddit);
        //    }
        //    catch (Exception Ex)
        //    {
        //        return null;
        //    }

        //}
        #endregion
    }
}
