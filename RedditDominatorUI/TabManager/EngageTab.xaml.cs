using DominatorHouseCore.Models;
using RedditDominatorUI.RDViews.Engage;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RedditDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for EngageTab.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class EngageTab : UserControl
    {
        private static EngageTab _objEngageTab;

        public EngageTab()
        {
            InitializeComponent();
            _objEngageTab = this;
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyComment").ToString(),
                    Content = new Lazy<UserControl>(Comment.GetSingeltonObjectComment)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReply").ToString(),
                    Content = new Lazy<UserControl>(Reply.GetSingeltonObjectReply)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyEditComment").ToString(),
                    Content = new Lazy<UserControl>(EditComment.GetSingletonObjectEditComment)
                }
            };
            EngageTabs.ItemsSource = tabItems;
        }

        public static int SelectedIndex => 0;

        public static EngageTab GetSingeltonObject_EngageTab()
        {
            return _objEngageTab ?? (_objEngageTab = new EngageTab());
        }

        public void SetIndex(int index)
        {
            EngageTabs.SelectedIndex = index;
        }
    }
}