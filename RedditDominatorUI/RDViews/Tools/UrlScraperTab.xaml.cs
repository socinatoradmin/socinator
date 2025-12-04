using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using RedditDominatorUI.CustomControl;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RedditDominatorUI.RDViews.Tools
{
    /// <summary>
    ///     Interaction logic for UrlScraperTab.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class UrlScraperTab : UserControl
    {
        public UrlScraperTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(UrlScraperConfig.GetSingeltonObjectUrlScraperConfig)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.UrlScraper))
                }
            };
            UrlScraperTabs.ItemsSource = tabItems;
        }
    }
}