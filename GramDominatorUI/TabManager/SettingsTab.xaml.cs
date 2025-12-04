using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;

namespace GramDominatorUI.TabManager
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
                    Title = Application.Current.FindResource("LangKeyBlacklistusers") != null
                        ? Application.Current.FindResource("LangKeyBlacklistusers").ToString()
                        : "Blacklistusers",
                    Content = new Lazy<UserControl>(() => new BlacklistUserControl())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyWhitelistUsers") != null
                        ? Application.Current.FindResource("LangKeyWhitelistUsers").ToString()
                        : "Whitelist Users",
                    Content = new Lazy<UserControl>(() => new WhitelistuserControl())
                }
                //new TabItemTemplates
                //{
                //    Title=FindResource("LangKeySettings").ToString(),
                //    Content=new Lazy<UserControl>(()=>new Setting())
                //}
            };
            SettingsTabs.ItemsSource = tabItems;
        }
    }
}