using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using TwtDominatorUI.CustomControl;
using TwtDominatorUI.TDViews.Tools.Comment;

namespace TwtDominatorUI.TDViews.Tools
{
    /// <summary>
    ///     Interaction logic for CommentTab.xaml
    /// </summary>
    public partial class CommentTab : UserControl
    {
        public CommentTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(() => new CommentConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountWiseReport(ActivityType.Comment))
                }
            };
            CommentTabControl.ItemsSource = TabItems;
        }

        //private static CommentTab objCommentTab;
        //public static CommentTab GetSingletonCommentTab()
        //{
        //    return objCommentTab ?? (objCommentTab = new CommentTab());
        //}
    }
}