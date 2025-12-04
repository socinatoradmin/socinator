using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using GramDominatorUI.CustomControl;
using GramDominatorUI.GDViews.Tools.Like;

namespace GramDominatorUI.GDViews.Tools
{
    /// <summary>
    ///     Interaction logic for LikeCommentsTab.xaml
    /// </summary>
    public partial class LikeCommentsTab : UserControl
    {
        public LikeCommentsTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") != null
                        ? Application.Current.FindResource("LangKeyConfiguration").ToString()
                        : "Configuration",
                    Content = new Lazy<UserControl>(LikeCommentsConfiguration.GetSingeltonObjectLikeConfiguration)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") != null
                        ? Application.Current.FindResource("LangKeyReports").ToString()
                        : "Reports",
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.LikeComment))
                }
            };
            LikeCommentsTabs.ItemsSource = tabItems;
        }

        private static LikeCommentsTab CurrentLikeCommentTab { get; set; }

        public static LikeCommentsTab GetSingeltonLikeCommentTab()
        {
            return CurrentLikeCommentTab ?? (CurrentLikeCommentTab = new LikeCommentsTab());
        }
    }
}