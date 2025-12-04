using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using TwtDominatorUI.CustomControl;
using TwtDominatorUI.TDViews.Tools.Tweet;

namespace TwtDominatorUI.TDViews.Tools
{
    /// <summary>
    ///     Interaction logic for TweetToTab.xaml
    /// </summary>
    public partial class TweetToTab : UserControl
    {
        public TweetToTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),
                    Content = new Lazy<UserControl>(() => new TweetToConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountWiseReport(ActivityType.TweetTo))
                }
            };
            CommentTabControl.ItemsSource = TabItems;
        }
    }
}