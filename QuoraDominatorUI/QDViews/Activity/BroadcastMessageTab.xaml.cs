using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using QuoraDominatorUI.CustomControl;
using QuoraDominatorUI.QDViews.Activity.Message;

namespace QuoraDominatorUI.QDViews
{
    /// <summary>
    ///     Interaction logic for MessageTab.xaml
    /// </summary>
    public partial class BroadcastMessageTab
    {
        public BroadcastMessageTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(() => new MessageConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.BroadcastMessages))
                }
            };
            MessageTabs.ItemsSource = tabItems;
        }
    }
}