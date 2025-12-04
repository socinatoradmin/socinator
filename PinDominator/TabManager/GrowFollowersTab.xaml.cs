using DominatorHouseCore.Models;
using PinDominator.PDViews.GrowFollowers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PinDominator.TabManager
{
    /// <summary>
    ///     Interaction logic for GrowFollowersTab.xaml
    /// </summary>
    public partial class GrowFollowersTab
    {
        private static GrowFollowersTab _objGrowFollowersTab;

        private GrowFollowersTab()
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
                    Content = new Lazy<UserControl>(UnFollower.GetSingeltonObjectUnFollower)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyFollowBacks") == null
                        ? "Follow Backs"
                        : Application.Current.FindResource("LangKeyFollowBacks")?.ToString(),
                    Content = new Lazy<UserControl>(FollowBack.GetSingeltonObjectFollowBack)
                }
            };

            GrowFollowerTab.ItemsSource = tabItems;
        }


        public static GrowFollowersTab GetSingeltonObjectGrowFollowersTab()
        {
            return _objGrowFollowersTab ?? (_objGrowFollowersTab = new GrowFollowersTab());
        }

        public void SetIndex(int index)
        {
            GrowFollowerTab.SelectedIndex = index;
        }
    }
}