using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using GramDominatorUI.CustomControl;
using GramDominatorUI.GDViews.Tools.MediaUnliker;

namespace GramDominatorUI.GDViews.Tools
{
    /// <summary>
    ///     Interaction logic for MediaUnlikerTab.xaml
    /// </summary>
    public partial class MediaUnlikerTab : UserControl
    {
        public MediaUnlikerTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") != null
                        ? Application.Current.FindResource("LangKeyConfiguration").ToString()
                        : "Configuration",
                    Content = new Lazy<UserControl>(MediaUnlikerConfiguration
                        .GetSingeltonObjectMediaUnlikerConfiguration)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") != null
                        ? Application.Current.FindResource("LangKeyReports").ToString()
                        : "Reports",
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.Unlike))
                }
            };
            MediaUnlikerTabs.ItemsSource = tabItems;
        }
    }
}