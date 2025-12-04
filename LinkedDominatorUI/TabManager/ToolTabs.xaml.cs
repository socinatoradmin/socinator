using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.LDViewModel.Accounts;
using LinkedDominatorUI.LDViews.Tools.Tabs;
using LinkedDominatorUI.LDViews.Tools.Tabs.Engage;
using LinkedDominatorUI.LDViews.Tools.Tabs.GrowConnection;
using LinkedDominatorUI.LDViews.Tools.Tabs.Messenger;
using LinkedDominatorUI.LDViews.Tools.Tabs.Scraper;

namespace LinkedDominatorUI.TabManager
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
            //UserFilterAction.UserFilterControl = GlobalMethods.ShowUserFilterControl;
        }


        private static ToolTabs ObjToolTabs { get; set; }

        public static ToolTabs GetSingletonToolTabs()
        {
            return ObjToolTabs ?? (ObjToolTabs = new ToolTabs());
        }

        public List<TabItemTemplates> InitializeTabControls()
        {
            try
            {
                var tabItems = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConnectionRequest").ToString(),
                        Content = new Lazy<UserControl>(ConnectionRequestTab.GetSingeltonObjectConnectionRequestTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyAcceptConnectionRequest").ToString(),
                        Content = new Lazy<UserControl>(AcceptConnectionTab.GetSingeltonObjectAcceptConnectionTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyRemoveConnections").ToString(),
                        Content = new Lazy<UserControl>(RemoveConnectionsTab.GetSingeltonObjectRemoveConnectionsTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyWithdrawConnectionRequest").ToString(),
                        Content = new Lazy<UserControl>(WithdrawConnectionRequestTab
                            .GetSingeltonObjectWithdrawConnectionRequestTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyExportConnection").ToString(),
                        Content = new Lazy<UserControl>(ExportConnectionTab.GetSingeltonObjExportConnectionTab)
                    },

                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyFollowPages").ToString(),
                        Content = new Lazy<UserControl>(FollowPageTab.GetSingeltonObjectFollowPageTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeySendPageInvitations").ToString(),
                        Content = new Lazy<UserControl>(SendPageInvitationTab.GetSingletonObjectSendPageInvitationTab)
                    },

                    //new TabItemTemplates
                    //{
                    //    Title = "Event Inviter",
                    //    Content = new Lazy<UserControl>(SendEventInvitationTabs.Instance)
                    //},
                    //new TabItemTemplates
                    //{
                    //    Title = FindResource("LangKeySendGroupInvitations").ToString(),
                    //    Content = new Lazy<UserControl>(SendGroupMemberInvitationTab.GetSingletonObjectSendGroupInvitationTab)
                    //},
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyBlockUser").ToString(),
                        Content = new Lazy<UserControl>(BlockUserTab.GetSingletonObjectBlockUserTab)
                    },

                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyBroadcastMessages").ToString(),
                        Content = new Lazy<UserControl>(BroadcastMessagesTab.GetSingletonObjectBroadcastMessagesTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyAutoReplyToNewMessage").ToString(),
                        Content = new Lazy<UserControl>(AutoReplyToNewMessagesTab
                            .GetSingeltonObjectAutoReplyToNewMessagesTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeySendMessageToNewConnection").ToString(),
                        Content = new Lazy<UserControl>(SendMessageToNewConnectionTab
                            .GetSingletonObjectSendMessageToNewConnectionTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeySendGreetingsToConnections").ToString(),
                        Content = new Lazy<UserControl>(SendGreetingsToConnectionsTab
                            .GetSingletonSendGreetingsToConnectionsTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyDeleteConversations").ToString(),
                        Content = new Lazy<UserControl>(DeleteConversationTab.GetSingeltonObjectDeleteConversationsTab)
                    },

                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyLike").ToString(),
                        Content = new Lazy<UserControl>(LikeTab.GetSingeltonObjectLikeTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyComment").ToString(),
                        Content = new Lazy<UserControl>(CommentTab.GetSingeltonObjectCommentTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeySharePost").ToString(),
                        Content = new Lazy<UserControl>(ShareTab.GetSingeltonObjectShareTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyGroupJoiner").ToString(),
                        Content = new Lazy<UserControl>(GroupJoinerTab.GetSingeltonObjectGroupJoinerTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyGroupUnjoiner").ToString(),
                        Content = new Lazy<UserControl>(GroupUnJoinerTab.GetSingeltonObjectGroupUnJoinerTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyUserScraper").ToString(),
                        Content = new Lazy<UserControl>(UserScraperTab.GetSingeltonObjectUserScraperTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyJobScraper").ToString(),
                        Content = new Lazy<UserControl>(JobScraperTab.GetSingletonObjectJobScraperTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyCompanyScraper").ToString(),
                        Content = new Lazy<UserControl>(CompanyScraperTab.GetSingeltonObjectCompanyScraperTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyMessageConversationsScraper").ToString(),
                        Content = new Lazy<UserControl>(MessageConversationScraperTab
                            .GetSingletonObjectMessageConversationScraperTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyProfileEndorsement").ToString(),
                        Content = new Lazy<UserControl>(ProfileEndorsementTab.GetSingeltonObjectProfileEndorsementTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeySalesnavigatorUserscraper").ToString(),
                        Content = new Lazy<UserControl>(LDViews.Tools.Tabs.SalesNavigatorScraper.UserScraperTab
                            .GetSingeltonObjectUserScraperTab)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeySalesnavigatorCompanyscraper").ToString(),
                        Content = new Lazy<UserControl>(LDViews.Tools.Tabs.SalesNavigatorScraper.CompanyScraperTab
                            .GetSingletonObjectCompanyScraperTab)
                    },
                    new TabItemTemplates
                    {
                        Title = "LangKeyPrivateBlacklistUsers".FromResourceDictionary(),
                        Content = new Lazy<UserControl>(() =>
                            new PrivateBlacklistUserControl(new PrivateBlackListViewModel(SocialNetworks.LinkedIn)))
                    },
                    new TabItemTemplates
                    {
                        Title = "LangKeyPrivateWhitelistUsers".FromResourceDictionary(),
                        Content = new Lazy<UserControl>(() =>
                            new PrivateWhitelistUserControl(new PrivateWhiteListViewModel(SocialNetworks.LinkedIn)))
                    }
                };
                return tabItems;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
                return null;
            }
        }

        //Connection Request
    }
}