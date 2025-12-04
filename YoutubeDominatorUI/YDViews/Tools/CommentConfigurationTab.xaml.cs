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
    ///     Interaction logic for CommentConfigurationTab.xaml
    /// </summary>
    public partial class CommentConfigurationTab
    {
        public CommentConfigurationTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(() => CommentConfiguration.GetSingeltonObjectCommentConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.Comment))
                }
            };
            CommentTabs.ItemsSource = tabItems;
        }
    }
}