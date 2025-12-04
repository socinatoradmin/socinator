using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using LinkedDominatorUI.LDViews.Scraper;

namespace LinkedDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for ScraperTab.xaml
    /// </summary>
    public partial class ScraperTab : UserControl
    {
        private static ScraperTab objScraperTab;

        public ScraperTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyUserScraper").ToString(),
                        Content = new Lazy<UserControl>(() => UserScraper.GetSingeltonObjectUserScraper())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyJobScraper").ToString(),
                        Content = new Lazy<UserControl>(() => JobScraper.GetSingeltonObjectJobScraper())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyCompanyScraper").ToString(),
                        Content = new Lazy<UserControl>(() => CompanyScraper.GetSingeltonObjectCompanyScraper())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyMessageConversationsScraper").ToString(),
                        Content = new Lazy<UserControl>(() =>
                            AttachmentsMessageScraper.GetSingletonMessageConversationScraper())
                    }
                    //   new TabItemTemplates
                    //{
                    //    Title=FindResource("LangKeySalesNavigtor").ToString(),
                    //    Content=new Lazy<UserControl>(()=> new SalesNavigator_UserScraper())
                    //}
                };
                ScraperTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static ScraperTab GetSingeltonObjectScraperTab()
        {
            return objScraperTab ?? (objScraperTab = new ScraperTab());
        }

        public void SetIndex(int index)
        {
            //GrowConnectionsTab is the name of this Tab
            ScraperTabs.SelectedIndex = index;
        }
    }
}