using System;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDModels;
using TwtDominatorUI.TDViews.GrowFollowers;

namespace TwtDominatorUI.Utility.GrowFollowersReportPack.UnfollowerPack
{
    public class UnfollowerViewCampaign : ITDViewCampaign
    {
        public void ManageCampaign(TemplateModel templateDetails, CampaignDetails campaignDetails,
            bool isEditCampaignName,
            Visibility cancelEditVisibility, string campaignButtonContent, string templateId)
        {
            var objUnfollower = Unfollower.GetSingletonObjectUnfollower();

            if (campaignButtonContent == ConstantVariable.CreateCampaign())
                objUnfollower.CampaignName =
                    $"{SocialNetworks.Twitter} {campaignDetails.SubModule} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";


            objUnfollower.IsEditCampaignName = isEditCampaignName;
            objUnfollower.CancelEditVisibility = cancelEditVisibility;
            objUnfollower.TemplateId = templateId;

            objUnfollower.CampaignName = campaignButtonContent == ConstantVariable.UpdateCampaign()
                ? campaignDetails.CampaignName
                : objUnfollower.CampaignName;


            objUnfollower.CampaignButtonContent = campaignButtonContent;
            objUnfollower.SelectedAccountCount = campaignDetails.SelectedAccountList.Count +
                                                 $" {"LangKeyAccountSelected".FromResourceDictionary()}";
            objUnfollower.UnFollowFooter.list_SelectedAccounts = campaignDetails.SelectedAccountList;

            // Doubt FollowerModel FollowBack
            objUnfollower.ObjViewModel.UnfollowerModel =
                templateDetails.ActivitySettings.GetActivityModel<UnfollowerModel>(objUnfollower.ObjViewModel.Model,
                    true);

            objUnfollower.MainGrid.DataContext = objUnfollower.ObjViewModel;

            TabSwitcher.ChangeTabIndex(1, 3);
        }
    }
}