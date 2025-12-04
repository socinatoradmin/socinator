using DominatorHouseCore.Utility;
using GramDominatorCore.GDViewModel.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using GramDominatorCore.GDModel;
using DominatorHouseCore;
using DominatorHouseCore.FileManagers;

namespace GramDominatorUI.CustomControl
{
    /// <summary>
    /// Interaction logic for SelectAccountControl.xaml
    /// </summary>
    public partial class SelectAccountControl : UserControl
    {

        public AccountManagerViewModel objAccountManagerViewModel = (AccountManagerViewModel)AccountManagerViewModel.GetAccountManagerViewModel().Clone();

        public SelectAccountControl(List<string> lstSelectedAccount)
        {

            InitializeComponent();

            this.DataContext = objAccountManagerViewModel;
                 
            //Read all accounts from bin files
            var items = AccountsFileManager.GetAll();

            //Iterate all account model from bin file and add to AccountManagerViewModel object
            objAccountManagerViewModel.LstAccountModel.Clear();
            items.ForEach(x => objAccountManagerViewModel.LstAccountModel.Add(x));

            //Get the group Url 
            objAccountManagerViewModel.Groups.Clear();
            objAccountManagerViewModel.LstAccountModel.Where(x => x.AccountBaseModel.AccountGroup.Content != null).Distinct().ToList().
                ForEach(x => objAccountManagerViewModel.Groups.Add(x.AccountBaseModel.AccountGroup));

            //Select the account which is already selected
            objAccountManagerViewModel.LstAccountModel?.ToList().ForEach(x =>
                {
                    x.IsAccountSelected = lstSelectedAccount.Contains(x.AccountBaseModel.UserName);
                });

            //Assign the default view to ICollectionView         
            objAccountManagerViewModel.AccountsDetailCollection = CollectionViewSource.GetDefaultView(objAccountManagerViewModel.LstAccountModel.ToList());
        }


        public SelectAccountControl()
        {
            InitializeComponent();

            this.DataContext = objAccountManagerViewModel;

            var accountModels = AccountsFileManager.GetAll();

            try
            {
                foreach (var item in accountModels)
                    objAccountManagerViewModel.LstAccountModel.Add(item);                

                foreach (var group in objAccountManagerViewModel.LstAccountModel
                    .Where(x => x.AccountBaseModel.AccountGroup.Content != null).ToList())
                {
                    objAccountManagerViewModel.Groups.Add(group.AccountBaseModel.AccountGroup);
                }

                objAccountManagerViewModel.Groups.Clear();
                //Get the group Url 
                objAccountManagerViewModel.LstAccountModel.Where(x => x.AccountBaseModel.AccountGroup.Content != null)
                    .Distinct().ToList().ForEach(x => objAccountManagerViewModel.Groups.Add(x.AccountBaseModel.AccountGroup));


                objAccountManagerViewModel.AccountsDetailCollection =
                    CollectionViewSource.GetDefaultView(objAccountManagerViewModel.LstAccountModel.ToList());
                foreach (var selectedAccounts in lstSelectedAccount)
                    objAccountManagerViewModel.LstAccountModel.FirstOrDefault(x => x.AccountBaseModel.UserName == selectedAccounts)
                        .IsAccountSelected = true;

                AccountsFileManager.UpdateAccounts(objAccountManagerViewModel.LstAccountModel);
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        private static SelectAccountControl CurrentSelectAccountControl { get; set; } = null;

        /// <summary>
        /// GetSingeltonObjectSelectAccountControl is used to get the object of the current user control,
        /// if object is already created then its wont create a new object object, simply it returns already created object,
        /// otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static SelectAccountControl GetSingeltonObjectSelectAccountControl()
        {
            return CurrentSelectAccountControl ?? (CurrentSelectAccountControl = new SelectAccountControl());
        }
        private void chkgroup_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                objAccountManagerViewModel.LstAccountModel.Where(x => x.AccountBaseModel.AccountGroup.IsContentSelected == true).Select(x => { x.IsAccountSelected = true; return x; }).ToList();
                AccountGroupSelected();
            }
            catch (Exception ex)
            {

            }
        }


