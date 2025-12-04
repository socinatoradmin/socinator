using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using TwtDominatorUI.TDViews.TwtMessenger;

namespace TwtDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for TwtMessengerTab.xaml
    /// </summary>
    public partial class TwtMessengerTab : UserControl
    {
        private static TwtMessengerTab objTwtMessengerTab;

        private TwtMessengerTab()
        {
            InitializeComponent();

            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyBroadcastMessage").ToString(),
                    Content = new Lazy<UserControl>(BroadCastMessage.GetSingletonObjectBroadCastMessage)
                },

                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyMessageToNewFollowers").ToString(),
                    Content = new Lazy<UserControl>(ReplyToNewFollowers.GetSingletonObjectReplyToNewFollowers)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyAutoReply").ToString(),
                    Content = new Lazy<UserControl>(AutoReply.GetSingletonObjectAutoReply)
                }
            };

            TwtMessengerTabControl.ItemsSource = tabItems;
        }


        public static TwtMessengerTab GetSingeltonObjectTwtMessengerTab()
        {
            return objTwtMessengerTab ?? (objTwtMessengerTab = new TwtMessengerTab());
        }

        public void SetIndex(int index)
        {
            TwtMessengerTabControl.SelectedIndex = index;
        }
    }
}