using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using RedditDominatorUI.TabManager;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RedditDominatorUI.RdCoreLibrary
{
    public class RdTabHandlerFactory : ITabHandlerFactory
    {
        private static RdTabHandlerFactory _instance;
        private readonly AccessorStrategies _strategies;
        private readonly IGenericFileManager genericFileManager;
        private RdTabHandlerFactory(AccessorStrategies strategies)
        {
            genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            _strategies = strategies;
            TabInitializer(strategies);
            NetworkName = $"{SocialNetworks.Reddit.ToString()}  Dominator";
        }

        public string NetworkName { get; set; }

        public List<TabItemTemplates> NetworkTabs { get; set; }
        public List<TabItemTemplates> HelpSectionTabs { get; set; }

        public void UpdateAccountCustomControl(SocialNetworks networks)
        {
            NetworkTabs[0].Content = new Lazy<UserControl>(() => new AccountTab(_strategies));
        }

        public static RdTabHandlerFactory Instance(AccessorStrategies strategies)
        {
            return _instance ?? (_instance = new RdTabHandlerFactory(strategies));
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
                        Title = "Grow Followers",
                        Content = new Lazy<UserControl>(GrowFollowerTab.GetSingeltonObject_GrowFollowerTab)
                    },
                    new TabItemTemplates
                    {
                        Title = "Subscribe Channels",
                        Content = new Lazy<UserControl>(SubscribeChannelsTab.GetSingeltonObject_SubscribeChannelsTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyEngage")?.ToString(),
                        Content = new Lazy<UserControl>(EngageTab.GetSingeltonObject_EngageTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyVoter")?.ToString(),
                        Content = new Lazy<UserControl>(VotingTab.GetSingletonObjectVotingTab)
                    },
                     new TabItemTemplates
                    {
                        Title= Application.Current.FindResource("LangKeyMessenger")?.ToString(),
                        Content=new Lazy<UserControl>(MessangerTab.GetSingeltonObject_MessangerTab)
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyScraper")?.ToString(),
                        Content = new Lazy<UserControl>(() => new ScraperTab())
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyCampaigns")?.ToString(),
                        Content = new Lazy<UserControl>(() => new CampaignTab())
                    },

                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeySettings")?.ToString(),
                        Content = new Lazy<UserControl>(() => new SettingsTab())
                    }
                };
                if (genericFileManager != null)
                {
                    var otherConfigModel = genericFileManager.GetModel<RedditOtherConfigModel>(ConstantVariable.GetOtherRedditSettingsFile()) ??
                        new RedditOtherConfigModel();
                    if (otherConfigModel != null && otherConfigModel.IsEnableFeedActivity)
                    {
                        NetworkTabs.Insert(7, new TabItemTemplates
                        {
                            Title = Application.Current.FindResource("LangKeyRedditFeed")?.ToString(),
                            Content = new Lazy<UserControl>(RedditFeed.GetSingletonInstance)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}