using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.GroupScraper;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for GroupScarperConfiguration.xaml
    /// </summary>
    public partial class GroupScarperConfiguration
    {
        public GroupScarperConfiguration()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(GroupScraperTools.GetSingeltonObjectGroupScraperTools)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.GroupScraper))
                }
            };
            SendRequesttabs.ItemsSource = tabItems;
        }

        //        private static GroupScarperConfiguration _currentSendRequestConfigurationTab;
        //
        //        public static GroupScarperConfiguration GetSingeltonObjectSendRequestConfigurationTab()
        //        {
        //            return _currentSendRequestConfigurationTab ?? (_currentSendRequestConfigurationTab = new GroupScarperConfiguration());
        //        }
    }
}