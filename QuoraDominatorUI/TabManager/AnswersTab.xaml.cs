using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using QuoraDominatorUI.QDViews.Answers;

namespace QuoraDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for MessagesTab.xaml
    /// </summary>
    public partial class AnswersTab
    {
        private static AnswersTab _answersTab;

        public AnswersTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyAnswerOnQuestion").ToString(),
                    Content = new Lazy<UserControl>(AnswerOnQuestion.GetSingeltonAnswerOnQuestion)
                }
            };
            AnswersTabs.ItemsSource = tabItems;
        }

        public static AnswersTab GetSingeltonObjectAnswersTab()
        {
            return _answersTab ?? (_answersTab = new AnswersTab());
        }

        public void SetIndex(int index)
        {
            AnswersTabs.SelectedIndex = index;
        }
    }
}