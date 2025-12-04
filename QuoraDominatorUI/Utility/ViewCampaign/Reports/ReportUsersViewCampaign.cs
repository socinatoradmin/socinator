using System;
using System.Globalization;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using QuoraDominatorCore.Models;
using QuoraDominatorUI.QDViews.Reports;

namespace QuoraDominatorUI.Utility.ViewCampaign.Reports
{
    public class ReportUsersViewCampaign : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var reportUsers = ReportUsers.GetSingeltonObjectReportUsers();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                reportUsers.CampaignName =
                    $"{SocialNetworks.Quora} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            reportUsers.IsEditCampaignName = true;
            reportUsers.CancelEditVisibility = Visibility.Visible;
            reportUsers.TemplateId = campaignDetails.TemplateId;
            reportUsers.CampaignButtonContent = openCampaignType;
            reportUsers.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : reportUsers.CampaignName;

            reportUsers.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            reportUsers.ReportUsersFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            reportUsers.ObjViewModel.ReportUserModel =
                JsonConvert.DeserializeObject<ReportUserModel>(templateDetails.ActivitySettings);
            try
            {
                var Description = reportUsers.ObjViewModel.ReportUserModel.ReportDescription.ToString();
                reportUsers.ObjViewModel.ReportUserModel.SavedQueries.ForEach(query =>
                {
                    reportUsers.ObjViewModel.ReportUserModel.SelectedSubOption.ReportOptionTitle = reportUsers.ObjViewModel.ReportUserModel.SelectedOption;
                    reportUsers.ObjViewModel.ReportUserModel.SelectedSubOption.ReportDescription = Description;
                    query.CustomFilters = JsonConvert.SerializeObject(reportUsers.ObjViewModel.ReportUserModel.SelectedSubOption);
                });
            }
            catch (Exception ex) { }
            reportUsers.MainGrid.DataContext = reportUsers.ObjViewModel;
            TabSwitcher.ChangeTabIndex(3, 0);
        }
    }
}