using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDModels;
using TwtDominatorUI.TDViews.GrowFollowers;

namespace TwtDominatorUI.Utility.GrowFollowersReportPack.FollowBackPack
{
    public class FollowBackViewCampaign : ITDViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objFollowBack = FollowBack.GetSingletonObjectFollowBack();
            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objFollowBack.CampaignName =
                    $"{SocialNetworks.Twitter} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            objFollowBack.IsEditCampaignName = isEditCampaignName;
            objFollowBack.CancelEditVisibility = cancelEditVisibility;
            objFollowBack.TemplateId = templateId;

            objFollowBack.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objFollowBack.CampaignName;

            objFollowBack.CampaignButtonContent = campaignButtonContent;
            objFollowBack.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                 $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objFollowBack.FollowBackFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            objFollowBack.ObjViewModel.FollowerModel =
                templateDetails.ActivitySettings.GetActivityModel<FollowerModel>(objFollowBack.ObjViewModel.Model);

            objFollowBack.MainGrid.DataContext = objFollowBack.ObjViewModel;

            TabSwitcher.ChangeTabIndex(1, 1);
        }
    }
}