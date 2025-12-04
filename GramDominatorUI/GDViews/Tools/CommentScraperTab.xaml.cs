using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using GramDominatorUI.CustomControl;
using GramDominatorUI.GDViews.Tools.Scraper;

namespace GramDominatorUI.GDViews.Tools
{
    /// <summary>
    ///     Interaction logic for CommentScraperTab.xaml
    /// </summary>
    public partial class CommentScraperTab : UserControl
    {
        public CommentScraperTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyConfiguration") != null
                        ? Application.Current.FindResource("LangKeyConfiguration").ToString()
                        : "Configuration",
                    Content = new Lazy<UserControl>(CommentScraperConfiguration
                        .GetSingeltonObjectCommentScraperConfiguration)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReports") != null
                        ? Application.Current.FindResource("LangKeyReports").ToString()
                        : "Reports",
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.CommentScraper))
                }
            };
            CommentScraperTabControl.ItemsSource = tabItems;
        }
    }
}