using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.EventInviter;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for EventInviterConfiguration.xaml
    /// </summary>
    public partial class EventInviterConfiguration
    {
        public EventInviterConfiguration()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(EventInviterTools.GetSingeltonObjectEventInviterTools)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.EventInviter))
                }
            };
            SendRequesttabs.ItemsSource = tabItems;
        }

        //        private static FanpageLikerConfiguration _currentSendRequestConfigurationTab;
        //
        //        public static FanpageLikerConfiguration GetSingeltonObjectSendRequestConfigurationTab()
        //        {
        //            return _currentSendRequestConfigurationTab ?? (_currentSendRequestConfigurationTab = new FanpageLikerConfiguration());
        //        }
    }
}