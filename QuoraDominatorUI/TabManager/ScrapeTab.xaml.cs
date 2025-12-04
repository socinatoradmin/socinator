using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using QuoraDominatorUI.QDViews.Scrape;

namespace QuoraDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for ScrapeTab.xaml
    /// </summary>
    public partial class ScrapeTab
    {
        private static ScrapeTab _objScrapeTab;

        public ScrapeTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyUsers").ToString(),
                    Content = new Lazy<UserControl>(UserScraper.GetSingeltonObjectUserScraper)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyQuestions").ToString(),
                    Content = new Lazy<UserControl>(QuestionsScraper.GetSingeltonObjectQuestionsScraper)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyAnswers").ToString(),
                    Content = new Lazy<UserControl>(AnswersScraper.GetSingeltonObjectAnswersScraper)
                }
            };
            ScrapeTabControl.ItemsSource = tabItems;
        }

        public static ScrapeTab GetSingletonObjectScrapeTab()
        {
            return _objScrapeTab ?? (_objScrapeTab = new ScrapeTab());
        }

        public void SetIndex(int index)
        {
            ScrapeTabControl.SelectedIndex = index;
        }
    }
}