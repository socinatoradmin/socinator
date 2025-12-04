using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using PinDominator.CustomControl;
using PinDominator.PDViews.Tools.Follow;

namespace PinDominator.PDViews.Tools
{
    /// <summary>
    ///     Interaction logic for FollowTab.xaml
    /// </summary>
    public partial class FollowTab
    {
        public FollowTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(FollowConfiguration.GetSingletonObjectFollowConfiguration)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.Follow))
                }
            };
            FollowTabs.ItemsSource = tabItems;
        }
    }
}