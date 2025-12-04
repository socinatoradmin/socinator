using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using LinkedDominatorUI.LDViews.Messenger;

namespace LinkedDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for MessengerTab.xaml
    /// </summary>
    public partial class MessengerTab : UserControl
    {
        private static MessengerTab objMessengerTab;

        public MessengerTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyBroadcastMessages").ToString(),
                        Content = new Lazy<UserControl>(() => BroadcastMessages.GetSingletonBroadcastMessages())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyAutoReplyToNewMessage").ToString(),
                        Content = new Lazy<UserControl>(() => AutoReplyToNewMessage.GetSingeltonAutoReplyToNewMessage())
                    },

                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeySendMessageToNewConnection").ToString(),
                        Content = new Lazy<UserControl>(() =>
                            SendMessageToNewConnection.GetSingeltonSendMessageToNewConnection())
                    },

                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeySendGreetingsToConnections").ToString(),
                        Content = new Lazy<UserControl>(() =>
                            SendGreetingsToConnections.GetSingeltonSendGreetingsToConnections())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyDeleteConversations").ToString(),
                        Content = new Lazy<UserControl>(() => DeleteConversations.GetSingletonDeleteConversations())
                    }

                    //new TabItemTemplates
                    //{
                    //    Title=FindResource("LangKeySendMessageToGroupMembers").ToString(),
                    //    Content=new Lazy<UserControl>(()=>new MessageGroupMembers())
                    //},
                };
                MessengerTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static MessengerTab GetSingeltonObjectMessengerTab()
        {
            return objMessengerTab ?? (objMessengerTab = new MessengerTab());
        }

        public void SetIndex(int index)
        {
            //MessengerTabs is the name of this Tab
            MessengerTabs.SelectedIndex = index;
        }
    }
}