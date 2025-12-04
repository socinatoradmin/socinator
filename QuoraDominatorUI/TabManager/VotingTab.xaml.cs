using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using QuoraDominatorUI.QDViews.Voting;

namespace QuoraDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for VotingTab.xaml
    /// </summary>
    public partial class VotingTab
    {
        private static VotingTab _objVotingTab;

        public VotingTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyUpvoteAnswers") == null
                        ? "Upvote Answers"
                        : Application.Current.FindResource("LangKeyUpvoteAnswers")?.ToString(),

                    Content = new Lazy<UserControl>(UpvoteAnswers.GetSingeltonObjectUpvoteAnswers)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyDownvoteAnswers") == null
                        ? "Downvote Answers"
                        : Application.Current.FindResource("LangKeyDownvoteAnswers")?.ToString(),

                    Content = new Lazy<UserControl>(DownvoteAnswers.GetSingeltonObjectDownvoteAnswers)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyDownvoteQuestions") == null
                        ? "Downvote Questions"
                        : Application.Current.FindResource("LangKeyDownvoteQuestions")?.ToString(),
                    Content = new Lazy<UserControl>(DownvoteQuestions.GetSingeltonObjectDownvote)
                },
                new TabItemTemplates
                {
                    Title = "Upvote Posts",
                    Content = new Lazy<UserControl>(UpvotePosts.GetSingeltonInstance)
                },
                new TabItemTemplates
                {
                    Title = "Downvote Posts",
                    Content = new Lazy<UserControl>(DownvotePost.GetSingeltonInstance)
                }
            };
            VotingTabControl.ItemsSource = tabItems;
        }

        public static VotingTab GetSingletonObjectVotingTab()
        {
            return _objVotingTab ?? (_objVotingTab = new VotingTab());
        }

        public void SetIndex(int index)
        {
            VotingTabControl.SelectedIndex = index;
        }
    }
}