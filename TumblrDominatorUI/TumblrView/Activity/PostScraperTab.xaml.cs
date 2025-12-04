using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TumblrDominatorUI.CustomControl;
using TumblrDominatorUI.TumblrView.Activity.PostScraper;

namespace TumblrDominatorUI.TumblrView.Activity
{
    /// <summary>
    ///     Interaction logic for ScraperTab.xaml
    /// </summary>
    public partial class PostScraperTab
    {
        public PostScraperTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") == null
                        ? "Configuration"
                        : Application.Current.FindResource("LangKeyConfiguration")?.ToString(),
                    Content = new Lazy<UserControl>(PostScraperConfig.GetSingletonObjectPostScraperConfig)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") == null
                        ? "Reports"
                        : Application.Current.FindResource("LangKeyReports")?.ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.PostScraper))
                }
            };
            PostScraperTabs.ItemsSource = tabItems;
        }

        private static PostScraperTab PostScraperTabItem { get; set; }

        public static object Locker { get; set; } = new object();
    }
}