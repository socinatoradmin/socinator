using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using YoutubeDominatorUI.YDViews.Scraper;

namespace YoutubeDominatorUI.TabManager
{
    public partial class ScraperTab
    {
        private static ScraperTab _objScraperTab;

        public ScraperTab()
        {
            InitializeComponent();
            _objScraperTab = this;

            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyPostScraper").ToString(),
                    Content = new Lazy<UserControl>(PostScraper.GetSingeltonObjectPostScraper)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyChannelScraper").ToString(),
                    Content = new Lazy<UserControl>(ChannelScraper.GetSingeltonObjectChannelScraper)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyCommentScraper").ToString(),
                    Content = new Lazy<UserControl>(CommentScraper.GetSingeltonObjectCommentScraper)
                }
            };
            ScraperTabs.ItemsSource = tabItems;
        }

        public static ScraperTab GetSingeltonObject_ScraperTab()
        {
            return _objScraperTab ?? (_objScraperTab = new ScraperTab());
        }

        public void SetIndex(int index)
        {
            ScraperTabs.SelectedIndex = index;
        }
    }
}