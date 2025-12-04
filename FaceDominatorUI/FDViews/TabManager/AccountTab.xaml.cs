using CommonServiceLocator;
using DominatorHouseCore;
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

namespace FaceDominatorUI.FDViews.TabManager
{
    /// <summary>
    ///     Interaction logic for AccountTab.xaml
    /// </summary>
    public partial class AccountTab
    {
        public AccountTab()
        {
            InitializeComponent();
            InitializeTabs();
        }

        public List<TabItemTemplates> TabItems { get; set; }

        private void InitializeTabs()
        {
            TabItems = InitializeAllTabs();
            AccountTabs.ItemsSource = TabItems;
        }

        public List<TabItemTemplates> InitializeAllTabs()
        {
            try
            {
                TabItems = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyAccountManager").ToString(),
                        Content = new Lazy<UserControl>(() =>
                            AccountManager.GetSingletonAccountManager("AccountManager", null, SocialNetworks.Facebook))
                    },
                    new TabItemTemplates
                    {
                        Title = "LangKeyAccountGrowth".FromResourceDictionary(),
                        Content = new Lazy<UserControl>(
                            () => InstanceProvider.GetInstance<AccountGrowthControl>())
                    }
                };
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return TabItems;
        }

        private void AccountManager_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBlockDetails = (FrameworkElement)sender as TextBlock;
            if (textBlockDetails?.Text == FindResource("LangKeyAccountsManager").ToString() &&
                AccountManagerViewModel.GetSingletonAccountManagerViewModel().LastControlType != "AccountDetail")
                AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl =
                    AccountCustomControl.GetAccountCustomControl(SocialNetworks.Facebook);
        }
    }
}