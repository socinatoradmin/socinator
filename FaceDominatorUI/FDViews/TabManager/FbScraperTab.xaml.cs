using DominatorHouseCore.Models;
using FaceDominatorUI.FDViews.FbScraper;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager
{
    /// <summary>
    ///     Interaction logic for FbScraperTab.xaml
    /// </summary>
    public partial class FbScraperTab
    {
        public FbScraperTab()
        {
            InitializeComponent();
            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyProfileScraper").ToString(),
                    Content = new Lazy<UserControl>(ProfileScraper.GetSingeltonObjectProfileScraper)
                },

                new TabItemTemplates
                {
                    Title = FindResource("LangKeyFanpageScraper").ToString(),
                    Content = new Lazy<UserControl>(FanpageScraper.GetSingeltonObjectFanpageScraper)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyGroupScraper").ToString(),
                    Content = new Lazy<UserControl>(GroupScraper.GetSingeltonObjectGroupScraper)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyCommentScraper").ToString(),
                    Content = new Lazy<UserControl>(CommentScraper.GetSingeltonObjectCommentScraper)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyCommentRepliesScraper").ToString(),
                    Content = new Lazy<UserControl>(CommnetrRepliesScraper.GetSingeltonObjectCommnetrRepliesScraper)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyPostScraper").ToString(),
                    Content = new Lazy<UserControl>(PostScraper.GetSingeltonObjectPostScraper)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyDownloadMedia").ToString(),
                    Content = new Lazy<UserControl>(DownloadPhotos.GetSingeltonObjectCurrentDownloadMedia)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyPlaceScraper").ToString(),
                    Content = new Lazy<UserControl>(PlaceScraper.GetSingeltonObjectMessageToPlaces)
                }
                //new TabItemTemplates
                //{
                //    Title=FindResource("LangKeyMarketplaceScraper").ToString(),
                //    Content = new Lazy<UserControl>(MarketPlaceScraper.GetSingeltonObjectMarketplaceScraper)
                //}
            };

            FbScraperTabs.ItemsSource = items;
        }

        private static FbScraperTab CurrentFbScraperTab { get; set; }

        public static FbScraperTab GetSingeltonObjectFbScraperTab()
        {
            return CurrentFbScraperTab ?? (CurrentFbScraperTab = new FbScraperTab());
        }

        public void SetIndex(int tabIndex)
        {
            FbScraperTabs.SelectedIndex = tabIndex;
        }
    }
}