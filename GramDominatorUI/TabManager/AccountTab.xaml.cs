using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel;
using DominatorUIUtility.Views;

namespace GramDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for AccountTab.xaml
    /// </summary>
    public partial class AccountTab : UserControl
    {
        private static AccountTab objAccountTab = null;

        public AccountTab(AccessorStrategies strategies)
        {
            InitializeComponent();
            //strategiee = strategies;
            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyAccountsManager") != null
                        ? Application.Current.FindResource("LangKeyAccountsManager").ToString()
                        : "Accounts Manager",
                    Content = new Lazy<UserControl>(() =>
                        AccountManager.GetSingletonAccountManager("AccountManager", null, SocialNetworks.Instagram))
                    //ImagePath= (Visual)Application.Current.FindResource("AccountManagerIcon") as Canvas
                },
                new TabItemTemplates
                {
                    Title = "LangKeyAccountGrowth".FromResourceDictionary(),
                    Content = new Lazy<UserControl>(() => InstanceProvider.GetInstance<AccountGrowthControl>())
                    //ImagePath= (Visual)Application.Current.FindResource("AccountGrowthIcon") as Canvas
                }
                //new TabItemTemplates
                //{
                //    Title = Application.Current.FindResource("LangKeyCreateAccounts") != null
                //        ? Application.Current.FindResource("LangKeyCreateAccounts").ToString()
                //        : "Create Accounts",
                //    Content = new Lazy<UserControl>(() =>
                //        CreateAccount.GetSingletonCreateAccount())
                //}
                //new TabItemTemplates
                //{
                //    Title=FindResource("langDashboardBeta").ToString(),
                //    Content = new Lazy<UserControl>(() => new DashBoard())
                //},
                //new TabItemTemplates
                //{
                //    Title =FindResource("langAccountStatsBeta").ToString(),
                //    Content = new Lazy<UserControl>(()=> new AccountStats())
                //}
            };

            AccountTabs.ItemsSource = items;
        }

        //public static AccountTab GetSingeltonObjectGrowFollowersTab()
        //{
        //    return objAccountTab ?? (objAccountTab = new AccountTab(strategiee));
        //}
        private void AccountManager_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBlockDetails = (FrameworkElement) sender as TextBlock;
            if (textBlockDetails?.Text == FindResource("LangKeyAccountsManager").ToString() &&
                AccountManagerViewModel.GetSingletonAccountManagerViewModel().LastControlType != "AccountDetail")
                AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl =
                    AccountCustomControl.GetAccountCustomControl(SocialNetworks.Instagram);
        }
    }
}