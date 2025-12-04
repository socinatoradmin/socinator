using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace TumblrDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for SettingsTab.xaml
    /// </summary>
    public partial class SettingsTab
    {
        private static SettingsTab _objSettingsTab;

        public SettingsTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyBlacklistedUsers") == null
                        ? "Blacklisted Users"
                        : Application.Current.FindResource("LangKeyBlacklistedUsers")?.ToString(),
                    Content = new Lazy<UserControl>(() => new BlacklistUserControl())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyWhitelistedUsers") == null
                        ? "Whitelist Users"
                        : Application.Current.FindResource("LangKeyWhitelistedUsers")?.ToString(),
                    Content = new Lazy<UserControl>(() => new WhitelistuserControl())
                }
            };
            SettingsTabs.ItemsSource = tabItems;
        }

        public static SettingsTab GetSingeltonSettingsTab()
        {
            return _objSettingsTab ?? (_objSettingsTab = new SettingsTab());
        }
    }
}