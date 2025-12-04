using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using YoutubeDominatorUI.YDViews.GrowSubscribers;

namespace YoutubeDominatorUI.TabManager
{
    public partial class GrowSubscribersTab
    {
        private static GrowSubscribersTab _objGrowSubscribersTab;

        public GrowSubscribersTab()
        {
            InitializeComponent();
            _objGrowSubscribersTab = this;

            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeySubscribe").ToString(),
                    Content = new Lazy<UserControl>(Subscribe.GetSingeltonObjectSubscribe)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyUnsubscribe").ToString(),
                    Content = new Lazy<UserControl>(Unsubscribe.GetSingeltonObjectUnsubscribe)
                }
            };
            GrowSubscribersTabs.ItemsSource = tabItems;
        }

        public static GrowSubscribersTab GetSingeltonObject_GrowSubscribersTab()
        {
            return _objGrowSubscribersTab ?? (_objGrowSubscribersTab = new GrowSubscribersTab());
        }

        public void SetIndex(int index)
        {
            GrowSubscribersTabs.SelectedIndex = index;
        }
    }
}