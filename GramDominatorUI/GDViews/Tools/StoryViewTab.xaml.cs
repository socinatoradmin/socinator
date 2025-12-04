using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using GramDominatorUI.CustomControl;
using GramDominatorUI.GDViews.Tools.StoryViewer;

namespace GramDominatorUI.GDViews.Tools
{
    /// <summary>
    ///     Interaction logic for StoryViewTab.xaml
    /// </summary>
    public partial class StoryViewTab : UserControl
    {
        public StoryViewTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") != null
                        ? Application.Current.FindResource("LangKeyConfiguration").ToString()
                        : "Configuration",
                    Content = new Lazy<UserControl>(StoryViewerConfiguration.StoryConfigurationSingleton)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") != null
                        ? Application.Current.FindResource("LangKeyReports").ToString()
                        : "Reports",
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.StoryViewer))
                }
            };
            StoryViewTabs.ItemsSource = tabItems;
        }
    }
}