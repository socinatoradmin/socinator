using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.CommentRepliesScraper;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for CommnetrRepliesScraperConfiguration.xaml
    /// </summary>
    public partial class CommnetrRepliesScraperConfiguration
    {
        public CommnetrRepliesScraperConfiguration()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(CommnetrRepliesScraperTools
                        .GetSingletonObjectCommnetrRepliesScraperTools)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.CommentRepliesScraper))
                }
            };
            CommnetrRepliesScraperabs.ItemsSource = tabItems;
        }
    }
}