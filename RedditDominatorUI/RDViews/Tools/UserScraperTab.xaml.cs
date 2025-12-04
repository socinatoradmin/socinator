using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using RedditDominatorUI.CustomControl;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RedditDominatorUI.RDViews.Tools
{
    /// <summary>
    ///     Interaction logic for UserScraperTab.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class UserScraperTab
    {
        public UserScraperTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(UserScraperConfig.GetSingeltonObjectUserScraperConfig)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.UserScraper))
                }
            };
            UserScraperTabs.ItemsSource = tabItems;
        }
    }
}