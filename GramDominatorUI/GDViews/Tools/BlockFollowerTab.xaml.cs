using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using GramDominatorUI.CustomControl;
using GramDominatorUI.GDViews.Tools.BlockFollower;

namespace GramDominatorUI.GDViews.Tools
{
    /// <summary>
    ///     Interaction logic for BlockFollowerTab.xaml
    /// </summary>
    public partial class BlockFollowerTab : UserControl
    {
        public BlockFollowerTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") != null
                        ? Application.Current.FindResource("LangKeyConfiguration").ToString()
                        : "Configuration",
                    Content = new Lazy<UserControl>(BlockFollowerConfiguration
                        .GetSingeltonObjectBlockFollowerConfiguration)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") != null
                        ? Application.Current.FindResource("LangKeyReports").ToString()
                        : "Reports",
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.BlockFollower))
                }
            };
            BlockFollowerTabs.ItemsSource = tabItems;
        }
    }
}