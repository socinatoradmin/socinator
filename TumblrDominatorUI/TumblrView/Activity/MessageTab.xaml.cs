using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TumblrDominatorUI.CustomControl;
using TumblrDominatorUI.TumblrView.Activity.Message;

namespace TumblrDominatorUI.TumblrView.Activity
{
    /// <summary>
    ///     Interaction logic for MessageTab.xaml
    /// </summary>
    public partial class MessageTab
    {
        public MessageTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") == null
                        ? "Configuration"
                        : Application.Current.FindResource("LangKeyConfiguration")?.ToString(),
                    Content = new Lazy<UserControl>(BroadcastMessagesConfig
                        .GetSingeltonObjectSendMessageToFollowerConfig)
                },

                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") == null
                        ? "Reports"
                        : Application.Current.FindResource("LangKeyReports")?.ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.BroadcastMessages))
                }
            };
            MessageTabs.ItemsSource = tabItems;
        }
    }
}