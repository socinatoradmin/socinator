using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDViewModel;
using GramDominatorUI.GDViews.Tools;
using GramDominatorUI.Utility;

namespace GramDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for ToolTabs.xaml
    /// </summary>
    public partial class ToolTabs : UserControl
    {
        public ToolTabs()
        {
            InitializeComponent();
            var tabItems = InitializeTabControls();
            ToolTab.ItemsSource = tabItems;
            UserFilterAction.UserFilterControl = GlobalMethods.ShowUserFilterControl;
        }


        private static ToolTabs ObjToolTabs { get; set; }

        public static ToolTabs GetSingletonToolTabs()
        {
            return ObjToolTabs ?? (ObjToolTabs = new ToolTabs());
        }


        public List<TabItemTemplates> InitializeTabControls()
        {
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyFollower") != null
                        ? Application.Current.FindResource("LangKeyFollower").ToString()
                        : "Grow Followers",
                    Content = new Lazy<UserControl>(() => new FollowTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyFollowBack") != null
                        ? Application.Current.FindResource("LangKeyFollowBack").ToString()
                        : "Follow Back﻿",
                    Content = new Lazy<UserControl>(() => new FollowBackTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyBlockFollower") != null
                        ? Application.Current.FindResource("LangKeyBlockFollower").ToString()
                        : "Block Follower﻿",
                    Content = new Lazy<UserControl>(() => new BlockFollowerTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyUnfollower") != null
                        ? Application.Current.FindResource("LangKeyUnfollower").ToString()
                        : "Unfollower",
                    Content = new Lazy<UserControl>(() => new UnfollowTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyLike") != null
                        ? Application.Current.FindResource("LangKeyLike").ToString()
                        : "Like",
                    Content = new Lazy<UserControl>(() => new LikeTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyComment") != null
                        ? Application.Current.FindResource("LangKeyComment").ToString()
                        : "Comment",
                    Content = new Lazy<UserControl>(() => new CommentTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyMediaUnliker") != null
                        ? Application.Current.FindResource("LangKeyMediaUnliker").ToString()
                        : "Media Unliker",
                    Content = new Lazy<UserControl>(() => new MediaUnlikerTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyLikeComments") != null
                        ? Application.Current.FindResource("LangKeyLikeComments").ToString()
                        : "Like Comments",
                    Content = new Lazy<UserControl>(LikeCommentsTab.GetSingeltonLikeCommentTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReplyComment") != null
                        ? Application.Current.FindResource("LangKeyReplyComment").ToString()
                        : "Reply Comment",
                    Content = new Lazy<UserControl>(() => new ReplyCommentTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyBroadcastMessages") != null
                        ? Application.Current.FindResource("LangKeyBroadcastMessages").ToString()
                        : "Broadcast Messages",
                    Content = new Lazy<UserControl>(BroadcastMessagesTab.GetSingeltonBroadcastMessagesTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyAutoReplyToNewMessage") != null
                        ? Application.Current.FindResource("LangKeyAutoReplyToNewMessage").ToString()
                        : "Auto Reply To New Message",
                    Content = new Lazy<UserControl>(AutoReplyToNewMessageTab.GetSingeltonAutoReplyToNewMessageTab)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeySendMessageToNewFollowers").ToString(),
                    Content = new Lazy<UserControl>(SendMessageToFollowerTab.GetSingeltonSendMessageToFollowerTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReposter") != null
                        ? Application.Current.FindResource("LangKeyReposter").ToString()
                        : "Reposter",
                    Content = new Lazy<UserControl>(() => new RepostsTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyDelete") != null
                        ? Application.Current.FindResource("LangKeyDelete").ToString()
                        : "Delete",
                    Content = new Lazy<UserControl>(() => new DeletePostTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyUserScraper") != null
                        ? Application.Current.FindResource("LangKeyUserScraper").ToString()
                        : "User Scraper",
                    Content = new Lazy<UserControl>(() => new UserScraperTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyHashtagsScraper") != null
                        ? Application.Current.FindResource("LangKeyHashtagsScraper").ToString()
                        : "Hashtags Scraper",
                    Content = new Lazy<UserControl>(() => new HashtagScraperTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyPostScraper") != null
                        ? Application.Current.FindResource("LangKeyPostScraper").ToString()
                        : "Post Scraper",
                    Content = new Lazy<UserControl>(() => new DownloadPhotosTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyCommentScraper") != null
                        ? Application.Current.FindResource("LangKeyCommentScraper").ToString()
                        : "Comment Scraper",
                    Content = new Lazy<UserControl>(() => new CommentScraperTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyStoryViewer") != null
                        ? Application.Current.FindResource("LangKeyStoryViewer").ToString()
                        : "Story Viewer",
                    Content = new Lazy<UserControl>(() => new StoryViewTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyPrivateBlacklist") != null
                        ? Application.Current.FindResource("LangKeyPrivateBlacklist").ToString()
                        : "Private Blacklist",
                    Content = new Lazy<UserControl>(() =>
                        new PrivateBlacklistUserControl(new PrivateBlackListViewModel(SocialNetworks.Instagram)))
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyPrivateWhitelist") != null
                        ? Application.Current.FindResource("LangKeyPrivateWhitelist").ToString()
                        : "Private Whitelist",
                    Content = new Lazy<UserControl>(() =>
                        new PrivateWhitelistUserControl(new PrivateWhiteListViewModel(SocialNetworks.Instagram)))
                }
            };
            return tabItems;
        }
    }
}