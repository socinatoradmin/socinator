using DominatorHouseCore.Models;
using RedditDominatorUI.RDViews.AutoFeedActivity;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RedditDominatorUI.TabManager
{
    /// <summary>
    /// Interaction logic for RedditFeed.xaml
    /// </summary>
    public partial class RedditFeed : UserControl
    {
        private static RedditFeed _Instance;
        public static int SelectedIndex => 0;
        public static RedditFeed GetSingletonInstance() => _Instance ?? (_Instance = new RedditFeed());
        public RedditFeed()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyRedditAutoActivity").ToString(),
                    Content = new Lazy<UserControl>(RedditPostAutoActivity.GetSingletonInstance)
                }
            };
            RedditFeedTabs.ItemsSource = tabItems;
        }
        public void SetIndex(int index)
        {
            RedditFeedTabs.SelectedIndex = index;
        }
    }
}
