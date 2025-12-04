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
    ///     Interaction logic for JobScraperTab.xaml
    /// </summary>
    public partial class JobScraperTab : UserControl
    {
        private static JobScraperTab _objJobScraperTab;

        public JobScraperTab()
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
                            JobScraperConfiguration.GetSingletonObjectJobScraperConfiguration())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.JobScraper))
                    }
                };
                JobScraperTabs.ItemsSource = tabItems;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        public static JobScraperTab GetSingletonObjectJobScraperTab()
        {
            return _objJobScraperTab ?? (_objJobScraperTab = new JobScraperTab());
        }
    }
}