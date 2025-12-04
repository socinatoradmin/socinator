using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using RedditDominatorUI.CustomControl;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RedditDominatorUI.RDViews.Tools
{
    /// <summary>
    ///     Interaction logic for SubscribeTab.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class SubscribeTab
    {
        public SubscribeTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(SubscriberConfiguration.GetSingeltonObjectSubscriberConfiguration)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.Subscribe))
                }
            };
            SubscribeTabs.ItemsSource = tabItems;
        }
    }
}