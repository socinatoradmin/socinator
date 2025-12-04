using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using YoutubeDominatorUI.CustomControl;
using YoutubeDominatorUI.YDViews.Tools.WatchVideo;

namespace YoutubeDominatorUI.YDViews.Tools
{
    /// <summary>
    ///     Interaction logic for ViewIncreaserConfigurationTab.xaml
    /// </summary>
    public partial class ViewIncreaserConfigurationTab
    {
        public ViewIncreaserConfigurationTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(() =>
                        ViewIncreaserConfiguration.GetSingeltonObjectViewIncreaserConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.ViewIncreaser))
                }
            };
            ViewIncreaserTabs.ItemsSource = tabItems;
        }
    }
}