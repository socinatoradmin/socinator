using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TumblrDominatorUI.CustomControl;
using TumblrDominatorUI.TumblrView.Activity.Follow;

namespace TumblrDominatorUI.TumblrView.Activity
{
    /// <summary>
    ///     Interaction logic for FollowTab.xaml
    /// </summary>
    public partial class FollowTab
    {
        public FollowTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") == null
                        ? "Configuration"
                        : Application.Current.FindResource("LangKeyConfiguration")?.ToString(),
                    Content = new Lazy<UserControl>(FollowConfiguration.GetSingeltonObjectFollowConfiguration)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") == null
                        ? "Reports"
                        : Application.Current.FindResource("LangKeyReports")?.ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.Follow))
                }
            };
            FollowTabs.ItemsSource = tabItems;
        }
    }
}