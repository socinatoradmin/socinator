using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using PinDominator.CustomControl;
using PinDominator.PDViews.Tools.Repost;

namespace PinDominator.PDViews.Tools
{
    /// <summary>
    ///     Interaction logic for RepinsTab.xaml
    /// </summary>
    public partial class RepinsTab
    {
        public RepinsTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(RePinConfiguration.GetSingeltonObjectRePinConfiguration)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.Repin))
                }
            };
            RepinsTabs.ItemsSource = tabItems;
        }
    }
}