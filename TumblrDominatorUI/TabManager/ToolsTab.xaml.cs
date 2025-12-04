using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TumblrDominatorCore.ViewModels;
using TumblrDominatorUI.TumblrView.Activity;

namespace TumblrDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for ToolsTab.xaml
    /// </summary>
    public partial class ToolsTab
    {
        private ToolsTab()
        {
            InitializeComponent();

            var tabItems = InitializeTabControls();
            ToolTab.ItemsSource = tabItems;
        }


        private static ToolsTab ObjToolTabs { get; set; }

        public static ToolsTab GetSingletonToolTabs()
        {
            return ObjToolTabs ?? (ObjToolTabs = new ToolsTab());
        }

        public List<TabItemTemplates> InitializeTabControls()
        {
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyFollow") == null
                        ? "Follow"
                        : Application.Current.FindResource("LangKeyFollow")?.ToString(),
                    Content = new Lazy<UserControl>(() => new FollowTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyUnfollow") == null
                        ? "Unfollow"
                        : Application.Current.FindResource("LangKeyUnfollow")?.ToString(),
                    Content = new Lazy<UserControl>(() => new UnfollowTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReblog") == null
                        ? "Reblog"
                        : Application.Current.FindResource("LangKeyReblog")?.ToString(),
                    Content = new Lazy<UserControl>(() => new ReblogTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyLike") == null
                        ? "Like"
                        : Application.Current.FindResource("LangKeyLike")?.ToString(),
                    Content = new Lazy<UserControl>(() => new LikeTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyUnLike") == null
                        ? "UnLike"
                        : Application.Current.FindResource("LangKeyUnLike")?.ToString(),
                    Content = new Lazy<UserControl>(() => new UnLikeTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyComments") == null
                        ? "Comments"
                        : Application.Current.FindResource("LangKeyComments")?.ToString(),
                    Content = new Lazy<UserControl>(() => new CommentTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyBroadcastMessages") == null
                        ? "Broadcast Messages"
                        : Application.Current.FindResource("LangKeyBroadcastMessages")?.ToString(),
                    Content = new Lazy<UserControl>(() => new MessageTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyPostScraper") == null
                        ? "PostScraper"
                        : Application.Current.FindResource("LangKeyPostScraper")?.ToString(),
                    Content = new Lazy<UserControl>(() => new PostScraperTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyUserScraper") == null
                        ? "UserScraper"
                        : Application.Current.FindResource("LangKeyUserScraper")?.ToString(),
                    Content = new Lazy<UserControl>(UserScraperTab.GetSingletonObjectUserScraperTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyCommentScraper") == null
                        ? "CommentScraper"
                        : Application.Current.FindResource("LangKeyCommentScraper")?.ToString(),
                    Content = new Lazy<UserControl>(CommentScraperTab.GetSingletonObjectCommentScraperTab)
                },
                new TabItemTemplates
                {
                    Title = "LangKeyPrivateBlacklistUsers".FromResourceDictionary(),
                    Content = new Lazy<UserControl>(() =>
                        new PrivateBlacklistUserControl(new PrivateBlackListViewModel(SocialNetworks.Tumblr)))
                },
                new TabItemTemplates
                {
                    Title = "LangKeyPrivateWhitelistUsers".FromResourceDictionary(),
                    Content = new Lazy<UserControl>(() =>
                        new PrivateWhitelistUserControl(new PrivateWhiteListViewModel(SocialNetworks.Tumblr)))
                }
            };
            return tabItems;
        }
    }
}