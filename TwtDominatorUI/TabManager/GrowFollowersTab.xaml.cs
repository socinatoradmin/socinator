using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using TwtDominatorUI.TDViews.GrowFollowers;

namespace TwtDominatorUI.TabManager
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
                    Title = Application.Current.FindResource("LangKeyFollower").ToString(),
                    Content = new Lazy<UserControl>(Follower.GetSingletonObjectFollower)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyFollowBack").ToString(),
                    Content = new Lazy<UserControl>(() => FollowBack.GetSingletonObjectFollowBack())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyMute").ToString(),
                    Content = new Lazy<UserControl>(MuteUsers.GetSingletonObjectMuteUsers)
                }
            };

            try
            {
                var title = Application.Current.FindResource("LangKeyUnfollower").ToString();

                tabItems.Add(new TabItemTemplates
                {
                    Title = title == null ? "Unfollow" : title,

                    Content = new Lazy<UserControl>(Unfollower.GetSingletonObjectUnfollower)
                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }

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