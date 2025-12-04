using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using GramDominatorUI.GDViews.Instachats;

namespace GramDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for InstachatTab.xaml
    /// </summary>
    public partial class InstachatTab : UserControl
    {
        private static InstachatTab objInstachatTab;

        public InstachatTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyBroadcastMessages") != null
                        ? Application.Current.FindResource("LangKeyBroadcastMessages").ToString()
                        : "Broadcast Messages",
                    Content = new Lazy<UserControl>(BroadcastMessages.GetSingeltonBroadcastMessages)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyAutoReplyToNewMessage") != null
                        ? Application.Current.FindResource("LangKeyAutoReplyToNewMessage").ToString()
                        : "Auto Reply To New Message",
                    Content = new Lazy<UserControl>(AutoReplyToNewMessage.GetSingeltonAutoReplyToNewMessage)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeySendMessageToNewFollowers") != null
                        ? Application.Current.FindResource("LangKeySendMessageToNewFollowers").ToString()
                        : "Send Message To New Follower",
                    Content = new Lazy<UserControl>(SendMessageToFollower.GetSingeltonSendMessageToFollower)
                }
            };

            InstachatTabs.ItemsSource = tabItems;
        }

        public static InstachatTab GetSingeltonObjectInstachatTab()
        {
            return objInstachatTab ?? (objInstachatTab = new InstachatTab());
        }

        public void SetIndex(int index)
        {
            InstachatTabs.SelectedIndex = index;
        }
    }
}