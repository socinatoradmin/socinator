using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using RedditDominatorUI.CustomControl;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RedditDominatorUI.RDViews.Tools
{
    /// <summary>
    /// Interaction logic for AutoReplyTab.xaml
    /// </summary>
    public partial class AutoReplyTab : UserControl
    {
        public AutoReplyTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(AutoReplyConfiguration
                        .GetSingeltonObjectAutoReplyConfiguration)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.AutoReplyToNewMessage))
                }
            };
            AutoReplyTabs.ItemsSource = tabItems;
        }
    }
}
