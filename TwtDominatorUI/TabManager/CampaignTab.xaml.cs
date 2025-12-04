using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;

namespace TwtDominatorUI.TabManager
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

            #region Initilize Actions

            // DominatorUIUtility.CustomControl.Campaigns.EditOrDuplicateCampaign = Utility.CampaignHelper.ManageCampaign;
            //  DominatorUIUtility.Behaviours.ReportManager.ExportReports += Utility.ReportHelper.ExportReports;
            // DominatorUIUtility.Behaviours.ReportManager.FilterByQueryType += Utility.ReportHelper.FilterByQueryType;

            #endregion

            #region initialize functions

            //  DominatorUIUtility.Behaviours.ReportManager.GetSavedQuery += TwtDominatorUI.Utility.ReportHelper.GetSavedQuery;
            //  DominatorUIUtility.Behaviours.ReportManager.GetReportDetail += Utility.ReportHelper.GetReportDetail;
            //  DominatorUIUtility.Behaviours.ReportManager.GetHeader += Utility.ReportHelper.GetHeader;

            #endregion

            string TitleCampaign = null;

            try
            {
                TitleCampaign = Application.Current.FindResource("LangKeyAllCampaigns").ToString();
            }
            catch
            {
            }

            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = TitleCampaign == null ? "Campaign" : TitleCampaign,
                    Content = new Lazy<UserControl>(() => new Campaigns(SocialNetworks.Twitter))
                }
            };

            CampaignTabs.ItemsSource = tabItems;
        }

        public static CampaignTab GetSingeltonObjectCampaignTab()
        {
            if (objCampaignTab == null)
                objCampaignTab = new CampaignTab();
            return objCampaignTab;
        }

        public void SetIndex(int index)
        {
            CampaignTabs.SelectedIndex = index;
        }
    }
}