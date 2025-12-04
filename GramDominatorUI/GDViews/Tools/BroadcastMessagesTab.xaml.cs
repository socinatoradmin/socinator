using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using GramDominatorUI.CustomControl;
using GramDominatorUI.GDViews.Tools.BroadcastMessages;

namespace GramDominatorUI.GDViews.Tools
{
    /// <summary>
    ///     Interaction logic for BroadcastMessagesTab.xaml
    /// </summary>
    public partial class BroadcastMessagesTab : UserControl
    {
        public BroadcastMessagesTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") != null
                        ? Application.Current.FindResource("LangKeyConfiguration").ToString()
                        : "Configuration",
                    Content = new Lazy<UserControl>(BroadcastMessagesConfig
                        .GetSingeltonObjectSendMessageToFollowerConfig)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") != null
                        ? Application.Current.FindResource("LangKeyReports").ToString()
                        : "Reports",
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.BroadcastMessages))
                }
            };
            BroadcastMessages.ItemsSource = TabItems;
        }

        private static BroadcastMessagesTab CurrentBroadcastMessagesTab { get; set; }

        public static BroadcastMessagesTab GetSingeltonBroadcastMessagesTab()
        {
            return CurrentBroadcastMessagesTab ?? (CurrentBroadcastMessagesTab = new BroadcastMessagesTab());
        }
    }
}