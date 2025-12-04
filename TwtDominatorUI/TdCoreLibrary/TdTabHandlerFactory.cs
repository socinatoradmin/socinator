using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using TwtDominatorUI.TabManager;

namespace TwtDominatorUI.TdCoreLibrary
{
    public class TdTabHandlerFactory : ITabHandlerFactory
    {
        private static TdTabHandlerFactory _instance;
        private AccessorStrategies _strategies;

        private TdTabHandlerFactory(AccessorStrategies strategies)
        {
            TabInitializer(strategies);
            NetworkName = $"{SocialNetworks.Twitter.ToString()}  Dominator";
        }

        public string NetworkName { get; set; }

        public List<TabItemTemplates> NetworkTabs { get; set; }
        public List<TabItemTemplates> HelpSectionTabs { get; set; }

        public void UpdateAccountCustomControl(SocialNetworks networks)
        {
            NetworkTabs[0].Content = new Lazy<UserControl>(() => new AccountTab(_strategies));
        }

        public static TdTabHandlerFactory Instance(AccessorStrategies strategies)
        {
            return _instance ?? (_instance = new TdTabHandlerFactory(strategies));
        }

        private void TabInitializer(AccessorStrategies strategies)
        {
            _strategies = strategies;
            NetworkTabs = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyAccounts") == null
                        ? "Accounts"
                        : Application.Current.FindResource("LangKeyAccounts")?.ToString(),
                    Content = new Lazy<UserControl>(() => new AccountTab(_strategies))
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyGrowFollowers") == null
                        ? "Grow Followers"
                        : Application.Current.FindResource("LangKeyGrowFollowers")?.ToString(),
                    Content = new Lazy<UserControl>(GrowFollowersTab.GetSingeltonObjectGrowFollowersTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyPoster") == null
                        ? "Poster"
                        : Application.Current.FindResource("LangKeyPoster")?.ToString(),
                    Content = new Lazy<UserControl>(TwtBlasterTab.GetSingeltonObjectTwtBlasterTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyEngage") == null
                        ? "Engage"
                        : Application.Current.FindResource("LangKeyEngage")?.ToString(),
                    Content = new Lazy<UserControl>(TwtEngageTab.GetSingeltonObjectTwtEngageTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyMessenger") == null
                        ? "Messenger"
                        : Application.Current.FindResource("LangKeyMessenger")?.ToString(),
                    Content = new Lazy<UserControl>(TwtMessengerTab.GetSingeltonObjectTwtMessengerTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyScraper") == null
                        ? "Scraper"
                        : Application.Current.FindResource("LangKeyScraper")?.ToString(),
                    Content = new Lazy<UserControl>(ScraperTab.GetSingeltonObjectScraperTab)
                },

                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyCampaigns") == null
                        ? "Campaigns"
                        : Application.Current.FindResource("LangKeyCampaigns")?.ToString(),
                    Content = new Lazy<UserControl>(() => CampaignTab.GetSingeltonObjectCampaignTab())
                },
                //new TabItemTemplates
                //{
                //    Title =Application.Current.FindResource("LangKeyLivechat") == null
                //        ? "LiveChat"
                //        : Application.Current.FindResource("LangKeyLivechat")?.ToString(),
                //    Content=new Lazy<UserControl>(()=>new DominatorUIUtility.CustomControl.LiveChat(SocialNetworks.Twitter))
                //},
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeySettings") == null
                        ? "Settings"
                        : Application.Current.FindResource("LangKeySettings")?.ToString(),
                    Content = new Lazy<UserControl>(() => new SettingsTab())
                }
            };
        }
    }
}