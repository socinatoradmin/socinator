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
    public class ReportAnswersViewCampaign : IViewCampaignsFactory
    {
        public void ViewCampaigns(string campaignId, string openCampaignType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var campaignDetails = campaignFileManager.GetCampaignById(campaignId);
            var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
            var templateDetails = templatesFileManager.GetTemplateById(campaignDetails.TemplateId);
            var reportAnswers = ReportAnswers.GetSingeltonObjectReportAnswers();
            if (openCampaignType == ConstantVariable.CreateCampaign())
                reportAnswers.CampaignName =
                    $"{SocialNetworks.Quora} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            reportAnswers.IsEditCampaignName = true;
            reportAnswers.CancelEditVisibility = Visibility.Visible;
            reportAnswers.TemplateId = campaignDetails.TemplateId;
            reportAnswers.CampaignButtonContent = openCampaignType;
            reportAnswers.CampaignName = openCampaignType == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : reportAnswers.CampaignName;
            reportAnswers.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                 $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            reportAnswers.ReportAnswersFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;
            reportAnswers.ObjViewModel.ReportAnswerModel =
                JsonConvert.DeserializeObject<ReportAnswerModel>(templateDetails.ActivitySettings);
            try
            {
                var Description = reportAnswers.ObjViewModel.ReportAnswerModel.ReportDescription.ToString();
                reportAnswers.ObjViewModel.ReportAnswerModel.SavedQueries.ForEach(query =>
                {
                    reportAnswers.ObjViewModel.ReportAnswerModel.SelectedOption.ReportDescription = Description;
                    reportAnswers.ObjViewModel.ReportAnswerModel.SelectedOption.SubOption = new ReportSubOption { Title = reportAnswers.ObjViewModel.ReportAnswerModel?.SelectedSubOption?.Title, Description = reportAnswers.ObjViewModel.ReportAnswerModel?.SelectedSubOption?.Description, HaveSubOption = reportAnswers.ObjViewModel.ReportAnswerModel.EnableSubOption };
                    query.CustomFilters = JsonConvert.SerializeObject(reportAnswers.ObjViewModel.ReportAnswerModel.SelectedOption);
                });
            }
            catch { }
            reportAnswers.MainGrid.DataContext = reportAnswers.ObjViewModel;
            TabSwitcher.ChangeTabIndex(3, 1);
        }
    }
}