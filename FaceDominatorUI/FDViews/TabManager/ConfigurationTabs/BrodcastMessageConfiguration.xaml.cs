using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.BrodcastMessage;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for BrodcastMessageConfiguration.xaml
    /// </summary>
    public partial class BrodcastMessageConfiguration
    {
        public BrodcastMessageConfiguration()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(BroadcastMessageTools.GetSingeltonObjectGroupJoinerTools)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.BroadcastMessages))
                }
            };
            SendRequesttabs.ItemsSource = tabItems;
        }

        //        private static BrodcastMessageConfiguration _currentSendRequestConfigurationTab;
        //
        //        public static BrodcastMessageConfiguration GetSingeltonObjectSendRequestConfigurationTab()
        //        {
        //            return _currentSendRequestConfigurationTab ?? (_currentSendRequestConfigurationTab = new BrodcastMessageConfiguration());
        //        }
    }
}