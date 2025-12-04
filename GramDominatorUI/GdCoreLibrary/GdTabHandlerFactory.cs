using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.CustomControl;
using GramDominatorUI.TabManager;
using GramDominatorUI.Utility;

namespace GramDominatorUI.GDCoreLibrary
{
    public class GdTabHandlerFactory : ITabHandlerFactory
    {
        private static GdTabHandlerFactory _instance;
        private AccessorStrategies _strategies;

        private GdTabHandlerFactory(AccessorStrategies strategies)
        {
            TabInitializer(strategies);
            NetworkName = $"{SocialNetworks.Instagram.ToString()}  Dominator";

            #region Initilize Actions                

            UserFilterAction.UserFilterControl = GlobalMethods.ShowUserFilterControl;

            #endregion
        }

        public string NetworkName { get; set; }

        public List<TabItemTemplates> NetworkTabs { get; set; }

        public List<TabItemTemplates> HelpSectionTabs { get; set; }
        public void UpdateAccountCustomControl(SocialNetworks networks)
        {
            NetworkTabs[0].Content = new Lazy<UserControl>(() => new AccountTab(_strategies));
        }

        public static GdTabHandlerFactory Instance(AccessorStrategies strategies)
        {
            return _instance ?? (_instance = new GdTabHandlerFactory(strategies));
        }

        private void TabInitializer(AccessorStrategies strategies)
        {
            _strategies = strategies;

            NetworkTabs = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyAccounts") != null
                        ? Application.Current.FindResource("LangKeyAccounts").ToString()
                        : "Accounts",
                    Content = new Lazy<UserControl>(() => new AccountTab(_strategies))
                },
                //new TabItemTemplates
                //{
                //    Title = (Application.Current.FindResource("LangKeyCreateAccounts")!= null)?Application.Current.FindResource("LangKeyCreateAccounts").ToString():"Create Accounts",
                //    Content = new Lazy<UserControl>(AccountTab.GetSingeltonObjectGrowFollowersTab)
                //},
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyGrowFollowers") != null
                        ? Application.Current.FindResource("LangKeyGrowFollowers").ToString()
                        : "Grow Followers",
                    Content = new Lazy<UserControl>(GrowFollowersTab.GetSingeltonObjectGrowFollowersTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyInstaposter") != null
                        ? Application.Current.FindResource("LangKeyInstaposter").ToString()
                        : "Insta Poster",
                    Content = new Lazy<UserControl>(InstaPosterTab.GetSingeltonObjectInstaPosterTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyInstachat") != null
                        ? Application.Current.FindResource("LangKeyInstachat").ToString()
                        : "Insta Chat",
                    Content = new Lazy<UserControl>(InstachatTab.GetSingeltonObjectInstachatTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyInstaEngage") != null
                        ? Application.Current.FindResource("LangKeyInstaEngage").ToString()
                        : "Insta Engage",
                    Content = new Lazy<UserControl>(InstaLikerInstaCommenterTab
                        .GetSingeltonObjectInstaLikerInstaCommenterTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyInstascrape") != null
                        ? Application.Current.FindResource("LangKeyInstascrape").ToString()
                        : "Insta Scrape",
                    Content = new Lazy<UserControl>(InstaScrapeTab.GetSingeltonObjectInstaScrapeTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyStory") != null
                        ? Application.Current.FindResource("LangKeyStory").ToString()
                        : "Insta Story",
                    Content = new Lazy<UserControl>(InstaStoryTab.objSingaltonStoryViewerTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyCampaigns") != null
                        ? Application.Current.FindResource("LangKeyCampaigns").ToString()
                        : "Campaigns",
                    Content = new Lazy<UserControl>(CampaignTab.GetSingeltonObjectCampaignTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeySettings") != null
                        ? Application.Current.FindResource("LangKeySettings").ToString()
                        : "Settings",
                    Content = new Lazy<UserControl>(() => new SettingsTab())
                },
                //new TabItemTemplates
                //{
                //    Title = Application.Current.FindResource("LangKeyLivechat")?.ToString(),
                //    Content = new Lazy<UserControl>(() => new LiveChat(SocialNetworks.Instagram))
                //    // Content=new Lazy<UserControl>(()=>new LiveChat(UpdateAccountChatList,UpdateCurrentThreadChatList,SendMessageToUser))
                //}
            };
        }
    }
}