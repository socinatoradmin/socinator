using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TumblrDominatorUI.CustomControl;
using TumblrDominatorUI.TumblrView.Activity.Unfollow;

namespace TumblrDominatorUI.TumblrView.Activity
{
    /// <summary>
    ///     Interaction logic for UnfollowTab.xaml
    /// </summary>
    public partial class UnfollowTab
    {
        public UnfollowTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") == null
                        ? "Configuration"
                        : Application.Current.FindResource("LangKeyConfiguration")?.ToString(),
                    Content = new Lazy<UserControl>(() => new UnfollowConfiguration())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") == null
                        ? "Reports"
                        : Application.Current.FindResource("LangKeyReports")?.ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.Unfollow))
                }
            };
            UnFollowTabs.ItemsSource = tabItems;
        }
    }
}