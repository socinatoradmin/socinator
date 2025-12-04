using DominatorHouseCore.Models;
using PinDominator.PDViews.PinScrape;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PinDominator.TabManager
{
    /// <summary>
    ///     Interaction logic for ScraperTab.xaml
    /// </summary>
    public partial class ScraperTab
    {
        private static ScraperTab _objScraperTab;

        public ScraperTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyUserScraper") == null
                        ? "User Scraper"
                        : FindResource("LangKeyUserScraper")?.ToString(),
                    Content = new Lazy<UserControl>(UserScraper.GetSingeltonObjectUserScraper)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyBoardScraper") == null
                        ? "Board Scraper"
                        : FindResource("LangKeyBoardScraper")?.ToString(),
                    Content = new Lazy<UserControl>(BoardScraper.GetSingeltonObjectBoardScraper)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyPinScraper") == null
                        ? "Pin Scraper"
                        : FindResource("LangKeyPinScraper")?.ToString(),
                    Content = new Lazy<UserControl>(PinScraper.GetSingletonObjectPinScraper)
                }
            };
            ScraperTabs.ItemsSource = tabItems;
        }

        public static ScraperTab GetSingeltonObjectPinScrapeTab()
        {
            return _objScraperTab ?? (_objScraperTab = new ScraperTab());
        }

        public void SetIndex(int index)
        {
            ScraperTabs.SelectedIndex = index;
        }
    }
}