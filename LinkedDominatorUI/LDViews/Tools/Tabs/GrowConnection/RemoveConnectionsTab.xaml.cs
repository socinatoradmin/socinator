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
    ///     Interaction logic for RemoveConnectionsTab.xaml
    /// </summary>
    public partial class RemoveConnectionsTab : UserControl
    {
        private static RemoveConnectionsTab objRemoveConnectionsTab;

        public RemoveConnectionsTab()
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
                            RemoveConnectionConfiguration.GetSingeltonObjectRemoveConnectionsConfiguration())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.RemoveConnections))
                    }
                };
                RemoveConnectionsTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static RemoveConnectionsTab GetSingeltonObjectRemoveConnectionsTab()
        {
            return objRemoveConnectionsTab ?? (objRemoveConnectionsTab = new RemoveConnectionsTab());
        }
    }
}