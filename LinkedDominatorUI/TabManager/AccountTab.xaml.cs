using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel;
using DominatorUIUtility.Views;

namespace LinkedDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for AccountTab.xaml
    /// </summary>
    public partial class AccountTab : UserControl
    {
        public AccountTab(AccessorStrategies strategies)
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyAccountsManager").ToString(),
                        Content = new Lazy<UserControl>(() =>
                            AccountManager.GetSingletonAccountManager("AccountManager", null, SocialNetworks.LinkedIn))
                    },
                    new TabItemTemplates
                    {
                        Title = "LangKeyAccountGrowth".FromResourceDictionary(),
                        Content = new Lazy<UserControl>(
                            () => InstanceProvider.GetInstance<AccountGrowthControl>())
                    }
                    //new TabItemTemplates
                    //{
                    // Title=FindResource("LangKeyDashboardBeta").ToString(),
                    // Content = new Lazy<UserControl>(() => new DashBoard())
                    //},

                    //new TabItemTemplates
                    //{
                    // Title =FindResource("LangKeyAccountStatsBeta").ToString(),
                    // Content = new Lazy<UserControl>(()=> new AccountStats())
                    //}
                };
                AccountTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        private void AccountManager_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBlockDetails = (FrameworkElement) sender as TextBlock;
            if (textBlockDetails?.Text == FindResource("LangKeyAccountsManager").ToString() &&
                AccountManagerViewModel.GetSingletonAccountManagerViewModel().LastControlType != "AccountDetail")
                AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl =
                    AccountCustomControl.GetAccountCustomControl(SocialNetworks.LinkedIn);
        }
    }
}