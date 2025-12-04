using DominatorHouseCore.Models;
using RedditDominatorUI.RDViews.Voting;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RedditDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for VotingTab.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class VotingTab
    {
        private static VotingTab _currVotingTab;

        private VotingTab()
        {
            InitializeComponent();
            _currVotingTab = this;
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyUpvote").ToString(),
                    Content = new Lazy<UserControl>(Upvote.GetSingletonObjectUpvote)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyDownvote").ToString(),
                    Content = new Lazy<UserControl>(Downvote.GetSingletonObjectDownvote)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyRemoveVote").ToString(),
                    Content = new Lazy<UserControl>(Removevote.GetSingletonObjectRemovevote)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyUpvoteComments").ToString(),
                    Content = new Lazy<UserControl>(UpvoteForComment.GetSingletonObjectUpvoteForComment)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyDownvoteComments").ToString(),
                    Content = new Lazy<UserControl>(DownvoteForComment.GetSingeltonObjectDownvoteForComment)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyRemoveVoteFromComments").ToString(),
                    Content = new Lazy<UserControl>(RemovevoteForComment.GetSingletonObjectRemovevoteForComment)
                }
            };
            VotingTabs.ItemsSource = tabItems;
        }

        public static int SelectedIndex { get; internal set; }

        public static VotingTab GetSingletonObjectVotingTab()
        {
            return _currVotingTab ?? (_currVotingTab = new VotingTab());
        }

        public void SetIndex(int index)
        {
            VotingTabs.SelectedIndex = index;
        }
    }
}