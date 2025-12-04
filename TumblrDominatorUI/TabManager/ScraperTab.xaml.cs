using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using TumblrDominatorUI.TumblrView.Scraper;

namespace TumblrDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for MessagesTab.xaml
    /// </summary>
    public partial class ScraperTab
    {
        public ScraperTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyPostScraper").ToString(),
                    Content = new Lazy<UserControl>(PostScraper.GetSingletonObjectPostScraperConfig)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyUserScraper").ToString(),
                    Content = new Lazy<UserControl>(UserScraper.GetSingletonObjectUserScraperConfig)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyCommentScraper").ToString(),
                    Content = new Lazy<UserControl>(CommentScraper.GetSingletonObjectCommentScraperConfig)
                }
            };

            PostScraperTabs.ItemsSource = tabItems;
        }

        private static ScraperTab CurrentScraperTab { get; set; }

        /// <summary>
        ///     GetSingeltonObjectInstaLikerInstaCommenterTab is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static ScraperTab GetSingeltonObjectScraperTab()
        {
            return CurrentScraperTab ?? (CurrentScraperTab = new ScraperTab());
        }

        public void SetIndex(int index)
        {
            PostScraperTabs.SelectedIndex = index;
        }
    }
}