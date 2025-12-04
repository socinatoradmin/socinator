using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;

namespace GramDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for CampaignTab.xaml
    /// </summary>
    public partial class CampaignTab : UserControl
    {
        private static CampaignTab objCampaignTab;

        private CampaignTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyAllCampaigns") != null
                        ? Application.Current.FindResource("LangKeyAllCampaigns").ToString()
                        : "All Campaigns",
                    Content = new Lazy<UserControl>(() => new Campaigns(SocialNetworks.Instagram))
                }
            };
            CampaignTabs.ItemsSource = tabItems;

            #region Initialize Func

            // ReportManager.GetSavedQuery = ReportHelper.GetSavedQuery;
            // ReportManager.GetReportDetail = ReportHelper.GetReportDetail;
            // ReportManager.GetHeader = ReportHelper.GetHeader;
            // ReportManager.ExportReports = ReportHelper.ExportReports;

            #endregion
        }

        public static CampaignTab GetSingeltonObjectCampaignTab()
        {
            return objCampaignTab ?? (objCampaignTab = new CampaignTab());
        }

        public void setIndex(int index)
        {
            CampaignTabs.SelectedIndex = index;
        }
    }
}