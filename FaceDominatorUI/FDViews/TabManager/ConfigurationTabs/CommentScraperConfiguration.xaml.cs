using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.CommentScraper;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for CommentScraperConfiguration.xaml
    /// </summary>
    public partial class CommentScraperConfiguration
    {
        public CommentScraperConfiguration()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(CommentScraperTools.GetSingeltonObjectCommentScraperTools)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.CommentScraper))
                }
            };
            SendRequesttabs.ItemsSource = tabItems;
        }

        //        private static CommentScraperConfiguration _currentCommentScraperConfigurationTab;
        //
        //        public static CommentScraperConfiguration GetSingeltonObjectCommentScraperConfigurationTab()
        //        {
        //            return _currentCommentScraperConfigurationTab ?? (_currentCommentScraperConfigurationTab = new CommentScraperConfiguration());
        //        }
    }
}