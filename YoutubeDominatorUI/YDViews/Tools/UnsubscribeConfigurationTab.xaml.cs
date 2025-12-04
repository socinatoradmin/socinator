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
    ///     Interaction logic for UnsubscribeConfigurationTab.xaml
    /// </summary>
    public partial class UnsubscribeConfigurationTab
    {
        public UnsubscribeConfigurationTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(() =>
                        UnsubscribeConfiguration.GetSingeltonObjectUnsubscribeConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.UnSubscribe))
                }
            };
            UnsubscribeTabs.ItemsSource = tabItems;
        }
    }
}