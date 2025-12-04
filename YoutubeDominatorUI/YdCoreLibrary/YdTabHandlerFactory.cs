using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using YoutubeDominatorUI.TabManager;

namespace YoutubeDominatorUI.YdCoreLibrary
{
    public class YdTabHandlerFactory : ITabHandlerFactory
    {
        private static YdTabHandlerFactory _instance;
        private AccessorStrategies _strategies;

        private YdTabHandlerFactory(AccessorStrategies strategies)
        {
            TabInitializer(strategies);
            NetworkName = $"{SocialNetworks.YouTube.ToString()}  Dominator";
            // UserFilterAction.UserFilterControl = GlobalMethods.ShowUserFilterControl;
        }

        public string NetworkName { get; set; }

        public List<TabItemTemplates> NetworkTabs { get; set; }
        public List<TabItemTemplates> HelpSectionTabs { get; set; }
        public void UpdateAccountCustomControl(SocialNetworks networks)
        {
            NetworkTabs[0].Content = new Lazy<UserControl>(() => new AccountTab(_strategies));
        }

        public static YdTabHandlerFactory Instance(AccessorStrategies strategies)
        {
            return _instance ?? (_instance = new YdTabHandlerFactory(strategies));
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
                    Title = "Engage",
                    //Title=Application.Current.FindResource("LangKeyGrowFollowers?")== null
                    //    ? "Grow Followers"
                    //    : Application.Current.FindResource("LangKeyGrowFollowers")?.ToString(),
                    Content = new Lazy<UserControl>(EngageTab.GetSingeltonObject_EngageTab)
                },
                new TabItemTemplates
                {
                    Title = "Subscriber",
                    //Title=Application.Current.FindResource("langTwtBlaster?")== null
                    //    ? "Twitter Blaster"
                    //    : Application.Current.FindResource("langTwtBlaster")?.ToString(),
                    Content = new Lazy<UserControl>(GrowSubscribersTab.GetSingeltonObject_GrowSubscribersTab)
                },
                new TabItemTemplates
                {
                    Title = "Scraper",
                    //Title=Application.Current.FindResource("langTwtEngage?")== null
                    //    ? "Twitter Engage"
                    //    : Application.Current.FindResource("langTwtEngage")?.ToString(),
                    Content = new Lazy<UserControl>(ScraperTab.GetSingeltonObject_ScraperTab)
                },
                new TabItemTemplates
                {
                    Title = "WatchVideo",
                    //Title=Application.Current.FindResource("langTwtEngage?")== null
                    //    ? "Twitter Engage"
                    //    : Application.Current.FindResource("langTwtEngage")?.ToString(),
                    Content = new Lazy<UserControl>(WatchVideoTab.GetSingeltonObject_WatchVideoTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyCampaigns").ToString(),
                    //Title=Application.Current.FindResource("langScraper")== null
                    //    ? "Scraper"
                    //    : Application.Current.FindResource("langScraper")?.ToString(),
                    Content = new Lazy<UserControl>(CampaignTab.GetSingeltonObject_CampaignTab)
                },
                new TabItemTemplates
                {
                    Title = "Settings",
                    //Title=Application.Current.FindResource("langMessenger")== null
                    //    ? "Messenger"
                    //    : Application.Current.FindResource("langMessenger")?.ToString(),
                    Content = new Lazy<UserControl>(SettingsTab.GetSingeltonObject_SettingsTab)
                }
            };
        }
    }
}