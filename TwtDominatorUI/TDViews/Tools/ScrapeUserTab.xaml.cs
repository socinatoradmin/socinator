using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using TwtDominatorUI.CustomControl;
using TwtDominatorUI.TDViews.Tools.ScrapeUser;

namespace TwtDominatorUI.TDViews.Tools
{
    /// <summary>
    ///     Interaction logic for ScrapeUserTab.xaml
    /// </summary>
    public partial class ScrapeUserTab : UserControl
    {
        private static ScrapeUserTab objScrapeUserTab;

        public ScrapeUserTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(() => new ScrapeUserConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountWiseReport(ActivityType.UserScraper))
                }
            };
            ScrapeUserTabControl.ItemsSource = TabItems;
        }

        public static ScrapeUserTab GetSingletonScrapeUserTab()
        {
            return objScrapeUserTab ?? (objScrapeUserTab = new ScrapeUserTab());
        }
    }
}