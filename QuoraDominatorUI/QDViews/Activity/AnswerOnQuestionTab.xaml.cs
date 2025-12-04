using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using QuoraDominatorUI.CustomControl;
using QuoraDominatorUI.QDViews.Activity.Answer;

namespace QuoraDominatorUI.QDViews.Activity
{
    /// <summary>
    ///     Interaction logic for AnswerScraperConfigurationTab.xaml
    /// </summary>
    public partial class AnswerOnQuestionTab
    {
        public AnswerOnQuestionTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(() => new AnswerOnQuestion())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.AnswerOnQuestions))
                }
            };
            AnswerScraperTabs.ItemsSource = tabItems;
        }
    }
}