using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using LinkedDominatorUI.CustomControl;
using LinkedDominatorUI.LDViews.Tools.GrowConnection;

namespace LinkedDominatorUI.LDViews.Tools.Tabs.GrowConnection
{
    /// <summary>
    ///     Interaction logic for SendPageInvitationTab.xaml
    /// </summary>
    public partial class SendPageInvitationTab : UserControl
    {
        private static SendPageInvitationTab objSendPageInvitationTab;

        public SendPageInvitationTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConfiguration").ToString(),
                        Content = new Lazy<UserControl>(SendPageInvitationConfiguration
                            .GetSingeltonObjectSendPageInvitationConfiguration)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.SendPageInvitations))
                    }
                };
                sendPageInvitationTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        public static SendPageInvitationTab GetSingletonObjectSendPageInvitationTab()
        {
            return objSendPageInvitationTab ?? (objSendPageInvitationTab = new SendPageInvitationTab());
        }
    }
}