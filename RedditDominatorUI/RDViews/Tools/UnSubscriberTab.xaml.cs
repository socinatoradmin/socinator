using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using RedditDominatorUI.CustomControl;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RedditDominatorUI.RDViews.Tools
{
    /// <summary>
    ///     Interaction logic for UnSubscriberTab.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class UnSubscriberTab
    {
        public UnSubscriberTab()
        {
            InitializeComponent();
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(UnSubscriberConfiguration
                        .GetSingeltonObjectUnSubscriberConfiguration)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.UnSubscribe))
                }
            };
            UnSubscribeTabs.ItemsSource = tabItems;
        }
    }
}