using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.PageInviter;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for PageInviterConfiguration.xaml
    /// </summary>
    public partial class PageInviterConfiguration
    {
        public PageInviterConfiguration()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(PageInviterTools.GetSingeltonObjectPageInviter)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.PageInviter))
                }
            };
            SendRequesttabs.ItemsSource = tabItems;
        }

        //        private static PageInviterConfiguration _currentSendRequestConfigurationTab;
        //
        //        public static PageInviterConfiguration GetSingeltonObjectSendRequestConfigurationTab()
        //        {
        //            return _currentSendRequestConfigurationTab ?? (_currentSendRequestConfigurationTab = new PageInviterConfiguration());
        //        }
    }
}