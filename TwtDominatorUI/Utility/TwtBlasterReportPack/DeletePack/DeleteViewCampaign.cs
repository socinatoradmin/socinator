using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDModels;
using TwtDominatorUI.TDViews.TwtBlaster;

namespace TwtDominatorUI.Utility.TwtBlasterReportPack.DeletePack
{
    public class DeleteViewCampaign : ITDViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objDelete = Delete.GetSingletonObjectDelete();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objDelete.CampaignName =
                    $"{SocialNetworks.Twitter} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            objDelete.IsEditCampaignName = isEditCampaignName;
            objDelete.CancelEditVisibility = cancelEditVisibility;
            objDelete.TemplateId = templateId;


            objDelete.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objDelete.CampaignName;

            objDelete.CampaignButtonContent = campaignButtonContent;
            objDelete.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                             $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objDelete.DeleteFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            // Doubt FollowerModel FollowBack
            objDelete.ObjViewModel.DeleteModel =
                templateDetails.ActivitySettings.GetActivityModel<DeleteModel>(objDelete.ObjViewModel.Model, true);

            objDelete.MainGrid.DataContext = objDelete.ObjViewModel;

            TabSwitcher.ChangeTabIndex(2, 4);
        }
    }
}