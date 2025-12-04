using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using YoutubeDominatorUI.CustomControl;
using YoutubeDominatorUI.YDViews.Tools.Engage;

namespace YoutubeDominatorUI.YDViews.Tools
{
    /// <summary>
    ///     Interaction logic for LikeConfigurationTab.xaml
    /// </summary>
    public partial class LikeConfigurationTab
    {
        public LikeConfigurationTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(() => LikeConfiguration.GetSingeltonObjectLikeConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.Like))
                }
            };
            LikeTabs.ItemsSource = tabItems;
        }
    }
}