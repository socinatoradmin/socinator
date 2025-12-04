using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using RedditDominatorCore.RDViewModel;
using RedditDominatorUI.RDViews.Tools;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RedditDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for ToolsTab.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class ToolsTab : UserControl
    {
        public ToolsTab()
        {
            InitializeComponent();
            var tabItems = InitializeTabs();
            ToolTab.ItemsSource = tabItems;
        }

        private List<TabItemTemplates> InitializeTabs()
        {
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = "LangKeyFollow".FromResourceDictionary(),
                    Content = new Lazy<UserControl>(() => new FollowTab())
                },
                new TabItemTemplates
                {
                    Title = "LangKeyUnfollow".FromResourceDictionary(),
                    Content = new Lazy<UserControl>(() => new UnFollowTab())
                },
                //new TabItemTemplates
                //{
                //    Title = FindResource("langReply").ToString(),
                //    Content=new Lazy<UserControl>(()=>new ReplyTab())
                //},
                //new TabItemTemplates
                //{
                //    Title = FindResource("langDelete").ToString(),
                //    Content=new Lazy<UserControl>(()=>new DeleteTab())
                //},
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyUpvote") == null
                        ? "Upvote"
                        : Application.Current.FindResource("LangKeyUpvote")?.ToString(),
                    Content = new Lazy<UserControl>(() => new UpvoteTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyDownvote") == null
                        ? "Downvote"
                        : Application.Current.FindResource("LangKeyDownvote")?.ToString(),
                    Content = new Lazy<UserControl>(() => new DownvoteTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyChannelScraper") == null
                        ? "ChannelScraper"
                        : Application.Current.FindResource("LangKeyChannelScraper")?.ToString(),
                    Content = new Lazy<UserControl>(() => new ChannelScraperTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyCommentScraper") == null
                        ? "CommentScraper"
                        : Application.Current.FindResource("LangKeyCommentScraper")?.ToString(),
                    Content = new Lazy<UserControl>(() => new CommentScraperTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyRemoveVote") == null
                        ? "Remove Vote"
                        : Application.Current.FindResource("LangKeyRemoveVote")?.ToString(),
                    Content = new Lazy<UserControl>(() => new RemoveVoteTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeySubscribe") == null
                        ? "Subscribe"
                        : Application.Current.FindResource("LangKeySubscribe")?.ToString(),
                    Content = new Lazy<UserControl>(() => new SubscribeTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyUnsubscribe") == null
                        ? "UnSubscribe"
                        : Application.Current.FindResource("LangKeyUnsubscribe")?.ToString(),
                    Content = new Lazy<UserControl>(() => new UnSubscriberTab())
                },
                //new TabItemTemplates
                //{
                //    Title = FindResource("langDownvote").ToString(),
                //    Content=new Lazy<UserControl>(()=>new ReplyTab())
                //},
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyUrlscraper").ToString(),
                    Content = new Lazy<UserControl>(() => new UrlScraperTab())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyUserScraper").ToString(),
                    Content = new Lazy<UserControl>(() => new UserScraperTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyComment") == null
                        ? "Comment"
                        : Application.Current.FindResource("LangKeyComment")?.ToString(),
                    Content = new Lazy<UserControl>(() => new CommentTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReply") == null
                        ? "Reply"
                        : Application.Current.FindResource("LangKeyReply")?.ToString(),
                    Content = new Lazy<UserControl>(() => new ReplyTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyEditComment") == null
                        ? "Edit Comment"
                        : Application.Current.FindResource("LangKeyEditComment")?.ToString(),
                    Content = new Lazy<UserControl>(() => new EditCommentTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyUpvoteComments") == null
                        ? "Upvote Comments"
                        : Application.Current.FindResource("LangKeyUpvoteComments")?.ToString(),
                    Content = new Lazy<UserControl>(() => new UpvoteForCommentTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyDownvoteComments") == null
                        ? "Downvote Comments"
                        : Application.Current.FindResource("LangKeyDownvoteComments")?.ToString(),
                    Content = new Lazy<UserControl>(() => new DownvoteForCommentTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyRemoveVoteFromComments") == null
                        ? "Remove Vote From Comments"
                        : Application.Current.FindResource("LangKeyRemoveVoteFromComments")?.ToString(),
                    Content = new Lazy<UserControl>(() => new RemoveVoteForCommentTab())
                },
                new TabItemTemplates
                {
                    Title =Application.Current.FindResource("LangKeyBroadcastMessages") == null
                        ? "BroadcastMesseges"
                        : Application.Current.FindResource("LangKeyBroadcastMessages")?.ToString(),
                    Content=new Lazy<UserControl>(()=>new BroadcastMessageTab())
                },
                new TabItemTemplates
                {
                    Title =Application.Current.FindResource("LangKeyAutoReplyToNewMessage") == null
                        ? "AutoReply"
                        : Application.Current.FindResource("LangKeyAutoReplyToNewMessage")?.ToString(),
                    Content=new Lazy<UserControl>(()=>new AutoReplyTab())
                },
                new TabItemTemplates
                {
                    Title = "LangKeyPrivateBlacklistUsers".FromResourceDictionary(),
                    Content = new Lazy<UserControl>(() =>
                        new PrivateBlacklistUserControl(new PrivateBlackListViewModel(SocialNetworks.Reddit)))
                },
                new TabItemTemplates
                {
                    Title = "LangKeyPrivateWhitelistUsers".FromResourceDictionary(),
                    Content = new Lazy<UserControl>(() =>
                        new PrivateWhitelistUserControl(new PrivateWhiteListViewModel(SocialNetworks.Reddit)))
                }
            };
            return tabItems;
        }
    }
}