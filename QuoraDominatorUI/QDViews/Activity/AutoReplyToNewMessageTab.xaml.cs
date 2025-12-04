using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using QuoraDominatorUI.CustomControl;
using QuoraDominatorUI.QDViews.Activity.Message;

namespace QuoraDominatorUI.QDViews.Activity
{
    /// <summary>
    ///     Interaction logic for AutoReplyToNewMessageTab.xaml
    /// </summary>
    public partial class AutoReplyToNewMessageTab
    {
        public AutoReplyToNewMessageTab()
        {
            InitializeComponent();
            var list = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(() => new AutoReplyToNewMessageConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.AutoReplyToNewMessage))
                }
            };
            MainTab.ItemsSource = list;
        }
    }
}