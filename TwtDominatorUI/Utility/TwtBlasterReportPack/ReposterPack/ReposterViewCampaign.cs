using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDModels;
using TwtDominatorUI.TDViews.TwtBlaster;

namespace TwtDominatorUI.Utility.TwtBlasterReportPack.ReposterPack
{
    public class ReposterViewCampaign : ITDViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objReposter = Reposter.GetSingletonObjectReposter();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objReposter.CampaignName =
                    $"{SocialNetworks.Twitter} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";


            objReposter.IsEditCampaignName = isEditCampaignName;
            objReposter.CancelEditVisibility = cancelEditVisibility;
            objReposter.TemplateId = templateId;


            objReposter.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objReposter.CampaignName;


            objReposter.CampaignButtonContent = campaignButtonContent;
            objReposter.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objReposter.ReposterFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            // Doubt FollowerModel FollowBack
            objReposter.ObjViewModel.ReposterModel =
                templateDetails.ActivitySettings.GetActivityModel<ReposterModel>(objReposter.ObjViewModel.Model);

            objReposter.MainGrid.DataContext = objReposter.ObjViewModel;

            TabSwitcher.ChangeTabIndex(2, 0);
        }
    }
}