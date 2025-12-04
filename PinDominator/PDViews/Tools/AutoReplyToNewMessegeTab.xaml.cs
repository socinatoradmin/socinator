using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using PinDominator.CustomControl;
using PinDominator.PDViews.Tools.AutoReplyToNewMessage;

namespace PinDominator.PDViews.Tools
{
    /// <summary>
    ///     Interaction logic for AutoReplyToNewMessegeTab.xaml
    /// </summary>
    public partial class AutoReplyToNewMessegeTab
    {
        public AutoReplyToNewMessegeTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(AutoReplyToNewMessageConfiguration
                        .GetSingletonObjectAutoReplyToNewMessageConfiguration)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.AutoReplyToNewMessage))
                }
            };
            AutoReplyToNewMessageTabs.ItemsSource = tabItems;
        }
    }
}