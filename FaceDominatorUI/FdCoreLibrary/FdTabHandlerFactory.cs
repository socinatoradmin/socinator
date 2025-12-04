using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using FaceDominatorUI.FDViews.TabManager;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FaceDominatorUI.FdCoreLibrary
{
    public class FdTabHandlerFactory : ITabHandlerFactory
    {
        private static FdTabHandlerFactory _instance;

        private FdTabHandlerFactory()
        {
            TabInitializer();
            NetworkName = $"{SocialNetworks.Facebook.ToString()}  Dominator";
        }

        public string NetworkName { get; set; }

        public List<TabItemTemplates> NetworkTabs { get; set; }
        public List<TabItemTemplates> HelpSectionTabs { get; set; }

        public void UpdateAccountCustomControl(SocialNetworks networks)
        {
            NetworkTabs[0].Content = new Lazy<UserControl>(() => new AccountTab());
        }

        public static FdTabHandlerFactory Instance(AccessorStrategies strategies)
        {
            return _instance ?? (_instance = new FdTabHandlerFactory());
        }

        private void TabInitializer()
        {
            NetworkTabs = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyAccounts") == null
                        ? "Accounts"
                        : Application.Current.FindResource("LangKeyAccounts")?.ToString(),
                    Content = new Lazy<UserControl>(() => new AccountTab())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyGrowFriends") == null
                        ? "Grow Friends"
                        : Application.Current.FindResource("LangKeyGrowFriends")?.ToString(),
                    Content = new Lazy<UserControl>(FbFriendsTab.GetSingeltonObjectFbFriendsTab)
                },
                //new TabItemTemplates
                //{
                //    Title=Application.Current.FindResource("FdlangFbPoster").ToString(),
                //    Content=new Lazy<UserControl>(FbPosterTab.GetSingeltonObjectFbPosterTab)
                //},

                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyBroadcastMessages") == null
                        ? "Broadcast Messages"
                        : Application.Current.FindResource("LangKeyBroadcastMessages")?.ToString(),
                    Content = new Lazy<UserControl>(FbMessangerTab.GetSingeltonObjectFbMessangerTab)
                },

                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyGrowGroups") == null
                        ? "Grow Groups"
                        : Application.Current.FindResource("LangKeyGrowGroups")?.ToString(),
                    Content = new Lazy<UserControl>(FbGroupsTab.GetSingeltonObjectFbGroupsTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyEngage") == null
                        ? "Engage"
                        : Application.Current.FindResource("LangKeyEngage")?.ToString(),
                    Content = new Lazy<UserControl>(FbLikerCommentorTab.GetSingeltonObjectFbLikerCommentorTab)
                },
                //new TabItemTemplates
                //{
                //    Title =Application.Current.FindResource("LangKeyEventCreater") == null
                //        ? "Fb Creator"
                //        : Application.Current.FindResource("LangKeyEventCreater")?.ToString(),
                //    Content=new Lazy<UserControl>(FbCreaterTab.GetSingeltonObjectCreaterTab)

                //},
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyInviter") == null
                        ? "Fb Inviter"
                        : Application.Current.FindResource("LangKeyInviter")?.ToString(),
                    Content = new Lazy<UserControl>(FbInviterTab.GetSingeltonObjectFbInviterTab)
                },
                //new TabItemTemplates
                //{
                //    Title=Application.Current.FindResource("FdlangFbEvents").ToString(),
                //    Content=new Lazy<UserControl>(FbEventsTab.GetSingeltonObjectFbInviterTab)

                //},
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyScraper") == null
                        ? "Scraper"
                        : Application.Current.FindResource("LangKeyScraper")?.ToString(),
                    Content = new Lazy<UserControl>(FbScraperTab.GetSingeltonObjectFbScraperTab)
                },
                //new TabItemTemplates
                //{
                //    Title=Application.Current.FindResource("FdlangFbInviter").ToString(),
                //    Content=new Lazy<UserControl>(FbInviterTab.GetSingeltonObjectFbInviterTab)

                //},
                //new TabItemTemplates
                //{
                //    Title =Application.Current.FindResource("LangKeyEventCreater") == null
                //        ? "Event Creater"
                //        : Application.Current.FindResource("LangKeyEventCreater")?.ToString(),
                //    Content=new Lazy<UserControl>(FbScraperTab.GetSingeltonObjectFbScraperTab)

                //},

                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyCampaigns") == null
                        ? "Campaigns"
                        : Application.Current.FindResource("LangKeyCampaigns")?.ToString(),
                    Content = new Lazy<UserControl>(CampaignTab.GetSingeltonObjectCampaignTab)
                },
                //new TabItemTemplates
                //{
                //    Title = Application.Current.FindResource("LangKeyLivechat") == null
                //        ? "LiveChat"
                //        : Application.Current.FindResource("LangKeyLivechat")?.ToString(),
                //    Content = new Lazy<UserControl>(() => new LiveChat(SocialNetworks.Facebook))
                //},
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeySettings") == null
                        ? "Settings"
                        : Application.Current.FindResource("LangKeySettings")?.ToString(),
                    Content = new Lazy<UserControl>(SettingsTab.GetSingeltonObjectFbSettingsTab)
                }
            };
        }
    }
}