using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Models;

namespace GramDominatorUI.GDViews.Tools
{
    /// <summary>
    ///     Interaction logic for MessagesTab.xaml
    /// </summary>
    public partial class MessagesTab : UserControl
    {
        public MessagesTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyBroadcastMessages").ToString(),
                    Content = new Lazy<UserControl>(BroadcastMessagesTab.GetSingeltonBroadcastMessagesTab)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyAutoReplyToNewMessage").ToString(),
                    Content = new Lazy<UserControl>(AutoReplyToNewMessageTab.GetSingeltonAutoReplyToNewMessageTab)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeySendMessageToNewFollowers").ToString(),
                    Content = new Lazy<UserControl>(SendMessageToFollowerTab.GetSingeltonSendMessageToFollowerTab)
                }
            };
            Messages.ItemsSource = TabItems;
        }

        private static MessagesTab CurrentMessagesTab { get; set; }

        public static MessagesTab GetSingeltonSendMessageToFollowerTab()
        {
            return CurrentMessagesTab ?? (CurrentMessagesTab = new MessagesTab());
        }
    }
}