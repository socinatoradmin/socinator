using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;

namespace PinDominator.TabManager
{
    /// <summary>
    ///     Interaction logic for CampaignTab.xaml
    /// </summary>
    public partial class CampaignTab
    {
        public CampaignTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyAllCampaigns") == null
                        ? "Campaigns"
                        : Application.Current.FindResource("LangKeyAllCampaigns")?.ToString(),
                    Content = new Lazy<UserControl>(() => new Campaigns(SocialNetworks.Pinterest))
                }
            };
            CampaignTabs.ItemsSource = tabItems;
            //ReportManager.GetSavedQuery = ReportHelper.GetSavedQuery;
            //ReportManager.GetReportDetail = ReportHelper.GetReportDetail;
            //ReportManager.ExportReports = ReportHelper.ExportReports;
            //ReportManager.GetHeader = ReportHelper.GetHeader;
            //Campaigns.EditOrDuplicateCampaign = CampaignHelper.ManageCampaign;
        }
    }
}