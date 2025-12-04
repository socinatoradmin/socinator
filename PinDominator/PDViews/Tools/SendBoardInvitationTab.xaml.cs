using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using PinDominator.CustomControl;
using PinDominator.PDViews.Tools.SendBoardInvitation;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace PinDominator.PDViews.Tools
{
    /// <summary>
    ///     Interaction logic for SendBoardInvitationTab.xaml
    /// </summary>
    public partial class SendBoardInvitationTab
    {
        public SendBoardInvitationTab()
        {
            InitializeComponent();

            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(SendBoardInvitationConfiguration
                        .GetSingletonObjectSendBoardInvitationConfiguration)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.SendBoardInvitation))
                }
            };
            SendBoardInvitationTabs.ItemsSource = tabItems;
        }
    }
}