using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TumblrDominatorUI.TumblrView.GrowFollowers;

namespace TumblrDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for GrowFollowersTab.xaml
    /// </summary>
    public partial class GrowFollowersTab
    {
        private static GrowFollowersTab _objGrowFollowersTab;

        public GrowFollowersTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyFollower") == null
                        ? "Follower"
                        : Application.Current.FindResource("LangKeyFollower")?.ToString(),
                    Content = new Lazy<UserControl>(Follower.GetSingeltonObjectFollower)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyUnfollower") == null
                        ? "Unfollower"
                        : Application.Current.FindResource("LangKeyUnfollower")?.ToString(),

                    Content = new Lazy<UserControl>(UnFollower.GetSingeltonObjectUnfollower)
                }
            };

            GrowFollowerTabControl.ItemsSource = tabItems;
        }

        public static GrowFollowersTab GetSingletonObjectGrowFollowersTab()
        {
            return _objGrowFollowersTab ?? (_objGrowFollowersTab = new GrowFollowersTab());
        }

        public void SetIndex(int index)
        {
            GrowFollowerTabControl.SelectedIndex = index;
        }
    }
}