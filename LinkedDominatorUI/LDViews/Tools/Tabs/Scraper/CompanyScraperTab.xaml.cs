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
    ///     Interaction logic for CompanyScraperTab.xaml
    /// </summary>
    public partial class CompanyScraperTab : UserControl
    {
        private static CompanyScraperTab objCompanyScraperTab;

        public CompanyScraperTab()
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
                            CompanyScraperConfiguration.GetSingeltonObjectCompanyScraperConfiguration())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.CompanyScraper))
                    }
                };
                CompanyScraperTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static CompanyScraperTab GetSingeltonObjectCompanyScraperTab()
        {
            return objCompanyScraperTab ?? (objCompanyScraperTab = new CompanyScraperTab());
        }
    }
}