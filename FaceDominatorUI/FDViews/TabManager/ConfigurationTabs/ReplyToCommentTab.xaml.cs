using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.ReplyToCommentTools;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for ReplyToCommentTab.xaml
    /// </summary>
    public partial class ReplyToCommentTab
    {
        public ReplyToCommentTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(ReplyToCommentConfiguration
                        .GetSingeltonObjectReplyToCommentConfiguration)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.ReplyToComment))
                }
            };
            ReplyTocommentsTabs.ItemsSource = tabItems;
        }
    }
}