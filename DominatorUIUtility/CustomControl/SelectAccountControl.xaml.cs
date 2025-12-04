using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for SelectAccountControl.xaml
    /// </summary>
    public partial class SelectAccountControl : INotifyPropertyChanged
    {
        private SelectAccountViewModel _objAccountViewModel = new SelectAccountViewModel();


        public SelectAccountControl(ICollection<string> lstSelectedAccount)
        {
            InitializeComponent();

            DataContext = ObjAccountViewModel;

            var savedAccounts = InstanceProvider.GetInstance<IAccountCollectionViewModel>()
                .BySocialNetwork(SocinatorInitialize.ActiveSocialNetwork);

            ObjAccountViewModel.LstSelectAccount.Clear();
            savedAccounts.ForEach(x =>
            {
                //add only account status should be success/UpdatingDetails

                if (x.AccountBaseModel.Status == AccountStatus.Success ||
                    x.AccountBaseModel.Status == AccountStatus.UpdatingDetails)
                {
                    ObjAccountViewModel.LstSelectAccount.Add(new SelectAccountModel
                    {
                        UserName = x.UserName,
                        GroupName = x.AccountBaseModel.AccountGroup.Content,
                        AccountNikeName = x.AccountBaseModel.AccountName,
                        BrowserAutomation = x.IsRunProcessThroughBrowser
                    });
                    if (ObjAccountViewModel.SelectAccountModel.Groups.Any(group =>
                            group.Content == x.AccountBaseModel.AccountGroup.Content) == false)
                        ObjAccountViewModel.SelectAccountModel.Groups.Add(
                            new ContentSelectGroup
                            {
                                Content = x.AccountBaseModel.AccountGroup.Content
                            });
                }
            });

            //Select the account which is already selected
            ObjAccountViewModel.LstSelectAccount?.ToList().ForEach(x =>
            {
                x.IsAccountSelected = lstSelectedAccount.Contains(x.UserName);
            });

            //Assign the view to ICollectionView         
            ObjAccountViewModel.AccountCollectionView =
                CollectionViewSource.GetDefaultView(ObjAccountViewModel.LstSelectAccount);
        }

        private bool IsUnCheckedFromAccountDetails { get; set; }

        public SelectAccountViewModel ObjAccountViewModel
        {
            get => _objAccountViewModel;
            set
            {
                if (_objAccountViewModel == value)
                    return;
                _objAccountViewModel = value;
                OnPropertyChanged(nameof(ObjAccountViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void chkgroup_Checked(object sender, RoutedEventArgs e)
        {
            ObjAccountViewModel.SelectDeselectAccountByGroup(true);
            ObjAccountViewModel.AccountGroupSelected();
        }

        private void chkgroup_Unchecked(object sender, RoutedEventArgs e)
        {
            if (IsUnCheckedFromAccountDetails)
                return;

            ObjAccountViewModel.SelectDeselectAccountByGroup(false);
            ObjAccountViewModel.AccountGroupSelected();
        }

        private void cmbAllGroups_DropDownClosed(object sender, EventArgs e)
        {
            ObjAccountViewModel.AccountGroupSelected();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterAccount();
            ObjAccountViewModel.ChangeSelectionAfterFilter();
        }

        private void FilterAccount()
        {
            try
            {
                if(ObjAccountViewModel!=null && ObjAccountViewModel.AccountCollectionView != null )
                switch (cmbSearchFilter.SelectedIndex)
                {
                    case 0:
                        ObjAccountViewModel.AccountCollectionView.Filter = FilterByGroupName;
                        break;
                    case 1:
                        ObjAccountViewModel.AccountCollectionView.Filter = FilterByAccounts;
                        break;
                    case 2:
                        ObjAccountViewModel.AccountCollectionView.Filter = FilterByAccountsNikeName;
                        break;
                    default:
                        if (!string.IsNullOrEmpty(txtSearch.Text))
                            ObjAccountViewModel.AccountCollectionView.Filter = FilterByAccounts;
                        else
                            ObjAccountViewModel.AccountCollectionView.Filter = null;
                        break;
                }
            }
            catch
            {
            }
        }

        private bool FilterByGroupName(object groupName)
        {
            try
            {
                var objAccountViewModel = groupName as SelectAccountModel;

                return objAccountViewModel != null &&
                       objAccountViewModel.GroupName.IndexOf(txtSearch.Text,
                           StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private bool FilterByAccounts(object accountName)
        {
            try
            {
                var objAccountViewModel = accountName as SelectAccountModel;
                return objAccountViewModel != null &&
                       objAccountViewModel.UserName.IndexOf(txtSearch.Text,
                           StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private bool FilterByAccountsNikeName(object model)
        {
            try
            {
                var objAccountViewModel = model as SelectAccountModel;
                return objAccountViewModel != null &&
                       objAccountViewModel.AccountNikeName.IndexOf(txtSearch.Text,
                           StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        public ObservableCollectionBase<string> GetSelectedAccount()
        {
            return new ObservableCollectionBase<string>(ObjAccountViewModel.LstSelectAccount
                .Where(x => x.IsAccountSelected).Select(x => x.UserName).ToList());
        }

        private void Filter(object sender, SelectionChangedEventArgs e)
        {
            FilterAccount();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}