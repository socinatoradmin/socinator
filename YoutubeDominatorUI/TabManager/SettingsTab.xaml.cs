using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace YoutubeDominatorUI.TabManager
{
    public partial class SettingsTab
    {
        private static SettingsTab _objSettingsTab;

        public SettingsTab()
        {
            InitializeComponent();
            _objSettingsTab = this;

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

        public static SettingsTab GetSingeltonObject_SettingsTab()
        {
            return _objSettingsTab ?? (_objSettingsTab = new SettingsTab());
        }

        public void SetIndex(int index)
        {
            SettingsTabs.SelectedIndex = index;
        }
    }
}