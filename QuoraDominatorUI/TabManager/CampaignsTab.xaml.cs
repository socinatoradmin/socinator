using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;

namespace QuoraDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for CampaignsTab.xaml
    /// </summary>
    public partial class CampaignsTab
    {
        private static CampaignsTab _objCampaignsTab;

        public CampaignsTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.TryFindResource("LangKeyAllCampaigns").ToString(),
                    Content = new Lazy<UserControl>(() => new Campaigns(SocialNetworks.Quora))
                }
            };
            CampaignTabs.ItemsSource = tabItems;
        }

        public static CampaignsTab GetSingletonObjectCampaignsTab()
        {
            return _objCampaignsTab ?? (_objCampaignsTab = new CampaignsTab());
        }
    }
}