using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;

namespace QuoraDominatorUI.TabManager
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
            try
            {
                var tabItems = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyBlacklistusers").ToString(),
                        Content = new Lazy<UserControl>(() => new BlacklistUserControl())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyWhitelistUsers").ToString(),
                        Content = new Lazy<UserControl>(() => new WhitelistuserControl())
                    }
                };
                SettingsTabs.ItemsSource = tabItems;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static SettingsTab GetSingeltonSettingsTab()
        {
            return _objSettingsTab ?? (_objSettingsTab = new SettingsTab());
        }
    }
}