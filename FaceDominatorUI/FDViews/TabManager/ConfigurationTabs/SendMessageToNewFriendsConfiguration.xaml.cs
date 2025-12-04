using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.SendMessageToNewFriends;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for SendMessageToNewFriendsConfiguration.xaml
    /// </summary>
    public partial class SendMessageToNewFriendsConfiguration
    {
        public SendMessageToNewFriendsConfiguration()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(SendMessageToNewFriendsTools
                        .GetSingeltonObjectSendMessageToNewFriendsTools)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.SendMessageToNewFriends))
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