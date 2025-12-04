using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using QuoraDominatorUI.CustomControl;

namespace QuoraDominatorUI.QDViews.Activity
{
    /// <summary>
    ///     Interaction logic for AnswerScraperConfigurationTab.xaml
    /// </summary>
    public partial class AnswerScraperConfigurationTab
    {
        public AnswerScraperConfigurationTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(() => new AnswerScraperConfiguration.AnswerScraperConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.AnswersScraper))
                }
            };
            AnswerScraperTabs.ItemsSource = tabItems;
        }
    }
}