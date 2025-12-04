using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using GramDominatorUI.CustomControl;
using GramDominatorUI.GDViews.Tools.SendMessageToFollowers;

namespace GramDominatorUI.GDViews.Tools
{
    /// <summary>
    ///     Interaction logic for SendMessageToFollowerTab.xaml
    /// </summary>
    public partial class SendMessageToFollowerTab : UserControl
    {
        public SendMessageToFollowerTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") != null
                        ? Application.Current.FindResource("LangKeyConfiguration").ToString()
                        : "Configuration",
                    Content = new Lazy<UserControl>(SendMessageToFollowerConfig
                        .GetSingeltonObjectSendMessageToFollowerConfig)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") != null
                        ? Application.Current.FindResource("LangKeyReports").ToString()
                        : "Reports",
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.SendMessageToFollower))
                }
            };
            SendMessageToFollower.ItemsSource = TabItems;
        }

        private static SendMessageToFollowerTab CurrentSendMessageToFollowerTab { get; set; }

        public static SendMessageToFollowerTab GetSingeltonSendMessageToFollowerTab()
        {
            return CurrentSendMessageToFollowerTab ??
                   (CurrentSendMessageToFollowerTab = new SendMessageToFollowerTab());
        }
    }
}