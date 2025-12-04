using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.WatchPartyInviterTools;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for GroupInviterConfiguration.xaml
    /// </summary>
    public partial class WatchPartyInviterConfiguration
    {
        public WatchPartyInviterConfiguration()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(WatchPartyInviterTools.GetSingeltonObjectWatchPartyInviterTools)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.WatchPartyInviter))
                }
            };
            SendRequesttabs.ItemsSource = tabItems;
        }
    }
}