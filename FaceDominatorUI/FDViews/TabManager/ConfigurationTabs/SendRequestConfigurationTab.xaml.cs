using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.SendRequest;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for SendRequestConfigurationTab.xaml
    /// </summary>
    public partial class SendRequestConfigurationTab
    {
        public SendRequestConfigurationTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(() => new SendRequestTools())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.SendFriendRequest))
                }
            };
            SendRequesttabs.ItemsSource = tabItems;
        }

        /*
                private static SendRequestConfigurationTab _currentSendRequestConfigurationTab;
        */

        /*
                public static SendRequestConfigurationTab GetSingeltonObjectSendRequestConfigurationTab()
                {
                    return _currentSendRequestConfigurationTab ?? (_currentSendRequestConfigurationTab = new SendRequestConfigurationTab());
                }
        */
    }
}