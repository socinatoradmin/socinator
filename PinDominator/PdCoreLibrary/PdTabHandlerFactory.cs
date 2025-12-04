using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using PinDominator.TabManager;

namespace PinDominator.PdCoreLibrary
{
    public class PdTabHandlerFactory : ITabHandlerFactory
    {
        private static PdTabHandlerFactory _instance;
        private AccessorStrategies _strategies;

        private PdTabHandlerFactory(AccessorStrategies strategies)
        {
            TabInitializer(strategies);
            NetworkName = $"{SocialNetworks.Pinterest.ToString()}  Dominator";
        }

        public string NetworkName { get; set; }

        public List<TabItemTemplates> NetworkTabs { get; set; }
        public List<TabItemTemplates> HelpSectionTabs { get; set; }

        public void UpdateAccountCustomControl(SocialNetworks networks)
        {
            NetworkTabs[0].Content = new Lazy<UserControl>(() => new AccountTab());
        }

        public static PdTabHandlerFactory Instance(AccessorStrategies strategies)
        {
            return _instance ?? (_instance = new PdTabHandlerFactory(strategies));
        }

        private void TabInitializer(AccessorStrategies strategies)
        {
            _strategies = strategies;
            try
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
                        Title = Application.Current.FindResource("LangKeyBoards") == null
                            ? "Boards"
                            : Application.Current.FindResource("LangKeyBoards")?.ToString(),
                        Content = new Lazy<UserControl>(BoardsTab.GetSingletonObjectCreateBoardTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyGrowFollowers") == null
                            ? "Create Boards"
                            : Application.Current.FindResource("LangKeyGrowFollowers")?.ToString(),
                        Content = new Lazy<UserControl>(GrowFollowersTab.GetSingeltonObjectGrowFollowersTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyPinPoster") == null
                            ? "Pin Poster"
                            : Application.Current.FindResource("LangKeyPinPoster").ToString(),
                        Content = new Lazy<UserControl>(PinPosterTab.GetSingeltonObjectPinPosterTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyPinCommenter") == null
                            ? "Pin Commentor"
                            : Application.Current.FindResource("LangKeyPinCommenter")?.ToString(),
                        Content = new Lazy<UserControl>(PinTryPinCommenterTab.GetSingeltonObjectPinTryPinCommenterTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyPinMessenger") == null
                            ? "Messenger"
                            : Application.Current.FindResource("LangKeyPinMessenger")?.ToString(),
                        Content = new Lazy<UserControl>(PinMessengerTab.GetSingeltonObjectPinMessengerTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyScraper") == null
                            ? "Scraper"
                            : Application.Current.FindResource("LangKeyScraper")?.ToString(),
                        Content = new Lazy<UserControl>(ScraperTab.GetSingeltonObjectPinScrapeTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyCampaigns") == null
                            ? "Campaigns"
                            : Application.Current.FindResource("LangKeyCampaigns")?.ToString(),
                        Content = new Lazy<UserControl>(() => new CampaignTab())
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeySettings") == null
                            ? "Settings"
                            : Application.Current.FindResource("LangKeySettings")?.ToString(),
                        Content = new Lazy<UserControl>(() => new SettingsTab())
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