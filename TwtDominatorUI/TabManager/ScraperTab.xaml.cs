using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using TwtDominatorUI.TDViews.Scraper;

namespace TwtDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for ScraperTab.xaml
    /// </summary>
    public partial class ScraperTab : UserControl
    {
        private static ScraperTab objScraperTab;

        private ScraperTab()
        {
            InitializeComponent();

            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyScrapeUsers").ToString(),
                    Content = new Lazy<UserControl>(ScrapeUser.GetSingletonObjectScrapeUser)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyScrapeTweet").ToString(),
                    Content = new Lazy<UserControl>(ScrapeTweet.GetSingletonObjectScrapeTweet)
                }
            };

            ScraperTabControl.ItemsSource = tabItems;
        }


        public static ScraperTab GetSingeltonObjectScraperTab()
        {
            return objScraperTab ?? (objScraperTab = new ScraperTab());
        }

        public void SetIndex(int index)
        {
            ScraperTabControl.SelectedIndex = index;
        }
    }
}