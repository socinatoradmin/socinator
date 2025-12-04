using DominatorHouseCore.Models;
using RedditDominatorUI.RDViews.UrlScraper;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
//using RedditDominatorUI.RDViews.PostScraper;

namespace RedditDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for UrlScraperTab.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class ScraperTab : UserControl
    {
        public ScraperTab()
        {
            InitializeComponent();
            //currUrlScraperTab = this;
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyUrlscraper").ToString(),
                    Content = new Lazy<UserControl>(UrlScraper.GetSingletonObjectUrlScraper)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyUserScraper").ToString(),
                    Content = new Lazy<UserControl>(UserScraper.GetSingletonObjectUserScraper)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyChannelScraper").ToString(),
                    Content = new Lazy<UserControl>(ChannelScraper.GetSingletonObjectChannelScraper)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyCommentScraper").ToString(),
                    Content = new Lazy<UserControl>(CommentScraper.GetSingletonObjectCommentScraper)
                }
            };
            UrlScraperTabs.ItemsSource = tabItems;
        }

        public static int SelectedIndex => 0;

        public void SetIndex(int index)
        {
            UrlScraperTabs.SelectedIndex = index;
        }
    }
}