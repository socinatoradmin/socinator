using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using PinDominator.PDViews.PinMessenger;

namespace PinDominator.TabManager
{
    /// <summary>
    ///     Interaction logic for PinMessengerTab.xaml
    /// </summary>
    public partial class PinMessengerTab
    {
        private static PinMessengerTab _objPinMessengerTab;

        public PinMessengerTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyBroadcastMesseges") == null
                        ? "Broadcast Messeges"
                        : Application.Current.FindResource("LangKeyBroadcastMesseges")?.ToString(),
                    Content = new Lazy<UserControl>(BroadcastMessages.GetSingletonObjectBroadcastMessages)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyAutoReplyToNewMessage") == null
                        ? "Auto Reply To New Message"
                        : Application.Current.FindResource("LangKeyAutoReplyToNewMessage")?.ToString(),
                    Content = new Lazy<UserControl>(AutoReplyToNewMessage.GetSingletonObjectAutoReplyToNewMessage)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyMessageToNewFollowers") == null
                        ? "Message To New Followers"
                        : FindResource("LangKeyMessageToNewFollowers")?.ToString(),
                    Content = new Lazy<UserControl>(SendMessageToNewFollowers
                        .GetSingletonObjectSendMessageToNewFollowers)
                }
            };
            PinMessengerTabs.ItemsSource = tabItems;
        }

        public static PinMessengerTab GetSingeltonObjectPinMessengerTab()
        {
            return _objPinMessengerTab ?? (_objPinMessengerTab = new PinMessengerTab());
        }

        public void SetIndex(int index)
        {
            PinMessengerTabs.SelectedIndex = index;
        }
    }
}