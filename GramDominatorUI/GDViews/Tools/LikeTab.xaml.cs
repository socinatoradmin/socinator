using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using GramDominatorUI.CustomControl;
using GramDominatorUI.GDViews.Tools.Like;

namespace GramDominatorUI.GDViews.Tools
{
    /// <summary>
    ///     Interaction logic for LikeTab.xaml
    /// </summary>
    public partial class LikeTab : UserControl
    {
        public LikeTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") != null
                        ? Application.Current.FindResource("LangKeyConfiguration").ToString()
                        : "Configuration",
                    Content = new Lazy<UserControl>(LikeConfiguration.GetSingeltonObjectLikeConfiguration)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") != null
                        ? Application.Current.FindResource("LangKeyReports").ToString()
                        : "Reports",
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.Like))
                }
            };
            LikeTabs.ItemsSource = tabItems;
        }
    }
}