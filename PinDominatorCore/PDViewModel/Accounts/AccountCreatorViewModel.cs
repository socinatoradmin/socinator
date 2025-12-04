using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PinDominatorCore.PDViewModel.Accounts
{
    public class AccountCreatorViewModel : BindableBase
    {
        private AccountCreatorModel _accountCreatorModel = new AccountCreatorModel();
        private int _CurrentSelectedTab = 0;
        public int CurrentSelectedTab
        {
            get=>_CurrentSelectedTab;
            set=>SetProperty(ref _CurrentSelectedTab, value,nameof(CurrentSelectedTab));
        }
        public AccountCreatorModel AccountCreatorModel
        {
            get { return _accountCreatorModel; }
            set { SetProperty(ref _accountCreatorModel, value); }
        }

        public AccountCreatorModel Model => AccountCreatorModel;

        public AccountCreatorViewModel()
        {
            AccountCreatorModel.JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfAccountToCreatePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfAccountToCreatePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfAccountToCreatePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfAccountToCreatePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyAccountToCreatePerDay".FromResourceDictionary(),

                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };

            AddCreateAccountCommand = new BaseCommand<object>(sender => true, AddAccount);
            DeleteAccountCommand = new BaseCommand<object>(sender => true, DeleteAccount);
            EditAccountDetails = new BaseCommand<object>(sender => true, EditSingleAccountExecute);
            ImportFromCsvCommand = new BaseCommand<object>(sender => true, ImportAccount);       
        }

        private void EditSingleAccountExecute(object obj)
        {
            try
            {
                AccountCreatorModel.CreateAccountInfo = obj as CreateAccountInfo;
                CurrentSelectedTab = 0;
            }
            catch (Exception) { }
        }

        public ICommand AddCreateAccountCommand { get; set; }
        public ICommand DeleteAccountCommand { get; set; }
        public ICommand EditAccountDetails { get; set; }
        public ICommand ImportFromCsvCommand { get; set; }

        private ModuleSettingsUserControl<AccountCreatorViewModel, AccountCreatorModel> ModuleSettingsUserControl { get; set; }
              
        private void ImportAccount(object obj)
        {
            try
            {
                AccountCreatorModel.LstCreateAccountInfo.Clear();
                AccountCreatorModel.LstImportAccount.Clear();
                AccountCreatorModel.LstImportAccount.AddRange(FileUtilities.FileBrowseAndReader());
                if(AccountCreatorModel.LstImportAccount.Count>0)
                {
                    GetAccountList(AccountCreatorModel.LstImportAccount);
                    GlobusLogHelper.log.Info("Account Details sucessfully uploaded !!");                 
                }
                else
                {
                    GlobusLogHelper.log.Info("You did not upload any Account details !!");
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
                GlobusLogHelper.log.Info("There is error in uploading Account details !!");
            }
        }
        
        private void GetAccountList(List<string> lstImportAccount)
        {
            try
            {
                var tempList = new List<CreateAccountInfo>();
                foreach(var account in AccountCreatorModel.LstImportAccount)
                {
                    try
                    {
                        var accountDetails = account.Split('\t');
                        if (accountDetails[0].ToLower() == "email") continue;
                        var createAccountInfo = new CreateAccountInfo();
                        createAccountInfo.Email = accountDetails[0];
                        createAccountInfo.Password = accountDetails[1];
                        createAccountInfo.Age = accountDetails[2];
                        createAccountInfo.Gender = accountDetails[3];
                        tempList.Add(createAccountInfo);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AccountCreatorModel.ObsCreateAccountInfo.AddRange(tempList);
                });
            }
            catch (Exception)
            {
            }
        }

        private void DeleteAccount(object obj)
        {
            try
            {
                Task.Factory.StartNew(async() => {
                    var accountDetails = obj as CreateAccountInfo;               
                    AccountCreatorModel.ObsCreateAccountInfo.Remove(accountDetails);
                });
            }
            catch
            {
                // ignored
            }
        }

        private void AddAccount(object obj)
        {
            Task.Factory.StartNew(() => {
                try
                {
                    if (AccountCreatorModel.ObsCreateAccountInfo.Count == 0)
                        AccountCreatorModel.ObsCreateAccountInfo = new ObservableCollectionBase<CreateAccountInfo>();
                    var accountAdded = false;
                    if (AccountCreatorModel != null && AccountCreatorModel.CreateAccountInfo != null)
                    {
                        var accountInfo = AccountCreatorModel.CreateAccountInfo;
                        accountInfo.Email = accountInfo.Email.Trim();
                        accountInfo.Password = accountInfo.Password.Trim();
                        accountInfo.Age = accountInfo.Age.Trim();
                        accountInfo.Gender = accountInfo.Male == true ? "male" : "female";
                        accountInfo.Id = accountInfo.Id ?? Guid.NewGuid().ToString();
                        accountInfo.IsCheckedToAccountManager = accountInfo.IsCheckedToAccountManager;
                        if (string.IsNullOrEmpty(accountInfo.Email) || string.IsNullOrEmpty(accountInfo.Password) ||
                            string.IsNullOrEmpty(accountInfo.Age) || string.IsNullOrEmpty(accountInfo.Gender))
                            return;

                        else if (AccountCreatorModel.LstCreateAccountInfo.Count > 0 &&
                                 !string.IsNullOrEmpty(accountInfo.Email))
                        {
                            AccountCreatorModel.LstCreateAccountInfo.ForEach(acc =>
                            {
                                if (AccountCreatorModel.ObsCreateAccountInfo.All(info => info.Id != acc.Id))
                                {
                                    AccountCreatorModel.ObsCreateAccountInfo.Add(new CreateAccountInfo
                                    {
                                        Id = acc.Id,
                                        Email = acc.Email,
                                        Password = acc.Password,
                                        Age = acc.Age,
                                        Gender = acc.Gender,
                                        IsCheckedToAccountManager = acc.IsCheckedToAccountManager
                                    });
                                    accountAdded = true;
                                }
                                else
                                {
                                    var accountToUpdate = AccountCreatorModel.ObsCreateAccountInfo.FirstOrDefault(x => x.Id == accountInfo.Id);
                                    if (accountToUpdate != null)
                                    {
                                        var index = AccountCreatorModel.ObsCreateAccountInfo.IndexOf(accountToUpdate);
                                        accountToUpdate.Email = accountInfo.Email;
                                        accountToUpdate.Password = accountInfo.Password;
                                        accountToUpdate.Age = accountInfo.Age;
                                        accountToUpdate.Gender = accountInfo.Gender;
                                        accountToUpdate.Id = accountInfo.Id;
                                        accountToUpdate.IsCheckedToAccountManager = accountInfo.IsCheckedToAccountManager;
                                        AccountCreatorModel.ObsCreateAccountInfo[index] = accountToUpdate;
                                    }
                                }
                            });
                        }
                        else if (AccountCreatorModel.ObsCreateAccountInfo.All(info => info.Id != accountInfo.Id))
                        {
                            AccountCreatorModel.ObsCreateAccountInfo.Add(new CreateAccountInfo
                            {
                                Id = accountInfo.Id,
                                Email = accountInfo.Email,
                                Password = accountInfo.Password,
                                Age = accountInfo.Age,
                                Gender = accountInfo.Gender,
                                IsCheckedToAccountManager= accountInfo.IsCheckedToAccountManager
                            });
                            accountAdded = true;
                        }
                        else
                        {
                            var accountToUpdate = AccountCreatorModel.ObsCreateAccountInfo.FirstOrDefault(x=>x.Id == accountInfo.Id);
                            if(accountToUpdate != null)
                            {
                                var index = AccountCreatorModel.ObsCreateAccountInfo.IndexOf(accountToUpdate);
                                accountToUpdate.Email = accountInfo.Email;
                                accountToUpdate.Password = accountInfo.Password;
                                accountToUpdate.Age = accountInfo.Age;
                                accountToUpdate.Gender = accountInfo.Gender;
                                accountToUpdate.Id = accountInfo.Id;
                                AccountCreatorModel.ObsCreateAccountInfo[index] = accountToUpdate;
                            }
                        }
                        AccountCreatorModel.CreateAccountInfo = new CreateAccountInfo();
                        CurrentSelectedTab = 1;
                        if(accountAdded)
                            ToasterNotification.ShowSuccess("Account Details Addded Succesfully");
                        else
                            ToasterNotification.ShowSuccess("Account Details Updated Succesfully");
                    }
                }
                catch
                {
                    // ignored
                }
            });
            
        }
    }
}
