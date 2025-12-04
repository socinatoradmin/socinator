using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using LinkedDominatorUI.CustomControl;
using LinkedDominatorUI.LDViews.Tools.Scraper;

namespace LinkedDominatorUI.LDViews.Tools.Tabs.Scraper
{
    /// <summary>
    ///     Interaction logic for MessageConversationScraperTab.xaml
    /// </summary>
    public partial class MessageConversationScraperTab : UserControl
    {
        private static MessageConversationScraperTab _objMessageConversationScraperTab;

        public MessageConversationScraperTab()
        {
            try
            {
                InitializeComponent();
                var tabItems = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConfiguration").ToString(),
                        Content = new Lazy<UserControl>(() =>
                            MessageConversationScraperConfiguration
                                .GetSingeltonObjectMessageConversationScraperConfiguration())
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyReports").ToString(),
                        Content = new Lazy<UserControl>(() => new AccountReport(ActivityType.AttachmnetsMessageScraper))
                    }
                };
                MessageConversationScraperTabs.ItemsSource = tabItems;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        public static MessageConversationScraperTab GetSingletonObjectMessageConversationScraperTab()
        {
            return _objMessageConversationScraperTab ??
                   (_objMessageConversationScraperTab = new MessageConversationScraperTab());
        }
    }
}