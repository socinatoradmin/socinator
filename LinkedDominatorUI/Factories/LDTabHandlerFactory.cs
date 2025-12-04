using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using LinkedDominatorUI.TabManager;

namespace LinkedDominatorUI.Factories
{
    public class LDTabHandlerFactory : ITabHandlerFactory
    {
        private static LDTabHandlerFactory _instance;

        private readonly AccessorStrategies _strategies;

        private LDTabHandlerFactory(AccessorStrategies strategies)
        {
            _strategies = strategies;
            TabInitializer(strategies);
            NetworkName = $"{SocialNetworks.LinkedIn.ToString()}  Dominator";
        }

        public string NetworkName { get; set; }

        public List<TabItemTemplates> NetworkTabs { get; set; }
        public List<TabItemTemplates> HelpSectionTabs { get; set; }

        public void UpdateAccountCustomControl(SocialNetworks networks)
        {
            NetworkTabs[0].Content = new Lazy<UserControl>(() => new AccountTab(_strategies));
        }

        public static LDTabHandlerFactory Instance(AccessorStrategies strategies)
        {
            return _instance ?? (_instance = new LDTabHandlerFactory(strategies));
        }

        private void TabInitializer(AccessorStrategies strategies)
        {
            try
            {
                NetworkTabs = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyAccounts") == null
                            ? "Accounts"
                            : Application.Current.FindResource("LangKeyAccounts")?.ToString(),
                        Content = new Lazy<UserControl>(() => new AccountTab(strategies))
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyGrowConnection").ToString(),
                        //Title=Application.Current.FindResource("LangKeyGrowFollowers﻿")== null
                        //    ? "Grow Followers"
                        //    : Application.Current.FindResource("LangKeyGrowFollowers")?.ToString(),
                        Content = new Lazy<UserControl>(GrowConnectionTab.GetSingeltonObjectobGrowConnectionTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyMessenger").ToString(),
                        Content = new Lazy<UserControl>(MessengerTab.GetSingeltonObjectMessengerTab)
                    },
                    //new TabItemTemplates
                    //{
                    //    Title=Application.Current.FindResource("LangKeyPoster").ToString(),
                    //    Content =new Lazy<UserControl>(PosterTab.GetSingeltonObjectPosterTab)
                    //},
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyEngage").ToString(),
                        Content = new Lazy<UserControl>(EngageTab.GetSingeltonObjectEngageTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyGroup").ToString(),
                        Content = new Lazy<UserControl>(GroupTab.GetSingeltonObjectGroupTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyScraper").ToString(),
                        Content = new Lazy<UserControl>(ScraperTab.GetSingeltonObjectScraperTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyProfileEndorsement").ToString(),
                        Content = new Lazy<UserControl>(ProfillingTab.GetSingeltonObjectProfillingTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeySalesnavigatorScraper").ToString(),
                        Content = new Lazy<UserControl>(SalesNavigatorScraperTab.GetSingeltonObjectScraperTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyCampaigns").ToString(),
                        Content = new Lazy<UserControl>(CampaignTab.GetSingeltonObjectCampaignTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeySettings").ToString(),
                        Content = new Lazy<UserControl>(SettingsTab.GetSingeltonObjectSettingsTab)
                    },
                    //new TabItemTemplates
                    //{
                    //   Title =Application.Current.FindResource("LangKeyLivechat") == null
                    //       ? "LiveChat"
                    //       : Application.Current.FindResource("LangKeyLivechat")?.ToString(),
                    //   Content=new Lazy<UserControl>(()=>new DominatorUIUtility.CustomControl.LiveChat(SocialNetworks.LinkedIn))
                    //},
                };
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }
    }
}