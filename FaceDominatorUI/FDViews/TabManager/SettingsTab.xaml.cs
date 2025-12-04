using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager
{
    /// <summary>
    ///     Interaction logic for SettingsTab.xaml
    /// </summary>
    public partial class SettingsTab
    {
        public SettingsTab()
        {
            InitializeComponent();
            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyBlacklist").ToString(),
                    Content = new Lazy<UserControl>(() => new BlacklistUserControl())
                },

                new TabItemTemplates
                {
                    Title = FindResource("LangKeyWhitelist").ToString(),
                    Content = new Lazy<UserControl>(() => new WhitelistuserControl())
                }
            };

            SettingsTabs.ItemsSource = items;
        }

        private static SettingsTab CurrentFbSettingsTab { get; set; }

        public static SettingsTab GetSingeltonObjectFbSettingsTab()
        {
            return CurrentFbSettingsTab ?? (CurrentFbSettingsTab = new SettingsTab());
        }

        public void SetIndex(int tabIndex)
        {
            SettingsTabs.SelectedIndex = tabIndex;
        }
    }
}