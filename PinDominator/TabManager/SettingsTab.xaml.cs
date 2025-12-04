using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PinDominator.TabManager
{
    /// <summary>
    ///     Interaction logic for SettingsTab.xaml
    /// </summary>
    public partial class SettingsTab
    {
        public SettingsTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyBlacklistedUsers") == null
                        ? "Blacklist Users"
                        : Application.Current.FindResource("LangKeyBlacklistedUsers")?.ToString(),
                    Content = new Lazy<UserControl>(() => new BlacklistUserControl())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyWhitelistUsers") == null
                        ? "Whitelist Users"
                        : Application.Current.FindResource("LangKeyWhitelistUsers")?.ToString(),
                    Content = new Lazy<UserControl>(() => new WhitelistuserControl())
                }
            };
            SettingsTabs.ItemsSource = tabItems;
        }
    }
}