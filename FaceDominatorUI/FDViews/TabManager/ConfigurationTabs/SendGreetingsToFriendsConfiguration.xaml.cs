using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.SendGreetingToFriends;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for AutoReplyMessageConfiguration.xaml
    /// </summary>
    public partial class SendGreetingsToFriendsConfiguration
    {
        public SendGreetingsToFriendsConfiguration()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(SendGreetingToFriendsTools
                        .GetSingeltonObjectSendGreetingsToFriendsTools)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.SendGreetingsToFriends))
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