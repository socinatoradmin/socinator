using DominatorHouseCore.Models;
using FaceDominatorUI.FDViews.FbFriends;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager
{
    /// <summary>
    ///     Interaction logic for FbFriendsTab.xaml
    /// </summary>
    public partial class FbFriendsTab
    {
        public FbFriendsTab()
        {
            InitializeComponent();
            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeySendFriendRequest").ToString(),
                    Content = new Lazy<UserControl>(SendFriendRequest.GetSingeltonObjectSendFriendRequest)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyIncomingFriendRequest").ToString(),
                    Content = new Lazy<UserControl>(IncommingFriendRequest.GetSingeltonObjectIncommingFriendRequest)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyUnfriend").ToString(),
                    Content = new Lazy<UserControl>(Unfriend.GetSingeltonObjectUnfriend)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyWithdrawSentRequest").ToString(),
                    Content = new Lazy<UserControl>(CancelSentRequest.GetSingeltonObjectCancelSentRequest)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyUnfollowFriend").ToString(),
                    Content = new Lazy<UserControl>(UnfollowFriend.GetSingeltonObjectUnfollow)
                }
            };

            FbFriendsTabs.ItemsSource = items;
        }


        private static FbFriendsTab CurrentFbFriendsTab { get; set; }

        public static FbFriendsTab GetSingeltonObjectFbFriendsTab()
        {
            return CurrentFbFriendsTab ?? (CurrentFbFriendsTab = new FbFriendsTab());
        }

        public void SetIndex(int tabIndex)
        {
            FbFriendsTabs.SelectedIndex = tabIndex;
        }
    }
}