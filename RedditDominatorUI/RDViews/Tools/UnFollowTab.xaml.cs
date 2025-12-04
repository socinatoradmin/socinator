using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using RedditDominatorUI.CustomControl;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RedditDominatorUI.RDViews.Tools
{
    /// <summary>
    ///     Interaction logic for UnFollowTab.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class UnFollowTab
    {
        public UnFollowTab()
        {
            InitializeComponent();

            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(UnFollowConfiguration.GetSingeltonObjectUnFollowConfiguration)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.Unfollow))
                }
            };
            UnFollowTabs.ItemsSource = tabItems;
        }
    }
}