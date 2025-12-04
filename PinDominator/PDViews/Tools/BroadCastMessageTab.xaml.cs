using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using PinDominator.CustomControl;
using PinDominator.PDViews.Tools.BroadCastMessage;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace PinDominator.PDViews.Tools
{
    /// <summary>
    ///     Interaction logic for BroadCastMessageTab.xaml
    /// </summary>
    public partial class BroadCastMessageTab
    {
        public BroadCastMessageTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(BroadCastMessageConfiguration
                        .GetSingletonObjectBroadCastMessageConfiguration)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.BroadcastMessages))
                }
            };
            BroadCastMessageTabs.ItemsSource = tabItems;
        }
    }
}