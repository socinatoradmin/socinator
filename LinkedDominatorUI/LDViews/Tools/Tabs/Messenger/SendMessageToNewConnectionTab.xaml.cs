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
    ///     Interaction logic for SendMessageToNewConnectionTab.xaml
    /// </summary>
    public partial class SendMessageToNewConnectionTab : UserControl
    {
        private static SendMessageToNewConnectionTab objSendMessageToNewConnectionTab;

        public SendMessageToNewConnectionTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConfiguration").ToString(),
                        Content = new Lazy<UserControl>(SendMessageToNewConnectionConfiguration
                            .GetSingeltonObjecSendMessageToNewConnectionConfiguration)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(
                            () => new AccountReport(ActivityType.SendMessageToNewConnection))
                    }
                };
                SendMessageToNewConnectionTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        public static SendMessageToNewConnectionTab GetSingletonObjectSendMessageToNewConnectionTab()
        {
            return objSendMessageToNewConnectionTab ??
                   (objSendMessageToNewConnectionTab = new SendMessageToNewConnectionTab());
        }
    }
}