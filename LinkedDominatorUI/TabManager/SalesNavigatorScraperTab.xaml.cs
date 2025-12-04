using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using LinkedDominatorUI.LDViews.SalesNavigatorScraper;
using LinkedDominatorUI.LDViews.SalesNavigatorUserScraper;

namespace LinkedDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for SalesNavigatorScraper.xaml
    /// </summary>
    public partial class SalesNavigatorScraperTab : UserControl
    {
        private static SalesNavigatorScraperTab objSalesNavigatorScraperTab;

        public SalesNavigatorScraperTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyUserScraper").ToString(),
                        Content = new Lazy<UserControl>(() => UserScraper.GetSingeltonObjectSalesUserScraper())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyCompanyScraper").ToString(),
                        Content = new Lazy<UserControl>(() => CompanyScraper.GetSingletonObjectCompanyScraper())
                    }
                };
                SalesNavigatorScraperTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static SalesNavigatorScraperTab GetSingeltonObjectScraperTab()
        {
            return objSalesNavigatorScraperTab ?? (objSalesNavigatorScraperTab = new SalesNavigatorScraperTab());
        }

        public void SetIndex(int index)
        {
            SalesNavigatorScraperTabs.SelectedIndex = index;
        }
    }
}