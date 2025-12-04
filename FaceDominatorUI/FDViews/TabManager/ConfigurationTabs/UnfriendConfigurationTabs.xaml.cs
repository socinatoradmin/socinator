using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.Unfriend;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for UnfriendConfigurationTabs.xaml
    /// </summary>
    public partial class UnfriendConfigurationTabs
    {
        public UnfriendConfigurationTabs()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(UnfriendTools.GetSingeltonObjectUnfriendTools)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.Unfriend))
                }
            };
            SendRequesttabs.ItemsSource = tabItems;
        }

        /*
                private static UnfriendConfigurationTabs _currentSendRequestConfigurationTab;
        */

        /*
                public static UnfriendConfigurationTabs GetSingeltonObjectSendRequestConfigurationTab()
                {
                    return _currentSendRequestConfigurationTab ?? (_currentSendRequestConfigurationTab = new UnfriendConfigurationTabs());
                }
        */
    }
}