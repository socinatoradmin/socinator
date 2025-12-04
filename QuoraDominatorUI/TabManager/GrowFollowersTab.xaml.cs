using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using QuoraDominatorUI.QDViews.GrowFollowers;

namespace QuoraDominatorUI.TabManager
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
                    Title = Application.Current.FindResource("LangKeyFollow") == null
                        ? "Follow"
                        : Application.Current.FindResource("LangKeyFollow")?.ToString(),
                    Content = new Lazy<UserControl>(Follower.GetSingeltonObjectFollower)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyUnfollow") == null
                        ? "Unfollow"
                        : Application.Current.FindResource("LangKeyUnfollow")?.ToString(),
                    Content = new Lazy<UserControl>(Unfollow.GetSingeltonObjectUnfollower)
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