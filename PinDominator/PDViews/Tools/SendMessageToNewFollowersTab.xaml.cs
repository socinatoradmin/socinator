using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using PinDominator.CustomControl;
using PinDominator.PDViews.Tools.SendMessageToNewFollowers;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace PinDominator.PDViews.Tools
{
    /// <summary>
    ///     Interaction logic for SendMessageToNewFollowersTab.xaml
    /// </summary>
    public partial class SendMessageToNewFollowersTab
    {
        public SendMessageToNewFollowersTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(SendMessageToNewFollowersConfiguration
                        .GetSingletonObjectSendMessageToNewFollowersConfiguration)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.SendMessageToFollower))
                }
            };
            SendMessageToNewFollowersTabs.ItemsSource = tabItems;
        }
    }
}