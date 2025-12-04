using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using TwtDominatorUI.CustomControl;
using TwtDominatorUI.TDViews.Tools.ScrapeTweet;

namespace TwtDominatorUI.TDViews.Tools
{
    /// <summary>
    ///     Interaction logic for ScrapeTweetTab.xaml
    /// </summary>
    public partial class ScrapeTweetTab : UserControl
    {
        private static ScrapeTweetTab objScrapeTweetTab;

        public ScrapeTweetTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(() => new ScrapeTweetConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountWiseReport(ActivityType.TweetScraper))
                }
            };
            ScrapeTweetTabControl.ItemsSource = TabItems;
        }

        public static ScrapeTweetTab GetSingletonScrapeTweetTab()
        {
            return objScrapeTweetTab ?? (objScrapeTweetTab = new ScrapeTweetTab());
        }
    }
}