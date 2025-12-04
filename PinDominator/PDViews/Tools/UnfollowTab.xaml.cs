using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using PinDominator.CustomControl;
using PinDominator.PDViews.Tools.Unfollow;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace PinDominator.PDViews.Tools
{
    /// <summary>
    ///     Interaction logic for UnfollowTab.xaml
    /// </summary>
    public partial class UnfollowTab
    {
        public UnfollowTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(UnFollowConfiguration.GetSingletonObjectUnFollowConfiguration)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.Unfollow))
                }
            };
            UnFollowTabs.ItemsSource = tabItems;
        }
    }
}