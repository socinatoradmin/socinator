using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace TumblrDominatorUI.TabManager
{
    /// <inheritdoc cref="UserControl" />
    /// <summary>
    ///     Interaction logic for CampaignsTab.xaml
    /// </summary>
    public partial class CampaignsTab
    {
        private static CampaignsTab _objCampaignsTab;

        public CampaignsTab()
        {
            InitializeComponent();
            try
            {
                var tabItems = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = Application.Current.FindResource("LangKeyAllCampaigns") == null
                            ? "All Campaigns"
                            : Application.Current.FindResource("LangKeyAllCampaigns")?.ToString(),
                        Content = new Lazy<UserControl>(() =>
                            new Campaigns(SocialNetworks.Tumblr))
                    }
                };
                CampaignTabs.ItemsSource = tabItems;

                //IViewCampaignsFactory campaignsFactory = SocinatorInitialize
                //    .GetSocialLibrary(SocialNetworks.Tumblr)
                //    .GetNetworkCoreFactory()
                //    .ViewCampaigns;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static CampaignsTab GetSingletonObjectCampaignsTab()
        {
            return _objCampaignsTab ?? (_objCampaignsTab = new CampaignsTab());
        }
    }
}