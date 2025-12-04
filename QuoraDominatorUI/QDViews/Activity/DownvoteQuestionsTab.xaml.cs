using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using QuoraDominatorUI.CustomControl;
using QuoraDominatorUI.QDViews.Activity.DownvoteQuestions;

namespace QuoraDominatorUI.QDViews.Activity
{
    /// <summary>
    ///     Interaction logic for Down vote Questions.xaml
    /// </summary>
    public partial class DownvoteQuestionsTab
    {
        public DownvoteQuestionsTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(() => new DownvoteQuestionsConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.DownvoteQuestions))
                }
            };
            DownvoteQuestionsTabs.ItemsSource = tabItems;
        }
    }
}