using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FaceDominatorUI.CustomControl;
using FaceDominatorUI.FDViews.Tools.DownloadMedia;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager.ConfigurationTabs
{
    /// <summary>
    ///     Interaction logic for DOwnloadMediaConfiguration.xaml
    /// </summary>
    public partial class DOwnloadMediaConfiguration
    {
        public DOwnloadMediaConfiguration()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyDownloadMedia").ToString(),

                    Content = new Lazy<UserControl>(() => new DownloadMediaTools())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyReports").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountReports(ActivityType.DownloadScraper))
                }
            };
            SendRequesttabs.ItemsSource = tabItems;
        }

        //        private static DOwnloadMediaConfiguration _currentSendRequestConfigurationTab;
        //
        //        public static DOwnloadMediaConfiguration GetSingeltonObjectSendRequestConfigurationTab()
        //        {
        //            return _currentSendRequestConfigurationTab ?? (_currentSendRequestConfigurationTab = new DOwnloadMediaConfiguration());
        //        }
    }
}