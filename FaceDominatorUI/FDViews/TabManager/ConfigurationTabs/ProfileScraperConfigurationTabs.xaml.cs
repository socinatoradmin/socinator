using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.ProfileScraper;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for ProfileScraperConfigurationTabs.xaml
    /// </summary>
    public partial class ProfileScraperConfigurationTabs
    {
        public ProfileScraperConfigurationTabs()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(ProfileScraperTools.GetSingeltonObjectGroupJoinerTools)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.ProfileScraper))
                }
            };
            SendRequesttabs.ItemsSource = tabItems;
        }

        /*
                private static ProfileScraperConfigurationTabs _currentSendRequestConfigurationTab;
        */

        /*
                public static ProfileScraperConfigurationTabs GetSingeltonObjectSendRequestConfigurationTab()
                {
                    return _currentSendRequestConfigurationTab ?? (_currentSendRequestConfigurationTab = new ProfileScraperConfigurationTabs());
                }
        */
    }
}