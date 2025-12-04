using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;

namespace TwtDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for SettingsTab.xaml
    /// </summary>
    public partial class SettingsTab : UserControl
    {
        public SettingsTab()
        {
            InitializeComponent();

            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyBlacklistedUsers").ToString(),
                    Content = new Lazy<UserControl>(() => new BlacklistUserControl())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyWhitelistedUsers").ToString(),
                    Content = new Lazy<UserControl>(() => new WhitelistuserControl())
                }
            };

            SettingsTabs.ItemsSource = tabItems;
        }
    }
}