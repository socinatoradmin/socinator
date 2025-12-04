using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using TwtDominatorCore.TDViewModel.ViewModel;
using TwtDominatorUI.TDViews.Tools;

namespace TwtDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for ToolsTab.xaml
    /// </summary>
    public partial class ToolsTab : UserControl
    {
        public ToolsTab()
        {
            InitializeComponent();
            var tabItems = InitializeTabs();
            ToolTabControl.ItemsSource = tabItems;
        }

        private List<TabItemTemplates> InitializeTabs()
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
                    Title = Application.Current.FindResource("LangKeyFollowBack") == null
                        ? "Follow Back"
                        : Application.Current.FindResource("LangKeyFollowBack")?.ToString(),
                    Content = new Lazy<UserControl>(() => new FollowBackTab())
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
                    Title = Application.Current.FindResource("LangKeyMute") == null
                        ? "Mute"
                        : Application.Current.FindResource("LangKeyMute")?.ToString(),
                    Content = new Lazy<UserControl>(() => new MuteTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReposter") == null
                        ? "Reposter"
                        : Application.Current.FindResource("LangKeyReposter")?.ToString(),
                    Content = new Lazy<UserControl>(() => new ReposterTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyRetweet") == null
                        ? "ReTweet"
                        : Application.Current.FindResource("LangKeyRetweet")?.ToString(),
                    Content = new Lazy<UserControl>(() => new RetweetTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyDelete") == null
                        ? "Delete"
                        : Application.Current.FindResource("LangKeyDelete")?.ToString(),
                    Content = new Lazy<UserControl>(() => new DeleteTab())
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
                    Title = Application.Current.FindResource("LangKeyComment") == null
                        ? "Comment"
                        : Application.Current.FindResource("LangKeyComment")?.ToString(),
                    Content = new Lazy<UserControl>(() => new CommentTab())
                },
                new TabItemTemplates

                {
                    Title = Application.Current.FindResource("LangKeyBroadcastMessage") == null
                        ? "Broadcast Message"
                        : Application.Current.FindResource("LangKeyBroadcastMessage")?.ToString(),

                    Content = new Lazy<UserControl>(() => new BroadcastMessageTab())
                },
                new TabItemTemplates

                {
                    Title = Application.Current.FindResource("LangKeyAutoReply") == null
                        ? "Auto Reply"
                        : Application.Current.FindResource("LangKeyAutoReply")?.ToString(),
                    Content = new Lazy<UserControl>(() => new AutoReplyTab())
                },
                new TabItemTemplates

                {
                    Title = Application.Current.FindResource("LangKeyMessageToNewFollowers") == null
                        ? "Message to New Followers"
                        : Application.Current.FindResource("LangKeyMessageToNewFollowers")?.ToString(),
                    Content = new Lazy<UserControl>(() => new MessageToNewFollowerTab())
                },
                new TabItemTemplates

                {
                    Title = Application.Current.FindResource("LangKeyScrapeUsers") == null
                        ? "Scrape Users"
                        : Application.Current.FindResource("LangKeyScrapeUsers")?.ToString(),
                    Content = new Lazy<UserControl>(() => new ScrapeUserTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyScrapeTweet") == null
                        ? "Scrape Tweet"
                        : Application.Current.FindResource("LangKeyScrapeTweet")?.ToString(),
                    Content = new Lazy<UserControl>(() => new ScrapeTweetTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyWelcomeTweet") == null
                        ? "Welcome Tweet"
                        : Application.Current.FindResource("LangKeyWelcomeTweet")?.ToString(),
                    Content = new Lazy<UserControl>(() => new WelcomeTweetTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyUnlike") == null
                        ? "Unlike"
                        : Application.Current.FindResource("LangKeyUnlike")?.ToString(),
                    Content = new Lazy<UserControl>(() => new UnLikeTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyTweetTo") == null
                        ? "TweetTo"
                        : Application.Current.FindResource("LangKeyTweetTo")?.ToString(),
                    Content = new Lazy<UserControl>(() => new TweetToTab())
                },

                new TabItemTemplates
                {
                    Title = "LangKeyPrivateBlacklistUsers".FromResourceDictionary(),
                    Content = new Lazy<UserControl>(() =>
                        new PrivateBlacklistUserControl(new PrivateBlackListViewModel(SocialNetworks.Twitter)))
                },
                new TabItemTemplates
                {
                    Title = "LangKeyPrivateWhitelistUsers".FromResourceDictionary(),
                    Content = new Lazy<UserControl>(() =>
                        new PrivateWhitelistUserControl(new PrivateWhiteListViewModel(SocialNetworks.Twitter)))
                }
            };
            return tabItems;
        }
    }
}