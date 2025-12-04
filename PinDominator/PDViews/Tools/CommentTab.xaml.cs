using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using PinDominator.CustomControl;
using PinDominator.PDViews.Tools.Comments;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace PinDominator.PDViews.Tools
{
    /// <summary>
    ///     Interaction logic for CommentTab.xaml
    /// </summary>
    public partial class CommentTab
    {
        public CommentTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(CommentConfiguration.GetSingletonObjectCommentConfiguration)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.Comment))
                }
            };
            CommentTabs.ItemsSource = tabItems;
        }
    }
}