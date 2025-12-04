using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.GroupInviter;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for GroupInviterConfiguration.xaml
    /// </summary>
    public partial class GroupInviterConfiguration
    {
        public GroupInviterConfiguration()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(GroupInviterTools.GetSingeltonObjectGroupInviterTools)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.GroupInviter))
                }
            };
            SendRequesttabs.ItemsSource = tabItems;
        }

        //        private static GroupInviterConfiguration _currentSendRequestConfigurationTab;
        //
        //        public static GroupInviterConfiguration GetSingeltonObjectSendRequestConfigurationTab()
        //        {
        //            return _currentSendRequestConfigurationTab ?? (_currentSendRequestConfigurationTab = new GroupInviterConfiguration());
        //        }
    }
}