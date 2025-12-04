using DominatorHouseCore.Models;
using RedditDominatorUI.RDViews.GrowFollowers;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RedditDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for GrowFollowers.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class GrowFollowerTab : UserControl
    {
        private static GrowFollowerTab _objGrowFollowerTab;

        public GrowFollowerTab()
        {
            InitializeComponent();
            _objGrowFollowerTab = this;
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyFollow").ToString(),
                    Content = new Lazy<UserControl>(Follow.GetSingletonObjectFollow)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyUnfollow").ToString(),
                    Content = new Lazy<UserControl>(UnFollower.GetSingeltonObjectUnfollower)
                }
            };
            GrowFollowerTabs.ItemsSource = tabItems;
        }

        public static int SelectedIndex => 0;

        public static GrowFollowerTab GetSingeltonObject_GrowFollowerTab()
        {
            return _objGrowFollowerTab ?? (_objGrowFollowerTab = new GrowFollowerTab());
        }

        public void SetIndex(int index)
        {
            GrowFollowerTabs.SelectedIndex = index;
        }
    }
}