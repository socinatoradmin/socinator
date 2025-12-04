using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using GramDominatorUI.GDViews.InstaScrape;

namespace GramDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for InstaScrapeTab.xaml
    /// </summary>
    public partial class InstaScrapeTab : UserControl
    {
        private static InstaScrapeTab objInstaScrapeTab;

        private InstaScrapeTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyUserScraper") != null
                        ? Application.Current.FindResource("LangKeyUserScraper").ToString()
                        : "User Scraper",
                    Content = new Lazy<UserControl>(UserScraper.GetSingeltonObjectUserScraper)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyHashtagsScraper") != null
                        ? Application.Current.FindResource("LangKeyHashtagsScraper").ToString()
                        : "Hashtags Scraper",
                    Content = new Lazy<UserControl>(HashtagsScraper.GetSingeltonObjectHashtagsScraper)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyPostScraper") != null
                        ? Application.Current.FindResource("LangKeyPostScraper").ToString()
                        : "Post Scraper",
                    Content = new Lazy<UserControl>(DownloadPhotos.GetSingeltonObjectDownloadPhotos)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyCommentScraper") != null
                        ? Application.Current.FindResource("LangKeyCommentScraper").ToString()
                        : "Comment Scraper",
                    Content = new Lazy<UserControl>(CommentScraper.GetSingeltonObjectCommentScraper)
                }
            };


            InstaScrapeTabs.ItemsSource = tabItems;
        }

        public static InstaScrapeTab GetSingeltonObjectInstaScrapeTab()
        {
            return objInstaScrapeTab ?? (objInstaScrapeTab = new InstaScrapeTab());
        }

        public void SetIndex(int index)
        {
            InstaScrapeTabs.SelectedIndex = index;
        }
    }
}