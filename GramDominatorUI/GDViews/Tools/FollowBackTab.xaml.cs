using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using GramDominatorUI.CustomControl;
using GramDominatorUI.GDViews.Tools.FollowBack;

namespace GramDominatorUI.GDViews.Tools
{
    /// <summary>
    ///     Interaction logic for FollowBackTab.xaml
    /// </summary>
    public partial class FollowBackTab : UserControl
    {
        public FollowBackTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") != null
                        ? Application.Current.FindResource("LangKeyConfiguration").ToString()
                        : "Configuration",
                    Content = new Lazy<UserControl>(FollowBackConfiguration.GetSingeltonObjectFollowBackConfiguration)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") != null
                        ? Application.Current.FindResource("LangKeyReports").ToString()
                        : "Reports",
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.FollowBack))
                }
            };
            FollowBackTabs.ItemsSource = tabItems;
        }
    }
}