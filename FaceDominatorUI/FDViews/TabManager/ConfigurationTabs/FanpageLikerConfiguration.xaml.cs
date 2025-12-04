using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.FanpageLiker;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for FanpageLikerConfiguration.xaml
    /// </summary>
    public partial class FanpageLikerConfiguration
    {
        public FanpageLikerConfiguration()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyConfiguration").ToString(),

                    Content = new Lazy<UserControl>(FanpageLikerTools.GetSingeltonObjectFanpageLikerTools)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.FanpageLiker))
                }
            };
            SendRequesttabs.ItemsSource = tabItems;
        }

        //        private static FanpageLikerConfiguration _currentSendRequestConfigurationTab;
        //
        //        public static FanpageLikerConfiguration GetSingeltonObjectSendRequestConfigurationTab()
        //        {
        //            return _currentSendRequestConfigurationTab ?? (_currentSendRequestConfigurationTab = new FanpageLikerConfiguration());
        //        }
    }
}