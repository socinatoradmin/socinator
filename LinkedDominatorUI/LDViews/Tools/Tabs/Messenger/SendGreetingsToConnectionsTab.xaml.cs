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
    ///     Interaction logic for SendGreetingsToConnectionsTab.xaml
    /// </summary>
    public partial class SendGreetingsToConnectionsTab : UserControl
    {
        private static SendGreetingsToConnectionsTab objSendGreetingsToConnectionsTab;

        public SendGreetingsToConnectionsTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConfiguration").ToString(),
                        Content = new Lazy<UserControl>(SendGreetingsToConnectionsConfiguration
                            .GetSingeltonObjectSendGreetingsToConnectionsConfiguration)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(
                            () => new AccountReport(ActivityType.SendGreetingsToConnections))
                    }
                };
                SendGreetingsToConnectionsTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        public static SendGreetingsToConnectionsTab GetSingletonSendGreetingsToConnectionsTab()
        {
            return objSendGreetingsToConnectionsTab ??
                   (objSendGreetingsToConnectionsTab = new SendGreetingsToConnectionsTab());
        }
    }
}