using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using QuoraDominatorUI.QDViews.Reports;

namespace QuoraDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for ReportTab.xaml
    /// </summary>
    public partial class ReportTab
    {
        private static ReportTab _objReportTab;

        public ReportTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyUsers").ToString(),
                    Content = new Lazy<UserControl>(ReportUsers.GetSingeltonObjectReportUsers)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyAnswers").ToString(),
                    Content = new Lazy<UserControl>(ReportAnswers.GetSingeltonObjectReportAnswers)
                }
            };
            ReportTabControl.ItemsSource = tabItems;
        }

        public static ReportTab GetSingletonObjectReportTab()
        {
            return _objReportTab ?? (_objReportTab = new ReportTab());
        }

        public void SetIndex(int index)
        {
            ReportTabControl.SelectedIndex = index;
        }
    }
}