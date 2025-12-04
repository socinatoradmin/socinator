using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.FanpageScraper;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for FanpageScraperConfiguration.xaml
    /// </summary>
    public partial class FanpageScraperConfiguration
    {
        public FanpageScraperConfiguration()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(FanpageScraperTools.GetSingeltonObjectFanpageScraperTools)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.FanpageScraper))
                }
            };
            SendRequesttabs.ItemsSource = tabItems;
        }

        //        private static FanpageScraperConfiguration _currentSendRequestConfigurationTab;
        //
        //        public static FanpageScraperConfiguration GetSingeltonObjectSendRequestConfigurationTab()
        //        {
        //            return _currentSendRequestConfigurationTab ?? (_currentSendRequestConfigurationTab = new FanpageScraperConfiguration());
        //        }
    }
}