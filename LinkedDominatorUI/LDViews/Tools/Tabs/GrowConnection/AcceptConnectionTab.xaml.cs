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
    ///     Interaction logic for AcceptConnectionTab.xaml
    /// </summary>
    public partial class AcceptConnectionTab : UserControl
    {
        private static AcceptConnectionTab objAcceptConnectionTab;

        public AcceptConnectionTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConfiguration").ToString(),
                        Content = new Lazy<UserControl>(() =>
                            AcceptConnectionRequestConfiguration
                                .GetSingeltonObjectAcceptConnectionRequestConfiguration())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.AcceptConnectionRequest))
                    }
                };
                AcceptConnectionRequestTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static AcceptConnectionTab GetSingeltonObjectAcceptConnectionTab()
        {
            return objAcceptConnectionTab ?? (objAcceptConnectionTab = new AcceptConnectionTab());
        }
    }
}