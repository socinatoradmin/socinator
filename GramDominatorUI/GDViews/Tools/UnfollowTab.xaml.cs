using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using GramDominatorUI.CustomControl;
using GramDominatorUI.GDViews.Tools.Unfollow;

namespace GramDominatorUI.GDViews.Tools
{
    /// <summary>
    ///     Interaction logic for UnfollowTab.xaml
    /// </summary>
    public partial class UnfollowTab : UserControl
    {
        public UnfollowTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") != null
                        ? Application.Current.FindResource("LangKeyConfiguration").ToString()
                        : "Configuration",
                    Content = new Lazy<UserControl>(() => new UnFollowConfiguration())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") != null
                        ? Application.Current.FindResource("LangKeyReports").ToString()
                        : "Reports",
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.Unfollow))
                }
            };
            UnFollowTabs.ItemsSource = TabItems;
        }
    }
}