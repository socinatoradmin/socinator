using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using YoutubeDominatorCore.YoutubeViewModel.BlacklistsViewModel;
using YoutubeDominatorUI.YDViews.Tools;

namespace YoutubeDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for ToolsTab.xaml
    /// </summary>
    public partial class ToolsTab
    {
        public ToolsTab()
        {
            InitializeComponent();
            var tabItems = InitializeTabControls();
            ToolTab.ItemsSource = tabItems;
        }

        private static ToolsTab ObjToolTabs { get; set; } = null;


        public List<TabItemTemplates> InitializeTabControls()
        {
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = "Like",
                    Content = new Lazy<UserControl>(() => new LikeConfigurationTab())
                },
                new TabItemTemplates
                {
                    Title = "LikeComment",
                    Content = new Lazy<UserControl>(() => new LikeCommentConfigurationTab())
                },
                new TabItemTemplates
                {
                    Title = "Dislike",
                    Content = new Lazy<UserControl>(() => new DislikeConfigurationTab())
                },
                new TabItemTemplates
                {
                    Title = "Subscribe",
                    Content = new Lazy<UserControl>(() => new SubscribeConfigurationTab())
                },
                new TabItemTemplates
                {
                    Title = "Unsubscribe",
                    Content = new Lazy<UserControl>(() => new UnsubscribeConfigurationTab())
                },
                new TabItemTemplates
                {
                    Title = "Comment",
                    Content = new Lazy<UserControl>(() => new CommentConfigurationTab())
                },
                new TabItemTemplates
                {
                    Title = "Report Video",
                    Content = new Lazy<UserControl>(() => new ReportVideoConfigurationTab())
                },
                new TabItemTemplates
                {
                    Title = "ViewIncreaser",
                    Content = new Lazy<UserControl>(() => new ViewIncreaserConfigurationTab())
                },
                new TabItemTemplates
                {
                    Title = "Channel Scraper",
                    Content = new Lazy<UserControl>(() => new ChannelScraperConfigurationTab())
                },
                new TabItemTemplates
                {
                    Title = "Post Scraper",
                    Content = new Lazy<UserControl>(() => new PostScraperConfigurationTab())
                },
                new TabItemTemplates
                {
                    Title = "Comment Scraper",
                    Content = new Lazy<UserControl>(() => new CommentScraperConfigurationTab())
                },
                new TabItemTemplates
                {
                    Title = "LangKeyPrivateBlacklistUsers".FromResourceDictionary(),
                    Content = new Lazy<UserControl>(() =>
                        new PrivateBlacklistUserControl(new PrivateBlackListViewModel(SocialNetworks.YouTube)))
                },
                new TabItemTemplates
                {
                    Title = "LangKeyPrivateWhitelistUsers".FromResourceDictionary(),
                    Content = new Lazy<UserControl>(() =>
                        new PrivateWhitelistUserControl(new PrivateWhiteListViewModel(SocialNetworks.YouTube)))
                }
            };
            return tabItems;
        }
    }
}