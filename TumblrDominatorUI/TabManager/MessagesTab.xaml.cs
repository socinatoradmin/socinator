using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using TumblrDominatorUI.TumblrView.Message;

namespace TumblrDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for MessagesTab.xaml
    /// </summary>
    public partial class MessagesTab
    {
        public MessagesTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyBroadcastMessages").ToString(),
                    Content = new Lazy<UserControl>(BroadcastMessages.GetSingeltonBroadcastMessages)
                }
            };

            BroadcastMsgTabs.ItemsSource = tabItems;
        }

        private static MessagesTab CurrentEngageTab { get; set; }

        /// <summary>
        ///     GetSingeltonObjectInstaLikerInstaCommenterTab is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static MessagesTab GetSingeltonObjectMsgTab()
        {
            return CurrentEngageTab ?? (CurrentEngageTab = new MessagesTab());
        }

        public void SetIndex(int index)
        {
            BroadcastMsgTabs.SelectedIndex = index;
        }
    }
}