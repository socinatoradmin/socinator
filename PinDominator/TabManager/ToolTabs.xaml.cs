using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using PinDominator.PDViews.Tools;
using PinDominatorCore.PDViewModel;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace PinDominator.TabManager
{
    /// <summary>
    ///     Interaction logic for ToolTabs.xaml
    /// </summary>
    public partial class ToolTabs
    {
        public ToolTabs()
        {
            InitializeComponent();
            var tabItems = InitializeTabs();
            //DominatorJobProcessFactory.PdAccountConfigScheduler += AccountConfigScheduler;
            //DominatorScraperFactory.PdAccountConfigScraper += AccountConfigScraper;
            ToolTab.ItemsSource = tabItems;
        }

        private List<TabItemTemplates> InitializeTabs()
        {
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyCreateBoard").ToString(),
                    Content = new Lazy<UserControl>(() => new CreateBoardTab())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyAcceptBoardInvitation").ToString(),
                    Content = new Lazy<UserControl>(() => new AcceptBoardInvitationTab())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeySendBoardInvitation").ToString(),
                    Content = new Lazy<UserControl>(() => new SendBoardInvitationTab())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyFollow").ToString(),
                    Content = new Lazy<UserControl>(() => new FollowTab())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyUnfollow").ToString(),
                    Content = new Lazy<UserControl>(() => new UnfollowTab())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyFollowBacks").ToString(),
                    Content = new Lazy<UserControl>(() => new FollowBackTab())
                },
                //new TabItemTemplates
                //{
                //    Title = FindResource("LangKeyTry").ToString(),
                //    Content = new Lazy<UserControl>(() => new TryTab())
                //},
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyComments").ToString(),
                    Content = new Lazy<UserControl>(() => new CommentTab())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyBroadcastMesseges").ToString(),
                    Content = new Lazy<UserControl>(() => new BroadCastMessageTab())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyAutoReplyToNewMessage").ToString(),
                    Content = new Lazy<UserControl>(() => new AutoReplyToNewMessegeTab())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyMessageToNewFollowers").ToString(),
                    Content = new Lazy<UserControl>(() => new SendMessageToNewFollowersTab())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyUserScraper").ToString(),
                    Content = new Lazy<UserControl>(() => new UserScraperTab())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyPinScraper").ToString(),
                    Content = new Lazy<UserControl>(() => new PinScraperTab())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyBoardScraper").ToString(),
                    Content = new Lazy<UserControl>(() => new BoardScraperTab())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyEditPin").ToString(),
                    Content = new Lazy<UserControl>(() => new EditPinTab())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyDeletePins").ToString(),
                    Content = new Lazy<UserControl>(() => new DeletePinsTab())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyRepin").ToString(),
                    Content = new Lazy<UserControl>(() => new RepinsTab())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyPrivateBlacklist").ToString(),
                    Content = new Lazy<UserControl>(() =>
                        new PrivateBlacklistUserControl(new PrivateBlackListViewModel(SocialNetworks.Pinterest)))
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyPrivateWhitelist").ToString(),
                    Content = new Lazy<UserControl>(() =>
                        new PrivateWhitelistUserControl(new PrivateWhiteListViewModel(SocialNetworks.Pinterest)))
                }
            };
            return tabItems;
        }
    }
}