using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.PostScraper;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for PostScraperConfiguration.xaml
    /// </summary>
    public partial class PostScraperConfiguration
    {
        public PostScraperConfiguration()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(PostScraperTools.GetSingeltonObjectPostScraperTools)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.PostScraper))
                }
            };
            SendRequesttabs.ItemsSource = tabItems;
        }

        /*
                private static BrodcastMessageConfiguration _currentSendRequestConfigurationTab;
        */

        /*
                public static BrodcastMessageConfiguration GetSingeltonObjectSendRequestConfigurationTab()
                {
                    return _currentSendRequestConfigurationTab ?? (_currentSendRequestConfigurationTab = new BrodcastMessageConfiguration());
                }
        */
    }
}