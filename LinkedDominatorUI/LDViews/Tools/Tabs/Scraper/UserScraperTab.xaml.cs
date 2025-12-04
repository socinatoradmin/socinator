using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using LinkedDominatorUI.CustomControl;
using LinkedDominatorUI.LDViews.Tools.Scraper;

namespace LinkedDominatorUI.LDViews.Tools.Tabs
{
    /// <summary>
    ///     Interaction logic for UserScraperTab.xaml
    /// </summary>
    public partial class UserScraperTab : UserControl
    {
        private static UserScraperTab objUserScraperTab;

        public UserScraperTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConfiguration").ToString(),
                        Content = new Lazy<UserControl>(() =>
                            UserScraperConfiguration.GetSingeltonObjectUserScraperConfiguration())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.UserScraper))
                    }
                };
                UserScraperTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static UserScraperTab GetSingeltonObjectUserScraperTab()
        {
            return objUserScraperTab ?? (objUserScraperTab = new UserScraperTab());
        }
    }
}