using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace YoutubeDominatorUI.TabManager
{
    public partial class CampaignTab
    {
        private static CampaignTab _objCampaignTab;

        public CampaignTab()
        {
            InitializeComponent();
            _objCampaignTab = this;

            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyAllCampaigns").ToString(),
                    Content = new Lazy<UserControl>(() => new Campaigns(SocialNetworks.YouTube))
                }
            };

            #region Initialize Actions

            //Campaigns.EditOrDuplicateCampaign = CampaignHelper.ManageCampaign;
            //ReportManager.ExportReports = ReportHelper.ExportReports;
            //  ReportManager.FilterByQueryType = ReportHelper.FilterByQueryType;
            // UserFilterAction.UserFilterControl = GlobalMethods.ShowUserFilterControl;

            #endregion


            CampaignTabs.ItemsSource = tabItems;
            //ReportManager.GetSavedQuery = ReportHelper.GetSavedQuery;
            // ReportManager.GetHeader = ReportHelper.GetHeader;
            //ReportManager.GetReportDetail = ReportHelper.GetReportDetail;
            //  return TabItems;
        }

        public static CampaignTab GetSingeltonObject_CampaignTab()
        {
            return _objCampaignTab ?? (_objCampaignTab = new CampaignTab());
        }

        public void SetIndex(int index)
        {
            CampaignTabs.SelectedIndex = index;
        }
    }
}