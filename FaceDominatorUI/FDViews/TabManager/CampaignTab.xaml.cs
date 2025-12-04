using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FaceDominatorUI.FDViews.TabManager
{
    /// <summary>
    ///     Interaction logic for CampaignTab.xaml
    /// </summary>
    public partial class CampaignTab
    {
        public CampaignTab()
        {
            InitializeComponent();
            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyCampaign").ToString(),
                    Content = new Lazy<UserControl>(() => new Campaigns(SocialNetworks.Facebook))
                }
            };

            CampaignTabs.ItemsSource = items;

            //IViewCampaignsFactory CampaignsFactory = SocinatorInitialize
            //       .GetSocialLibrary(SocialNetworks.Facebook).GetNetworkCoreFactory()
            //        .ViewCampaigns;

            //IReportFactory reportFactory = SocinatorInitialize
            //       .GetSocialLibrary(SocialNetworks.Facebook).GetNetworkCoreFactory()
            //        .ReportFactory;


            //ReportManager.ExportReports = reportFactory.ExportReports;


            //Campaigns.EditOrDuplicateCampaign = CampaignsFactory.ViewCampaigns;
        }

        private static CampaignTab CurrentCampaignTab { get; set; }

        public static CampaignTab GetSingeltonObjectCampaignTab()
        {
            return CurrentCampaignTab ?? (CurrentCampaignTab = new CampaignTab());
        }

        internal void SetIndex(int tabIndex)
        {
            CampaignTabs.SelectedIndex = tabIndex;
        }
    }
}