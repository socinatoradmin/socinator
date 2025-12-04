using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TumblrDominatorUI.CustomControl;
using TumblrDominatorUI.TumblrView.Activity.CommentScraper;

namespace TumblrDominatorUI.TumblrView.Activity
{
    /// <summary>
    ///     Interaction logic for ScraperTab.xaml
    /// </summary>
    public partial class CommentScraperTab
    {
        private CommentScraperTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") == null
                        ? "Configuration"
                        : Application.Current.FindResource("LangKeyConfiguration")?.ToString(),
                    Content = new Lazy<UserControl>(CommentScraperConfig.GetSingletonObjectCommentScraperConfig)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") == null
                        ? "Reports"
                        : Application.Current.FindResource("LangKeyReports")?.ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.CommentScraper))
                }
            };
            CommentScraperTabs.ItemsSource = tabItems;
        }

        private static CommentScraperTab CommentScraperTabItem { get; set; }

        public static object Locker { get; set; } = new object();

        public static CommentScraperTab GetSingletonObjectCommentScraperTab()
        {
            lock (Locker)
            {
                if (CommentScraperTabItem == null)
                    CommentScraperTabItem = new CommentScraperTab();
            }

            return CommentScraperTabItem;
        }
    }
}