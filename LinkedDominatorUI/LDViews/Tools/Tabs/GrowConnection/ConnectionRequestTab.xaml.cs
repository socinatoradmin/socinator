using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using LinkedDominatorUI.CustomControl;
using LinkedDominatorUI.LDViews.Tools.GrowConnection;

namespace LinkedDominatorUI.LDViews.Tools.Tabs
{
    /// <summary>
    ///     Interaction logic for ConnectionRequestTab.xaml
    /// </summary>
    public partial class ConnectionRequestTab : UserControl
    {
        private static ConnectionRequestTab objConnectionRequestTab;

        public ConnectionRequestTab()
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
                            ConnectionRequestConfiguration.GetSingeltonObjectConnectionRequestConfiguration())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.ConnectionRequest))
                    }
                };
                ConnectionRequestTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static ConnectionRequestTab GetSingeltonObjectConnectionRequestTab()
        {
            return objConnectionRequestTab ?? (objConnectionRequestTab = new ConnectionRequestTab());
        }
    }
}