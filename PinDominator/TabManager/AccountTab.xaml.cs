using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel;
using DominatorUIUtility.Views;
using PinDominator.PDViews.CreateAccount;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PinDominator.TabManager
{
    /// <summary>
    ///     Interaction logic for AccountTab.xaml
    /// </summary>
    public partial class AccountTab
    {
        public AccountTab()
        {
            InitializeComponent();

            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyAccountsManager") == null
                        ? "Accounts Manager"
                        : Application.Current.FindResource("LangKeyAccountsManager")?.ToString(),
                    Content = new Lazy<UserControl>(() =>
                        AccountManager.GetSingletonAccountManager("AccountManager", null, SocialNetworks.Pinterest))
                },
                //new TabItemTemplates
                //{
                //    Title = "LangKeyAccountGrowth".FromResourceDictionary(),
                //    Content = new Lazy<UserControl>(() => InstanceProvider.GetInstance<AccountGrowthControl>())
                //},
                new TabItemTemplates
                {
                    Title = "Account Creator",
                    Content = new Lazy<UserControl>(AccountCreator.GetSingletonAccountCreator)
                }
            };
            AccountTabs.ItemsSource = items;
        }

        private void AccountManager_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBlockDetails = (FrameworkElement) sender as TextBlock;
            if (textBlockDetails?.Text == FindResource("LangKeyAccountsManager").ToString() &&
                AccountManagerViewModel.GetSingletonAccountManagerViewModel().LastControlType != "AccountDetail")
                AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl =
                    AccountCustomControl.GetAccountCustomControl(SocialNetworks.Pinterest);
        }

        public void SetIndex(int index)
        {
            AccountTabs.SelectedIndex = index;
        }
    }
}