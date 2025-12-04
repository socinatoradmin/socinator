using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using QuoraDominatorUI.CustomControl;
using QuoraDominatorUI.QDViews.Activity.ReportAnswer;

namespace QuoraDominatorUI.QDViews.Activity
{
    /// <summary>
    ///     Interaction logic for ReportAnswer.xaml
    /// </summary>
    public partial class ReportAnswerTab
    {
        public ReportAnswerTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(() => new ReportAnswerConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.ReportAnswers))
                }
            };
            ReportAnswerTabs.ItemsSource = tabItems;
        }
    }
}