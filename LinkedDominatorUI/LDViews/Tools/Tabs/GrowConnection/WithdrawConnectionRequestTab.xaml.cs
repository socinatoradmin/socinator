using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using LinkedDominatorUI.CustomControl;
using LinkedDominatorUI.LDViews.Tools.GrowConnection;

namespace LinkedDominatorUI.LDViews.Tools.Tabs
{
    /// <summary>
    ///     Interaction logic for WithdrawConnectionRequestTab.xaml
    /// </summary>
    public partial class WithdrawConnectionRequestTab : UserControl
    {
        private static WithdrawConnectionRequestTab objWithdrawConnectionRequestTab;

        public WithdrawConnectionRequestTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConfiguration").ToString(),
                        Content = new Lazy<UserControl>(WithdrawConnectionRequestConfiguration
                            .GetSingeltonObjectWithdrawConnectionRequestConfiguration)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.WithdrawConnectionRequest))
                    }
                };
                WithdrawConnectionRequestTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static WithdrawConnectionRequestTab GetSingeltonObjectWithdrawConnectionRequestTab()
        {
            return objWithdrawConnectionRequestTab ??
                   (objWithdrawConnectionRequestTab = new WithdrawConnectionRequestTab());
        }
    }
}