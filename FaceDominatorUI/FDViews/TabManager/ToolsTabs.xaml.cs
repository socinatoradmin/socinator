using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDViewModel;
using FaceDominatorUI.FDViews.TabManager.ConfigurationTabs;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager
{
    /// <summary>
    ///     Interaction logic for ToolsTabs.xaml
    /// </summary>
    public partial class ToolsTabs
    {
        public ToolsTabs()
        {
            InitializeComponent();
            var tabItems = InitializeTabControls();
            ToolTab.ItemsSource = tabItems;
        }

        public List<TabItemTemplates> InitializeTabControls()
        {
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeySendFriendRequest").ToString(),
                    Content = new Lazy<UserControl>(() => new SendRequestConfigurationTab())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyIncomingFriendRequest").ToString(),
                    Content = new Lazy<UserControl>(() => new IncommingFriendConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyUnfriend").ToString(),
                    Content = new Lazy<UserControl>(() => new UnfriendConfigurationTabs())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyWithdrawSentRequest").ToString(),
                    Content = new Lazy<UserControl>(() => new CancelSentRequestConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyBrodcastMessage").ToString(),
                    Content = new Lazy<UserControl>(() => new BrodcastMessageConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyAutoReplyToMessage").ToString(),
                    Content = new Lazy<UserControl>(() => new AutoReplyMessageConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeySendMessageToNewFriends").ToString(),
                    Content = new Lazy<UserControl>(() => new SendMessageToNewFriendsConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeySendGreetingsToFriends").ToString(),
                    Content = new Lazy<UserControl>(() => new SendGreetingsToFriendsConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeySendMessageToFanpages").ToString(),
                    Content = new Lazy<UserControl>(() => new SendMessageToFanpagesConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyFanPageLiker").ToString(),
                    Content = new Lazy<UserControl>(() => new FanpageLikerConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeySendMessageToPlaces").ToString(),
                    Content = new Lazy<UserControl>(() => new SendMessageToPlacesConfiguration())
                },

                new TabItemTemplates
                {
                    Title = FindResource("LangKeyPostLike").ToString(),
                    Content = new Lazy<UserControl>(() => new PostLikerConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyPostComment").ToString(),
                    Content = new Lazy<UserControl>(() => new PostCommentorConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyCommentLiker").ToString(),
                    Content = new Lazy<UserControl>(() => new CommentLikerConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyGroupJoiner").ToString(),
                    Content = new Lazy<UserControl>(() => new GroupJoinerTabs())
                },

                new TabItemTemplates
                {
                    Title = FindResource("LangKeyGroupUnjoiner").ToString(),
                    Content = new Lazy<UserControl>(() => new UnjoinConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyProfileScraper").ToString(),
                    Content = new Lazy<UserControl>(() => new ProfileScraperConfigurationTabs())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyFanpageScraper").ToString(),
                    Content = new Lazy<UserControl>(() => new FanpageScraperConfiguration())
                },


                new TabItemTemplates
                {
                    Title = FindResource("LangKeyGroupScraper").ToString(),
                    Content = new Lazy<UserControl>(() => new GroupScarperConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyCommentScraper").ToString(),
                    Content = new Lazy<UserControl>(() => new CommentScraperConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyCommentRepliesScraper").ToString(),
                    Content = new Lazy<UserControl>(() => new CommnetrRepliesScraperConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyPostScraper").ToString(),
                    Content = new Lazy<UserControl>(() => new PostScraperConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyDownloadMedia").ToString(),
                    Content = new Lazy<UserControl>(() => new DOwnloadMediaConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyPlaceScraper").ToString(),
                    Content = new Lazy<UserControl>(() => new PlaceScraperConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyPageInviter").ToString(),
                    Content = new Lazy<UserControl>(() => new PageInviterConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyGroupInviter").ToString(),
                    Content = new Lazy<UserControl>(() => new GroupInviterConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyEventInviter").ToString(),
                    Content = new Lazy<UserControl>(() => new EventInviterConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyWatchPartyInviter").ToString(),
                    Content = new Lazy<UserControl>(() => new WatchPartyInviterConfiguration())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReplyToComments").ToString(),
                    Content = new Lazy<UserControl>(() => new ReplyToCommentTab())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyEventCreater").ToString(),
                    Content = new Lazy<UserControl>(() => new EventCreaterConfiguration())
                },
                new TabItemTemplates
                {
                    Title = "LangKeyMakeGroupAdmin".FromResourceDictionary(),
                    Content = new Lazy<UserControl>(() => new MakeGroupAdminConfiguration())
                },
                new TabItemTemplates
                {
                    Title = "LangKeyPrivateBlacklistUsers".FromResourceDictionary(),
                    Content = new Lazy<UserControl>(() =>
                        new PrivateBlacklistUserControl(new PrivateBlackListViewModel(SocialNetworks.Facebook)))
                },
                new TabItemTemplates
                {
                    Title = "LangKeyPrivateWhitelistUsers".FromResourceDictionary(),
                    Content = new Lazy<UserControl>(() =>
                        new PrivateWhitelistUserControl(new PrivateWhiteListViewModel(SocialNetworks.Facebook)))
                }
            };
            return tabItems;
        }
    }
}