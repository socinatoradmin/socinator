using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using YoutubeDominatorUI.YDViews.WatchVideo;

namespace YoutubeDominatorUI.TabManager
{
    public partial class WatchVideoTab
    {
        private static WatchVideoTab _objWatchVideoTab;

        public WatchVideoTab()
        {
            InitializeComponent();
            _objWatchVideoTab = this;

            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyViewIncreaser").ToString(),
                    Content = new Lazy<UserControl>(ViewIncreaser.GetSingeltonObjectViewIncreaser)
                }
            };
            WatchVideoTabs.ItemsSource = tabItems;
        }

        public static WatchVideoTab GetSingeltonObject_WatchVideoTab()
        {
            return _objWatchVideoTab ?? (_objWatchVideoTab = new WatchVideoTab());
        }

        public void SetIndex(int index)
        {
            WatchVideoTabs.SelectedIndex = index;
        }
    }
}