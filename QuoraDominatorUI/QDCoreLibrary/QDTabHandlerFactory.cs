using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.CustomControl;
using QuoraDominator.TabManager;
using QuoraDominatorUI.TabManager;
using QuoraDominatorUI.Utility;

namespace QuoraDominatorUI.QDCoreLibrary
{
    public class QdTabHandlerFactory : ITabHandlerFactory
    {
        private static QdTabHandlerFactory _instance;

        private QdTabHandlerFactory()
        {
            TabInitializer();
            NetworkName = $"{SocialNetworks.Quora.ToString()}  Dominator";

            UserFilterAction.UserFilterControl = GlobalMethods.ShowUserFilterControl;
        }

        public string NetworkName { get; set; }

        public List<TabItemTemplates> NetworkTabs { get; set; }

        public List<TabItemTemplates> HelpSectionTabs { get; set; }
        public void UpdateAccountCustomControl(SocialNetworks networks)
        {
            NetworkTabs[0].Content = new Lazy<UserControl>(() => new AccountTab());
        }

        public static QdTabHandlerFactory Instance()
        {
            return _instance ?? (_instance = new QdTabHandlerFactory());
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
                    Title = Application.Current.FindResource("LangKeyGrowFollowers") == null
                        ? "Grow Followers"
                        : Application.Current.FindResource("LangKeyGrowFollowers")?.ToString(),
                    Content = new Lazy<UserControl>(GrowFollowersTab.GetSingletonObjectGrowFollowersTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyVoting") == null
                        ? "Voting"
                        : Application.Current.FindResource("LangKeyVoting")?.ToString(),
                    Content = new Lazy<UserControl>(VotingTab.GetSingletonObjectVotingTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyReport") == null
                        ? "Report"
                        : Application.Current.FindResource("LangKeyReport")?.ToString(),
                    Content = new Lazy<UserControl>(ReportTab.GetSingletonObjectReportTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyScrape") == null
                        ? "Scrape"
                        : Application.Current.FindResource("LangKeyScrape")?.ToString(),
                    Content = new Lazy<UserControl>(ScrapeTab.GetSingletonObjectScrapeTab)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyMessages") == null
                        ? "Messages"
                        : Application.Current.FindResource("LangKeyMessages")?.ToString(),
                    Content = new Lazy<UserControl>(MessagesTab.GetSingeltonObjectMessagesTab)
                },
                new TabItemTemplates
                {
                    Title = "LangKeyAnswers".FromResourceDictionary(),
                    Content = new Lazy<UserControl>(AnswersTab.GetSingeltonObjectAnswersTab)
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
                },
                //new TabItemTemplates
                //{
                //    Title = Application.Current.FindResource("LangKeyLivechat")?.ToString(),
                //    Content = new Lazy<UserControl>(() => new LiveChat(SocialNetworks.Quora))
                //}
            };
        }
    }
}