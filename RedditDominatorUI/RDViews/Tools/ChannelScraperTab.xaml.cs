using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using RedditDominatorUI.CustomControl;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RedditDominatorUI.RDViews.Tools
{
    /// <summary>
    ///     Interaction logic for ChannelScraperTab.xaml
    /// </summary>
    public partial class ChannelScraperTab : UserControl
    {
        public ChannelScraperTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(ChannelScraperConfiguration
                        .GetSingeltonObjectChannelScraperConfiguration)
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