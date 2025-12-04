using DominatorHouseCore.Models;
using FaceDominatorUI.FDViews.FbMessanger;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager
{
    /// <summary>
    ///     Interaction logic for FbMessangerTab.xaml
    /// </summary>
    public partial class FbMessangerTab
    {
        public FbMessangerTab()
        {
            InitializeComponent();

            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyBrodcastMessage").ToString(),
                    Content = new Lazy<UserControl>(BrodcastMessage.GetSingeltonObjectBrodcastMessage)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyAutoReplyToMessage").ToString(),
                    Content = new Lazy<UserControl>(AutoReplyMessage.GetSingeltonObjectAutoReplyMessage)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeySendMessageToNewFriends").ToString(),
                    Content = new Lazy<UserControl>(MessageRecentFriends.GetSingeltonObjectMessageRecentFriends)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeySendGreetingsToFriends").ToString(),
                    Content = new Lazy<UserControl>(SendGreetingsToFriends.GetSingeltonObjectSendGreetingsToFriends)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeySendMessageToFanpages").ToString(),
                    Content = new Lazy<UserControl>(MessageToFanpages.GetSingeltonObjectMessageToFanpages)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeySendMessageToPlaces").ToString(),
                    Content = new Lazy<UserControl>(MessageToPlaces.GetSingeltonObjectMessageToPlaces)
                }
            };

            FbMessangerTabs.ItemsSource = items;
        }


        private static FbMessangerTab CurrentFbMessangerTab { get; set; }

        public static FbMessangerTab GetSingeltonObjectFbMessangerTab()
        {
            return CurrentFbMessangerTab ?? (CurrentFbMessangerTab = new FbMessangerTab());
        }

        public void SetIndex(int tabIndex)
        {
            FbMessangerTabs.SelectedIndex = tabIndex;
        }
    }
}