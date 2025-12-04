using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.GroupJoiner;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for GroupJoinerTabs.xaml
    /// </summary>
    public partial class GroupJoinerTabs
    {
        public GroupJoinerTabs()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(GroupJoinerTools.GetSingeltonObjectGroupJoinerTools)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.GroupJoiner))
                }
            };
            SendRequesttabs.ItemsSource = tabItems;
        }

        //        private static GroupJoinerTabs _currentSendRequestConfigurationTab;
        //
        //        public static GroupJoinerTabs GetSingeltonObjectSendRequestConfigurationTab()
        //        {
        //            return _currentSendRequestConfigurationTab ?? (_currentSendRequestConfigurationTab = new GroupJoinerTabs());
        //        }
    }
}