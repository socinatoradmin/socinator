using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TumblrDominatorUI.TabManager;

namespace TumblrDominatorUI.TumblrCoreLibrary
{
    public class TumblrTabHandlerFactory : ITabHandlerFactory
    {
        private static TumblrTabHandlerFactory _instance;
        private AccessorStrategies _strategies;

        private TumblrTabHandlerFactory(AccessorStrategies strategies)
        {
            _strategies = strategies;
            TabInitializer(strategies);
            NetworkName = $"{SocialNetworks.Tumblr.ToString()}  Dominator";
        }

        public string NetworkName { get; set; }


        public List<TabItemTemplates> NetworkTabs { get; set; }
        public List<TabItemTemplates> HelpSectionTabs { get; set; }

        public void UpdateAccountCustomControl(SocialNetworks networks)
        {
            NetworkTabs[0].Content = new Lazy<UserControl>(() => new AccountTab(_strategies));
        }

        public static TumblrTabHandlerFactory Instance(AccessorStrategies strategies)
        {
            return _instance ?? (_instance = new TumblrTabHandlerFactory(strategies));
        }

        private void TabInitializer(AccessorStrategies strategies)
        {
            try
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
                        Content = new Lazy<UserControl>(GrowFollowersTab.GetSingletonObjectGrowFollowersTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyEngage") == null
                            ? "Engage"
                            : Application.Current.FindResource("LangKeyEngage")?.ToString(),
                        Content = new Lazy<UserControl>(EngageTab.GetSingeltonObjectEngageTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyReblog") == null
                            ? "Reblog"
                            : Application.Current.FindResource("LangKeyReblog")?.ToString(),
                        Content = new Lazy<UserControl>(BlogTab.GetSingeltonObjectBlogTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyBroadcastMessages") == null
                            ? "BroadCast Message"
                            : Application.Current.FindResource("LangKeyBroadcastMessages")?.ToString(),
                        Content = new Lazy<UserControl>(MessagesTab.GetSingeltonObjectMsgTab)
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
                        Content = new Lazy<UserControl>(CampaignsTab.GetSingletonObjectCampaignsTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeySettings") == null
                            ? "Settings"
                            : Application.Current.FindResource("LangKeySettings")?.ToString(),
                        Content = new Lazy<UserControl>(SettingsTab.GetSingeltonSettingsTab)
                    }
                };
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}