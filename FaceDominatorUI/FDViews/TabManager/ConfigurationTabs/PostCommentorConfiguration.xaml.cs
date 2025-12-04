using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.PostCommentor;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for PostCommentorConfiguration.xaml
    /// </summary>
    public partial class PostCommentorConfiguration
    {
        public PostCommentorConfiguration()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(PostCommentorTools.GetSingeltonObjectPostCommentorTools)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.PostCommentor))
                }
            };
            SendRequesttabs.ItemsSource = tabItems;
        }

        //        private static PostCommentorConfiguration _currentSendRequestConfigurationTab;
        //
        //        public static PostCommentorConfiguration GetSingeltonObjectSendRequestConfigurationTab()
        //        {
        //            return _currentSendRequestConfigurationTab ?? (_currentSendRequestConfigurationTab = new PostCommentorConfiguration());
        //        }
    }
}