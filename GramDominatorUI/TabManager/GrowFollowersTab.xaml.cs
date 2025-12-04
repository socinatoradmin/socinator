using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using GramDominatorUI.GDViews.GrowFollowers;

namespace GramDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for GrowFollowersTab.xaml
    /// </summary>
    public partial class GrowFollowersTab : UserControl
    {
        private static GrowFollowersTab objGrowFollowersTab;

        private GrowFollowersTab()
        {
            InitializeComponent();

            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyFollower") != null
                        ? Application.Current.FindResource("LangKeyFollower").ToString()
                        : "Grow Followers",
                    Content = new Lazy<UserControl>(Follower.GetSingeltonObjectFollower)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyUnfollower") != null
                        ? Application.Current.FindResource("LangKeyUnfollower").ToString()
                        : "Unfollower",
                    Content = new Lazy<UserControl>(UnFollower.GetSingeltonObjectUnfollower)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyFollowBack") != null
                        ? Application.Current.FindResource("LangKeyFollowBack").ToString()
                        : "Follow Back﻿",
                    Content = new Lazy<UserControl>(FollowBack.GetSingeltonObjectFollowBack)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyBlockFollower") != null
                        ? Application.Current.FindResource("LangKeyBlockFollower").ToString()
                        : "Block Follower﻿",
                    Content = new Lazy<UserControl>(BlockFollower.GetSingeltonObjectBlockFollower)
                },
                //new TabItemTemplates
                //{
                //    Title = Application.Current.FindResource("LangkeyAddToCloseFriend") != null
                //        ? Application.Current.FindResource("LangkeyAddToCloseFriend").ToString()
                //        : "Close Friends",
                //    Content = new Lazy<UserControl>(CloseFriendTab.GetSingletonInstance)
                //}
            };

            GrowFollowerTab.ItemsSource = tabItems;
        }


        public static GrowFollowersTab GetSingeltonObjectGrowFollowersTab()
        {
            return objGrowFollowersTab ?? (objGrowFollowersTab = new GrowFollowersTab());
        }

        public void SetIndex(int index)
        {
            GrowFollowerTab.SelectedIndex = index;
        }
    }
}