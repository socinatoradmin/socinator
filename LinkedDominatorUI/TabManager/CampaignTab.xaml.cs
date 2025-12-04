using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;

namespace LinkedDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for CampaignTab.xaml
    /// </summary>
    public partial class CampaignTab : UserControl
    {
        private static CampaignTab ojCampaignTab;

        public CampaignTab()
        {
            try
            {
                InitializeComponent();
                try
                {
                    #region Initilize Actions                

                    //Campaigns.EditOrDuplicateCampaign = CampaignHelper.ManageCampaign;
                    //ReportManager.ExportReports = ReportHelper.ExportReports;
                    //ReportManager.FilterByQueryType = ReportHelper.FilterByQueryType;
                    //UserFilterAction.UserFilterControl = GlobalMethods.ShowUserFilterControl;

                    #endregion

                    #region Initilize Functions

                    //ReportManager.GetSavedQuery = ReportHelper.GetSavedQuery;
                    //ReportManager.GetReportDetail = ReportHelper.GetReportDetail;
                    //ReportManager.GetHeader = ReportHelper.GetHeader;

                    #endregion
                }
                catch (Exception Ex)
                {
                    Ex.ErrorLog();
                }

                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyCampaign").ToString(),
                        Content = new Lazy<UserControl>(() => new Campaigns(SocialNetworks.LinkedIn))
                    }
                };
                CampaignsTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static CampaignTab GetSingeltonObjectCampaignTab()
        {
            return ojCampaignTab ?? (ojCampaignTab = new CampaignTab());
        }

        public void SetIndex(int index)
        {
            //CampaignsTabs is the name of this Tab
            CampaignsTabs.SelectedIndex = index;
        }
    }
}