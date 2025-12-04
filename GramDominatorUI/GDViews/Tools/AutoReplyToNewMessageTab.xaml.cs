using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using GramDominatorUI.CustomControl;
using GramDominatorUI.GDViews.Tools.AutoReplyToNewMessages;

namespace GramDominatorUI.GDViews.Tools
{
    /// <summary>
    ///     Interaction logic for AutoReplyToNewMessageTab.xaml
    /// </summary>
    public partial class AutoReplyToNewMessageTab : UserControl
    {
        public AutoReplyToNewMessageTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") != null
                        ? Application.Current.FindResource("LangKeyConfiguration").ToString()
                        : "Configuration",
                    Content = new Lazy<UserControl>(AutoReplyToNewMessageConfig
                        .GetSingeltonObjectSendMessageToFollowerConfig)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") != null
                        ? Application.Current.FindResource("LangKeyReports").ToString()
                        : "Reports",
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.AutoReplyToNewMessage))
                }
            };
            AutoReplyToNewMessage.ItemsSource = TabItems;
        }

        private static AutoReplyToNewMessageTab CurrentAutoReplyToNewMessageTab { get; set; }

        public static AutoReplyToNewMessageTab GetSingeltonAutoReplyToNewMessageTab()
        {
            return CurrentAutoReplyToNewMessageTab ??
                   (CurrentAutoReplyToNewMessageTab = new AutoReplyToNewMessageTab());
        }
    }
}