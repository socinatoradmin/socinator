using DominatorHouseCore.Models;
using RedditDominatorUI.RDViews.Subscribe;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RedditDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for SubscribeChannelsTab.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class SubscribeChannelsTab : UserControl
    {
        private static SubscribeChannelsTab _objSubscribeChannelsTab;

        public SubscribeChannelsTab()
        {
            InitializeComponent();
            _objSubscribeChannelsTab = this;
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeySubscribe").ToString(),
                    Content = new Lazy<UserControl>(Subscribe.GetSingletonObjectSubscribe)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyUnsubscribe").ToString(),
                    Content = new Lazy<UserControl>(UnSubscribe.GetSingletonObjectUnSubscribe)
                }
            };
            SubscribeChannelsTabs.ItemsSource = tabItems;
        }

        public static int SelectedIndex => 0;

        public static SubscribeChannelsTab GetSingeltonObject_SubscribeChannelsTab()
        {
            return _objSubscribeChannelsTab ?? (_objSubscribeChannelsTab = new SubscribeChannelsTab());
        }

        public void SetIndex(int index)
        {
            SubscribeChannelsTabs.SelectedIndex = index;
        }
    }
}