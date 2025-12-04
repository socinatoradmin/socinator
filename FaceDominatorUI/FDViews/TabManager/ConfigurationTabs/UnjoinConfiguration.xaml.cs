using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.GroupUnjoinerTools;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for UnjoinConfiguration.xaml
    /// </summary>
    public partial class UnjoinConfiguration
    {
        public UnjoinConfiguration()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(GroupUnjoinerTools.GetSingeltonObjectGroupJoinerTools)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.GroupUnJoiner))
                }
            };
            SendRequesttabs.ItemsSource = tabItems;
        }

        /*
                private static UnjoinConfiguration _currentSendRequestConfigurationTab;
        */

        /*
                public static UnjoinConfiguration GetSingeltonObjectSendRequestConfigurationTab()
                {
                    return _currentSendRequestConfigurationTab ?? (_currentSendRequestConfigurationTab = new UnjoinConfiguration());
                }
        */
    }
}