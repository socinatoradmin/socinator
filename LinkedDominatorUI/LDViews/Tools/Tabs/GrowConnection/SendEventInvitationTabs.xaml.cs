using DominatorHouseCore.Models;
using LinkedDominatorUI.LDViews.Tools.GrowConnection;
using System.Collections.Generic;
using System;
using System.Windows.Controls;
using DominatorHouseCore;
using LinkedDominatorUI.CustomControl;
using DominatorHouseCore.Enums;
namespace LinkedDominatorUI.LDViews.Tools.Tabs.GrowConnection
{
    /// <summary>
    /// Interaction logic for SendEventInvitationTabs.xaml
    /// </summary>
    public partial class SendEventInvitationTabs : UserControl
    {
        private static SendEventInvitationTabs _instance;
        public SendEventInvitationTabs()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConfiguration").ToString(),
                        Content = new Lazy<UserControl>(SendEventInvitationBase
                            .GetSingleton)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.EventInviter))
                    }
                };
                sendEventInvitationTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }
        public static SendEventInvitationTabs Instance()=>_instance ?? (_instance = new SendEventInvitationTabs());
    }
}
