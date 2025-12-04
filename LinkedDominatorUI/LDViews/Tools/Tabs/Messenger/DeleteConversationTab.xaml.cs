using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using LinkedDominatorUI.CustomControl;
using LinkedDominatorUI.LDViews.Tools.Messenger;

namespace LinkedDominatorUI.LDViews.Tools.Tabs.Messenger
{
    /// <summary>
    ///     Interaction logic for DeleteConversationTab.xaml
    /// </summary>
    public partial class DeleteConversationTab : UserControl
    {
        private static DeleteConversationTab ObjDeleteConversationTab;

        public DeleteConversationTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConfiguration").ToString(),
                        Content = new Lazy<UserControl>(DeleteConversationsConfiguration
                            .GetSingeltonObjectDeleteConversationsConfiguration)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.DeleteConversations))
                    }
                };
                DeleteConversationsTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        public static DeleteConversationTab GetSingeltonObjectDeleteConversationsTab()
        {
            return ObjDeleteConversationTab ?? (ObjDeleteConversationTab = new DeleteConversationTab());
        }
    }
}