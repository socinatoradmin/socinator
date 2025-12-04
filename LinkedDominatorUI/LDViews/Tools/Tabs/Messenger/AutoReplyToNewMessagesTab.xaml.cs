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
    ///     Interaction logic for AutoReplyToNewMessagesTab.xaml
    /// </summary>
    public partial class AutoReplyToNewMessagesTab : UserControl
    {
        private static AutoReplyToNewMessagesTab objAutoReplyToNewMessagesTab;

        public AutoReplyToNewMessagesTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConfiguration").ToString(),
                        Content = new Lazy<UserControl>(AutoReplyToNewMessagesConfiguration
                            .GetSingeltonObjectAutoReplyToNewMessagesConfiguration)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.AutoReplyToNewMessage))
                    }
                };
                AutoReplyToNewMessagesTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static AutoReplyToNewMessagesTab GetSingeltonObjectAutoReplyToNewMessagesTab()
        {
            return objAutoReplyToNewMessagesTab ?? (objAutoReplyToNewMessagesTab = new AutoReplyToNewMessagesTab());
        }
    }
}