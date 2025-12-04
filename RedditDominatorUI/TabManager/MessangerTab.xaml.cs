using DominatorHouseCore.Models;
using RedditDominatorUI.RDViews.Messanger;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RedditDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for MessangerTab.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class MessangerTab : UserControl
    {
        private static MessangerTab _objMessangerTab;

        public MessangerTab()
        {
            InitializeComponent();
            _objMessangerTab = this;
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyBroadcastMessages").ToString(),
                    Content = new Lazy<UserControl>(BroadcastMessage.GetSingeltonBroadcastMessages)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyAutoReply").ToString(),
                    Content = new Lazy<UserControl>(AutoReply.GetSingletonObjectAutoReply)
                }
            };
            MessangerTabs.ItemsSource = tabItems;
        }

        public static int SelectedIndex => 0;

        public static MessangerTab GetSingeltonObject_MessangerTab()
        {
            return _objMessangerTab ?? (_objMessangerTab = new MessangerTab());
        }
        public void SetIndex(int index)
        {
            MessangerTabs.SelectedIndex = index;
        }
    }
}