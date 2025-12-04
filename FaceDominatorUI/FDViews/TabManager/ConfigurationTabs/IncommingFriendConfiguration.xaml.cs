using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.IncommingFriendRequest;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for IncommingFriendConfiguration.xaml
    /// </summary>
    public partial class IncommingFriendConfiguration
    {
        public IncommingFriendConfiguration()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(IncommingFriendRequestTools
                        .GetSingeltonObjectIncommingFriendRequestTools)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.IncommingFriendRequest))
                }
            };
            SendRequesttabs.ItemsSource = tabItems;
        }

        //        private static IncommingFriendConfiguration _currentSendRequestConfigurationTab;
        //
        //        public static IncommingFriendConfiguration GetSingeltonObjectIncommingFriendConfigurationTab()
        //        {
        //            return _currentSendRequestConfigurationTab ?? (_currentSendRequestConfigurationTab = new IncommingFriendConfiguration());
        //        }
    }
}