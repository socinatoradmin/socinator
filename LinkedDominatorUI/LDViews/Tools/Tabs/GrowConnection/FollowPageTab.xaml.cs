using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using LinkedDominatorUI.CustomControl;
using LinkedDominatorUI.LDViews.Tools.GrowConnection;

namespace LinkedDominatorUI.LDViews.Tools.Tabs.GrowConnection
{
    /// <summary>
    ///     Interaction logic for FollowPageTab.xaml
    /// </summary>
    public partial class FollowPageTab : UserControl
    {
        private static FollowPageTab objFollowPageTab;

        public FollowPageTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConfiguration").ToString(),
                        Content = new Lazy<UserControl>(() =>
                            FollowPageConfiguration.GetSingeltonObjectFollowPageConfiguration())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.FollowPages))
                    }
                };
                FollowPageTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static FollowPageTab GetSingeltonObjectFollowPageTab()
        {
            return objFollowPageTab ?? (objFollowPageTab = new FollowPageTab());
        }
    }
}