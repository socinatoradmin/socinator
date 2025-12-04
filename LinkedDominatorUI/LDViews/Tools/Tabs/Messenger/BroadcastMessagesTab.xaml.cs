using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using LinkedDominatorUI.CustomControl;
using LinkedDominatorUI.LDViews.Tools.Messenger;

namespace LinkedDominatorUI.LDViews.Tools.Tabs
{
    /// <summary>
    ///     Interaction logic for BroadcastMessagesTab.xaml
    /// </summary>
    public partial class BroadcastMessagesTab : UserControl
    {
        private static BroadcastMessagesTab objBroadcastMessagesTab;

        public BroadcastMessagesTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConfiguration").ToString(),
                        Content = new Lazy<UserControl>(BroadcastMessagesConfiguration
                            .GetSingeltonObjectBroadcastMessagesConfiguration)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.BroadcastMessages))
                    }
                };
                BroadcastMessagesTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        public static BroadcastMessagesTab GetSingletonObjectBroadcastMessagesTab()
        {
            return objBroadcastMessagesTab ?? (objBroadcastMessagesTab = new BroadcastMessagesTab());
        }
    }
}