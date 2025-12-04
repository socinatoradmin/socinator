using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.PostLiker;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for PostLikerConfiguration.xaml
    /// </summary>
    public partial class PostLikerConfiguration
    {
        public PostLikerConfiguration()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(PostLikerTools.GetSingeltonObjectPostLikerCommentorTools)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.PostLiker))
                }
            };
            SendRequesttabs.ItemsSource = tabItems;
        }

        //        private static PostLikerConfiguration _currentSendRequestConfigurationTab;
        //
        //        public static PostLikerConfiguration GetSingeltonObjectSendRequestConfigurationTab()
        //        {
        //            return _currentSendRequestConfigurationTab ?? (_currentSendRequestConfigurationTab = new PostLikerConfiguration());
        //        }
    }
}