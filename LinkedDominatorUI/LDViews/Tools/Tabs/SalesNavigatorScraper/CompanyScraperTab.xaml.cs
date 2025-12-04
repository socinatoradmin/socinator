using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using LinkedDominatorUI.CustomControl;
using LinkedDominatorUI.LDViews.Tools.SalesNavigatorScraper;

namespace LinkedDominatorUI.LDViews.Tools.Tabs.SalesNavigatorScraper
{
    /// <summary>
    ///     Interaction logic for CompanyScraperTab.xaml
    /// </summary>
    public partial class CompanyScraperTab : UserControl
    {
        private static CompanyScraperTab _objCompanyScraperTab;

        public CompanyScraperTab()
        {
            try
            {
                InitializeComponent();
                var tabItems = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConfiguration").ToString(),
                        Content = new Lazy<UserControl>(() =>
                            CompanyScraperConfiguration.GetSingletonObjectCompanyScraperConfiguration())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() =>
                            new AccountReport(ActivityType.SalesNavigatorCompanyScraper))
                    }
                };
                CompanyScraperTabs.ItemsSource = tabItems;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static CompanyScraperTab GetSingletonObjectCompanyScraperTab()
        {
            return _objCompanyScraperTab ?? (_objCompanyScraperTab = new CompanyScraperTab());
        }
    }
}