        private void chkgroup_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                objAccountManagerViewModel.LstAccountModel.Where(x => x.AccountBaseModel.AccountGroup.IsContentSelected == false).Select(x => { x.IsAccountSelected = false; return x; }).ToList();
                AccountGroupSelected();
            }
            catch (Exception ex)
            {

            }
        }



        private void chkSelectAllAccount_Checked(object sender, RoutedEventArgs e)
        {
            objAccountManagerViewModel.LstAccountModel.Select(x => { x.IsAccountSelected = true; return x; }).ToList();
        }

        private void chkSelectAllAccount_Unchecked(object sender, RoutedEventArgs e)
        {
            objAccountManagerViewModel.LstAccountModel.Select(x => { x.IsAccountSelected = false; return x; }).ToList();
        }

        private void cmbAllGroups_DropDownClosed(object sender, EventArgs e)
        {
            AccountGroupSelected();
        }

        private void AccountGroupSelected()
        {
            try
            {
                int selectedGroups = objAccountManagerViewModel.Groups.Count(x => x.IsContentSelected == true);
                cmbAllGroups.Text = $"{selectedGroups} group(s) selected";
            }
            catch (Exception ex)
            {

            }
        }
        public static ObservableCollectionBase<string> lstSelectedAccount = new ObservableCollectionBase<string>();
        public ObservableCollectionBase<string> GetSelectedAccount()
        {
            lstSelectedAccount = new ObservableCollectionBase<string>(objAccountManagerViewModel.LstAccountModel.Where(x => x.IsAccountSelected == true).Select(x => x.AccountBaseModel.UserName).ToList());
            return lstSelectedAccount;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            switch (cmbSearchFilter.SelectedIndex)
            {
                case 0:
                    objAccountManagerViewModel.AccountsDetailCollection.Filter = FilterByGroupName;
                    break;
                case 1:
                    objAccountManagerViewModel.AccountsDetailCollection.Filter = FilterByAccounts;

                    break;
                case 2:
                    objAccountManagerViewModel.AccountsDetailCollection.Filter = FilterByNosOfFollower;
                    break;
                case 3:
                    objAccountManagerViewModel.AccountsDetailCollection.Filter = FilterByNosOfFollowing;
                    break;
                default:
                    objAccountManagerViewModel.AccountsDetailCollection.Filter = FilterByGroupName;
                    break;
            }

        }

        private bool FilterByGroupName(object groupName)
        {
            try
            {
                DominatorAccountModel objDominatorAccountModel = groupName as DominatorAccountModel;
                return objDominatorAccountModel.AccountBaseModel.AccountGroup.Content.IndexOf(txtSearch.Text, StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception ex)
            {
            }

            return false;
        }
        private bool FilterByAccounts(object AccountName)
        {
            try
            {
                DominatorAccountModel objDominatorAccountModel = AccountName as DominatorAccountModel;
                return objDominatorAccountModel.AccountBaseModel.UserName.IndexOf(txtSearch.Text, StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception ex)
            {
            }
            return false;
        }
        private bool FilterByNosOfFollower(object NosOfFollower)
        {
            try
            {
               // DominatorAccountModel objDominatorAccountModel = NosOfFollower as DominatorAccountModel;
                AccountModel objDominatorAccountModel = NosOfFollower as AccountModel;
                return objDominatorAccountModel.FollowersCount.ToString().IndexOf(txtSearch.Text, StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception ex)
            {
            }
            return false;
        }
        private bool FilterByNosOfFollowing(object NosOfFollowing)
        {
            try
            {
                //DominatorAccountModel objDominatorAccountModel = NosOfFollowing as DominatorAccountModel;
                AccountModel objDominatorAccountModel = NosOfFollowing as AccountModel;
                return objDominatorAccountModel.FollowingCount.ToString().IndexOf(txtSearch.Text, StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception ex)
            {
            }
            return false;
        }
    }
}
