using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using PinDominator.CustomControl;
using PinDominator.PDViews.Tools.FollowBack;

namespace PinDominator.PDViews.Tools
{
    /// <summary>
    ///     Interaction logic for FollowBackTab.xaml
    /// </summary>
    public partial class FollowBackTab
    {
        public FollowBackTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(FollowBackConfiguration.GetSingletonObjectFollowBackConfiguration)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.FollowBack))
                }
            };
            FollowBackTabs.ItemsSource = tabItems;
        }
    }
}