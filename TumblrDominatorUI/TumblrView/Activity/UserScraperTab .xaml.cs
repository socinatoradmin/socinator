using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TumblrDominatorUI.CustomControl;
using TumblrDominatorUI.TumblrView.Activity.UserScraper;

namespace TumblrDominatorUI.TumblrView.Activity
{
    /// <summary>
    ///     Interaction logic for ScraperTab.xaml
    /// </summary>
    public partial class UserScraperTab
    {
        private UserScraperTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") == null
                        ? "Configuration"
                        : Application.Current.FindResource("LangKeyConfiguration")?.ToString(),
                    Content = new Lazy<UserControl>(UserScraperConfig.GetSingletonObjectUserScraperConfig)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") == null
                        ? "Reports"
                        : Application.Current.FindResource("LangKeyReports")?.ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.UserScraper))
                }
            };
            UserScraperTabs.ItemsSource = tabItems;
        }

        private static UserScraperTab UserScraperTabItem { get; set; }

        public static object Locker { get; set; } = new object();

        public static UserScraperTab GetSingletonObjectUserScraperTab()
        {
            lock (Locker)
            {
                if (UserScraperTabItem == null)
                    UserScraperTabItem = new UserScraperTab();
            }

            return UserScraperTabItem;
        }
    }
}