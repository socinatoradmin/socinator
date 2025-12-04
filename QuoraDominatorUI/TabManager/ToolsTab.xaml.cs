using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using QuoraDominatorCore.ViewModel;
using QuoraDominatorUI.QDViews;
using QuoraDominatorUI.QDViews.Activity;

namespace QuoraDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for ToolsTab.xaml
    /// </summary>
    public partial class ToolsTab
    {
        public ToolsTab()
        {
            InitializeComponent();
            InitializeTabControls();
        }

        private static ToolsTab ObjToolTabs { get; set; }

        public static ToolsTab GetSingletonToolTabs()
        {
            return ObjToolTabs ?? (ObjToolTabs = new ToolsTab());
        }

        private void InitializeTabControls()
        {
            try
            {
                var tabItems = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = Application.Current.TryFindResource("LangKeyFollow")?.ToString(),
                        Content = new Lazy<UserControl>(() => new FollowTab())
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.TryFindResource("LangKeyUnfollow")?.ToString(),
                        Content = new Lazy<UserControl>(() => new UnfollowTab())
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.TryFindResource("LangKeyReportUser")?.ToString(),
                        Content = new Lazy<UserControl>(() => new ReportUserTab())
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.TryFindResource("LangKeyReportAnswer")?.ToString(),
                        Content = new Lazy<UserControl>(() => new ReportAnswerTab())
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.TryFindResource("LangKeyUpvoteAnswers")?.ToString(),
                        Content = new Lazy<UserControl>(() => new UpvoteAnswersTab())
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.TryFindResource("LangKeyDownvoteAnswers")?.ToString(),
                        Content = new Lazy<UserControl>(() => new DownvoteAnswersTab())
                    },

                    new TabItemTemplates
                    {
                        Title = Application.Current.TryFindResource("LangKeyDownvoteQuestions")?.ToString(),
                        Content = new Lazy<UserControl>(() => new DownvoteQuestionsTab())
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.TryFindResource("LangKeyBroadcastMessages")?.ToString(),
                        Content = new Lazy<UserControl>(() => new BroadcastMessageTab())
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.TryFindResource("LangKeyAutoReplyToNewMessage")?.ToString(),
                        Content = new Lazy<UserControl>(() => new AutoReplyToNewMessageTab())
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.TryFindResource("LangKeySendMessageToNewFollowers")?.ToString(),
                        Content = new Lazy<UserControl>(() => new SendMessageToNewFollowerTab())
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.TryFindResource("LangKeyUserScraper")?.ToString(),
                        Content = new Lazy<UserControl>(() => new UserScraperConfigurationTab())
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.TryFindResource("LangKeyQuestionScraper")?.ToString(),
                        Content = new Lazy<UserControl>(() => new QuestionScraperConfigurationTab())
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.TryFindResource("LangKeyAnswerScraper")?.ToString(),
                        Content = new Lazy<UserControl>(() => new AnswerScraperConfigurationTab())
                    },
                    new TabItemTemplates
                    {
                        Title = Application.Current.TryFindResource("LangKeyAnswerOnQuestion")?.ToString(),
                        Content = new Lazy<UserControl>(() => new AnswerOnQuestionTab())
                    },
                    new TabItemTemplates
                    {
                        Title = "LangKeyPrivateBlacklistUsers".FromResourceDictionary(),
                        Content = new Lazy<UserControl>(() =>
                            new PrivateBlacklistUserControl(new PrivateBlackListViewModel(SocialNetworks.Quora)))
                    },
                    new TabItemTemplates
                    {
                        Title = "LangKeyPrivateWhitelistUsers".FromResourceDictionary(),
                        Content = new Lazy<UserControl>(() =>
                            new PrivateWhitelistUserControl(new PrivateWhiteListViewModel(SocialNetworks.Quora)))
                    }
                };
                ToolTab.ItemsSource = tabItems;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}