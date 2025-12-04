using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RedditDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for SettingsTab.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class SettingsTab : UserControl
    {
        private static SettingsTab _objSettingsTab;

        public SettingsTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyBlacklistusers") == null
                        ? "Blacklist Users"
                        : Application.Current.FindResource("LangKeyBlacklistusers")?.ToString(),
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

        public static SettingsTab GetSingeltonSettingsTab()
        {
            return _objSettingsTab ?? (_objSettingsTab = new SettingsTab());
        }
    }
}