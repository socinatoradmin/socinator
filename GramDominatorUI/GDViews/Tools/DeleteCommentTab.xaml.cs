using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using GramDominatorUI.CustomControl;
using GramDominatorUI.GDViews.Tools.DeleteComment;

namespace GramDominatorUI.GDViews.Tools
{
    /// <summary>
    ///     Interaction logic for DeleteCommentTab.xaml
    /// </summary>
    public partial class DeleteCommentTab : UserControl
    {
        public DeleteCommentTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") != null
                        ? Application.Current.FindResource("LangKeyConfiguration").ToString()
                        : "Configuration",
                    Content = new Lazy<UserControl>(DeleteCommentConfiguration
                        .GetSingeltonObjectDeleteCommentConfiguration)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") != null
                        ? Application.Current.FindResource("LangKeyReports").ToString()
                        : "Reports",
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.DeleteComment))
                }
            };
            DeleteComment.ItemsSource = TabItems;
        }
    }
}