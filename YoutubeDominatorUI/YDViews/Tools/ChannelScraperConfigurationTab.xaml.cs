using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using YoutubeDominatorUI.CustomControl;
using YoutubeDominatorUI.YDViews.Tools.Scraper;

namespace YoutubeDominatorUI.YDViews.Tools
{
    /// <summary>
    ///     Interaction logic for ChannelScraperConfigurationTab.xaml
    /// </summary>
    public partial class ChannelScraperConfigurationTab
    {
        public ChannelScraperConfigurationTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(() =>
                        ChannelScraperConfiguration.GetSingeltonObjectChannelScraperConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.ChannelScraper))
                }
            };
            ChannelScraperTabs.ItemsSource = tabItems;
        }
    }
}