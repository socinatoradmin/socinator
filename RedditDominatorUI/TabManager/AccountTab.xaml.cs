using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel;
using DominatorUIUtility.Views;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RedditDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for AccountTab.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class AccountTab : UserControl
    {
        public AccountTab(AccessorStrategies strategies)
        {
            InitializeComponent();

            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyAccountsManager").ToString(),
                    Content = new Lazy<UserControl>(() =>
                        AccountManager.GetSingletonAccountManager("AccountManager", null, SocialNetworks.Reddit))
                },
                //new TabItemTemplates
                //{
                //    Title = "LangKeyAccountGrowth".FromResourceDictionary(),
                //    Content = new Lazy<UserControl>(() => InstanceProvider.GetInstance<AccountGrowthControl>())
                //}
            };
            AccountTabs.ItemsSource = items;
        }

        private void AccountManager_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBlockDetails = (FrameworkElement)sender as TextBlock;
            if (textBlockDetails?.Text == FindResource("LangKeyAccountsManager").ToString() &&
                AccountManagerViewModel.GetSingletonAccountManagerViewModel().LastControlType != "AccountDetail")
                AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl =
                    AccountCustomControl.GetAccountCustomControl(SocialNetworks.Reddit);
        }
    }
}