using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using PinDominator.CustomControl;
using PinDominator.PDViews.Tools.AcceptBoardInvitation;

namespace PinDominator.PDViews.Tools
{
    /// <summary>
    ///     Interaction logic for AcceptBoardInvitationTab.xaml
    /// </summary>
    public partial class AcceptBoardInvitationTab
    {
        public AcceptBoardInvitationTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(AcceptBoardInvitationConfiguration
                        .GetSingletonObjectAcceptBoardInvitationConfiguration)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.AcceptBoardInvitation))
                }
            };
            AcceptBoardInvitationTabs.ItemsSource = tabItems;
        }
    }
}