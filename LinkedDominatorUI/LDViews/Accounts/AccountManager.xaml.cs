using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDViewModel.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LinkedDominatorUI.LDViews.Accounts
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
          
            AccountManagerControl.DataContext = AccountManagerViewModel;
            //TotalFollowers.Badge = Global.GetTotalFollowers();
            //TotalFollowings.Badge = Global.GetTotalFollowings();

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
            AccountModel ObjAccountModel = ((FrameworkElement)sender).DataContext as AccountModel;
          //  AccountManagerViewModel.UpdateAccount(ObjAccountModel);
            //TotalFollowers.Badge = Global.GetTotalFollowers();
            //TotalFollowings.Badge = Global.GetTotalFollowings();
        }
    }
}
