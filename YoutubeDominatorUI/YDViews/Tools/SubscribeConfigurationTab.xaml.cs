using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using YoutubeDominatorUI.CustomControl;
using YoutubeDominatorUI.YDViews.Tools.GrowSubscribers;

namespace YoutubeDominatorUI.YDViews.Tools
{
    /// <summary>
    ///     Interaction logic for SubscribeConfigurationTab.xaml
    /// </summary>
    public partial class SubscribeConfigurationTab
    {
        public SubscribeConfigurationTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(() =>
                        SubscribeConfiguration.GetSingeltonObjectSubscribeConfiguration())
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