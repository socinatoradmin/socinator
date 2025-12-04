using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDViewModel.Accounts;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;


namespace GramDominatorUI.GDViews.Accounts
{
    /// <summary>
    /// Interaction logic for AccountManager.xaml
    /// </summary>
    public partial class AccountManager : UserControl
    {
        public AccountManagerViewModel AccountManagerViewModel { get; set; } = AccountManagerViewModel.GetAccountManagerViewModel();

        public AccountManager()
        {

            InitializeComponent();
           // AccountManagerViewModel.InitializeAccountDetails();
            AccountManagerControl.DataContext = AccountManagerViewModel;
         }


        private static AccountManager _objAccountManager = null;

        public static AccountManager GetSingeltonObjectAccountManager()
        {
            return _objAccountManager ?? (_objAccountManager = new AccountManager());
        }

        private void editContextMenu_Click(object sender, RoutedEventArgs e)
        {
            AccountManagerViewModel.EditAccount(sender);
        }

        private void MangeblacklistedContextMenu_Click(object sender, RoutedEventArgs e)
        {
            BlacklistUserControl objBlacklistUserControl = new BlacklistUserControl();

            var window = new Window()
            {
                Content = objBlacklistUserControl,
                Topmost = true,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight
            };

            window.ShowDialog();
        }

        private void MangewhitelistUserContextMenu_Click(object sender, RoutedEventArgs e)
        {
            WhitelistuserControl objWhitelistuserControl = new WhitelistuserControl();

            var window = new Window()
            {
                Content = objWhitelistuserControl,
                Topmost = true,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight
            };
            window.ShowDialog();
        }

        private void DeleteSingleContextMenu_Click(object sender, RoutedEventArgs e)
        {
            AccountManagerViewModel.DeleteAccountByContextMenu(sender);
        }

        private void chkgroup_Checked(object sender, RoutedEventArgs e)
        {
            AccountManagerViewModel.SelectAccountByGroup(sender);
        }

        private void chkgroup_Unchecked(object sender, RoutedEventArgs e)
        {
            AccountManagerViewModel.SelectAccountByGroup(sender);
        }

        private void MenuCheckAccount_OnClick(object sender, RoutedEventArgs e)
        {
            DominatorAccountModel objDominatorAccountModel =
                ((FrameworkElement) sender).DataContext as DominatorAccountModel;
            AccountManagerViewModel.UpdateAccount(objDominatorAccountModel);
        }


    }
}
