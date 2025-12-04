using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using QuoraDominatorUI.QDViews.Messages;

namespace QuoraDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for MessagesTab.xaml
    /// </summary>
    public partial class MessagesTab
    {
        private static MessagesTab _objMessagesTab;

        public MessagesTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyBroadcastMessages").ToString(),
                    Content = new Lazy<UserControl>(BroadcastMessages.GetSingeltonBroadcastMessages)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyAutoReplyToNewMessage").ToString(),
                    Content = new Lazy<UserControl>(AutoReplyToNewMessage.GetSingeltonAutoReplyToNewMessage)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeySendMessageToFollowers").ToString(),
                    Content = new Lazy<UserControl>(SendMessageToFollower.GetSingeltonSendMessageToFollower)
                }
            };
            MessagesTabs.ItemsSource = tabItems;
        }

        public static MessagesTab GetSingeltonObjectMessagesTab()
        {
            return _objMessagesTab ?? (_objMessagesTab = new MessagesTab());
        }

        public void SetIndex(int index)
        {
            MessagesTabs.SelectedIndex = index;
        }
    }
}